using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using IzinMesaiTakip.Models;

namespace IzinMesaiTakip.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private IzinMesaiTakipEntities db = new IzinMesaiTakipEntities();

        // GET: Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Eğer zaten giriş yapmışsa ana sayfaya yönlendir
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // GET: Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(string ad, string soyad, string email, string password)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Tüm alanları doldurunuz.");
                return View();
            }

            var mevcut = db.Kullanici.FirstOrDefault(k => k.Eposta == email);
            if (mevcut != null)
            {
                ModelState.AddModelError("", "Bu e-posta ile kayıtlı kullanıcı zaten mevcut.");
                return View();
            }

            // Varsayılan rol ve departman ataması (mevcut yoksa oluştur)
            var calisanRol = db.Rol.FirstOrDefault(r => r.RolAdi == "Çalışan");
            if (calisanRol == null)
            {
                calisanRol = new Rol { RolAdi = "Çalışan" };
                db.Rol.Add(calisanRol);
                db.SaveChanges();
            }

            var departman = db.Departman.FirstOrDefault(d => d.DepartmanAdi == "Genel");
            if (departman == null)
            {
                departman = new Departman { DepartmanAdi = "Genel" };
                db.Departman.Add(departman);
                db.SaveChanges();
            }

            var yeni = new Kullanici
            {
                Ad = ad,
                Soyad = soyad,
                Eposta = email,
                Sifre = password,
                RolID = calisanRol.RolID,
                DepartmanID = departman.DepartmanID
            };

            db.Kullanici.Add(yeni);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Kayıt başarıyla tamamlandı. Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
               
                var kullanici = db.Kullanici
                    .Include(k => k.Rol)
                    .FirstOrDefault(k => k.Eposta == email && k.Sifre == password);

                if (kullanici != null)
                {
                    // Forms Authentication ile giriş yap
                    FormsAuthentication.SetAuthCookie(kullanici.KullaniciID.ToString(), false);
                    
                    // Session'a kullanıcı bilgilerini kaydet
                    Session["KullaniciID"] = kullanici.KullaniciID;
                    Session["KullaniciAdi"] = kullanici.Ad + " " + kullanici.Soyad;
                    Session["RolID"] = kullanici.RolID;
                    Session["RolAdi"] = kullanici.Rol?.RolAdi;
                    Session["DepartmanID"] = kullanici.DepartmanID;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "E-posta veya şifre hatalı!");
                }
            }

            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            // Session'ı temizle
            Session.Clear();
            
            // Forms Authentication'dan çıkış yap
            FormsAuthentication.SignOut();
            
            return RedirectToAction("Login");
        }

        // Test için session temizleme
        [AllowAnonymous]
        public ActionResult ClearSession()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return Content("Session temizlendi. <a href='/'>Ana sayfaya git</a>");
        }


        // Test kullanıcıları oluştur (sadece geliştirme için)
        [AllowAnonymous]
        public ActionResult CreateTestUsers()
        {
            try
            {
                // Rolleri oluştur
                var yoneticiRol = db.Rol.FirstOrDefault(r => r.RolAdi == "Yönetici");
                if (yoneticiRol == null)
                {
                    yoneticiRol = new Rol { RolAdi = "Yönetici" };
                    db.Rol.Add(yoneticiRol);
                    db.SaveChanges();
                }

                var calisanRol = db.Rol.FirstOrDefault(r => r.RolAdi == "Çalışan");
                if (calisanRol == null)
                {
                    calisanRol = new Rol { RolAdi = "Çalışan" };
                    db.Rol.Add(calisanRol);
                    db.SaveChanges();
                }

                // Departman oluştur
                var departman = db.Departman.FirstOrDefault(d => d.DepartmanAdi == "Genel");
                if (departman == null)
                {
                    departman = new Departman { DepartmanAdi = "Genel" };
                    db.Departman.Add(departman);
                    db.SaveChanges();
                }

                // Test kullanıcıları oluştur
                var adminKullanici = db.Kullanici.FirstOrDefault(k => k.Eposta == "admin@test.com");
                if (adminKullanici == null)
                {
                    adminKullanici = new Kullanici
                    {
                        Ad = "Admin",
                        Soyad = "User",
                        Eposta = "admin@test.com",
                        Sifre = "123456",
                        RolID = yoneticiRol.RolID,
                        DepartmanID = departman.DepartmanID
                    };
                    db.Kullanici.Add(adminKullanici);
                }

                var calisanKullanici = db.Kullanici.FirstOrDefault(k => k.Eposta == "calisan@test.com");
                if (calisanKullanici == null)
                {
                    calisanKullanici = new Kullanici
                    {
                        Ad = "Çalışan",
                        Soyad = "User",
                        Eposta = "calisan@test.com",
                        Sifre = "123456",
                        RolID = calisanRol.RolID,
                        DepartmanID = departman.DepartmanID
                    };
                    db.Kullanici.Add(calisanKullanici);
                }

                db.SaveChanges();
                return Content("Test kullanıcıları başarıyla oluşturuldu!");
            }
            catch (Exception ex)
            {
                return Content("Hata: " + ex.Message);
            }
        }

        public static bool IsInRole(string roleName)
        {
            if (System.Web.HttpContext.Current.Session["RolAdi"] != null)
            {
                return System.Web.HttpContext.Current.Session["RolAdi"].ToString() == roleName;
            }
            return false;
        }

 
        public static bool IsAdmin()
        {
            return IsInRole("Yönetici");
        }

  
        public static bool IsEmployee()
        {
            return IsInRole("Çalışan");
        }
    }
} 