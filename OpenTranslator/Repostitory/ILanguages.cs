using System.Collections.Generic;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public interface ILanguages : IBaseRepository<Language>
	{
		IEnumerable<Language> GetLanguages(); 
		Language GetLanguageID(int Id); 
	}
}