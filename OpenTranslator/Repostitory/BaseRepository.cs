using System;
using System.Data.Entity;

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
            try
            {
                GetStringTranslationEntities().Set(typeof(T)).Add(objToSave);
                GetStringTranslationEntities().SaveChanges();

            }catch (Exception e)
            {
                Console.WriteLine("There was an error trying to save the object. ", e);
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