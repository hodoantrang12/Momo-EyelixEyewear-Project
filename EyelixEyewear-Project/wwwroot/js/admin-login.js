document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('adminLoginForm');
    const usernameInput = document.getElementById('username');
    const passwordInput = document.getElementById('password');
    const rememberCheckbox = document.getElementById('remember');
    const loginMessage = document.getElementById('loginMessage');
    const loginButton = loginForm.querySelector('.btn-login');
    const btnText = loginButton.querySelector('.btn-text');
    const btnLoader = loginButton.querySelector('.btn-loader');

    // =====================================
    // FORM SUBMIT - Client-side validation only
    // =====================================
    loginForm.addEventListener('submit', function(e) {
        const username = usernameInput.value.trim();
        const password = passwordInput.value;

        // Validate inputs
        if (!username || !password) {
            e.preventDefault();
            showMessage('Please fill in all fields', 'error');
            return;
        }

        if (password.length < 6) {
            e.preventDefault();
            showMessage('Password must be at least 6 characters', 'error');
            return;
        }

        // Show loading state khi submit
        setLoadingState(true);
        hideMessage();

        // Form sẽ tự submit lên Server (AdminController hoặc AccountController)
        // Server sẽ xử lý authentication và redirect
    });

    // =====================================
    // MESSAGE DISPLAY
    // =====================================
    function showMessage(text, type) {
        loginMessage.textContent = text;
        loginMessage.className = `form-message ${type}`;
        loginMessage.style.display = 'block';
    }

    function hideMessage() {
        loginMessage.style.display = 'none';
    }

    // =====================================
    // LOADING STATE
    // =====================================
    function setLoadingState(isLoading) {
        loginButton.disabled = isLoading;
        
        if (isLoading) {
            if (btnText) btnText.style.display = 'none';
            if (btnLoader) btnLoader.style.display = 'inline-flex';
        } else {
            if (btnText) btnText.style.display = 'inline';
            if (btnLoader) btnLoader.style.display = 'none';
        }
    }

    // =====================================
    // REMEMBER ME - Chỉ lưu username
    // =====================================
    // Load saved username if remember me was checked
    const savedUsername = localStorage.getItem('adminUsername');
    const wasRemembered = localStorage.getItem('adminRemember') === 'true';
    
    if (wasRemembered && savedUsername) {
        rememberCheckbox.checked = true;
        usernameInput.value = savedUsername;
    }

    // Save username when form submits nếu remember me được check
    loginForm.addEventListener('submit', function() {
        if (rememberCheckbox.checked) {
            localStorage.setItem('adminRemember', 'true');
            localStorage.setItem('adminUsername', usernameInput.value.trim());
        } else {
            localStorage.removeItem('adminRemember');
            localStorage.removeItem('adminUsername');
        }
    });

    // =====================================
    // PASSWORD FIELD ENHANCEMENTS
    // =====================================
    // Enter key to submit
    passwordInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            loginForm.dispatchEvent(new Event('submit'));
        }
    });

    // =====================================
    // INPUT FIELD ANIMATIONS
    // =====================================
    const formInputs = document.querySelectorAll('.form-input');
    formInputs.forEach(input => {
        // Focus animation
        input.addEventListener('focus', function() {
            this.parentElement.classList.add('focused');
        });
        
        // Blur animation
        input.addEventListener('blur', function() {
            if (this.value === '') {
                this.parentElement.classList.remove('focused');
            }
        });

        // Clear error message on input
        input.addEventListener('input', function() {
            hideMessage();
        });
    });

    // =====================================
    // HANDLE BROWSER BACK BUTTON
    // =====================================
    window.addEventListener('pageshow', function(event) {
        if (event.persisted) {
            setLoadingState(false);
            hideMessage();
        }
    });
});