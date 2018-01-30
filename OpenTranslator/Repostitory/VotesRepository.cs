using System;
using System.Collections.Generic;
using System.Linq;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
	public class VotesRepository: BaseRepository<Vote>, IVotes
	{
        #region Constructor
        public VotesRepository() : base(){}
        #endregion

        #region IVotes implementation

        /// <summary>
        /// Returns a particulary Vote instance, find it by its translationId and its CookieID Guid 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CookieID"></param>
        /// <returns></returns>
        public Vote GetVoteByTranslationID(int translationId,Guid CookieID)
		{
			return GetDbContext().Votes.Where(x => x.Translation_Id == translationId && x.CookieID == CookieID).FirstOrDefault();
		}

        /// <summary>
        /// Returns a particulary Vote instance, finds it by its translationId
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
		public Vote GetVoteByTranslationID(decimal translationId)
		{
			return GetDbContext().Votes.Where(x => x.Translation_Id == translationId ).FirstOrDefault();
		}

        /// <summary>
        /// Returns a particulary Vote instance, finds it by its CookieID Guid
        /// </summary>
        /// <param name="CookieID"></param>
        /// <returns></returns>
		public Vote GetVoteByCookieID(Guid CookieID)
		{
			return GetDbContext().Votes.Where(x=>x.CookieID==CookieID).FirstOrDefault();
		}

        /// <summary>
        /// Returns a list of Vote instances, find them by their CookieID Guid
        /// </summary>
        /// <param name="CookieID"></param>
        /// <returns></returns>
		public List<Vote> GetVoteList(Guid CookieID)
		{
			return GetDbContext().Votes.Where(x=>x.CookieID==CookieID).ToList();
		}

        #endregion

	}
}