using System.Data.Entity;

namespace OpenTranslator.Repostitory
{
    /// <summary>
    /// IBase interface will handle all the common methods 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T>
    {
        void Save(T objToSave);
        void Delete(int id);
        void Update(T objToUpdate);
    }
}