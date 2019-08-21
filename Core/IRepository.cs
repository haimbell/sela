using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Core
{
    /// <summary>
    /// Repository is our interface between the domain and data layer
    /// </summary>
    /// <typeparam name="T">Type of object the <see cref="IRepository{T}"/> is representing</typeparam>
    public interface IRepository<T>
        where T : class
    {

        /// <summary>
        /// Set Object
        /// </summary>
        void Set(T item);

        /// <summary>
        /// Get Object by Id
        /// </summary>
        T Get(object id);


        IEnumerable<T> All();
    }


    public interface IContext
    {
        void Add<T>(T entity);

        IEnumerable<T> All<T>();

    }

    /// <summary>
    /// Factory will use to create a <see cref="IContext"/> instance who deals with where we store and how we query the data
    /// </summary>
    public interface IContextFactory
    {
        /// <summary>
        /// Get an <see cref="IContext"/> isntance
        /// </summary>
        /// <returns></returns>
        IContext CreateContext();
    }
}