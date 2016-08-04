using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppMartin.ViewModels
{
    public class RespuestasPorPregunta
    {
        public int RespuestaID { get; set; }
        public string Contenido { get; set; }
        public bool Seleccionado { get; set; }
    }
}