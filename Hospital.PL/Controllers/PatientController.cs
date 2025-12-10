using Hospital.BBL.DTOs.PatientDTOs;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Models.PatientModule;
using Hospital.DAL.Models.Shared;
using Hospital.DAL.Repositories.Classes;
using Hospital.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital.DAL.Contexts;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Hospital.PL.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly ApplicationDbContext _context;
        private const string DefaultPatientPassword = "Patient@123";

        public PatientController(
            IPatientService patientService,
            ILogger<PatientController> logger,
            IWebHostEnvironment environment,
            UserManager<ApplicationUser> userManager,
            IDoctorService doctorService,
            IAppointmentService appointmentService,
            ApplicationDbContext context)
        {
            _patientService = patientService;
            _logger = logger;
            _environment = environment;
            _userManager = userManager;
            _doctorService = doctorService;
            _appointmentService = appointmentService;
            _context = context;
        }

        #region Index
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allPatients = _patientService.GetAllPatients(true);
            IEnumerable<GetAllPatientsDto> patients = allPatients;

            // If user is a Doctor (not Admin), filter to show only patients with appointments
            if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var doctors = _doctorService.GetAllDoctors(true);
                    var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

                    if (doctor != null)
                    {
                        // Get all appointments for this doctor
                        var appointments = _appointmentService.GetAllAppointments(true)
                            .Where(a => a.DoctorId == doctor.Id && a.PatientId.HasValue);

                        // Get unique patient IDs from appointments
                        var patientIds = appointments
                            .Select(a => a.PatientId.Value)
                            .Distinct()
                            .ToList();

                        // Filter patients to only those with appointments
                        patients = allPatients.Where(p => patientIds.Contains(p.Id));
                    }
                    else
                    {
                        // Doctor profile not found, show no patients
                        patients = Enumerable.Empty<GetAllPatientsDto>();
                    }
                }
            }

            // Doctors can view but not modify
            ViewBag.CanModify = User.IsInRole("Admin");

            // Check if current user is a Patient and has a profile
            int? currentUserPatientId = null;
            if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var existingPatient = allPatients.FirstOrDefault(p => p.Email == user.Email);
                    ViewBag.HasProfile = existingPatient != null;
                    if (existingPatient != null)
                    {
                        currentUserPatientId = existingPatient.Id;
                    }
                }
            }

            ViewBag.CurrentUserPatientId = currentUserPatientId;

            return View(patients);
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var patients = _patientService.GetAllPatients(true);
            var existingPatient = patients.FirstOrDefault(p => p.Email == user.Email);

            if (existingPatient != null)
            {
                return RedirectToAction(nameof(Details), new { id = existingPatient.Id });
            }

            return RedirectToAction(nameof(Create));
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
                    TempData["Created"] = "Patient Created successfully";

                    return RedirectToAction(nameof(Index), "Home");
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
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var patient = _patientService.GetPatientById(id.Value);
            if (patient is null) return NotFound();

            // Check if current user is viewing their own profile
            bool isOwnProfile = false;
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Patient") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && patient.Email == user.Email)
                {
                    isOwnProfile = true;
                }
            }

            // Load medical records for this patient
            var medicalRecordsQuery = _context.MedicalRecords
                .Include(m => m.Doctor)
                .Where(m => m.PatientId == id.Value && !m.IsDeleted);

            // If user is a Doctor (not Admin), restrict to their own records
            if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var doctors = _doctorService.GetAllDoctors(true);
                    var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

                    if (doctor != null)
                    {
                        medicalRecordsQuery = medicalRecordsQuery.Where(m => m.DoctorId == doctor.Id);
                    }
                    else
                    {
                        // If doctor profile not found, they shouldn't see any records
                        medicalRecordsQuery = medicalRecordsQuery.Where(m => false);
                    }
                }
            }

            var medicalRecords = await medicalRecordsQuery
                .OrderByDescending(m => m.RecordDate)
                .ToListAsync();

            ViewBag.IsOwnProfile = isOwnProfile;
            ViewBag.CanEdit = User.IsInRole("Admin") || isOwnProfile;
            ViewBag.MedicalRecords = medicalRecords;

            return View(patient);
        }
        #endregion


        #region Edit
        [HttpGet]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var patient = _patientService.GetPatientById(id.Value);
            if (patient is null) return NotFound();

            // If user is Patient (not Admin), verify they can only edit their own profile
            if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || patient.Email != user.Email)
                {
                    return Forbid(); // Access denied
                }
            }

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

            // Pass flag to view to hide Status field for patients
            ViewBag.IsPatient = User.IsInRole("Patient") && !User.IsInRole("Admin");

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Edit([FromRoute] int id, PatientViewModel viewModel)
        {
            // If user is Patient (not Admin), verify they can only edit their own profile
            if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Forbid();
                }

                var patient = _patientService.GetPatientById(id);
                if (patient == null || patient.Email != user.Email)
                {
                    return Forbid(); // Access denied
                }

                // Force email to match user's email (patients can't change their email)
                viewModel.Email = user.Email;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.IsPatient = User.IsInRole("Patient") && !User.IsInRole("Admin");
                return View(viewModel);
            }

            try
            {
                var existingPatient = _patientService.GetPatientById(id);
                if (existingPatient == null)
                {
                    return NotFound();
                }

                var dto = new UpdatePatientDto()
                {
                    Id = id,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    Gender = viewModel.Gender,
                    BloodType = viewModel.BloodType,
                    // Only Admin can change Status
                    Status = User.IsInRole("Admin") ? viewModel.Status : existingPatient.Status,
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
                {
                    TempData["Edited"] = "Patient Updated successfully";
                    // If patient edited their own profile, redirect to Details instead of Index
                    if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Details", new { id = id });
                    }
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to update patient.");
                ViewBag.IsPatient = User.IsInRole("Patient") && !User.IsInRole("Admin");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    _logger.LogError(ex.Message);

                ViewBag.IsPatient = User.IsInRole("Patient") && !User.IsInRole("Admin");
                return View(viewModel);
            }
        }
        #endregion


        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        async Task<bool> DeleteAspPatient(GetPatientByIdDto patient)//async function to perform te delete from Asp Table
        {
            if (patient == null || string.IsNullOrWhiteSpace(patient.Email))
                return false;

            //remove from AspNetUser table
            var AspUser = await _userManager.FindByEmailAsync(patient.Email);
            if (AspUser != null)
            {
                await _userManager.DeleteAsync(AspUser);
                return true;
            }
            return false;
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return BadRequest();

            try
            {
                GetPatientByIdDto? UserPatient = _patientService.GetPatientById(id);
                if (UserPatient == null)
                {
                    TempData["ErrorMessage"] = "Patient not found.";
                    return RedirectToAction(nameof(Index));
                }

                bool AspDelete = await DeleteAspPatient(UserPatient);
                bool deleted = _patientService.DeletePatient(id);

                if (deleted)
                {
                    TempData["Deleted"] = "Patient Deleted successfully";
                    return RedirectToAction(nameof(Index));
                }

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

            if (viewModel == null)
                return;

            var normalizedEmail = viewModel.Email?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedEmail))
            {
                _logger.LogWarning("Attempted to create identity account with empty email.");
                if (User.IsInRole("Admin"))
                {
                    TempData["GeneratedCredentials"] = $"Patient profile created but could not create login account because email is empty.";
                }
                return;
            }

            // Validate email format
            if (!IsValidEmail(normalizedEmail))
            {
                _logger.LogWarning($"Attempted to create identity account with invalid email format: {normalizedEmail}");
                if (User.IsInRole("Admin"))
                {
                    TempData["GeneratedCredentials"] = $"Patient profile created but could not create login account due to invalid email format: {normalizedEmail}";
                }
                return;
            }

            var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (existingUser == null)
            {
                var passwordToUse = !string.IsNullOrWhiteSpace(viewModel.AccountPassword)
                    ? viewModel.AccountPassword
                    : DefaultPatientPassword;

                // Validate password meets requirements
                var passwordValidator = new PasswordValidator<ApplicationUser>();
                var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, null, passwordToUse);

                if (!passwordValidationResult.Succeeded)
                {
                    _logger.LogWarning($"Invalid password format for user {normalizedEmail}");
                    if (User.IsInRole("Admin"))
                    {
                        TempData["GeneratedCredentials"] = $"Patient profile created but could not create login account for {normalizedEmail} due to invalid password format. Password must contain uppercase, lowercase, number, and special character.";
                    }
                    return;
                }

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
                    var roleResult = await _userManager.AddToRoleAsync(user, "Patient");
                    if (roleResult.Succeeded)
                    {
                        if (User.IsInRole("Admin"))
                        {
                            TempData["GeneratedCredentials"] = $"Patient account created for {normalizedEmail}. Temporary password: {passwordToUse}.";
                        }
                    }
                    else
                    {
                        _logger.LogError($"Failed to add patient role for {normalizedEmail}");
                        if (User.IsInRole("Admin"))
                        {
                            TempData["GeneratedCredentials"] = $"Patient account created for {normalizedEmail} but failed to assign role. Please contact administrator.";
                        }
                    }
                }
                else
                {
                    var errors = string.Join(", ", resultUser.Errors.Select(e => e.Description));
                    _logger.LogError($"Error creating user for patient {normalizedEmail}: {errors}");

                    if (User.IsInRole("Admin"))
                    {
                        TempData["GeneratedCredentials"] = $"Patient profile created but failed to create login for {normalizedEmail}. Error: {errors}";
                    }
                }
            }
            else
            {
                // User exists, ensure they have Patient role
                var isInPatientRole = await _userManager.IsInRoleAsync(existingUser, "Patient");
                if (!isInPatientRole)
                {
                    var roleResult = await _userManager.AddToRoleAsync(existingUser, "Patient");
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation($"Added Patient role to existing user: {normalizedEmail}");
                    }
                    else
                    {
                        _logger.LogError($"Failed to add Patient role to existing user: {normalizedEmail}");
                    }
                }

                if (User.IsInRole("Admin"))
                {
                    TempData["GeneratedCredentials"] = $"Patient profile created. User account already exists for {normalizedEmail}.";
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Simple email validation regex
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}