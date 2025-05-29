using MvcBeeyondScreenClient.Filters;
using MvcBeeyondScreenClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetBeeyondScreen.DTOs;
using NugetBeeyondScreen.Models;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcBeeyondScreenClient.Services
{
    public class ServiceCine
    {
        private string ApiUrl;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        

        public ServiceCine
            (IConfiguration configuration,
            IHttpContextAccessor contextAccessor)
        {
            this.ApiUrl =
                configuration.GetValue<string>("ApiUrls:ApiBeeyondScreen");
            this.header = new
                MediaTypeWithQualityHeaderValue("application/json");
            this.contextAccessor = contextAccessor;
        }

        #region Métodos de Películas

        // Obtener detalles de una película
        public async Task<ModelDetailsPelicula> GetDetailsPeliculaAsync(int idPelicula)
        {
            string request = "api/peliculas/"+idPelicula;
            return await this.CallApiAsync<ModelDetailsPelicula>(request);
        }

        // Obtener todas las películas
        public async Task<List<Pelicula>> GetPeliculasAsync()
        {
            string request = "api/peliculas";
            return await this.CallApiAsync<List<Pelicula>>(request);
        }

        // Insertar una película
        public async Task<bool> InsertPeliculaAsync(Pelicula pelicula)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string json = JsonConvert.SerializeObject(pelicula);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("api/peliculas", content);
                return response.IsSuccessStatusCode;
            }
        }

        // Actualizar una película
        public async Task<bool> UpdatePeliculaAsync(Pelicula pelicula)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string json = JsonConvert.SerializeObject(pelicula);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"api/peliculas", content);
                return response.IsSuccessStatusCode;
            }
        }

        // Eliminar una película
        public async Task<bool> DeletePeliculaAsync(int idPelicula)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                HttpResponseMessage response = await client.DeleteAsync($"api/peliculas/{idPelicula}");
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<OpcionesDTO> GetOpcionesASync()
        {
            string request = "api/horariopeliculas/opciones";
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            return await this.CallApiAsync<OpcionesDTO>(request,token);
        }

        #endregion

        #region Métodos de Usuarios

        // Obtener boletos de un usuario
        [AuthorizeUsers]
        public async Task<List<ViewFacturaBoleto>> GetFacturasBoletoUserAsync()
        {
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            string request = "api/usuarios/boletos";
            return await this.CallApiAsync<List<ViewFacturaBoleto>>(request,token);
        }

        // Obtener una factura específica de un usuario
        [AuthorizeUsers]
        public async Task<ViewFacturaBoleto> GetFacturaBoletoUserAsync(int idBoletoUser)
        {
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            string request = "api/usuarios/boletos/"+idBoletoUser;
            return await this.CallApiAsync<ViewFacturaBoleto>(request,token);
        }

        // Obtener el último ID de usuario
        public async Task<int> GetLastIdUserAsync()
        {
            string request = "api/usuarios/lastid";
            return await this.CallApiAsync<int>(request);
        }

        // Obtener todos los usuarios
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            string request = "api/usuarios";
            return await this.CallApiAsync<List<Usuario>>(request);
        }

        // Encontrar un usuario por ID
        public async Task<Usuario> FindUsuarioAsync(int idUsuario)
        {
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            string request = "api/usuarios/"+idUsuario;
            return await this.CallApiAsync<Usuario>(request, token);
        }

        // Actualizar perfil de usuario sin cambiar contraseña
        public async Task UpdateUsuarioProfileAsync(int idUsuario, string nombre, string email, string imagen, bool cambiarPassword)
        {
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            string request = "api/usuarios/perfil?cambiarPassword="+cambiarPassword;

            UsuarioModel usuarioData = new UsuarioModel
            {
                IdUsuario = idUsuario,
                Nombre = nombre,
                Email = email,
                Imagen = imagen
            };

            string json = JsonConvert.SerializeObject(usuarioData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            await this.CallApiAsync<UsuarioModel>(request, token, content);
        }

        // Actualizar perfil de usuario con cambio de contraseña
        public async Task UpdateUsuarioAsync
            (int idUsuario, string nombre, string email, string imagen, string currentPassword, string newPassword,string confirmPassword, bool cambiarPassword)
        {
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            string request = "api/usuarios/perfil?cambiarPassword="+cambiarPassword+ "&currentPassword="+currentPassword+ "&newPassword="+newPassword+ "&confirmPassword="+confirmPassword;

            UsuarioModel usuarioData = new UsuarioModel
            {
                IdUsuario = idUsuario,
                Nombre = nombre,
                Email = email,
                Imagen = imagen,
            };

            string json = JsonConvert.SerializeObject(usuarioData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            await this.CallApiAsync<Usuario>(request, token, content);
        }

        // Registrar nuevo usuario
        public async Task RegisterUserAsync(string nombre, string email, string password, string imagen)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string request = "api/usuarios/register";

                RegisterDTO usuarioData = new RegisterDTO
                {
                    Nombre = nombre,
                    Email = email,
                    Password = password,
                    Imagen = imagen
                };

                string json = JsonConvert.SerializeObject(usuarioData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Error al registrar usuario: " + response.StatusCode);
                }
            }
        }

        // Login de usuario
        public async Task<string> GetTokenAsync(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string request = "api/auth/login";

                LoginModel loginData = new LoginModel
                {
                    UserName = email,
                    Password = password
                };

                string json = JsonConvert.SerializeObject(loginData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content
                        .ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<UsuarioModel> GetPerfilAsync()
        {
            string token =
                this.contextAccessor.HttpContext.Session.GetString("TOKEN");
            string request = "api/usuarios/perfil";
            UsuarioModel usuario = await
                this.CallApiAsync<UsuarioModel>(request, token);
            return usuario;
        }

        #endregion

        #region Métodos de HorarioPelicula
        public async Task<List<HorarioPelicula>> GetHorarioPeliculasAsync()
        {
            string request = "api/horariopeliculas";
            return await this.CallApiAsync<List<HorarioPelicula>>(request);
        }
        public async Task InserHorarioPeliculaAsync
            (int idHorarioPelicula, int idPelicula, int idSala, int idVersion, DateTime horaFuncion, int asientosDisponibles, char estado)
        {
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            string request = "api/horariopeliculas";

            HorarioPelicula horarioPelicula = new HorarioPelicula();
            horarioPelicula.IdHorario = idHorarioPelicula;
            horarioPelicula.IdPelicula = idPelicula;
            horarioPelicula.IdSala = idSala;
            horarioPelicula.IdVersion = idVersion;
            horarioPelicula.HoraFuncion = horaFuncion;
            horarioPelicula.AsientosDisponibles = asientosDisponibles;
            horarioPelicula.Estado = estado;

            string json = JsonConvert.SerializeObject(horarioPelicula);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            await this.PostApiAsync<Usuario>(request, token, content);
        }

        public async Task DeleteHorarioPeliculaAsync
            (int IdHorario)
        {
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            string request = "api/HorarioPeliculas/" + IdHorario;
            await this.DeleteCallApiAsync(request, token);
        }
        #endregion

        #region Métodos Asientos
        public async Task<ModelAsientosReserva> ReservaAsientoSalaHorarioIdAsync
            (int id)
        {
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            string request = "api/asientos/reserva/" + id;
            return await this.CallApiAsync<ModelAsientosReserva>(request, token);
        }

        public async Task AsientosReservaAsync
            (Asiento asiento)
        {
            string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
            int idUsuario = int.Parse(this.contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string request = "api/asientos/reserva?usuarioId="+idUsuario;

            string json = JsonConvert.SerializeObject(asiento);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            await this.PostApiAsync<Usuario>(request, token, content);
        }



        #endregion
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response = await
                    client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        private async Task DeleteCallApiAsync
        (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.DeleteAsync(request);
            }
        }
        private async Task<T> CallApiAsync<T>
        (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T>
        (string request, string token, StringContent content)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.PutAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        private async Task<T> PostApiAsync<T>
        (string request, string token, StringContent content)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        //private async Task<T> CallApiAsync<T>(string request, bool requiresAuth = false)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(this.ApiUrl);
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(this.header);

        //        // Si se requiere autenticación y tenemos un token, lo añadimos
        //        if (requiresAuth && !string.IsNullOrEmpty(this.AuthToken))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", "bearer " + this.AuthToken);
        //        }

        //        HttpResponseMessage response = await client.GetAsync(request);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            T data = await response.Content.ReadAsAsync<T>();
        //            return data;
        //        }
        //        else
        //        {
        //            return default(T);
        //        }
        //    }
        //}

    }
}
