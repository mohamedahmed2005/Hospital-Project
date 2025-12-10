using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Contexts;
using Hospital.DAL.Models.MedicalRecordModule;
using Hospital.DAL.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.DTOs.PatientDTOs;

namespace Hospital.PL.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class MedicalRecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;

        public MedicalRecordController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IDoctorService doctorService,
            IPatientService patientService,
            IAppointmentService appointmentService)
        {
            _context = context;
            _userManager = userManager;
            _doctorService = doctorService;
            _patientService = patientService;
            _appointmentService = appointmentService;
        }

        // Helper method to get current doctor by email
        private async Task<GetAllDoctorsDto?> GetCurrentDoctorAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            // Get doctor by Email (your current approach)
            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d =>
                !string.IsNullOrEmpty(d.Email) &&
                d.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

            return doctor;
        }

        // Helper method to check doctor profile
        private async Task<IActionResult> CheckDoctorProfileAsync()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null)
            {
                TempData["Error"] = "Please complete your doctor profile before accessing medical records.";
                return RedirectToAction("Create", "DoctorProfiles");
            }

            return null; // No error, doctor exists
        }

        private IEnumerable<GetAllPatientsDto> GetPatientsForDoctor(int doctorId)
        {
            // Get all appointments for this doctor
            var appointments = _appointmentService.GetAllAppointments(true)
                .Where(a => a.DoctorId == doctorId && a.PatientId.HasValue);

            // Get unique patient IDs from appointments
            var patientIds = appointments
                .Select(a => a.PatientId.Value)
                .Distinct()
                .ToList();

            // Return only patients with appointments
            return _patientService.GetAllPatients(true)
                .Where(p => patientIds.Contains(p.Id));
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? patientId)
        {
            // Check if doctor profile exists
            var profileCheck = await CheckDoctorProfileAsync();
            if (profileCheck != null) return profileCheck;

            var doctor = await GetCurrentDoctorAsync();

            var query = _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Where(m => m.DoctorId == doctor.Id && !m.IsDeleted);

           /* if (patientId.HasValue)
            {
                query = query.Where(m => m.PatientId == patientId.Value);
            } it limit data after editing-creating to show one patient only--hassan*/

            var records = await query.OrderByDescending(m => m.RecordDate).ToListAsync();

            ViewBag.Doctor = doctor;
            // Only show patients that have appointments with this doctor
            ViewBag.Patients = GetPatientsForDoctor(doctor.Id)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            ViewBag.SelectedPatientId = patientId;
            ViewBag.HasDoctorProfile = true; // This tells the view the doctor profile exists
               
            return View(records);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? patientId)
        {
            // Check if doctor profile exists
            var profileCheck = await CheckDoctorProfileAsync();
            if (profileCheck != null) return profileCheck;

            var doctor = await GetCurrentDoctorAsync();

            ViewBag.Doctor = doctor;
            // Only show patients that have appointments with this doctor
            ViewBag.Patients = GetPatientsForDoctor(doctor.Id)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            ViewBag.SelectedPatientId = patientId;
            ViewBag.HasDoctorProfile = true; // This tells the view the doctor profile exists

            var model = new MedicalRecord
            {
                PatientId = patientId ?? 0,
                DoctorId = doctor.Id,
                RecordDate = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecord model)
        {
            // Check if doctor profile exists
            var profileCheck = await CheckDoctorProfileAsync();
            if (profileCheck != null) return profileCheck;

            var doctor = await GetCurrentDoctorAsync();

            // Set required fields
            model.DoctorId = doctor.Id;
            model.RecordDate = DateTime.Now;
            model.IsDeleted = false;
            model.Notes = model.Notes ?? string.Empty;

            // Remove validation for properties we set manually
            ModelState.Remove("Doctor");
            ModelState.Remove("Patient");
            ModelState.Remove("DoctorId");

            // Manually validate required fields
            if (model.PatientId <= 0)
            {
                ModelState.AddModelError("PatientId", "Please select a patient.");
            }
            if (string.IsNullOrWhiteSpace(model.Diagnosis))
            {
                ModelState.AddModelError("Diagnosis", "Diagnosis is required.");
            }
            if (string.IsNullOrWhiteSpace(model.Treatment))
            {
                ModelState.AddModelError("Treatment", "Treatment is required.");
            }
            if (string.IsNullOrWhiteSpace(model.Prescription))
            {
                ModelState.AddModelError("Prescription", "Prescription is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.MedicalRecords.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Medical record created successfully!";
                    return RedirectToAction("Index", new { patientId = model.PatientId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the medical record. Please try again.");
                    TempData["Error"] = "Failed to create medical record: " + ex.Message;
                }
            }

            ViewBag.Doctor = doctor;
            // Only show patients that have appointments with this doctor
            ViewBag.Patients = GetPatientsForDoctor(doctor.Id)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            ViewBag.SelectedPatientId = model?.PatientId;
            ViewBag.HasDoctorProfile = true; // This tells the view the doctor profile exists

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            // Check if doctor profile exists
            var profileCheck = await CheckDoctorProfileAsync();
            if (profileCheck != null) return profileCheck;

            var doctor = await GetCurrentDoctorAsync();

            var record = await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctor.Id && !m.IsDeleted);

            if (record == null)
                return NotFound();

            return View(record);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();

            // Check if doctor profile exists
            var profileCheck = await CheckDoctorProfileAsync();
            if (profileCheck != null) return profileCheck;

            var doctor = await GetCurrentDoctorAsync();

            var record = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctor.Id && !m.IsDeleted);

            if (record == null)
                return NotFound();

            // Only show patients that have appointments with this doctor
            ViewBag.Patients = GetPatientsForDoctor(doctor.Id)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            ViewBag.HasDoctorProfile = true; // This tells the view the doctor profile exists

            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalRecord model)
        {
            // Check if doctor profile exists
            var profileCheck = await CheckDoctorProfileAsync();
            if (profileCheck != null) return profileCheck;

            var doctor = await GetCurrentDoctorAsync();

            var record = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctor.Id && !m.IsDeleted);

            if (record == null)
                return NotFound();

            // Remove validation for properties we don't need from form
            ModelState.Remove("Doctor");
            ModelState.Remove("Patient");
            ModelState.Remove("DoctorId");

            if (ModelState.IsValid)
            {
                record.Diagnosis = model.Diagnosis;
                record.Treatment = model.Treatment;
                record.Prescription = model.Prescription;
                record.Notes = model.Notes ?? string.Empty;
                record.PatientId = model.PatientId;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Medical record updated successfully!";
                return RedirectToAction("Index", new { patientId = model.PatientId });
            }

            // Only show patients that have appointments with this doctor
            ViewBag.Patients = GetPatientsForDoctor(doctor.Id)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            ViewBag.HasDoctorProfile = true; // This tells the view the doctor profile exists

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Check if doctor profile exists
            var profileCheck = await CheckDoctorProfileAsync();
            if (profileCheck != null) return profileCheck;

            var doctor = await GetCurrentDoctorAsync();

            var record = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctor.Id && !m.IsDeleted);

            if (record == null)
                return NotFound();

            record.IsDeleted = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Medical record deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}