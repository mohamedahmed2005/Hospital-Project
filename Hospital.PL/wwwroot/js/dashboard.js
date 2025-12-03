// Dashboard Observer Pattern for Responsive Design
class ResponsiveObserver {
  constructor() {
    this.observers = [];
    this.breakpoints = {
      mobile: 576,
      tablet: 768,
      tabletLarge: 992,
      desktop: 1200,
    };
    this.currentBreakpoint = this.getCurrentBreakpoint();
    this.init();
  }

  // Get current breakpoint based on window width
  getCurrentBreakpoint() {
    const width = window.innerWidth;
    if (width < this.breakpoints.mobile) return "mobile";
    if (width < this.breakpoints.tablet) return "tablet";
    if (width < this.breakpoints.tabletLarge) return "tabletLarge";
    if (width < this.breakpoints.desktop) return "desktop";
    return "desktopLarge";
  }

  // Subscribe to breakpoint changes
  subscribe(callback) {
    this.observers.push(callback);
    // Immediately call with current breakpoint
    callback(this.currentBreakpoint);
  }

  // Unsubscribe from breakpoint changes
  unsubscribe(callback) {
    this.observers = this.observers.filter((obs) => obs !== callback);
  }

  // Notify all observers of breakpoint change
  notify(breakpoint) {
    this.observers.forEach((observer) => observer(breakpoint));
  }

  // Handle window resize
  handleResize() {
    const newBreakpoint = this.getCurrentBreakpoint();
    if (newBreakpoint !== this.currentBreakpoint) {
      this.currentBreakpoint = newBreakpoint;
      this.notify(this.currentBreakpoint);
    }
  }

  // Initialize observer
  init() {
    // Throttle resize events
    let resizeTimeout;
    window.addEventListener("resize", () => {
      clearTimeout(resizeTimeout);
      resizeTimeout = setTimeout(() => {
        this.handleResize();
      }, 150);
    });

    // Initial notification
    this.notify(this.currentBreakpoint);
  }
}

// Create global responsive observer instance
const responsiveObserver = new ResponsiveObserver();

// Dashboard Responsive Manager
class DashboardResponsiveManager {
  constructor() {
    this.statCards = document.querySelectorAll(".stat-card");
    this.dashboardCards = document.querySelectorAll(".dashboard-card");
    this.appointmentItems = document.querySelectorAll(".appointment-item");
    this.recentItems = document.querySelectorAll(".recent-item");
    this.quickActionBtns = document.querySelectorAll(".quick-action-btn");
    this.init();
  }

  // Adjust layout based on breakpoint
  adjustLayout(breakpoint) {
    // Adjust stat cards
    this.statCards.forEach((card) => {
      if (breakpoint === "mobile") {
        card.classList.add("mobile-view");
        card.classList.remove("desktop-view");
      } else {
        card.classList.remove("mobile-view");
        card.classList.add("desktop-view");
      }
    });

    // Adjust dashboard cards
    this.dashboardCards.forEach((card) => {
      if (breakpoint === "mobile" || breakpoint === "tablet") {
        card.classList.add("mobile-card");
        card.classList.remove("desktop-card");
      } else {
        card.classList.remove("mobile-card");
        card.classList.add("desktop-card");
      }
    });

    // Adjust appointment items
    this.appointmentItems.forEach((item) => {
      if (breakpoint === "mobile") {
        item.classList.add("mobile-appointment");
        item.classList.remove("desktop-appointment");
      } else {
        item.classList.remove("mobile-appointment");
        item.classList.add("desktop-appointment");
      }
    });

    // Adjust recent items
    this.recentItems.forEach((item) => {
      if (breakpoint === "mobile") {
        item.classList.add("mobile-recent");
        item.classList.remove("desktop-recent");
      } else {
        item.classList.remove("mobile-recent");
        item.classList.add("desktop-recent");
      }
    });

    // Adjust quick action buttons
    this.quickActionBtns.forEach((btn) => {
      if (breakpoint === "mobile") {
        btn.classList.add("mobile-btn");
        btn.classList.remove("desktop-btn");
      } else {
        btn.classList.remove("mobile-btn");
        btn.classList.add("desktop-btn");
      }
    });
  }

  init() {
    // Subscribe to responsive observer
    responsiveObserver.subscribe((breakpoint) => {
      this.adjustLayout(breakpoint);
    });
  }
}

// Intersection Observer for fade-in animations
class FadeInObserver {
  constructor() {
    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("visible");
            // Optional: Unobserve after animation
            // this.observer.unobserve(entry.target);
          }
        });
      },
      {
        threshold: 0.1,
        rootMargin: "0px 0px -50px 0px",
      }
    );
    this.init();
  }

  init() {
    document.querySelectorAll(".fade-in").forEach((el) => {
      this.observer.observe(el);
    });
  }
}

// Initialize on DOM ready
document.addEventListener("DOMContentLoaded", () => {
  // Initialize responsive manager
  new DashboardResponsiveManager();

  // Initialize fade-in observer
  new FadeInObserver();

  // Add smooth scroll behavior
  document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
    anchor.addEventListener("click", function (e) {
      const href = this.getAttribute("href");
      if (href !== "#" && href.startsWith("#")) {
        e.preventDefault();
        const target = document.querySelector(href);
        if (target) {
          target.scrollIntoView({
            behavior: "smooth",
            block: "start",
          });
        }
      }
    });
  });

  // Add loading states to buttons
  document.querySelectorAll(".quick-action-btn").forEach((btn) => {
    btn.addEventListener("click", function () {
      if (!this.href.includes("#")) {
        this.classList.add("loading");
        setTimeout(() => {
          this.classList.remove("loading");
        }, 1000);
      }
    });
  });
});

// Export for use in other scripts if needed
if (typeof module !== "undefined" && module.exports) {
  module.exports = { ResponsiveObserver, DashboardResponsiveManager };
}

