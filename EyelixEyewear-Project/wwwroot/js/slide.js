document.addEventListener('DOMContentLoaded', function () {
    const track = document.getElementById('productTrack');
    const btnNext = document.getElementById('btnNext');
    let originalItems = document.querySelectorAll('.product-slide-item');

    if (!track || !btnNext || originalItems.length === 0) return;

    const itemsToShow = window.innerWidth > 768 ? 3 : 1;
    const itemWidth = 100 / itemsToShow;
    const transitionSpeed = 500;


    originalItems.forEach(item => {
        item.style.width = `${itemWidth}%`;
        item.style.flex = `0 0 ${itemWidth}%`;
    });

    // CLONE ONE-WAY

    for (let i = 0; i < itemsToShow; i++) {
        if (originalItems[i]) {
            let clone = originalItems[i].cloneNode(true);
            clone.style.width = `${itemWidth}%`;
            clone.style.flex = `0 0 ${itemWidth}%`;
            track.appendChild(clone);
        }
    }

    let currentIndex = 0;
    let isTransitioning = false;
    // Cập nhật lại danh sách (bao gồm cả clone vừa thêm)
    const allItems = document.querySelectorAll('.product-slide-item');
    const totalOriginals = originalItems.length; // Số lượng SP thật

    // --- HÀM XỬ LÝ NEXT ---
    function nextSlide() {
        if (isTransitioning) return;
        isTransitioning = true;

        // A. TÌM SẢN PHẨM SẮP HIỆN RA ĐỂ GẮN HIỆU ỨNG
        // Sản phẩm mới nằm ở bên phải cùng của khung nhìn
        let nextItemIndex = currentIndex + itemsToShow;

        // Nếu nextItemIndex vượt quá danh sách DOM -> quay về clone đầu
        if (nextItemIndex >= allItems.length) nextItemIndex = 0;

        const nextItem = allItems[nextItemIndex];
        if (nextItem) {
            nextItem.classList.remove('animate-enter');
            void nextItem.offsetWidth; // Reset animation trigger
            nextItem.classList.add('animate-enter');
        }

        // B. DI CHUYỂN
        track.style.transition = `transform ${transitionSpeed}ms ease-in-out`;
        currentIndex++;
        track.style.transform = `translateX(-${currentIndex * itemWidth}%)`;

        // C. XỬ LÝ VÒNG LẶP (RESET)
        track.addEventListener('transitionend', () => {
            if (currentIndex >= totalOriginals) {
                track.style.transition = 'none'; // Tắt hiệu ứng chuyển động
                currentIndex = 0; // Nhảy ngay lập tức về đầu
                track.style.transform = `translateX(0)`;
            }
            isTransitioning = false;
        }, { once: true }); // Chỉ chạy sự kiện này 1 lần mỗi khi bấm
    }

    // Sự kiện click
    btnNext.addEventListener('click', nextSlide);

    // Kích hoạt animation cho 3 cái đầu tiên khi vừa vào trang
    for (let i = 0; i < itemsToShow; i++) {
        if (allItems[i]) allItems[i].classList.add('animate-enter');
    }
});