using OpenTranslator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTranslator.Repostitory
{
	public interface ITranslationMode
	{
	
		void InsertTranslationMode(TranslationMode mode); // C
		IEnumerable<TranslationMode> GetTranslationMode(); // R
		TranslationMode GetTranslationModeByID(string TextId, string LanguageCode); // R
		void UpdateTranslationMode(TranslationMode Language); //U
		void DeleteTranslationMode(int Id); //D
		void Save();
	
	}
}