using Hospital.DAL.Models.AppointmentModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Data.Configurations
{
    public class AppointmentConfigurations : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.Property(a => a.Id)
                .IsRequired()
                .UseIdentityColumn(1, 1);

            builder.Property(a => a.Status)
               .HasConversion((AppointmentStatus) => AppointmentStatus.ToString(), (_AppointmentStatus) => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), _AppointmentStatus));

            builder.Property(a => a.AppointmentType)
               .HasConversion((AppointmentType) => AppointmentType.ToString(), (_AppointmentType) => (AppointmentType)Enum.Parse(typeof(AppointmentType), _AppointmentType));

            builder.Property(p => p.Appointment_Date)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(p => p.Appointment_Time)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(a => a.Notes)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(a => a.PatientId)
                .IsRequired(false);

            builder.Property(a => a.DoctorId)
                .IsRequired(false);

            builder.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
