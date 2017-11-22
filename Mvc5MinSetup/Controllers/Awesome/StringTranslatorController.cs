using Mvc5MinSetup.Models;
using Mvc5MinSetup.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.AwesomeMvc;
using Mvc5MinSetup.ViewModels.Input;
using HtmlAgilityPack;

namespace Mvc5MinSetup.Controllers.Awesome
{
    public class StringTranslatorController : Controller
    {
      public StringTranslationDBEntities entities =new StringTranslationDBEntities();

		private static object MapToGridModel(TranslationDetailsView o)
        {
			var a = Db.Languages.Where(x=> x.LanguageCode==o.LanguageCode).FirstOrDefault();
            return
                new
                {
					Id = o.TextId,
					TextId = o.TextId,
					Text= o.Original_Text,
					Translated_Text=o.Translated_Text,
					LanguageName = a.LanguageName
                };
        }

        public ActionResult GridGetItems(GridParams g, string search)
        {
            search = (search ?? "").ToLower();
            var items = entities.TranslationDetailsViews.Where(o => o.Original_Text.ToLower().Contains(search)).AsQueryable();

	
            return Json(new GridModelBuilder<TranslationDetailsView>(items, g)
            {
                Key = "TextId", // needed for api select, update, tree, nesting, EF
				GetItem = () => items.Single(x=> x.TextId == g.Key),// called by the grid.api.update ( edit popupform success js func )
 				Map = MapToGridModel
            }.Build());
        }

		 public ActionResult Edit(string id)
        {
            var dinner = entities.TranslationDetailsViews.Where(x=>x.TextId==id).FirstOrDefault();

            var input = new TranslatorInput
            {
                TextId = dinner.TextId,
                Text = dinner.Original_Text,
                LanguageCode= dinner.LanguageCode
            };

            return PartialView("Create", input);
        }

        [HttpPost]
        public ActionResult Edit(TranslatorInput input)
        {
            if (!ModelState.IsValid) return PartialView("Create", input);
            
					using (var transaction = entities.Database.BeginTransaction())  
					{  
						try  
						{  
							Text text= entities.Texts.FirstOrDefault(x=>x.TextId == input.TextId);
							text.Original_Text=input.Text;
							text.System=true;
							entities.SaveChanges();  

							Translation translation = entities.Translations.FirstOrDefault(x=>x.TextId == input.TextId);
							translation.LanguageCode=input.LanguageCode;
							translation.Translated_Text=input.Text;
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

		public ActionResult Delete(string id)
        {
            return PartialView(new DeleteConfirmInput
            {
                TextId = id,
                Message = string.Format("Are you sure you want to delete record ?")
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
							entities.Translations.Remove(translation);
							entities.SaveChanges();
							Text text = entities.Texts.FirstOrDefault(x => x.TextId == input.TextId);
							entities.Texts.Remove(text);
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
             return Json(new { Id = input.TextId });
        }

        public ActionResult Create()
        {
            return PartialView();
        }

		[HttpPost]
        public ActionResult Create(TranslatorInput input)
        {
            if (!ModelState.IsValid) return PartialView(input);

			using (var transaction = entities.Database.BeginTransaction())  
					{  
						try  
						{  
							string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input.Text, "en|" + input.LanguageCode);
							HtmlWeb web = new HtmlWeb();  
							HtmlDocument doc = web.Load(url); 
					
							var itemList = doc.DocumentNode.SelectNodes("//span[@id='result_box']")
								  .Select(p => p.InnerText)
								  .ToList();
							var TranslatedText = itemList[0];

							long ticks = DateTime.Now.Ticks;
							byte[] bytes = BitConverter.GetBytes(ticks);
							string id = Convert.ToBase64String(bytes)
													.Replace('+', '_')
													.Replace('/', '-')
													.TrimEnd('=');
							input.TextId = id;
							Text text= new Text();
							text.Original_Text=input.Text;
							text.TextId= input.TextId;
							text.System=true;
							entities.Texts.Add(text);  
							entities.SaveChanges();  

							Translation translation =new Translation();
							translation.LanguageCode=input.LanguageCode;
							translation.TextId=text.TextId;
							translation.Translated_Text=TranslatedText;
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
			
			
           var dinner = entities.TranslationDetailsViews.Where(x=>x.TextId==input.TextId).FirstOrDefault();
            // returning the key to call grid.api.update
            return Json(MapToGridModel(dinner));
        }

    }
}