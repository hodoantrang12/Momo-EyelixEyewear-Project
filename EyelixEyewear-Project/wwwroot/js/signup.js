document.addEventListener('DOMContentLoaded', function() {
    const signupForm = document.getElementById('signupForm');
    const emailInput = document.getElementById('email');
    const fullnameInput = document.getElementById('fullname');
    const usernameInput = document.getElementById('username');
    const passwordInput = document.getElementById('password');
    const togglePasswordBtn = document.getElementById('togglePassword');
    const submitBtn = document.querySelector('.btn-signup');
    const facebookBtn = document.querySelector('.btn-facebook');
    const formMessage = document.getElementById('formMessage');

    // =====================================
    // PASSWORD VISIBILITY TOGGLE
    // =====================================
    if (togglePasswordBtn) {
        togglePasswordBtn.addEventListener('click', function() {
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                togglePasswordBtn.textContent = 'Hide';
            } else {
                passwordInput.type = 'password';
                togglePasswordBtn.textContent = 'Show';
            }
        });
    }

    // =====================================
    // REAL-TIME VALIDATION
    // =====================================
    function validateForm() {
        const email = emailInput.value.trim();
        const fullname = fullnameInput.value.trim();
        const username = usernameInput.value.trim();
        const password = passwordInput.value;
        
        // Enable button only if all fields are valid
        if (email && fullname && username && password.length >= 6) {
            submitBtn.disabled = false;
        } else {
            submitBtn.disabled = true;
        }
    }

    // Add input event listeners for real-time validation
    document.querySelectorAll('.form-input').forEach(input => {
        input.addEventListener('input', function() {
            validateForm();
            hideMessage();
        });
    });

    // =====================================
    // FORM SUBMIT - Client-side validation only
    // =====================================
    signupForm.addEventListener('submit', function(e) {
        const email = emailInput.value.trim();
        const fullname = fullnameInput.value.trim();
        const username = usernameInput.value.trim();
        const password = passwordInput.value;

        // Detailed validation before submit
        if (!email) {
            e.preventDefault();
            showMessage('Please enter your email address.', 'error');
            emailInput.focus();
            return;
        }

        if (!isValidEmail(email)) {
            e.preventDefault();
            showMessage('Please enter a valid email address.', 'error');
            emailInput.focus();
            return;
        }

        if (!fullname) {
            e.preventDefault();
            showMessage('Please enter your full name.', 'error');
            fullnameInput.focus();
            return;
        }

        if (!username) {
            e.preventDefault();
            showMessage('Please enter a username.', 'error');
            usernameInput.focus();
            return;
        }

        if (username.length < 3) {
            e.preventDefault();
            showMessage('Username must be at least 3 characters.', 'error');
            usernameInput.focus();
            return;
        }

        if (!password) {
            e.preventDefault();
            showMessage('Please enter a password.', 'error');
            passwordInput.focus();
            return;
        }

        if (password.length < 6) {
            e.preventDefault();
            showMessage('Password must be at least 6 characters.', 'error');
            passwordInput.focus();
            return;
        }

        // Nếu validation pass, form sẽ submit lên Server
        // Server (AccountController) sẽ xử lý logic đăng ký
        setLoadingState(true);
    });

    // =====================================
    // MESSAGE DISPLAY
    // =====================================
    function showMessage(message, type = 'error') {
        if (!formMessage) return;
        
        formMessage.textContent = message;
        formMessage.className = `form-message ${type}`;
        formMessage.style.display = 'block';
        
        if (type === 'success') {
            setTimeout(hideMessage, 3000);
        }
    }

    function hideMessage() {
        if (formMessage) {
            formMessage.style.display = 'none';
            formMessage.textContent = '';
        }
    }

    // =====================================
    // LOADING STATE
    // =====================================
    function setLoadingState(isLoading) {
        if (isLoading) {
            submitBtn.disabled = true;
            submitBtn.classList.add('loading');
            emailInput.disabled = true;
            fullnameInput.disabled = true;
            usernameInput.disabled = true;
            passwordInput.disabled = true;
        } else {
            submitBtn.classList.remove('loading');
            emailInput.disabled = false;
            fullnameInput.disabled = false;
            usernameInput.disabled = false;
            passwordInput.disabled = false;
            validateForm();
        }
    }

    // =====================================
    // FACEBOOK LOGIN (Placeholder)
    // =====================================
    if (facebookBtn) {
        facebookBtn.addEventListener('click', function(e) {
            e.preventDefault();
            console.log('Facebook login clicked');
            showMessage('This feature is coming soon.', 'error');
        });
    }

    // =====================================
    // UTILITY FUNCTIONS
    // =====================================
    function isValidEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    }

    // Initial validation check
    validateForm();

    // Handle browser back button
    window.addEventListener('pageshow', function(event) {
        if (event.persisted) {
            setLoadingState(false);
            hideMessage();
        }
    });
});