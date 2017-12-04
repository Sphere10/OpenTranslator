using System.Linq;
using System.Web.Mvc;

using OpenTranslator.Models;

using Omu.AwesomeMvc;

namespace OpenTranslator.Controllers.Awesome.Lookup
{
    public class ChefLookupController : Controller
    {
        //public ActionResult GetItem(int? v)
        //{
        //    var o = Db.Get<Chef>(v) ?? new Chef();
        //    return Json(new KeyContent(o.Id, o.FirstName + " " + o.LastName));
        //}

        //public ActionResult Search(string search, int page)
        //{
        //    const int PageSize = 7;
        //    search = (search ?? "").ToLower().Trim();

        //    var list = Db.Chefs.Where(f => (f.FirstName + " " + f.LastName).ToLower().Contains(search));
        //    return Json(new AjaxListResult
        //    {
        //        Items = list.Skip((page - 1) * PageSize).Take(PageSize).Select(o => new KeyContent(o.Id, o.FirstName + " " + o.LastName)),
        //        More = list.Count() > page * PageSize
        //    });
        //}
    }
}