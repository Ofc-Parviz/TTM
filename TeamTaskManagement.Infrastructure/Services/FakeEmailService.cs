using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Application.Enums;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Shared.Classes;
using Microsoft.AspNetCore.Hosting;

namespace TeamTaskManagement.Infrastructure.Services
{
    /// <summary>
    /// سرویس ایمیل فیک، برای خالی نبودن عریضه
    /// </summary>
    internal sealed class FakeEmailService : IEmailService {

        public Task<Result> SendDeadlineChangeEmailAsync
            (string receiverEmail,
             string receiverDisplayname,
             string taskTitle,
             string teamDisplayname,
             string changedByDisplayname,
             DateTime? newDeadlineUtc,
             CancellationToken ct = default) => Task.FromResult(Result.Success());

        public Task<Result> SendDetailsUpdatedEmailAsync
            (string receiverEmail,
             string receiverDisplayname,
             string teamDisplayname,
             string changedByDisplayname,
             string newTeamTaskTitle,
             string newTeamTaskDescription,
             CancellationToken ct = default) => Task.FromResult(Result.Success());

        public Task<Result> SendEmailToInvitedUserAsync
            (string receiverEmail,
             string receiverDisplayname,
             string suggestedRoleTitle,
             string senderDisplayname,
             string hostTeamDisplayname,
             CancellationToken ct = default) => Task.FromResult(Result.Success());

        public Task<Result> SendMembershipUpdateEmailToTeamMembersAsync
            (string[] memberEmails,
             string teamDisplayname,
             string affectedMemberDisplayname,
             string affectedMemberRoleTitle,
             MembershipUpdateEmailType membershipUpdateType,
             CancellationToken ct = default) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskAssignedToMemberEmailAsync
            (string receiverEmail,
             string receiverDisplayname,
             string taskTitle,
             string teamDisplayname,
             string creatorDisplayname,
             DateTime? deadlineUtc) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskAssigneeAssignedEmailAsync
            (string receiverEmail,
             string receiverDisplayname,
             string taskTitle,
             string teamDisplayname,
             string rotatedByDisplayname) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskAssigneeRemovedEmailAsync(string receiverEmail, string receiverDisplayname, string taskTitle, string teamDisplayname, string newAssigneeDisplayname) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskAssigneeRotatedToLeaderEmailAsync(string leaderEmail, string leaderDisplayname, string taskTitle, string teamDisplayname, string oldAssigneeDisplayname, string newAssigneeDisplayname, string rotatedByDisplayname) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskCreatedNotificationToLeaderEmailAsync(string leaderEmail, string leaderDisplayname, string taskTitle, string teamDisplayname, string creatorDisplayname, string assigneeDisplayname, DateTime? deadlineUtc) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskCreatorAssignedEmailAsync(string receiverEmail, string receiverDisplayname, string taskTitle, string teamDisplayname, string rotatedByDisplayname) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskCreatorChangedToAssigneeEmailAsync(string receiverEmail, string receiverDisplayname, string taskTitle, string teamDisplayname, string newCreatorDisplayname, bool newCreatorIsAssigneeByItself) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskCreatorRemovedEmailAsync(string receiverEmail, string receiverDisplayname, string taskTitle, string teamDisplayname, string newCreatorDisplayname) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskStatusChangedToAssigneeEmailAsync(string receiverEmail, string receiverDisplayname, string taskTitle, string teamDisplayname, TeamTaskStatus newStatus, string changedByDisplayname) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskStatusChangedToCreatorEmailAsync(string receiverEmail, string receiverDisplayname, string taskTitle, string teamDisplayname, TeamTaskStatus newStatus, string changedByDisplayname) => Task.FromResult(Result.Success());

        public Task<Result> SendTaskStatusChangedToLeaderEmailAsync(string leaderEmail, string leaderDisplayname, string taskTitle, string teamDisplayname, TeamTaskStatus newStatus, string changedByDisplayname, string assigneeDisplayname) => Task.FromResult(Result.Success());

        public Task<Result> SendTeamCreationEmailAsync(string receiverEmail, string receiverDisplayname, string createdTeamDisplayname, CancellationToken ct = default) {
            return Task.FromResult(Result.Success());
        }

        public Task<Result> SendUserRegistrationWelcomeEmailAsync
            (string email,
             string userDisplayname,
             string userFirstname,
             string userLastname,
             CancellationToken ct = default)
            => Task.FromResult(Result.Success());
    }
}
