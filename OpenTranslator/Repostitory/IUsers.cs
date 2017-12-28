using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
	public interface IUsers
	{
		void InsertUser(UserMaster User); // C
		IEnumerable<UserMaster> GetUsers(); // R
		UserMaster GetUserID(int Id); // R
		UserMaster GetUser(string Email,string password); // R
		void UpdateUser(UserMaster User); //U
		void DeleteUser(int Id); //D
		void Save();
	}
}