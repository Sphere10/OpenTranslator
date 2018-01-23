using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using OpenTranslator.Models;

using Omu.AwesomeMvc;
using OpenTranslator.Data;
using OpenTranslator.Repostitory;
using OpenTranslator.Models.Input;
using System;

namespace OpenTranslator.Controllers.Awesome
{
	public class LanguagesController : Controller
	{
		private ILanguages Ilanguages;
		private ITranslation ITranslation;
		public LanguagesController()
		{
			this.Ilanguages = new LanguageRepository(new StringTranslationEntities());
			this.ITranslation= new TranslationRepository(new StringTranslationEntities());
		}

		public ActionResult GetAllLanguages()
		{
			var items = Ilanguages.GetLanguages().ToList();
			return Json(items.Select(o => new KeyContent(o.LanguageCode, o.LanguageName)));
		}

		public ActionResult LanguageItems(GridParams g)
		{
			var items = Ilanguages.GetLanguages().AsQueryable();
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
			if (!ModelState.IsValid)
			{
				return PartialView(input);
			}

			try
			{
				Language language = new Language();
				language.LanguageCode = input.LanguageCode;
				language.LanguageName = input.LanguageName;
				Ilanguages.InsertLanguage(language);
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

			var language = Ilanguages.GetLanguageID(id);
            var languageExist = ITranslation.GetTranslation().Where(x=> x.LanguageCode == language.LanguageCode).FirstOrDefault();
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
			if (!ModelState.IsValid)
			{
				return PartialView("Create", input);
			}
			var Language = new Language();
			Language.Id = Convert.ToDecimal(input.Id);
			Language.LanguageCode = input.LanguageCode;
			Language.LanguageName = input.LanguageName;
			Ilanguages.UpdateLanguage(Language);
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
			Ilanguages.DeleteLanguage(input.Id);
			return Json(new { input.Id });
		}
	}


}
