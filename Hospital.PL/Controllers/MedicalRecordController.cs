using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Contexts;
using Hospital.DAL.Models.MedicalRecordModule;
using Hospital.DAL.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital.PL.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class MedicalRecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public MedicalRecordController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IDoctorService doctorService,
            IPatientService patientService)
        {
            _context = context;
            _userManager = userManager;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? patientId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Get doctor by email
            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile not found.";
                return RedirectToAction("Doctor", "Dashboard");
            }

            var query = _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Where(m => m.DoctorId == doctor.Id && !m.IsDeleted);

            if (patientId.HasValue)
            {
                query = query.Where(m => m.PatientId == patientId.Value);
            }

            var records = await query.OrderByDescending(m => m.RecordDate).ToListAsync();

            ViewBag.Doctor = doctor;
            ViewBag.Patients = _patientService.GetAllPatients(true)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            ViewBag.SelectedPatientId = patientId;

            return View(records);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? patientId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile not found.";
                return RedirectToAction("Doctor", "Dashboard");
            }

            ViewBag.Doctor = doctor;
            ViewBag.Patients = _patientService.GetAllPatients(true)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            ViewBag.SelectedPatientId = patientId;

            return View(new MedicalRecord());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecord model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile not found.";
                return RedirectToAction("Doctor", "Dashboard");
            }

            // Set required fields that aren't in the form
            model.DoctorId = doctor.Id;
            model.RecordDate = DateTime.Now;
            model.IsDeleted = false;
            model.Notes = model.Notes ?? string.Empty;

            // Remove validation for properties we set manually or don't need from form
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
            ViewBag.Patients = _patientService.GetAllPatients(true)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            ViewBag.SelectedPatientId = model?.PatientId;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
                return NotFound();

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

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
                return NotFound();

            var record = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctor.Id && !m.IsDeleted);

            if (record == null)
                return NotFound();

            ViewBag.Patients = _patientService.GetAllPatients(true)
                .Select(p => new { p.Id, p.Name })
                .ToList();

            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalRecord model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
                return NotFound();

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

            ViewBag.Patients = _patientService.GetAllPatients(true)
                .Select(p => new { p.Id, p.Name })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
                return NotFound();

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

