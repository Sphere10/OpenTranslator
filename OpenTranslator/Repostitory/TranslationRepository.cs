using OpenTranslator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace OpenTranslator.Repostitory
{
	public class TranslationRepository : ITranslation
	{
		private StringTranslationDBEntities DBcontext;
		public TranslationRepository(StringTranslationDBEntities objempcontext)
		{
			this.DBcontext = objempcontext;
		}
		public void InsertTextTranslation(Text text, Translation translation)
		{
			using (var transaction = DBcontext.Database.BeginTransaction())  
				{  
					try  
					{
						DBcontext.Texts.Add(text);
						DBcontext.SaveChanges();
						DBcontext.Translations.Add(translation);
						DBcontext.SaveChanges();
						transaction.Commit();  
					}
					catch
					{
						transaction.Rollback();  
					}
			}
			
		}
		public void InsertTranslation(Translation translation)
		{
			using (var transaction = DBcontext.Database.BeginTransaction())  
				{  
					try  
					{
						DBcontext.Translations.Add(translation);
						DBcontext.SaveChanges();
						transaction.Commit();  
					}
					catch
					{
						transaction.Rollback();  
					}
			}
			
		}
		public IEnumerable<Text> GetText()
		{
			return DBcontext.Texts.ToList();
		}
		public IEnumerable<Translation> GetTranslation()
		{
			return DBcontext.Translations.ToList();
		}
		public Translation GetTranslationID(int Id)
		{
			return DBcontext.Translations.Find(Id);
		}
		public void UpdateTranslation(Translation translation)
		{
			DBcontext.Entry(translation).State = EntityState.Modified;
			DBcontext.SaveChanges();
		}
		public void DeleteTranslation(string TextId)
		{
			
			DBcontext.Translations.RemoveRange(DBcontext.Translations.Where(x=>x.TextId == TextId));
			DBcontext.SaveChanges();
			Text text = DBcontext.Texts.Where(x=>x.TextId == TextId).FirstOrDefault();
			DBcontext.Texts.Remove(text);
			DBcontext.SaveChanges();
		}

		public void Save()
		{
		}
	}
}