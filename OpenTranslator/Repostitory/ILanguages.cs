using System.Collections.Generic;

using OpenTranslator.Data;
using OpenTranslator.Models.Input;

namespace OpenTranslator.Repostitory
{
    public interface ILanguages : IBaseRepository<Language>
	{
		Language GetLanguageID(int Id); 
		bool IsLanguageAlreadyStoraged(LanguageInput input, string languageName);
		bool IsLanguageNameAlreadyStoraged(LanguageInput input, string languageName);
	}
}