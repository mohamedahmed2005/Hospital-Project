using Hospital.DAL.Models.DepartmentModule;
using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Data.Configurations
{
    public class DepartmentConfigurations : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.Property(d => d.Id)
                .IsRequired()
                .UseIdentityColumn(1, 1);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.DepartmentSpeciality)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Status)
                .IsRequired();

            builder.Property(d => d.Status)
                .HasConversion((DepartmentStatus) => DepartmentStatus.ToString(), (_DepartmentStatus) => (DepartmentStatus)Enum.Parse(typeof(DepartmentStatus), _DepartmentStatus));

            builder.Property(d => d.Location)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.KeyServices)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(d => d.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(d => d.DoctorId)
                .IsRequired(false);

            builder.HasOne(d => d.DepartmentHead)
                .WithMany()
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            #region 1-to-M Department With Doctors
            builder.HasMany(D => D.Doctors)
                .WithOne(E => E.Department)
                .HasForeignKey(E => E.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull); 
            #endregion
        }
    }

}
