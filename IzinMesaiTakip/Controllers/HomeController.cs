using System.Linq;
using System.Web.Mvc;
using IzinMesaiTakip.Models;
using IzinMesaiTakip.Filters;

namespace IzinMesaiTakip.Controllers
{
    // [AuthorizationFilter("Yönetici", "Çalışan")] // Anasayfa için yetki kontrolü yok
    public class HomeController : Controller
    {
        private IzinMesaiTakipEntities db = new IzinMesaiTakipEntities();

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("Landing");
            }

            ViewBag.KullaniciSayisi = db.Kullanici.Count();
            ViewBag.IzinSayisi = db.Izin.Count();
            ViewBag.MesaiSaati = db.FazlaMesai.Sum(x => (double?)x.Saat) ?? 0;

            return View();
        }
    }
}
