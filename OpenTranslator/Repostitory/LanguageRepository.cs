using System.Collections.Generic;
using System.Linq;

using OpenTranslator.Data;
using OpenTranslator.Models.Input;

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
		
		/// <summary>
        /// Check if language already exists in storage DB
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool IsLanguageAlreadyStoraged(LanguageInput input, string languageName)
        {
            var languageQuery = GetAll().Where(x => x.LanguageCode == input.LanguageCode || x.LanguageName.Equals(languageName));

            return languageQuery.FirstOrDefault() != null;
        }

		/// <summary>
        /// Check if language already exists in storage DB
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool IsLanguageNameAlreadyStoraged(LanguageInput input, string languageName)
        {
            var languageQuery = GetAll().Where(x => (x.LanguageCode==input.LanguageCode|| x.LanguageName.Equals(languageName))&&x.Id!= System.Convert.ToDecimal(input.Id));

            return languageQuery.FirstOrDefault() != null;
        }

        #endregion

    }
}