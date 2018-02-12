using System;
using System.Linq;
using System.Web.Mvc;

using Omu.AwesomeMvc;

using System.Collections.Generic;

using OpenTranslator.Models;


namespace OpenTranslator.Controllers.Awesome
{
    public class MissingTranslationController : BaseController
	{
        #region Constructor
        public MissingTranslationController(){}
        #endregion

        #region Public Methods
        
        public ActionResult Index()
		{
			if (Request.Cookies["UserId"] == null)
			{
				return RedirectToAction("Index", "User");
			}
			else
			{
				return View("Index","_AdminLayout");
			}

		}

		public ActionResult embeded()
		{
			return View("Index","_LayoutEmbedAdmin");
		}

		public ActionResult GetAllMissingTranslations(GridParams g)
		{
			var items = ILanguages.GetAll().AsQueryable();

			List<languages> data = new List<languages>();
			AdminController admin= new AdminController();		
            
			foreach (var column in items)
			{
				System.Data.DataTable table = admin.getTable();
                
				for (int i = table.Rows.Count - 1; i >= 0; i--)
				{
					if (table.Rows[i][column.LanguageName].ToString() != "")
						table.Rows[i].Delete();
				}

				languages language= new languages
                {
				    Id= Convert.ToInt32(column.Id), 
				    LanguageCode=column.LanguageCode,
				    LanguageName=column.LanguageName,
				    MissingTranslationCount=table.Rows.Count
                };

				data.Add(language);
			}

			var key = Convert.ToInt32(g.Key);
			var model = new GridModelBuilder<languages>(data.AsQueryable(), g)
			{
				Key = "Id",
				GetItem = () => data.Single(x => x.Id == key)
			}.Build();

			return Json(model);

		}

        #endregion

    }
}


