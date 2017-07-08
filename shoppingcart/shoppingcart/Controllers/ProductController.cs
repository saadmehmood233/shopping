using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shoppingcart.Models;
using PagedList;
using PagedList.Mvc;

namespace shoppingcart.Controllers
{
    public class ProductController : Controller
    {
        private shoppingcartEntities db = new shoppingcartEntities();
        private static List<Product> cart = new List<Product>();

        public ActionResult Index(string category = "All", int page = 1, int pageSize = 8)
        {
            Category cat = db.Categories.Where(x=>x.name == category).FirstOrDefault();
            List<Category> categories = db.Categories.ToList();

            List<Product> products = null;
            if (cat == null)
                products = db.Products.ToList();
            else
                products = db.Products.Where(x=>x.category_id == cat.id).ToList();

            PagedList<Product> pagedProduct = new PagedList<Product>(products, page, pageSize);

            CategoriesProducts model = new CategoriesProducts
            {
                activeCategory = category,
                categories = categories,
                products = pagedProduct
            };

            return View(model);
        }

        public ActionResult Detail(int id)
        {
            Product product = db.Products.Where(x=>x.id == id).FirstOrDefault();
            if (product == null)
                return RedirectToAction("Index");
            return View(product);
        }

	}
}