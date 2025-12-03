using Hospital.DAL.Models.MedicalRecordModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Data.Configurations
{
    public class MedicalRecordConfigurations : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.Property(m => m.Id)
                .IsRequired()
                .UseIdentityColumn(1, 1);

            builder.Property(m => m.RecordDate)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(m => m.Diagnosis)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(m => m.Treatment)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(m => m.Prescription)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(m => m.Notes)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(m => m.PatientId)
                .IsRequired();

            builder.Property(m => m.DoctorId)
                .IsRequired();

            builder.HasOne(m => m.Patient)
                .WithMany()
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Doctor)
                .WithMany()
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

