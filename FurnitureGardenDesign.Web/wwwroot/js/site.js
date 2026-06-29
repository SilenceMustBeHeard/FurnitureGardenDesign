// STAR RATING FUNCTIONALITY

document.querySelectorAll(".star-rating").forEach(rating => {
    const designId = rating.dataset.designId;
    const input = document.getElementById(`ratingInput-${designId}`);
    const stars = rating.querySelectorAll(".star");

    stars.forEach(star => {
        star.addEventListener("click", () => {
            const value = star.dataset.value;
            input.value = value;

            stars.forEach(s => {
                s.classList.toggle("active", s.dataset.value <= value);
            });
        });
    });
});

// SMOOTH SCROLL FOR ANCHOR LINKS
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        const href = this.getAttribute('href');
        if (href && href !== '#') {
            const target = document.querySelector(href);
            if (target) {
                e.preventDefault();
                target.scrollIntoView({ behavior: 'smooth' });
            }
        }
    });
});

// TOOLTIP INITIALIZATION

const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
tooltipTriggerList.map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

console.log('✨ Site.js initialized - Luxury Smart Home Theme ✨');