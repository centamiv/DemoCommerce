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
        public ActionResult Categories()
        {
            var model = new Dictionary<Marchegestite, int>();
            var marche = _db.Marchegestite.Where(i => i.idaziendamaster == Const.IdAziendaMaster).OrderBy(i => i.descrizionemarca).ToList();

            foreach (var item in marche)
            {
                model.Add(item, 0);
            }

            var listini = _db.listinimarche.Take(10)
                .Where(i => i.datainiziovalidità == null || i.datainiziovalidità <= DateTime.Today)
                .Where(i => i.datafinevalidità >= DateTime.Today)
                .Where(i => i.inuso == "Si")
                .Where(i => i.idaziendamaster == Const.IdAziendaMaster);

            foreach (var listino in listini)
            {

                var resultList = _db.Database.SqlQuery<Product>(GetProductQuery(listino.idlistino)).ToList();

                foreach (var item in marche)
                {
                    model[item] += resultList.Count(i => i.idmarca == item.idmarca);
                }

            }

            return View(model);
        }

        [HttpGet]
        public ActionResult List(int page = 1, string search = "", int marca = 0)
        {
            var pageLen = 12;
            var model = new ListProductViewModel();
            var from = (page - 1) * pageLen;

            var listini = _db.listinimarche.Take(10)
                .Where(i => i.datainiziovalidità == null || i.datainiziovalidità <= DateTime.Today)
                .Where(i => i.datafinevalidità >= DateTime.Today)
                .Where(i => i.inuso == "Si")
                .Where(i => i.idaziendamaster == Const.IdAziendaMaster);

            var results = new List<Product>();

            foreach (var listino in listini)
            {

                var resultList = _db.Database.SqlQuery<Product>(GetProductQuery(listino.idlistino)).ToList();
                if (marca != 0)
                {
                    resultList = resultList.Where(i => i.idmarca == marca).ToList();
                }
                if (!string.IsNullOrEmpty(search))
                {
                    resultList = resultList.Where(i => i.descrizionebrevearticolo.ToUpper().Contains(search.ToUpper())).ToList();
                }
                results.AddRange(resultList);

            }

            model.Products = results.Skip(from).Take(12).ToList();
            model.ProductTotal = results.Count();
            model.ProductFrom = from + 1;
            model.ProductTo = from + pageLen;
            model.PageTotal = (int)Math.Ceiling((double)model.ProductTotal / pageLen);
            model.CurrentPage = page;
            model.CategoryDesc = _db.Marchegestite.Where(i => i.idmarca == marca).Select(i => i.descrizionemarca).FirstOrDefault();
            model.Category = marca;
            model.Search = search;

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