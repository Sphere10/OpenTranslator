using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Mvc5MinSetup.Models;

using Omu.AwesomeMvc;
using Mvc5MinSetup.Data;

namespace Mvc5MinSetup.Controllers.Awesome
{
    public class LanguagesController : Controller
    {
		public StringTranslationDBEntities entities =new StringTranslationDBEntities();

        public ActionResult GetAllLanguages()
        {
            var items = entities.Languages.AsEnumerable().Select(o => new KeyContent(o.LanguageCode, o.LanguageName));
            return Json(items);
        }
		 public ActionResult get(string code)
        {
            var items = entities.Languages.Where(x=> x.LanguageCode==code).FirstOrDefault();
            return Json(items);
        }
    }
}