using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;


namespace IzinMesaiTakip.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizationFilter : AuthorizeAttribute
    {
        private string[] _allowedRoles;

        public AuthorizationFilter(params string[] roles)
        {
            _allowedRoles = roles;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            System.Diagnostics.Debug.WriteLine($"=== AuthorizationFilter Çalışıyor ===");
            System.Diagnostics.Debug.WriteLine($"Controller: {httpContext.Request.RequestContext.RouteData.Values["controller"]}");
            System.Diagnostics.Debug.WriteLine($"Action: {httpContext.Request.RequestContext.RouteData.Values["action"]}");
            
            // Kullanıcı giriş yapmış mı kontrol et
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                System.Diagnostics.Debug.WriteLine("Kullanıcı giriş yapmamış - Authorization FAILED");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"Kullanıcı giriş yapmış: {httpContext.User.Identity.Name}");
            
            // Session'dan rol bilgisini al
            var userRole = httpContext.Session["RolAdi"]?.ToString();
            System.Diagnostics.Debug.WriteLine($"Session'dan rol: {userRole}");

            // Session'da rol yoksa, veritabanından kontrol et
            if (string.IsNullOrEmpty(userRole))
            {
                var kullaniciId = httpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(kullaniciId) && int.TryParse(kullaniciId, out int id))
                {
                    using (var db = new IzinMesaiTakip.Models.IzinMesaiTakipEntities())
                    {
                        var kullanici = db.Kullanici.Include(k => k.Rol).FirstOrDefault(k => k.KullaniciID == id);
                        if (kullanici != null)
                        {
                            userRole = kullanici.Rol?.RolAdi;
                            // Session'a rol bilgisini kaydet
                            httpContext.Session["RolAdi"] = userRole;
                            httpContext.Session["KullaniciAdi"] = kullanici.Ad + " " + kullanici.Soyad;
                            System.Diagnostics.Debug.WriteLine($"Veritabanından rol alındı: {userRole}");
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(userRole))
            {
                System.Diagnostics.Debug.WriteLine("Rol bulunamadı - Authorization FAILED");
                return false;
            }

            // Rol kontrolü
            if (_allowedRoles != null && _allowedRoles.Length > 0)
            {
                System.Diagnostics.Debug.WriteLine($"İzin verilen roller: {string.Join(", ", _allowedRoles)}");
                foreach (var role in _allowedRoles)
                {
                    if (role.Equals(userRole, StringComparison.OrdinalIgnoreCase))
                    {
                        System.Diagnostics.Debug.WriteLine($"Rol eşleşti: {userRole} = {role} - Authorization SUCCESS");
                        return true;
                    }
                    // Admin ve Yönetici eşdeğer kabul et
                    if ((role.Equals("Yönetici", StringComparison.OrdinalIgnoreCase) && userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ||
                        (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) && userRole.Equals("Yönetici", StringComparison.OrdinalIgnoreCase)))
                    {
                        System.Diagnostics.Debug.WriteLine($"Admin/Yönetici eşleşti: {userRole} = {role} - Authorization SUCCESS");
                        return true;
                    }
                    // Çalışan ve Calisan eşdeğer kabul et
                    if ((role.Equals("Çalışan", StringComparison.OrdinalIgnoreCase) && userRole.Equals("Calisan", StringComparison.OrdinalIgnoreCase)) ||
                        (role.Equals("Calisan", StringComparison.OrdinalIgnoreCase) && userRole.Equals("Çalışan", StringComparison.OrdinalIgnoreCase)))
                    {
                        System.Diagnostics.Debug.WriteLine($"Çalışan/Calisan eşleşti: {userRole} = {role} - Authorization SUCCESS");
                        return true;
                    }
                }
                System.Diagnostics.Debug.WriteLine($"Hiçbir rol eşleşmedi - Authorization FAILED");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("Rol kontrolü yok - Authorization SUCCESS");
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Eğer kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Account", action = "Login" }
                    )
                );
            }
            else
            {
                // Giriş yapmış ama yetkisiz kullanıcı için hata sayfası
                filterContext.Result = new HttpStatusCodeResult(403, "Bu sayfaya erişim yetkiniz yok.");
            }
        }
    }
} 