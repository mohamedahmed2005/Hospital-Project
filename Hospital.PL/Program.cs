using Hospital.BBL.Mapping;
using Hospital.BBL.Services.AttachementService;
using Hospital.BBL.Services.Classes;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Contexts;
using Hospital.DAL.Models.Shared;
using Hospital.DAL.Repositories.Classes;
using Hospital.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                // Add DateOnly and TimeOnly model binders
                options.ModelBinderProviders.Insert(0, new Microsoft.AspNetCore.Mvc.ModelBinding.Binders.SimpleTypeModelBinderProvider());
            });
            
            // Configure DateOnly and TimeOnly JSON serialization
            builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });
            #region Configure Services
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
                    b => b.MigrationsAssembly("Hospital.DAL"));
                options.UseLazyLoadingProxies();

            });
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(D => D.AddProfile(new DoctorMappingProfiles()));
            builder.Services.AddAutoMapper(D => D.AddProfile(new DepartmentMappingProfiles()));
            builder.Services.AddAutoMapper(P => P.AddProfile(new PatientMappingProfiles()));
            builder.Services.AddAutoMapper(A => A.AddProfile(new AppointmentMappingProfiles()));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // External authentication (Google)
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthSection["ClientId"] ?? string.Empty;
                    options.ClientSecret = googleAuthSection["ClientSecret"] ?? string.Empty;

                    // Optional: allow overriding callback path from configuration
                    var callbackPath = googleAuthSection["CallbackPath"];
                    if (!string.IsNullOrWhiteSpace(callbackPath))
                    {
                        options.CallbackPath = callbackPath;
                    }
                });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
                options.ReturnUrlParameter = "returnUrl";
            });
            builder.Services.AddScoped<IAttachementService, AttachementService>();
            #endregion

            var app = builder.Build();

            // Seed initial admin user if it doesn't exist
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await SeedAdminUserAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            await app.RunAsync();
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure Admin role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Doctor"))
            {
                await roleManager.CreateAsync(new IdentityRole("Doctor"));
            }
            if (!await roleManager.RoleExistsAsync("Patient"))
            {
                await roleManager.CreateAsync(new IdentityRole("Patient"));
            }

            // Check if admin user already exists
            var adminEmail = "admin@novahealth.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                // Create default admin user
                adminUser = new ApplicationUser
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                // Ensure existing admin user has Admin role
                var roles = await userManager.GetRolesAsync(adminUser);
                if (!roles.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
