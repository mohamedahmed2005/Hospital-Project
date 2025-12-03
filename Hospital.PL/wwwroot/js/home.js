

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

const myCarousel = document.querySelector("#testimonialCarousel");
const carousel = new bootstrap.Carousel(myCarousel, {
  interval: 5000,
  wrap: true,
});

function animateCounter(element, target, duration) {
  let start = 0;
  target = Number(target);

  const steps = duration / 16;   // roughly 60 FPS
  const increment = target / steps;

  function update() {
    start += increment;

    if (start < target) {
      element.textContent = Math.floor(start).toLocaleString();
      requestAnimationFrame(update);
    } else {
      element.textContent = target.toLocaleString();
    }
  }

  update();
}

document.addEventListener("DOMContentLoaded", () => {
  const counters = document.querySelectorAll(".counter");

  counters.forEach(counter => {
    const target = Number(counter.getAttribute("data-target"));
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
  });
});




