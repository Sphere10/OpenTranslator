using Mvc5MinSetup.Models;
using Mvc5MinSetup.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omu.AwesomeMvc;
using Mvc5MinSetup.ViewModels.Input;

namespace Mvc5MinSetup.Controllers.Awesome
{
    public class StringTranslatorController : Controller
    {
      public StringTranslationDBEntities entities =new StringTranslationDBEntities();

		private static object MapToGridModel(TranslationDetailsView o)
        {
            return
                new
                {
					Id = o.TextId,
					TextId = o.TextId,
					Text= o.Original_Text,
					Translated_Text=o.Translated_Text,
					LanguageName = o.LanguageCode
                };
        }

        public ActionResult GridGetItems(GridParams g, string search)
        {
            search = (search ?? "").ToLower();
            var items = entities.TranslationDetailsViews.Where(o => o.Original_Text.ToLower().Contains(search)).AsQueryable();

	
            return Json(new GridModelBuilder<TranslationDetailsView>(items, g)
            {
                Key = "TextId", // needed for api select, update, tree, nesting, EF
				/* GetItem = () => items.(Convert.ToInt32(g.Key))*/// called by the grid.api.update ( edit popupform success js func )

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
           
           using (entities = new StringTranslationDBEntities())  
				{  
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
				} 
            

            // returning the key to call grid.api.update
            return Json(new { TextId = input.TextId });
        }

		public ActionResult Delete(string id, string gridId)
        {
            return PartialView(new DeleteConfirmInput
            {
                TextId = id,
                GridId = gridId,
                Message = string.Format("Are you sure you want to delete record ?")
            });
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmInput input)
        {
           using (entities = new StringTranslationDBEntities())  
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
				} 
            

            // returning the key to call grid.api.update
            return Json(new { TextId = input.TextId });
        }

        public ActionResult Create()
        {
            return PartialView();
        }

		[HttpPost]
        public ActionResult Create(TranslatorInput input)
        {
            if (!ModelState.IsValid) return PartialView(input);

			using (entities = new StringTranslationDBEntities())  
				{  
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

							Text text= new Text();
							text.Original_Text=input.Text;
							text.TextId= id;
							text.System=true;
							entities.Texts.Add(text);  
							entities.SaveChanges();  

							Translation translation =new Translation();
							translation.LanguageCode=input.LanguageCode;
							translation.TextId=text.TextId;
							translation.Translated_Text=input.Text;
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
				}  
			
           return PartialView(input);
        }

    }
}