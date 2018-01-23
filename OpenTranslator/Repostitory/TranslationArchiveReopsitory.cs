using System.Collections.Generic;
using System.Linq;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public class TranslationArchiveReopsitory : ITranslationArchive
	{
		private StringTranslationEntities DBcontext;
		
		public TranslationArchiveReopsitory(StringTranslationEntities objempcontext)
		{
			this.DBcontext = objempcontext;
		}
		public void InsertDeletedRecords(string TextId)
		{
				List<Translation> deleteItems = DBcontext.Translations.Where(x => x.TextId == TextId).ToList();
					foreach(var record in deleteItems)
					{
						TranslationArchive archive= new TranslationArchive();
						archive.TextId=record.TextId;
						archive.LanguageCode=record.LanguageCode;
						archive.Translated_Text=record.Translated_Text;
						
						DBcontext.TranslationArchives.Add(archive);
						DBcontext.SaveChanges();
					}

		}
		
	}
}