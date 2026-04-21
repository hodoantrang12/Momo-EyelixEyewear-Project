let searchTimeout;
const searchModal = document.getElementById('searchModal');
const searchInput = document.getElementById('searchInput');
const searchResults = document.getElementById('searchResults');

// Mở search modal
function openSearchModal() {
    searchModal.classList.add('show');
    searchInput.focus();
    searchInput.value = '';
    showEmptyState();
}

// Đóng search modal
function closeSearchModal() {
    searchModal.classList.remove('show');
    searchInput.value = '';
    searchResults.innerHTML = '';
}

// Đóng khi click bên ngoài
searchModal.addEventListener('click', function (e) {
    if (e.target === searchModal) {
        closeSearchModal();
    }
});

// Đóng khi nhấn ESC
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape' && searchModal.classList.contains('show')) {
        closeSearchModal();
    }
});

// Search khi user gõ
searchInput.addEventListener('input', function () {
    const query = this.value.trim();

    // Clear timeout cũ
    clearTimeout(searchTimeout);

    if (query.length === 0) {
        showEmptyState();
        return;
    }

    if (query.length < 2) {
        searchResults.innerHTML = `
            <div class="search-empty">
                <p>Type at least 2 characters to search...</p>
            </div>
        `;
        return;
    }

    // Hiển thị loading
    showLoading();

    // Debounce - chờ 500ms sau khi user ngừng gõ
    searchTimeout = setTimeout(() => {
        performSearch(query);
    }, 500);
});

// Hiển thị loading
function showLoading() {
    searchResults.innerHTML = `
        <div class="search-loading">
            <div class="search-spinner"></div>
            <p style="color: #999; font-size: 14px;">Searching...</p>
        </div>
    `;
}

// Hiển thị empty state
function showEmptyState() {
    searchResults.innerHTML = `
        <div class="search-empty">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="11" cy="11" r="8" />
                <path d="m21 21-4.35-4.35" />
            </svg>
            <p>Start typing to search for products...</p>
        </div>
    `;
}

// Thực hiện search
function performSearch(query) {
    fetch(`/Product/SearchProducts?query=${encodeURIComponent(query)}`)
        .then(response => response.json())
        .then(data => {
            if (data.success && data.products.length > 0) {
                displayResults(data.products);
            } else {
                showNoResults(query);
            }
        })
        .catch(error => {
            console.error('Search error:', error);
            searchResults.innerHTML = `
                <div class="no-results">
                    <p>An error occurred while searching. Please try again.</p>
                </div>
            `;
        });
}

// Hiển thị kết quả
function displayResults(products) {
    let html = '';

    products.forEach(product => {
        const imageUrl = product.imageUrl || '/images/default-product.jpg';
        const hasDiscount = product.discountPrice && product.discountPrice > 0;

        let priceHtml = '';
        if (hasDiscount) {
            priceHtml = `
                <div class="search-product-price has-discount">
                    <span>${product.discountPrice.toLocaleString('vi-VN')} VND</span>
                    <span class="original-price">${product.price.toLocaleString('vi-VN')} VND</span>
                </div>
            `;
        } else {
            priceHtml = `
                <div class="search-product-price">${product.price.toLocaleString('vi-VN')} VND</div>
            `;
        }

        html += `
            <a href="/Product/Detail/${product.id}" class="search-product-item">
                <img src="${imageUrl}" alt="${product.name}" class="search-product-image">
                <div class="search-product-info">
                    <div class="search-product-name">${product.name}</div>
                    ${priceHtml}
                </div>
            </a>
        `;
    });

    searchResults.innerHTML = html;
}

// Hiển thị không có kết quả
function showNoResults(query) {
    searchResults.innerHTML = `
        <div class="no-results">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="11" cy="11" r="8" />
                <path d="m21 21-4.35-4.35" />
            </svg>
            <p>No products found for "<strong>${query}</strong>"</p>
        </div>
    `;
}