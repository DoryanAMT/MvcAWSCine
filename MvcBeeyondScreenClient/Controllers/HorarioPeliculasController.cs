using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcBeeyondScreenClient.Filters;
using MvcBeeyondScreenClient.Models;
using MvcBeeyondScreenClient.Services;
using NugetBeeyondScreen.Models;

namespace MvcBeeyondScreenClient.Controllers
{
    public class HorarioPeliculasController : Controller
    {
        private ServiceCine service;
        public HorarioPeliculasController(ServiceCine service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<HorarioPelicula> horarioPeliculas = await this.service.GetHorarioPeliculasAsync();
            return View(horarioPeliculas);
        }
        [AuthorizeUsers]
        public async Task<IActionResult> Create()
        {
            OpcionesDTO opciones = await this.service.GetOpcionesASync();
            ViewData["COMBOPELICULAS"] = opciones.Peliculas;
            ViewData["COMBOSALAS"] = opciones.Salas;
            ViewData["COMBOVERSIONES"] = opciones.Versiones;
            List<Evento> eventos = opciones.Eventos;
            ViewData["CALENDARIOHORARIOPELICULAS"] = eventos;

            return View();
        }
        [HttpPost]
        [AuthorizeUsers]
        public async Task<IActionResult> Create
            (HorarioPelicula horarioPelicula)
        {
            await this.service.InserHorarioPeliculaAsync(1, horarioPelicula.IdPelicula,
                horarioPelicula.IdSala, horarioPelicula.IdVersion, horarioPelicula.HoraFuncion,
                horarioPelicula.AsientosDisponibles, horarioPelicula.Estado);
            return RedirectToAction("Index");
        }
        [AuthorizeUsers]
        public async Task<IActionResult> Delete
            (int idHorarioPelicula)
        {
            await this.service.DeleteHorarioPeliculaAsync(idHorarioPelicula);
            return RedirectToAction("Index");
        }

    }
}
