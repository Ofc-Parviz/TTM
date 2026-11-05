using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Primitives;

namespace TeamTaskManagement.Domain.ValueObjects {

    /// <summary>
    /// شناسنامه ی سبک عضو تیم - برای احراز دسترسی در عملیات ها
    /// در متدهای دامین بین اگریگیت ها منتقل میشود. توسط لایه اپلیکیشن
    /// از روی عضو تیم واقعی (که از اگریگیت تیم لود شده) ساخته میشود.
    /// عمداً هیچ ارجاعی به انتیتی عضو تیم ندارد تا مرز اگریگیت حفظ شود.
    /// </summary>
    public sealed class ActingMember : ValueObject {

        public ActingMember(Guid memberId, TeamMemberRole role) {
            MemberId = memberId;
            Role = role;
        }

        public Guid MemberId { get; }

        public TeamMemberRole Role { get; }

        public override IEnumerable<object> GetAtomicValues() {
            yield return MemberId;
            yield return Role;
        }
    }
}