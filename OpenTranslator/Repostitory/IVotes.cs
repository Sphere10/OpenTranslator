using System;
using System.Collections.Generic;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public interface IVotes : IBaseRepository<Vote>
	{
		Vote GetVoteByCookieID(Guid CookieID);
		Vote GetVoteByTranslationID(decimal TranslationId);
		Vote GetVoteByTranslationID(int translationId,Guid CookieID );
		List<Vote> GetVoteList(Guid CookieID);
	}
}