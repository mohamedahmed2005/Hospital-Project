(() => {
  let currentLanguage = localStorage.getItem("language") === "ar" ? "ar" : "en";
  document.addEventListener("languageChanged", (e) => {
    currentLanguage = e.detail.language;
  });


  const tableViewBtn = document.getElementById("tableViewBtn");
  const gridViewBtn = document.getElementById("gridViewBtn");
  const tableView = document.getElementById("tableView");
  const gridView = document.getElementById("gridView");
  const departmentSearchInput = document.getElementById("departmentSearchInput");
  const departmentSearchButton = document.getElementById("departmentSearchButton");
  const departmentStatusFilter = document.getElementById("departmentStatusFilter");
  const departmentPageSizeSelect = document.getElementById("departmentsPageSize");
  const departmentPaginationWrapper = document.getElementById(
    "departmentsPagination"
  );
  const departmentPaginationToolbar = document.getElementById(
    "departmentsPaginationToolbar"
  );
  const departmentEmptyState = document.getElementById("departmentsEmptyState");
  const departmentCountLabelEn = document.getElementById(
    "departmentsCountLabelEn"
  );
  const departmentCountLabelAr = document.getElementById(
    "departmentsCountLabelAr"
  );
  const departmentTableRows = Array.from(
    document.querySelectorAll("#departmentsTableBody tr[data-keyword]")
  );
  const departmentCards = Array.from(
    document.querySelectorAll("#departmentCardContainer [data-keyword]")
  );
  const departmentEntries = departmentTableRows.map((row, index) => ({
    row,
    card: departmentCards[index] || null,
    keyword: (row.dataset.keyword || "").toLowerCase(),
    status: (row.dataset.status || "").toLowerCase(),
  }));
  let departmentFilteredEntries = [...departmentEntries];
  let departmentCurrentPage = 1;
  let departmentActiveView = "table";

  function setDepartmentView(view) {
    departmentActiveView = view;
    if (view === "table") {
      tableViewBtn?.classList.add("active");
      gridViewBtn?.classList.remove("active");
      if (tableView) tableView.style.display = "block";
      if (gridView) gridView.style.display = "none";
    } else {
      gridViewBtn?.classList.add("active");
      tableViewBtn?.classList.remove("active");
      if (gridView) gridView.style.display = "block";
      if (tableView) tableView.style.display = "none";
    }
  }

  tableViewBtn?.addEventListener("click", () => setDepartmentView("table"));
  gridViewBtn?.addEventListener("click", () => setDepartmentView("grid"));

  function updateDepartmentCountLabel(startIndex, endIndex, totalFiltered) {
    if (departmentCountLabelEn) {
      departmentCountLabelEn.textContent =
        totalFiltered === 0
          ? "Showing 0 department(s)"
          : `Showing ${startIndex + 1}-${endIndex} of ${totalFiltered} department(s)`;
    }
    if (departmentCountLabelAr) {
      departmentCountLabelAr.textContent =
        totalFiltered === 0
          ? "إظهار 0 قسم"
          : `إظهار ${startIndex + 1}-${endIndex} من ${totalFiltered} قسم`;
    }
  }

  function renderDepartmentPagination(totalPages) {
    if (!departmentPaginationWrapper) return;
    if (totalPages <= 1) {
      departmentPaginationWrapper.innerHTML = "";
      return;
    }

    let buttons = `<button type="button" data-page="${departmentCurrentPage - 1
      }" ${departmentCurrentPage === 1 ? "disabled" : ""}>‹</button>`;

    for (let page = 1; page <= totalPages; page++) {
      buttons += `<button type="button" data-page="${page}" ${page === departmentCurrentPage ? 'class="active"' : ""
        }>${page}</button>`;
    }

    buttons += `<button type="button" data-page="${departmentCurrentPage + 1
      }" ${departmentCurrentPage === totalPages ? "disabled" : ""}>›</button>`;

    departmentPaginationWrapper.innerHTML = buttons;
  }

  function applyDepartmentPagination() {
    const rowsPerPage = parseInt(departmentPageSizeSelect?.value || "10", 10);
    const totalFiltered = departmentFilteredEntries.length;
    const hasEntries = departmentEntries.length > 0;

    if (departmentPaginationToolbar) {
      departmentPaginationToolbar.classList.toggle("d-none", !hasEntries);
    }

    departmentEntries.forEach(({ row, card }) => {
      if (row) row.style.display = "none";
      if (card) card.style.display = "none";
    });

    if (totalFiltered === 0) {
      // Only show the "no matches" message if there are departments but they're filtered out
      if (hasEntries) {
        departmentEmptyState?.classList.remove("d-none");
      } else {
        departmentEmptyState?.classList.add("d-none");
      }
      renderDepartmentPagination(0);
      updateDepartmentCountLabel(0, 0, 0);
      return;
    }

    departmentEmptyState?.classList.add("d-none");

    const totalPages = Math.max(1, Math.ceil(totalFiltered / rowsPerPage));
    departmentCurrentPage = Math.min(departmentCurrentPage, totalPages);

    const startIndex = (departmentCurrentPage - 1) * rowsPerPage;
    const pageEntries = departmentFilteredEntries.slice(
      startIndex,
      startIndex + rowsPerPage
    );

    pageEntries.forEach(({ row, card }) => {
      if (row) row.style.display = "";
      if (card) card.style.display = "";
    });

    updateDepartmentCountLabel(
      startIndex,
      startIndex + pageEntries.length,
      totalFiltered
    );
    renderDepartmentPagination(totalPages);
  }

  function filterDepartments() {
    const query = (departmentSearchInput?.value || "").trim().toLowerCase();
    const status = (departmentStatusFilter?.value || "").toLowerCase();

    departmentFilteredEntries = departmentEntries.filter(({ keyword, status: rowStatus }) => {
      const matchesQuery = !query || keyword.includes(query);
      const matchesStatus = !status || rowStatus === status;
      return matchesQuery && matchesStatus;
    });

    departmentCurrentPage = 1;
    applyDepartmentPagination();
  }

  departmentSearchInput?.addEventListener("input", filterDepartments);
  departmentStatusFilter?.addEventListener("change", filterDepartments);
  departmentPageSizeSelect?.addEventListener("change", () => {
    departmentCurrentPage = 1;
    applyDepartmentPagination();
  });
  departmentPaginationWrapper?.addEventListener("click", (event) => {
    const button = event.target.closest("button[data-page]");
    if (!button || button.disabled) return;
    const page = parseInt(button.dataset.page, 10);
    if (Number.isNaN(page) || page === departmentCurrentPage) return;
    departmentCurrentPage = Math.max(1, page);
    applyDepartmentPagination();
  });

  function animateStats() {
    const stats = [
      {
        element: document.getElementById("totalDepartments"),
        target: 12,
        duration: 2000,
      },
      {
        element: document.getElementById("medicalStaff"),
        target: 85,
        duration: 2000,
      },
      {
        element: document.getElementById("activeDepartments"),
        target: 10,
        duration: 2000,
      },
      {
        element: document.getElementById("monthlyPatients"),
        target: 2450,
        duration: 2000,
      },
    ];

    stats.forEach((stat) => {
      let start = 0;
      const increment = stat.target / (stat.duration / 16);
      const timer = setInterval(() => {
        start += increment;
        if (start >= stat.target) {
          stat.element.textContent = stat.target;
          clearInterval(timer);
        } else {
          stat.element.textContent = Math.floor(start);
        }
        stat.element.classList.add("counting");
      }, 16);
    });
  }

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

  function validateAndAddDepartment() {
    if (validateForm("addDepartmentForm")) {
      const message =
        currentLanguage === "en"
          ? "Department added successfully!"
          : "تمت إضافة القسم بنجاح!";
      showAlert(message, "success");

      document.getElementById("addDepartmentForm").reset();
      document
        .getElementById("addDepartmentForm")
        .classList.remove("was-validated");
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("addDepartmentModal")
      );
      modal.hide();

      animateStats();
    }
  }

  function validateAndUpdateDepartment() {
    if (validateForm("editDepartmentForm")) {
      const message =
        currentLanguage === "en"
          ? "Department information updated successfully!"
          : "تم تحديث معلومات القسم بنجاح!";
      showAlert(message, "success");

      const modal = bootstrap.Modal.getInstance(
        document.getElementById("editDepartmentModal")
      );
      modal.hide();
    }
  }

  function validateAndAddStaff() {
    if (validateForm("addStaffForm")) {
      const message =
        currentLanguage === "en"
          ? "Staff member added successfully!"
          : "تمت إضافة الموظف بنجاح!";
      showAlert(message, "success");

      document.getElementById("addStaffForm").reset();
      document.getElementById("addStaffForm").classList.remove("was-validated");
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("addStaffModal")
      );
      modal.hide();
    }
  }

  function removeStaff() {
    const reason = document.getElementById("removeReason").value;
    if (!reason) {
      const message =
        currentLanguage === "en"
          ? "Please provide a reason for removal."
          : "يرجى تقديم سبب الإزالة.";
      showAlert(message, "warning");
      return;
    }

    const message =
      currentLanguage === "en"
        ? "Staff member removed successfully!"
        : "تمت إزالة الموظف بنجاح!";
    showAlert(message, "success");
    const modal = bootstrap.Modal.getInstance(
      document.getElementById("removeStaffModal")
    );
    modal.hide();
  }

  function deleteDepartment() {
    const reason = document.getElementById("deleteReason").value;
    if (!reason) {
      const message =
        currentLanguage === "en"
          ? "Please provide a reason for deletion."
          : "يرجى تقديم سبب الحذف.";
      showAlert(message, "warning");
      return;
    }

    const message =
      currentLanguage === "en"
        ? "Department deleted successfully!"
        : "تم حذف القسم بنجاح!";
    showAlert(message, "success");
    const modal = bootstrap.Modal.getInstance(
      document.getElementById("deleteDepartmentModal")
    );
    modal.hide();
  }

  document.addEventListener("DOMContentLoaded", function () {
    animateStats();
    setDepartmentView("table");
    filterDepartments();
  });
})();
