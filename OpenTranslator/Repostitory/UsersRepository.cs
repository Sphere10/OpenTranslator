using System.Linq;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public class UsersRepository : BaseRepository<UserMaster>, IUsers
	{
        #region Constructor

        public UsersRepository() : base() { }

        #endregion

        #region IUser Implementations

		public UserMaster GetUserID(int Id)
		{
			return GetDbContext().UserMasters.Find(Id);
		}

		public UserMaster GetUserByMailAndPwd(string Email,string password)
		{
			return GetDbContext().UserMasters.Where(x=>x.EmailId==Email&&x.Password==password).FirstOrDefault();
		}

        #endregion

        

	}
}