using Omu.AwesomeMvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mvc5MinSetup.ViewModels.Input
{
	public class TranslatorInput
	{
		
        public string TextId { get; set; }

		[Required]
        public string Text { get; set; }

        [Required]
        [UIHint("Odropdown")]
        [AweUrl(Action = "GetAllLanguages", Controller = "Languages")]
        [DisplayName("Select Language")]
        public string LanguageCode { get; set; }
	}
}