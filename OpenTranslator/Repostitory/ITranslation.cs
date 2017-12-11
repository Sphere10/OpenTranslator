using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
	public interface ITranslation
	{
		void InsertTextTranslation(Text text, Translation translation,TranslationLog translation_log); // C
		void InsertTranslation(Translation translation); // C
		IEnumerable<Text> GetText(); // R
		IEnumerable<Translation> GetTranslation(); // R
		Translation GetTranslationID(string Id); // R
		void UpdateTranslation(Translation translation); //U
		void DeleteTranslation(string TextId); //D
		void Save();
	}
}