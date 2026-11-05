using TeamTaskManagement.Domain.Enums;

namespace TeamTaskManagement.Domain.ReadModels {

    /// <summary>
    /// مدل خواندنی دعوتنامه برای کوئری‌ها
    /// </summary>
    public sealed class InviteReadModel {

        public Guid InviteId { get; init; }
        public Guid TeamId { get; init; }
        public string TeamDisplayname { get; init; } = string.Empty;

        public TeamInviteStatus Status { get; init; }
        public string SuggestedRoleTitle { get; init; } = string.Empty;
        public TeamMemberRole SuggestedAccessLevel { get; init; }

        public DateTime CreatedAtUtc { get; init; }
        public DateTime LastModifiedAtUtc { get; init; }

        // Sender
        public Guid SenderMemberId { get; init; }
        public Guid SenderUserId { get; init; }
        public string SenderRoleTitle { get; init; } = string.Empty;
        public TeamMemberRole SenderAccessLevel { get; init; }
        public TeamMemberStatus SenderMembershipStatus { get; init; }
        public DateTime SenderJoinedAtUtc { get; init; }
        public DateTime SenderLastStatusChangeAt { get; init; }

        // Receiver
        public Guid ReceiverUserId { get; init; }
    }
}