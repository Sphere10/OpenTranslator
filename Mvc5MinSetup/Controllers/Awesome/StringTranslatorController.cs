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

		private static object MapToGridModel(StringTranslationGrid o)
        {
			//var a = Db.Languages.Where(x=> x.LanguageCode==o.LanguageCode).FirstOrDefault();

            return
                new
                {
					Id = o.TextId,
					TextId = o.TextId,
					Text= o.Original_Text,
					Marathi=o.mr,
					Hindi=o.hi,
					French=o.fr,
					Chinese=o.zh,
					Spanish=o.es
                };
        }

        public ActionResult GridGetItems(GridParams g, string search)
        {
            search = (search ?? "").ToLower();
            //var items = entities.TranslationDetailsViews.Where(o => o.Original_Text.ToLower().Contains(search)).AsQueryable();
			 var items = entities.StringTranslationGrids.Where(o => o.Original_Text.ToLower().Contains(search)).AsQueryable();
			 //var items = entities.StringTranslation().Where(o => o.Original_Text.ToLower().Contains(search)).AsQueryable();
            return Json(new GridModelBuilder<StringTranslationGrid>(items, g)
            {
                Key = "TextId", // needed for api select, update, tree, nesting, EF
				GetItem = () => items.Single(x=> x.TextId == g.Key),// called by the grid.api.update ( edit popupform success js func )
 				Map = MapToGridModel
            }.Build());
        }

		public ActionResult Edit(string id)
		{
			var dinner = entities.Translations.Where(x => x.TextId == id).ToList();
			List<string>list =new List<string>();

			var textid= dinner.First().TextId;
			var text= entities.Texts.Where(o=>o.TextId == id).First().Original_Text;
			//var text=dinner.First().Original_Text;
			foreach(var s in dinner)
			{
				
				list.Add(s.LanguageCode);
			}

			var test = entities.StringTranslationGrids.Where(x => x.TextId == id).FirstOrDefault();
			var input = new TranslatorInput
			{
				TextId = textid,
				Text = text,
				LanguageCode = list,
				Marathi=test.mr,
				Hindi=test.hi,
				French=test.fr,
				Chinese=test.zh,
				Spanish=test.es
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
					Text text = entities.Texts.FirstOrDefault(x => x.TextId == input.TextId);
					text.Original_Text = input.Text;
					text.System = true;
					entities.SaveChanges();
					
					foreach(var a in input.LanguageCode)
					{
						Translation translation = entities.Translations.FirstOrDefault(x => x.TextId == input.TextId && x.LanguageCode == a);

						if(translation==null)
						{
							translation =new Translation();
							translation.TextId=input.TextId;
							entities.Translations.Add(translation);  
						}
						 
						translation.LanguageCode = a;
							if(a.Equals("mr"))
							{
								translation.Translated_Text = input.Marathi;
							}
							if(a.Equals("hi"))
							{
								translation.Translated_Text = input.Hindi;
							}
							if(a.Equals("fr"))
							{
							translation.Translated_Text = input.French;
							}
							if(a.Equals("zh"))
							{
								translation.Translated_Text = input.Chinese;
							}
							if(a.Equals("es"))
							{
								translation.Translated_Text = input.Spanish;
							}

							entities.SaveChanges();

						
						
					}
					
					transaction.Commit();
				}
				catch (Exception ex)
				{
					var a = ex;
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

					foreach(var a in input.LanguageCode)
					{
							Translation translation =new Translation();
							translation.TextId=text.TextId;
							translation.LanguageCode=a;

								if(a.Equals("mr"))
								{
									translation.Translated_Text = input.Marathi;
								}
								if(a.Equals("hi"))
								{
									translation.Translated_Text = input.Hindi;
								}
								if(a.Equals("fr"))
								{
								translation.Translated_Text = input.French;
								}
								if(a.Equals("zh"))
								{
									translation.Translated_Text = input.Chinese;
								}
								if(a.Equals("es"))
								{
									translation.Translated_Text = input.Spanish;
								}


							
							entities.Translations.Add(translation);  
							entities.SaveChanges();  
						}

							transaction.Commit();  
						}  
						catch (Exception ex)  
						{  
							var a= ex;
							transaction.Rollback();  
						}  
					}  
			
			
           var dinner = entities.StringTranslationGrids.Where(x=>x.TextId==input.TextId).FirstOrDefault();

            // returning the key to call grid.api.update
            return Json(MapToGridModel(dinner));
        }

    }
}