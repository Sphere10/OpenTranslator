
using System.Collections.Generic;

namespace OpenTranslator.Repostitory
{
    /// <summary>
    /// IBase interface will handle all the common methods 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T>
    {
        void Save(T objToSave);
        void Update(T objToUpdate);
        void Delete(int id);
        void DeleteRange(string textId);

        IEnumerable<T> GetAll();
    }
}