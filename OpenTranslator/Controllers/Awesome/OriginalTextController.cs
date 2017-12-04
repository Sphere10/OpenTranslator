using System;
using System.Linq;
using System.Web.Mvc;

using OpenTranslator.Models;
using OpenTranslator.ViewModels.Input;

using Omu.AwesomeMvc;
using OpenTranslator.Data;
using System.Collections.Generic;
using OpenTranslator.Utils;

namespace OpenTranslator.Controllers
{
    public class OriginalTextController : Controller
    {
		public StringTranslationDBEntities entities =new StringTranslationDBEntities();
		
		public ActionResult GetColumnsItems(string[] columns)
		{
			List<Language> list= new List<Language>();
			list = entities.Languages.ToList();
			var column = new List<String>();
			for (int i = 0; i < list.Count; i++)
			{
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
			columns.Add(new Column { Bind = "Original_Text", Header = "Original Text" });

			for (int i = 0; i < list.Count; i++)
			{
				columns.Add(new Column { Header = list[i].LanguageName.ToString(),  Bind = list[i].LanguageCode.ToString() });
			}
			//columns.Add(new Column { ClientFormat = GridUtils.EditFormatForGrid("OriginalTextGrid","TextId"), Width = 50 });
			//columns.Add(new Column { ClientFormat = GridUtils.DeleteFormatForGrid("OriginalTextGrid","TextId"), Width = 50 });
			 g.Columns = columns.ToArray();

			var baseColumns = new[] { "TextId", "Original Text"};

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
            // needed so we could add addresses even before the restaurant is created/saved
            long ticks = DateTime.Now.Ticks;
			byte[] bytes = BitConverter.GetBytes(ticks);
			string id = Convert.ToBase64String(bytes)
									.Replace('+', '_')
									.Replace('/', '-')
									.TrimEnd('=');

            return PartialView(new TextInput { TextId = id });
        }

        [HttpPost]
        public ActionResult Create(TextInput input)
        {
           if (!ModelState.IsValid)
           {
               return PartialView(input);
           }

            var Originaltext = entities.Texts.FirstOrDefault(x => x.TextId == input.TextId);
            if (Originaltext == null)
            {
                Text text = new Text();
                text.TextId = input.TextId;
                text.System = true;
                entities.Texts.Add(text);
                entities.SaveChanges();
                // use MapToGridModel like in Grid Crud Demo when grid uses Map
                return Json(entities.Texts.FirstOrDefault(x => x.TextId == input.TextId));
            }
            else
            {
                Originaltext.System = true;
                entities.SaveChanges();
                // use MapToGridModel like in Grid Crud Demo when grid uses Map
                return Json(entities.TranslationSP().FirstOrDefault(x => x.TextId == input.TextId));
            }
        }

        public ActionResult Edit(string id)
        {
            var Originaltext = entities.Texts.FirstOrDefault(x => x.TextId == id);
            return PartialView("Create", new TextInput { TextId = id });
        }

        [HttpPost]
        public ActionResult Edit(TextInput input)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Create", input);
            }

             var Originaltext = entities.Texts.FirstOrDefault(x => x.TextId == input.TextId);
			
			 entities.SaveChanges();
            return Json(new { Id =  input.TextId });
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