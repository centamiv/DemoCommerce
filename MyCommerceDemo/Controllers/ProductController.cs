using MyCommerceDemo.Database;
using MyCommerceDemo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace MyCommerceDemo.Controllers
{
    public class ProductController : Controller
    {
        private onelightnetEntities _db;

        public ProductController()
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


        [HttpGet]
        public ActionResult List(int page = 1)
        {
            var model = new ListProductViewModel();
            var from = (page - 1) * 12;

            var listini = _db.listinimarche
                .Where(i => i.datainiziovalidità == null || i.datainiziovalidità <= DateTime.Today)
                .Where(i => i.datafinevalidità >= DateTime.Today)
                .Where(i => i.inuso == "Si")
                .Where(i => i.idaziendamaster == Const.IdAziendaMaster);
            
            var results = new List<Product>();

            foreach (var listino in listini)
            {

                var resultList = _db.Database.SqlQuery<Product>(GetProductQuery(listino.idlistino)).ToList();
                results.AddRange(resultList);

            }

            model.Products = results.Skip(from).Take(12).ToList();
            model.ProductTotal = results.Count();
            model.ProductFrom = from;
            model.Page = page;

            return View(model);
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            var idlistino = int.Parse(id.Split('-')[0]);
            var idmarca = int.Parse(id.Split('-')[1]);
            var idarticolo = int.Parse(id.Split('-')[2]);

            var results = _db.Database.SqlQuery<Product>(GetProductQuery(idlistino)).Where(i => i.idmarca == idmarca).Where(i => i.idarticolo == idarticolo);

            var model = results.First();

            return View(model);
        }

    }
}