using leave_mangment.contracts;
using leave_mangment.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace leave_mangment.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }
        public async Task Create(T entity)
        {
            await _db.AddAsync(entity);
        }

        public void Delete(T entity)
        {
             _db.Remove(entity);
        }

        public async Task<bool> exists(Expression<Func<T, bool>> expression = null)
        {
            IQueryable<T> query = _db;
            return await query.AnyAsync(expression);
        }

        public async Task<T> Find(Expression<Func<T, bool>> expression = null, List<string> Includes = null)
        {
            IQueryable<T> query = _db;
            if (Includes != null) 
            {
                foreach (var table in Includes)
                {
                    query = query.Include(table);
                }
            }
            return await query.FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> FindAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> Includes = null)
        {
            IQueryable<T> query = _db;

            if(expression != null)
            {
                query = query.Where(expression);
            }

            if (Includes != null)
            {
                foreach (var table in Includes)
                {
                    query = query.Include(table);
                }
            }

            if(orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public void Update(T entity)
        {
            _db.Update(entity);
        }
    }
}
