using OpenTranslator.Data;
using OpenTranslator.Utils;
using OpenTranslator.ViewModels.Input;
using Omu.AwesomeMvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using OpenTranslator.Repostitory;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace OpenTranslator.Controllers.Awesome
{
    public class AdminController : Controller
    {
        public StringTranslationDBEntities entities =new StringTranslationDBEntities();

		private ITranslation ITranslation;
		private ILanguages ILanguages;
		public AdminController()
		{
			this.ITranslation = new TranslationRepository(new StringTranslationDBEntities());
			this.ILanguages = new LanguageRepository(new StringTranslationDBEntities());
		}

        public ActionResult GetColumnsItems(string[] columns)
		{
			List<Language> list= new List<Language>();
			list = ILanguages.GetLanguages().ToList();
			var column = new List<String>();
			for (int i = 0; i < list.Count; i++)
			{
				if(!list[i].LanguageCode.Equals("en"))
					column.Add(list[i].LanguageName.ToString());
			}
			var col = column.ToArray();
			columns = columns ??  col;

			return Json(columns.Select(o => new KeyContent(o, o)));
		}

		public ActionResult Index()
        {
            return View();
        }

        public ActionResult OriginalTextGridGetItems(GridParams g, string[] selectedColumns, bool? choosingColumns)
        {
			List<Language> list= new List<Language>();
			list = ILanguages.GetLanguages().ToList();
			var columns = new List<Column>();
			columns.Add(new Column { Bind = "TextId", Header = "TextId" });

			for (int i = 0; i < list.Count; i++)
			{
				columns.Add(new Column { Header = list[i].LanguageName.ToString(), ClientFormatFunc = "formatCell", Bind = list[i].LanguageCode.ToString() });
			}
			//columns.Add(new Column { ClientFormat = GridUtils.EditFormatForGrid("OriginalTextGrid","TextId"), Width = 50 });
			columns.Add(new Column { ClientFormat = GridUtils.DeleteFormatForGrid("OriginalTextGrid","TextId"), Width = 50 });
			 g.Columns = columns.ToArray();

			var baseColumns = new[] { "TextId", "English"};

			//first load
			if (g.Columns.Length == 0)
			{
				g.Columns = columns.ToArray();
			}

			if (choosingColumns.HasValue)
			{
				selectedColumns = selectedColumns ?? new string[] { };

				//make sure we always have Id and Person columns
				selectedColumns = selectedColumns.Union(baseColumns).ToArray();

				var currectColumns = g.Columns.ToList();

				//remove unselected columns
				currectColumns = currectColumns.Where(o => selectedColumns.Contains(o.Header)).ToList();

				//add missing columns
				var missingColumns = selectedColumns.Except(currectColumns.Select(o => o.Header)).ToArray();

				currectColumns.AddRange(columns.Where(o => missingColumns.Contains(o.Header)));

				g.Columns = currectColumns.ToArray();
			}
			
			
			var languages = ILanguages.GetLanguages().Select(x => x.LanguageCode).Distinct();
			var query = from r in ITranslation.GetTranslation()
						group r by r.TextId into nameGroup
						select new {
							Name = nameGroup.Key,
							Values = from lang in languages
									 join ng in nameGroup 
										  on lang equals ng.LanguageCode into languageGroup
									 select new {
										 Column = lang,
										 Value = languageGroup.Any() ? 
												 languageGroup.First().Translated_Text : null
									 }
						};

			DataTable table = new DataTable();
			var languages1 = ILanguages.GetLanguages().Select(x => x.LanguageName).Distinct();
			table.Columns.Add("TextId");  // first column
			foreach (var language in languages1)
				table.Columns.Add(language); // columns for each language

			foreach (var key in query)
			{
				var row = table.NewRow();
				var items = key.Values.Select(v => v.Value).ToList(); // data for columns
				items.Insert(0, key.Name); // data for first column
				row.ItemArray = items.ToArray();
				table.Rows.Add(row);
			}
			var list1 = table.AsEnumerable().AsQueryable();
            var model = new GridModelBuilder<TranslationSP_Result>(entities.TranslationSP().ToList().AsQueryable(), g)
            {
                Key = "TextId",
                GetItem = () => entities.TranslationSP().Single(x=> x.TextId == g.Key)
            }.Build();
            return Json(model);
        }
		
		public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(AdminInput input)
        {
           if (!ModelState.IsValid)
           {
               return PartialView(input);
           }
		    if(this.doesTextIdExist(input)==true)
			{
				return PartialView(input);
				
			}
			else
			{
			
				input.TextId = input.TextId;
				Text text= new Text();
				text.TextId= input.TextId;
				text.System=true;
			
				Translation translation =new Translation();
				translation.LanguageCode= input.LanguageCode;
				translation.TextId=text.TextId;
				translation.Translated_Text = input.Text;
			
				ITranslation.InsertTextTranslation(text,translation);
  			// returning the key to call grid.api.update
				return Json(entities.TranslationSP().FirstOrDefault(x => x.TextId == input.TextId));
			 }           
        }

		[HttpPost]
		public bool doesTextIdExist(AdminInput input) {

			var id = Convert.ToDecimal(input.Id);
			if(input.Id == 0)
			{
				var text = ITranslation.GetText().Where(x=>x.TextId==input.TextId).FirstOrDefault();
				if(text!=null)
				{
					ViewBag.errormsg="TextId already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				var text = ITranslation.GetText().Where(x=>x.TextId==input.TextId&&x.Id!=id).FirstOrDefault();
				if(text!=null)
				{
					ViewBag.errormsg="TextId already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public ActionResult Edit(string TextId, string code)
		{
			Translation translation = ITranslation.GetTranslation().Where(x => x.TextId == TextId && x.LanguageCode == code).FirstOrDefault();
			
			if(translation==null)
			{
				return PartialView("EditTranslation", new TranslationInput { TextId = TextId, TranslationText = null, LanguageCode = code });
			}
			else
			{
				var id = Convert.ToInt32(translation.Id.ToString());
				return PartialView("EditTranslation", new TranslationInput { TextId = TextId, TranslationText = translation.Translated_Text, LanguageCode = code, Id = id });
			}
			
		}

		
        [HttpPost]
        public ActionResult Edit(TranslationInput input)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("EditTranslation", input);
            }
			if(input.Id != 0)
			{
				var Translatedtext = ITranslation.GetTranslation().Where(x => x.TextId == input.TextId && x.LanguageCode == input.LanguageCode).FirstOrDefault();
				Translatedtext.Translated_Text = input.TranslationText;
				ITranslation.UpdateTranslation(Translatedtext);
				return Json(Translatedtext);
			}
			else
			{
				var Translatedtext = new Translation();
				Translatedtext.Translated_Text = input.TranslationText;
				Translatedtext.LanguageCode = input.LanguageCode;
				Translatedtext.TextId = input.TextId;
				ITranslation.InsertTranslation(Translatedtext);
				return Json(Translatedtext);
			}
             

			 
        }

        public ActionResult Delete(string id, string gridId)
        {
            
            return PartialView(new DeleteConfirmInput
            {
                TextId = id,
                Message = string.Format("Are you sure you want to delete")
            });
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmInput input)
        {
            ITranslation.DeleteTranslation(input.TextId);
            return Json(new { Id = input.TextId });
        }

    }
}