using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Infrastructure
{
    internal static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> entity, ISpecification<TEntity> spec)
        {
            // _dbContext.Set<Product>().Where(p => p.Id == id).Include(p => p.Brand).Include(p => p.Category).SingleOrDefaultAsync() as T;

            var query = entity;// query = _dbContext.Set<Product>()

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);//query = _dbContext.Set<Product>().Where(p => p.Id == id)
            //if (spec.Wheres != null && spec.Wheres.Count() > 0)
            //    query = spec.Wheres.Aggregate(query, (currentQuery, whereExpression) => currentQuery.Where(whereExpression));
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDesc != null)
                query = query.OrderByDescending(spec.OrderByDesc);

            //Includes
            //1- p => p.Brand
            //2- p => p.Category
            query = spec.Includes.Aggregate(query, (currentQuery, IncludeExpression) => currentQuery.Include(IncludeExpression));

            //query = _dbContext.Set<Product>().Where(p => p.Id == id).Include(p => p.Brand).Include(p => p.Category)

            return query;
        }
    }
}
