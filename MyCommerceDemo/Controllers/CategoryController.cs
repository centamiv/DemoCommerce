using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCommerceDemo.Controllers
{
    public class CategoryController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

    }
}