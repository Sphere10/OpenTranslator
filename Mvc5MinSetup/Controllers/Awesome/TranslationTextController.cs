using System;
using System.Linq;
using System.Web.Mvc;

using Mvc5MinSetup.Models;
using Mvc5MinSetup.ViewModels.Input;

using Omu.AwesomeMvc;
using Mvc5MinSetup.Data;

namespace Mvc5MinSetup.Controllers
{
    public class TranslationTextController : Controller
    {
		public StringTranslationDBEntities entities =new StringTranslationDBEntities();

        public ActionResult GridGetItems(GridParams g, string TextId)
        {
			var items = entities.Translations.Where(o => o.TextId == TextId).AsQueryable();
			var key = Convert.ToInt32(g.Key);
			var model = new GridModelBuilder<Translation>(items, g)
			{
				Key = "Id", 
				GetItem = () => items.Single(x=> x.Id == key)
			}.Build();
			return Json(model);
			
        }

        public ActionResult Create(string TextId)
        {
            return PartialView(new TranslationInput { TextId = TextId });
        }

        [HttpPost]
       public ActionResult Create(TranslationInput input)
       {
            if (!ModelState.IsValid)
            {
               return PartialView(input);
            }
			using (var transaction = entities.Database.BeginTransaction())  
			{  
				try  
				{  
					var Originaltext = entities.Texts.FirstOrDefault(x => x.TextId == input.TextId);
					if(Originaltext == null)
					{
						Text text= new Text();
						text.TextId= input.TextId;
						text.System=false;
						entities.Texts.Add(text);  
						entities.SaveChanges();  
					}
					Translation translation = new Translation();
					translation.TextId = input.TextId;
					translation.LanguageCode = input.LanguageCode;
					translation.Translated_Text = input.TranslationText;
					entities.Translations.Add(translation);  
					entities.SaveChanges();
					transaction.Commit(); 
						// use MapToGridModel like in Grid Crud Demo when grid uses Map
					return Json(translation);
				}  
				catch (Exception ex)  
				{  
					var a= ex;
					transaction.Rollback();
					return PartialView(input);
				}  
            }  

       }

        public ActionResult Edit(int id)
        {
          
		    var translatedtext = entities.Translations.FirstOrDefault(x => x.Id == id);
            return PartialView(
                "Create",
                new TranslationInput
                    {
						
                        TextId = translatedtext.TextId,
                        LanguageCode = translatedtext.LanguageCode,
                        TranslationText = translatedtext.Translated_Text
                    });
        }

        [HttpPost]
        public ActionResult Edit(TranslationInput input)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Create", input);
            }

             var Translatedtext = entities.Translations.FirstOrDefault(x => x.Id == input.Id);
			 Translatedtext.Translated_Text = input.TranslationText;
			 Translatedtext.LanguageCode = input.LanguageCode;
			 entities.SaveChanges();
			 
            return Json(new {input.Id});
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
            Translation translation = entities.Translations.FirstOrDefault(x => x.Id == input.Id);
			entities.Translations.Remove(translation);
			entities.SaveChanges();

            return Json(new { input.Id });
        }
    }
}