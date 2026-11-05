using FluentAssertions;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Entities;
using static TeamTaskManagement.Domain.Tests.Entities.DomainTestHelper;

namespace TeamTaskManagement.Domain.Tests.Entities;

/// <summary>
/// تست های اگریگیت تیم
/// </summary>
public sealed class TeamTests {

    #region Create Team

    [Fact]
    public void Create_Adds_Leader_And_Raises_DomainEvents() {

        var leaderUserId = Guid.NewGuid();

        var result = Team.Create
            (leaderUserId,
             CreateName("تیم الف"),
             CreateTeamDescription("توضیحات تیم"),
             CreateName("مالک"));

        result.IsSuccess.Should().BeTrue();

        var team = result.Value;
        var leader = team.CurrentMembers
            .Should().ContainSingle().Which;

        leader.UserId.Should().Be(leaderUserId);
        leader.AccessLevel.Should().Be(TeamMemberRole.Leader);
        team.DomainEvents.Should().HaveCount(2);
    }

    #endregion

    #region Update Details

    [Fact]
    public void UpdateDetails_ByLeader_Succeeds() {

        var (team, leader) = CreateTeamWithLeader();

        var result = team.UpdateDetails
            (leader.Id,
             CreateName("نام جدید"),
             CreateTeamDescription("توضیح جدید"),
             CreateName("لیدر"));

        result.IsSuccess.Should().BeTrue();
        team.TeamDisplayname.Value.Should().Be("نام جدید");
    }

