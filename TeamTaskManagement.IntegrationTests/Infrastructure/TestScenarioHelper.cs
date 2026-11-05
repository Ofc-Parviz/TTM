using FluentAssertions;
using TeamTaskManagement.Shared.Enums;
using TeamTaskManagement.Web.Contracts.Teams;
using TeamTaskManagement.Web.Contracts.TeamTasks;

namespace TeamTaskManagement.IntegrationTests.Infrastructure;

/// <summary>
/// سناریوهای پرتکرار آماده‌سازی داده برای تست‌های یکپارچگی.
/// </summary>
public static class TestScenarioHelper {

    public static async Task<Guid> CreateTeamAsync(
        ApiCallsHelper leaderApi,
        string? teamDisplayname = null) {

        teamDisplayname ??= $"تیم_{Guid.NewGuid():N}"[..20];

        var createResult = await leaderApi.CreateTeamAsync(new CreateTeamRequest(
            "لیدر",
            teamDisplayname,
            "توضیحات تیم تست"));

        createResult.EnsureSuccessStatusCode();

        var teams = (await leaderApi.GetMyTeamsAsync()).GetRequiredContent();
        var createdTeam = teams.SingleOrDefault(team => team.TeamDisplayname == teamDisplayname);

        return createdTeam?.TeamId
            ?? throw new InvalidOperationException("تیم ایجادشده در فهرست تیم‌های کاربر پیدا نشد.");
    }

    public static async Task<Guid> CreatePendingInviteAsync(
        ApiCallsHelper leaderApi,
        ApiCallsHelper receiverApi,
        Guid teamId,
        Guid receiverUserId,
        string suggestedRoleTitle = "عضو") {

        var sendResult = await leaderApi.SendInviteAsync(new SendTeamInviteRequest(
            teamId,
            receiverUserId.ToString(),
            suggestedRoleTitle,
            TeamMemberRoleInput.Member));

        sendResult.EnsureSuccessStatusCode();

        var receivedInvites = (await receiverApi.GetMyInvitesAsync(
            directionFilter: "Received",
            statusFilter: "Pending",
            teamId: teamId)).GetRequiredContent();

        var invite = receivedInvites.SingleOrDefault(item => item.TeamId == teamId);

        return invite?.InviteId
            ?? throw new InvalidOperationException("دعوت‌نامهٔ موردانتظار در فهرست دعوت‌نامه‌های دریافتی پیدا نشد.");
    }

    public static async Task<Guid> AcceptInviteAsync(
        ApiCallsHelper leaderApi,
        ApiCallsHelper receiverApi,
        Guid teamId,
        Guid receiverUserId,
        string suggestedRoleTitle = "عضو") {

        var inviteId = await CreatePendingInviteAsync(
            leaderApi,
            receiverApi,
            teamId,
            receiverUserId,
            suggestedRoleTitle);

        var acceptResult = await receiverApi.AcceptInviteAsync(
            new AcceptInviteRequest(teamId, inviteId));

        acceptResult.EnsureSuccessStatusCode();
        return inviteId;
    }

    public static async Task<TeamMembershipData> CreateTeamWithMemberAsync(
        ApiCallsHelper leaderApi,
        ApiCallsHelper memberApi,
        Guid leaderUserId,
        Guid memberUserId) {

        var teamId = await CreateTeamAsync(leaderApi);

        await AcceptInviteAsync(
            leaderApi,
            memberApi,
            teamId,
            memberUserId,
            "مسئول انجام وظیفه");

        var teamDetails = (await leaderApi.GetTeamByIdAsync(teamId)).GetRequiredContent();
        var leaderMember = teamDetails.Members.SingleOrDefault(member => member.UserId == leaderUserId);
        var activeMember = teamDetails.Members.SingleOrDefault(member => member.UserId == memberUserId);

        leaderMember.Should().NotBeNull("عضو لیدر باید در جزئیات تیم حضور داشته باشد");
        activeMember.Should().NotBeNull("عضوی که دعوت‌نامه را پذیرفته باید در تیم حضور داشته باشد");

        return new TeamMembershipData(teamId, leaderMember!.MemberId, activeMember!.MemberId);
    }
}

public sealed record TeamMembershipData(
    Guid TeamId,
    Guid LeaderMemberId,
    Guid MemberMemberId);
