using MyCommerceDemo.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCommerceDemo.Controllers
{
    public class UserController : Controller
    {

        private onelightnetEntities _db;

        public UserController()
        {
            _db = new onelightnetEntities();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Check()
        {
            var username = Request["username"];
            var password = Request["password"];

            var idaziendamaster = Const.IdAziendaMaster;

            var utente = _db.tuteweb.Where(i => i.idaziendamaster == idaziendamaster).Where(i => i.mail == username || i.utente == username).Where(i => i.pswd == password).FirstOrDefault();
            if (utente == null)
            {
                return RedirectToAction("Login", "User");
            }

            Session["User"] = utente;
            return RedirectToAction("List", "Product");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
    }
}