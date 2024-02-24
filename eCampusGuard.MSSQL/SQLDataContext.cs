using System;
using Microsoft.EntityFrameworkCore;

namespace eCampusGuard.MSSQL
{
	public class SQLDataContext : DbContext
	{
		public SQLDataContext()
		{

		}

		public SQLDataContext(DbContextOptions<SQLDataContext> options) : base(options)
		{
			// Create DbSets for entities

			// Define properties, relations, etc.
		}
	}
}

