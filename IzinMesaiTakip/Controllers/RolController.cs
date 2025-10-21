using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using IzinMesaiTakip.Models;

namespace IzinMesaiTakip.Controllers
{
    public class RolController : Controller
    {
        private IzinMesaiTakipEntities db = new IzinMesaiTakipEntities();

        // GET: Rol
        public ActionResult Index()
        {
            var roller = db.Rol.ToList();
            return View(roller);
        }

        // GET: Rol/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rol = db.Rol.Find(id);
            if (rol == null)
                return HttpNotFound();

            return View(rol);
        }

        // GET: Rol/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rol/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Rol rol)
        {
            if (ModelState.IsValid)
            {
                db.Rol.Add(rol);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rol);
        }

        // GET: Rol/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rol = db.Rol.Find(id);
            if (rol == null)
                return HttpNotFound();

            return View(rol);
        }

        // POST: Rol/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Rol rol)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rol).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rol);
        }

        // GET: Rol/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rol = db.Rol.Find(id);
            if (rol == null)
                return HttpNotFound();

            return View(rol);
        }

        // POST: Rol/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var rol = db.Rol.Find(id);
            db.Rol.Remove(rol);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult Listele()
        {
            var roller = db.Rol.Select(r => new
            {
                r.RolID,
                r.RolAdi
            }).ToList();

            return Json(roller, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}