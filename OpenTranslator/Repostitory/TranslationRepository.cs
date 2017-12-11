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
		private StringTranslationEntities DBcontext;


		public TranslationRepository(StringTranslationEntities objempcontext)
		{
			this.DBcontext = objempcontext;
		}

		public void InsertTextTranslation(Text text, Translation translation, TranslationLog translation_log)
		{
			using (var transaction = DBcontext.Database.BeginTransaction())
			{
				try
				{
					DBcontext.Texts.Add(text);
					DBcontext.SaveChanges();
					DBcontext.Translations.Add(translation);
					DBcontext.SaveChanges();
					DBcontext.TranslationLogs.Add(translation_log);
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
		public Translation GetTranslationID(string Id)
		{
			return DBcontext.Translations.Where(x => x.TextId == Id).FirstOrDefault();
		}
		public void UpdateTranslation(Translation translation)
		{
			DBcontext.SaveChanges();
			DBcontext.Entry(translation).State = EntityState.Modified;
		}
		public void DeleteTranslation(string TextId)
		{
			using (var transaction = DBcontext.Database.BeginTransaction())
			{
				try
				{

					DBcontext.Translations.RemoveRange(DBcontext.Translations.Where(x => x.TextId == TextId));
					DBcontext.SaveChanges();
					Text text = DBcontext.Texts.Where(x => x.TextId == TextId).FirstOrDefault();
					DBcontext.Texts.Remove(text);
					DBcontext.SaveChanges();
					transaction.Commit();
				}
				catch
				{
					transaction.Rollback();
				}
			}
		}

		public void Save()
		{
		}
	}
}