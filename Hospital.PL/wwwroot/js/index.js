const languageSwitcher = document.getElementById("languageSwitcher");
const darkModeToggle = document.getElementById("darkModeToggle");
const body = document.body;
const html = document.documentElement;

let currentLanguage = localStorage.getItem("language") === "ar" ? "ar" : "en";
let currentTheme = localStorage.getItem("theme") || "";

function applyLanguage(language, emitEvent = true) {
  const isArabic = language === "ar";

  document.querySelectorAll(".en-text").forEach((el) => {
    el.style.display = isArabic ? "none" : "inline";
  });
  document.querySelectorAll(".ar-text").forEach((el) => {
    el.style.display = isArabic ? "inline" : "none";
  });

  document.querySelectorAll("option[data-ar]").forEach((el) => {
    if (!el.dataset.en) {
      el.dataset.en = el.textContent.trim();
    }
    el.textContent = isArabic ? el.dataset.ar : el.dataset.en || el.textContent;
  });

  document.querySelectorAll("[data-ar-placeholder]").forEach((el) => {
    if (!el.dataset.enPlaceholder) {
      el.dataset.enPlaceholder = el.getAttribute("placeholder") || "";
    }
    el.placeholder = isArabic
      ? el.dataset.arPlaceholder || ""
      : el.dataset.enPlaceholder || "";
  });

  body.classList.toggle("rtl", isArabic);
  body.setAttribute("dir", isArabic ? "rtl" : "ltr");
  html.setAttribute("lang", isArabic ? "ar" : "en");
  html.setAttribute("dir", isArabic ? "rtl" : "ltr");

  if (languageSwitcher) {
    languageSwitcher.innerHTML = isArabic ? "EN" : "<small>ع</small>";
  }

  currentLanguage = language;
  localStorage.setItem("language", language);

  if (emitEvent) {
    document.dispatchEvent(
      new CustomEvent("languageChanged", { detail: { language } })
    );
  }
}

function applyTheme(theme, emitEvent = true) {
  const isDark = theme === "dark";
  body.classList.toggle("dark-mode", isDark);
  if (darkModeToggle) {
    darkModeToggle.innerHTML = isDark
      ? '<i class="bi bi-sun"></i>'
      : '<i class="bi bi-moon"></i>';
  }
  localStorage.setItem("theme", theme);
  currentTheme = theme;

  if (emitEvent) {
    document.dispatchEvent(
      new CustomEvent("themeChanged", { detail: { theme } })
    );
  }
}

applyLanguage(currentLanguage, false);
document.dispatchEvent(
  new CustomEvent("languageChanged", { detail: { language: currentLanguage } })
);

if (
  !currentTheme &&
  window.matchMedia("(prefers-color-scheme: dark)").matches
) {
  currentTheme = "dark";
} else if (!currentTheme) {
  currentTheme = "light";
}

applyTheme(currentTheme, false);
document.dispatchEvent(
  new CustomEvent("themeChanged", { detail: { theme: currentTheme } })
);

languageSwitcher?.addEventListener("click", () => {
  const nextLanguage = currentLanguage === "en" ? "ar" : "en";
  applyLanguage(nextLanguage);
});

darkModeToggle?.addEventListener("click", () => {
  const nextTheme = body.classList.contains("dark-mode") ? "light" : "dark";
  applyTheme(nextTheme);
});

function showAlert(message, type = "success") {
  const alertMessage = document.getElementById("alertMessage");
  alertMessage.innerHTML = `
    <div class="alert alert-${type} alert-dismissible fade show" role="alert">
      ${message}
      <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
  `;
  alertMessage.classList.add("show");
  setTimeout(() => alertMessage.classList.remove("show"), 5000);
}

function validateForm(form) {
  if (!form.checkValidity()) {
    form.classList.add("was-validated");
    return false;
  }
  return true;
}

function setupPasswordToggle(inputId, btnId) {
  const input = document.getElementById(inputId);
  const btn = document.getElementById(btnId);
  if (input && btn) {
    btn.addEventListener("click", () => {
      const type = input.type === "password" ? "text" : "password";
      input.type = type;
      btn.innerHTML =
        type === "password"
          ? '<i class="bi bi-eye"></i>'
          : '<i class="bi bi-eye-slash"></i>';
    });
  }
}

