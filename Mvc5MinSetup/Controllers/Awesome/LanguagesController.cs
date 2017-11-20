using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Mvc5MinSetup.Models;

using Omu.AwesomeMvc;

namespace Mvc5MinSetup.Controllers.Awesome
{
    public class LanguagesController : Controller
    {
        public ActionResult GetAllLanguages()
        {
            var items = Db.Languages.Select(o => new KeyContent(o.LanguageCode, o.LanguageName));
            return Json(items);
        }
		 public ActionResult get(string code)
        {
            var items = Db.Languages.Where(x=> x.LanguageCode==code).FirstOrDefault();
            return Json(items);
        }
    }
}