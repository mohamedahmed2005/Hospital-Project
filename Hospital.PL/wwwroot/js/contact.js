const languageSwitcher = document.getElementById('languageSwitcher');
const body = document.body;
let currentLanguage = 'en';

if (localStorage.getItem('language') === 'ar') {
    switchToArabic();
}

languageSwitcher.addEventListener('click', () => {
    if (currentLanguage === 'en') {
        switchToArabic();
    } else {
        switchToEnglish();
    }
});

function switchToArabic() {
    document.querySelectorAll('.en-text').forEach(el => {
        el.style.display = 'none';
    });
    document.querySelectorAll('.ar-text').forEach(el => {
        el.style.display = 'inline';
    });

    document.querySelectorAll('option[data-ar]').forEach(el => {
        el.textContent = el.dataset.ar;
    });

    body.classList.add('rtl');
    body.setAttribute('dir', 'rtl');
    document.documentElement.setAttribute('lang', 'ar');
    document.documentElement.setAttribute('dir', 'rtl');

    languageSwitcher.textContent = 'EN';

    currentLanguage = 'ar';

    localStorage.setItem('language', 'ar');
}

function switchToEnglish() {
    document.querySelectorAll('.ar-text').forEach(el => {
        el.style.display = 'none';
    });
    document.querySelectorAll('.en-text').forEach(el => {
        el.style.display = 'inline';
    });

    document.querySelectorAll('option[data-en]').forEach(el => {
        el.textContent = el.dataset.en;
    });

    body.classList.remove('rtl');
    body.setAttribute('dir', 'ltr');
    document.documentElement.setAttribute('lang', 'en');
    document.documentElement.setAttribute('dir', 'ltr');

    languageSwitcher.textContent = 'ع';

    currentLanguage = 'en';

    localStorage.setItem('language', 'en');
}

const darkModeToggle = document.getElementById('darkModeToggle');

if (localStorage.getItem('theme') === 'dark' ||
    (window.matchMedia('(prefers-color-scheme: dark)').matches && !localStorage.getItem('theme'))) {
    body.classList.add('dark-mode');
    darkModeToggle.innerHTML = '<i class="bi bi-sun"></i>';
}

darkModeToggle.addEventListener('click', () => {
    body.classList.toggle('dark-mode');

    if (body.classList.contains('dark-mode')) {
        darkModeToggle.innerHTML = '<i class="bi bi-sun"></i>';
        localStorage.setItem('theme', 'dark');
    } else {
        darkModeToggle.innerHTML = '<i class="bi bi-moon"></i>';
        localStorage.setItem('theme', 'light');
    }
});

function showAlert(message, type = 'success') {
    const alertMessage = document.getElementById('alertMessage');
    alertMessage.innerHTML = `
    <div class="alert alert-${type} alert-dismissible fade show" role="alert">
      ${message}
      <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    </div>
  `;
    alertMessage.classList.add('show');

    setTimeout(() => {
        alertMessage.classList.remove('show');
    }, 5000);
}

function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return false;
    }
    return true;
}

document.addEventListener('DOMContentLoaded', function () {
    const copyAddressBtn = document.querySelector('.btn-outline-primary');
    if (copyAddressBtn) {
        copyAddressBtn.addEventListener('click', function () {
            const address = '123 Medical Drive, Health City, HC 12345';
            navigator.clipboard.writeText(address).then(() => {
                const message = currentLanguage === 'en'
                    ? 'Address copied to clipboard!'
                    : 'تم نسخ العنوان إلى الحافظة!';
                showAlert(message, 'success');
            });
        });
    }

    const contactForm = document.querySelector('form.needs-validation');
    if (contactForm) {
        contactForm.addEventListener('submit', function (e) {
            e.preventDefault();

            if (validateForm('contactForm')) {
                const formData = {
                    firstName: this.querySelector('input[type="text"]').value,
                    lastName: this.querySelectorAll('input[type="text"]')[1].value,
                    email: this.querySelector('input[type="email"]').value,
                    phone: this.querySelector('input[type="tel"]').value,
                    department: this.querySelector('select').value,
                    subject: this.querySelectorAll('input[type="text"]')[2].value,
                    message: this.querySelector('textarea').value,
                    urgent: this.querySelector('input[type="checkbox"]').checked
                };

                console.log('Form submitted:', formData);

                const message = currentLanguage === 'en'
                    ? 'Thank you for your message! We will get back to you within 24 hours.'
                    : 'شكرًا لك على رسالتك! سوف نعود إليك في غضون 24 ساعة.';
                showAlert(message, 'success');

                this.reset();
                this.classList.remove('was-validated');
            }
        });
    }

    const currentPage = window.location.pathname.split('/').pop();
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPage) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });

    const fadeElements = document.querySelectorAll('.fade-in');

    const fadeInOnScroll = () => {
        fadeElements.forEach(element => {
            const elementTop = element.getBoundingClientRect().top;
            const elementVisible = 150;

            if (elementTop < window.innerHeight - elementVisible) {
                element.classList.add('visible');
            }
        });
    };

    window.addEventListener('load', fadeInOnScroll);
    window.addEventListener('scroll', fadeInOnScroll);
});