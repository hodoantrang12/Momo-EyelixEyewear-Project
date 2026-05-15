// ==========================================
// CART.JS - Shopping Cart Management
// ==========================================

// ==========================================
// 1. ADD TO CART (with Login Check)
// ==========================================
function addToCart(productId, quantity) {
    if (!quantity) quantity = 1;
    quantity = parseInt(quantity);

    const btn = event ? event.currentTarget : null;
    if (btn) {
        var originalText = btn.innerText;
        btn.disabled = true;
        btn.innerText = "Adding...";
    }

    $.ajax({
        url: '/Order/AddToCartAjax',   // ✅ ĐÃ SỬA: dùng action trả JSON thay vì Redirect
        type: 'POST',
        data: { productId: productId, quantity: quantity },
        success: function (response) {
            if (response.success) {
                updateHeaderCartCount();

                // Use SweetAlert2 if available
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success!',
                        text: response.message,
                        showConfirmButton: false,
                        timer: 1500,
                        toast: true,
                        position: 'top-end'
                    });
                } else {
                    alert(response.message);
                }
            }
            else if (response.requireLogin) {
                if (confirm(response.message)) {
                    window.location.href = "/Account/Login";
                }
            }
            else {
                alert(response.message);
            }

            if (btn) {
                btn.disabled = false;
                btn.innerText = originalText;
            }
        },
        error: function () {
            alert('Connection error. Please try again.');
            if (btn) {
                btn.disabled = false;
                btn.innerText = originalText;
            }
        }
    });
}

// Alias để tương thích với code cũ gọi addToCartAjax()
function addToCartAjax(productId, quantity) {
    addToCart(productId, quantity);
}

// ==========================================
// 2. UPDATE HEADER CART COUNT
// ==========================================
function updateHeaderCartCount() {
    $.ajax({
        url: '/Order/GetCartCount',
        type: 'GET',
        success: function (response) {
            const countElement = $('.cart-count');
            if (countElement.length > 0) {
                countElement.text(response.count);
                if (response.count > 0) {
                    countElement.show();
                    countElement.css('display', 'flex');
                } else {
                    countElement.hide();
                }
            }
        }
    });
}

// ==========================================
// 3. UPDATE QUANTITY
// ==========================================
function updateQuantity(productId, newQuantity) {
    if (newQuantity < 1) return;

    $.ajax({
        url: '/Order/UpdateCart',
        type: 'POST',
        data: { productId: productId, quantity: newQuantity },
        success: function (response) {
            if (response.success) {
                window.location.reload();
            } else {
                alert('Failed to update quantity');
            }
        },
        error: function () {
            alert('Connection error. Please try again.');
        }
    });
}

// ==========================================
// 4. REMOVE FROM CART
// ==========================================
function removeFromCart(productId) {
    if (!confirm('Are you sure you want to remove this item?')) return;

    $.ajax({
        url: '/Order/RemoveFromCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success || response.Success) {
                window.location.reload();
            } else {
                alert('Failed to remove item: ' + (response.message || response.Message || 'Unknown error'));
            }
        },
        error: function () {
            alert('Connection error. Please try again.');
        }
    });
}

// ==========================================
// 5. UPDATE SHIPPING & RECALCULATE TOTAL
// ==========================================
function updateShipping(method) {
    $.ajax({
        url: '/Order/UpdateShipping',
        type: 'POST',
        data: { shippingMethod: method },
        success: function (response) {
            if (response.success) {
                $('#summary-total').text(response.total.toLocaleString('vi-VN') + ' VND');
                $('#order-total').text(response.total.toLocaleString('vi-VN') + ' VND');
                const vatAmount = (response.total * 0.08).toFixed(0);
                $('.vat-info').text(`(includes ${parseInt(vatAmount).toLocaleString('vi-VN')} VND VAT)`);
            }
        },
        error: function () {
            console.error('Failed to update shipping');
        }
    });
}

// ==========================================
// 6. DOCUMENT READY - EVENT HANDLERS
// ==========================================
$(document).ready(function () {
    console.log('Cart.js loaded and ready');

    // Update cart count on page load
    updateHeaderCartCount();

    // Quantity Controls
    /*$(document).off('click', '.qty-btn.minus').on('click', '.qty-btn.minus', function (e) {
        e.preventDefault();
        e.stopPropagation();
        const pid = $(this).data('product-id');
        const input = $(`.qty-input[data-product-id="${pid}"]`);
        let qty = parseInt(input.val());
        if (qty > 1) {
            updateQuantity(pid, qty - 1);
        } else {
            alert('Minimum quantity is 1');
        }
    });

    $(document).on('click', '.qty-btn.plus', function (e) {
        e.preventDefault();
        e.stopPropagation();
        const pid = $(this).data('product-id');
        const input = $(`.qty-input[data-product-id="${pid}"]`);
        let qty = parseInt(input.val());
        updateQuantity(pid, qty + 1);
    });*/

    // Remove Item
    /*$(document).off('click', '.remove-item').on('click', '.remove-item', function (e) {
        e.preventDefault();
        e.stopPropagation();
        const pid = $(this).data('product-id');
        removeFromCart(pid);
    });*/

    // Shipping Method
    $('input[name="shipping"]').change(function () {
        updateShipping($(this).val());
    });

    // Checkout Button
    $('#place-order').click(function () {
        $('#checkout-form').submit();
    });
});