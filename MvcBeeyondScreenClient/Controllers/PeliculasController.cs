using Microsoft.AspNetCore.Mvc;
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
            List<Pelicula> peliculas = await this.service.GetPeliculasAsync();
            return View(peliculas);
        }
        public async Task<IActionResult> Details
            (int idPelicula)
        {
            ModelDetailsPelicula model = await this.service.GetDetailsPeliculaAsync(idPelicula);
            return View(model);
        }
    }
}
