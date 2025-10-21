using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using IzinMesaiTakip.Models;
using IzinMesaiTakip.Filters;

namespace IzinMesaiTakip.Controllers
{
    [AuthorizationFilter("Yönetici", "Çalışan", "Calisan")]
    public class FazlaMesaiController : Controller
    {
        private IzinMesaiTakipEntities db = new IzinMesaiTakipEntities();

        // Listeleme
        public ActionResult Index()
        {
            db.Configuration.LazyLoadingEnabled = false;
            
            // Kullanıcı rolüne göre veri filtreleme
            var userRole = Session["RolAdi"]?.ToString();
            var currentUserId = Convert.ToInt32(Session["KullaniciID"]);
            var currentUserDepartmanId = Convert.ToInt32(Session["DepartmanID"]);
            
            var query = db.FazlaMesai
                .Include(f => f.Kullanici)
                .Include(f => f.Kullanici.Departman)
                .AsQueryable();
            
            // Çalışan sadece kendi fazla mesailerini görebilir
            if (userRole == "Çalışan" || userRole == "Calisan")
            {
                query = query.Where(f => f.KullaniciID == currentUserId);
            }
            // Yönetici sadece kendi departmanındaki çalışanların fazla mesailerini görebilir
            else if (userRole == "Yönetici")
            {
                query = query.Where(f => f.Kullanici.DepartmanID == currentUserDepartmanId && 
                                       (f.Kullanici.Rol.RolAdi == "Çalışan" || f.Kullanici.Rol.RolAdi == "Calisan"));
            }
            // Admin tüm fazla mesaileri görebilir (filtreleme yok)
            
            var mesailer = query.ToList();
            return View(mesailer);
        }

        // Detay
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var mesai = db.FazlaMesai.Find(id);
            if (mesai == null) return HttpNotFound();

            return View(mesai);
        }

        // Ekle POST
        [HttpPost]
        public ActionResult Create()
        {
            try
            {
                var mesai = new FazlaMesai();
                
                // Form verilerini manuel olarak al
                mesai.KullaniciID = int.Parse(Request.Form["KullaniciID"]);
                mesai.Tarih = DateTime.Parse(Request.Form["Tarih"]);
                
                // Saat değerini manuel olarak parse et
                var saatValue = Request.Form["Saat"];
                if (!string.IsNullOrEmpty(saatValue))
                {
                    // Virgülü noktaya çevir
                    saatValue = saatValue.Replace(',', '.');
                    decimal saat;
                    if (decimal.TryParse(saatValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out saat))
                    {
                        mesai.Saat = Math.Round(saat, 2);
                    }
                }
                
                mesai.Aciklama = Request.Form["Aciklama"];
                mesai.Durum = Request.Form["Durum"] == "true";
                mesai.OlusturmaTarihi = DateTime.Now;
                
                db.FazlaMesai.Add(mesai);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        // Güncelle GET - AJAX için JSON döndürür
        public JsonResult Edit(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var mesai = db.FazlaMesai
                .Include(f => f.Kullanici)
                .FirstOrDefault(f => f.MesaiID == id);

            if (mesai == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var veri = new
            {
                mesai.MesaiID,
                mesai.KullaniciID,
                Tarih = mesai.Tarih?.ToString("yyyy-MM-dd"),
                Saat = mesai.Saat?.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                mesai.Aciklama,
                mesai.Durum,
                OlusturmaTarihi = mesai.OlusturmaTarihi?.ToString("yyyy-MM-dd")
            };

            return Json(veri, JsonRequestBehavior.AllowGet);
        }

        // Güncelle POST
        [HttpPost]
        public ActionResult Edit()
        {
            try
            {
                // Rol kontrolü - sadece yönetici ve admin onay/red yapabilir
                var userRole = Session["RolAdi"]?.ToString();
                if (userRole == "Çalışan" || userRole == "Calisan")
                {
                    return Json(new { success = false, message = "Çalışanlar fazla mesai onaylama yetkisine sahip değil" });
                }

                var mesaiID = int.Parse(Request.Form["MesaiID"]);
                var mesai = db.FazlaMesai.Find(mesaiID);
                
                if (mesai == null)
                    return Json(new { success = false, message = "Fazla mesai kaydı bulunamadı" });

                // Yönetici sadece kendi departmanındaki çalışanların fazla mesailerini onaylayabilir
                if (userRole == "Yönetici")
                {
                    var currentUserDepartmanId = Convert.ToInt32(Session["DepartmanID"]);
                    var mesaiKullanici = db.Kullanici.Include(k => k.Departman).FirstOrDefault(k => k.KullaniciID == mesai.KullaniciID);
                    
                    if (mesaiKullanici == null || mesaiKullanici.DepartmanID != currentUserDepartmanId)
                    {
                        return Json(new { success = false, message = "Sadece kendi departmanınızdaki çalışanların fazla mesailerini onaylayabilirsiniz" });
                    }
                }
                
                // Form verilerini manuel olarak al
                mesai.KullaniciID = int.Parse(Request.Form["KullaniciID"]);
                mesai.Tarih = DateTime.Parse(Request.Form["Tarih"]);
                
                // Saat değerini manuel olarak parse et
                var saatValue = Request.Form["Saat"];
                if (!string.IsNullOrEmpty(saatValue))
                {
                    // Virgülü noktaya çevir
                    saatValue = saatValue.Replace(',', '.');
                    decimal saat;
                    if (decimal.TryParse(saatValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out saat))
                    {
                        mesai.Saat = Math.Round(saat, 2);
                    }
                }
                
                mesai.Aciklama = Request.Form["Aciklama"];
                mesai.Durum = Request.Form["Durum"] == "true";
                mesai.OlusturmaTarihi = DateTime.Now; // Güncelleme tarihini de güncelle
                
                db.Entry(mesai).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        // Sil POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var mesai = db.FazlaMesai.Find(id);
            if (mesai == null)
                return Json(new { success = false, message = "Fazla mesai kaydı bulunamadı" });

            db.FazlaMesai.Remove(mesai);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
