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
            return View();
        }

        public ActionResult Edit(string TextId, string code)
        {
            AdminController adminController = new AdminController();
            return adminController.Edit(TextId,code,"User");
        }
    }
}
