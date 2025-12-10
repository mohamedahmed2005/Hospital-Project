using Hospital.DAL.Models.AppointmentModule;
using Hospital.BBL.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital.BBL.DTOs.AppointmentDTOs
{
    public class AddAppointmentDto
    {
        [Required(ErrorMessage = "Appointment date is required")]
        [FutureDate(maxDaysInFuture: 365)]
        public DateOnly Appointment_Date { get; set; }

        [Required(ErrorMessage = "Appointment time is required")]
        public TimeSpan Appointment_Time { get; set; }

        [Required(ErrorMessage = "Appointment type is required")]
        public AppointmentType AppointmentType { get; set; }

        [Required(ErrorMessage = "Appointment status is required")]
        public AppointmentStatus Status { get; set; }

        public bool IsAvailable { get; set; } = true;

        [MaxLength(1000, ErrorMessage = "Max length should be 1000 character")]
        [MinLength(3, ErrorMessage = "Min length should be 3 characters")]
        [Required(ErrorMessage = "Notes are required")]
        public string Notes { get; set; } = null!;

        public IFormFile? Image { get; set; }

        // 🔹 إضافات جديدة لربط المرضى والدكاترة
        [Required(ErrorMessage = "Patient is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Doctor is required")]
        public int DoctorId { get; set; }
    }
}
