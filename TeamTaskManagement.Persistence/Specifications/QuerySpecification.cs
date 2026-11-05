using System.Linq.Expressions;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Domain.Specifications;

namespace TeamTaskManagement.Persistence.Specifications {
    /// <summary>
    /// کلاس پایه برای الگوی اسپاسیفیکیشن 
    /// برای کوئری های دیتابیس لایه پرسیستنس
    /// </summary>
    public abstract class QuerySpecification
        <TEntity> where TEntity : BaseEntity {

        /// <summary>
        /// سازنده ی اول: از اسپاسیفیکیشن دامین می گیره
        /// </summary>
        protected QuerySpecification(Specification<TEntity> domainSpec) {
            Criteria = domainSpec.ToExpression();
        }

        /// <summary>
        /// سازنده ی دوم: برای کوئری های خالص بدون قانون دامنه ای
        /// </summary>
        protected QuerySpecification(Expression<Func<TEntity, bool>>? criteria)
            => Criteria = criteria;

        public bool IsSplitQuery { get; protected set; }

        public Expression<Func<TEntity, bool>>? Criteria { get; }

        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = new();

        public List<string> IncludeStrings { get; } = new();

        public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }

        public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; private set; }

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
            => IncludeExpressions.Add(includeExpression);

        protected void AddInclude(string includeString)
            => IncludeStrings.Add(includeString);

        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
            => OrderByExpression = orderByExpression;

        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
            => OrderByDescendingExpression = orderByDescendingExpression;
    }
}
