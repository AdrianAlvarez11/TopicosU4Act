﻿using SistemaHorario.Models;
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

        public IEnumerable<Actividad> GetByDay(Dias dia)
        {
            return conexion.Table<Actividad>().Where(x=>x.Dia == dia);
        }

        public void Insert(Actividad act)
        {
            conexion.Insert(act);
        }
        public void Update(Actividad act) 
        { 
            conexion.Update(act); 
        }
        public void Delete(Actividad act) 
        { 
            conexion.Delete(act);
        }

    }
}
