using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Application.Users.Responses;
using static TeamTaskManagement.Shared.Classes.Constants;

namespace TeamTaskManagement.Application.Users.Queries
{
    /// <summary>
    /// مپ کردن ریسپانس های مربوط به کاربر
    /// </summary>
    internal static class UserMappings {

        #region Map User Response

        /// <summary>
        /// تبدیل اطلاعات خام کاربر به ریسپانس مناسب
        /// </summary>
        public static UserResponse MapUserResponse(User? user) {

            return new UserResponse
                (user?.Id ?? Guid.Empty,
                 user?.Email.Value ?? NotAvailable,
                 user?.Displayname.Value ?? NotAvailable,
                 user?.Firstname.Value ?? NotAvailable,
                 user?.Lastname.Value ?? NotAvailable,
                 user?.CreatedAtUtc ?? DateTime.MinValue,
                 user?.LastModifiedAtUtc ?? DateTime.MinValue);
        }

        #endregion

        #region Map User Simple Response

        /// <summary>
        /// تبدیل اطلاعات خام کاربر به مینی ریسپانس مناسب
        /// </summary>
        public static UserSimpleResponse MapUserSimpleResponse(User? user)
            => MapUserSimpleResponse
               (user?.Id ?? Guid.Empty,
                user?.Email.Value ?? NotAvailable,
                user?.Displayname.Value ?? NotAvailable);

        public static UserSimpleResponse MapUserSimpleResponse
            (Guid userId,
             string email,
             string displayname)
            => new UserSimpleResponse(userId, email, displayname);

        public static UserSimpleResponse MapUserSimpleResponse
            (string email,
             string displayname)
            => MapUserSimpleResponse(Guid.Empty, email, displayname);

        /// <summary>
        /// تبدیل اطلاعات خام کاربر به مینی ریسپانس مناسب
        /// - دریافت نام نمایشی و ایمیل از دیکشنری
        /// </summary>
        public static UserSimpleResponse MapUserSimpleResponse
            (Guid userId, IReadOnlyDictionary<Guid, (string Email, string Displayname)> dict) {

            var (email, displayname) = MapDictionaryItem(userId, dict);

            return MapUserSimpleResponse(userId, email, displayname);
        }

        #endregion

        #region Map User Simple Collection Response

        /// <summary>
        /// تبدیل دیکشنری ایمیل ها و نام های نمایشی
        /// به لیستی از مینی ریسپانس های کاربر
        /// </summary>
        public static IReadOnlyList<UserSimpleResponse> MapUserSimpleCollectionResponse
            (IReadOnlyDictionary<Guid, (string Email, string Displayname)> dict) {

            return dict.Select(item => MapUserSimpleResponse
                (item.Key,
                 item.Value.Email,
                 item.Value.Displayname))
                .ToList();
        }

        #endregion

        #region Map Email & Displaynames Dictionary Item

        /// <summary>
        /// مپ کردن آیتم مد نظر از دیکشنری ایمیل ها و
        /// نام های نمایشی کاربران بر اساس شناسه کاربری
        /// </summary>
        /// <param name="userId">شناسه کاربر هدف</param>
        /// <param name="dict">دیکشنری حاوی اطلاعات لازم</param>
        /// <returns>
        /// دو متغیر حاوی ایمیل و نام نمایشی(یا حاوی
        /// متن جایگزین در صورت نیافتن) بازخواهد گشت
        /// </returns>
        public static (string Email, string Displayname) MapDictionaryItem
            (Guid userId, IReadOnlyDictionary<Guid, (string Email, string Displayname)> dict) {

            var email = NotAvailable;
            var displayname = NotAvailable;

            if (dict.TryGetValue(userId, out var dictItem)) {

                email = dictItem.Email;
                displayname = dictItem.Displayname;
            }

            return (email, displayname);
        }

        #endregion
    }
}
