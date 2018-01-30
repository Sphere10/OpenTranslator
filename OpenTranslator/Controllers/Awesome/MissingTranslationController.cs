using System;
using System.Linq;
using System.Web.Mvc;

using Omu.AwesomeMvc;

using OpenTranslator.Data;
using OpenTranslator.Repostitory;
using OpenTranslator.Models.Input;
using OpenTranslator.Models;
using System.Collections.Generic;

namespace OpenTranslator.Controllers.Awesome
{
    public class MissingTranslationController : Controller
	{
		private ILanguages Ilanguages;
		private ITranslation ITranslation;
		public MissingTranslationController()
		{
			this.Ilanguages = new LanguageRepository();
			this.ITranslation= new TranslationRepository();
		}
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
			var items =Ilanguages.GetAll().AsQueryable();
			List<languages> data = new List<languages>();
			AdminController admin= new AdminController();			
			foreach (var column in items)
					{
						System.Data.DataTable table = admin.getTable();
						for (int i = table.Rows.Count - 1; i >= 0; i--)
						{
							// whatever your criteria is

							if (table.Rows[i][column.LanguageName].ToString() != "")
								table.Rows[i].Delete();
						}
						languages a= new languages();
						a.Id= Convert.ToInt32(column.Id); 
						a.LanguageCode=column.LanguageCode;
						a.LanguageName=column.LanguageName;
						a.MissingTranslationCount=table.Rows.Count;
						data.Add(a);
					}

			var key = Convert.ToInt32(g.Key);
			var model = new GridModelBuilder<languages>(data.AsQueryable(), g)
			{
				Key = "Id",
				GetItem = () => data.Single(x => x.Id == key)
			}.Build();
			return Json(model);

		}

		}
	}


