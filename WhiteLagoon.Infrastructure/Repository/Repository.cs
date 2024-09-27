using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class Repository<Type> : IRepository<Type> where Type : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<Type> DbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
           DbSet = _db.Set<Type>();
        }

        public IEnumerable<Type> GetAll(Expression<Func<Type, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<Type> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                //Villa, VillaNumber -- case sensitive
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.ToList();
        }
        public Type Get(Expression<Func<Type, bool>> filter, string? includeProperties = null)
        {
            IQueryable<Type> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                //Villa, VillaNumber -- case sensitive
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault()!;
        }
        public void Add(Type entity)
        {
            DbSet.Add(entity);
        }
        public void Delete(Type entity)
        {
            DbSet.Remove(entity);
        }

        public bool Any(Expression<Func<Type, bool>> filter)
        {
            return DbSet.Any(filter);
        }
    }
}
