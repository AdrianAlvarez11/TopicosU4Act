using SistemaHorario.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.Viewmodels
{

    public enum Dias { Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo}
    public class HorarioViewmodel
    {
        public ObservableCollection<ActMostrable> ActMostrables { get; set; } = new();
        public IEnumerable<Dias> ValoresDias => Enum.GetValues(typeof(Dias)).Cast<Dias>();
        public Dias DiaSeleccionado { get; set; } = Dias.Lunes;

        public string Error { get; set; } = "";

        public Clase Clase { get; set; }
        public Actividad Actividad { get; set; }

        public HorarioViewmodel() 
        {
            ActMostrable x = new()
            {
                Detalles = "pene",
                HoraFinal = 1,
                HoraInicio = 2
            };
            ActMostrables.Add(x);

            
        }
    }
}
