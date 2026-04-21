// =====================================
// POPUP MODAL
// =====================================
document.addEventListener('DOMContentLoaded', function () {
    const popup = document.getElementById('popup-modal');
    if (!popup) return;

    const closeBtn = popup.querySelector('.close-modal');
    const dismissBtn = popup.querySelector('.btn-dismiss');

    // Show popup after 3 seconds
    setTimeout(() => {
        popup.classList.add('active');
    }, 3000);

    // Close popup
    function closePopup() {
        popup.classList.remove('active');
    }

    if (closeBtn) closeBtn.addEventListener('click', closePopup);
    if (dismissBtn) dismissBtn.addEventListener('click', closePopup);

    // Close when clicking outside
    popup.addEventListener('click', function (e) {
        if (e.target === popup) {
            closePopup();
        }
    });
});

// =====================================
// MOBILE MENU
// =====================================
let menuToggle = document.createElement('button');
menuToggle.className = 'mobile-menu-toggle';
menuToggle.innerHTML = '☰';
menuToggle.style.cssText = `
    display: none;
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 1000;
    background: white;
    border: 1px solid #ddd;
    padding: 10px 15px;
    font-size: 24px;
    border-radius: 5px;
    cursor: pointer;
`;

document.body.appendChild(menuToggle);

menuToggle.addEventListener('click', function () {
    let menu = document.querySelector('.menu');
    if (menu) menu.classList.toggle('active');
});

// Show mobile menu button on small screens
function checkScreenSize() {
    if (window.innerWidth <= 768) {
        menuToggle.style.display = 'block';
    } else {
        menuToggle.style.display = 'none';
        const menu = document.querySelector('.menu');
        if (menu) menu.classList.remove('active');
    }
}

window.addEventListener('resize', checkScreenSize);
checkScreenSize();

// =====================================
// DROPDOWN MENU
// =====================================
const dropdowns = document.querySelectorAll('.dropdown');

dropdowns.forEach(dropdown => {
    const link = dropdown.querySelector('a');

    if (link) {
        link.addEventListener('click', function (e) {
            if (window.innerWidth <= 768) {
                e.preventDefault();
                dropdown.classList.toggle('active');
            }
        });
    }
});

// =====================================
// SMOOTH SCROLL
// =====================================
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        const href = this.getAttribute('href');
        if (href === '#') return;

        e.preventDefault();
        const target = document.querySelector(href);
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// =====================================
// STICKY HEADER
// =====================================
let lastScroll = 0;
const header = document.getElementById('header');

if (header) {
    window.addEventListener('scroll', () => {
        const currentScroll = window.pageYOffset;

        if (currentScroll > 100) {
            header.classList.add('sticky');
        } else {
            header.classList.remove('sticky');
        }

        lastScroll = currentScroll;
    });
}

// =====================================
// PRODUCT SLIDER
// =====================================
function initProductSlider() {
    const sliders = document.querySelectorAll('.product-slider');

    sliders.forEach(slider => {
        let isDown = false;
        let startX;
        let scrollLeft;

        slider.addEventListener('mousedown', (e) => {
            isDown = true;
            slider.classList.add('active');
            startX = e.pageX - slider.offsetLeft;
            scrollLeft = slider.scrollLeft;
        });

        slider.addEventListener('mouseleave', () => {
            isDown = false;
            slider.classList.remove('active');
        });

        slider.addEventListener('mouseup', () => {
            isDown = false;
            slider.classList.remove('active');
        });

        slider.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - slider.offsetLeft;
            const walk = (x - startX) * 2;
            slider.scrollLeft = scrollLeft - walk;
        });
    });
}

initProductSlider();

// =====================================
// NOTIFICATION
// =====================================
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = 'notification';
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${type === 'success' ? '#28a745' : '#000'};
        color: white;
        padding: 15px 25px;
        border-radius: 5px;
        z-index: 10000;
        animation: slideInRight 0.3s ease;
        box-shadow: 0 2px 8px rgba(0,0,0,0.2);
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// Add animation styles
const style = document.createElement('style');
style.textContent = `
    @keyframes slideInRight {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

// =====================================
// LAZY LOADING IMAGES
// =====================================
const images = document.querySelectorAll('img[data-src]');

const imageObserver = new IntersectionObserver((entries, observer) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            const img = entry.target;
            // ✅ Đảm bảo đường dẫn bắt đầu từ /
            const src = img.dataset.src;
            img.src = src.startsWith('/') ? src : '/' + src;
            img.removeAttribute('data-src');
            observer.unobserve(img);
        }
    });
});

images.forEach(img => imageObserver.observe(img));

// =====================================
// FORM VALIDATION
// =====================================
const forms = document.querySelectorAll('form:not([method="POST"])'); // Chỉ validate form không submit lên server

forms.forEach(form => {
    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const email = form.querySelector('input[type="email"]');

        if (email && !validateEmail(email.value)) {
            showNotification('Please enter a valid email address', 'error');
            return;
        }

        // If newsletter form
        if (form.classList.contains('newsletter-form')) {
            // TODO: Submit to server
            // fetch('/Newsletter/Subscribe', { method: 'POST', body: new FormData(form) })
            showNotification('Thank you for subscribing!');
            form.reset();
        }
    });
});

function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// =====================================
// SCROLL REVEAL ANIMATION
// =====================================
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
};

const revealObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.style.opacity = '1';
            entry.target.style.transform = 'translateY(0)';
        }
    });
}, observerOptions);

const animateElements = document.querySelectorAll('.product-card, .blog-card, .collection-item');

animateElements.forEach(el => {
    el.style.opacity = '0';
    el.style.transform = 'translateY(30px)';
    el.style.transition = 'all 0.6s ease';
    revealObserver.observe(el);
});

// =====================================
// BACK TO TOP BUTTON
// =====================================
const backToTop = document.createElement('button');
backToTop.className = 'back-to-top';
backToTop.innerHTML = '↑';
backToTop.style.cssText = `
    position: fixed;
    bottom: 30px;
    right: 30px;
    width: 50px;
    height: 50px;
    background: #000;
    color: white;
    border: none;
    border-radius: 50%;
    font-size: 24px;
    cursor: pointer;
    opacity: 0;
    visibility: hidden;
    transition: all 0.3s ease;
    z-index: 999;
