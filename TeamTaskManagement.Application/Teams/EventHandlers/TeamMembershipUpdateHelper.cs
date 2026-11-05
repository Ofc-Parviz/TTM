using TeamTaskManagement.Application.Enums;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;

namespace TeamTaskManagement.Application.Teams.EventHandlers {
    internal class TeamMembershipUpdateHelper {

        #region Injections

        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;

        public TeamMembershipUpdateHelper
            (IEmailService emailService,
             IUserRepository userRepository,
             ITeamRepository teamRepository) {

            _emailService = emailService;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }

        #endregion

        // این کلاس رو برای تجمیع منطق مشترک بین هندلر های 
        // رویداد های اعضای تیم ایجاد کردم. یادم باشه بعدا 
        // بهش سر بزنم و تکمیلش کنم
        // # Check Later

        private Task HandleMembershipUpdate
            (Guid teamId,
             Guid userId,
             string RoleTitle,
             string TeamDisplayname,
             MembershipUpdateEmailType membershipUpdateType) {

            // # Check Later
            return Task.CompletedTask;
        }
    }
}
