
namespace TeamTaskManagement.Domain.Exceptions {
    /// <summary>
    /// کلاس پایه برای تمامی اکسپشن های دامین
    /// </summary>
    public abstract class DomainException : Exception {

        protected DomainException(string message) : base(message) { }
    }
    // دامین فعلا به اکسپشن خاصی نداره، اینو
    // اینجا نوشتم اگر بعدا لازم بود بسازم و ازش ارث بری کنم
}
