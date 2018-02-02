using System.Collections.Generic;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public interface ITranslation : IBaseRepository<Translation>
    {
		void InsertTextTranslation(Text text, Translation translation,TranslationLog translationlog); // C
		IEnumerable<Text> GetText(); // R
		Translation GetTranslationID(string Id); // R
		List<Translation> GetTranslationLogByCode(string TextId,string LanguageCode);
		List<Translation> GetTranslationByTextID(string TextId);
		IEnumerable<Translation> GetTranslation(); 
	}
}