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

  function validateAndAddPatient() {
    if (validateForm("addPatientForm")) {
      const message =
        currentLanguage === "en"
          ? "Patient added successfully!"
          : "تمت إضافة المريض بنجاح!";
      showAlert(message, "success");

      document.getElementById("addPatientForm").reset();
      document.getElementById("addPatientForm").classList.remove("was-validated");
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("addPatientModal")
      );
      modal.hide();
    }
  }

  function validateAndUpdatePatient() {
    if (validateForm("editPatientForm")) {
      const message =
        currentLanguage === "en"
          ? "Patient information updated successfully!"
          : "تم تحديث معلومات المريض بنجاح!";
      showAlert(message, "success");

      const modal = bootstrap.Modal.getInstance(
        document.getElementById("editPatientModal")
      );
      modal.hide();
    }
  }

  function validateAndAddRecord() {
    if (validateForm("addRecordForm")) {
      const message =
        currentLanguage === "en"
          ? "Medical record added successfully!"
          : "تمت إضافة السجل الطبي بنجاح!";
      showAlert(message, "success");

      document.getElementById("addRecordForm").reset();
      document.getElementById("addRecordForm").classList.remove("was-validated");
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("addRecordModal")
      );
      modal.hide();
    }
  }

  function deletePatient() {
    const message =
      currentLanguage === "en"
        ? "Patient deleted successfully!"
        : "تم حذف المريض بنجاح!";
    showAlert(message, "success");

    const modal = bootstrap.Modal.getInstance(
      document.getElementById("deletePatientModal")
    );
    modal.hide();
  }

  function calculateAge(dateOfBirth) {
    const today = new Date();
    const birthDate = new Date(dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    if (
      monthDiff < 0 ||
      (monthDiff === 0 && today.getDate() < birthDate.getDate())
    ) {
      age--;
    }

    return age;
  }

  document.getElementById("dateOfBirth")?.addEventListener("change", function () {
    const age = calculateAge(this.value);
  });

  const patientSearchInput = document.getElementById("searchInput");
  const patientSearchButton = document.getElementById("searchButton");
  const patientStatusFilter = document.getElementById("statusFilter");
  const patientPageSizeSelect = document.getElementById("patientsPageSize");
  const patientPaginationWrapper = document.getElementById("patientsPagination");
  const patientPaginationToolbar = document.getElementById(
    "patientsPaginationToolbar"
  );
  const patientEmptyState = document.getElementById("patientsEmptyState");
  const patientCountLabelEn = document.getElementById("patientsCountLabelEn");
  const patientCountLabelAr = document.getElementById("patientsCountLabelAr");
  const patientRows = Array.from(
    document.querySelectorAll("#patientsTableBody tr[data-keywords]")
  );
  let patientFilteredRows = [...patientRows];
  let patientCurrentPage = 1;

  function updatePatientCountLabel(startIndex, endIndex, totalFiltered) {
    if (patientCountLabelEn) {
      patientCountLabelEn.textContent =
        totalFiltered === 0
          ? "Showing 0 patient(s)"
          : `Showing ${startIndex + 1}-${endIndex} of ${totalFiltered} patient(s)`;
    }
    if (patientCountLabelAr) {
      patientCountLabelAr.textContent =
        totalFiltered === 0
          ? "إظهار 0 مريض"
          : `إظهار ${startIndex + 1}-${endIndex} من ${totalFiltered} مريض`;
    }
  }

  function renderPatientPagination(totalPages) {
    if (!patientPaginationWrapper) return;
    if (totalPages <= 1) {
      patientPaginationWrapper.innerHTML = "";
      return;
    }

    let buttons = `<button type="button" data-page="${patientCurrentPage - 1
      }" ${patientCurrentPage === 1 ? "disabled" : ""}>‹</button>`;

    for (let page = 1; page <= totalPages; page++) {
      buttons += `<button type="button" data-page="${page}" ${page === patientCurrentPage ? 'class="active"' : ""
        }>${page}</button>`;
    }

    buttons += `<button type="button" data-page="${patientCurrentPage + 1
      }" ${patientCurrentPage === totalPages ? "disabled" : ""}>›</button>`;

    patientPaginationWrapper.innerHTML = buttons;
  }

  function applyPatientPagination() {
    const rowsPerPage = parseInt(patientPageSizeSelect?.value || "10", 10);
    const totalFiltered = patientFilteredRows.length;
    const hasRows = patientRows.length > 0;

    if (patientPaginationToolbar) {
      patientPaginationToolbar.classList.toggle("d-none", !hasRows);
    }

    patientRows.forEach((row) => {
      row.style.display = "none";
    });

    if (totalFiltered === 0) {
      // Only show the "no matches" message if there are patients but they're filtered out
      if (hasRows) {
        patientEmptyState?.classList.remove("d-none");
      } else {
        patientEmptyState?.classList.add("d-none");
      }
      renderPatientPagination(0);
      updatePatientCountLabel(0, 0, 0);
      return;
    }

    patientEmptyState?.classList.add("d-none");

    const totalPages = Math.max(1, Math.ceil(totalFiltered / rowsPerPage));
    patientCurrentPage = Math.min(patientCurrentPage, totalPages);

    const startIndex = (patientCurrentPage - 1) * rowsPerPage;
    const pageRows = patientFilteredRows.slice(
      startIndex,
      startIndex + rowsPerPage
    );

    pageRows.forEach((row) => {
      row.style.display = "";
    });

    updatePatientCountLabel(
      startIndex,
      startIndex + pageRows.length,
      totalFiltered
    );
    renderPatientPagination(totalPages);
  }

  function filterPatients() {
    const query = (patientSearchInput?.value || "").trim().toLowerCase();
    const status = (patientStatusFilter?.value || "").toLowerCase();

    patientFilteredRows = patientRows.filter((row) => {
      const keywords = row.dataset.keywords || "";
      const rowStatus = row.dataset.status || "";
      const matchesQuery = !query || keywords.includes(query);
      const matchesStatus = !status || rowStatus === status;
      row.dataset.matches = matchesQuery && matchesStatus ? "true" : "false";
      return matchesQuery && matchesStatus;
    });

    patientCurrentPage = 1;
    applyPatientPagination();
  }

  patientSearchButton?.addEventListener("click", filterPatients);
  patientSearchInput?.addEventListener("input", filterPatients);
  patientSearchInput?.addEventListener("keyup", (e) => {
    if (e.key === "Enter") {
      filterPatients();
    }
  });
  patientStatusFilter?.addEventListener("change", filterPatients);
  patientPageSizeSelect?.addEventListener("change", () => {
    patientCurrentPage = 1;
    applyPatientPagination();
  });
  patientPaginationWrapper?.addEventListener("click", (event) => {
    const button = event.target.closest("button[data-page]");
    if (!button || button.disabled) return;
    const page = parseInt(button.dataset.page, 10);
    if (Number.isNaN(page) || page === patientCurrentPage) return;
    patientCurrentPage = Math.max(1, page);
    applyPatientPagination();
  });

  document.addEventListener("DOMContentLoaded", function () {
    if (localStorage.getItem("language") === "ar") {
      const searchInput = document.getElementById("searchInput");
      if (searchInput) {
        searchInput.placeholder = searchInput.dataset.arPlaceholder;
      }
    }

    filterPatients();
  });
})();
