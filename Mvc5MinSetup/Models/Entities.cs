using System;
using System.Collections.Generic;

namespace Mvc5MinSetup.Models
{
    public class Entity
    {
        public string TextId { get; set; }
		public string Text { get; set; }
		public bool System { get; set; }
	}	

	 public class Language : Entity
    {
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
    }
}