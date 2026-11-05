using System.Reflection;

namespace TeamTaskManagement.Persistence {
    /// <summary>
    /// دریافت اسمبلی لایه پرسیستنس
    /// </summary>
    public static class AssemblyReference {

        /// <summary>
        /// اسمبلی حاوی کل عناصر لایه پرسیستنس - برای رفرنس
        /// دادن یکباره کانفیگوریشن ها به دیتابیس کانتکست
        /// </summary>
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