document.addEventListener("DOMContentLoaded", () => {
  const loginForm = document.getElementById("loginForm");
  if (loginForm) {
    setupPasswordToggle("loginPassword", "toggleLoginPassword");
    loginForm.addEventListener("submit", (e) => {
      e.preventDefault();
      if (validateForm(loginForm)) {
        showAlert(
          currentLanguage === "en"
            ? "Login successful!"
            : "تم تسجيل الدخول بنجاح!",
          "success"
        );
        setTimeout(() => (window.location.href = "home.html"), 2000);
      }
    });
  }

  const registerForm = document.getElementById("registerForm");
  if (registerForm) {
    setupPasswordToggle("registerPassword", "toggleRegisterPassword");
    setupPasswordToggle("confirmPassword", "toggleConfirmPassword");
    registerForm.addEventListener("submit", (e) => {
      e.preventDefault();
      if (!validateForm(registerForm)) return;
      const p1 = document.getElementById("registerPassword").value;
      const p2 = document.getElementById("confirmPassword").value;
      if (p1 !== p2)
        return showAlert(
          currentLanguage === "en"
            ? "Passwords do not match."
            : "كلمات المرور غير متطابقة.",
          "danger"
        );
      if (p1.length < 8)
        return showAlert(
          currentLanguage === "en"
            ? "Password too short."
            : "كلمة المرور قصيرة جداً.",
          "danger"
        );
      showAlert(
        currentLanguage === "en" ? "Account created!" : "تم إنشاء الحساب!",
        "success"
      );
      setTimeout(() => (window.location.href = "index.html"), 2000);
    });
  }

  const forgotForm = document.getElementById("forgotPasswordForm");
  if (forgotForm) {
    forgotForm.addEventListener("submit", (e) => {
      e.preventDefault();
      if (validateForm(forgotForm)) {
        showAlert(
          currentLanguage === "en" ? "Reset link sent!" : "تم إرسال الرابط!",
          "success"
        );
        forgotForm.reset();
        forgotForm.classList.remove("was-validated");
      }
    });
  }

  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) entry.target.classList.add("visible");
      });
    },
    { threshold: 0.1 }
  );
  document.querySelectorAll(".fade-in").forEach((el) => observer.observe(el));

  // Account Dropdown Functionality
  function initializeAccountDropdowns() {
    const accountDropdowns = ['adminDropdown', 'doctorDropdown', 'patientDropdown'];
    const allDropdowns = [];

    // Helper function to close all dropdowns
    function closeAllDropdowns() {
      allDropdowns.forEach(({ dropdown, dropdownButton, dropdownMenu }) => {
        dropdown.classList.remove('show');
        dropdownMenu.classList.remove('show');
        dropdownButton.setAttribute('aria-expanded', 'false');
      });
    }

    // Setup individual dropdown buttons
    accountDropdowns.forEach(dropdownId => {
      const dropdownButton = document.getElementById(dropdownId);
      if (!dropdownButton) return;

      const dropdown = dropdownButton.closest('.dropdown');
      const dropdownMenu = dropdown?.querySelector('.dropdown-menu');

      if (!dropdown || !dropdownMenu) return;

      allDropdowns.push({ dropdown, dropdownButton, dropdownMenu });

      // Click handler to toggle dropdown
      dropdownButton.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();

        const isOpen = dropdown.classList.contains('show');

        // Close all dropdowns first
        closeAllDropdowns();

        // Toggle current dropdown if it wasn't open
        if (!isOpen) {
          dropdown.classList.add('show');
          dropdownMenu.classList.add('show');
          dropdownButton.setAttribute('aria-expanded', 'true');
        }
      });

      // Keyboard support - Enter and Space keys
      dropdownButton.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault();
          e.stopPropagation();
          dropdownButton.click();
        }
      });

      // Close dropdown when clicking on menu items
      dropdownMenu.addEventListener('click', (e) => {
        // Check if clicked element is a link or inside a link
        if (e.target.tagName === 'A' || e.target.closest('a')) {
          closeAllDropdowns();
        }
      });
    });

    // Click outside handler - close all dropdowns when clicking outside
    document.addEventListener('click', (e) => {
      // Check if click is inside any dropdown
      const clickedInsideDropdown = allDropdowns.some(({ dropdown }) =>
        dropdown.contains(e.target)
      );

      // If clicked outside all dropdowns, close them all
      if (!clickedInsideDropdown) {
        closeAllDropdowns();
      }
    });

    // Escape key handler - close all dropdowns
    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape') {
        const openDropdown = allDropdowns.find(({ dropdown }) =>
          dropdown.classList.contains('show')
        );

        if (openDropdown) {
          closeAllDropdowns();
          openDropdown.dropdownButton.focus();
        }
      }
    });

    // Close dropdowns when navbar toggler is clicked (mobile menu)
    const navbarToggler = document.querySelector('.navbar-toggler');
    if (navbarToggler) {
      navbarToggler.addEventListener('click', () => {
        closeAllDropdowns();
      });
    }
  }

  // Initialize account dropdowns
  initializeAccountDropdowns();
});
