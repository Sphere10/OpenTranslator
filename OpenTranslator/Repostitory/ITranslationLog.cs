using System.Collections.Generic;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory 
{
    public interface ITranslationLog : IBaseRepository<TranslationLog>
    {
		TranslationLog GetTranslationLogID(int Id); // R
		List<TranslationLog> GetTranslationLogByCode(string TextId,string code); // R
	}
}