// Password Strength Validator
function validatePasswordStrength(password) {
    const requirements = {
        length: password.length >= 8,
        uppercase: /[A-Z]/.test(password),
        lowercase: /[a-z]/.test(password),
        number: /\d/.test(password),
        special: /[@$!%*?&#]/.test(password)
    };

    const strength = Object.values(requirements).filter(Boolean).length;

    return {
        requirements,
        strength,
        isValid: strength === 5
    };
}

function getPasswordStrengthLabel(strength) {
    switch (strength) {
        case 0:
        case 1:
            return { text: 'Very Weak', color: '#dc3545', width: '20%' };
        case 2:
            return { text: 'Weak', color: '#fd7e14', width: '40%' };
        case 3:
            return { text: 'Fair', color: '#ffc107', width: '60%' };
        case 4:
            return { text: 'Good', color: '#20c997', width: '80%' };
        case 5:
            return { text: 'Strong', color: '#28a745', width: '100%' };
        default:
            return { text: '', color: '#e9ecef', width: '0%' };
    }
}

// Initialize password strength indicator
function initPasswordStrengthIndicator(passwordInputId, containerId) {
    const passwordInput = document.getElementById(passwordInputId);
    const container = document.getElementById(containerId);

    if (!passwordInput || !container) return;

    passwordInput.addEventListener('input', function () {
        const password = this.value;
        const validation = validatePasswordStrength(password);
        const strengthInfo = getPasswordStrengthLabel(validation.strength);

        // Update strength bar
        const strengthBar = container.querySelector('.password-strength-bar');
        const strengthText = container.querySelector('.password-strength-text');

        if (strengthBar && strengthText) {
            strengthBar.style.width = strengthInfo.width;
            strengthBar.style.backgroundColor = strengthInfo.color;
            strengthText.textContent = strengthInfo.text;
            strengthText.style.color = strengthInfo.color;
        }

        // Update requirements checklist
        updateRequirementStatus(container, 'length', validation.requirements.length);
        updateRequirementStatus(container, 'uppercase', validation.requirements.uppercase);
        updateRequirementStatus(container, 'lowercase', validation.requirements.lowercase);
        updateRequirementStatus(container, 'number', validation.requirements.number);
        updateRequirementStatus(container, 'special', validation.requirements.special);

        // Show/hide container based on input focus
        if (password.length > 0) {
            container.style.display = 'block';
        }
    });

    passwordInput.addEventListener('blur', function () {
        // Keep showing if password is not empty
        if (this.value.length === 0) {
            container.style.display = 'none';
        }
    });

    passwordInput.addEventListener('focus', function () {
        if (this.value.length > 0) {
            container.style.display = 'block';
        }
    });
}

function updateRequirementStatus(container, requirementName, isMet) {
    const element = container.querySelector(`[data-requirement="${requirementName}"]`);
    if (element) {
        const icon = element.querySelector('i');
        if (isMet) {
            element.classList.remove('text-muted');
            element.classList.add('text-success');
            if (icon) {
                icon.classList.remove('bi-circle');
                icon.classList.add('bi-check-circle-fill');
            }
        } else {
            element.classList.remove('text-success');
            element.classList.add('text-muted');
            if (icon) {
                icon.classList.remove('bi-check-circle-fill');
                icon.classList.add('bi-circle');
            }
        }
    }
}

// Phone Number Validation
function validateEgyptianPhoneNumber(phoneNumber) {
    // Remove spaces, dashes, and parentheses
    const cleaned = phoneNumber.replace(/[\s\-\(\)]/g, '');

    // Check if it matches Egyptian phone pattern (11 digits starting with 01)
    const pattern = /^01[0-2,5]{1}[0-9]{8}$/;
    return pattern.test(cleaned);
}

// Initialize phone number validation
function initPhoneNumberValidation(phoneInputId) {
    const phoneInput = document.getElementById(phoneInputId);
    if (!phoneInput) return;

    phoneInput.addEventListener('input', function () {
        const value = this.value;
        const isValid = validateEgyptianPhoneNumber(value);

        if (value.length > 0) {
            if (isValid) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        } else {
            this.classList.remove('is-valid', 'is-invalid');
        }
    });
}

// Name Validation (letters and spaces only)
function validateName(name) {
    // Supports English and Arabic letters
    const pattern = /^[a-zA-Z\u0600-\u06FF\s]+$/;
    return pattern.test(name) && name.trim().length >= 2;
}

// Initialize name validation
function initNameValidation(nameInputId) {
    const nameInput = document.getElementById(nameInputId);
    if (!nameInput) return;

    nameInput.addEventListener('input', function () {
        const value = this.value;
        const isValid = validateName(value);

        if (value.length > 0) {
            if (isValid) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        } else {
            this.classList.remove('is-valid', 'is-invalid');
        }
    });

    // Prevent numbers and special characters
    nameInput.addEventListener('keypress', function (e) {
        const char = String.fromCharCode(e.which);
        if (!/^[a-zA-Z\u0600-\u06FF\s]$/.test(char)) {
            e.preventDefault();
        }
    });
}

// Email Validation
function validateEmail(email) {
    // Standard email pattern
    const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return pattern.test(email);
}

// Initialize email validation
function initEmailValidation(emailInputId) {
    const emailInput = document.getElementById(emailInputId);
    if (!emailInput) return;

    emailInput.addEventListener('input', function () {
        const value = this.value;
        const isValid = validateEmail(value);

        if (value.length > 0) {
            if (isValid) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        } else {
            this.classList.remove('is-valid', 'is-invalid');
        }
    });
}
