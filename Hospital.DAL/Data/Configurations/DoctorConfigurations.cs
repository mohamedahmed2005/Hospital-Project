using Hospital.DAL.Models.DoctorModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Data.Configurations
{
    public class DoctorConfigurations : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.Property(d => d.Id)
                .IsRequired()
                .UseIdentityColumn(1, 1);

            builder.Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Bio)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(d => d.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            builder.HasIndex(d => d.PhoneNumber)
                .IsUnique();

            builder.Property(d => d.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(d => d.Email)
                .IsUnique();

            builder.Property(d => d.ExpertYears)
                .IsRequired();

            builder.Property(d => d.Education)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(d => d.Status)
                .IsRequired();

            builder.Property(d => d.Status)
               .HasConversion((DoctorStatus) => DoctorStatus.ToString(), (_DoctorStatus) => (DoctorStatus)Enum.Parse(typeof(DoctorStatus), _DoctorStatus));

            builder.Ignore(d => d.Name);

            #region 1-to-M Doctor With Patients 
            builder.HasMany(D=>D.Patients)
                .WithOne(E=>E.Doctor)
                .HasForeignKey(E=>E.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);
            #endregion
        }
    }

}
