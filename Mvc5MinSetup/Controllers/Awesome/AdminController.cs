using Mvc5MinSetup.Data;
using Mvc5MinSetup.Utils;
using Mvc5MinSetup.ViewModels.Input;
using Omu.AwesomeMvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Mvc5MinSetup.Controllers.Awesome
{
    public class AdminController : Controller
    {
        public StringTranslationDBEntities entities =new StringTranslationDBEntities();

        public ActionResult GetColumnsItems(string[] columns)
		{
			List<Language> list= new List<Language>();
			list = entities.Languages.ToList();
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
			list = entities.Languages.ToList();
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
			using (var transaction = entities.Database.BeginTransaction())  
					{  
						try  
						{  
			     			long ticks = DateTime.Now.Ticks;
							byte[] bytes = BitConverter.GetBytes(ticks);
							string id = Convert.ToBase64String(bytes)
													.Replace('+', '_')
													.Replace('/', '-')
													.TrimEnd('=');
							input.TextId = id;
							Text text= new Text();
							text.TextId= input.TextId;
							text.System=true;
							entities.Texts.Add(text);  
							entities.SaveChanges();  

							Translation translation =new Translation();
							translation.LanguageCode= input.LanguageCode;
							translation.TextId=text.TextId;
							translation.Translated_Text = input.Text;
							entities.Translations.Add(translation);  
							entities.SaveChanges();  
  
							transaction.Commit();  
						}  
						catch (Exception ex)  
						{  
							var a= ex;
							transaction.Rollback();  
						}  
					}  
			// returning the key to call grid.api.update
            return Json(entities.TranslationSP().FirstOrDefault(x => x.TextId == input.TextId));
			            
        }

		public ActionResult Edit(string TextId, string code)
		{
			Translation translation = entities.Translations.Where(x => x.TextId == TextId && x.LanguageCode == code).FirstOrDefault();
			var id = Convert.ToInt32(translation.Id.ToString());
			return PartialView("EditTranslation", new TranslationInput { TextId = TextId, TranslationText = translation.Translated_Text, LanguageCode = code, Id = id });
		}

		//public ActionResult Edit(string id, string code)
  //      {
  //          var translatedtext = entities.Translations.Where(x => x.TextId == id && x.LanguageCode == code).FirstOrDefault();
  //          return PartialView(
  //              "Create",
  //              new TranslationInput
  //                  {
						
  //                      TextId = translatedtext.TextId,
  //                      LanguageCode = translatedtext.LanguageCode,
  //                      TranslationText = translatedtext.Translated_Text
  //                  });
  //      }

        [HttpPost]
        public ActionResult Edit(TranslationInput input)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("EditTranslation", input);
            }

             var Translatedtext = entities.Translations.Where(x => x.TextId == input.TextId && x.LanguageCode == input.LanguageCode).FirstOrDefault();
			 Translatedtext.Translated_Text = input.TranslationText;
			 entities.SaveChanges();
			 return Json(entities.TranslationSP().FirstOrDefault(x => x.TextId == input.TextId));
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
            using (var transaction = entities.Database.BeginTransaction())  
			{  
				try  
				{
					Translation translation = entities.Translations.FirstOrDefault(x => x.TextId == input.TextId);

					if(translation == null)
					{
						Text text = entities.Texts.FirstOrDefault(x => x.TextId == input.TextId);
						entities.Texts.Remove(text);
					}
					else
					{
						entities.Translations.RemoveRange(entities.Translations.Where(x=>x.TextId == input.TextId));
						entities.SaveChanges();
						Text text = entities.Texts.FirstOrDefault(x => x.TextId == input.TextId);
						entities.Texts.Remove(text);
					}
					
					entities.SaveChanges();
					transaction.Commit();  
				}  
				catch (Exception ex)  
				{  
					var a= ex;
					transaction.Rollback();  
				}  
			} 
            return Json(new { Id = input.TextId });
        }

    }
}