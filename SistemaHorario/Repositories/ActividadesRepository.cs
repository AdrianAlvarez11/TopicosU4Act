using SistemaHorario.Models;
using SistemaHorario.Viewmodels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.Repositories
{
    public class ActividadesRepository 
    {
        SQLiteConnection conexion;

        public ActividadesRepository() 
        {
            string ruta = FileSystem.AppDataDirectory + "/horario.sqlite";
            conexion = new(ruta);
            conexion.CreateTable<Actividad>();
        }

        public IEnumerable<Actividad> Get(Dias dia)
        {
            return conexion.Table<Actividad>().Where(x=>x.Dia == dia).OrderBy(x=>x.HoraInicio);
        }

    }
}
