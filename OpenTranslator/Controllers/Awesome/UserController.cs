using OpenTranslator.Data;
using OpenTranslator.Models.Input;
using OpenTranslator.Repostitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenTranslator.Controllers.Awesome
{
    public class UserController : Controller
    {
		
        // GET: User
        public ActionResult Index()
        {
			if (Request.Cookies["UserId"] != null)
			{
				Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(-1);
				Response.Buffer= true;
				Response.ExpiresAbsolute=DateTime.Now.AddDays(-1d);
				Response.Expires =-1500;
				Response.CacheControl = "no-cache";
			}
            return View();
        }
		

		[HttpPost]
        public ActionResult getSession()
        {
           var selectedColumns = (string[])Session["SelectedColumns"];
           return Json(new {selectedColumns });
        }
        public ActionResult Edit(string TextId, string code)
        {
            AdminController adminController = new AdminController();
            return adminController.Edit(TextId,code,"User");
        }

		[HttpPost]
        public ActionResult Edit(TranslationInput input)
        {
              AdminController adminController = new AdminController();
           return adminController.Edit(input);
        }
	
			}
		}
