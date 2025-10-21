using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using IzinMesaiTakip.Models;
using IzinMesaiTakip.Filters;
using System;

namespace IzinMesaiTakip.Controllers
{
    [AuthorizationFilter("Yönetici", "Admin")]
    public class YonetimController : Controller
    {
        private IzinMesaiTakipEntities db = new IzinMesaiTakipEntities();
        // Tüm kullanıcıları listele
        public ActionResult Index()
        {
            // Lazy loading'i devre dışı bırak ve navigation property'leri açıkça yükle
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



        // Kullanıcıyı düzenle (Rol ve Departman)
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var kullanici = db.Kullanici.Find(id);
            if (kullanici == null)
                return HttpNotFound();

            ViewBag.RolID = new SelectList(db.Rol, "RolID", "RolAdi", kullanici.RolID);
            ViewBag.DepartmanID = new SelectList(db.Departman, "DepartmanID", "DepartmanAdi", kullanici.DepartmanID);

            return View(kullanici);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Kullanici model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = db.Kullanici.Find(model.KullaniciID);
                if (kullanici != null)
                {
                    kullanici.RolID = model.RolID;
                    kullanici.DepartmanID = model.DepartmanID;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.RolID = new SelectList(db.Rol, "RolID", "RolAdi", model.RolID);
            ViewBag.DepartmanID = new SelectList(db.Departman, "DepartmanID", "DepartmanAdi", model.DepartmanID);
            return View(model);
        }
    }
}
