using shoppingcart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shoppingcart.Controllers
{
    public class AdminController : Controller
    {
        shoppingcartEntities db = new shoppingcartEntities();
        public ActionResult Index()
        {
            if(Session["isAdmin"] == null )
                return Redirect("/Product/Index");

            bool isAdmin = (bool) Session["isAdmin"];
            if (!isAdmin)
                return Redirect(Request.UrlReferrer.ToString());

            List<Product> model = db.Products.ToList();
            return View(model);
        }
	}
}