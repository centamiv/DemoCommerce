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

        private readonly onelightnetEntities _db;

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
                var model = new LoginUserViewModel
                {
                    Message = "Nome utente o password non validi"
                };
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

        [HttpGet]
        [ActionName("Menu")]
        public ActionResult MenuGet()
        {
            return View();
        }

        [HttpGet]
        [ActionName("Data")]
        public ActionResult DataGet()
        {
            long idCliente = 0;
            if (Session["User"] != null)
            {
                idCliente = (long)(Session["User"] as MyCommerceDemo.Database.tuteweb).idcliente;
            }

            var model = new CLIENTI();
            if (idCliente != 0)
            {
                model = _db.CLIENTI.Where(i => i.idcliente == idCliente).FirstOrDefault();
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("DataConfirm")]
        public ActionResult DataConfirmPost()
        {
            long idCliente = 0;
            if (Session["User"] != null)
            {
                idCliente = (long)(Session["User"] as MyCommerceDemo.Database.tuteweb).idcliente;
            }
            CLIENTI model;
            if (idCliente == 0)
            {
                model = new CLIENTI();
            }
            else
            {
                model = _db.CLIENTI.Where(i => i.idcliente == idCliente).FirstOrDefault();
            }

            var nome = Request["nome"];
            var ragsoc = Request["ragsoc"];
            var piva = Request["piva"];
            var indirizzo = Request["indirizzo"];
            var comune = Request["comune"];
            var cap = Request["cap"];
            var citta = Request["citta"];
            var sigla = Request["sigla"];
            var telefono = Request["telefono"];
            var email = Request["email"];

            model.denominazione = ragsoc;
            model.contauno = nome ;
            model.PIVA = piva;
            model.indirizzolegale = indirizzo;
            model.comunelegale = comune;
            model.caplegale = cap;
            model.cittàlegale = citta;
            model.siglalegale = sigla;
            model.telefono1legale = telefono;
            model.mailcontauno = email;
            //model.escludidaelencoclienti
            if (idCliente == 0)
            {
                _db.CLIENTI.Add(model);
            }
            _db.SaveChanges();

            var user = (Session["User"] as MyCommerceDemo.Database.tuteweb);
            user.idcliente = model.idcliente;
            Session["User"] = user;

            return RedirectToAction("Data");
        }

        [HttpGet]
        [ActionName("Orders")]
        public ActionResult OrdersGet()
        {
            long idCliente = 0;
            if (Session["User"] != null)
            {
                idCliente = (long)(Session["User"] as MyCommerceDemo.Database.tuteweb).idcliente;
            }


            var model = new List<MyCommerceDemo.Database.datiordineclienteweb>();
            if (idCliente != 0)
            {
                var ordini = _db.datiordineclienteweb
                    .Where(i => i.idcliente == idCliente)
                    .Where(i => i.idaziendamaster == Const.IdAziendaMaster)
                    .OrderByDescending(i => i.dataconsegnaprevista)
                    .ToList();
                model.AddRange(ordini);
            }

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
            var ragsoc = Request["ragsoc"];
            var piva = Request["piva"];


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

            long cliente = 0;
            if (!string.IsNullOrEmpty(piva))
            {
                cliente = _db.CLIENTI.Where(i => i.PIVA.Trim() == piva.Trim()).Select(i => i.idcliente).FirstOrDefault();
                if (cliente == 0)
                {
                    var newCliente = new CLIENTI()
                    {
                        denominazione = ragsoc,
                        contattouno = nome + " " + cognome,
                        PIVA = piva,
                        mailcontauno = email
                    };
                    _db.CLIENTI.Add(newCliente);
                    _db.SaveChanges();
                    cliente = newCliente.idcliente;
                }
            }

            utente = new tuteweb
            {
                utente = email,
                mail = email,
                nome = nome,
                cognome = cognome,
                idaziendamaster = Const.IdAziendaMaster,
                pswd = password,
                idcliente = cliente,
                Shopoweb = "Si"
            };
            _db.tuteweb.Add(utente);
            _db.SaveChanges();

            Session["User"] = utente;
            return RedirectToAction("List", "Product");
        }
    }
}