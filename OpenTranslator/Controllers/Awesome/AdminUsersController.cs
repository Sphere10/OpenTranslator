using System;
using System.Linq;
using System.Web.Mvc;

using Omu.AwesomeMvc;

using OpenTranslator.Data;
using OpenTranslator.Models.Input;

namespace OpenTranslator.Controllers.Awesome
{
    public class AdminUsersController : BaseController
    {
        #region Constructor
        public AdminUsersController(){ }
        #endregion

        #region public methods
        
        // GET: User
        public ActionResult Index()
		{
			if (Request.Cookies["UserId"] == null)
			{
				return RedirectToAction("Index", "User");
			}
			
			return View("Index","_AdminLayout");
			
		}
		public ActionResult embeded()
		{
			return View("Index","_LayoutEmbedAdmin");
		}

		public ActionResult UserDataItems(GridParams g)
		{
			var items = IUser.GetAll().AsQueryable();
			var key = Convert.ToInt32(g.Key);

            var model = new GridModelBuilder<UserMaster>(items, g)
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
		public ActionResult Create(UserInput input)
		{
			if (!ModelState.IsValid || this.DoesUserEmailExist(input) == true)
			{
				return PartialView(input);
			}

			try
			{
				UserMaster user = new UserMaster {
                    EmailId = input.EmailId,
                    Password = input.Password,
                };

                IUser.Save(user);
				return Json(user);
			}
			catch (Exception ex)
			{
                Console.WriteLine("Error trying to insert user ", ex);
				return Json("");
			}
		}


		[HttpPost]
		public bool DoesUserEmailExist(UserInput input)
		{
            var userQuery = IUser.GetAll().Where(x => x.EmailId == input.EmailId);

            // This means it's an update of existing user
            if (input.Id != null)
            {
                userQuery = userQuery.Where(y => y.Id != Convert.ToDecimal(input.Id));
            }

            var user = userQuery.FirstOrDefault();

            if (user != null)
            {
                ViewBag.errormsg = "Email already exist.";
                return true;
            }

            return false;

		}

		public ActionResult Edit(int id)
		{

			var user = IUser.GetUserID(id);
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
			if (!ModelState.IsValid || this.DoesUserEmailExist(input) == true)
			{
				return PartialView("Create", input);
			}

			var user = IUser.GetUserID(Convert.ToInt32(input.Id));

            user.EmailId = input.EmailId;
			user.Password = input.Password;

            IUser.Update(user);

            return Json(new { input.Id });

		}
		public ActionResult Delete(string id, string gridId)
		{
			return PartialView(new DeleteConfirmInput
			{
				TextId = id,
				Message = string.Format("Are you sure you want to delete?")
			});
		}

		[HttpPost]
		public ActionResult Delete(DeleteConfirmInput input)
		{
			try
			{
                IUser.Delete(input.Id);
			}
			catch (Exception ex)
			{
                Console.WriteLine("Error trying to Delete User. ", ex);
			}

			return Json(new { Id = input.TextId });
		}

        #endregion
    }
}
