using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AppMartin.Models
{
    public class Respuesta
    {
        public int RespuestaID { get; set; }

        [DisplayName("Respuesta")]
        public string Contenido { get; set; }

        public virtual ICollection<Pregunta> Preguntas { get; set; }
    }
}