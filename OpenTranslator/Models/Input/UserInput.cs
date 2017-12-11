using Omu.AwesomeMvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OpenTranslator.Models.Input
{
    public class UserInput
    {
        [UIHint("Hidden")]
        public string Id { get; set; }

		[Required]
		[Display(Name = "Email")]
		[RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$",ErrorMessage="Please provide valid email id.")]
		public string EmailId { get; set; }

		[Required]
        [Display(Name = "Password")]
		public string Password { get; set; }
      
     

     
    }
}