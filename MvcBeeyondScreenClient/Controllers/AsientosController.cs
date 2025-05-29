using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcBeeyondScreenClient.Filters;
using MvcBeeyondScreenClient.Services;
using NugetBeeyondScreen.Models;

namespace MvcBeeyondScreenClient.Controllers
{
    public class AsientosController : Controller
    {
        private ServiceCine service;
        public AsientosController(ServiceCine service)
        {
            this.service = service;
        }
        [AuthorizeUsers]
        public async Task<IActionResult> AsientosReserva
            (int idHorario)
        {
            ModelAsientosReserva model = await this.service.ReservaAsientoSalaHorarioIdAsync(idHorario);
            return View(model);
        }
        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> AsientosReserva
            (Asiento asiento)
        {
            await this.service.AsientosReservaAsync(asiento);
            return RedirectToAction("Index", "PeliculasController");
        }

    }
}
