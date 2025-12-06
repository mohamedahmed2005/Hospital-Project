// Get current language from index.js or default to 'en'
function getCurrentLanguage() {
    // Try to get from index.js if available
    if (typeof currentLanguage !== 'undefined') {
        return currentLanguage;
    }
    // Fallback to localStorage
    return localStorage.getItem('language') === 'ar' ? 'ar' : 'en';
}

// Use showAlert from index.js if available, otherwise define our own
function showAlert(message, type = 'success') {
    // Check if showAlert exists from index.js
    if (typeof window.showAlert === 'function') {
        window.showAlert(message, type);
        return;
    }
    
    // Fallback implementation
    let alertMessage = document.getElementById('alertMessage');
    
    // Create element if it doesn't exist
    if (!alertMessage) {
        alertMessage = document.createElement('div');
        alertMessage.id = 'alertMessage';
        alertMessage.className = 'alert-message';
        document.body.insertBefore(alertMessage, document.body.firstChild);
    }
    
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

// Global function for copying address (can be called from onclick)
// Attach to window to ensure global scope
window.copyAddressToClipboard = function(event) {
    try {
        
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }
        
        const addressElement = document.getElementById('hospitalAddress');
        const address = addressElement ? addressElement.textContent.trim() : '123 Medical Drive, Health City, HC 12345';
        const copyIcon = document.getElementById('copyIcon');
        const copyText = document.getElementById('copyText');
        const copyTextAr = document.getElementById('copyTextAr');
            
    // Function to update button visual feedback
    function updateButtonFeedback(success) {
        if (success) {
            if (copyIcon) {
                copyIcon.className = 'bi bi-check-circle me-1';
            }
            const originalTextEn = copyText ? copyText.textContent : 'Copy Address';
            const originalTextAr = copyTextAr ? copyTextAr.textContent : 'نسخ العنوان';
            
            // Get current language
            const lang = getCurrentLanguage();
            
            if (lang === 'en' && copyText) {
                copyText.textContent = 'Copied!';
            } else if (lang === 'ar' && copyTextAr) {
                copyTextAr.textContent = 'تم النسخ!';
            }
            
            // Reset button after 2 seconds
            setTimeout(() => {
                if (copyIcon) {
                    copyIcon.className = 'bi bi-clipboard me-1';
                }
                if (copyText) {
                    copyText.textContent = originalTextEn;
                }
                if (copyTextAr) {
                    copyTextAr.textContent = originalTextAr;
                }
            }, 2000);
        }
    }
    
    // Function to show message
    function showMessage(message, type) {
        const alertMessage = document.getElementById('alertMessage');
        if (alertMessage) {
            showAlert(message, type);
        } else {
            // Fallback: use alert if element doesn't exist
            alert(message);
        }
    }
    
    // Try modern clipboard API first
    if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(address).then(() => {
            updateButtonFeedback(true);
            const lang = typeof currentLanguage !== 'undefined' ? currentLanguage : 'en';
            const message = lang === 'en'
                ? 'Address copied to clipboard!'
                : 'تم نسخ العنوان إلى الحافظة!';
            showMessage(message, 'success');
        }).catch(err => {
            console.error('Clipboard API failed:', err);
            // Fallback to execCommand
            copyWithFallback();
        });
    } else {
        // Fallback for older browsers
        copyWithFallback();
    }
    
    function copyWithFallback() {
        const textArea = document.createElement('textarea');
        textArea.value = address;
        textArea.style.position = 'fixed';
        textArea.style.top = '0';
        textArea.style.left = '0';
        textArea.style.width = '2em';
        textArea.style.height = '2em';
        textArea.style.padding = '0';
        textArea.style.border = 'none';
        textArea.style.outline = 'none';
        textArea.style.boxShadow = 'none';
        textArea.style.background = 'transparent';
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        
        try {
            const successful = document.execCommand('copy');
            if (successful) {
                updateButtonFeedback(true);
                const lang = typeof currentLanguage !== 'undefined' ? currentLanguage : 'en';
                const message = lang === 'en'
                    ? 'Address copied to clipboard!'
                    : 'تم نسخ العنوان إلى الحافظة!';
                showMessage(message, 'success');
            } else {
                throw new Error('execCommand failed');
            }
        } catch (err) {
            console.error('Copy failed:', err);
            const lang = typeof currentLanguage !== 'undefined' ? currentLanguage : 'en';
            const message = lang === 'en'
                ? 'Failed to copy. Please copy manually: ' + address
                : 'فشل النسخ. يرجى النسخ يدوياً: ' + address;
            showMessage(message, 'danger');
        } finally {
            document.body.removeChild(textArea);
        }
    }
    } catch (error) {
        console.error('Error in copyAddressToClipboard:', error);
        alert('An error occurred while copying the address. Please try again or copy manually.');
    }
};

function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return false;
    }
    return true;
}

document.addEventListener('DOMContentLoaded', function () {
    const copyAddressBtn = document.getElementById('copyAddressBtn');
    
    
    if (copyAddressBtn) {
        // Add event listener for copy button
        copyAddressBtn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            
            // Use window.copyAddressToClipboard to ensure we're calling the global function
            if (window.copyAddressToClipboard) {
                window.copyAddressToClipboard(e);
            } else {
                console.error('copyAddressToClipboard function not found!');
                alert('Copy function not available. Please refresh the page.');
            }
        });
    } else {
        console.error('Copy address button not found!');
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

                const message = getCurrentLanguage() === 'en'
                    ? 'Thank you for your message! We will get back to you within 24 hours.'
                    : 'شكرًا لك على رسالتك! سوف نعود إليك في غضون 24 ساعة.';
                showAlert(message, 'success');

                this.reset();
                this.classList.remove('was-validated');
            }
        });
    }

    // Fix active link detection - match by pathname properly
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href) {
            // Normalize both paths for comparison (remove leading/trailing slashes)
            const hrefPath = href.toLowerCase().replace(/^\/|\/$/g, '');
            const currentPathNormalized = currentPath.replace(/^\/|\/$/g, '');
            
            // Check if current path matches the href path
            // Handles cases like /Home/Contact matching href="/Home/Contact"
            if (currentPathNormalized === hrefPath || 
                currentPathNormalized.endsWith('/' + hrefPath) ||
                currentPathNormalized === hrefPath.split('/').pop()) {
                link.classList.add('active');
            } else {
                link.classList.remove('active');
            }
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