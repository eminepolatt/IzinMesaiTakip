using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using IzinMesaiTakip.Models;
using IzinMesaiTakip.Filters;

namespace IzinMesaiTakip.Controllers
{
    [AuthorizationFilter("Yönetici", "Admin")]
    public class KullaniciController : Controller
    {
        private IzinMesaiTakipEntities db = new IzinMesaiTakipEntities();

        // GET: Kullanici
        public ActionResult Index()
        {
            db.Configuration.LazyLoadingEnabled = false;
            
            // Kullanıcı rolüne göre veri filtreleme
            var userRole = Session["RolAdi"]?.ToString();
            var currentUserDepartmanId = Convert.ToInt32(Session["DepartmanID"]);
            
            var query = db.Kullanici
                .Include(k => k.Rol)
                .Include(k => k.Departman)
                .AsQueryable();
            
            // Yönetici sadece kendi departmanındaki çalışanları görebilir
            if (userRole == "Yönetici")
            {
                query = query.Where(k => k.DepartmanID == currentUserDepartmanId && 
                                       (k.Rol.RolAdi == "Çalışan" || k.Rol.RolAdi == "Calisan"));
            }
            // Admin tüm kullanıcıları görebilir (filtreleme yok)
            
            var kullanicilar = query.ToList();
            return View(kullanicilar);
        }


        public ActionResult YonetimPaneli()
        {
           
            db.Configuration.LazyLoadingEnabled = false;

            var kullanicilar = db.Kullanici
                .Include(k => k.Rol)
                .Include(k => k.Departman)
                .ToList();

            return View(kullanicilar);
        }

        // GET: Kullanici/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kullanici kullanici = db.Kullanici.Find(id);
            if (kullanici == null)
            {
                return HttpNotFound();
            }
            return View(kullanici);
        }




        // POST: Kullanicis/Create
        [HttpPost]
        public ActionResult Create(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                db.Kullanici.Add(kullanici);
                db.SaveChanges();
                return Json(new { success = true });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(", ", errors) });
        }



        // AJAX için kullanıcı bilgilerini getir
        public JsonResult GetKullanici(int id)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;

                var kullanici = db.Kullanici
                    .Include(k => k.Rol)
                    .Include(k => k.Departman)
                    .FirstOrDefault(k => k.KullaniciID == id);

                if (kullanici == null)
                    return Json(null, JsonRequestBehavior.AllowGet);

                var veri = new
                {
                    kullanici.KullaniciID,
                    kullanici.Ad,
                    kullanici.Soyad,
                    Email = kullanici.Eposta,
                    kullanici.Sifre,
                    kullanici.RolID,
                    kullanici.DepartmanID
                };

                return Json(veri, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }






        [HttpPost]
        public ActionResult Edit(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kullanici).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(", ", errors) });
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var kullanici = db.Kullanici.Find(id);
            if (kullanici == null)
                return Json(new { success = false, message = "Kullanıcı bulunamadı" });

            db.Kullanici.Remove(kullanici);
            db.SaveChanges();

            return Json(new { success = true });
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