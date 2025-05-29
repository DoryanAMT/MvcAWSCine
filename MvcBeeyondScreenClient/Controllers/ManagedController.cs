using MvcBeeyondScreenClient.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MvcBeeyondScreenClient.Services;
using NugetBeeyondScreen.Models;
using NugetBeeyondScreen.Helpers;

namespace MvcBeeyondScreenClient.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceCine service;
        public ManagedController(ServiceCine service)
        {
            this.service = service;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login
            (LoginModel model)
        {
            string token = await this.service.GetTokenAsync(model.UserName, model.Password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
            else
            {
                HttpContext.Session.SetString("TOKEN", token);

                UsuarioModel usuario = await this.service.GetPerfilAsync();

                ClaimsIdentity identity =
                new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, ClaimTypes.Role);

                Claim claimToken = new Claim("TOKEN", token);
                identity.AddClaim(claimToken);

                Claim claimId =
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString());
                identity.AddClaim(claimId);

                Claim claimName =
                    new Claim(ClaimTypes.Name, usuario.Nombre);
                identity.AddClaim(claimName);

                Claim claimEmail =
                    new Claim(ClaimTypes.Email, usuario.Email);
                identity.AddClaim(claimEmail);

                Claim claimImagen =
                    new Claim("Imagen", usuario.Imagen);
                identity.AddClaim(claimImagen);

                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });

                return RedirectToAction("Index", "Peliculas");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Peliculas");
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
