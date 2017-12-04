using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Mvc5MinSetup.Models;

using Omu.AwesomeMvc;
using Mvc5MinSetup.Data;
using Mvc5MinSetup.ViewModels.Input;
using System;

namespace Mvc5MinSetup.Controllers.Awesome
{
    public class LanguagesController : Controller
    {
		public StringTranslationDBEntities entities =new StringTranslationDBEntities();

        public ActionResult GetAllLanguages()
        {
            var items = entities.Languages.AsEnumerable().Select(o => new KeyContent(o.LanguageCode, o.LanguageName));
            return Json(items);
        }
		 public ActionResult get(string code)
        {
            var items = entities.Languages.Where(x=> x.LanguageCode==code).FirstOrDefault();
            return Json(items);
        }

		public ActionResult LanguageItems(GridParams g)
        {
			var items = entities.Languages.AsQueryable();
            var key = Convert.ToInt32(g.Key);
            var model = new GridModelBuilder<Language>(items, g)
            {
                Key = "Id", 
                GetItem = () => items.Single(x=> x.Id == key)
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
            using (var transaction = entities.Database.BeginTransaction())  
            {  
                try  
                {  
                    Language language= new Language();
                    language.LanguageCode = input.LanguageCode;
                    language.LanguageName = input.LanguageName;
                    entities.Languages.Add(language);  
                    entities.SaveChanges();  
                    transaction.Commit(); 
                    // use MapToGridModel like in Grid Crud Demo when grid uses Map
                    return Json(language);
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
          
		    var language = entities.Languages.FirstOrDefault(x => x.Id == id);
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
			var id = Convert.ToDecimal(input.Id);
             var language = entities.Languages.FirstOrDefault(x => x.Id == id);
			 language.LanguageCode = input.LanguageCode;
             language.LanguageName = input.LanguageName;
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
            Language language = entities.Languages.FirstOrDefault(x => x.Id == input.Id);
			entities.Languages.Remove(language);
			entities.SaveChanges();

            return Json(new { input.Id });
        }
    }


    }
