using System;
using System.Linq;
using System.Web.Mvc;

using Omu.AwesomeMvc;

using OpenTranslator.Data;
using OpenTranslator.Models.Input;

namespace OpenTranslator.Controllers.Awesome
{
    public class LanguagesController : BaseController
	{
        #region Constructor
        public LanguagesController(){}
        #endregion

        #region public methods
        
        public ActionResult Index()
		{
			if (Request.Cookies["UserId"] == null)
			{
				return RedirectToAction("Index", "User");
			}

			return View("Index","_AdminLayout");

		}
		public ActionResult embeded()
		{
			return View("Index","_LayoutEmbedAdmin");
		}

		public ActionResult GetAllLanguages()
		{
			var items = ILanguages.GetAll().ToList();
			return Json(items.Select(o => new KeyContent(o.LanguageCode, o.LanguageName)));
		}

		public ActionResult LanguageItems(GridParams g)
		{
			var items = ILanguages.GetAll().AsQueryable();
			var key = Convert.ToInt32(g.Key);
			var model = new GridModelBuilder<Language>(items, g)
			{
				Key = "Id",
				GetItem = () => items.Single(x => x.Id == key)
			}.Build();

			return Json(model);

		}

		public ActionResult Create()
		{
			return PartialView();
		}

		[HttpPost]
		public ActionResult Create(LanguageInput input)
		{
			if (!ModelState.IsValid || ILanguages.IsLanguageAlreadyStoraged(input, input.LanguageName) == true)
			{
				ViewBag.errormsg = "LanguageCode or Language already exist.";
				return PartialView(input);
			}

			try
			{
				Language language = new Language();
				language.LanguageCode = input.LanguageCode;
				language.LanguageName = input.LanguageName;

                ILanguages.Save(language);
				
                // use MapToGridModel like in Grid Crud Demo when grid uses Map
				return Json(language);
			}
			catch (Exception ex)
			{
				var a = ex;
				return PartialView(input);
			}


		}

		public ActionResult Edit(int id)
		{

			var language = ILanguages.GetLanguageID(id);
            var languageExist = ITranslation.GetAll().Where(x=> x.LanguageCode == language.LanguageCode).FirstOrDefault();

            if(languageExist!=null)
                ViewBag.languageExist = true;

			return PartialView(
				"Create",
				new LanguageInput
				{

					Id = language.Id.ToString(),
					LanguageCode = language.LanguageCode,
					LanguageName = language.LanguageName
				});
		}

		[HttpPost]
		public ActionResult Edit(LanguageInput input)
		{
			if (!ModelState.IsValid || ILanguages.IsLanguageNameAlreadyStoraged(input, input.LanguageName) == true)
			{
				ViewBag.errormsg = "LanguageCode or Language already exist.";
				return PartialView("Create", input);
			}

			var language= ILanguages.GetAll().Where(x=>x.Id==Convert.ToDecimal(input.Id)).FirstOrDefault();
			string[] selectedColumns = (string[])System.Web.HttpContext.Current.Session["SelectedColumns"];

            if (selectedColumns != null)
			{
				selectedColumns = selectedColumns.Select<string,string>(s => s == language.LanguageName ? input.LanguageName : s).ToArray();
				System.Web.HttpContext.Current.Session["SelectedColumns"] = selectedColumns;
			}

			language.LanguageCode = input.LanguageCode;
			language.LanguageName = input.LanguageName;

            ILanguages.Update(language);

			return Json(new { input.Id });
		}

		public ActionResult Delete(int id)
		{
			return PartialView(new DeleteConfirmInput
			{
				Id = id,
				Message = string.Format("Are you sure you want to delete")
			});
		}

		[HttpPost]
		public ActionResult Delete(DeleteConfirmInput input)
		{
			var language = ILanguages.GetLanguageID(input.Id);
			string[] selectedColumns = (string[])System.Web.HttpContext.Current.Session["SelectedColumns"];
			selectedColumns = selectedColumns.Where(s => s != language.LanguageName).ToArray();
			System.Web.HttpContext.Current.Session["SelectedColumns"] = selectedColumns;

            ILanguages.Delete(input.Id);

			return Json(new { input.Id });
		}

        #endregion
    }


}
