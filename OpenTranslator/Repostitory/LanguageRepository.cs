using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
	public class LanguageRepository : BaseRepository<Language>, ILanguages
	{
        #region constructor
        public LanguageRepository() : base()
		{
		}
        #endregion

        #region ILanguage interface implementation

        /// <summary>
        /// Get all the languages in the store
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Language> GetLanguages()
		{
			return GetStringTranslationEntities().Languages.ToList();
		}

        /// <summary>
        /// Get a particular Language object by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
		public Language GetLanguageID(int Id)
		{
			return GetStringTranslationEntities().Languages.Find(Id);
		}

        #endregion

    }
}