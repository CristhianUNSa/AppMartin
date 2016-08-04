using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppMartin.Models;
using AppMartin.ViewModels;

namespace AppMartin.Controllers
{
    public class PreguntasController : Controller
    {
        private AppMartinContext db = new AppMartinContext();

        // GET: Preguntas
        public ActionResult Index()
        {
            var preguntas = db.Preguntas.Include(p => p.RespuestaCorrecta);
            return View(preguntas.ToList());
        }

        // GET: Preguntas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta pregunta = db.Preguntas.Find(id);
            if (pregunta == null)
            {
                return HttpNotFound();
            }
            return View(pregunta);
        }

        // GET: Preguntas/Create
        public ActionResult Create()
        {
            ViewBag.RespuestaID = new SelectList(db.Respuestas, "RespuestaID", "Contenido");
            return View();
        }

        // POST: Preguntas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PreguntaID,Contenido,RespuestaID")] Pregunta pregunta)
        {
            if (ModelState.IsValid)
            {
                db.Preguntas.Add(pregunta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RespuestaID = new SelectList(db.Respuestas, "RespuestaID", "Contenido", pregunta.RespuestaID);
            return View(pregunta);
        }

        // GET: Preguntas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta pregunta = db.Preguntas
                                .Include(r => r.Respuestas)
                                .Where(p => p.PreguntaID == id)
                                .Single();
            if (pregunta == null)
            {
                return HttpNotFound();
            }
            PopulateRespuestasPosibles(pregunta);
            ViewBag.RespuestaID = new SelectList(db.Respuestas, "RespuestaID", "Contenido", pregunta.RespuestaID);
            return View(pregunta);
        }

        private void PopulateRespuestasPosibles(Pregunta pregunta)
        {
            var todasRespuestas = db.Respuestas;
            var respuestasDeLaPregunta = new HashSet<int>(pregunta.Respuestas.Select(r => r.RespuestaID));
            var viewModel = new List<RespuestasPorPregunta>();
            foreach (var respuesta in todasRespuestas)
            {
                viewModel.Add(new RespuestasPorPregunta()
                {
                    RespuestaID = respuesta.RespuestaID,
                    Contenido = respuesta.Contenido,
                    Seleccionado = respuestasDeLaPregunta.Contains(respuesta.RespuestaID)
                });
            }
            ViewBag.Respuestas = viewModel;
        }

        // POST: Preguntas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PreguntaID,Contenido,RespuestaID")] int? id, string[] respuestasSeleccionadas)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta preguntaAEditar = db.Preguntas
                                        .Include(r => r.Respuestas)
                                        .Where(p => p.PreguntaID == id)
                                        .Single();
            if (TryUpdateModel(preguntaAEditar, "", new string[] { "Contenido" }))
            {
                try
                {

                    ModificarRespuestas(respuestasSeleccionadas, preguntaAEditar);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "No se pudo guardar, intente de nuevo y si el problema persiste, comuniquese con un administrador.");
                }
            }

            ViewBag.RespuestaID = new SelectList(db.Respuestas, "RespuestaID", "Contenido", preguntaAEditar.RespuestaID);


            return View(preguntaAEditar);
        }

        private void ModificarRespuestas(string[] respuestasSeleccionadas, Pregunta preguntaAEditar)
        {
            if(respuestasSeleccionadas == null)
            {
                preguntaAEditar.Respuestas = new List<Respuesta>();
                return;
            }
            var respuestasSeleccionadasHS = new HashSet<string>(respuestasSeleccionadas);
            var respuestasDeLaPregunta = new HashSet<int>(
                preguntaAEditar.Respuestas.Select(r=>r.RespuestaID));
            foreach (var respuesta in db.Respuestas)
            {
                if (respuestasSeleccionadasHS.Contains(respuesta.RespuestaID.ToString()))
                {
                    if (!respuestasDeLaPregunta.Contains(respuesta.RespuestaID))
                    {
                        preguntaAEditar.Respuestas.Add(respuesta);
                    }
                }
                else
                {
                    if (respuestasDeLaPregunta.Contains(respuesta.RespuestaID))
                    {
                        preguntaAEditar.Respuestas.Remove(respuesta);
                    }
                }
            }
        }

        // GET: Preguntas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta pregunta = db.Preguntas.Find(id);
            if (pregunta == null)
            {
                return HttpNotFound();
            }
            return View(pregunta);
        }

        // POST: Preguntas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pregunta pregunta = db.Preguntas.Find(id);
            db.Preguntas.Remove(pregunta);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
