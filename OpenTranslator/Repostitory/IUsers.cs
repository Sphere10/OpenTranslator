using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public interface IUsers : IBaseRepository<UserMaster>
	{
		UserMaster GetUserID(int Id); // R
		UserMaster GetUserByMailAndPwd(string Email,string password); // R
	}
}