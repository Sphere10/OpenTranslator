using OpenTranslator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace OpenTranslator.Repostitory
{
	public class UsersRepository : IUsers
	{
		private StringTranslationDBEntities DBcontext;
		public UsersRepository(StringTranslationDBEntities objempcontext)
		{
			this.DBcontext = objempcontext;
		}
		public void InsertUser(UserMaster User)
		{
			DBcontext.UserMasters.Add(User);
			DBcontext.SaveChanges();
		}
		public IEnumerable<UserMaster> GetUsers()
		{
			return DBcontext.UserMasters.ToList();
		}
		public UserMaster GetUserID(int Id)
		{
			return DBcontext.UserMasters.Find(Id);
		}
		public void UpdateUser(UserMaster User)
		{
			DBcontext.Entry(User).State = EntityState.Modified;
			DBcontext.SaveChanges();
		}
		public void DeleteUser(int Id)
		{
			UserMaster User = DBcontext.UserMasters.Find(Id);
			DBcontext.UserMasters.Remove(User);
			DBcontext.SaveChanges();
		}

		public void Save()
		{
		}
	}
}