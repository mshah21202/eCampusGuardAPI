using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eCampusGuard.Core.Consts;

namespace eCampusGuard.Core.Interfaces
{
	public interface IBaseRepositoryReadOnly<T> where T: class
	{
        T GetById(int id);
        IEnumerable<T> GetAll(string[] includes = null, Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);
        T Find(Expression<Func<T, bool>> criteria, string[] includes = null);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null, Expression<Func<T, object>> orderBy = null,
         string orderByDirection = OrderBy.Ascending, int? take = 0, int? skip = 0);
    }
}

