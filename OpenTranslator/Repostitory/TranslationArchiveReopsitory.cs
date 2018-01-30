using System.Collections.Generic;
using System.Linq;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public class TranslationArchiveRepository : BaseRepository<TranslationArchive>, ITranslationArchive
	{
        #region Constructor
        public TranslationArchiveRepository() : base() {}
        #endregion

        #region ITranslationArchive implementation
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TextId"></param>
        public void InsertDeletedRecords(string TextId)
		{
            //TODO: what does this method do exactly?
			List<Translation> deleteItems = GetDbContext().Translations.Where(x => x.TextId == TextId).ToList();

            foreach (var record in deleteItems)
			{
				TranslationArchive archive= new TranslationArchive();
				archive.TextId=record.TextId;
				archive.LanguageCode=record.LanguageCode;
				archive.Translated_Text=record.Translated_Text;

                Save(archive);
			}

		}
		
        #endregion
	}
}