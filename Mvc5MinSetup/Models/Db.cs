using System;
using System.Collections.Generic;
using System.Linq;
using Omu.ValueInjecter;

namespace Mvc5MinSetup.Models
{
    public static class Db
    {
        public static IList<Language> Languages = new List<Language>();
        public static int Gid = 151;

        public static object Set<T>()
        {
            if (typeof(T) == typeof(Language)) return Languages;
            return null;
        }

		public static T Insert<T>(T o) where T : Entity
        {
           // o.Id = Gid += 2;
            ((IList<T>)Set<T>()).Add(o);
            return o;
        }
        static Db()
        {
            Insert(new Language { LanguageName = "English", LanguageCode = "en" });
            Insert(new Language { LanguageName = "French", LanguageCode = "fr" });
            Insert(new Language { LanguageName = "Japaneese", LanguageCode = "jp" });
            Insert(new Language { LanguageName = "Hindi", LanguageCode = "hn" });
            Insert(new Language { LanguageName = "Tamil", LanguageCode = "tl" });
			
           
        }
		
    }
}