using MyCommerceDemo.Database;
using MyCommerceDemo.Models;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCommerceDemo.Controllers
{
    public class CartController : Controller
    {
        private onelightnetEntities _db;

        public CartController()
        {
            _db = new onelightnetEntities();
        }

        private string GetProductQuery(long idlistino)
        {
            long idCliente = 0;
            if (Session["User"] != null)
            {
                idCliente = (long)(Session["User"] as MyCommerceDemo.Database.tuteweb).idcliente;
            }
            return $"select l.unitàmisura, l.statoarticolo, l.codicearticolo, l.idarticolo, l.idmarca, l.idaziendamaster, l.prezzolistinoaut, l.prezzolistinoman, l.ivavendita, l.descrizionebrevearticolo, l.descrizioneestesaarticolo, {idlistino} as idlistino, s.sconto1, s.sconto2, s.ricarico1, s.ricarico2 from listino{idlistino} l left join scontimarche s on s.idlistino = l.idlistino and s.idcliente = {idCliente} and s.idmarca = l.idmarca where statoarticolo = 'In uso'";
        }

        [HttpPost]
        public ActionResult Add()
        {
            var id = Request["code"];
            var qty = int.Parse(Request["qty"]);

            var idlistino = int.Parse(id.Split('-')[0]);
            var idmarca = int.Parse(id.Split('-')[1]);
            var idarticolo = int.Parse(id.Split('-')[2]);

            var results = _db.Database.SqlQuery<Product>(GetProductQuery(idlistino)).Where(i => i.idmarca == idmarca).Where(i => i.idarticolo == idarticolo);

            var product = results.First();

            var cart = Session["Cart"] as Dictionary<Product, int>;
            if (cart == null)
            {
                cart = new Dictionary<Product, int>();
            }

            var existingProduct = cart.Keys.Where(i => i.Id == product.Id).FirstOrDefault();
            if (existingProduct != null)
            {
                cart[existingProduct] += qty;
            }
            else
            {
                cart.Add(product, qty);
            }

            Session["Cart"] = cart;

            return RedirectToAction("Show");
        }

        [HttpGet]
        public ActionResult Show()
        {
            var cart = Session["Cart"] as Dictionary<Product, int>;
            if (cart == null)
            {
                cart = new Dictionary<Product, int>();
            }

            return View(cart);
        }

        [HttpGet]
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as Dictionary<Product, int>;
            if (cart == null)
            {
                cart = new Dictionary<Product, int>();
            }

            var model = new CheckoutCartViewModel();
            model.Items = cart;

            long idCliente = 0;
            if (Session["User"] != null)
            {
                idCliente = (long)(Session["User"] as MyCommerceDemo.Database.tuteweb).idcliente;
            }

            var cliente = new CLIENTI();
            if (idCliente != 0)
            {
                cliente = _db.CLIENTI.Where(i => i.idcliente == idCliente).FirstOrDefault();
            }

            model.Cliente = cliente;

            return View(model);
        }

        [HttpPost]
        public ActionResult Charge()
        {
            var utente = Session["User"] as MyCommerceDemo.Database.tuteweb;
            var cart = Session["Cart"] as Dictionary<Product, int>;


            var myCharge = new Stripe.ChargeCreateOptions
            {
                Amount = (long)cart.Sum(item => item.Key.DiscountPrice * item.Value) * 100,
                Currency = "EUR",
                ReceiptEmail = Request.Form["stripeEmail"],
                Description = Const.Title,
                Source = Request.Form["stripeToken"],
                Capture = true
            };
            var chargeService = new Stripe.ChargeService();
            Stripe.Charge stripeCharge = chargeService.Create(myCharge);

            if (stripeCharge.Status == "succeeded")
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
                var dataconsegna = Request["dataconsegna"];

                model.denominazione = ragsoc;
                model.contauno = nome;
                model.PIVA = piva;
                model.indirizzolegale = indirizzo;
                model.comunelegale = comune;
                model.caplegale = cap;
                model.cittàlegale = citta;
                model.siglalegale = sigla;
                model.telefono1legale = telefono;
                model.mailcontauno = email;
                model.clientecontatto = "Contatto";
                //model.escludidaelencoclienti
                if (idCliente == 0)
                {
                    _db.CLIENTI.Add(model);
                }
                _db.SaveChanges();

                var user = (Session["User"] as MyCommerceDemo.Database.tuteweb);
                user.idcliente = model.idcliente;
                Session["User"] = user;



                // Carrello in ordine

                var ordine = new MyCommerceDemo.Database.datiordineclienteweb()
                {
                    idcliente = idCliente,
                    cliente = ragsoc,
                    totaleivaesclusa = cart.Sum(item => item.Key.DiscountPrice * item.Value),
                    dataconsegnaprevista = DateTime.Parse(dataconsegna),
                    idaziendamaster = Const.IdAziendaMaster,
                    statoordine = "Attesa convalida",
                    dataordine = DateTime.Now,
                    metodoconsegna = "Ritira il cliente",
                    descrizioneordine = ""
                };
                _db.datiordineclienteweb.Add(ordine);
                _db.SaveChanges();

                foreach (var item in cart)
                {
                    var riga = new MyCommerceDemo.Database.articoliordineclienteweb()
                    {
                        unitàmisura = item.Key.unitàmisura,
                        codicearticolo = item.Key.codicearticolo,
                        descrizionebrevearticolo = item.Key.descrizionebrevearticolo,
                        codiceabarrearticolo = item.Key.codicearticolo,
                        quantità = item.Value,
                        idaziendamaster = Const.IdAziendaMaster,
                        idlistino = item.Key.idlistino,
                        idordine = ordine.idordine,
                        dataordine = ordine.dataordine,
                        idcliente = ordine.idcliente,
                        prezzounitario = item.Key.DiscountPrice,
                        totaleivaesclusa = item.Key.DiscountPrice * item.Value,
                        coefk = 0
                    };
                    _db.articoliordineclienteweb.Add(riga);
                }
                _db.SaveChanges();

                return RedirectToAction("Orders", "User");
            }

            return RedirectToAction("Orders", "User");
        }

        [HttpGet]
        public ActionResult Del(string id)
        {
            var cart = Session["Cart"] as Dictionary<Product, int>;
            if (cart != null)
            {
                var existingProduct = cart.Keys.Where(i => i.Id == id).FirstOrDefault();
                if (existingProduct != null)
                {
                    cart.Remove(existingProduct);
                }

                Session["Cart"] = cart;
            }

            return RedirectToAction("Show");
        }

    }
}