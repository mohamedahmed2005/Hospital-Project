using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Contexts;
using Hospital.DAL.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital.PL.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorAppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DoctorAppointmentController(
            IAppointmentService appointmentService,
            IDoctorService doctorService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> MyAppointments(string date)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Find doctor by email
            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile not found. Please contact administrator.";
                return RedirectToAction("Doctor", "Dashboard");
            }

            // Parse date from string if provided
            DateOnly? parsedDate = null;
            bool showAll = string.IsNullOrEmpty(date) || date.ToLower() == "all";
            
            if (!showAll && !string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var dateValue))
            {
                parsedDate = dateValue;
            }

            // Get all appointments for this doctor first
            var allAppointments = _appointmentService.GetAllAppointments(false)
                .Where(a => a.DoctorId.HasValue && a.DoctorId.Value == doctor.Id)
                .OrderBy(a => a.Appointment_Date)
                .ThenBy(a => a.Appointment_Time)
                .ToList();

            // Filter by selected date if provided, otherwise show all upcoming appointments
            var today = DateOnly.FromDateTime(DateTime.Now);
            var selectedDate = parsedDate ?? today;
            
            IEnumerable<Hospital.BBL.DTOs.AppointmentDTOs.GetAllAppointmentsDto> appointments;
            
            if (showAll)
            {
                // Show all appointments (past and future)
                appointments = allAppointments.ToList();
            }
            else if (parsedDate.HasValue)
            {
                // Show appointments for specific date
                appointments = allAppointments.Where(a => a.Appointment_Date == selectedDate).ToList();
            }
            else
            {
                // Show all upcoming appointments
                appointments = allAppointments.Where(a => a.Appointment_Date >= today).ToList();
            }

            ViewBag.Doctor = doctor;
            ViewBag.SelectedDate = selectedDate;
            ViewBag.Appointments = appointments;
            ViewBag.AllAppointments = allAppointments;
            ViewBag.HasDateFilter = parsedDate.HasValue;
            ViewBag.DateParam = date ?? "";

            return View();
        }

        #region Details
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Details(int? ID)
        {
            Console.WriteLine("\n\n\nAt DoctorApp Details \n id:" + ID + "\n\n\n");
            TempData["FromMyAppointment"] = "return to My Appointment";
            return RedirectToAction(nameof(Details), "Appointment", new { id = ID });
        }
        #endregion



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability([FromBody] ToggleAvailabilityRequest request)
        {
            if (request == null || request.AppointmentId <= 0)
                return Json(new { success = false, message = "Invalid request" });

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            // Verify the appointment belongs to this doctor
            var doctors = _doctorService.GetAllDoctors(true);
            var doctor = doctors.FirstOrDefault(d => d.Email == user.Email);

            if (doctor == null)
                return Json(new { success = false, message = "Doctor profile not found" });

            var appointment = _appointmentService.GetAppointmentById(request.AppointmentId);
            if (appointment == null || !appointment.DoctorId.HasValue || appointment.DoctorId.Value != doctor.Id)
                return Json(new { success = false, message = "Appointment not found or access denied" });

            var result = _appointmentService.ToggleAppointmentAvailability(request.AppointmentId);
            
            if (result)
            {
                var updatedAppointment = _appointmentService.GetAppointmentById(request.AppointmentId);
                return Json(new { success = true, isAvailable = updatedAppointment?.IsAvailable ?? false });
            }

            return Json(new { success = false, message = "Failed to update availability" });
        }
    }

    public class ToggleAvailabilityRequest
    {
        public int AppointmentId { get; set; }
    }
}

