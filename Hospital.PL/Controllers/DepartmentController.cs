using Hospital.BBL.DTOs.DepartmentDTOs;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Models.DepartmentModule;
using Hospital.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hospital.PL.Controllers
{
    [Authorize]
    public class DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger, IWebHostEnvironment environment, IDoctorService doctorService) : Controller
    {
        private readonly IDepartmentService _departmentService = departmentService;
        private readonly ILogger<DepartmentController> _logger = logger;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly IDoctorService _doctorService = doctorService;

        #region Index - GetAllDepartments
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var departments = _departmentService.GetAllDepartments(true);
            // Doctors can view but not modify
            ViewBag.CanModify = User.IsInRole("Admin");
            return View(departments);
        }
        #endregion


        #region Create - AddDepartment
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new DepartmentViewModel()
            {
                Doctors = _doctorService.GetAllDoctors(true)
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(DepartmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = new AddDepartmentDto()
                    {
                        Name = viewModel.Name,
                        DepartmentSpeciality = viewModel.DepartmentSpeciality,
                        Status = viewModel.Status,
                        Location = viewModel.Location,
                        KeyServices = viewModel.KeyServices,
                        Description = viewModel.Description,
                        DoctorId = viewModel.DoctorId
                    };

                    int result = _departmentService.AddDepartment(dto);

                    if (result > 0)
                        return RedirectToAction(nameof(Index));

                    ModelState.AddModelError(string.Empty, "Failed to add department.");
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex.Message);
                }
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(e => e.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                ViewBag.Errors = errors;

                viewModel.Doctors = _doctorService.GetAllDoctors(true)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    });
            }
            return View(viewModel);
        }
        #endregion


        #region Details - GetDepartmentById
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var department = _departmentService.GetDepartmentById(id.Value);
            if (department is null) return NotFound();

            return View(department);
        }
        #endregion


        #region Delete - DeleteDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (id == 0) return BadRequest();

            try
            {
                bool deleted = _departmentService.DeleteDepartment(id);

                if (deleted)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "Failed to delete department.");
                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError(ex.Message);
                    return View("Errorview", ex);
                }
            }
        }
        #endregion


        #region Edit - UpdateDepartment
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var dept = _departmentService.GetDepartmentById(id.Value);
            if (dept is null) return NotFound();

            var model = new DepartmentViewModel()
            {
                Id = dept.Id,
                Name = dept.Name,
                DepartmentSpeciality = dept.DepartmentSpeciality,
                Status = dept.Status,
                Location = dept.Location,
                KeyServices = dept.KeyServices,
                Description = dept.Description,
                DoctorId = dept.DoctorId
            };

            model.Doctors = _doctorService.GetAllDoctors(true)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = d.Id == dept.DoctorId 
                });

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit([FromRoute] int? id, DepartmentViewModel viewModel)
        {
            if (!id.HasValue)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                viewModel.Doctors = _doctorService.GetAllDoctors(true)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name,
                        Selected = d.Id == viewModel.DoctorId
                    });

                return View(viewModel);
            }

            try
            {
                var dto = new UpdateDepartmentDto()
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                    DepartmentSpeciality = viewModel.DepartmentSpeciality,
                    Status = viewModel.Status,
                    Location = viewModel.Location,
                    KeyServices = viewModel.KeyServices,
                    Description = viewModel.Description,
                    DoctorId = viewModel.DoctorId
                };

                int result = _departmentService.UpdateDepartment(dto);

                if (result > 0)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "Failed to update department.");

                viewModel.Doctors = _doctorService.GetAllDoctors(true)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name,
                        Selected = d.Id == viewModel.DoctorId
                    });

                return View(viewModel);
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                else
                {
                    _logger.LogError(ex.Message);
                }

                viewModel.Doctors = _doctorService.GetAllDoctors(true)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name,
                        Selected = d.Id == viewModel.DoctorId
                    });

                return View(viewModel);
            }
        }
        #endregion
    }

}
