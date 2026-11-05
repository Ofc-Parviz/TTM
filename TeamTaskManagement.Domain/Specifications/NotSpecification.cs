using System.Linq.Expressions;

namespace TeamTaskManagement.Domain.Specifications {
    /// <summary>
    /// نات اسپاسیفیکیشن
    /// </summary>
    internal sealed class NotSpecification<T> : Specification<T> where T : class {

        private readonly Specification<T> _inner;

        internal NotSpecification(Specification<T> inner) {
            _inner = inner;
        }

        public override Expression<Func<T, bool>> ToExpression() {

            var expr = _inner.ToExpression();
            var body = Expression.Not(expr.Body);
            return Expression.Lambda<Func<T, bool>>(body, expr.Parameters[0]);
        }
    }
}
