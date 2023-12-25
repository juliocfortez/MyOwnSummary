using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Repositories.IRepository;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace MyOwnSummary_API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }
        public async Task Create(T entity)
        {
            await dbSet.AddAsync(entity);
            await Save();
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;
            if(!tracked) query = dbSet.AsNoTracking();
            if(filter != null) query = query.Where(filter);
            return await query.FirstOrDefaultAsync();
        }

        
        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null) query = query.Where(filter);
            return await query.ToListAsync();
        }

        public async Task Remove(T entity)
        {
            dbSet.Remove(entity);
            await Save();
        }

        public bool Contains(T entity)
        {
            return dbSet.Contains(entity);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
