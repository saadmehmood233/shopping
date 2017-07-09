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

        [HttpPost]
        public ActionResult Checkout(ICollection<int> product_id, ICollection<int> quantity)
        {
            List<CheckoutCartItem> cart = Session["checkout_cart"] as List<CheckoutCartItem>;

            if (cart == null)
            {
                cart = new List<CheckoutCartItem>();
                Session["checkout_cart"] = cart;
            }

            for (int i = 0; i < product_id.Count; i++)
            {
                CheckoutCartItem item = new CheckoutCartItem 
                {
                    product_id = product_id.ElementAt(i),
                    quantity = quantity.ElementAt(i)
                };
                cart.Add(item);
            }

            Session["checkout_cart"] = cart;

            return Redirect("/Product/Checkout");
        }

        public ActionResult Checkout()
        {
            if (Session["user_id"] == null)
            {
                TempData["message"] = "Login is Required to checkout";
                return Redirect("/Account/Login");
            }

            int user_id = (int)Session["user_id"];
            UserInfo model = db.UserInfoes.Where(x => x.user_id == user_id).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult PlaceOrder(ShipmentInfo info)
        {
            List<CheckoutCartItem> cart = Session["checkout_cart"] as List<CheckoutCartItem>;
            if (cart == null)
                return Redirect(Request.UrlReferrer.ToString());

            db.ShipmentInfoes.Add(info);
            db.SaveChanges();

            Order order = new Order();
            order.user_id  = (int)Session["user_id"];
            order.shipment_id = info.id;
            order.created_on = DateTime.Now;
            order.status = "Pending";

            db.Orders.Add(order);
            db.SaveChanges();

            foreach (var cartItem in cart)
            {
                Order_detail detail = new Order_detail { 
                    product_id = cartItem.product_id,
                    quantity = cartItem.quantity,
                    order_id = order.id
                };

                db.Order_detail.Add(detail);
            }
            db.SaveChanges();
            Session["checkout_cart"] = null;
            Session["cart"] = null;
            TempData["message"] = "Your order has been saved. Your order id is : " + order.id;
            return Redirect("/Product/Index");
        }


        public ActionResult Orders()
        {
            int id = (int) Session["user_id"];
            List<Order> orders = db.Orders.Where(x=>x.user_id == id).ToList();
            return View(orders);
        }

	}

}