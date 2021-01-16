using MyCommerceDemo.Database;
using MyCommerceDemo.Models;
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
        [ActionName("Login")]
        public ActionResult LoginGet()
        {
            var model = new LoginUserViewModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult LoginPost()
        {
            var username = Request["username"];
            var password = Request["password"];

            var utente = _db.tuteweb.Where(i => i.idaziendamaster == Const.IdAziendaMaster).Where(i => i.mail == username || i.utente == username).Where(i => i.pswd == password).FirstOrDefault();
            if (utente == null)
            {
                var model = new LoginUserViewModel();
                model.Message = "Nome utente o password non validi";
                return View(model);
            }

            Session["User"] = utente;
            return RedirectToAction("List", "Product");
        }

        [HttpGet]
        [ActionName("Register")]
        public ActionResult RegisterGet()
        {
            var model = new RegisterUserViewModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("Register")]
        public ActionResult RegisterPost()
        {
            var nome = Request["nome"];
            var cognome = Request["cognome"];
            var email = Request["email"];
            var password = Request["password"];
            var passwordconfirm = Request["passwordconfirm"];


            var model = new RegisterUserViewModel();

            if (password != passwordconfirm)
            {
                model.Message = "Password non corrispondente";
                return View(model);
            }

            var utente = _db.tuteweb.Where(i => i.idaziendamaster == Const.IdAziendaMaster)
                .Where(i => i.mail == email || i.utente == email)
                .FirstOrDefault();
            if (utente != null)
            {
                model.Message = "Utente già registrato";
                return View(model);
            }

            utente = new tuteweb();
            utente.utente = email;
            utente.mail = email;
            utente.nome = nome;
            utente.cognome = cognome;
            utente.idaziendamaster = Const.IdAziendaMaster;
            utente.pswd = password;
            utente.idcliente = 0;
            utente.Shopoweb = "Si";
            _db.tuteweb.Add(utente);
            _db.SaveChanges();

            Session["User"] = utente;
            return RedirectToAction("List", "Product");
        }
    }
}