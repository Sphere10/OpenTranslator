using Omu.AwesomeMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc5MinSetup.Data;
using Mvc5MinSetup.ViewModels.Input;

namespace Mvc5MinSetup.Controllers.Awesome
{
    public class UserController : Controller
    {
		public StringTranslationDBEntities entities =new StringTranslationDBEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult UserDataItems(GridParams g)
        {
			var items = entities.UserMasters.AsQueryable();
            var key = Convert.ToInt32(g.Key);
            var model = new GridModelBuilder<UserMaster>(items, g)
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
        public ActionResult Create(UserInput input)
        {
           if (!ModelState.IsValid)
           {
               return PartialView(input);
           }
		   if(this.doesUserEmailExist(input)==true)
			{
				return PartialView(input);
				
			}
			else
			{
			using (var transaction = entities.Database.BeginTransaction())  
					{  
						try  
						{  
					   UserMaster user= new UserMaster();
						user.EmailId=input.EmailId;
						user.Password=input.Password;
						entities.UserMasters.Add(user);
			     		
							entities.SaveChanges();  
  
							transaction.Commit();  
							return Json(user);
						}  
						catch (Exception ex)  
						{  
							var a= ex;
							transaction.Rollback(); 
					        return Json("");
						}  
					} 
			}
			// returning the key to call grid.api.update
           // return Json(entities.Open_Translator_SP().FirstOrDefault(x => x.TextId == input.TextId));
		   
			            
        }
		[HttpPost]
		public bool doesUserEmailExist(UserInput input) {
			var id = Convert.ToDecimal(input.Id);
			if(input.Id==null)
			{
				var user = entities.UserMasters.Where(x=>x.EmailId==input.EmailId).FirstOrDefault();
				if(user!=null)
				{
					ViewBag.errormsg="Email already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				var user = entities.UserMasters.Where(x=>x.EmailId==input.EmailId&&x.Id!=id).FirstOrDefault();
				if(user!=null)
				{
					ViewBag.errormsg="Email already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}
			

			
		}

		public ActionResult Edit(int id)
        {
          
		    var user = entities.UserMasters.FirstOrDefault(x => x.Id == id);
            return PartialView(
                "Create",
                new UserInput
                    {
						
                        Id = user.Id.ToString(),
                        EmailId = user.EmailId,
                        Password = user.Password
                    });
        }

        [HttpPost]
        public ActionResult Edit(UserInput input)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Create", input);
            }
			var id = Convert.ToDecimal(input.Id);
			if(this.doesUserEmailExist(input)==true)
			{
				 return PartialView("Create", input);
			}
			else
			{
			var user = entities.UserMasters.FirstOrDefault(x => x.Id == id);
			 user.EmailId = input.EmailId;
             user.Password = input.Password;
			 entities.SaveChanges();

            return Json(new {input.Id});
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
            using (var transaction = entities.Database.BeginTransaction())  
			{  
				try  
				{
					 UserMaster user = entities.UserMasters.FirstOrDefault(x => x.Id == input.Id);
					entities.UserMasters.Remove(user);
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