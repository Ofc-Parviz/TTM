using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.Primitives;

namespace TeamTaskManagement.Persistence.Specifications {
    public static class SpecificationEvaluator {
        /// <summary>
        /// تبدیل کوئری اسپاسیفیکیشن به کوئری مناسب برای ریپوزیتوری ها
        /// </summary>
        public static IQueryable<TEntity> GetQuery
            <TEntity>(IQueryable<TEntity> inputQueryable,
            QuerySpecification<TEntity> specification) where TEntity : BaseEntity {

            IQueryable<TEntity> queryable = inputQueryable;

            if (specification.Criteria is not null)
                queryable = queryable.Where(specification.Criteria);

            queryable = specification.IncludeExpressions.Aggregate(
                queryable, (current, includeExpression) => current.Include(includeExpression));

            queryable = specification.IncludeStrings.Aggregate(
                queryable, (current, include) => current.Include(include));

            if (specification.OrderByExpression is not null)
                queryable = queryable.OrderBy(specification.OrderByExpression);

            else if(specification.OrderByDescendingExpression is not null)
                queryable = queryable.OrderByDescending(specification.OrderByDescendingExpression);

            if (specification.IsSplitQuery)
                queryable = queryable.AsSplitQuery();

            return queryable;
        }
    }
}
