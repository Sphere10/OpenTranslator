using System;
using System.Web.Mvc;

using OpenTranslator.Models.Input;

namespace OpenTranslator.Controllers.Awesome
{
	public class UserController : BaseController
	{

        #region Public Methods
        
        // GET: User
        public ActionResult Index()
		{
			if (Request.Cookies["UserId"] != null)
			{
				Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(-1);
				Response.Buffer = true;
				Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);
				Response.Expires = -1500;
				Response.CacheControl = "no-cache";
			}
			return View("Index","_Layout");
		}

		public ActionResult embeded()
		{
			return View("Index","_LayoutEmbedUser");
		}

		[HttpPost]
		public ActionResult getSession()
		{
			var selectedColumns = (string[])Session["SelectedColumns"];
			return Json(new { selectedColumns });
		}

		public ActionResult Edit(string TextId, string code)
		{
			AdminController adminController = new AdminController();
			return adminController.Edit(TextId, code, "User");
		}

		[HttpPost]
		public ActionResult Edit(TranslationInput input)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("EditTranslation", input);
			}
			AdminController adminController = new AdminController();

			return adminController.Edit(input);
		}

		public ActionResult Aboutus()
		{
			return View();
		}

        #endregion
    }
}
