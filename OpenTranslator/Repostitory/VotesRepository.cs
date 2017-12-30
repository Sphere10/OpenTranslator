using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
	public class VotesRepository: IVotes
	{
		private StringTranslationEntities DBcontext;
		
		public VotesRepository(StringTranslationEntities objempcontext)
		{
			this.DBcontext = objempcontext;
		}
		public Vote GetVoteBytranslationId(int id,Guid CookieID)
		{
			return DBcontext.Votes.Where(x => x.Translation_Id == id && x.CookieID == CookieID).FirstOrDefault();
		}
		public Vote GetVoteByTranslationID(decimal translationId)
		{
			return DBcontext.Votes.Where(x => x.Translation_Id == translationId ).FirstOrDefault();
		}

		public Vote GetVoteByCookieID(Guid CookieID)
		{
			return DBcontext.Votes.Where(x=>x.CookieID==CookieID).FirstOrDefault();
		}
		public List<Vote> GetVoteList(Guid CookieID)
		{
			return DBcontext.Votes.Where(x=>x.CookieID==CookieID).ToList();
		}

		public void InsertVote(Vote vote)
		{
			DBcontext.Votes.Add(vote);
			DBcontext.SaveChanges();
		}
		public void RemoveVote(decimal voteId)
		{
			Vote vote = DBcontext.Votes.Find(voteId);
			DBcontext.Votes.Remove(vote);
			DBcontext.SaveChanges();
		}
		

	}
}