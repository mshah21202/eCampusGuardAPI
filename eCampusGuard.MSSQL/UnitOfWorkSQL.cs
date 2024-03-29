using System;
using System.Data;
using System.Runtime.CompilerServices;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using eCampusGuard.MSSQL.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace eCampusGuard.MSSQL
{
	public class UnitOfWorkSQL : IUnitOfWork
	{
        public readonly SQLDataContext _context;
        public IBaseRepository<AppUser> AppUsers { get; }
        public IBaseRepository<AppRole> AppRoles { get; }
        public IBaseRepository<AppUserRole> AppUserRoles { get; }
        public IBaseRepository<Notification> Notifications { get; }
        public IBaseRepository<PermitApplication> PermitApplications { get; }
        public IBaseRepository<Permit> Permits { get; }
        public IBaseRepository<Area> Areas { get; }
        public IBaseRepository<AccessLog> AccessLogs { get; }



        public UnitOfWorkSQL(SQLDataContext context)
		{
            _context = context;
            AppUsers = new BaseRepository<AppUser>(_context);
            AppRoles = new BaseRepository<AppRole>(_context);
            AppUserRoles = new BaseRepository<AppUserRole>(_context);
            Notifications = new BaseRepository<Notification>(_context);
            PermitApplications = new BaseRepository<PermitApplication>(_context);
            Permits = new BaseRepository<Permit>(_context);
            Areas = new BaseRepository<Area>(_context);
            AccessLogs = new BaseRepository<AccessLog>(_context);
        }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public async Task<T> CallStoredFunction<T>(string funcName, params KeyValuePair<string, object>[] funcParams) where T : new()
        {
            // Get connection
            var con = (SqlConnection)_context.Database.GetDbConnection();
            // Create command
            var cmd = con.CreateCommand();
            // Set command type and text
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = $"dbo.{funcName}";

            // Open connection & get parameters
            con.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            con.Close();


            // Populate input parameters with their respective values
            foreach (var parameter in funcParams)
            {
                cmd.Parameters[$"@{parameter.Key}"].Value = parameter.Value;
            }

            // Open connection and execute
            con.Open();
            await cmd.ExecuteNonQueryAsync();
            con.Close();

            // Loop over parameters again and build result dictionary
            var paramsEnum = cmd.Parameters.GetEnumerator();
            paramsEnum.Reset();
            T result = new();

            while (paramsEnum.MoveNext())
            {
                SqlParameter current = (SqlParameter)paramsEnum.Current;
                if (current.Direction == ParameterDirection.ReturnValue)
                {
                    return result = (T)current.Value;
                }
            }

            return result;
        }

        public async Task<List<T>> CallStoredTableValuedFunction<T>(string funcName, params KeyValuePair<string, object>[] funcParams) where T : class
        {
            string funcParamsString = string.Join(",", funcParams.Select(p => p.Value));
            FormattableString queryS = FormattableStringFactory.Create("SELECT * FROM " + funcName + "({0})", funcParamsString);
            var query = _context.Set<T>().FromSqlInterpolated(queryS);
            List<T> result = await query.ToListAsync();
            return result;
        }

        public async Task<Dictionary<string, object>> CallStoredProcedure(string procName, KeyValuePair<string, object>[] inputParams)
        {
            // Get connection
            var con = (SqlConnection)_context.Database.GetDbConnection();
            // Create command
            var cmd = con.CreateCommand();
            // Set command type and text
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = $"dbo.{procName}";

            // Open connection & get parameters
            con.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            con.Close();

            // BUG: Output parameter's direction gets set to ParameterDirection.InputOutput
            // FIX: Change ParameterDirection.InputOutput to ParameterDirection.Output
            foreach (SqlParameter item in cmd.Parameters)
            {
                if (item.Direction == ParameterDirection.InputOutput)
                {
                    item.Direction = ParameterDirection.Output;
                }
            }

            // Populate input parameters with their respective values
            foreach (var parameter in inputParams)
            {
                cmd.Parameters[$"@{parameter.Key}"].Value = parameter.Value;
            }

            // Open connection and execute
            con.Open();
            await cmd.ExecuteNonQueryAsync();
            con.Close();

            // Loop over parameters again and build result dictionary
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (SqlParameter item in cmd.Parameters)
            {
                if (item.Direction != ParameterDirection.Input)
                {
                    result.Add(item.ParameterName.Substring(1), item.Value);
                }
            }

            return result;
        }


        public void Commit()
        {
            throw new NotImplementedException();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }


    }
}

