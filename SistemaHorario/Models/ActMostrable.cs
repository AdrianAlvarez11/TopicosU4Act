using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.Models
{
    public class ActMostrable
    {
        public int HoraInicio { get; set; }
        public int HoraFinal { get; set; }

        public string Detalles { get; set; } = "";

    }
}
