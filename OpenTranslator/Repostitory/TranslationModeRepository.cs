using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public class TranslationModeRepository : ITranslationMode
	{
		private StringTranslationEntities DBcontext;
		
		public TranslationModeRepository(StringTranslationEntities objempcontext)
		{
			this.DBcontext = objempcontext;
		}
		public void DeleteTranslationMode(int Id)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TranslationMode> GetTranslationMode()
		{
			throw new NotImplementedException();
		}

		public TranslationMode GetTranslationModeByID(string TextId, string LanguageCode)
		{
			return DBcontext.TranslationModes.Where(x=>x.TextId==TextId && x.LanguageCode==LanguageCode).FirstOrDefault();
		}

		public void InsertTranslationMode(TranslationMode mode)
		{
			DBcontext.TranslationModes.Add(mode);
			DBcontext.SaveChanges();
		}

		public void Save()
		{
			throw new NotImplementedException();
		}


		public void UpdateTranslationMode(TranslationMode mode)
		{
			DBcontext.Entry(mode).State = EntityState.Modified;
			DBcontext.SaveChanges();
		}

		
	}
}