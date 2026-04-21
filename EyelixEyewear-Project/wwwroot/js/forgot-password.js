// Forgot Password Page JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Form Elements
    const forgotForm = document.getElementById('forgotForm');
    const identifierInput = document.getElementById('identifier');
    const sendBtn = document.querySelector('.btn-send');
    const formMessage = document.getElementById('formMessage');

    // Initialize
    init();

    function init() {
        setupEventListeners();
        updateSendButton();
    }

    // Setup Event Listeners
    function setupEventListeners() {
        // Input Events
        identifierInput.addEventListener('input', handleInputChange);

        // Form Submit
        forgotForm.addEventListener('submit', handleFormSubmit);

        // Prevent form submission on Enter
        identifierInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter' && !sendBtn.disabled) {
                e.preventDefault();
                forgotForm.dispatchEvent(new Event('submit'));
            }
        });
    }

    // Handle Input Change
    function handleInputChange() {
        updateSendButton();
        hideMessage();
    }

    // Update Send Button State
    function updateSendButton() {
        const identifier = identifierInput.value.trim();
        
        if (identifier) {
            sendBtn.disabled = false;
        } else {
            sendBtn.disabled = true;
        }
    }

    // Show Message
    function showMessage(message, type = 'error') {
        formMessage.textContent = message;
        formMessage.className = `form-message ${type}`;
        formMessage.setAttribute('role', 'alert');
        
        // Auto-hide messages after 5 seconds
        setTimeout(hideMessage, 5000);
    }

    // Hide Message
    function hideMessage() {
        formMessage.className = 'form-message';
        formMessage.textContent = '';
    }

    // Form Validation
    function validateForm(identifier) {
        // Check if identifier is empty
        if (!identifier) {
            showMessage('Please enter your email, phone, or username.');
            identifierInput.focus();
            return false;
        }

        // Check if it's a valid format (email, phone, or username)
        if (identifier.length < 3) {
            showMessage('Please enter a valid email, phone, or username.');
            identifierInput.focus();
            return false;
        }

        return true;
    }

    // Validate Identifier Type
    function validateIdentifier(identifier) {
        // Check if it's an email
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (emailRegex.test(identifier)) {
            return { type: 'email', valid: true };
        }

        // Check if it's a phone number
        const phoneRegex = /^[\d\s\-\+\(\)]+$/;
        if (phoneRegex.test(identifier) && identifier.replace(/\D/g, '').length >= 10) {
            return { type: 'phone', valid: true };
        }

        // Check if it's a username
        const usernameRegex = /^[a-zA-Z0-9._]+$/;
        if (usernameRegex.test(identifier) && identifier.length >= 3) {
            return { type: 'username', valid: true };
        }

        return { type: 'unknown', valid: false };
    }

    // Handle Form Submit
    async function handleFormSubmit(e) {
        e.preventDefault();
        
        const identifier = identifierInput.value.trim();

        // Validate form
        if (!validateForm(identifier)) {
            return;
        }

        // Validate identifier type
        const validation = validateIdentifier(identifier);
        if (!validation.valid) {
            showMessage('Please enter a valid email, phone number, or username.');
            identifierInput.focus();
            return;
        }

        // Show loading state
        setLoadingState(true);

        try {
            // Simulate API call
            const response = await sendResetLink(identifier);
            
            if (response.success) {
                // Success
                let successMessage = '';
                if (validation.type === 'email') {
                    successMessage = `We've sent a password reset link to ${identifier}. Please check your email.`;
                } else if (validation.type === 'phone') {
                    successMessage = `We've sent a password reset code to your phone. Please check your messages.`;
                } else {
                    successMessage = `We've sent a password reset link to the email associated with this account.`;
                }
                
                showMessage(successMessage, 'success');
                
                // Clear input after success
                setTimeout(() => {
                    identifierInput.value = '';
                    updateSendButton();
                }, 3000);
            } else {
                throw new Error(response.message || 'Failed to send reset link');
            }

        } catch (error) {
            // Error
            console.error('Forgot password error:', error);
            showMessage(error.message || 'Sorry, there was a problem with your request. Please try again.');
        } finally {
            setLoadingState(false);
        }
    }

    // Set Loading State
    function setLoadingState(isLoading) {
        if (isLoading) {
            sendBtn.classList.add('loading');
            sendBtn.disabled = true;
            identifierInput.disabled = true;
        } else {
            sendBtn.classList.remove('loading');
            updateSendButton();
            identifierInput.disabled = false;
        }
    }

    // Send Reset Link (API Call Simulation)
    function sendResetLink(identifier) {
        return new Promise((resolve, reject) => {
            // Simulate network delay
            setTimeout(() => {                
                const validation = validateIdentifier(identifier);
                
                if (validation.valid) {
                    resolve({
                        success: true,
                        message: 'Reset link sent successfully',
                        identifier: identifier,
                        type: validation.type
                    });
                } else {
                    reject(new Error('No account found with this information.'));
                }
            }, 2000);
        });
    }

    // Handle browser back button
    window.addEventListener('pageshow', function(event) {
        if (event.persisted) {
            setLoadingState(false);
            hideMessage();
        }
    });
});

// Utility Functions
function isValidEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

function isValidPhone(phone) {
    const re = /^[\d\s\-\+\(\)]+$/;
    return re.test(phone) && phone.replace(/\D/g, '').length >= 10;
}

function isValidUsername(username) {
    return username.length >= 3 && /^[a-zA-Z0-9._]+$/.test(username);
}

// Export for testing (if needed)
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        isValidEmail,
        isValidPhone,
        isValidUsername
    };

}
