﻿using NugetBeeyondScreen.Models;

namespace MvcBeeyondScreenClient.Models
{
    public class IndexAdmin
    {
        public List<Pelicula> Peliculas { get; set; }
        public List<HorarioDTO> HorarioPelicula { get; set; }
    }
}
