using SistemaHorario.Models;
using SistemaHorario.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SistemaHorario.Viewmodels
{

    public enum Dias { Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo}
    public class HorarioViewmodel : INotifyPropertyChanged
    {
        private Dias diaSeleccionado;

        public ObservableCollection<ActMostrable> ActMostrables { get; set; } = new();
        public ActividadesRepository repoActividades { get; set; } = new();
        public ClasesRepository repoClases { get; set; } = new();
        public IEnumerable<Dias> ValoresDias => Enum.GetValues(typeof(Dias)).Cast<Dias>();
        public IEnumerable<int> ValoresHoras => Enumerable.Range(0, 25).ToList();
        public Dias DiaSeleccionado
        {
            get { return diaSeleccionado; }
            set { 
                diaSeleccionado = value;
                HacerMostrable();
            }
        }

        public string Error { get; set; } = "";

        public Clase Clase { get; set; }
        public Actividad Actividad { get; set; }

        public ICommand VerAgregarActividadCommand { get; set; }
        public ICommand VerAgregarClaseCommand { get; set; }
        public ICommand AgregarClaseCommand { get; set; }
        public ICommand AgregarActividadCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand VerEditarCommand { get; set; }
        public ICommand EditarClaseCommand { get; set; }
        public ICommand EditarActividadCommand { get; set; }
        public ICommand VerEliminarCommand { get; set; }
        public ICommand EliminarClaseCommand { get; set; }
        public ICommand EliminarActividadCommand { get; set; }


        public HorarioViewmodel() 
        {
            VerAgregarActividadCommand = new Command(VerAgregarActividad);
            VerAgregarClaseCommand = new Command(VerAgregarClase);
            AgregarActividadCommand = new Command(AgregarActividad);
            AgregarClaseCommand = new Command(AgregarClase);
            CancelarCommand = new Command(Cancelar);
            VerEditarCommand = new Command<ActMostrable>(VerEditar);
            EditarActividadCommand = new Command(EditarActividad);
            EditarClaseCommand = new Command(EditarClase);
            VerEliminarCommand = new Command<ActMostrable>(VerEliminar);
            EliminarActividadCommand = new Command(EliminarActividad);
            EliminarClaseCommand = new Command(EliminarClase);

            HacerMostrable();

        }

        private void EliminarClase()
        {
            if(Clase!=null)
            {
                repoClases.Delete(Clase);
                HacerMostrable();
                Cancelar();

            }
        }

        private void EliminarActividad()
        {
            if (Actividad != null)
            {
                repoActividades.Delete(Actividad);
                HacerMostrable();
                Cancelar();
            } 
        }

        private void VerEliminar(ActMostrable mostrable)
        {
            Actividad = new();
            Clase = new();

            //verificar si el objeto mostrable era de tipo actividad o clase para borrarla en la BD

            bool esActividad = repoActividades.GetByDay(DiaSeleccionado).Any(x => x.HoraInicio == mostrable.HoraInicio);

            if (esActividad)
            {
                Actividad = (Actividad)repoActividades.GetByDay(DiaSeleccionado).Where(x => x.HoraInicio == mostrable.HoraInicio).First();
            }
            else
            {
                Clase = (Clase)repoClases.GetByDay(DiaSeleccionado).Where(x => x.HoraInicio == mostrable.HoraInicio).First();
            }

            Actualizar();

            if (Actividad.Descripcion != "") //verificar cual de los dos se quedó vacío
            {
                Shell.Current.GoToAsync("//EliminarActividad");
            }

            if (Clase.Nombre != "") //verificar cual de los dos se quedó vacío
            {
                Shell.Current.GoToAsync("//EliminarClase");
            }
        }

        private void EditarClase()
        {
            Error = "";
            if (Clase != null)
            {
                if (string.IsNullOrWhiteSpace(Clase.Nombre))
                {
                    Error += "Introduzca el nombre de la clase\n";
                }
                if (string.IsNullOrWhiteSpace(Clase.Maestro))
                {
                    Error += "Introduzca el nombre del maestro\n";
                }
                if (string.IsNullOrWhiteSpace(Clase.Aula))
                {
                    Error += "Introduzca el aula donde tomará la clase\n";
                }
                if (Clase.HoraInicio < 0 || Clase.HoraInicio > 23 || Clase.HoraFinal < 1 || Clase.HoraFinal > 24)
                {
                    Error += "Introduzca una hora válida en formato 24 horas\n";
                }
                if (Clase.HoraInicio > Clase.HoraFinal)
                {
                    Error += "La hora final no puede ser más temprano que la hora de inicio\n";
                }

                bool ClaseEmpalmada = repoClases.GetByDay(Clase.Dia).Any(c =>
                 ((Clase.HoraInicio >= c.HoraInicio && Clase.HoraInicio < c.HoraFinal) ||
                 (Clase.HoraFinal > c.HoraInicio && Clase.HoraFinal <= c.HoraFinal)) && c.Id!=Clase.Id);

                bool ActEmpalmada = repoActividades.GetByDay(Clase.Dia).Any(a =>
                 ((Clase.HoraInicio >= a.HoraInicio && Clase.HoraInicio < a.HoraFinal) ||
                 (Clase.HoraFinal > a.HoraInicio && Clase.HoraFinal <= a.HoraFinal)) && a.Id != Clase.Id);

                if (ClaseEmpalmada || ActEmpalmada)
                {
                    Error += "Estas horas están ocupadas\n";
                }

                Actualizar(nameof(Error));

                if (Error == "")
                {
                    repoClases.Update(Clase);
                    HacerMostrable();
                    Cancelar();

                }

            }
        }

        private void EditarActividad()
        {
            Error = "";
            if (Actividad != null)
            {
                if (string.IsNullOrWhiteSpace(Actividad.Descripcion))
                {
                    Error += "Introduzca la descripción de la actividad\n";
                }

                if (Actividad.HoraInicio < 0 || Actividad.HoraInicio > 23 || Actividad.HoraFinal < 1 || Actividad.HoraFinal > 24)
                {
                    Error += "Introduzca una hora válida en formato 24 horas\n";
                }
                if (Actividad.HoraInicio > Actividad.HoraFinal)
                {
                    Error += "La hora final no puede ser más temprano que la hora de inicio\n";
                }

                bool ClaseEmpalmada = repoClases.GetByDay(Actividad.Dia).Any(c =>
                 ((Actividad.HoraInicio >= c.HoraInicio && Actividad.HoraInicio < c.HoraFinal) ||
                 (Actividad.HoraFinal > c.HoraInicio && Actividad.HoraFinal <= c.HoraFinal)) && c.Id != Actividad.Id);

                bool ActEmpalmada = repoActividades.GetByDay(Actividad.Dia).Any(a =>
                 ((Actividad.HoraInicio >= a.HoraInicio && Actividad.HoraInicio < a.HoraFinal) ||
                 (Actividad.HoraFinal > a.HoraInicio && Actividad.HoraFinal <= a.HoraFinal)) && a.Id != Actividad.Id);

                if (ClaseEmpalmada || ActEmpalmada)
                {
                    Error += "Estas horas están ocupadas\n";
                }

                Actualizar(nameof(Error));

                if (Error == "")
                {
                    repoActividades.Update(Actividad);
                    HacerMostrable();
                    Cancelar();

                }

            }
        }

        private void VerEditar(ActMostrable mostrable)
        {
            Actividad = new();
            Clase = new();

            //verificar si el objeto mostrable era de tipo actividad o clase para poder editarlo en la BD

            bool esActividad = repoActividades.GetByDay(DiaSeleccionado).Any(x => x.HoraInicio == mostrable.HoraInicio);

            if (esActividad) 
            {
                Actividad = (Actividad)repoActividades.GetByDay(DiaSeleccionado).Where(x => x.HoraInicio == mostrable.HoraInicio).First();
            }
            else
            {
                Clase = (Clase)repoClases.GetByDay(DiaSeleccionado).Where(x => x.HoraInicio == mostrable.HoraInicio).First();
            }

            Actualizar();

            if (Actividad.Descripcion!="") //verificar cual de los dos se quedó vacío
            {
                Shell.Current.GoToAsync("//EditarActividad");
            }

            if(Clase.Nombre!="") //verificar cual de los dos se quedó vacío
            {
                Shell.Current.GoToAsync("//EditarClase");
            }
        }

        private void Cancelar()
        {
            Error = "";
            Actualizar(nameof(Error));
            Shell.Current.GoToAsync("//Horario");
            
        }

        private void AgregarClase()
        {
            Error = "";
            if (Clase != null)
            {
                if (string.IsNullOrWhiteSpace(Clase.Nombre))
                {
                    Error += "Introduzca el nombre de la clase\n";
                }
                if (string.IsNullOrWhiteSpace(Clase.Maestro))
                {
                    Error += "Introduzca el nombre del maestro\n";
                }
                if (string.IsNullOrWhiteSpace(Clase.Aula))
                {
                    Error += "Introduzca el aula donde tomará la clase\n";
                }
                if(Clase.HoraInicio <0 || Clase.HoraInicio >23 || Clase.HoraFinal < 1 || Clase.HoraFinal > 24)
                {
                    Error += "Introduzca una hora válida en formato 24 horas\n";
                }
                if (Clase.HoraInicio > Clase.HoraFinal)
                {
                    Error += "La hora final no puede ser más temprano que la hora de inicio\n";
                }

                bool ClaseEmpalmada = repoClases.GetByDay(Clase.Dia).Any(c =>
                 (Clase.HoraInicio >= c.HoraInicio && Clase.HoraInicio < c.HoraFinal) ||
                 (Clase.HoraFinal > c.HoraInicio && Clase.HoraFinal <= c.HoraFinal));

                bool ActEmpalmada = repoActividades.GetByDay(Clase.Dia).Any(a =>
                 (Clase.HoraInicio >= a.HoraInicio && Clase.HoraInicio < a.HoraFinal) ||
                 (Clase.HoraFinal > a.HoraInicio && Clase.HoraFinal <= a.HoraFinal));

                if (ClaseEmpalmada || ActEmpalmada)
                {
                    Error += "Estas horas están ocupadas\n";
                }

                Actualizar(nameof(Error));

                if (Error == "")
                {
                    repoClases.Insert(Clase);
                    HacerMostrable();
                    Cancelar();
                    
                }

            }
        }

        private void HacerMostrable()
        {
            ActMostrables.Clear();

            List<ActMostrable> listaMostrable = new();

            var clases = repoClases.GetByDay(DiaSeleccionado).ToList();
            foreach ( var c in clases)
            {
                ActMostrable nuevo = new ActMostrable()
                {
                    Detalles = $"{c.Nombre}\n{c.Maestro}\n{c.Aula}",
                    HoraInicio = c.HoraInicio,
                    HoraFinal = c.HoraFinal

                };

                listaMostrable.Add(nuevo);
            }

            var actividades = repoActividades.GetByDay(DiaSeleccionado).ToList();
            foreach (var a in actividades)
            {
                ActMostrable nuevo = new ActMostrable()
                {
                    Detalles = a.Descripcion,
                    HoraInicio = a.HoraInicio,
                    HoraFinal = a.HoraFinal

                };
                listaMostrable.Add(nuevo);
            }

            var ordenado = listaMostrable.OrderBy(x=>x.HoraInicio).ToList();
            foreach (var item in ordenado)
            {
                ActMostrables.Add(item);
            }
        }

        private void AgregarActividad()
        {
            Error = "";
            if (Actividad != null)
            {
                if (string.IsNullOrWhiteSpace(Actividad.Descripcion))
                {
                    Error += "Introduzca la descripción de la actividad\n";
                }
               
                if (Actividad.HoraInicio < 0 || Actividad.HoraInicio > 23 || Actividad.HoraFinal < 1 || Actividad.HoraFinal > 24)
                {
                    Error += "Introduzca una hora válida en formato 24 horas\n";
                }
                if (Actividad.HoraInicio > Actividad.HoraFinal)
                {
                    Error += "La hora final no puede ser más temprano que la hora de inicio\n";
                }

                bool ClaseEmpalmada = repoClases.GetByDay(Actividad.Dia).Any(c =>
                 (Actividad.HoraInicio >= c.HoraInicio && Actividad.HoraInicio < c.HoraFinal) ||
                 (Actividad.HoraFinal > c.HoraInicio && Actividad.HoraFinal <= c.HoraFinal));

                bool ActEmpalmada = repoActividades.GetByDay(Actividad.Dia).Any(a =>
                 (Actividad.HoraInicio >= a.HoraInicio && Actividad.HoraInicio < a.HoraFinal) ||
                 (Actividad.HoraFinal > a.HoraInicio && Actividad.HoraFinal <= a.HoraFinal));

                if (ClaseEmpalmada || ActEmpalmada)
                {
                    Error += "Estas horas están ocupadas\n";
                }

                Actualizar(nameof(Error));

                if (Error == "")
                {
                    repoActividades.Insert(Actividad);
                    HacerMostrable();
                    Cancelar();
                    
                }

            }
        }

        private void VerAgregarClase()
        {
            Error = "";
            Clase = new();
            Actualizar();
            Shell.Current.GoToAsync("//VerAggClase");
            
        }

        private void VerAgregarActividad()
        {
            Error = "";
            Actividad = new();
            Actualizar();
            Shell.Current.GoToAsync("//VerAggAct");
        }

        public void Actualizar(string? name = null)
        {
            PropertyChanged?.Invoke(this, new(name));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
