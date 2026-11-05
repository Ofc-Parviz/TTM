using System.Linq.Expressions;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Entities;

namespace TeamTaskManagement.Domain.Specifications.ForTeamTask {
    /// <summary>
    /// اسپاسیفیکیشن برای قانون کسب وکار - اینجا تعریف میشه
    /// که منظور از «وظیفه ی باز» برای «عضو تیم» یعنی چی
    /// </summary>
    public sealed class OpenTasksForMemberSpecification : Specification<TeamTask> {

        private readonly Guid _memberId;

        /// <summary>
        /// شناسه عضو تیم اینجکت میشود
        /// </summary>
        public OpenTasksForMemberSpecification(Guid memberId) {
            _memberId = memberId;
        }

        public override Expression<Func<TeamTask, bool>> ToExpression() =>
            task =>
                (task.AppointedToMemberId == _memberId ||
                 task.CreatedByMemberId == _memberId) &&
                task.Status != TeamTaskStatus.ClosedAsComplete &&
                task.Status != TeamTaskStatus.ClosedAsFailure &&
                task.Status != TeamTaskStatus.ClosedAsDeadlineFailure;

        // وظیفه باز یعنی: هر وظیفه ای که وضعیتش شامل هیچکدام از 
        // موارد «بسته شده بعنوان شکست» - «بسته شده بعنوان شکست ددلاین» - «بسته شده بعنوان موفق» نیست 
        // عضوی که وظیفه باز دارد یعنی:
        // عضوی از تیم که «مسئول انجام» یا «محول کننده» حداقل یک «وظیفه باز» باشد
    }
}
