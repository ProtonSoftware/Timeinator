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
        /// <summary>
        /// TODO: Comments
        /// </summary>
        protected TimeinatorMobileDbContext Db { get; set; }
         
        protected abstract DbSet<T> DbSet { get; }

        public BaseRepository(TimeinatorMobileDbContext db)
        {
            Db = db;
        }

        public virtual void Add(T entity) => DbSet.Add(entity);

        public virtual void Delete(T entity) => DbSet.Remove(entity);

        public void Delete(K id)
        {
            var entity = new T()
            {
                Id = id,
            };
            Db.Entry(entity).State = EntityState.Deleted;
        }

        public T GetById(K id) => DbSet.Find(id);

        public IQueryable<T> GetAll() => DbSet;

        public OperationResult SaveChanges()
        {
            try
            {
                var changes = Db.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                if (ex.ForeginKeyViolation())
                    return new OperationResult("Foreign key violation!");

                throw ex;
            }

            return new OperationResult(true);
        }
    }
}
