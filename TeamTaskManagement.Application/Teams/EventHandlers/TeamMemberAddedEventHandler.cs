using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Application.Enums;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.EventHandlers {
    /// <summary>
    /// هندلر رویداد: عضو جدید به تیم اضافه شد
    /// </summary>
    internal sealed class TeamMemberAddedEventHandler
        : IDomainEventHandler<TeamMemberAddedEvent> {

        #region Injections

        private readonly ILogger<TeamMemberAddedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;

        public TeamMemberAddedEventHandler(
            ILogger<TeamMemberAddedEventHandler> logger,
            IEmailService emailService,
            IUserRepository userRepository,
            ITeamRepository teamRepository) {

            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }

        #endregion

        public async Task Handle(TeamMemberAddedEvent notification, CancellationToken ct) {

            var team = await _teamRepository
                .GetByIdAsync(notification.TeamId, ct);

            if (team is null) {
                _logger.LogWarning("TeamMemberAddedEvent:" +
                    " Team {TeamId} not found", notification.TeamId);
                return;
            }

            var affectedUser = await _userRepository
                .GetByIdAsync(notification.AddedMemberUserId, ct);

            if (affectedUser is null) {
                _logger.LogWarning("TeamMemberAddedEvent:" +
                    " User {UserId} not found", notification.AddedMemberUserId);
                return;
            }


            // این کد برای نسخه اولیه این هندلر
            // بود که مشکل ان-یک کوئری ایجاد میکرد
            // کامنتش کردم داخل این ریجن که تفاوتش
            // با کد درست مشخص باشه
            #region N + 1 Problem Commented Code

            // کد مشکل دار برای جمع آوری ایمیل
            // اعضای فعال تیم به جز خود عضو جدید
            //var memberEmails = new List<string>();

            //var affectedMemberRoleTitle = string.Empty;

            //foreach (var member in team.CurrentMembers) {

            //    if (member.UserId == notification.AddedMemberUserId) {
            //        affectedMemberRoleTitle = member.RoleDisplayname.Value;
            //        continue;
            //    }

            //    var user = await _userRepository
            //        .GetByIdAsync(member.UserId, ct);

            //    if (user is not null)
            //        memberEmails.Add(user.Email.Value);
            //}

            #endregion


            // کد درست بدون مشکل ان-یک :
            #region N + 1 Fixed Active Code

            // برداشتن شناسه کاربری اعضای فعال - بجز عضو جدید
            var userIds = team.CurrentMembers
                .Where(x => x.Id != notification.AddedMemberId)
                .Select(x => x.UserId)
                .ToList();

            // دریافت دیکشنری از ایمیل های دیگران در یک کوئری واحد
            // اینجوری میشه مشکل ان-یک کوئری رو حل کرد
            var emailsDics = await _userRepository.GetEmailsByUserIdsAsync(userIds, ct);

            // تبدیل دیکشنری به آرایه ای از ایمیل ها
            var memberEmails = emailsDics.Values.ToArray();

            // متوقف کردن عملیات در صورت خالی بودن آرایه
            if (memberEmails.Length is 0) return;

            #endregion


            // دریافت نام نقشی که عضو جدید در تیم به عهده گرفته
            var affectedMemberRoleTitle = team.CurrentMembers
                .FirstOrDefault(x => x.Id == notification.AddedMemberId)?
                .RoleDisplayname.Value ?? string.Empty;


            // اجرای سرویس ایمیل
            var emailResult = await _emailService
                .SendMembershipUpdateEmailToTeamMembersAsync
                (memberEmails,
                 team.TeamDisplayname.Value,
                 affectedUser.Displayname.Value,
                 affectedMemberRoleTitle,
                 MembershipUpdateEmailType.Added);

            if (emailResult.IsFailure) {

                // لاگ در صورت شکست در فرستادن ایمیل

                var eventHandlerName = nameof(TeamMemberAddedEventHandler);

                _logger.LogError(
                    "Failed to send {EventName} emails for Team '{TeamId}'. Error: '{Error}'.",
                    eventHandlerName,
                    notification.TeamId,
                    emailResult.Error.Message);
            }
        }
    }
}
