using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shoppingcart.Models;

namespace shoppingcart.Controllers
{
    public class AccountController : Controller
    {
        private shoppingcartEntities db = new shoppingcartEntities();
        public ActionResult Login()
        {
            if (Session["user_id"] != null)
                return Redirect("/Product/Index");
            return View();
        }

        public ActionResult Register()
        {
            if (Session["user_id"] != null)
                return Redirect("/Product/Index");
            return View();
        }

        public ActionResult UserProfile()
        {
            if (Session["user_id"] == null)
                return Redirect("/Product/Index");

            int id = (int) Session["user_id"];

            user user = db.users.Where(x => x.id == id).FirstOrDefault();
            if (user == null)
                return Redirect(Request.UrlReferrer.ToString());

            UserInfo userInfo = db.UserInfoes.Where(x => x.user_id == id).FirstOrDefault();

            UserInfoModel model = new UserInfoModel 
            {
                user = user,
                userInfo = userInfo
            };

            return View(model);
        }

        public ActionResult Logout()
        {
            Session["user_id"] = null;
            Session["isAdmin"] = null;
            Session["username"] = null;

            return Redirect("/Product/Index");
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            user user = db.users.Where(x => x.email == email && x.password == password).FirstOrDefault();
            if(user != null)
            {
                Session["user_id"] = user.id;
                Session["isAdmin"] = user.isAdmin;
                Session["username"] = user.name;
                TempData["message"] = "Successfully Logged in.";
                return Redirect("/Product/Index");
            }
            else
            {
                TempData["message"] = "Email or Password is incorrect.";
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult Register(user user, UserInfo userInfo)
        {
            db.users.Add(user);
            db.SaveChanges();

            userInfo.user_id = user.id;

            db.UserInfoes.Add(userInfo);
            db.SaveChanges();

            Session["user_id"] = user.id;

            return Redirect("/Product/Index");
        }

        [HttpPost]
        public ActionResult Update(user user, UserInfo userInfo)
        {
            int id = (int) Session["user_id"];
            user userData = db.users.Where(x=>x.id == id).FirstOrDefault();

            userData.name = user.name;
            Session["username"] = user.name;
            userData.password = user.password;

            db.SaveChanges();

            UserInfo userInfoData = db.UserInfoes.Where(x=>x.user_id == id).FirstOrDefault();

            userInfoData.address = userInfo.address;
            userInfoData.phone = userInfo.phone;
            userInfoData.country = userInfo.country;
            userInfoData.city = userInfo.city;
            userInfoData.state = userInfo.state;
            userInfoData.postal_code = userInfo.postal_code;

            db.SaveChanges();

            UserInfoModel model = new UserInfoModel 
            {
                user = userData,
                userInfo = userInfoData
            };
            TempData["message"] = "Profile has been updated.";
            return Redirect("/Account/UserProfile"); ;
        }
	}
}