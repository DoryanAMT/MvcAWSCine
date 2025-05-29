// JavaScript para la página de películas

document.addEventListener("DOMContentLoaded", function () {
    // Inicializar la búsqueda de películas
    initPeliculasSearch();

    // Inicializar los filtros de categorías
    initCategoryFilters();

    // Inicializar modal de trailer
    initTrailerModal();

    // Aplicar animación escalonada a las tarjetas
    applyStaggeredAnimation();
});

// Función para búsqueda de películas
function initPeliculasSearch() {
    const searchInput = document.getElementById('peliculaSearch');
    if (!searchInput) return;

    searchInput.addEventListener('keyup', function () {
        const searchTerm = this.value.toLowerCase();
        const peliculas = document.querySelectorAll('.pelicula-item');

        peliculas.forEach(pelicula => {
            const titulo = pelicula.querySelector('.card-title').textContent.toLowerCase();
            const sinopsis = pelicula.querySelector('.sinopsis-preview').textContent.toLowerCase();

            if (titulo.includes(searchTerm) || sinopsis.includes(searchTerm)) {
                pelicula.style.display = 'block';
            } else {
                pelicula.style.display = 'none';
            }
        });
    });
}

// Función para filtros de categorías
function initCategoryFilters() {
    const filterButtons = document.querySelectorAll('.category-filters .btn');
    if (filterButtons.length === 0) return;

    filterButtons.forEach(button => {
        button.addEventListener('click', function () {
            // Actualizar botones activos
            filterButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');

            const filter = this.getAttribute('data-filter');
            const peliculas = document.querySelectorAll('.pelicula-item');

            // Implementación básica de filtros (se puede expandir según necesidades)
            if (filter === 'all') {
                peliculas.forEach(pelicula => {
                    pelicula.style.display = 'block';
                });
            } else if (filter === 'recent') {
                // Ejemplo: filtrar películas del último año
                const currentYear = new Date().getFullYear();

                peliculas.forEach(pelicula => {
                    const year = parseInt(pelicula.querySelector('.movie-info span').textContent);
                    if (year >= currentYear - 1) {
                        pelicula.style.display = 'block';
                    } else {
                        pelicula.style.display = 'none';
                    }
                });
            } else if (filter === 'popular') {
                // En un escenario real, esto podría filtrar por calificación o visitas
                // Para este ejemplo, mostraremos todas (se puede personalizar)
                peliculas.forEach(pelicula => {
                    pelicula.style.display = 'block';
                });
            }

            // Volver a aplicar animación después de filtrar
            applyStaggeredAnimation();
        });
    });
}

// Función para inicializar modal de trailer
function initTrailerModal() {
    const trailerButtons = document.querySelectorAll('.trailer-btn');
    const trailerModal = document.getElementById('trailerModal');
    const trailerContainer = document.getElementById('trailerContainer');
    const modalTitle = document.getElementById('trailerModalLabel');

    if (!trailerButtons.length || !trailerModal || !trailerContainer) return;

    // Crear modal de Bootstrap
    const modal = new bootstrap.Modal(trailerModal);

    trailerButtons.forEach(button => {
        button.addEventListener('click', function () {
            const movieId = this.getAttribute('data-id');
            const movieTitle = this.closest('.movie-card').querySelector('.card-title').textContent;

            // Actualizar título del modal
            modalTitle.textContent = `Trailer: ${movieTitle}`;

            // En un escenario real, aquí se cargaría el trailer desde una API
            // Para este ejemplo, usamos un placeholder
            trailerContainer.innerHTML = `
                <div class="d-flex justify-content-center align-items-center bg-dark h-100">
                    <div class="text-center text-white">
                        <i class="fas fa-film fa-3x mb-3"></i>
                        <h5>Trailer no disponible</h5>
                        <p class="mb-0">ID de película: ${movieId}</p>
                    </div>
                </div>
            `;

            // Mostrar modal
            modal.show();
        });
    });

    // Limpiar contenedor del trailer al cerrar el modal
    trailerModal.addEventListener('hidden.bs.modal', function () {
        trailerContainer.innerHTML = '';
    });
}

// Función para animar las tarjetas con efecto escalonado
function applyStaggeredAnimation() {
    const peliculas = document.querySelectorAll('.pelicula-item');

    peliculas.forEach((pelicula, index) => {
        // Resetear la animación primero
        pelicula.style.animation = 'none';
        pelicula.offsetHeight; // Forzar un reflow

        // Aplicar animación con retraso incremental
        pelicula.style.animation = `fadeIn 0.5s ease-out ${index * 0.1}s forwards`;
        pelicula.style.opacity = '0';
    });
}