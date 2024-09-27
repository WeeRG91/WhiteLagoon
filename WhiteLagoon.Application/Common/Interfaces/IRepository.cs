using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IRepository<Type>where Type : class
    {
        IEnumerable<Type> GetAll(Expression<Func<Type, bool>>? filter = null, string? includeProperties = null);
        Type Get(Expression<Func<Type, bool>> filter, string? includeProperties = null);
        void Add(Type entity);
        bool Any(Expression<Func<Type, bool>> filter);
        void Delete(Type entity);
    }
}
