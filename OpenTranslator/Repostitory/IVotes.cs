using System;
using System.Collections.Generic;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public interface IVotes
	{
		Vote GetVoteByCookieID(Guid CookieID);
		Vote GetVoteByTranslationID(decimal TranslationId);
		List<Vote> GetVoteList(Guid CookieID);
		void InsertVote(Vote vote); // C
		void RemoveVote(decimal voteId);
		Vote GetVoteBytranslationId(int id,Guid CookieID);
	}
}