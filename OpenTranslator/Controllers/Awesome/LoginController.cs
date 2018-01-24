using System;
using System.Web.Mvc;

using OpenTranslator.Models.Input;
using OpenTranslator.Repostitory;
using OpenTranslator.Data;

namespace OpenTranslator.Controllers.Awesome
{
    public class LoginController : Controller
    {
		private IUsers IUsers;

		public LoginController()
		{
			this.IUsers = new UsersRepository(new StringTranslationEntities());
		}

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // GET: Login/Details/5
        public ActionResult Login(LoginInput input)
        {
			if (!ModelState.IsValid)
			{
				return PartialView(input);
			}

			var a= IUsers.GetUser(input.Email,input.Password);
			if(a!=null)
			{
				if (Request.Cookies["UserId"] == null)
				{
					Response.Cookies["UserId"].Value = a.Id.ToString();
					Response.Cookies["UserId"].Expires = DateTime.Now.AddMonths(1);
				}		
				return Json(new { url = "Test"});
			}
			else
			{
				ViewBag.Message = "Email or Password is incorrect.";
					return PartialView(new LoginInput { Email = input.Email, Password=input.Password});
			}
           
        }
		[HttpPost]
		 public ActionResult LoginNew(string uname , string password)
        {
			var Users= IUsers.GetUser(uname,password);
			if(Users !=null)
			{
				if (Request.Cookies["UserId"] == null)
				{
					Response.Cookies["UserId"].Value = Users.Id.ToString();
					Response.Cookies["UserId"].Expires = DateTime.Now.AddMonths(1);
				}		
				return Json(new { value = "s"});; //RedirectToAction("Index", "Admin");
			}
			else
			{
				ViewBag.Message = "Email or Password is incorrect.";
				return Json(new { value = "Email or Password is incorrect."});;
			}
           
        }

        // GET: Login/Create
        public ActionResult Create()
        {
            return PartialView();
        }
		

        // POST: Login/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
