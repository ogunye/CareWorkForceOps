// CareWorkOps — landing.js
// Scroll fade-in + sticky nav tint

(function () {
    'use strict';

    // ─── SCROLL FADE-IN ─────────────────────────────────────────
    const fadeEls = document.querySelectorAll('.fade-in');
    if ('IntersectionObserver' in window) {
        const io = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    io.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });
        fadeEls.forEach(el => io.observe(el));
    } else {
        // Fallback for older browsers
        fadeEls.forEach(el => el.classList.add('visible'));
    }

    // ─── STICKY NAV SCROLL TINT ──────────────────────────────────
    const header = document.querySelector('.landing-navbar');
    if (header) {
        const onScroll = () => {
            if (window.scrollY > 48) {
                header.style.boxShadow = '0 4px 20px rgba(15,23,42,0.10)';
            } else {
                header.style.boxShadow = '0 2px 8px rgba(15,23,42,0.08)';
            }
        };
        window.addEventListener('scroll', onScroll, { passive: true });
    }

    // ─── SMOOTH ANCHOR SCROLL ────────────────────────────────────
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });

})();
