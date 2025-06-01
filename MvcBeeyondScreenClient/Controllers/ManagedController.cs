using MvcBeeyondScreenClient.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MvcBeeyondScreenClient.Services;
using NugetBeeyondScreen.Models;
using NugetBeeyondScreen.Helpers;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

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

                // Deserializar el token JWT para extraer claims adicionales
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Extraer los claims específicos del token
                string? jsonUserData = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserData")?.Value;
                UsuarioModel? userData = jsonUserData != null ? JsonConvert.DeserializeObject<UsuarioModel>(jsonUserData) : null;
                string? role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                ClaimsIdentity identity =
                new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, ClaimTypes.Role);

                Claim claimToken = new Claim("TOKEN", token);
                identity.AddClaim(claimToken);

                Claim claimId =
                    new Claim(ClaimTypes.NameIdentifier, userData.IdUsuario.ToString());
                identity.AddClaim(claimId);

                Claim claimName =
                    new Claim(ClaimTypes.Name, userData.Nombre);
                identity.AddClaim(claimName);

                Claim claimEmail =
                    new Claim(ClaimTypes.Email, userData.Email);
                identity.AddClaim(claimEmail);

                Claim claimImagen =
                    new Claim("Imagen", userData.Imagen);
                identity.AddClaim(claimImagen);

                if(role != null)
                {
                    Claim claimRole = new Claim(ClaimTypes.Role, role);
                    identity.AddClaim(claimRole);
                }

                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });
                string controller =
                TempData["controller"]?.ToString();
                string action =
                    TempData["action"]?.ToString();
                if (TempData["id"] != null)
                {
                    string id =
                        TempData["id"].ToString();
                    return RedirectToAction(action, controller
                        , new { id = id });
                }
                else
                {
                    return RedirectToAction(action, controller);
                }

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
