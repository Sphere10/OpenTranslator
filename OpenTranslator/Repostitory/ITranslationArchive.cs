using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
	public interface ITranslationArchive
	{
		void InsertDeletedRecords(string TextId); // C
		//IEnumerable<Language> GetLanguages(); // R
		//Language GetLanguageID(int Id); // R
		//void UpdateLanguage(Language Language); //U
		//void DeleteLanguage(int Id); //D
		//void Save();
	}
}