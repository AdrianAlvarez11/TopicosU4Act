using SistemaHorario.Viewmodels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.Models
{
    [Table("Clases")]
    public class Clase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [NotNull, MaxLength(100)]
        public string Maestro { get; set; } = null!;

        [NotNull]
        public string Aula { get; set; } = null!;

        [NotNull]
        public int HoraInicio { get; set; } 

        [NotNull]
        public int HoraFinal {  get; set; }

        [NotNull]
        public Dias Dia { get; set; }



    }
}
