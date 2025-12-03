const languageSwitcher = document.getElementById("languageSwitcher");
const body = document.body;
let currentLanguage = "en";

if (localStorage.getItem("language") === "ar") {
  switchToArabic();
}

languageSwitcher.addEventListener("click", () => {
  if (currentLanguage === "en") {
    switchToArabic();
  } else {
    switchToEnglish();
  }
});

function switchToArabic() {
  document.querySelectorAll(".en-text").forEach((el) => {
    el.style.display = "none";
  });
  document.querySelectorAll(".ar-text").forEach((el) => {
    el.style.display = "inline";
  });

  body.classList.add("rtl");
  body.setAttribute("dir", "rtl");
  document.documentElement.setAttribute("lang", "ar");
  document.documentElement.setAttribute("dir", "rtl");

  languageSwitcher.textContent = "EN";

  currentLanguage = "ar";

  localStorage.setItem("language", "ar");
}

function switchToEnglish() {
  document.querySelectorAll(".ar-text").forEach((el) => {
    el.style.display = "none";
  });
  document.querySelectorAll(".en-text").forEach((el) => {
    el.style.display = "inline";
  });

  body.classList.remove("rtl");
  body.setAttribute("dir", "ltr");
  document.documentElement.setAttribute("lang", "en");
  document.documentElement.setAttribute("dir", "ltr");

  languageSwitcher.textContent = "ع";

  currentLanguage = "en";

  localStorage.setItem("language", "en");
}

const darkModeToggle = document.getElementById("darkModeToggle");

if (
  localStorage.getItem("theme") === "dark" ||
  (window.matchMedia("(prefers-color-scheme: dark)").matches &&
    !localStorage.getItem("theme"))
) {
  body.classList.add("dark-mode");
  darkModeToggle.innerHTML = '<i class="bi bi-sun"></i>';
}

darkModeToggle.addEventListener("click", () => {
  body.classList.toggle("dark-mode");

  if (body.classList.contains("dark-mode")) {
    darkModeToggle.innerHTML = '<i class="bi bi-sun"></i>';
    localStorage.setItem("theme", "dark");
  } else {
    darkModeToggle.innerHTML = '<i class="bi bi-moon"></i>';
    localStorage.setItem("theme", "light");
  }
});

const fadeElements = document.querySelectorAll(".fade-in");

const fadeInOnScroll = () => {
  fadeElements.forEach((element) => {
    const elementTop = element.getBoundingClientRect().top;
    const elementVisible = 150;

    if (elementTop < window.innerHeight - elementVisible) {
      element.classList.add("visible");
    }
  });
};

window.addEventListener("load", fadeInOnScroll);
window.addEventListener("scroll", fadeInOnScroll);

function animateCounter(element, target, duration) {
  let start = 0;
  const increment = target / (duration / 16);

  const updateCounter = () => {
    start += increment;
    if (start < target) {
      element.textContent = Math.floor(start);
      requestAnimationFrame(updateCounter);
    } else {
      element.textContent = target;
    }
  };

  updateCounter();
}

function animateStats() {
  const counters = document.querySelectorAll(".counter");

  counters.forEach((counter) => {
    const target = parseInt(counter.getAttribute("data-target"));
    animateCounter(counter, target, 2000);
  });
}

document.addEventListener("DOMContentLoaded", function () {
  const currentPage = window.location.pathname.split("/").pop();
  const navLinks = document.querySelectorAll(".nav-link");

  navLinks.forEach((link) => {
    if (link.getAttribute("href") === currentPage) {
      link.classList.add("active");
    } else {
      link.classList.remove("active");
    }
  });

  animateStats();
});
