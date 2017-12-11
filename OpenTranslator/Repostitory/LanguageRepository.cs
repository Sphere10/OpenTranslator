using OpenTranslator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace OpenTranslator.Repostitory
{
	public class LanguageRepository : ILanguages
	{
		private StringTranslationEntities DBcontext;
		
		public LanguageRepository(StringTranslationEntities objempcontext)
		{
			this.DBcontext = objempcontext;
		}

		public void InsertLanguage(Language Language)
		{
			DBcontext.Languages.Add(Language);
			DBcontext.SaveChanges();
		}
		public IEnumerable<Language> GetLanguages()
		{
			return DBcontext.Languages.ToList();
		}
		public Language GetLanguageID(int Id)
		{
			return DBcontext.Languages.Find(Id);
		}
		public void UpdateLanguage(Language Language)
		{
			DBcontext.Entry(Language).State = EntityState.Modified;
			DBcontext.SaveChanges();
		}
		public void DeleteLanguage(int Id)
		{
			Language Language = DBcontext.Languages.Find(Id);
			DBcontext.Languages.Remove(Language);
			DBcontext.SaveChanges();
		}

		public void Save()
		{
		}
	}
}