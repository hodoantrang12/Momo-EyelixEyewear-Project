
// Format currency
function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN') + ' VND';
}

// Format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
}

// Show loading
function showLoading(message = 'Loading...') {
    Swal.fire({
        title: message,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
}

// Hide loading
function hideLoading() {
    Swal.close();
}

// Confirm dialog
function confirmAction(title, text, confirmText = 'Yes') {
    return Swal.fire({
        title: title,
        text: text,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e00d0d',
        cancelButtonColor: '#64748b',
        confirmButtonText: confirmText,
        cancelButtonText: 'Cancel'
    });
}

// Success toast
function showSuccess(message) {
    Swal.fire({
        icon: 'success',
        title: 'Success!',
        text: message,
        showConfirmButton: false,
        timer: 2000,
        toast: true,
        position: 'top-end'
    });
}

// Error toast
function showError(message) {
    Swal.fire({
        icon: 'error',
        title: 'Error!',
        text: message,
        confirmButtonColor: '#e00d0d'
    });
}

// Table search helper
function searchTable(inputId, tableId) {
    const input = $(inputId).val().toLowerCase();
    $(tableId + ' tbody tr').each(function () {
        const text = $(this).text().toLowerCase();
        $(this).toggle(text.indexOf(input) > -1);
    });
}

// Initialize tooltips (if using Bootstrap)
$(document).ready(function () {
    // Add any global initializations here
    console.log('Admin panel loaded');
});