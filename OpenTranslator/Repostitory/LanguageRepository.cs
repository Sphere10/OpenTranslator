using System.Collections.Generic;
using System.Linq;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
	public class LanguageRepository : BaseRepository<Language>, ILanguages
	{
        #region constructor

        public LanguageRepository() : base() { }
        
        #endregion

        #region ILanguage interface implementation

        /// <summary>
        /// Get a particular Language object by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
		public Language GetLanguageID(int Id)
		{
			return GetDbContext().Languages.Find(Id);
		}

        #endregion

    }
}