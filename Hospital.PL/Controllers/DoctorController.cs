using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.DTOs.PatientDTOs;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Models.Shared;
using Hospital.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Hospital.PL.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IDepartmentService _departmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string DefaultDoctorPassword = "Doctor@123";

        public DoctorController(
            IDoctorService doctorService,
            ILogger<DoctorController> logger,
            IWebHostEnvironment environment,
            IDepartmentService departmentService,
            UserManager<ApplicationUser> userManager)
        {
            _doctorService = doctorService;
            _logger = logger;
            _environment = environment;
            _departmentService = departmentService;
            _userManager = userManager;
        }

        #region Index - GetAllDoctors
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var doctors = _doctorService.GetAllDoctors(true);
            // Patients can view but not modify doctors
            ViewBag.CanModify = User.IsInRole("Admin") || (User.IsInRole("Doctor") && !User.IsInRole("Admin"));
            return View(doctors);
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var doctors = _doctorService.GetAllDoctors(true);
            var existingDoctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (existingDoctor != null)
            {
                return RedirectToAction(nameof(Details), new { id = existingDoctor.Id });
            }

            return RedirectToAction(nameof(Create));
        }
        #endregion

        #region Create - AddDoctor
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create()
        {
            // If user is Doctor, check if they already have a profile
            if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var doctors = _doctorService.GetAllDoctors(true);
                    var existingDoctor = doctors.FirstOrDefault(d => d.Email == user.Email);
                    if (existingDoctor != null)
                    {
                        // Redirect to edit their existing profile
                        return RedirectToAction("Edit", new { id = existingDoctor.Id });
                    }
                }
            }

            var Model = new DoctorViewModel()
            {
                Departments = _departmentService.GetAllDepartments(true)
                .Select(D => new SelectListItem { Value = D.Id.ToString(), Text = D.Name })
            };

            // Pre-fill email if user is Doctor
            if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    Model.Email = user.Email;
                    Model.FirstName = user.FirstName;
                    Model.LastName = user.LastName;
                }
            }

            return View(Model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create(DoctorViewModel viewModel)
        {
            // If user is Doctor (not Admin), ensure email matches their account
            if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Force email to match user's email
                    viewModel.Email = user.Email;
                    
                    // Check if profile already exists
                    var doctors = _doctorService.GetAllDoctors(true);
                    var existingDoctor = doctors.FirstOrDefault(d => d.Email == user.Email);
                    if (existingDoctor != null)
                    {
                        ModelState.AddModelError(string.Empty, "You already have a doctor profile. Please edit your existing profile.");
                        viewModel.Departments = _departmentService.GetAllDepartments(true)
                            .Select(D => new SelectListItem { Value = D.Id.ToString(), Text = D.Name });
                        return View(viewModel);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var doctorDto = new AddDoctorDto()
                    {
                        FirstName = viewModel.FirstName,
                        LastName = viewModel.LastName,
                        Specialization = viewModel.Specialization,
                        Bio = viewModel.Bio,
                        PhoneNumber = viewModel.PhoneNumber,
                        Email = viewModel.Email,
                        Education = viewModel.Education,
                        Image = viewModel.Image,
                        DepartmentId = viewModel.DepartmentId
                    };

                    int result = _doctorService.AddDoctor(doctorDto);

                    if (result > 0)
                    {
                        TempData["Created"] = "Doctor Created successfully";
                        if (User.IsInRole("Admin"))
                        {
                            await EnsureDoctorIdentityAccountAsync(viewModel);
                        }

                        if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
                        {
                            // Redirect to dashboard after creating profile
                            return RedirectToAction("Doctor", "Dashboard");
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Cannot create doctor");
                    }
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex.Message);
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(e => e.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                ViewBag.Errors = errors;

                viewModel.Departments = _departmentService.GetAllDepartments(true)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    });
            }

            return View(viewModel);
        }

        #endregion

        #region Details - GetDoctorById
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var doctor = _doctorService.GetDoctorById(id.Value);
            if (doctor is null) return NotFound();

            // Check if current user is viewing their own profile
            bool isOwnProfile = false;
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && doctor.Email == user.Email)
                {
                    isOwnProfile = true;
                }
            }

            ViewBag.IsOwnProfile = isOwnProfile;

            return View(doctor);
        }
        #endregion

        #region Delete - DeleteDoctor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        async Task<bool> DeleteAspDoctor(GetDoctorByIdDto patient)//async function to perform te delete from Asp Table
        {

            //remove from AspNetUser table
            var AspUser = await _userManager.FindByEmailAsync(patient.Email);
            await _userManager.DeleteAsync(AspUser);
            return true;
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return BadRequest();

            try
            {

                GetDoctorByIdDto UserDoctor = _doctorService.GetDoctorById(id);
                bool AspDelete= await DeleteAspDoctor(UserDoctor);

                bool deleted = _doctorService.DeleteDoctor(id);

                if (deleted)
                {
                    TempData["Deleted"] = "Doctor deleted successfully";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to delete doctor.");
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
                    return View("Errorview", ex);
                }
            }
        }
        #endregion

        #region Edit - UpdateDoctor
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Edit(int? id)
        {
            // If user is Doctor (not Admin), get their own profile
            if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var doctors = _doctorService.GetAllDoctors(true);
                    var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);
                    if (doctor != null)
                    {
                        id = doctor.Id; // Use their own ID
                    }
                    else
                    {
                        // No profile exists, redirect to create
                        return RedirectToAction("Create");
                    }
                }
            }

            if (!id.HasValue) return BadRequest();

            var doctorEntity = _doctorService.GetDoctorById(id.Value);
            if (doctorEntity is null) return NotFound();

            // If user is Doctor (not Admin), verify they can only edit their own profile
            if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && doctorEntity.Email != user.Email)
                {
                    return Forbid(); // Access denied
                }
            }

            var viewModel = new DoctorViewModel()
            {
                Id = doctorEntity.Id,
                FirstName = doctorEntity.FirstName,
                LastName = doctorEntity.LastName,
                Specialization = doctorEntity.Specialization,
                Bio = doctorEntity.Bio,
                PhoneNumber = doctorEntity.PhoneNumber,
                Email = doctorEntity.Email,
                ExpertYears = doctorEntity.ExpertYears,
                Education = doctorEntity.Education,
                Status = doctorEntity.Status,
                DepartmentId = doctorEntity.DepartmentId,
            };
            viewModel.Departments = _departmentService.GetAllDepartments(true)
                .Select(D => new SelectListItem { Value = D.Id.ToString(), Text = D.Name, Selected = D.Id == doctorEntity.DepartmentId });
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Edit([FromRoute] int? id, DoctorViewModel viewModel)
        {
            // If user is Doctor (not Admin), get their own profile ID
            if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var doctors = _doctorService.GetAllDoctors(true);
                    var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);
                    if (doctor != null)
                    {
                        id = doctor.Id; // Use their own ID
                        viewModel.Id = doctor.Id;
                        viewModel.Email = user.Email; // Ensure email matches
                    }
                    else
                    {
                        return RedirectToAction("Create");
                    }
                }
            }

            if (!id.HasValue)
                return BadRequest();

            // Verify access if user is Doctor
            if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
            {
                var doctorEntity = _doctorService.GetDoctorById(id.Value);
                if (doctorEntity == null)
                    return NotFound();
                
                var user = await _userManager.GetUserAsync(User);
                if (user != null && doctorEntity.Email != user.Email)
                {
                    return Forbid(); // Access denied
                }
            }

            if (!ModelState.IsValid)
            {
                viewModel.Departments = _departmentService.GetAllDepartments(true)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name,
                        Selected = d.Id == viewModel.DepartmentId
                    });

                return View(viewModel);
            }

            try
            {
                var doctorDto = new UpdateDoctorDto()
                {
                    Id = viewModel.Id,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Specialization = viewModel.Specialization,
                    Bio = viewModel.Bio,
                    PhoneNumber = viewModel.PhoneNumber,
                    Email = viewModel.Email,
                    Education = viewModel.Education,
                    ExpertYears = viewModel.ExpertYears,
                    Status = viewModel.Status,
                    DepartmentId = viewModel.DepartmentId,
                    Image = viewModel.Image
                };
                int result = _doctorService.UpdateDoctor(doctorDto);

                if (result > 0)
                {
                    TempData["Edited"] = "Doctor updated successfully";
                    if (User.IsInRole("Doctor") && !User.IsInRole("Admin"))
                    {
                       
                        // Redirect to dashboard after updating profile
                        return RedirectToAction("Doctor", "Dashboard");
                    }
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to update doctor.");

                viewModel.Departments = _departmentService.GetAllDepartments(true)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name,
                        Selected = d.Id == viewModel.DepartmentId
                    });

                return View(viewModel);
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                else
                {
                    _logger.LogError(ex.Message);
                }

                viewModel.Departments = _departmentService.GetAllDepartments(true)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name,
                        Selected = d.Id == viewModel.DepartmentId
                    });

                return View(viewModel);
            }
        }

        #endregion

        private async Task EnsureDoctorIdentityAccountAsync(DoctorViewModel viewModel)
        {
            var normalizedEmail = viewModel.Email?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                return;

            var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (existingUser == null)
            {
                var passwordToUse = !string.IsNullOrWhiteSpace(viewModel.AccountPassword)
                    ? viewModel.AccountPassword
                    : DefaultDoctorPassword;

                var user = new ApplicationUser
                {
                    UserName = normalizedEmail,
                    Email = normalizedEmail,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user, passwordToUse);
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Doctor");
                    TempData["GeneratedCredentials"] = $"Doctor account created for {normalizedEmail}. Temporary password: {passwordToUse}.";
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        _logger.LogError($"Error creating doctor identity for {normalizedEmail}: {error.Description}");
                    }
                    TempData["GeneratedCredentials"] = $"Doctor profile created but failed to create login for {normalizedEmail}. Check logs for details.";
                }
            }
            else
            {
                if (!await _userManager.IsInRoleAsync(existingUser, "Doctor"))
                {
                    await _userManager.AddToRoleAsync(existingUser, "Doctor");
                }
            }
        }
    }
}
