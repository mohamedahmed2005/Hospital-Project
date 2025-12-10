// Counter animation for About page - same as home.js
// Note: Language switcher, dark mode, and fade-in are handled by index.js in the layout

document.addEventListener("DOMContentLoaded", function () {
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll(".nav-link");

    navLinks.forEach(link => {
        const href = link.getAttribute("href").toLowerCase();
        if (href === currentPath || href === currentPath.split("/").pop()) {
            link.classList.add("active");
        } else {
            link.classList.remove("active");
        }
    });

    animateCounters();
});


// Also run on window load as fallback
window.addEventListener("load", function () {
    animateCounters();
});

function animateCounters() {
    const counters = document.querySelectorAll(".counter");

    if (counters.length > 0) {
        counters.forEach(counter => {
            // Skip if already animated (has a number > 0 and not starting with 0)
            const currentText = counter.textContent.trim();
            if (currentText !== "0" && !isNaN(Number(currentText)) && Number(currentText) > 0) {
                return; // Already animated
            }

            const targetAttr = counter.getAttribute("data-target");
            const target = Number(targetAttr);

            // Only animate if target is a valid number and greater than 0
            if (!isNaN(target) && target > 0) {
                let value = 0;
                const step = target / 100; // 100 updates

                function update() {
                    value += step;

                    if (value < target) {
                        counter.textContent = Math.floor(value);
                        requestAnimationFrame(update);
                    } else {
                        counter.textContent = target;
                    }
                }

                update();
            } else {
                // If target is invalid, just show the current value or 0
                counter.textContent = targetAttr || "0";
            }
        });
    }
}
