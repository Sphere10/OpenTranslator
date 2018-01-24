using System;
using System.Data.Entity;
using System.Linq;

using OpenTranslator.Data;

namespace OpenTranslator.Repostitory
{
    /// <summary>
    /// Base Repository Implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T: class
    {
        private StringTranslationEntities DBcontext;

        #region Base constructor
        
        public BaseRepository()
        {
            DBcontext = new StringTranslationEntities();
        }

        #endregion

        #region public getter methods

        public StringTranslationEntities GetStringTranslationEntities()
        {
            return this.DBcontext;
        }

        #endregion

        #region interface implementations

        /// <summary>
        /// Delete a range of objects that belongs to a particulary Text objects, then removes the Text object itself
        /// </summary>
        /// <param name="textId"></param>
        public void DeleteRange(string textId)
        {
            using (var transaction = DBcontext.Database.BeginTransaction())
            {
                DbSet listOfEntities = GetStringTranslationEntities().Set(typeof(T));

                try
                {
                    //TODO: improve this!
                    if (typeof(Translation).IsAssignableFrom(typeof(T)))
                    {
                        listOfEntities.RemoveRange(GetStringTranslationEntities().Translations.Where(x => x.TextId == textId.ToString()));
                    }

                    if (typeof(TranslationLog).IsAssignableFrom(typeof(T)))
                    {
                        listOfEntities.RemoveRange(GetStringTranslationEntities().TranslationLogs.Where(x => x.TextId == textId.ToString()));
                    }

                    GetStringTranslationEntities().SaveChanges();

                    Text text = GetStringTranslationEntities().Texts.Where(x => x.TextId == textId.ToString()).FirstOrDefault();
                    GetStringTranslationEntities().Texts.Remove(text);
                    GetStringTranslationEntities().SaveChanges();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine("There is an error", e);
                }
            }
        }

        /// <summary>
        /// Delete an object from the StringTranslationEntities and save the changes to DB
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            DbSet listOfEntities = GetStringTranslationEntities().Set(typeof(T));
            var objectToRemove = listOfEntities.Find(id);

            //This is because some models have their id's as string type. TODO: Fix this and use only one type for id's.
            if (objectToRemove == null)
            {
                objectToRemove = GetStringTranslationEntities().Set(typeof(T)).Find(id.ToString());
            }

            try
            {
                listOfEntities.Remove(objectToRemove);
                GetStringTranslationEntities().SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine("There is an error", e);
            }
            
        }

        /// <summary>
        /// Add a new object to the StringTranslationEntities and save the changes to DB
        /// </summary>
        /// <param name="objToSave"></param>
        public void Save(T objToSave)
        {
            using (var transaction = DBcontext.Database.BeginTransaction())
            {
                try
                {
                    GetStringTranslationEntities().Set(typeof(T)).Add(objToSave);
                    GetStringTranslationEntities().SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine("There was an error trying to save the object. ", e);
                }

            }

        }


        /// <summary>
        /// Update an existing object from the StringTranslationEntities and save the changes on DB
        /// </summary>
        /// <param name="objToUpdate"></param>
        public void Update (T objToUpdate)
        {
            try
            {
                GetStringTranslationEntities().Entry(objToUpdate).State = EntityState.Modified;
                GetStringTranslationEntities().SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error trying to update the object. ", e);
            }

        }

        #endregion
    }
}