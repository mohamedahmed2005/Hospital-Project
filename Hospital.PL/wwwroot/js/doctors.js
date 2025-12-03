(() => {
  let currentLanguage = localStorage.getItem("language") === "ar" ? "ar" : "en";
  document.addEventListener("languageChanged", (e) => {
    currentLanguage = e.detail.language;
  });


  function showAlert(message, type = "success") {
    const alertMessage = document.getElementById("alertMessage");
    if (!alertMessage) return;
    alertMessage.innerHTML = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
          ${message}
          <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
      `;
    alertMessage.classList.add("show");

    setTimeout(() => {
      alertMessage.classList.remove("show");
    }, 5000);
  }

  function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form.checkValidity()) {
      form.classList.add("was-validated");
      return false;
    }
    return true;
  }

  function validateAndAddDoctor() {
    if (validateForm("addDoctorForm")) {
      const message =
        currentLanguage === "en"
          ? "Doctor added successfully!"
          : "تمت إضافة الطبيب بنجاح!";
      showAlert(message, "success");

      document.getElementById("addDoctorForm").reset();
      document.getElementById("addDoctorForm").classList.remove("was-validated");
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("addDoctorModal")
      );
      modal.hide();
    }
  }

  function validateAndUpdateDoctor() {
    if (validateForm("editDoctorForm")) {
      const message =
        currentLanguage === "en"
          ? "Doctor information updated successfully!"
          : "تم تحديث معلومات الطبيب بنجاح!";
      showAlert(message, "success");

      const modal = bootstrap.Modal.getInstance(
        document.getElementById("editDoctorModal")
      );
      modal.hide();
    }
  }

  function deleteDoctor() {
    const message =
      currentLanguage === "en"
        ? "Doctor deleted successfully!"
        : "تم حذف الطبيب بنجاح!";
    showAlert(message, "success");
    const modal = bootstrap.Modal.getInstance(
      document.getElementById("deleteDoctorModal")
    );
    modal.hide();
  }

  const doctorSearchInput = document.getElementById("doctorSearchInput");
  const departmentFilter = document.getElementById("departmentFilter");
  const doctorPageSizeSelect = document.getElementById("doctorsPageSize");
  const doctorPaginationWrapper = document.getElementById("doctorsPagination");
  const doctorPaginationToolbar = document.getElementById(
    "doctorsPaginationToolbar"
  );
  const doctorEmptyState = document.getElementById("doctorsEmptyState");
  const doctorCountLabelEn = document.getElementById("doctorsCountLabelEn");
  const doctorCountLabelAr = document.getElementById("doctorsCountLabelAr");
  const doctorRows = Array.from(
    document.querySelectorAll("#doctorsTableBody tr[data-name]")
  );
  let doctorFilteredRows = [...doctorRows];
  let doctorCurrentPage = 1;

  function updateDoctorCountLabel(startIndex, endIndex, totalFiltered) {
    if (doctorCountLabelEn) {
      doctorCountLabelEn.textContent =
        totalFiltered === 0
          ? "Showing 0 doctor(s)"
          : `Showing ${startIndex + 1}-${endIndex} of ${totalFiltered} doctor(s)`;
    }
    if (doctorCountLabelAr) {
      doctorCountLabelAr.textContent =
        totalFiltered === 0
          ? "إظهار 0 طبيب"
          : `إظهار ${startIndex + 1}-${endIndex} من ${totalFiltered} طبيب`;
    }
  }

  function renderDoctorPagination(totalPages) {
    if (!doctorPaginationWrapper) return;
    if (totalPages <= 1) {
      doctorPaginationWrapper.innerHTML = "";
      return;
    }

    let buttons = `<button type="button" data-page="${doctorCurrentPage - 1
      }" ${doctorCurrentPage === 1 ? "disabled" : ""}>‹</button>`;

    for (let page = 1; page <= totalPages; page++) {
      buttons += `<button type="button" data-page="${page}" ${page === doctorCurrentPage ? 'class="active"' : ""
        }>${page}</button>`;
    }

    buttons += `<button type="button" data-page="${doctorCurrentPage + 1
      }" ${doctorCurrentPage === totalPages ? "disabled" : ""}>›</button>`;

    doctorPaginationWrapper.innerHTML = buttons;
  }

  function applyDoctorPagination() {
    const rowsPerPage = parseInt(doctorPageSizeSelect?.value || "10", 10);
    const totalFiltered = doctorFilteredRows.length;
    const hasRows = doctorRows.length > 0;

    if (doctorPaginationToolbar) {
      doctorPaginationToolbar.classList.toggle("d-none", !hasRows);
    }

    doctorRows.forEach((row) => {
      row.style.display = "none";
    });

    if (totalFiltered === 0) {
      // Only show the "no matches" message if there are doctors but they're filtered out
      if (hasRows) {
        doctorEmptyState?.classList.remove("d-none");
      } else {
        doctorEmptyState?.classList.add("d-none");
      }
      renderDoctorPagination(0);
      updateDoctorCountLabel(0, 0, 0);
      return;
    }

    doctorEmptyState?.classList.add("d-none");

    const totalPages = Math.max(1, Math.ceil(totalFiltered / rowsPerPage));
    doctorCurrentPage = Math.min(doctorCurrentPage, totalPages);

    const startIndex = (doctorCurrentPage - 1) * rowsPerPage;
    const pageRows = doctorFilteredRows.slice(
      startIndex,
      startIndex + rowsPerPage
    );

    pageRows.forEach((row) => {
      row.style.display = "";
    });

    updateDoctorCountLabel(startIndex, startIndex + pageRows.length, totalFiltered);
    renderDoctorPagination(totalPages);
  }

  function filterDoctors() {
    const query = (doctorSearchInput?.value || "").trim().toLowerCase();
    const department = (departmentFilter?.value || "").toLowerCase();

    doctorFilteredRows = doctorRows.filter((row) => {
      const name = row.dataset.name || "";
      const specialty = row.dataset.specialty || "";
      const dept = (row.dataset.department || "").toLowerCase();
      const matchesQuery =
        !query ||
        name.includes(query) ||
        specialty.includes(query) ||
        dept.includes(query);
      const matchesDepartment = !department || dept === department;
      return matchesQuery && matchesDepartment;
    });

    doctorCurrentPage = 1;
    applyDoctorPagination();
  }

  // Event listeners
  doctorSearchInput?.addEventListener("input", filterDoctors);
  departmentFilter?.addEventListener("change", filterDoctors);
  doctorPageSizeSelect?.addEventListener("change", () => {
    doctorCurrentPage = 1;
    applyDoctorPagination();
  });

  doctorPaginationWrapper?.addEventListener("click", (e) => {
    const button = e.target.closest("button");
    if (!button || button.disabled) return;
    const page = parseInt(button.dataset.page, 10);
    if (Number.isNaN(page) || page === doctorCurrentPage) return;
    doctorCurrentPage = Math.max(1, page);
    applyDoctorPagination();
  });

  document.addEventListener("DOMContentLoaded", function () {
    filterDoctors();
  });

  // Expose functions to global scope for form validation
  window.validateAndAddDoctor = validateAndAddDoctor;
  window.validateAndUpdateDoctor = validateAndUpdateDoctor;
  window.deleteDoctor = deleteDoctor;
})();
