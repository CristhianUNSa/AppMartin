using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AppMartin.Models
{
    public class Pregunta
    {
        public int PreguntaID { get; set; }
        public string Contenido { get; set; }

        [DisplayName("Respuesta Correcta")]
        public int RespuestaID { get; set; }

        [ForeignKey("RespuestaID")]
        public virtual Respuesta RespuestaCorrecta { get; set; }

        public virtual ICollection<Respuesta> Respuestas { get; set; }
    }
}