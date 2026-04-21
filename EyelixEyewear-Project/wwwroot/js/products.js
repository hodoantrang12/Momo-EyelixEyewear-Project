// =====================================================
// FILE: wwwroot/js/products.js - HOÀN CHỈNH
// =====================================================

// ==========================================
// GLOBAL VARIABLES
// ==========================================
let currentFilter = 'all';
let currentSort = 'featured';
let activeFilters = {
    material: [],
    shape: [],
    gender: [],
    color: [],
    minPrice: 0,
    maxPrice: 1000000
};

// ==========================================
// PRICE RANGE SLIDER LOGIC
// ==========================================
function initPriceRangeSlider() {
    const minRange = document.getElementById('minRange');
    const maxRange = document.getElementById('maxRange');
    const minInput = document.getElementById('minPrice');
    const maxInput = document.getElementById('maxPrice');

    if (!minRange || !maxRange || !minInput || !maxInput) return;

    // Sync range slider with input
    minRange.addEventListener('input', () => {
        let minVal = parseInt(minRange.value);
        let maxVal = parseInt(maxRange.value);

        if (minVal >= maxVal - 10000) {
            minRange.value = maxVal - 10000;
            minVal = maxVal - 10000;
        }
        minInput.value = minVal;
        updateSliderTrack();
    });

    maxRange.addEventListener('input', () => {
        let minVal = parseInt(minRange.value);
        let maxVal = parseInt(maxRange.value);

        if (maxVal <= minVal + 10000) {
            maxRange.value = minVal + 10000;
            maxVal = minVal + 10000;
        }
        maxInput.value = maxVal;
        updateSliderTrack();
    });

    // Sync input with range slider
    minInput.addEventListener('change', () => {
        let minVal = parseInt(minInput.value);
        let maxVal = parseInt(maxInput.value);

        if (minVal >= maxVal - 10000) {
            minInput.value = maxVal - 10000;
            minVal = maxVal - 10000;
        }
        minRange.value = minVal;
        updateSliderTrack();
    });

    maxInput.addEventListener('change', () => {
        let minVal = parseInt(minInput.value);
        let maxVal = parseInt(maxInput.value);

        if (maxVal <= minVal + 10000) {
            maxInput.value = minVal + 10000;
            maxVal = minVal + 10000;
        }
        maxRange.value = maxVal;
        updateSliderTrack();
    });

    updateSliderTrack();
}

function updateSliderTrack() {
    const minRange = document.getElementById('minRange');
    const maxRange = document.getElementById('maxRange');
    const track = document.querySelector('.slider-track');

    if (!minRange || !maxRange || !track) return;

    const min = parseInt(minRange.min);
    const max = parseInt(minRange.max);
    const minVal = parseInt(minRange.value);
    const maxVal = parseInt(maxRange.value);

    const percentMin = ((minVal - min) / (max - min)) * 100;
    const percentMax = ((maxVal - min) / (max - min)) * 100;

    track.style.background = `linear-gradient(to right, 
        #e5e5e5 0%, 
        #e5e5e5 ${percentMin}%, 
        #f08438 ${percentMin}%, 
        #f08438 ${percentMax}%, 
        #e5e5e5 ${percentMax}%, 
        #e5e5e5 100%)`;
}

function applyPriceFilter() {
    const minPrice = parseInt(document.getElementById('minPrice').value) || 0;
    const maxPrice = parseInt(document.getElementById('maxPrice').value) || 1000000;

    activeFilters.minPrice = minPrice;
    activeFilters.maxPrice = maxPrice;

    console.log('Price filter:', minPrice, '-', maxPrice);
    applyAllFilters();
}

// ==========================================
// TOGGLE FILTER SECTION (Accordion)
// ==========================================
function toggleFilterSection(header) {
    const section = header.parentElement;
    const content = section.querySelector('.filter-content');
    const icon = header.querySelector('.toggle-icon');

    if (!content || !icon) return;

    if (section.classList.contains('open')) {
        section.classList.remove('open');
        content.classList.add('collapsed');
        icon.textContent = '▼';
    } else {
        section.classList.add('open');
        content.classList.remove('collapsed');
        icon.textContent = '▲';
    }
}

// ==========================================
// TOGGLE FILTER OPTION
// ==========================================
function toggleFilter(element) {
    const filterType = element.dataset.filterType;
    const filterValue = element.dataset.value;

    if (!filterType || !filterValue) return;

    const checkbox = element.querySelector('.filter-checkbox');
    if (!checkbox) return;

    const isChecked = element.classList.contains('active');

    if (isChecked) {
        checkbox.textContent = '☐';
        element.classList.remove('active');
        const index = activeFilters[filterType].indexOf(filterValue);
        if (index > -1) activeFilters[filterType].splice(index, 1);
    } else {
        checkbox.textContent = '☑';
        element.classList.add('active');
        if (!activeFilters[filterType].includes(filterValue)) {
            activeFilters[filterType].push(filterValue);
        }
    }

    applyAllFilters();
}

