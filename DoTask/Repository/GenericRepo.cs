using DoTask.Helpers;
using DoTask.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DoTask.Repository
{
    public  class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        protected ApplicationDbContext RepositoryContext { get; set; }

        public GenericRepo(ApplicationDbContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        public IQueryable<T> FindAll()
        {
            return this.RepositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            try
            {
                return this.RepositoryContext.Set<T>()
                .Where(expression).AsNoTracking();
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }

        public void Create(T entity)
        {
            try
            {
                this.RepositoryContext.Set<T>().Add(entity);
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }

        public void Update(T entity)
        {
            try
            {
                RepositoryContext.Set<T>().Attach(entity);
                RepositoryContext.Entry(entity).State = EntityState.Modified;
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }

        public void Delete(T entity)
        {
            try
            {
                RepositoryContext.Entry(entity).State = EntityState.Deleted;
                this.RepositoryContext.Set<T>().Remove(entity);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        public async Task SaveAsync()
        {
            await RepositoryContext.SaveChangesAsync();
        }
    }
}



   