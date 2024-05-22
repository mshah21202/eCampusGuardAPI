using System;
using System.Linq.Expressions;
using eCampusGuard.Core.Consts;
using eCampusGuard.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eCampusGuard.MSSQL.Repositories
{
	public class BaseRepository<T> : IBaseRepository<T> where T: class
	{
        protected SQLDataContext _context { get; set; }


        public BaseRepository(SQLDataContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll(string[] includes = null, Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);
            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(string[] includes = null, Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending, bool tracking = false)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            if (!tracking)
            {
                query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public T? GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public T? Find(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return query.SingleOrDefault(criteria);
        }


        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null, Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending, int? take = 0, int? skip = 0)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);
            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);
            if (skip > 0)
                query = query.Skip(skip.Value);

            if (take > 0)
                query = query.Take(take.Value);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return query.ToList();
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> criteria, bool Tracking = false, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (Tracking)
            {
                query = query.AsNoTracking();
            }
            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.SingleOrDefaultAsync(criteria);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null, Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending, int? take = 0, int? skip = 0)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);
            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            if (skip > 0)
                query = query.Skip(skip.Value);

            if (take > 0)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return entities;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public T Update(T entity)
        {
            _context.Update(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Attach(T entity)
        {
            _context.Set<T>().Attach(entity);
        }

        public void AttachRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AttachRange(entities);
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Count(criteria);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().CountAsync(criteria);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending, bool Tracking = false, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (Tracking)
            {
                query = query.AsNoTracking();
            }

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.FirstOrDefaultAsync(criteria);
        }

        public T? FirstOrDefault(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return query.FirstOrDefault(criteria);
        }
    }
}

