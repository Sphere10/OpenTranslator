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
        [UIHint("LanguageCode")]
        [DisplayName("Select Language")]
        public List<string> LanguageCode { get; set; }

		public string Marathi { get; set; }
		public string Hindi { get; set; }
		public string French { get; set; }
		public string Chinese { get; set; }
		public string Spanish { get; set; }


	}
}