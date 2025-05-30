using System.ComponentModel.DataAnnotations.Schema;
using NugetBeeyondScreen.Models;

namespace MvcBeeyondScreenClient.Models
{
    public class HorarioDTO
    {
        public int IdHorario { get; set; }
        [ForeignKey("IDPELICULA")]
        public int IdPelicula { get; set; }
        public DateTime Hora { get; set; }
        public int Sala { get; set; }
        public int Aforo { get; set; }
        public string Estado { get; set; }

        public Pelicula Pelicula { get; set; }
    }
}
