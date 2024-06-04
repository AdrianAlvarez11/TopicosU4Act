using SistemaHorario.Viewmodels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.Models
{
    [Table("Actividades")]
    public class Actividad
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(150)]
        public string Descripcion { get; set; } = null!;

        [NotNull]
        public int HoraInicio { get; set; }
        [NotNull]
        public int HoraFinal {  get; set; }

        [NotNull]
        public Dias Dia { get; set; }
    }
}
