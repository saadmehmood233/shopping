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

        public ActionResult AddtoCart(int id)
        {
            List<Product> cart = Session["cart"] as List<Product>;
            if (cart == null)
            {
                cart = new List<Product>();
                Session["cart_size"] = cart.Count;
                Session["cart"] = cart;
            }
            Product cartItem = db.Products.Where(x=>x.id == id).FirstOrDefault();

            int index = cart.FindIndex(x => x.id == cartItem.id);
            if (cartItem != null && index < 0)
            {
                cart.Add(cartItem);
                Session["cart"] = cart;
                Session["cart_size"] = cart.Count;
                TempData["message"] = "Item has been added to the cart.";
            }
            else
            {
                TempData["message"] = "Item already exists in the cart.";
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult RemovefromCart(int id)
        {
            List<Product> cart = Session["cart"] as List<Product>;
            if (cart == null)
            {
                cart = new List<Product>();
                Session["cart_size"] = cart.Count;
                Session["cart"] = cart;
            }

            cart.RemoveAll(x => x.id == id);
            Session["cart"] = cart;
            Session["cart_size"] = cart.Count;

            TempData["message"] = "Item has been removed from cart.";
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult ShowCart()
        {
            List<Product> cart = Session["cart"] as List<Product>;
            return View(cart);
        }

	}
}