using System.Collections.Generic;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public interface ITranslationLog
	{
		void InsertTranslationLog(TranslationLog translation_Log); // C
		IEnumerable<TranslationLog> GetTranslationLog(); // R
		TranslationLog GetTranslationLogID(int Id); // R
		List<TranslationLog> GetTranslationLogByCode(string TextId,string code); // R
		void UpdateTranslationLog(TranslationLog translationLog); //U
		void DeleteTranslationLog(string TextId); //D
		void Save();
	}
}