(function () {
    'use strict';

    // THEME

    function setDarkTheme() {
        // Force dark theme always
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        localStorage.setItem('fgd-theme', 'dark');
        console.log('✨ Luxury Dark Theme Activated');
    }

    // SCENE BACKGROUND

    function initSceneEffects() {
        const sceneBg = document.querySelector('.scene-bg');
        if (!sceneBg) return;

        const luxuryBg = new Image();
        // Alternatively, you can use a another image by setting the src to a relative path like 'images/luxury-bg.jpg'
        // or an external URL. Make sure to choose a high-quality image that fits the luxury theme.
        luxuryBg.src = 'https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=1920&q=80';

        function updateSceneBackground() {
            sceneBg.style.backgroundImage = `url('${luxuryBg.src}')`;
            sceneBg.style.backgroundSize = 'cover';
            sceneBg.style.backgroundPosition = 'center';
            sceneBg.style.backgroundAttachment = 'scroll';

            // Add subtle overlay for better text contrast
            sceneBg.style.backgroundColor = 'rgba(0,0,0,0.3)';
            sceneBg.style.backgroundBlendMode = 'overlay';
        }

        updateSceneBackground();
    }

    // GLASS CARD EFFECTS

    function initGlassCardEffects() {
        const glassCards = document.querySelectorAll('.glass-card, .admin-catalog-card, .user-catalog-card');

        glassCards.forEach(card => {
            card.addEventListener('mouseenter', function () {
                this.style.transform = 'translateY(-6px)';
                this.style.transition = 'transform 0.3s cubic-bezier(0.2, 0.9, 0.4, 1.1), box-shadow 0.3s ease';
                this.style.boxShadow = '0 25px 50px rgba(0, 0, 0, 0.4), 0 0 0 1px rgba(192, 154, 108, 0.3)';
                this.style.borderColor = 'rgba(192, 154, 108, 0.5)';
            });

            card.addEventListener('mouseleave', function () {
                this.style.transform = 'translateY(0)';
                this.style.boxShadow = '';
                this.style.borderColor = '';
            });
        });
    }

    //  STAR RATING WITH GOLD EFFECT

    function initStarRating() {
        const starContainers = document.querySelectorAll('.star-rating');

        starContainers.forEach(container => {
            const stars = container.querySelectorAll('.star');
            const ratingInput = container.querySelector('input[type="hidden"]');

            stars.forEach(star => {
                star.addEventListener('mouseenter', function () {
                    const value = parseInt(this.getAttribute('data-value'));
                    highlightStars(stars, value);
                });

                star.addEventListener('mouseleave', function () {
                    const currentRating = ratingInput ? parseInt(ratingInput.value) : 0;
                    highlightStars(stars, currentRating);
                });

                star.addEventListener('click', function () {
                    const value = parseInt(this.getAttribute('data-value'));
                    if (ratingInput) ratingInput.value = value;
                    highlightStars(stars, value);

                    // Gold sparkle effect
                    this.style.transform = 'scale(1.3)';
                    this.style.textShadow = '0 0 10px #ffd700';
                    setTimeout(() => {
                        this.style.transform = '';
                        this.style.textShadow = '';
                    }, 200);
                });
            });
        });

        function highlightStars(stars, rating) {
            stars.forEach((star, index) => {
                if (index < rating) {
                    star.classList.add('active');
                    star.style.color = '#ffd700';
                } else {
                    star.classList.remove('active');
                    star.style.color = '';
                }
            });
        }
    }

    // FLOATING ANIMATION

    function initFloatingElements() {
        const floatingElements = document.querySelectorAll('.hero-section, .hero-glass, .glass-card');

        floatingElements.forEach((el, index) => {
            el.style.animation = `floatLuxury ${3 + (index * 0.2)}s ease-in-out infinite`;
            el.style.animationDelay = `${index * 0.1}s`;
        });

        if (!document.querySelector('#floating-keyframes')) {
            const style = document.createElement('style');
            style.id = 'floating-keyframes';
            style.textContent = `
                @keyframes floatLuxury {
                    0%, 100% { transform: translateY(0px); }
                    50% { transform: translateY(-8px); }
                }
            `;
            document.head.appendChild(style);
        }
    }

    // 6. NOTIFICATION TOAST

    function initToastNotifications() {
        if (!document.querySelector('.toast-container')) {
            const container = document.createElement('div');
            container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            container.style.zIndex = '1100';
            document.body.appendChild(container);
        }

        const successAlert = document.getElementById('tempSuccess');
        const errorAlert = document.getElementById('tempError');

        if (successAlert && successAlert.innerText.trim()) {
            showToast(successAlert.innerText, 'success');
            successAlert.remove();
        }

        if (errorAlert && errorAlert.innerText.trim()) {
            showToast(errorAlert.innerText, 'error');
            errorAlert.remove();
        }

        function showToast(message, type = 'success') {
            const container = document.querySelector('.toast-container');
            const toast = document.createElement('div');
            toast.className = `toast glass-toast align-items-center text-white border-0 mb-2`;

            const bgColor = type === 'success'
                ? 'linear-gradient(135deg, rgba(40, 167, 69, 0.95), rgba(32, 134, 55, 0.95))'
                : 'linear-gradient(135deg, rgba(220, 53, 69, 0.95), rgba(176, 42, 55, 0.95))';
            const icon = type === 'success' ? '✓' : '⚠';

            toast.style.background = bgColor;
            toast.style.backdropFilter = 'blur(10px)';
            toast.style.borderRadius = '12px';
            toast.style.border = '1px solid rgba(255,255,255,0.2)';

            toast.innerHTML = `
                <div class="d-flex">
                    <div class="toast-body d-flex align-items-center gap-3">
                        <span style="font-size: 1.3rem; font-weight: bold;">${icon}</span>
                        <span style="font-family: 'Inter', sans-serif;">${message}</span>
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            `;

            container.appendChild(toast);

            if (typeof bootstrap !== 'undefined' && bootstrap.Toast) {
                const bsToast = new bootstrap.Toast(toast, { delay: 4000, autohide: true });
                bsToast.show();
                toast.addEventListener('hidden.bs.toast', () => toast.remove());
            } else {
                setTimeout(() => toast.remove(), 4000);
            }
        }
    }

    // INPUT GLOW

    function initInputGlowEffects() {
        const inputs = document.querySelectorAll('input:not([type="hidden"]), textarea, select');

        inputs.forEach(input => {
            input.addEventListener('focus', function () {
                this.style.boxShadow = '0 0 0 3px rgba(192, 154, 108, 0.4), 0 0 15px rgba(192, 154, 108, 0.3)';
                this.style.transition = 'box-shadow 0.2s ease';
                this.style.borderColor = '#c09a6c';
            });

            input.addEventListener('blur', function () {
                this.style.boxShadow = '';
                this.style.borderColor = '';
            });
        });
    }

    // SCROLL REVEAL

    function initScrollReveal() {
        const revealElements = document.querySelectorAll('.glass-card, .admin-catalog-card, .user-catalog-card, .carousel-section');

        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -30px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry, index) => {
                if (entry.isIntersecting) {
                    setTimeout(() => {
                        entry.target.classList.add('revealed');
                    }, index * 100);
                    observer.unobserve(entry.target);
                }
            });
        }, observerOptions);

        revealElements.forEach(el => {
            el.style.opacity = '0';
            el.style.transform = 'translateY(30px)';
            el.style.transition = 'opacity 0.6s cubic-bezier(0.2, 0.9, 0.4, 1.1), transform 0.6s cubic-bezier(0.2, 0.9, 0.4, 1.1)';
            observer.observe(el);
        });

        const style = document.createElement('style');
        style.textContent = `
            .glass-card.revealed, .admin-catalog-card.revealed, .user-catalog-card.revealed,
            .carousel-section.revealed {
                opacity: 1 !important;
                transform: translateY(0) !important;
            }
        `;
        document.head.appendChild(style);
    }

    // COPYRIGHT YEAR

    function updateCopyrightYear() {
        const footerText = document.querySelector('footer .container');
        if (footerText) {
            const currentYear = new Date().getFullYear();
            if (!footerText.innerHTML.includes(currentYear.toString())) {
                footerText.innerHTML = `&copy; ${currentYear} - Furniture &amp; Garden Design | Luxury Smart Living`;
            }
        }
    }

    // IMAGE EFFECTS - SMART ZOOM

    function initImageEffects() {
        const images = document.querySelectorAll('.admin-catalog-img, .user-catalog-img, .review-img');

        images.forEach(img => {
            img.addEventListener('mouseenter', function () {
                this.style.transform = 'scale(1.05)';
                this.style.transition = 'transform 0.4s cubic-bezier(0.2, 0.9, 0.4, 1.1)';
                this.style.filter = 'brightness(1.05)';
            });

            img.addEventListener('mouseleave', function () {
                this.style.transform = 'scale(1)';
                this.style.filter = 'brightness(1)';
            });
        });
    }

    // ROLE INDICATOR

    function initRoleIndicator() {
        const roleIndicators = document.querySelectorAll('.glass-caption.admin-glow, .glass-caption.manager-glow');

        roleIndicators.forEach(indicator => {
            indicator.style.animation = 'luxuryPulse 2s ease-in-out infinite';
        });

        if (!document.querySelector('#pulse-keyframes')) {
            const style = document.createElement('style');
            style.id = 'pulse-keyframes';
            style.textContent = `
                @keyframes luxuryPulse {
                    0%, 100% {
                        opacity: 1;
                        box-shadow: 0 0 5px rgba(192, 154, 108, 0.3);
                    }
                    50% {
                        opacity: 0.85;
                        box-shadow: 0 0 15px rgba(192, 154, 108, 0.6);
                    }
                }
            `;
            document.head.appendChild(style);
        }
    }

    //  HOME AMBIENT EFFECT

    function initAmbientEffect() {
        const sceneWrapper = document.querySelector('.scene-wrapper');
        if (sceneWrapper && !document.querySelector('.ambient-overlay')) {
            const overlay = document.createElement('div');
            overlay.className = 'ambient-overlay';
            overlay.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                pointer-events: none;
                background: radial-gradient(circle at 50% 50%, rgba(192,154,108,0.05), transparent 70%);
                z-index: 1;
                animation: ambientShift 10s ease-in-out infinite;
            `;
            sceneWrapper.appendChild(overlay);

            const style = document.createElement('style');
            style.textContent = `
                @keyframes ambientShift {
                    0%, 100% { opacity: 0.3; }
                    50% { opacity: 0.6; }
                }
            `;
            document.head.appendChild(style);
        }
    }

    // INITIALIZE ALL FEATURES

    document.addEventListener('DOMContentLoaded', () => {
        console.log('🏰 Furniture And Garden Design Initializing...');

        setDarkTheme();
        initSceneEffects();
        initGlassCardEffects();
        initStarRating();
        initFloatingElements();
        initToastNotifications();
        initInputGlowEffects();
        initScrollReveal();
        updateCopyrightYear();
        initImageEffects();
        initRoleIndicator();
        initAmbientEffect();

        console.log('✨ Furniture And Garden Design Ready ✨');
    });
})();