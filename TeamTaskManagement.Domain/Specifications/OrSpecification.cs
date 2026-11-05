using System.Linq.Expressions;

namespace TeamTaskManagement.Domain.Specifications {
    /// <summary>
    /// اور اسپاسیفیکیشن
    /// </summary>
    internal sealed class OrSpecification<T> : Specification<T> where T : class {

        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        internal OrSpecification(Specification<T> left, Specification<T> right) {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> ToExpression() {

            var leftExpr = _left.ToExpression();
            var rightExpr = _right.ToExpression();

            var replaced = new ParameterReplacer(
                rightExpr.Parameters[0],
                leftExpr.Parameters[0])
                .Visit(rightExpr.Body);

            var body = Expression.OrElse(leftExpr.Body, replaced);
            return Expression.Lambda<Func<T, bool>>(body, leftExpr.Parameters[0]);
        }
    }
}
