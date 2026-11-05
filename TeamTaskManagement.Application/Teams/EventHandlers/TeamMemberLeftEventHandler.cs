using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Application.Enums;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.EventHandlers {
    /// <summary>
    /// هندلر رویداد: عضو تیم را ترک کرد
    /// </summary>
    internal sealed class TeamMemberLeftEventHandler
        : IDomainEventHandler<TeamMemberLeftEvent> {

        #region Injections

        private readonly ILogger<TeamMemberLeftEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;

        public TeamMemberLeftEventHandler(
            ILogger<TeamMemberLeftEventHandler> logger,
            IEmailService emailService,
            IUserRepository userRepository,
            ITeamRepository teamRepository) {

            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }

        #endregion

        public async Task Handle(TeamMemberLeftEvent notification, CancellationToken ct) {

            var team = await _teamRepository
                .GetByIdAsync(notification.TeamId, ct);

            if (team is null) {
                _logger.LogWarning("TeamMemberLeftEvent:" +
                    " Team {TeamId} not found", notification.TeamId);
                return;
            }

            var affectedUser = await _userRepository
                .GetByIdAsync(notification.LeftMemberUserId, ct);

            if (affectedUser is null) {
                _logger.LogWarning("TeamMemberLeftEvent:" +
                    " User {UserId} not found", notification.LeftMemberUserId);
                return;
            }


            // این کد برای نسخه اولیه این هندلر
            // بود که مشکل ان-یک کوئری ایجاد میکرد
            // کامنتش کردم داخل این ریجن که تفاوتش
            // با کد درست مشخص باشه
            #region N + 1 Problem Commented Code

            //var memberEmails = new List<string>();

            //foreach (var member in team.CurrentMembers) {

            //    var user = await _userRepository
            //        .GetByIdAsync(member.UserId, ct);

            //    if (user is not null)
            //        memberEmails.Add(user.Email.Value);
            //}

            #endregion


            // کد درست بدون مشکل ان-یک :
            #region N + 1 Fixed Active Code

            // دریافت لیست شناسه کاربری دیگر اعضای تیم
            var userIds = team
                .CurrentMembers
                .Select(x => x.UserId)
                .ToList();
            // نیازی به فیلتر کردن نیست چون عضوی که تیم رو
            // ترک کرده خود به خود توی لیست اعضای فعال نمیاد

            // دریافت دیکشنری ایمیل های اعضا
            // کوئری یکجا اجرا شونده بدون مشکل ان-یک
            var emailsDict = await _userRepository
                .GetEmailsByUserIdsAsync(userIds, ct);

            // تبدیل دیکشنری به آرایه ای از ایمیل ها
            var memberEmails = emailsDict.Values.ToArray();

            // متوقف کردن عملیات در صورت خالی بودن آرایه
            if (memberEmails.Length is 0) return;

            #endregion


            // اجرای سرویس ایمیل
            var emailResult = await _emailService
                .SendMembershipUpdateEmailToTeamMembersAsync
                (memberEmails,
                 team.TeamDisplayname.Value,
                 affectedUser.Displayname.Value,
                 string.Empty, // چون عضو دیگر در تیم نیست، نقشش مهم نیست
                 MembershipUpdateEmailType.Left);

            if (emailResult.IsFailure) {

                // لاگ در صورت شکست در ارسال ایمیل

                var eventHandlerName = nameof(TeamMemberLeftEventHandler);

                _logger.LogError(
                    "Failed to send {EventName} emails for Team {TeamId}. Error: {Error}",
                    eventHandlerName,
                    notification.TeamId,
                    emailResult.Error.Message);
            }
        }
    }
}