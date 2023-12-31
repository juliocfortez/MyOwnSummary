﻿using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Repositories.IRepository;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MyOwnSummary_API.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(User user)
        {
            _context.Update<User>(user);
            await Save();
        }


        public IQueryable<User> GetAllIncluding(params Expression<Func<User, object>>[] includes) 
        {
            IQueryable<User> query = _context.Users;
            foreach (var include in includes)
            {
                query = _context.Users.Include(include);
            }
            return query;
        }

        
    }
}
