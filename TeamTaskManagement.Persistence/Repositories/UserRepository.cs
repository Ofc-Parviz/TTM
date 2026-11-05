using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.Specifications.ForUser;
using TeamTaskManagement.Persistence.Specifications;
using TeamTaskManagement.Persistence.Specifications.ForUser;

namespace TeamTaskManagement.Persistence.Repositories {
    /// <summary>
    /// پیاده کننده قرارداد دسترسی به اطلاعات اگریگیت کاربر از دیتابیس
    /// </summary>
    public class UserRepository : IUserRepository {

        #region Context Injection

        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) => _context = context;

        #endregion

        #region Repository Implimentations

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cToken = default) {

            var querySpec = new UserByIdQuerySpecification(id);

            return await ApplyQuerySpecification(querySpec)
                .FirstOrDefaultAsync(cToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cToken = default) {

            var querySpec = new UserByEmailQuerySpecification(email);

            return await ApplyQuerySpecification(querySpec)
                .FirstOrDefaultAsync(cToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cToken = default) {

            var domainSpec = new UserByEmailSpecification(email);

            return await _context.Set<User>()
                .AnyAsync(domainSpec.ToExpression(), cToken);
        }

        public async Task InsertAsync(User user, CancellationToken cToken = default) {

            await _context.Set<User>().AddAsync(user, cToken);
        }

        public Task UpdateAsync(User user, CancellationToken cToken = default) {

            // برای معرفی تغییرات کاربر به کانتکست، از
            // منطق مشترک کلاس ریپوزیتوری-هلپر استفاده میکنیم
            return RepositoryHelper.UpdateEntityAsync(_context, user);
        }

        public async Task<IReadOnlyDictionary<Guid, string>> GetEmailsByUserIdsAsync
            (IEnumerable<Guid> userIds,
             CancellationToken ct = default) {

            // تبدیل شناسه های دریافت شده به لیست یکتا
            var ids = userIds?.Distinct().ToList() ?? new List<Guid>();

            // اگر لیست شناسه ها خالی بود، دیکشنری خالی برمیگرده
            if(ids.Count is 0)
                return new Dictionary<Guid, string>().AsReadOnly();

            // کوئری یکباره برای جلوگیری از مشکل N + 1
            var results = await _context.Set<User>()
                // از کاربر هایی که شناسه شون توی لیست ورودیه
                .Where(user => ids.Contains(user.Id))
                // ایمیل و شناسه استخراج بشه
                .Select(user => new {
                    UserId = user.Id,
                    Email = user.Email.Value,
                })
                // اجرای کوئری و متریالایز کردن
                .ToListAsync(ct);
            // یادم باشه بعدا برای این کوئری اسپاسیفیکیشن بنویسم
            // # Check Later

            // تبدیل نتایج به دیکشنری فقط خوندنی
            return results.ToDictionary(key => key.UserId, value => value.Email).AsReadOnly();
        }

        public async Task<IReadOnlyDictionary<Guid, (string Email, string Displayname)>>
            GetEmailsAndDisplaynamesByUserIdsAsync
            (IEnumerable<Guid> userIds, CancellationToken ct = default) {


            // تبدیل شناسه های دریافت شده به هش ست یکتا
            var ids = userIds?.ToHashSet() ?? new HashSet<Guid>();

            // اگر لیست شناسه ها خالی بود، دیکشنری خالی برمیگرده
            if (ids.Count is 0)
                return new Dictionary<Guid, (string, string)>().AsReadOnly();

            // کوئری یکباره برای جلوگیری از مشکل N + 1
            var results = await _context.Set<User>()
                // از کاربر هایی که شناسه شون توی لیست ورودیه
                .Where(user => ids.Contains(user.Id))
                // ایمیل و شناسه و نام نمایشی استخراج بشه
                .Select(user => new {
                    UserId = user.Id,
                    Email = user.Email.Value,
                    Displayname = user.Displayname.Value
                })
                // اجرای کوئری و متریالایز کردن
                .ToListAsync(ct);
            // یادم باشه بعدا برای این کوئری اسپاسیفیکیشن بنویسم
            // # Check Later

            // تبدیل نتایج به دیکشنری فقط خوندنی
            return results.ToDictionary
                (key => key.UserId,
                 value => (value.Email,
                           value.Displayname)).AsReadOnly();
        }

        public async Task<bool> AnyByIdAsync(Guid id, CancellationToken cToken = default) {

            var querySpec = new UserByIdQuerySpecification(id);

            return await ApplyQuerySpecification(querySpec).AnyAsync(cToken);
        }

        #endregion

        #region Internal-Shared Methods

        /// <summary>
        /// متد اعمال کننده کوئری-اسپاسیفیکیشن
        /// </summary>
        private IQueryable<User> ApplyQuerySpecification(QuerySpecification<User>
            querySpec) => SpecificationEvaluator.GetQuery(_context.Set<User>(), querySpec);

        #endregion
    }
}
