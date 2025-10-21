using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IzinMesaiTakip.Models;
using IzinMesaiTakip.Filters;

namespace IzinMesaiTakip.Controllers
{
    [AuthorizationFilter("Yönetici", "Çalışan", "Calisan")]
    public class IzinController : Controller
    {
        private readonly IzinMesaiTakipEntities db = new IzinMesaiTakipEntities();

        // GET: Izin
        public ActionResult Index()
        {
            db.Configuration.LazyLoadingEnabled = false;
            
            // Kullanıcı rolüne göre veri filtreleme
            var userRole = Session["RolAdi"]?.ToString();
            var currentUserId = Convert.ToInt32(Session["KullaniciID"]);
            var currentUserDepartmanId = Convert.ToInt32(Session["DepartmanID"]);
            
            var query = db.Izin
                .Include(i => i.IzinTur)
                .Include(i => i.Kullanici)
                .Include(i => i.Kullanici.Departman)
                .AsQueryable();
            
            // Çalışan sadece kendi izinlerini görebilir
            if (userRole == "Çalışan" || userRole == "Calisan")
            {
                query = query.Where(i => i.KullaniciID == currentUserId);
            }
            // Yönetici sadece kendi departmanındaki çalışanların izinlerini görebilir
            else if (userRole == "Yönetici")
            {
                query = query.Where(i => i.Kullanici.DepartmanID == currentUserDepartmanId && 
                                       (i.Kullanici.Rol.RolAdi == "Çalışan" || i.Kullanici.Rol.RolAdi == "Calisan"));
            }
            // Admin tüm izinleri görebilir (filtreleme yok)
            
            var izin = query.ToList();
            return View(izin);
        }

        // GET: Izin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Izin izin = db.Izin.Find(id);
            if (izin == null)
            {
                return HttpNotFound();
            }
            return View(izin);
        }

        // GET: Izin/Create
        public ActionResult Create()
        {
            ViewBag.KullaniciListesi = new SelectList(db.Kullanici, "KullaniciID", "Ad");
            ViewBag.IzinTurListesi = new SelectList(db.IzinTur, "IzinTurID", "TurAdi");
            return View();
        }

        // POST: Izin/Create
        [HttpPost]
        public ActionResult Create(Izin izin)
        {
            if (ModelState.IsValid)
            {
                izin.OlusturmaTarihi = DateTime.Now;
                db.Izin.Add(izin);
                db.SaveChanges();
                return Json(new { success = true });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(", ", errors) });
        }

        // GET: Izin/Edit/5 - AJAX için JSON döndürür
        public JsonResult Edit(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var izin = db.Izin
                .Include(i => i.IzinTur)
                .Include(i => i.Kullanici)
                .FirstOrDefault(i => i.IzinID == id);

            if (izin == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var veri = new
            {
                izin.IzinID,
                izin.KullaniciID,
                izin.IzinTurID,
                BaslangicTarih = izin.BaslangicTarih?.ToString("yyyy-MM-dd"),
                BitisTarih = izin.BitisTarih?.ToString("yyyy-MM-dd"),
                izin.Aciklama,
                izin.Durum
            };

            return Json(veri, JsonRequestBehavior.AllowGet);
        }

        // POST: Izin/Edit/5
        [HttpPost]
        public ActionResult Edit(Izin izin)
        {
            // Rol kontrolü - sadece yönetici ve admin onay/red yapabilir
            var userRole = Session["RolAdi"]?.ToString();
            if (userRole == "Çalışan" || userRole == "Calisan")
            {
                return Json(new { success = false, message = "Çalışanlar izin onaylama yetkisine sahip değil" });
            }

            // Yönetici sadece kendi departmanındaki çalışanların izinlerini onaylayabilir
            if (userRole == "Yönetici")
            {
                var currentUserDepartmanId = Convert.ToInt32(Session["DepartmanID"]);
                var izinKullanici = db.Kullanici.Include(k => k.Departman).FirstOrDefault(k => k.KullaniciID == izin.KullaniciID);
                
                if (izinKullanici == null || izinKullanici.DepartmanID != currentUserDepartmanId)
                {
                    return Json(new { success = false, message = "Sadece kendi departmanınızdaki çalışanların izinlerini onaylayabilirsiniz" });
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(izin).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(", ", errors) });
        }

        // GET: Izin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Izin izin = db.Izin.Find(id);
            if (izin == null)
            {
                return HttpNotFound();
            }
            return View(izin);
        }

        // POST: Izin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var izin = db.Izin.Find(id);
            if (izin == null)
                return Json(new { success = false, message = "İzin kaydı bulunamadı" });

            db.Izin.Remove(izin);
            db.SaveChanges();

            return Json(new { success = true });
        }

        // AJAX için kullanıcı listesi
        public JsonResult KullaniciListele()
        {
            var kullanicilar = db.Kullanici.Select(k => new
            {
                k.KullaniciID,
                AdSoyad = k.Ad + " " + k.Soyad
            }).ToList();

            return Json(kullanicilar, JsonRequestBehavior.AllowGet);
        }

        // AJAX için izin türü listesi
        public JsonResult IzinTurListele()
        {
            var izinTurleri = db.IzinTur.Select(i => new
            {
                i.IzinTurID,
                i.TurAdi
            }).ToList();

            return Json(izinTurleri, JsonRequestBehavior.AllowGet);
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
