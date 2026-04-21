let wishlistData = [
    {
        id: 1,
        name: "Aura",
        price: 550000,
        dateAdded: "December 1, 2025",
        stockStatus: "1000 in stock",
        image: "images/products/aura.png" 
    },
    {
        id: 2,
        name: "Neos",
        price: 650000,
        dateAdded: "December 1, 2025",
        stockStatus: "1000 in stock",
        image: "images/products/neos.png"
    },
    {
        id: 3,
        name: "Jules",
        price: 760000,
        dateAdded: "December 1, 2025",
        stockStatus: "115 in stock",
        image: "images/products/jules.png"
    }
];

// DOM Elements
const wishlistItemsContainer = document.getElementById("wishlist-items");
const applyActionBtn = document.getElementById("apply-action");
const addSelectedBtn = document.getElementById("add-selected-to-cart");
const addAllBtn = document.getElementById("add-all-to-cart");
const actionSelect = document.getElementById("action-select");

function renderWishlist() {
    wishlistItemsContainer.innerHTML = '';

    if (wishlistData.length === 0) {
        wishlistItemsContainer.innerHTML = '<tr><td colspan="8" style="text-align:center;">Your wishlist is currently empty.</td></tr>';
        return;
    }

    wishlistData.forEach((product, index) => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <!-- Checkbox -->
            <td style="text-align:center;">
                <input type="checkbox" class="product-checkbox" data-index="${index}" />
            </td>
            
            <!-- Remove Button (x) -->
            <td style="text-align:center;">
                <button class="remove-btn" onclick="removeFromWishlist(${index})">×</button>
            </td>

            <!-- Image -->
            <td>
                <img src="${product.image}" alt="${product.name}" class="product-img" />
            </td>

            <!-- Name -->
            <td class="product-name">${product.name}</td>

            <!-- Price -->
            <td class="unit-price">${product.price.toLocaleString("vi-VN")} VND <span style="font-size:12px; color:#999;">VAT</span></td>

            <!-- Date -->
            <td style="color:#777;">${product.dateAdded}</td>

            <!-- Stock -->
            <td class="in-stock"><i class="fas fa-check"></i> ${product.stockStatus}</td>

            <!-- Action Button -->
            <td>
                <button class="btn-add-cart-small" onclick="addToCartSingle(${index})">Add to Cart</button>
            </td>
        `;
        wishlistItemsContainer.appendChild(row);
    });
}

window.removeFromWishlist = function(index) {
    if(confirm("Remove this item from wishlist?")) {
        wishlistData.splice(index, 1);
        renderWishlist();
    }
};

window.addToCartSingle = function(index) {
    const product = wishlistData[index];
    addItemToLocalStorage(product);
    alert(`"${product.name}" has been added to your cart!`);
};

function addItemToLocalStorage(product) {
    let cart = JSON.parse(localStorage.getItem("eyelixCart")) || [];
    
    const existingItem = cart.find(item => item.name === product.name);
    
    if (existingItem) {
        existingItem.qty += 1;
    } else {
        cart.push({
            name: product.name,
            price: product.price,
            qty: 1,
            image: product.image
        });
    }
    
    localStorage.setItem("eyelixCart", JSON.stringify(cart));
    
    window.dispatchEvent(new Event("cartUpdated"));
}

addSelectedBtn.addEventListener('click', () => {
    const checkboxes = document.querySelectorAll('.product-checkbox:checked');
    if (checkboxes.length === 0) {
        alert("Please select items to add.");
        return;
    }

    checkboxes.forEach(box => {
        const index = box.getAttribute('data-index');
        addItemToLocalStorage(wishlistData[index]);
    });

    alert("Selected items have been added to cart!");
    window.location.href = "cart.html"; 
});

addAllBtn.addEventListener('click', () => {
    if (wishlistData.length === 0) return;

    wishlistData.forEach(product => {
        addItemToLocalStorage(product);
    });

    alert("All items from wishlist added to cart!");
    window.location.href = "cart.html";
});

applyActionBtn.addEventListener('click', () => {
    const action = actionSelect.value;
    const checkboxes = document.querySelectorAll('.product-checkbox:checked');

    if (checkboxes.length === 0) {
        alert("Please select items first.");
        return;
    }

    if (action === "") {
        alert("Please select an action.");
        return;
    }

    if (action === "add-cart") {
        checkboxes.forEach(box => {
            addItemToLocalStorage(wishlistData[box.getAttribute('data-index')]);
        });
        alert("Items added to cart.");
    } 
    else if (action === "remove") {
        let indicesToRemove = [];
        checkboxes.forEach(box => {
            indicesToRemove.push(parseInt(box.getAttribute('data-index')));
        });
        
        indicesToRemove.sort((a, b) => b - a);
        
        indicesToRemove.forEach(index => {
            wishlistData.splice(index, 1);
        });
        
        renderWishlist();
    }
});

// Init

renderWishlist();