// ==========================================
// APPLY ALL FILTERS (Sidebar + Type + Price)
// ==========================================
function applyAllFilters() {
    const products = document.querySelectorAll('.product-item');
    let visibleCount = 0;

    products.forEach(product => {
        let shouldShow = true;

        // 1. Check Type Filter (All, Bestseller, New, Sale)
        const productFilters = product.dataset.filter ? product.dataset.filter.split(' ') : [];
        if (currentFilter !== 'all' && !productFilters.includes(currentFilter)) {
            shouldShow = false;
        }

        // 2. Check Material
        if (shouldShow && activeFilters.material.length > 0) {
            const productMaterial = product.dataset.material || '';
            if (!activeFilters.material.includes(productMaterial)) {
                shouldShow = false;
            }
        }

        // 3. Check Shape
        if (shouldShow && activeFilters.shape.length > 0) {
            const productShape = product.dataset.shape || '';
            if (!activeFilters.shape.includes(productShape)) {
                shouldShow = false;
            }
        }

        // 4. Check Gender
        if (shouldShow && activeFilters.gender.length > 0) {
            const productGender = product.dataset.gender || '';
            if (!activeFilters.gender.includes(productGender)) {
                shouldShow = false;
            }
        }

        // 5. Check Color
        if (shouldShow && activeFilters.color.length > 0) {
            const productColor = product.dataset.color || '';
            if (!activeFilters.color.includes(productColor)) {
                shouldShow = false;
            }
        }

        // 6. Check Price Range
        if (shouldShow) {
            const productPrice = parseInt(product.dataset.price) || 0;
            if (productPrice < activeFilters.minPrice || productPrice > activeFilters.maxPrice) {
                shouldShow = false;
            }
        }

        // Show/Hide
        if (shouldShow) {
            product.style.display = 'block';
            visibleCount++;
        } else {
            product.style.display = 'none';
        }
    });

    console.log('Visible products:', visibleCount);
    updateResultCount(visibleCount);
    applySorting();
}

// ==========================================
// SORTING
// ==========================================
function applySorting() {
    const grid = document.getElementById('productsGrid');
    const products = Array.from(document.querySelectorAll('.product-item'));

    const visibleProducts = products.filter(p => p.style.display !== 'none');

    visibleProducts.sort((a, b) => {
        const priceA = parseInt(a.dataset.price) || 0;
        const priceB = parseInt(b.dataset.price) || 0;
        const idA = parseInt(a.dataset.productId) || 0;
        const idB = parseInt(b.dataset.productId) || 0;

        if (currentSort === 'price-low') return priceA - priceB;
        if (currentSort === 'price-high') return priceB - priceA;
        if (currentSort === 'newest') return idB - idA;
        return 0;
    });

    visibleProducts.forEach(product => grid.appendChild(product));
}

// ==========================================
// RESET ALL FILTERS
// ==========================================
function resetAllFilters() {
    activeFilters = {
        material: [],
        shape: [],
        gender: [],
        color: [],
        minPrice: 0,
        maxPrice: 1000000
    };

    // Reset checkboxes
    document.querySelectorAll('.filter-option').forEach(option => {
        const checkbox = option.querySelector('.filter-checkbox');
        if (checkbox) checkbox.textContent = '☐';
        option.classList.remove('active');
    });

    // Reset price inputs
    const minInput = document.getElementById('minPrice');
    const maxInput = document.getElementById('maxPrice');
    const minRange = document.getElementById('minRange');
    const maxRange = document.getElementById('maxRange');

    if (minInput) minInput.value = 50000;
    if (maxInput) maxInput.value = 800000;
    if (minRange) minRange.value = 50000;
    if (maxRange) maxRange.value = 800000;

    updateSliderTrack();

    // Reset type filter
    currentFilter = 'all';
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.classList.toggle('active', btn.dataset.filter === 'all');
    });

    // Show all products
    document.querySelectorAll('.product-item').forEach(p => p.style.display = 'block');

    const totalProducts = document.querySelectorAll('.product-item').length;
    updateResultCount(totalProducts);
}

// ==========================================
// UPDATE RESULT COUNT
// ==========================================
function updateResultCount(count) {
    const totalProducts = document.querySelectorAll('.product-item').length;
    let resultElement = document.querySelector('.filter-result-count');

    if (!resultElement) {
        resultElement = document.createElement('div');
        resultElement.className = 'filter-result-count';
        const filterSection = document.querySelector('.filter-section');
        if (filterSection) {
            filterSection.insertAdjacentElement('afterend', resultElement);
        }
    }

    resultElement.textContent = `Showing ${count} of ${totalProducts} products`;
}

// ==========================================
// CHANGE PRODUCT IMAGE
// ==========================================
function changeProductImage(clickedElement, newImageSrc) {
    const productItem = clickedElement.closest('.product-item');
    if (!productItem) return;

    const imgElement = productItem.querySelector('.product-image img');
    if (!imgElement) return;

    imgElement.style.opacity = '0.5';
    setTimeout(() => {
        imgElement.src = newImageSrc;
        imgElement.style.opacity = '1';
    }, 150);

    const allColors = productItem.querySelectorAll('.color-option');
    allColors.forEach(c => c.classList.remove('active'));
    clickedElement.classList.add('active');
}

// ==========================================
// INITIALIZE
// ==========================================
document.addEventListener('DOMContentLoaded', function () {
    console.log('Products.js loaded');

    // Initialize price range slider
    initPriceRangeSlider();

    // Open first section by default
    const firstSection = document.querySelector('.filter-section-sidebar');
    if (firstSection) {
        firstSection.classList.add('open');
        const content = firstSection.querySelector('.filter-content');
        if (content) content.classList.remove('collapsed');
        const icon = firstSection.querySelector('.toggle-icon');
        if (icon) icon.textContent = '▲';
    }

    // Filter Buttons (All, Bestseller, New, Sale)
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
            e.target.classList.add('active');
            currentFilter = e.target.dataset.filter;
            applyAllFilters();
        });
    });

    // Sort Dropdown
    const sortSelect = document.getElementById('sortBy');
    if (sortSelect) {
        sortSelect.addEventListener('change', (e) => {
            currentSort = e.target.value;
            applySorting();
        });
    }

    // Show initial count
    const totalProducts = document.querySelectorAll('.product-item').length;
    updateResultCount(totalProducts);
});