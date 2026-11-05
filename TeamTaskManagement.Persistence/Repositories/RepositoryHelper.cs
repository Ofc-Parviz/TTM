using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.Primitives;

namespace TeamTaskManagement.Persistence.Repositories {
    /// <summary>
    /// کلاس کمکی ریپوزیتوری ها - حاوی 
    /// منطق مشترک برای جلوگیری از تکرار کد
    /// </summary>
    internal static class RepositoryHelper {

        /// <summary>
        /// معرفی تغییرات انتیتی به ای-اف-کور به روش پایدار
        /// با منطق مشترک برای جلوگیری از تکرار کد نویسی
        /// </summary>
        /// <typeparam name="TEntity">نوع انتیتی</typeparam>
        /// <param name="context">کانتکست دیتابیس</param>
        /// <param name="entity">شی انتیتی ورودی</param>
        public static Task UpdateEntityAsync
            <TEntity>(ApplicationDbContext context,
             TEntity entity) where TEntity: BaseEntity {

            // دریافت ردیاب تغییر(چنج ترکر) ای-اف-کور
            // برای شی انتیتی وارد شده
            var entry = context.Entry(entity);

            // بررسی اینکه ای-اف-کور از قبل این انتیتی
            // و تغییراتش رو ردیابی کرده یانه
            if (entry.State == EntityState.Detached)
                // اگر نکرده بود، انتیتی رو اتچ میکنیم
                context.Set<TEntity>().Attach(entity);

            // انتیتی ورودی رو بعنوان ویرایش شده معرفی میکنیم
            entry.State = EntityState.Modified;

            // وظیفه اعمال تغییرات فیزیکی به
            // دیتابیس به عهده یونیت آف ورکه
            // اینجا فقط نتیجه موفق برمیگردونیم
            return Task.CompletedTask;
        }
    }
}
