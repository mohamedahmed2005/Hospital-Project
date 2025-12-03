using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Models.PatientModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Data.Configurations
{
    public class PatientConfigurations : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.Property(p => p.Id)
                .IsRequired()
                .UseIdentityColumn(1, 1);

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            builder.HasIndex(p => p.PhoneNumber)
                .IsUnique();

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => p.Email)
                .IsUnique();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.Status)
               .HasConversion((PatientStatus) => PatientStatus.ToString(), (_PatientStatus) => (PatientStatus)Enum.Parse(typeof(PatientStatus), _PatientStatus));

            builder.Property(p => p.Gender)
             .HasConversion((Gender) => Gender.ToString(), (_Gender) => (Gender)Enum.Parse(typeof(Gender), _Gender));

            builder.Property(p => p.BloodType)
             .HasConversion((BloodType) => BloodType.ToString(), (_BloodType) => (BloodType)Enum.Parse(typeof(BloodType), _BloodType));


            builder.Property(p => p.Height)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(p => p.Weight)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(p => p.DateOfBirth)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Allergies)
                .HasMaxLength(300);

            builder.Property(p => p.MedicalHistory)
                .HasMaxLength(2000);

            builder.Property(p => p.CurrentMedications)
                .HasMaxLength(1000);

            builder.Ignore(p => p.IsDeleted);
            builder.Ignore(p => p.Name);
            builder.Ignore(p => p.Age);

        }
    }
}
