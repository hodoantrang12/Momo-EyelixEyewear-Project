document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');
    const usernameInput = document.getElementById('username');
    const passwordInput = document.getElementById('password');
    const togglePasswordBtn = document.querySelector('.toggle-password');
    const loginBtn = document.querySelector('.btn-login');
    const formMessage = document.getElementById('formMessage');
    const facebookBtn = document.querySelector('.btn-facebook');

    // Showcase Slideshow
    const showcaseImages = document.querySelectorAll('.showcase-image');
    let currentImageIndex = 0;
    let slideInterval;

    // Initialize
    init();
    
    function init() {
        setupEventListeners();
        startShowcaseSlideshow();
        updateLoginButton();
    }

    // =====================================
    // EVENT LISTENERS
    // =====================================
    function setupEventListeners() {
        // Toggle Password Visibility
        if (togglePasswordBtn) {
            togglePasswordBtn.addEventListener('click', togglePasswordVisibility);
        }

        // Input Events
        usernameInput.addEventListener('input', handleInputChange);
        passwordInput.addEventListener('input', handleInputChange);

        // Form Submit - CHỈ validate, KHÔNG xử lý login logic
        loginForm.addEventListener('submit', handleFormSubmit);

        // Facebook Login (nếu có)
        if (facebookBtn) {
            facebookBtn.addEventListener('click', handleFacebookLogin);
        }

        // Enter key in username field
        usernameInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter' && !loginBtn.disabled) {
                passwordInput.focus();
            }
        });
    }

    // =====================================
    // PASSWORD VISIBILITY TOGGLE
    // =====================================
    function togglePasswordVisibility() {
        const type = passwordInput.getAttribute('type');
        const showText = togglePasswordBtn.querySelector('.show-text');
        
        if (type === 'password') {
            passwordInput.setAttribute('type', 'text');
            if (showText) showText.textContent = 'Hide';
        } else {
            passwordInput.setAttribute('type', 'password');
            if (showText) showText.textContent = 'Show';
        }
    }

    // =====================================
    // INPUT HANDLING
    // =====================================
    function handleInputChange() {
        updateLoginButton();
        hideMessage();
    }

    function updateLoginButton() {
        const username = usernameInput.value.trim();
        const password = passwordInput.value.trim();
        
        if (username && password) {
            loginBtn.disabled = false;
        } else {
            loginBtn.disabled = true;
        }
    }

    // =====================================
    // MESSAGE DISPLAY
    // =====================================
    function showMessage(message, type = 'error') {
        formMessage.textContent = message;
        formMessage.className = `form-message ${type}`;
        formMessage.setAttribute('role', 'alert');
        
        // Auto-hide success messages
        if (type === 'success') {
            setTimeout(hideMessage, 3000);
        }
    }

    function hideMessage() {
        formMessage.className = 'form-message';
        formMessage.textContent = '';
    }

    // =====================================
    // FORM VALIDATION (Client-side only)
    // =====================================
    function validateForm(username, password) {
        // Check if username is empty
        if (!username) {
            showMessage('Please enter your username, email, or phone number.');
            usernameInput.focus();
            return false;
        }

        // Check if password is empty
        if (!password) {
            showMessage('Please enter your password.');
            passwordInput.focus();
            return false;
        }

        // Check password length
        if (password.length < 6) {
            showMessage('Password must be at least 6 characters.');
            passwordInput.focus();
            return false;
        }

        return true;
    }

    // =====================================
    // FORM SUBMIT - Chỉ validate, không xử lý login
    // =====================================
    function handleFormSubmit(e) {
        const username = usernameInput.value.trim();
        const password = passwordInput.value.trim();

        // Client-side validation
        if (!validateForm(username, password)) {
            e.preventDefault(); // Prevent submit nếu validation fail
            return;
        }

        // Nếu validation pass, form sẽ tự submit lên Server
        // Server (AccountController) sẽ xử lý logic đăng nhập
        setLoadingState(true);
    }

    // =====================================
    // LOADING STATE
    // =====================================
    function setLoadingState(isLoading) {
        if (isLoading) {
            loginBtn.classList.add('loading');
            loginBtn.disabled = true;
            usernameInput.disabled = true;
            passwordInput.disabled = true;
        } else {
            loginBtn.classList.remove('loading');
            updateLoginButton();
            usernameInput.disabled = false;
            passwordInput.disabled = false;
        }
    }

    // =====================================
    // FACEBOOK LOGIN (Placeholder)
    // =====================================
    function handleFacebookLogin(e) {
        e.preventDefault();
        console.log('Facebook login clicked');
        showMessage('This feature is coming soon.', 'error');
    }

    // =====================================
    // SHOWCASE SLIDESHOW
    // =====================================
    function startShowcaseSlideshow() {
        if (showcaseImages.length > 1) {
            slideInterval = setInterval(rotateShowcaseImages, 4000);
        }
    }

    function rotateShowcaseImages() {
        showcaseImages[currentImageIndex].classList.remove('active');
        currentImageIndex = (currentImageIndex + 1) % showcaseImages.length;
        showcaseImages[currentImageIndex].classList.add('active');
    }

    // =====================================
    // CLEANUP
    // =====================================
    window.addEventListener('beforeunload', function() {
        if (slideInterval) {
            clearInterval(slideInterval);
        }
    });

    // Handle browser back button
    window.addEventListener('pageshow', function(event) {
        if (event.persisted) {
            setLoadingState(false);
            hideMessage();
        }
    });
});

// =====================================
// UTILITY FUNCTIONS
// =====================================
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