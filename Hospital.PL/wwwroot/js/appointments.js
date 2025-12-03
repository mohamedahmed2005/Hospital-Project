(() => {
  let currentLanguage = localStorage.getItem("language") === "ar" ? "ar" : "en";

  const handleLanguageChange = (language) => {
    currentLanguage = language;
    if (typeof renderCalendarGrid === 'function') {
      renderCalendarGrid();
    }
  };

  document.addEventListener("languageChanged", (event) => {
    handleLanguageChange(event.detail.language);
  });

  handleLanguageChange(currentLanguage);

  // Wait for DOM to be ready before initializing
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeAppointments);
  } else {
    initializeAppointments();
  }

  function initializeAppointments() {
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

    function validateAndAddAppointment() {
      if (validateForm("addAppointmentForm")) {
        const message =
          currentLanguage === "en"
            ? "Appointment scheduled successfully!"
            : "تم جدولة الموعد بنجاح!";
        showAlert(message, "success");

        document.getElementById("addAppointmentForm").reset();
        document
          .getElementById("addAppointmentForm")
          .classList.remove("was-validated");
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("addAppointmentModal")
        );
        modal.hide();
      }
    }

    function validateAndUpdateAppointment() {
      if (validateForm("editAppointmentForm")) {
        const message =
          currentLanguage === "en"
            ? "Appointment updated successfully!"
            : "تم تحديث الموعد بنجاح!";
        showAlert(message, "success");

        const modal = bootstrap.Modal.getInstance(
          document.getElementById("editAppointmentModal")
        );
        modal.hide();
      }
    }

    function cancelAppointment() {
      const reason = document.getElementById("cancelReason").value;
      if (!reason) {
        const message =
          currentLanguage === "en"
            ? "Please provide a reason for cancellation."
            : "يرجى تقديم سبب للإلغاء.";
        showAlert(message, "warning");
        return;
      }
      const message =
        currentLanguage === "en"
          ? "Appointment cancelled successfully!"
          : "تم إلغاء الموعد بنجاح!";
      showAlert(message, "success");
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("cancelAppointmentModal")
      );
      modal.hide();
    }

    const tableViewBtn = document.getElementById("tableViewBtn");
    const calendarViewBtn = document.getElementById("calendarViewBtn");
    const tableView = document.getElementById("tableView");
    const calendarView = document.getElementById("calendarView");
    const appointmentsPageSizeSelect = document.getElementById(
      "appointmentsPageSize"
    );
    const appointmentsPaginationWrapper = document.getElementById(
      "appointmentsPagination"
    );
    const appointmentsPaginationToolbar = document.getElementById(
      "appointmentsPaginationToolbar"
    );
    const appointmentsEmptyState = document.getElementById(
      "appointmentsEmptyState"
    );
    const appointmentsCountLabelEn = document.getElementById(
      "appointmentsCountLabelEn"
    );
    const appointmentsCountLabelAr = document.getElementById(
      "appointmentsCountLabelAr"
    );

    tableViewBtn?.addEventListener("click", () => {
      tableViewBtn.classList.add("active");
      calendarViewBtn?.classList.remove("active");
      if (tableView) tableView.style.display = "block";
      if (calendarView) calendarView.style.display = "none";
    });

    calendarViewBtn?.addEventListener("click", () => {
      calendarViewBtn.classList.add("active");
      tableViewBtn?.classList.remove("active");
      if (calendarView) calendarView.style.display = "block";
      if (tableView) tableView.style.display = "none";
    });

    const prevWeekBtn = document.getElementById("prevWeek");
    const nextWeekBtn = document.getElementById("nextWeek");
    const calendarWeek = document.getElementById("calendarWeek");
    const calendarGrid = document.getElementById("calendarGrid");
    const appointmentsDataElement = document.getElementById("appointmentsData");
    let calendarAppointments = [];
    let calendarFilteredAppointments = [];

    if (appointmentsDataElement) {
      try {
        calendarAppointments = JSON.parse(appointmentsDataElement.textContent || "[]");
      } catch (error) {
        console.error("Failed to parse appointments calendar data", error);
        calendarAppointments = [];
      }
    }
    calendarFilteredAppointments = [...calendarAppointments];

    let currentWeek = new Date();
    currentWeek.setHours(0, 0, 0, 0);
    currentWeek.setDate(currentWeek.getDate() - currentWeek.getDay());

    function formatDateKey(date) {
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, "0");
      const day = String(date.getDate()).padStart(2, "0");
      return `${year}-${month}-${day}`;
    }

    window.renderCalendarGrid = function renderCalendarGrid() {
      if (!calendarGrid) return;
      calendarGrid.innerHTML = "";

      const locale = currentLanguage === "ar" ? "ar-EG" : "en-US";

      for (let offset = 0; offset < 7; offset++) {
        const dayDate = new Date(currentWeek);
        dayDate.setDate(currentWeek.getDate() + offset);

        const dayElement = document.createElement("div");
        dayElement.className = "calendar-day";

        const header = document.createElement("div");
        header.className = "fw-bold mb-2";
        header.innerHTML = `
      <div>${dayDate.toLocaleDateString(locale, { weekday: "short" })}</div>
      <div>${dayDate.getDate()}</div>
    `;
        dayElement.appendChild(header);

        const dateKey = formatDateKey(dayDate);
        const appointmentsForDay = calendarFilteredAppointments.filter(
          (appointment) => appointment.appointmentDate === dateKey
        );

        if (appointmentsForDay.length === 0) {
          const emptyState = document.createElement("div");
          emptyState.className = "text-muted small";
          emptyState.textContent =
            currentLanguage === "ar" ? "لا مواعيد" : "No appointments";
          dayElement.appendChild(emptyState);
        } else {
          appointmentsForDay.forEach((appointment) => {
            const appointmentElement = document.createElement("div");
            appointmentElement.className = "calendar-appointment";
            appointmentElement.title = `${appointment.appointmentType} - ${appointment.status}`;
            appointmentElement.dataset.appointmentId =
              appointment.appointmentId || "";
            appointmentElement.setAttribute("role", "button");
            appointmentElement.setAttribute("tabindex", "0");
            appointmentElement.innerHTML = `
          <div class="fw-semibold">${appointment.patientName}</div>
          <div class="text-muted small">${appointment.doctorName}</div>
          <div class="text-muted small">${appointment.time} • ${appointment.appointmentType}</div>
        `;

            const navigateToDetails = () => {
              if (appointment.detailsUrl) {
                window.location.href = appointment.detailsUrl;
              }
            };

            appointmentElement.addEventListener("click", navigateToDetails);
            appointmentElement.addEventListener("keydown", (event) => {
              if (event.key === "Enter" || event.key === " ") {
                event.preventDefault();
                navigateToDetails();
              }
            });

            dayElement.appendChild(appointmentElement);
          });
        }

        calendarGrid.appendChild(dayElement);
      }
    };

    function applyCalendarFilters(query, status, date) {
      calendarFilteredAppointments = calendarAppointments.filter((appointment) => {
        const keywords = (appointment.searchKey || "").toLowerCase();
        const rowStatus = (appointment.statusValue || "").toLowerCase();
        const appointmentDate = appointment.appointmentDate || "";
        const matchesQuery = !query || keywords.includes(query);
        const matchesStatus = !status || rowStatus === status;
        const matchesDate = !date || appointmentDate === date;
        return matchesQuery && matchesStatus && matchesDate;
      });
    }


    function updateCalendarWeek() {
      const startOfWeek = new Date(currentWeek);
      const endOfWeek = new Date(currentWeek);
      endOfWeek.setDate(endOfWeek.getDate() + 6);

      const options = { month: "long", day: "numeric", year: "numeric" };
      if (calendarWeek) {
        calendarWeek.textContent = `${startOfWeek.toLocaleDateString(
          "en-US",
          options
        )} - ${endOfWeek.toLocaleDateString("en-US", options)}`;
      }

      renderCalendarGrid();
    }

    prevWeekBtn?.addEventListener("click", () => {
      currentWeek.setDate(currentWeek.getDate() - 7);
      updateCalendarWeek();
    });

    nextWeekBtn?.addEventListener("click", () => {
      currentWeek.setDate(currentWeek.getDate() + 7);
      updateCalendarWeek();
    });

    updateCalendarWeek();

    const searchInput = document.getElementById("searchInput");
    const statusFilter = document.getElementById("statusFilter");
    const dateFilter = document.getElementById("dateFilter");
    const appointmentRows = Array.from(
      document.querySelectorAll("#appointmentsTableBody tr[data-keywords]")
    );
    let appointmentFilteredRows = [...appointmentRows];
    let appointmentCurrentPage = 1;

    function updateAppointmentsCountLabel(startIndex, endIndex, totalFiltered) {
      if (appointmentsCountLabelEn) {
        appointmentsCountLabelEn.textContent =
          totalFiltered === 0
            ? "Showing 0 appointment(s)"
            : `Showing ${startIndex + 1}-${endIndex} of ${totalFiltered} appointment(s)`;
      }
      if (appointmentsCountLabelAr) {
        appointmentsCountLabelAr.textContent =
          totalFiltered === 0
            ? "إظهار 0 موعد"
            : `إظهار ${startIndex + 1}-${endIndex} من ${totalFiltered} موعد`;
      }
    }

    function renderAppointmentsPagination(totalPages) {
      if (!appointmentsPaginationWrapper) return;
      if (totalPages <= 1) {
        appointmentsPaginationWrapper.innerHTML = "";
        return;
      }

      let buttons = `<button type="button" data-page="${appointmentCurrentPage - 1
        }" ${appointmentCurrentPage === 1 ? "disabled" : ""}>‹</button>`;

      for (let page = 1; page <= totalPages; page++) {
        buttons += `<button type="button" data-page="${page}" ${page === appointmentCurrentPage ? 'class="active"' : ""
          }>${page}</button>`;
      }

      buttons += `<button type="button" data-page="${appointmentCurrentPage + 1
        }" ${appointmentCurrentPage === totalPages ? "disabled" : ""}>›</button>`;

      appointmentsPaginationWrapper.innerHTML = buttons;
    }

    function applyAppointmentPagination() {
      const rowsPerPage = parseInt(appointmentsPageSizeSelect?.value || "10", 10);
      const totalFiltered = appointmentFilteredRows.length;
      const hasRows = appointmentRows.length > 0;

      if (appointmentsPaginationToolbar) {
        appointmentsPaginationToolbar.classList.toggle("d-none", !hasRows);
      }

      appointmentRows.forEach((row) => {
        row.style.display = "none";
      });

      if (totalFiltered === 0) {
        // Only show the "no matches" message if there are appointments but they're filtered out
        if (hasRows) {
          appointmentsEmptyState?.classList.remove("d-none");
        } else {
          appointmentsEmptyState?.classList.add("d-none");
        }
        renderAppointmentsPagination(0);
        updateAppointmentsCountLabel(0, 0, 0);
        return;
      }

      appointmentsEmptyState?.classList.add("d-none");

      const totalPages = Math.max(1, Math.ceil(totalFiltered / rowsPerPage));
      appointmentCurrentPage = Math.min(appointmentCurrentPage, totalPages);

      const startIndex = (appointmentCurrentPage - 1) * rowsPerPage;
      const pageRows = appointmentFilteredRows.slice(
        startIndex,
        startIndex + rowsPerPage
      );

      pageRows.forEach((row) => {
        row.style.display = "";
      });

      updateAppointmentsCountLabel(
        startIndex,
        startIndex + pageRows.length,
        totalFiltered
      );
      renderAppointmentsPagination(totalPages);
    }

    function filterAppointments() {
      const query = (searchInput?.value || "").trim().toLowerCase();
      const status = (statusFilter?.value || "").toLowerCase();
      const date = dateFilter?.value || "";

      appointmentFilteredRows = appointmentRows.filter((row) => {
        const keywords = row.dataset.keywords || "";
        const rowStatus = (row.dataset.status || "").toLowerCase();
        const rowDate = row.dataset.date || "";
        const matchesQuery = !query || keywords.includes(query);
        const matchesStatus = !status || rowStatus === status;
        const matchesDate = !date || rowDate === date;
        return matchesQuery && matchesStatus && matchesDate;
      });

      applyCalendarFilters(query, status, date);
      appointmentCurrentPage = 1;
      applyAppointmentPagination();
      renderCalendarGrid();
    }

    searchInput?.addEventListener("input", filterAppointments);
    statusFilter?.addEventListener("change", filterAppointments);
    dateFilter?.addEventListener("change", filterAppointments);
    appointmentsPageSizeSelect?.addEventListener("change", () => {
      appointmentCurrentPage = 1;
      applyAppointmentPagination();
    });
    appointmentsPaginationWrapper?.addEventListener("click", (event) => {
      const button = event.target.closest("button[data-page]");
      if (!button || button.disabled) return;
      const page = parseInt(button.dataset.page, 10);
      if (Number.isNaN(page) || page === appointmentCurrentPage) return;
      appointmentCurrentPage = Math.max(1, page);
      applyAppointmentPagination();
    });

    // Initialize filters and pagination
    filterAppointments();
  }
})();
