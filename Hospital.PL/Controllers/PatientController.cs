using Hospital.BBL.DTOs.PatientDTOs;
using Hospital.BBL.Services.Interfaces;
using Hospital.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Hospital.DAL.Models.Shared;

namespace Hospital.PL.Controllers
{
    [Authorize]
    public class PatientController(IPatientService patientService,ILogger<PatientController> logger,IWebHostEnvironment environment, UserManager<ApplicationUser> userManager): Controller
    {
        private readonly IPatientService _patientService = patientService;
        private readonly ILogger<PatientController> _logger = logger;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private const string DefaultPatientPassword = "Patient@123";

        #region Index
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var patients = _patientService.GetAllPatients(true);
            // Doctors can view but not modify
            ViewBag.CanModify = User.IsInRole("Admin");

            // Check if current user is a Patient and has a profile
            if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var existingPatient = patients.FirstOrDefault(p => p.Email == user.Email);
                    ViewBag.HasProfile = existingPatient != null;
                }
            }

            return View(patients);
        }
        #endregion


        #region Create
        [HttpGet]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Create()
        {
            // If user is Patient, check if they already have a profile
            if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var patients = _patientService.GetAllPatients(true);
                    var existingPatient = patients.FirstOrDefault(p => p.Email == user.Email);
                    if (existingPatient != null)
                    {
                        // Redirect to edit their existing profile (or details if they can't edit)
                        // For now, let's redirect to Details as patients might not have edit rights yet, 
                        // or if they do, we can redirect to Edit. 
                        // Based on requirements, "give him button to create profile", implying if they have one they don't need to create.
                        return RedirectToAction("Details", new { id = existingPatient.Id });
                    }
                }
            }

            var model = new PatientViewModel();

            // Pre-fill email if user is Patient
            if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    model.Email = user.Email;
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Create(PatientViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var dto = new AddPatientDto()
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    Gender = viewModel.Gender,
                    BloodType = viewModel.BloodType,
                    Height = viewModel.Height,
                    Weight = viewModel.Weight,
                    DateOfBirth = viewModel.DateOfBirth,
                    Address = viewModel.Address,
                    Allergies = viewModel.Allergies,
                    MedicalHistory = viewModel.MedicalHistory,
                    CurrentMedications = viewModel.CurrentMedications,
                    Image = viewModel.Image
                };

                // If user is Patient (not Admin), ensure email matches their account
                if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        // Force email to match user's email
                        dto.Email = user.Email;
                        viewModel.Email = user.Email; // Update viewmodel in case we return view

                        // Check if profile already exists
                        var patients = _patientService.GetAllPatients(true);
                        var existingPatient = patients.FirstOrDefault(p => p.Email == user.Email);
                        if (existingPatient != null)
                        {
                            ModelState.AddModelError(string.Empty, "You already have a patient profile.");
                            return View(viewModel);
                        }
                    }
                }

                int result = _patientService.AddPatient(dto);

                if (result > 0)
                {
                    await EnsurePatientIdentityAccountAsync(viewModel);

                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to add patient.");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    _logger.LogError(ex.Message);

                return View(viewModel);
            }
        }
        #endregion


        #region Details
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var patient = _patientService.GetPatientById(id.Value);
            if (patient is null) return NotFound();

            return View(patient);
        }
        #endregion


        #region Edit
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var patient = _patientService.GetPatientById(id.Value);
            if (patient is null) return NotFound();

            var vm = new PatientViewModel
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Gender = patient.Gender,
                BloodType = patient.BloodType,
                Status = patient.Status,
                Height = patient.Height,
                Weight = patient.Weight,
                DateOfBirth = patient.DateOfBirth,
                Address = patient.Address,
                Allergies = patient.Allergies,
                MedicalHistory = patient.MedicalHistory,
                CurrentMedications = patient.CurrentMedications
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit([FromRoute] int id, PatientViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var dto = new UpdatePatientDto()
                {
                    Id = id,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    Gender = viewModel.Gender,
                    BloodType = viewModel.BloodType,
                    Status = viewModel.Status,
                    Height = viewModel.Height,
                    Weight = viewModel.Weight,
                    DateOfBirth = viewModel.DateOfBirth,
                    Address = viewModel.Address,
                    Allergies = viewModel.Allergies,
                    MedicalHistory = viewModel.MedicalHistory,
                    CurrentMedications = viewModel.CurrentMedications,
                    Image = viewModel.Image
                };

                int result = _patientService.UpdatePatient(dto);

                if (result > 0)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "Failed to update patient.");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    _logger.LogError(ex.Message);

                return View(viewModel);
            }
        }
        #endregion


        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (id == 0) return BadRequest();

            try
            {
                bool deleted = _patientService.DeletePatient(id);

                if (deleted)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "Failed to delete patient.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError(ex.Message);
                    return View("ErrorView", ex);
                }
            }
        }
        #endregion
        private async Task EnsurePatientIdentityAccountAsync(PatientViewModel viewModel)
        {
            // Patients managing their own profile already have an identity account
            if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
                return;

            var normalizedEmail = viewModel.Email?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                return;

            var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (existingUser == null)
            {
                var passwordToUse = !string.IsNullOrWhiteSpace(viewModel.AccountPassword)
                    ? viewModel.AccountPassword
                    : DefaultPatientPassword;

                var user = new ApplicationUser
                {
                    UserName = normalizedEmail,
                    Email = normalizedEmail,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    EmailConfirmed = true
                };

                var resultUser = await _userManager.CreateAsync(user, passwordToUse);
                if (resultUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Patient");
                    if (User.IsInRole("Admin"))
                    {
                        TempData["GeneratedCredentials"] = $"Patient account created for {normalizedEmail}. Temporary password: {passwordToUse}.";
                    }
                }
                else
                {
                    foreach (var error in resultUser.Errors)
                    {
                        _logger.LogError($"Error creating user for patient {normalizedEmail}: {error.Description}");
                    }

                    if (User.IsInRole("Admin"))
                    {
                        TempData["GeneratedCredentials"] = $"Patient profile created but failed to create login for {normalizedEmail}. Check logs for details.";
                    }
                }
            }
            else
            {
                if (!await _userManager.IsInRoleAsync(existingUser, "Patient"))
                {
                    await _userManager.AddToRoleAsync(existingUser, "Patient");
                }
            }
        }
    }

}
