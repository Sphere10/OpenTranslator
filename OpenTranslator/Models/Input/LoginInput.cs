using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenTranslator.Models.Input
{
	public class LoginInput
	{
		 
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

		[UIHint("Password")]
        [Required(ErrorMessage = "The Password field is required.")]
        public string Password { get; set; }

	}
}