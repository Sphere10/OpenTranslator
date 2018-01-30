using System;
using System.Collections.Generic;
using System.Linq;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public class TranslationLogRepository : BaseRepository<TranslationLog>, ITranslationLog
	{
        #region Constructor

        public TranslationLogRepository() : base() {}

        #endregion

        #region ITranslationLog implementation

        /// <summary>
        /// Returns all translations logs that has a particular code and textId
        /// </summary>
        /// <param name="TextId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
		public List<TranslationLog> GetTranslationLogByCode(string TextId,string code)
		{
			return GetDbContext().TranslationLogs.Where(x=>x.TextId==TextId && x.LanguageCode==code).ToList();
		}

        /// <summary>
        /// Returns a singular translation by its Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
		public TranslationLog GetTranslationLogID(int Id)
		{
			throw new NotImplementedException();
		}

        #endregion

        
	}
}