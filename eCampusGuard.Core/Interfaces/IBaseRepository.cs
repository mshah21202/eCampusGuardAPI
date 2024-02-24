using System;
using eCampusGuard.Core.Consts;
using System.Linq.Expressions;

namespace eCampusGuard.Core.Interfaces
{
    public interface IBaseRepository<T> : IBaseRepositoryReadOnly<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(string[] includes = null, Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, bool Tracking = false, string[] includes = null);
        //IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? take=0, int? skip = 0);

        //Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? skip = 0, int? take = 0);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null, Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending, int? skip = 0, int? take = 0);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        T Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void Attach(T entity);
        void AttachRange(IEnumerable<T> entities);
        int Count();
        int Count(Expression<Func<T, bool>> criteria);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);
    }
}

