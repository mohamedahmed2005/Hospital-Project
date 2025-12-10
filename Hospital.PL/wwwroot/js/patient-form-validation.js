// Patient Form Real-Time Validation
// Validates Phone Number and Email fields in real-time

document.addEventListener('DOMContentLoaded', function () {
    // Initialize validation for Phone Number field
    const phoneInput = document.querySelector('input[name="PhoneNumber"]');
    if (phoneInput) {
        // Add ID if not present
        if (!phoneInput.id) {
            phoneInput.id = 'PhoneNumber';
        }
        initPhoneNumberValidation('PhoneNumber');
    }

    // Initialize validation for Email field
    const emailInput = document.querySelector('input[name="Email"]');
    if (emailInput) {
        // Add ID if not present
        if (!emailInput.id) {
            emailInput.id = 'Email';
        }
        initEmailValidation('Email');
    }
});
