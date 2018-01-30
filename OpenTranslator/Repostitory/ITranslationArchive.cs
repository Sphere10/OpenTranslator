using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    public interface ITranslationArchive : IBaseRepository<TranslationArchive>
	{
		void InsertDeletedRecords(string TextId); // C
		//Language GetLanguageID(int Id); // R
		//void UpdateLanguage(Language Language); //U
		//void DeleteLanguage(int Id); //D
		//void Save();
	}
}