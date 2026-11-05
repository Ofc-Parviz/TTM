using System.Linq.Expressions;

namespace TeamTaskManagement.Domain.Specifications {
    /// <summary>
    /// این ویزیتور پارامتر اکسپرشن دوم رو با پارامتر اول جایگزین میکنه
    /// تا ای-اف-کور بتونه به اس-کیو-ال ترجمه ش کنه
    /// </summary>
    internal sealed class ParameterReplacer : ExpressionVisitor {

        // اکسپرشن-اینووک توسط بعضی از کوئری-پروایدر
        // ها (از جمله ای-اف-کور در بعضی حالت ها) پشتیبانی
        // نمیشه و موقع ترجمه به اس-کیو-ال خطا میده. راه حل درستش
        //  استفاده از اکسپرشن-ویزیتور برای جایگزینی پارامترهاست

        private readonly ParameterExpression _source;
        private readonly ParameterExpression _target;

        internal ParameterReplacer(ParameterExpression source, ParameterExpression target) {
            _source = source;
            _target = target;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _source ? _target : base.VisitParameter(node);
    }
}
