using Microsoft.AspNetCore.Mvc;
using MvcBeeyondScreenClient.Filters;
using MvcBeeyondScreenClient.Services;
using NugetBeeyondScreen.Helpers;
using NugetBeeyondScreen.Models;
using System.Security.Claims;

namespace MvcBeeyondScreenClient.Controllers
{
    public class UsuariosController : Controller
    {
        private ServiceCine service;
        public UsuariosController(ServiceCine service)
        {
            this.service = service;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register
            (string nombre, string email, string imagen, string password)
        {
            await this.service.RegisterUserAsync(nombre, email, password, imagen);
            ViewData["MENSAJE"] = "Usuario registrado correctamente";
            return View();
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Perfil()
        {
            UsuarioModel usuario = await
                this.service.GetPerfilAsync();
            return View(usuario);
        }
        // **** CORREGIR CAMBIAR CONTRASEÑA
        [HttpPost]
        [AuthorizeUsers]
        public async Task<IActionResult> Perfil
            (Usuario usuario, string currentPassword,
            string newPassword, string confirmPassword, bool cambiarPassword)
        {
            try
            {
                int dato = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                // Obtener el usuario actual de la base de datos
                Usuario usuarioActual = await this.service.FindUsuarioAsync(dato);

                // Si se solicitó cambiar la contraseña
                if (cambiarPassword == true)
                {
                    // Verificar la contraseña actual
                    byte[] passActual = HelperCryptography.EncryptPassword(currentPassword, usuarioActual.Salt);
                    bool passCorrecta = HelperCryptography.CompararArrays(passActual, usuarioActual.Pass);

                    if (!passCorrecta)
                    {
                        ViewData["ERROR"] = "La contraseña actual es incorrecta";
                        return View();
                    }

                    // Verificar que las nuevas contraseñas coincidan
                    if (newPassword != confirmPassword)
                    {
                        ViewData["ERROR"] = "Las nuevas contraseñas no coinciden";
                        return View();
                    }

                    // Actualizar el usuario con la nueva contraseña
                    await this.service.UpdateUsuarioAsync(
                        dato,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Imagen,
                        currentPassword,
                        newPassword,
                        confirmPassword,
                        cambiarPassword
                    );
                }
                else
                {
                    // Actualizar el usuario sin cambiar la contraseña
                    await this.service.UpdateUsuarioProfileAsync(
                        dato,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Imagen,
                        cambiarPassword
                    );
                }

                // Actualizar la sesión si es el usuario actual
                // Actualizar los claims

                ViewData["MENSAJE"] = "Perfil actualizado correctamente";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ERROR"] = "Error al actualizar el perfil: " + ex.Message;
                return View(usuario);


            }
        }
        [AuthorizeUsers]
        public async Task<IActionResult> BoletosUser()
        {
            //int idUsuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<ViewFacturaBoleto> viewFacturaBoletos = await this.service.GetFacturasBoletoUserAsync();
            return View(viewFacturaBoletos);
        }
        [AuthorizeUsers]
        public async Task<IActionResult> DetailsBoletoUser
            (int idBoletoUser)
        {
            ViewFacturaBoleto viewFacturaBoleto = await this.service.GetFacturaBoletoUserAsync(idBoletoUser);
            return View(viewFacturaBoleto);
        }
    }
}