    [Fact]
    public void UpdateDetails_ByRegularMember_Fails() {

        var (team, leader) = CreateTeamWithLeader();

        var member = AddActiveMember(team, leader);

        var result = team.UpdateDetails
            (member.Id,
             CreateName("نام غیرمجاز"),
             CreateTeamDescription("توضیح غیرمجاز"),
             CreateName("عضو"));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Team.LeaderOnlyAction);
    }

    #endregion

    #region Send Invite

    [Fact]
    public void SendInvite_ByLeader_Succeeds() {

        var (team, leader) = CreateTeamWithLeader();

        var receiverUserId = Guid.NewGuid();

        var result = team.SendInvite
            (leader.Id,
             receiverUserId,
             TeamMemberRole.Member,
             CreateName("برنامه‌نویس"));

        result.IsSuccess.Should().BeTrue();
        team.Invites.Should().ContainSingle(invite => invite.ReceiverUserId == receiverUserId);
    }

    [Fact]
    public void SendInvite_ByRegularMember_Fails() {

        var (team, leader) = CreateTeamWithLeader();

        var member = AddActiveMember(team, leader);

        var result = team.SendInvite
            (member.Id,
             Guid.NewGuid(),
             TeamMemberRole.Member,
             CreateName("عضو جدید"));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Team.InsufficientPermission);
    }

    [Fact]
    public void SendInvite_ToSelf_Fails() {

        var (team, leader) = CreateTeamWithLeader();

        var result = team.SendInvite
            (leader.Id,
             leader.UserId,
             TeamMemberRole.Member,
             CreateName("عضو"));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Team.MemberCannotInviteItself);
    }

    [Fact]
    public void SendInvite_WithLeaderRole_Fails() {

        var (team, leader) = CreateTeamWithLeader();

        var result = team.SendInvite
            (leader.Id,
             Guid.NewGuid(),
             TeamMemberRole.Leader,
             CreateName("لیدر دوم"));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Team.TeamAlreadyHasLeader);
    }

    [Fact]
    public void SendInvite_WhenPendingInvitationAlreadyExists_Fails() {

        var (team, leader) = CreateTeamWithLeader();

        var receiverUserId = Guid.NewGuid();

        var firstResult = team.SendInvite
            (leader.Id,
             receiverUserId,
             TeamMemberRole.Member,
             CreateName("عضو"));

        firstResult.IsSuccess.Should().BeTrue();

        var secondResult = team.SendInvite
            (leader.Id,
             receiverUserId,
             TeamMemberRole.Member,
             CreateName("عضو دوم"));

        secondResult.IsFailure.Should().BeTrue();
        secondResult.Error.Should().Be(DomainErrors.Team.DuplicateInvite);
    }

    [Fact]
    public void Manager_Cannot_Invite_Manager() {

        var (team, leader) = CreateTeamWithLeader();

        var manager = AddActiveMember(team, leader, TeamMemberRole.Manager, "مدیر");

        var result = team.SendInvite
            (manager.Id,
             Guid.NewGuid(),
             TeamMemberRole.Manager,
             CreateName("مدیر دوم"));

        result.IsFailure.Should().BeTrue();

        result.Error.Should().Be(DomainErrors
            .Team.ManagerCannotInviteOthersInManagerRole);
    }

    #endregion

    #region Accept / Reject / Cancel Invite

    [Fact]
    public void AcceptInvite_Adds_ReceiverUser_AsActiveMember() {

        var (team, leader) = CreateTeamWithLeader();
        var receiverUserId = Guid.NewGuid();
        var invite = team.SendInvite
            (leader.Id,
             receiverUserId,
             TeamMemberRole.Member,
             CreateName("عضو")).Value;

        var result = team.AcceptInvite(invite.Id, receiverUserId);

        result.IsSuccess.Should().BeTrue();
        team.CurrentMembers.Should().Contain(member => member.UserId == receiverUserId);
        invite.Status.Should().Be(TeamInviteStatus.Accepted);
    }

    [Fact]
    public void AcceptInvite_ByDifferentUser_Fails() {

        var (team, leader) = CreateTeamWithLeader();
        var receiverUserId = Guid.NewGuid();
        var invite = team.SendInvite(
            leader.Id,
            receiverUserId,
            TeamMemberRole.Member,
            CreateName("عضو")).Value;

        var result = team.AcceptInvite(invite.Id, Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Team.InviteNotOwnedByUser);
    }

    [Fact]
    public void RejectInvite_Changes_InvitationStatus_Without_AddingMember() {

        var (team, leader) = CreateTeamWithLeader();
        var receiverUserId = Guid.NewGuid();
        var invite = team.SendInvite(
            leader.Id,
            receiverUserId,
            TeamMemberRole.Member,
            CreateName("عضو")).Value;

        var result = team.RejectInvite(invite.Id, receiverUserId);

        result.IsSuccess.Should().BeTrue();
        invite.Status.Should().Be(TeamInviteStatus.Rejected);
        team.CurrentMembers.Should().ContainSingle();
    }

    [Fact]
    public void CancelInvite_BySender_Succeeds() {

        var (team, leader) = CreateTeamWithLeader();

        var invite = team.SendInvite(
            leader.Id,
            Guid.NewGuid(),
            TeamMemberRole.Member,
            CreateName("عضو")).Value;

        var result = team.CancelInvite(invite.Id, leader.Id);

        result.IsSuccess.Should().BeTrue();
        invite.Status.Should().Be(TeamInviteStatus.Cancelled);
    }

    #endregion

    #region Remove / Leave / ChangeRole

    [Fact]
    public void RemoveMember_ByLeader_RemovesMember() {

        var (team, leader) = CreateTeamWithLeader();

        var member = AddActiveMember(team, leader);

        var result = team.RemoveMember(leader.Id, member.Id);

        result.IsSuccess.Should().BeTrue();
        team.CurrentMembers.Should().NotContain(candidate => candidate.Id == member.Id);
    }

    [Fact]
    public void RemoveLeader_Fails() {

        var (team, leader) = CreateTeamWithLeader();

        var result = team.RemoveMember(leader.Id, leader.Id);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Team.CannotRemoveLeader);
    }

    [Fact]
    public void LeaveTeam_ByRegularMember_RemovesMembership() {

        var (team, leader) = CreateTeamWithLeader();

        var member = AddActiveMember(team, leader);

        var result = team.LeaveTeam(member.Id);

        result.IsSuccess.Should().BeTrue();
        team.CurrentMembers.Should().NotContain(candidate => candidate.Id == member.Id);
    }

    [Fact]
    public void LeaveTeam_ByLeader_Fails() {

        var (team, leader) = CreateTeamWithLeader();

        var result = team.LeaveTeam(leader.Id);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Team.LeaderCannotLeaveTeam);
    }

    [Fact]
    public void ChangeMemberRole_ToManager_Succeeds() {

        var (team, leader) = CreateTeamWithLeader();
        var member = AddActiveMember(team, leader);

        var result = team.ChangeMemberRole(
            leader.Id,
            member.Id,
            TeamMemberRole.Manager,
            CreateName("مدیر"));

        result.IsSuccess.Should().BeTrue();
        member.AccessLevel.Should().Be(TeamMemberRole.Manager);
    }

    [Fact]
    public void ChangeMemberRole_ToLeader_Fails() {

        var (team, leader) = CreateTeamWithLeader();
        var member = AddActiveMember(team, leader);

        var result = team.ChangeMemberRole(
            leader.Id,
            member.Id,
            TeamMemberRole.Leader,
            CreateName("لیدر دوم"));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Team.TeamAlreadyHasLeader);
    }
    #endregion
}