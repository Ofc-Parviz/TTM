using System.Linq.Expressions;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Domain.Specifications.ForUser {
    /// <summary>
    /// دریافت اطلاعات کاربر بر اساس ایمیل
    /// </summary>
    public sealed class UserByEmailSpecification : Specification<User> {

        private readonly string _email;

        public UserByEmailSpecification(string email) => _email = email.ToLower().Trim();

        public override Expression<Func<User, bool>> ToExpression()
            => user => user.Email == UserEmail.FromDatabase(_email);
        // ولیو آبجکت ایمیل از قبل برای ای-اف-کور بعنوان فیلد متنی ترجمه شده
        // به همین دلیل، توی کوئری باید از تبدیل کردنش
        // خودداری و مستقیما با ولیو آبجکت مقایسه کرد
        // بعنوان مثال، خط زیر اشتباهه و اکسپشن میده :
        // => user => user.Email.Value == _email;
    }
}