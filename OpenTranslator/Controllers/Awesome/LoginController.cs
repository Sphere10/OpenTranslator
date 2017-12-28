﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenTranslator.Models.Input;
using OpenTranslator.Repostitory;
using OpenTranslator.Data;
using System.Web.Routing;

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
					return PartialView("Create", new LoginInput { Email = input.Email, Password=input.Password});
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