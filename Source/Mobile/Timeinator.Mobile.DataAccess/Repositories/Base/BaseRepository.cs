using Microsoft.EntityFrameworkCore;
using System.Linq;
using Timeinator.Core;

namespace Timeinator.Mobile.DataAccess
{
    /// <summary>
    /// Base repository class to derive from by every repository in the application
    /// </summary>
    public abstract class BaseRepository<T, K> : IRepository<T, K> where T : class, IBaseObject<K>, new()
    {
        #region Protected Properties

        /// <summary>
        /// The main database context of this application
        /// </summary>
        protected TimeinatorMobileDbContext Db { get; set; }
         
        /// <summary>
        /// The single table from the database for this repository to use 
        /// </summary>
        protected abstract DbSet<T> DbSet { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="db">The database context of this application</param>
        public BaseRepository(TimeinatorMobileDbContext db)
        {
            Db = db;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Adds specified entity to the table
        /// </summary>
        /// <param name="entity">The entity to add</param>
        public virtual void Add(T entity) => DbSet.Add(entity);

        /// <summary>
        /// Deletes specified entity from the table
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        public virtual void Delete(T entity) => DbSet.Remove(entity);

        /// <summary>
        /// Deletes an entity by specified id
        /// </summary>
        /// <param name="id">The id of entity</param>
        public void Delete(K id)
        {
            // Create empty entity with that id
            var entity = new T()
            {
                Id = id
            };
            
            // Set it's state to deleted so after saving changes it gets deleted
            Db.Entry(entity).State = EntityState.Deleted;
        }

        /// <summary>
        /// Gets entity from table by specified id
        /// </summary>
        /// <param name="id">The id of entity</param>
        /// <returns>Entity, if found</returns>
        public T GetById(K id) => DbSet.Find(id);

        /// <summary>
        /// Gets all saved entities in current table
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll() => DbSet;

        /// <summary>
        /// Attempts to save any changes and make pending queries in the database
        /// Should be called after making changes in entities for the changes to take place
        /// </summary>
        public OperationResult SaveChanges()
        {
            try
            {
                // Save changes in whole database
                Db.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                // Return error
                return new OperationResult(ex.Message);
            }

            // Return success
            return new OperationResult(true);
        }

        #endregion
    }
}
