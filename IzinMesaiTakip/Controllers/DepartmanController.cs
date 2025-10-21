using IzinMesaiTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IzinMesaiTakip.Controllers
{
    public class DepartmanController : Controller
    {
        private IzinMesaiTakipEntities db = new IzinMesaiTakipEntities();

        [HttpGet]
        public JsonResult Listele()
        {
            var departmanlar = db.Departman.Select(d => new {
                d.DepartmanID,
                d.DepartmanAdi
            }).ToList();

            return Json(departmanlar, JsonRequestBehavior.AllowGet);
        }

    }
}