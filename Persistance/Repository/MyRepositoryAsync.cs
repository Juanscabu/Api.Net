using Application.Interfaces;
using Ardalis.Specification.EntityFrameworkCore;
using Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repository
{
        public class MyRepositoryAsync<T> : RepositoryBase<T>, IRepositoryAsync<T> where T : class 
        {
            private readonly ApplicationDbContext dbContext;

            public MyRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
            {
                this.dbContext = dbContext;
            }
        }
    } 
