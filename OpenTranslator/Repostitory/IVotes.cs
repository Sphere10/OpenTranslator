using OpenTranslator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTranslator.Repostitory
{
	public interface IVotes
	{
		Vote GetVoteByCookieID(Guid CookieID);
		List<Vote> GetVoteList(Guid CookieID);
		void InsertVote(Vote vote); // C
		void RemoveVote(decimal voteId);
		Vote GetVoteBytranslationId(int id,Guid CookieID);
	}
}