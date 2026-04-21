// =====================
// Helper Functions
// =====================
function formatPrice(price) {
    return price.toLocaleString('vi-VN');
}

// ==========================================
// UPDATE TOTAL (Calculate shipping)
// ==========================================
function updateTotal() {
    const subtotalEl = document.getElementById('order-subtotal');
    const shippingEl = document.getElementById('order-shipping');
    const totalEl = document.getElementById('order-total');

    if (!subtotalEl || !totalEl) return;

    // Lấy subtotal từ data attribute (đã được server render)
    const subtotal = parseFloat(subtotalEl.getAttribute('data-value') || '0');

    // Lấy giá ship từ data attribute của radio button đã chọn
    const selectedShipping = document.querySelector('input[name="shipping"]:checked');
    const shippingCost = parseFloat(selectedShipping?.getAttribute('data-price') || '0');

    const total = subtotal + shippingCost;

    // Update UI
    if (shippingEl) {
        shippingEl.textContent = formatPrice(shippingCost) + ' VND';
    }
    totalEl.textContent = formatPrice(total) + ' VND';
    totalEl.setAttribute('data-value', total);
}

// ==========================================
// SHIPPING METHOD CHANGE
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    // Shipping change listener
    document.querySelectorAll('input[name="shipping"]').forEach(radio => {
        radio.addEventListener('change', updateTotal);
    });

    // Initial calculation
    updateTotal();
});

// ==========================================
// TOGGLE DIFFERENT SHIPPING ADDRESS
// ==========================================
const diffAddressCheckbox = document.getElementById('different-address');
const diffAddressForm = document.getElementById('shipping-address');

if (diffAddressCheckbox && diffAddressForm) {
    diffAddressCheckbox.addEventListener('change', () => {
        diffAddressForm.style.display = diffAddressCheckbox.checked ? 'block' : 'none';

        // Toggle required attributes
        const inputs = diffAddressForm.querySelectorAll('input, textarea');
        inputs.forEach(input => {
            if (diffAddressCheckbox.checked) {
                input.setAttribute('required', 'required');
            } else {
                input.removeAttribute('required');
            }
        });
    });
}

// ==========================================
// FORM VALIDATION & SUBMISSION
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    const checkoutForm = document.getElementById('checkout-form');
    const placeOrderBtn = document.getElementById('place-order');

    if (checkoutForm && placeOrderBtn) {
        placeOrderBtn.addEventListener('click', (e) => {
            // Client-side validation
            const paymentMethod = document.querySelector('input[name="payment"]:checked');
            if (!paymentMethod) {
                e.preventDefault();
                alert('Please select a payment method.');
                return;
            }

            // Check required fields
            const requiredFields = checkoutForm.querySelectorAll('[required]');
            let isValid = true;
            let firstInvalidField = null;

            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    isValid = false;
                    field.classList.add('error');
                    if (!firstInvalidField) {
                        firstInvalidField = field;
                    }
                } else {
                    field.classList.remove('error');
                }
            });

            if (!isValid) {
                e.preventDefault();
                alert('Please fill in all required fields (*)');
                if (firstInvalidField) {
                    firstInvalidField.focus();
                }
                return;
            }

            // Validate email format
            const emailField = document.getElementById('email');
            if (emailField && emailField.value) {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(emailField.value)) {
                    e.preventDefault();
                    alert('Please enter a valid email address.');
                    emailField.focus();
                    return;
                }
            }

            // Validate phone format (Vietnamese format)
            const phoneField = document.getElementById('phone');
            if (phoneField && phoneField.value) {
                const phoneRegex = /^[0-9]{10,11}$/;
                const cleanPhone = phoneField.value.replace(/[\s\-\(\)]/g, '');
                if (!phoneRegex.test(cleanPhone)) {
                    e.preventDefault();
                    alert('Please enter a valid phone number (10-11 digits).');
                    phoneField.focus();
                    return;
                }
            }

            // Show loading state
            placeOrderBtn.disabled = true;
            placeOrderBtn.textContent = 'Processing...';

            // Form sẽ tự submit lên Server (CheckoutController)
            // Server sẽ xử lý payment, create order, và redirect
        });

        // Real-time validation feedback
        const requiredFields = checkoutForm.querySelectorAll('[required]');
        requiredFields.forEach(field => {
            field.addEventListener('blur', () => {
                if (!field.value.trim()) {
                    field.classList.add('error');
                } else {
                    field.classList.remove('error');
                }
            });

            field.addEventListener('input', () => {
                if (field.value.trim()) {
                    field.classList.remove('error');
                }
            });
        });
    }
});

// ==========================================
// PAYMENT METHOD CHANGE
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    const paymentRadios = document.querySelectorAll('input[name="payment"]');
    const paymentDetails = document.querySelectorAll('.payment-details');

    paymentRadios.forEach(radio => {
        radio.addEventListener('change', () => {
            // Hide all payment details
            paymentDetails.forEach(detail => {
                detail.style.display = 'none';
            });

            // Show selected payment details
            const selectedDetail = document.getElementById(`${radio.value}-details`);
            if (selectedDetail) {
                selectedDetail.style.display = 'block';
            }
        });
    });
});

// ==========================================
// APPLY DISCOUNT CODE
// ==========================================
function applyDiscountCode() {
    const discountInput = document.getElementById('discount-code');
    if (!discountInput) return;

    const discountCode = discountInput.value.trim();
    if (!discountCode) {
        alert('Please enter a discount code');
        return;
    }

    const formData = new FormData();
    formData.append('discountCode', discountCode);

    const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]');
    if (csrfToken) {
        formData.append('__RequestVerificationToken', csrfToken.value);
    }

    // Show loading
    const applyBtn = document.getElementById('apply-discount-btn');
    if (applyBtn) {
        applyBtn.disabled = true;
        applyBtn.textContent = 'Applying...';
    }

    fetch('/Checkout/ApplyDiscount', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert(data.message || 'Discount code applied successfully!');
                window.location.reload();
            } else {
                alert(data.message || 'Invalid discount code');
                if (applyBtn) {
                    applyBtn.disabled = false;
                    applyBtn.textContent = 'Apply';
                }
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred. Please try again.');
            if (applyBtn) {
                applyBtn.disabled = false;
                applyBtn.textContent = 'Apply';
            }
        });
}

// Export function
window.applyDiscountCode = applyDiscountCode;

// ==========================================
// TERMS & CONDITIONS CHECKBOX
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    const termsCheckbox = document.getElementById('terms-checkbox');
    const placeOrderBtn = document.getElementById('place-order');

    if (termsCheckbox && placeOrderBtn) {
        termsCheckbox.addEventListener('change', () => {
            if (!termsCheckbox.checked) {
                placeOrderBtn.disabled = true;
            } else {
                placeOrderBtn.disabled = false;
            }
        });

        // Initial state
        if (!termsCheckbox.checked) {
            placeOrderBtn.disabled = true;
        }
    }
});