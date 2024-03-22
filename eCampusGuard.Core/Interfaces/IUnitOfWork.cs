using System;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<AppUser> AppUsers { get; }
        IBaseRepository<AppRole> AppRoles { get; }
        IBaseRepository<AppUserRole> AppUserRoles { get; }
        IBaseRepository<Notification> Notifications { get; }
        //IBaseRepository<Attendance> Attendances { get; }

        int Complete();
        Task<int> CompleteAsync();
        void BeginTransaction();
        void Commit();
        void Rollback();

        /// <summary>
        /// Executes a stored procedure with the specified parameters (in & out params)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <returns>
        /// <typeparamref name="T" >T</typeparamref> is the return type of the stored function
        /// </returns>
        Task<Dictionary<string, object>> CallStoredProcedure(string procName, KeyValuePair<string, object>[] inputParams);


        /// <summary>
        /// Executes a user defined stored function, converts <param name="funcParams"/> into parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcName"></param>
        /// <param name="funcParams"></param>
        /// <returns>
        /// <typeparamref name="T" >T</typeparamref> is the return type of the stored function
        /// </returns>
        Task<T> CallStoredFunction<T>(string funcName, params KeyValuePair<string, object>[] funcParams) where T : new();

        Task<List<T>> CallStoredTableValuedFunction<T>(string funcName, params KeyValuePair<string, object>[] funcParams) where T : class;

    }
}

