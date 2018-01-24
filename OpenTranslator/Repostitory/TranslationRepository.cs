using System.Collections.Generic;
using System.Linq;

using OpenTranslator.Data;

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

        public void InsertTextTranslation(Text text, Translation translation, TranslationLog translation_log)
		{
            _baseRepositoryText.Save(text);
            Save(translation);
            _baseRepositoryTranslationLog.Save(translation_log);
		}
		
		public IEnumerable<Text> GetText()
		{
			return GetStringTranslationEntities().Texts.ToList();
		}
		public IEnumerable<Translation> GetTranslation()
		{
			return GetStringTranslationEntities().Translations.ToList();
		}
		public Translation GetTranslationID(string Id)
		{
			return GetStringTranslationEntities().Translations.Where(x => x.TextId == Id).FirstOrDefault();
		}
		public List<Translation> GetTranslationLogByCode(string TextId,string LanguageCode)
		{
			return GetStringTranslationEntities().Translations.Where(x=>x.TextId==TextId && x.LanguageCode==LanguageCode).ToList();
		}
        
		public	List<Translation> GetTranslationByTextID(string TextId)
		{
			return GetStringTranslationEntities().Translations.Where(x=>x.TextId==TextId).ToList();
		}

        #endregion
	}
}