`;

document.body.appendChild(backToTop);

window.addEventListener('scroll', () => {
    if (window.pageYOffset > 300) {
        backToTop.style.opacity = '1';
        backToTop.style.visibility = 'visible';
    } else {
        backToTop.style.opacity = '0';
        backToTop.style.visibility = 'hidden';
    }
});

backToTop.addEventListener('click', () => {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
});

// =====================================
// COLLECTIONS CAROUSEL WITH NAVIGATION
// =====================================
function initCollectionsCarousel() {
    const carousel = document.querySelector('.collections-carousel');
    if (!carousel) return;

    const slides = carousel.querySelectorAll('.collection-slide');
    if (slides.length === 0) return;

    let currentSlide = 0;
    let autoPlayInterval;
    const transitionDuration = 400;

    const prevArrow = document.createElement('button');
    prevArrow.className = 'slide-nav prev';
    prevArrow.innerHTML = '<i class="fas fa-angle-left"></i>';
    prevArrow.setAttribute('aria-label', 'Previous slide');

    const nextArrow = document.createElement('button');
    nextArrow.className = 'slide-nav next';
    nextArrow.innerHTML = '<i class="fas fa-angle-right"></i>';
    nextArrow.setAttribute('aria-label', 'Next slide');

    carousel.appendChild(prevArrow);
    carousel.appendChild(nextArrow);

    function showSlide(index) {
        slides[index].style.display = 'flex';

        slides.forEach((slide, i) => {
            if (i === index) {
                setTimeout(() => {
                    slide.classList.add('active');
                }, 10);
            } else if (slide.classList.contains('active')) {
                slide.classList.remove('active');

                setTimeout(() => {
                    slide.style.display = 'none';
                }, transitionDuration);
            } else {
                slide.style.display = 'none';
                slide.classList.remove('active');
            }
        });
    }

    function nextSlide() {
        currentSlide = (currentSlide + 1) % slides.length;
        showSlide(currentSlide);
    }

    function prevSlide() {
        currentSlide = (currentSlide - 1 + slides.length) % slides.length;
        showSlide(currentSlide);
    }

    // Event listeners for arrows
    nextArrow.addEventListener('click', () => {
        nextSlide();
        resetAutoPlay();
    });

    prevArrow.addEventListener('click', () => {
        prevSlide();
        resetAutoPlay();
    });

    // Auto play functionality
    function startAutoPlay() {
        autoPlayInterval = setInterval(nextSlide, 5000);
    }

    function resetAutoPlay() {
        clearInterval(autoPlayInterval);
        startAutoPlay();
    }

    // Initialize
    slides.forEach(slide => slide.style.transition = `opacity ${transitionDuration / 1000}s ease-in-out`);
    showSlide(0);
    startAutoPlay();

    // Pause on hover
    carousel.addEventListener('mouseenter', () => {
        clearInterval(autoPlayInterval);
    });

    carousel.addEventListener('mouseleave', () => {
        startAutoPlay();
    });
}

initCollectionsCarousel();

// =====================================
// INSTAGRAM GRID LIGHTBOX
// =====================================
const instagramImages = document.querySelectorAll('.instagram-grid img');

instagramImages.forEach(img => {
    img.addEventListener('click', function () {
        const lightbox = document.createElement('div');
        lightbox.className = 'lightbox';

        // ✅ Đảm bảo đường dẫn ảnh đúng
        const imgSrc = this.src;

        lightbox.innerHTML = `
            <div class="lightbox-content">
                <button class="close-lightbox" aria-label="Close lightbox">&times;</button>
                <img src="${imgSrc}" alt="${this.alt || 'Instagram image'}">
            </div>
        `;
        lightbox.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.95);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10000;
            cursor: pointer;
        `;

        const lightboxContent = lightbox.querySelector('.lightbox-content');
        lightboxContent.style.cssText = `
            position: relative;
            max-width: 90%;
            max-height: 90%;
        `;

        const lightboxImg = lightbox.querySelector('img');
        lightboxImg.style.cssText = `
            max-width: 100%;
            max-height: 90vh;
            object-fit: contain;
        `;

        const closeBtn = lightbox.querySelector('.close-lightbox');
        closeBtn.style.cssText = `
            position: absolute;
            top: -40px;
            right: -40px;
            background: white;
            border: none;
            width: 35px;
            height: 35px;
            border-radius: 50%;
            font-size: 24px;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
        `;

        document.body.appendChild(lightbox);
        document.body.style.overflow = 'hidden';

        closeBtn.addEventListener('click', () => {
            lightbox.remove();
            document.body.style.overflow = '';
        });

        lightbox.addEventListener('click', (e) => {
            if (e.target === lightbox) {
                lightbox.remove();
                document.body.style.overflow = '';
            }
        });

        // ESC key to close
        document.addEventListener('keydown', function closeOnEsc(e) {
            if (e.key === 'Escape') {
                lightbox.remove();
                document.body.style.overflow = '';
                document.removeEventListener('keydown', closeOnEsc);
            }
        });
    });
});

console.log('Eyelix Eyewear - Website loaded successfully!');