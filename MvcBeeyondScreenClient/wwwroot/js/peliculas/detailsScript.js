// ~/js/movies/details.js

document.addEventListener('DOMContentLoaded', function () {
    // Efecto de scroll suave al cargar la página
    setTimeout(() => {
        window.scrollTo({
            top: 10,
            behavior: 'smooth'
        });
    }, 200);

    // Opcional: Inicializar tooltips de Bootstrap si son necesarios
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
});