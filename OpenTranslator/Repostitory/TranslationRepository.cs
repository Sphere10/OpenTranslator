using System;
using System.Collections.Generic;
using System.Linq;

using OpenTranslator.Data;
using OpenTranslator.Models.Input;

namespace OpenTranslator.Repostitory
{
    public class TranslationRepository : BaseRepository<Translation>, ITranslation
	{
        #region private properties

        private BaseRepository<Text> _baseRepositoryText = new BaseRepository<Text>();
        private BaseRepository<TranslationLog> _baseRepositoryTranslationLog = new BaseRepository<TranslationLog>();
        
        #endregion
        
        #region

        public TranslationRepository() : base(){}
        
        #endregion

        #region ITranslation interface implementation

        /// <summary>
        /// Insert Text object in db, then save translation and create a translation log for this instances
        /// </summary>
        /// <param name="text"></param>
        /// <param name="translation"></param>
        /// <param name="translation_log"></param>
        public void InsertTextTranslation(Text text, Translation translation, TranslationLog translation_log)
		{
            _baseRepositoryText.Save(text); 
            Save(translation);
            _baseRepositoryTranslationLog.Save(translation_log);
		}
		
        /// <summary>
        /// Returns a list of all Text objects in db
        /// </summary>
        /// <returns></returns>
		public IEnumerable<Text> GetText() 
		{
			return GetDbContext().Texts.ToList();
		}
		
        /// <summary>
        /// Returns a particular translation by its Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
		public Translation GetTranslationID(string Id)
		{
			return GetDbContext().Translations.Where(x => x.TextId == Id).FirstOrDefault();
		}

        /// <summary>
        /// Returns a particular translation by its language code and its textId
        /// </summary>
        /// <param name="TextId"></param>
        /// <param name="LanguageCode"></param>
        /// <returns></returns>
		public List<Translation> GetTranslationLogByCode(string TextId,string LanguageCode)
		{
			return GetDbContext().Translations.Where(x=>x.TextId==TextId && x.LanguageCode==LanguageCode).ToList();
		}

        /// <summary>
        /// Returns a particular translation by its TextId
        /// </summary>
        /// <param name="TextId"></param>
        /// <returns></returns>
        public List<Translation> GetTranslationByTextID(string TextId)
		{
			return GetDbContext().Translations.Where(x=>x.TextId==TextId).ToList();
		}

		/// <summary>
        /// Check if Text already exists in storage DB
        /// </summary>
        /// <param name="input"></param>
        /// <param name="translatedText"></param>
        /// <returns></returns>
        public bool IsTextAlreadyStoraged(AdminInput input, string translatedText)
        {
            var textQuery = GetAll().Where(x => x.LanguageCode == input.LanguageCode && x.Translated_Text.Equals(translatedText));

            return textQuery.FirstOrDefault() != null;
        }

        /// <summary>
        /// Check if Translation entity already exists in storage DB
        /// </summary>
        /// <param name="input"></param>
        /// <param name="translatedText"></param>
        /// <returns></returns>
        public bool IsTranslateAlreadyStoraged(AdminInput input, string translatedText)
        {
            var textQuery = GetAll().Where(x => x.TextId == input.TextId && x.LanguageCode == input.LanguageCode && x.Translated_Text.ToLower().Equals(translatedText.ToLower()));

            return textQuery.FirstOrDefault() != null;
        }

        /// <summary>
        /// Check if Text entity is already storaged in DB
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsTextIdAlreadyStoraged(AdminInput input)
        {
            var textQuery = GetText().Where(x => x.TextId.ToLower() == input.TextId.ToLower());

            if (input.Id != 0)
            {
                textQuery = textQuery.Where(y => y.Id != Convert.ToDecimal(input.Id));
            }

            var text = textQuery.FirstOrDefault();

            return text != null;
        }

        #endregion
    }
}