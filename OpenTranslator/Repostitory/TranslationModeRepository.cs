using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public class TranslationModeRepository : BaseRepository<TranslationMode>, ITranslationMode
	{
        #region Constructor
        public TranslationModeRepository() {}
        #endregion

        #region ITranslationMode implementation
        
        #endregion

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
			return GetDbContext().TranslationModes.Where(x=>x.TextId==TextId && x.LanguageCode==LanguageCode).FirstOrDefault();
		}

		public void InsertTranslationMode(TranslationMode mode)
		{
            GetDbContext().TranslationModes.Add(mode);
            GetDbContext().SaveChanges();
		}

		public void Save()
		{
			throw new NotImplementedException();
		}


		public void UpdateTranslationMode(TranslationMode mode)
		{
            GetDbContext().Entry(mode).State = EntityState.Modified;
            GetDbContext().SaveChanges();
		}

		
	}
}