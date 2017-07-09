using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using shoppingcart.Models;
using System.IO;

namespace shoppingcart.Controllers
{
    public class ProductsController : Controller
    {
        private shoppingcartEntities db = new shoppingcartEntities();

        // GET: /Products/
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // GET: /Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: /Products/Create
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {
            if (file != null)
            {
                var fileName = Guid.NewGuid().ToString() + 
                System.IO.Path.GetExtension(file.FileName);

                var uploadUrl = Server.MapPath("~/images");
                string path = Path.Combine(uploadUrl, fileName);
                
                file.SaveAs(path);

                product.img_path = path;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: /Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        
        [HttpPost]
        public ActionResult Edit(Product product, HttpPostedFileBase file)
        {
            if (file != null)
            {
                var fileName = Guid.NewGuid().ToString() +
                   System.IO.Path.GetExtension(file.FileName);

                var uploadUrl = Server.MapPath("~/images");
                string path = Path.Combine(uploadUrl, fileName);

                file.SaveAs(path);

                product.img_path = path;
            } 
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: /Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
