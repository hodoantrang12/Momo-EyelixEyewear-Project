// ==========================================
// CHANGE MAIN IMAGE (Thumbnail click)
// ==========================================
function changeImage(imagePath, element) {
    const mainImage = document.getElementById('mainImage');
    if (!mainImage) return;
    const src = imagePath.startsWith('/') ? imagePath : '/' + imagePath;

    // Smooth transition effect
    mainImage.style.opacity = '0.5';
    setTimeout(() => {
        mainImage.src = src;
        mainImage.style.opacity = '1';
    }, 150);

    // Update active thumbnail
    document.querySelectorAll('.thumbnail').forEach(thumb => {
        thumb.classList.remove('active');
    });
    element.classList.add('active');
}

// ==========================================
// COLOR SELECTION
// ==========================================
function selectColor(element, newImagePath) {
    // Update active color option
    document.querySelectorAll('.color-option').forEach(color => {
        color.classList.remove('active');
    });
    element.classList.add('active');

    // Change main image if new image is provided
    if (newImagePath) {
        const mainImage = document.getElementById('mainImage');
        if (mainImage) {
            // ✅ Đảm bảo đường dẫn bắt đầu từ /
            const src = newImagePath.startsWith('/') ? newImagePath : '/' + newImagePath;

            mainImage.style.opacity = '0.5';
            setTimeout(() => {
                mainImage.src = src;
                mainImage.style.opacity = '1';
            }, 150);
        }
    }

    // Reset thumbnail active state
    document.querySelectorAll('.thumbnail').forEach(thumb => {
        thumb.classList.remove('active');
    });

    // Set first thumbnail as active
    const firstThumbnail = document.querySelector('.thumbnail');
    if (firstThumbnail) {
        firstThumbnail.classList.add('active');
    }
}

// ==========================================
// QUANTITY CONTROLS
// ==========================================
function increaseQuantity() {
    const input = document.getElementById('quantity');
    if (!input) return;

    const currentValue = parseInt(input.value) || 1;
    const maxValue = parseInt(input.getAttribute('max')) || 999;

    if (currentValue < maxValue) {
        input.value = currentValue + 1;
    }
}

function decreaseQuantity() {
    const input = document.getElementById('quantity');
    if (!input) return;

    const currentValue = parseInt(input.value) || 1;
    const minValue = parseInt(input.getAttribute('min')) || 1;

    if (currentValue > minValue) {
        input.value = currentValue - 1;
    }
}

// ==========================================
// ADD TO CART (Submit to Server)
// ==========================================
function addToCart() {
    const productId = document.getElementById('product-id')?.value;
    const quantity = document.getElementById('quantity')?.value || 1;
    const selectedColor = document.querySelector('.color-option.active')?.getAttribute('data-color');

    if (!productId) {
        alert('Product not found!');
        return;
    }

    // Create form data
    const formData = new FormData();
    formData.append('productId', productId);
    formData.append('quantity', quantity);
    if (selectedColor) {
        formData.append('color', selectedColor);
    }

    // Get CSRF token
    const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]');
    if (csrfToken) {
        formData.append('__RequestVerificationToken', csrfToken.value);
    }

    // Show loading state
    const addToCartBtn = document.querySelector('.btn-add-to-cart');
    if (addToCartBtn) {
        addToCartBtn.disabled = true;
        const originalText = addToCartBtn.textContent;
        addToCartBtn.textContent = 'Adding...';

        // Send to server
        fetch('/Cart/AddToCart', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showNotification(data.message || 'Product added to cart!', 'success');

                    // Update cart count
                    if (window.updateCartCount) {
                        window.updateCartCount();
                    }

                    // Update cart badge
                    const cartBadge = document.querySelector('.cart-count');
                    if (cartBadge && data.cartCount) {
                        cartBadge.textContent = data.cartCount;
                        cartBadge.style.display = 'flex';
                    }
                } else {
                    showNotification(data.message || 'Failed to add to cart', 'error');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                showNotification('An error occurred. Please try again.', 'error');
            })
            .finally(() => {
                if (addToCartBtn) {
                    addToCartBtn.disabled = false;
                    addToCartBtn.textContent = originalText;
                }
            });
    }
}

// ==========================================
// TAB SWITCHING (Description, Reviews, etc.)
// ==========================================
function openTab(evt, tabName) {
    // Hide all tab contents
    const tabContents = document.querySelectorAll('.tab-content');
    tabContents.forEach(content => {
        content.classList.remove('active');
    });

    // Remove active class from all buttons
    const tabButtons = document.querySelectorAll('.tab-btn');
    tabButtons.forEach(btn => {
        btn.classList.remove('active');
    });

    // Show selected tab and mark button as active
    const selectedTab = document.getElementById(tabName);
    if (selectedTab) {
        selectedTab.classList.add('active');
    }

    if (evt && evt.currentTarget) {
        evt.currentTarget.classList.add('active');
    }
}

// ==========================================
// NOTIFICATION HELPER
// ==========================================
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = 'notification';
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${type === 'success' ? '#28a745' : '#dc3545'};
        color: white;
        padding: 15px 25px;
        border-radius: 5px;
        z-index: 10000;
        box-shadow: 0 2px 8px rgba(0,0,0,0.2);
        animation: slideInRight 0.3s ease;
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// ==========================================
// IMAGE ZOOM ON HOVER (Optional enhancement)
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    const mainImage = document.getElementById('mainImage');
    if (!mainImage) return;

    const imageContainer = mainImage.parentElement;

    imageContainer.addEventListener('mousemove', (e) => {
        const rect = imageContainer.getBoundingClientRect();
        const x = ((e.clientX - rect.left) / rect.width) * 100;
        const y = ((e.clientY - rect.top) / rect.height) * 100;

        mainImage.style.transformOrigin = `${x}% ${y}%`;
        mainImage.style.transform = 'scale(1.5)';
    });

    imageContainer.addEventListener('mouseleave', () => {
        mainImage.style.transform = 'scale(1)';
    });
});

// ==========================================
// QUANTITY INPUT VALIDATION
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    const quantityInput = document.getElementById('quantity');
    if (!quantityInput) return;

    quantityInput.addEventListener('input', () => {
        const value = parseInt(quantityInput.value);
        const min = parseInt(quantityInput.getAttribute('min')) || 1;
        const max = parseInt(quantityInput.getAttribute('max')) || 999;

        if (value < min) {
            quantityInput.value = min;
        } else if (value > max) {
            quantityInput.value = max;
        }
    });

    // Prevent non-numeric input
    quantityInput.addEventListener('keypress', (e) => {
        if (e.key && !/[0-9]/.test(e.key)) {
            e.preventDefault();
        }
    });
});