// ==========================================
// REVIEW SYSTEM - Star Rating & Submit
// ==========================================

document.addEventListener('DOMContentLoaded', function() {
    
    // Star Rating Input
    const stars = document.querySelectorAll('.star-rating-input i');
    const ratingInput = document.getElementById('selectedRating');
    
    stars.forEach(star => {
        // Hover effect
        star.addEventListener('mouseenter', function() {
            const rating = this.getAttribute('data-rating');
            highlightStars(rating);
        });
        
        // Click to select
        star.addEventListener('click', function() {
            const rating = this.getAttribute('data-rating');
            ratingInput.value = rating;
            highlightStars(rating);
            document.getElementById('ratingError').style.display = 'none';
        });
    });
    
    // Reset on mouse leave
    document.querySelector('.star-rating-input').addEventListener('mouseleave', function() {
        const selectedRating = ratingInput.value;
        if (selectedRating > 0) {
            highlightStars(selectedRating);
        } else {
            resetStars();
        }
    });
    
    // Highlight stars
    function highlightStars(rating) {
        stars.forEach((star, index) => {
            if (index < rating) {
                star.classList.remove('far');
                star.classList.add('fas');
            } else {
                star.classList.remove('fas');
                star.classList.add('far');
            }
        });
    }
    
    // Reset stars
    function resetStars() {
        stars.forEach(star => {
            star.classList.remove('fas');
            star.classList.add('far');
        });
    }
    
    // Submit Review Form
    const reviewForm = document.getElementById('reviewForm');
    if (reviewForm) {
        reviewForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const productId = document.getElementById('productId').value;
            const rating = document.getElementById('selectedRating').value;
            const comment = document.getElementById('reviewComment').value.trim();
            
            // Reset errors
            document.getElementById('ratingError').style.display = 'none';
            document.getElementById('commentError').style.display = 'none';
            
            // Validation
            let hasError = false;
            
            if (!rating || rating === '0') {
                document.getElementById('ratingError').style.display = 'block';
                hasError = true;
            }
            
            if (comment.length < 10) {
                document.getElementById('commentError').style.display = 'block';
                hasError = true;
            }
            
            if (hasError) return;
            
            // Disable button
            const submitBtn = this.querySelector('button[type="submit"]');
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Submitting...';
            
            // Submit via AJAX
            fetch('/Product/AddReview', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: `productId=${productId}&rating=${rating}&comment=${encodeURIComponent(comment)}`
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert('✓ ' + data.message);
                    location.reload();
                } else {
                    alert('✗ ' + data.message);
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = '<i class="fas fa-paper-plane"></i> Submit Review';
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Connection error. Please try again.');
                submitBtn.disabled = false;
                submitBtn.innerHTML = '<i class="fas fa-paper-plane"></i> Submit Review';
            });
        });
    }
});