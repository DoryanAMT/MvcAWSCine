using Microsoft.AspNetCore.Mvc;
using MvcBeeyondScreenClient.Models;
using MvcBeeyondScreenClient.Services;
using NugetBeeyondScreen.Models;

namespace MvcBeeyondScreenClient.Controllers
{
    public class PeliculasController : Controller
    {
        private ServiceCine service;
        public PeliculasController(ServiceCine service)
        {
            this.service = service;
        }
        public async Task<IActionResult> Index()
        {
            List<ModelDetailsPelicula> peliculas = await this.service.GetPeliculasConHorariosAsync();
            return View(peliculas);
        }
        public async Task<IActionResult> Details
            (int idPelicula)
        {
            ModelDetailsPelicula model = await this.service.GetDetailsPeliculaAsync(idPelicula);
            return View(model);
        }

        //Gestion del index del Admin
        public async Task<IActionResult> IndexAdmin()
        {
            var peliculas = await this.service.GetPeliculasAsync();
            var horarios = await this.service.GetHorarioPeliculasAsync();

            var dto = horarios.Select(h => new HorarioDTO
            {
                IdHorario = h.IdHorario,
                IdPelicula = h.IdPelicula,
                Hora = h.HoraFuncion,
                Sala = h.IdSala,
                Aforo = h.AsientosDisponibles,
                Estado = h.Estado.ToString(),
                Pelicula = h.IdPelicula != null ? peliculas.FirstOrDefault(p => p.IdPelicula == h.IdPelicula) : null
            }).ToList();

            var model = new IndexAdmin
            {
                Peliculas = peliculas,
                HorarioPelicula = dto
            };

            return View(model);
        }

        //Edit peli
        public async Task<IActionResult> UpdatePelicula(Pelicula peli)
        {
            await this.service.UpdatePeliculaAsync(peli);
            return RedirectToAction("IndexAdmin");
        }

        //Delete peli
        public async Task<IActionResult> DeletePelicula(int idPelicula)
        {
            await this.service.DeletePeliculaAsync(idPelicula);
            return RedirectToAction("IndexAdmin");
        }
        //Create peli
        public async Task<IActionResult> CreatePelicula(Pelicula peli)
        {
            await this.service.InsertPeliculaAsync(peli);
            return RedirectToAction("IndexAdmin");
        }
    }
}
