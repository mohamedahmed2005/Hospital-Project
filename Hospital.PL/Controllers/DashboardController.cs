using Hospital.BBL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Hospital.DAL.Models.Shared;
using Hospital.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hospital.PL.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IDepartmentService _departmentService;
        private readonly IAppointmentService _appointmentService;
        private readonly ApplicationDbContext _context;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IPatientService patientService,
            IDoctorService doctorService,
            IDepartmentService departmentService,
            IAppointmentService appointmentService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _patientService = patientService;
            _doctorService = doctorService;
            _departmentService = departmentService;
            _appointmentService = appointmentService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return RedirectToAction("Admin");
            else if (roles.Contains("Doctor"))
                return RedirectToAction("Doctor");
            else if (roles.Contains("Patient"))
                return RedirectToAction("Patient");
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            var patients = _patientService.GetAllPatients(true);
            var doctors = _doctorService.GetAllDoctors(true);
            var departments = _departmentService.GetAllDepartments(true);
            var appointments = _appointmentService.GetAllAppointments(true);

            ViewBag.PatientCount = patients.Count();
            ViewBag.DoctorCount = doctors.Count();
            ViewBag.DepartmentCount = departments.Count();
            ViewBag.AppointmentCount = appointments.Count();
            ViewBag.RecentPatients = patients.Take(5);
            ViewBag.RecentDoctors = doctors.Take(5);
            ViewBag.RecentAppointments = appointments.Take(5);

            // Analytics: appointments over the last 7 days
            var today = DateOnly.FromDateTime(DateTime.Now);
            var last7Days = Enumerable.Range(0, 7)
                .Select(offset => today.AddDays(-6 + offset))
                .ToList();

            var dailyAppointments = last7Days
                .Select(d => new
                {
                    Date = d,
                    Label = d.ToString("MMM dd"),
                    Count = appointments.Count(a => a.Appointment_Date == d)
                })
                .ToList();

            // Analytics: appointments by status
            var statusGroups = Enum.GetValues(typeof(Hospital.DAL.Models.AppointmentModule.AppointmentStatus))
                .Cast<Hospital.DAL.Models.AppointmentModule.AppointmentStatus>()
                .Select(status => new
                {
                    Status = status.ToString(),
                    Count = appointments.Count(a => a.Status == status)
                })
                .ToList();

            ViewBag.DailyAppointments = dailyAppointments;
            ViewBag.AppointmentStatusDistribution = statusGroups;

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Patient()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var appointments = _appointmentService.GetAllAppointments(true);
            var patients = _patientService.GetAllPatients(true);
            var patient = patients.FirstOrDefault(p => p.Email == user.Email);

            if (patient == null)
            {
                return RedirectToAction("Create", "Patient");
            }
            
            var userAppointments = new List<Hospital.BBL.DTOs.AppointmentDTOs.GetAllAppointmentsDto>();
            if (patient != null)
            {
                userAppointments = appointments.Where(a => 
                    a.PatientName == patient.Name || 
                    a.PatientName == $"{patient.FirstName} {patient.LastName}" ||
                    (!string.IsNullOrEmpty(a.PatientName) && a.PatientName.Contains(patient.FirstName) && a.PatientName.Contains(patient.LastName))
                ).ToList();
            }

            ViewBag.AppointmentCount = userAppointments.Count;
            var today = DateOnly.FromDateTime(DateTime.Now);
            ViewBag.UpcomingAppointments = userAppointments.Where(a => a.Appointment_Date >= today).OrderBy(a => a.Appointment_Date).Take(5);
            ViewBag.PastAppointments = userAppointments.Where(a => a.Appointment_Date < today).OrderByDescending(a => a.Appointment_Date).Take(5);

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Doctor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
            {
                ViewBag.Message = "Doctor profile not found. Please contact administrator.";
                return View();
            }

            var appointments = _appointmentService.GetAllAppointments(true);
            var doctorAppointments = appointments.Where(a => a.DoctorName == doctor.Name || a.DoctorName == $"{doctor.FirstName} {doctor.LastName}").ToList();

            // Get medical records count
            var medicalRecordsCount = await _context.MedicalRecords
                .CountAsync(m => m.DoctorId == doctor.Id && !m.IsDeleted);
            
            // Test if medical records functionality works
            bool medicalRecordsWorking = true;
            try
            {
                var testRecord = await _context.MedicalRecords
                    .FirstOrDefaultAsync(m => m.DoctorId == doctor.Id && !m.IsDeleted);
                // If we can query, it's working
            }
            catch
            {
                medicalRecordsWorking = false;
            }

            ViewBag.Doctor = doctor;
            ViewBag.AppointmentCount = doctorAppointments.Count;
            var today = DateOnly.FromDateTime(DateTime.Now);
            ViewBag.TodayAppointments = doctorAppointments.Where(a => a.Appointment_Date == today).OrderBy(a => a.Appointment_Time).ToList();
            ViewBag.UpcomingAppointments = doctorAppointments.Where(a => a.Appointment_Date >= today).OrderBy(a => a.Appointment_Date).Take(5);
            ViewBag.PatientCount = doctorAppointments.Where(a => !string.IsNullOrEmpty(a.PatientName)).Select(a => a.PatientName).Distinct().Count();
            ViewBag.MedicalRecordsCount = medicalRecordsCount;
            ViewBag.MedicalRecordsWorking = medicalRecordsWorking;

            return View();
        }
    }
}

