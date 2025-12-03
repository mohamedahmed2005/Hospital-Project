using Hospital.BBL.DTOs.AppointmentDTOs;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Contexts;
using Hospital.DAL.Models.AppointmentModule;
using Hospital.DAL.Models.Shared;
using Hospital.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hospital.PL.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPatientService _patientService;

        public AppointmentController(
            IAppointmentService appointmentService,
            ILogger<AppointmentController> logger,
            IWebHostEnvironment environment,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IPatientService patientService)
        {
            _appointmentService = appointmentService;
            _logger = logger;
            _environment = environment;
            _context = context;
            _userManager = userManager;
            _patientService = patientService;
        }

        private void PopulateDropdowns(int? selectedPatientId = null, int? selectedDoctorId = null)
        {
            var patients = _context.Patients.AsNoTracking().ToList();
            var doctors = _context.Doctors.AsNoTracking().ToList();

            ViewBag.Patients = new SelectList(patients, "Id", "Name", selectedPatientId);
            ViewBag.Doctors = new SelectList(doctors, "Id", "Name", selectedDoctorId);
        }


        #region Index - GetAllAppointments
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var appointments = _appointmentService.GetAllAppointments(true);
            
            // If user is Patient, filter to show only their appointments
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && User.IsInRole("Patient"))
                {
                    var patients = _patientService.GetAllPatients(true);
                    var patient = patients.FirstOrDefault(p => p.Email == user.Email);
                    if (patient != null)
                    {
                        appointments = appointments.Where(a => a.PatientId == patient.Id);
                    }
                }
            }
            
            return View(appointments);
        }
        #endregion

        #region Create - AddAppointment
        [HttpGet]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            int? patientId = null;
            string? patientName = null;

            // If user is Patient, auto-select their patient record
            if (User.IsInRole("Patient") && user != null)
            {
                var patients = _patientService.GetAllPatients(true);
                // Use case-insensitive comparison
                var patient = patients.FirstOrDefault(p => string.Equals(p.Email, user.Email, StringComparison.OrdinalIgnoreCase));
                
                if (patient != null)
                {
                    patientId = patient.Id;
                    patientName = patient.Name;
                    
                    // Restrict the dropdown to only this patient
                    ViewBag.Patients = new SelectList(new[] { patient }, "Id", "Name", patientId);
                    ViewBag.Doctors = new SelectList(_context.Doctors.AsNoTracking().ToList(), "Id", "Name");
                }
                else
                {
                     // Patient profile not found for this user
                     return RedirectToAction("Create", "Patient");
                }
            }
            else
            {
                PopulateDropdowns(patientId, null);
            }

            ViewBag.PatientId = patientId;
            ViewBag.PatientName = patientName;
            ViewBag.IsPatient = User.IsInRole("Patient");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> Create(AppointementViewModel viewModel)
        {
            // If user is Patient, ensure they can only create appointments for themselves
            if (User.IsInRole("Patient"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var patients = _patientService.GetAllPatients(true);
                    var patient = patients.FirstOrDefault(p => p.Email == user.Email);
                    if (patient != null)
                    {
                        viewModel.PatientId = patient.Id; // Force patient ID
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Patient profile not found. Please contact administrator.");
                        PopulateDropdowns(null, viewModel.DoctorId);
                        return View(viewModel);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var appointmentDto = new AddAppointmentDto()
                {
                    Appointment_Date = viewModel.Appointment_Date,
                    Appointment_Time = viewModel.Appointment_Time,
                    AppointmentType = viewModel.AppointmentType,
                    Status = AppointmentStatus.pending, // Default status for new appointments
                    IsAvailable = true,
                    Notes = viewModel.Notes,
                    Image = viewModel.Image,
                    PatientId = viewModel.PatientId,
                    DoctorId = viewModel.DoctorId
                };

                try
                {
                    int result = _appointmentService.AddAppointment(appointmentDto);
                    if (result > 0)
                    {
                        TempData["Success"] = "Appointment created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                        ModelState.AddModelError(string.Empty, "Failed to add appointment.");
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex.Message);
                }
            }

            PopulateDropdowns(viewModel.PatientId, viewModel.DoctorId);

            return View(viewModel);
        }

        #endregion

        #region Details - GetAppointmentById
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var appointment = _appointmentService.GetAppointmentById(id.Value);
            if (appointment is null) return NotFound();

            return View(appointment);
        }
        #endregion

        #region Delete - DeleteAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return BadRequest();

            var appointment = _appointmentService.GetAppointmentById(id);
            if (appointment is null) return NotFound();

            // If user is Patient, ensure they can only delete their own appointments
            if (User.IsInRole("Patient"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var patients = _patientService.GetAllPatients(true);
                    var patient = patients.FirstOrDefault(p => p.Email == user.Email);
                    if (patient == null || appointment.PatientId != patient.Id)
                    {
                        TempData["AppointmentError"] = "You can only delete your own appointments.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                TempData["AppointmentError"] = "Completed appointments cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                bool deleted = _appointmentService.DeleteAppointment(id);
                if (deleted)
                    return RedirectToAction(nameof(Index));
                else
                {
                    TempData["AppointmentError"] = "Failed to delete appointment.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    TempData["AppointmentError"] = ex.Message;
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

        #region Edit - UpdateAppointment
        [HttpGet]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var appointment = _appointmentService.GetAppointmentById(id.Value);
            if (appointment is null) return NotFound();

            // If user is Patient, ensure they can only edit their own appointments
            if (User.IsInRole("Patient"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var patients = _patientService.GetAllPatients(true);
                    var patient = patients.FirstOrDefault(p => p.Email == user.Email);
                    if (patient == null || appointment.PatientId != patient.Id)
                    {
                        TempData["AppointmentError"] = "You can only edit your own appointments.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            var appointmentEditViewModel = new AppointementViewModel()
            {
                Id = appointment.Id,
                Appointment_Date = appointment.Appointment_Date,
                Appointment_Time = appointment.Appointment_Time,
                AppointmentType = appointment.AppointmentType,
                Status = appointment.Status,
                Notes = appointment.Notes,
                PatientId = appointment.PatientId ?? 0,
                DoctorId = appointment.DoctorId ?? 0
            };

            PopulateDropdowns(appointmentEditViewModel.PatientId, appointmentEditViewModel.DoctorId);

            return View(appointmentEditViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Edit([FromRoute] int id, AppointementViewModel viewModel)
        {
            // If user is Patient, ensure they can only edit their own appointments
            if (User.IsInRole("Patient"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var patients = _patientService.GetAllPatients(true);
                    var patient = patients.FirstOrDefault(p => p.Email == user.Email);
                    if (patient == null)
                    {
                        TempData["AppointmentError"] = "Patient profile not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    // Verify the appointment belongs to this patient
                    var appointment = _appointmentService.GetAppointmentById(id);
                    if (appointment == null || appointment.PatientId != patient.Id)
                    {
                        TempData["AppointmentError"] = "You can only edit your own appointments.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    // Force patient ID to match logged-in patient
                    viewModel.PatientId = patient.Id;
                }
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(viewModel.PatientId, viewModel.DoctorId);
                return View(viewModel);
            }

            try
            {
                var appointmentDto = new UpdateAppointmentDto()
                {
                    Id = viewModel.Id,
                    Appointment_Date = viewModel.Appointment_Date,
                    Appointment_Time = viewModel.Appointment_Time,
                    AppointmentType = viewModel.AppointmentType,
                    Status = viewModel.Status,
                    Notes = viewModel.Notes,
                    PatientId = viewModel.PatientId,
                    DoctorId = viewModel.DoctorId
                };

                int result = _appointmentService.UpdateAppointment(appointmentDto);

                if (result > 0)
                {
                    TempData["Success"] = "Appointment updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update appointment.");
                    PopulateDropdowns(viewModel.PatientId, viewModel.DoctorId);
                    return View(viewModel);
                }
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

                PopulateDropdowns(viewModel.PatientId, viewModel.DoctorId);
                return View(viewModel);
            }
        }
        #endregion
    }

}
