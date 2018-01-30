using System.Collections.Generic;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public interface ILanguages : IBaseRepository<Language>
	{
		Language GetLanguageID(int Id); 
	}
}