using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenTranslator.Data;
using System.Data.Entity;

namespace OpenTranslator.Repostitory
{
	public class TranslationLogRepository : ITranslationLog
	{
		private StringTranslationEntities DBcontext;
		
		public TranslationLogRepository(StringTranslationEntities objempcontext)
		{
			this.DBcontext = objempcontext;
		}
		public void DeleteTranslationLog(string TextId)
		{
			DBcontext.TranslationLogs.RemoveRange(DBcontext.TranslationLogs.Where(x => x.TextId == TextId));
			DBcontext.SaveChanges();
		
		}

		public IEnumerable<TranslationLog> GetTranslationLog()
		{
			return DBcontext.TranslationLogs.ToList();
		}

		public List<TranslationLog> GetTranslationLogByCode(string TextId,string code)
		{
			return DBcontext.TranslationLogs.Where(x=>x.TextId==TextId && x.LanguageCode==code).ToList();
		}

		public TranslationLog GetTranslationLogID(int Id)
		{
			throw new NotImplementedException();
		}

		public void InsertTranslationLog(TranslationLog translation_Log)
		{
			DBcontext.TranslationLogs.Add(translation_Log);
			DBcontext.SaveChanges();
		}

		public void Save()
		{
			throw new NotImplementedException();
		}

		public void UpdateTranslationLog(TranslationLog translation_Log)
		{
			DBcontext.Entry(translation_Log).State = EntityState.Modified;
			DBcontext.SaveChanges();
		}
	}
}