using System;
using System.Collections.Generic;
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

        public StringTranslationEntities GetDbContext()
        {
            return this.DBcontext;
        }

        #endregion

        #region interface implementations

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
                    GetDbContext().Set(typeof(T)).Add(objToSave);
                    GetDbContext().SaveChanges();
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
        public void Update(T objToUpdate)
        {
            try
            {
                GetDbContext().Entry(objToUpdate).State = EntityState.Modified;
                GetDbContext().SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error trying to update the object. ", e);
            }

        }


        /// <summary>
        /// Delete an object from the StringTranslationEntities and save the changes to DB
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            DbSet listOfEntities = GetDbContext().Set(typeof(T));
            var objectToRemove = listOfEntities.Find(id);

            //This is because some models have their id's as string type. TODO: Fix this and use only one type for id's.
            //if (objectToRemove == null)
            //{
            //    objectToRemove = GetDbContext().Set(typeof(T)).Find(id.ToString());
            //}

            try
            {
                listOfEntities.Remove(objectToRemove);
                GetDbContext().SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("There is an error", e);
            }
            
        }

        /// <summary>
        /// Delete a range of objects that belongs to a particulary Text objects, then removes the Text object itself
        /// </summary>
        /// <param name="textId"></param>
        public void DeleteRange(string textId)
        {
            using (var transaction = DBcontext.Database.BeginTransaction())
            {
                DbSet listOfEntities = GetDbContext().Set(typeof(T));

                try
                {
                    //TODO: improve this!
                    if (typeof(Translation).IsAssignableFrom(typeof(T)))
                    {
                        listOfEntities.RemoveRange(GetDbContext().Translations.Where(x => x.TextId == textId));
                    }

                    if (typeof(TranslationLog).IsAssignableFrom(typeof(T)))
                    {
                        listOfEntities.RemoveRange(GetDbContext().TranslationLogs.Where(x => x.TextId == textId));
                    }

                    GetDbContext().SaveChanges();

                    // In case there is text associated to T entity, remove them all, otherwise do nothing.
                    Text text = GetDbContext().Texts.Where(x => x.TextId == textId).FirstOrDefault();
                    if (text != null)
                    {
                        GetDbContext().Texts.Remove(text);
                        GetDbContext().SaveChanges();
                    }

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
        /// Returns all the items in db
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return GetDbContext().Set(typeof(T)).OfType<T>().AsEnumerable<T>();
        }

        #endregion
    }
}