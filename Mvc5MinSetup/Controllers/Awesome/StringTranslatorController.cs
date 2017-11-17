using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc5MinSetup.Controllers.Awesome
{
    public class StringTranslatorController : Controller
    {
       
        public ActionResult Create()
        {
            return PartialView();
        }

		public ActionResult GridGetItems()
        {
           return Json("");
        }

    }
}