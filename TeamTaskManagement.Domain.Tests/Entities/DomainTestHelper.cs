using FluentAssertions;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Domain.Tests.Entities;

/// <summary>
/// داده‌ ساز های مشترک برای آماده سازی سناریو های تست دامین
/// </summary>
internal static class DomainTestHelper {

    public static DisplayName CreateName(string value)
        => DisplayName.Create(value).Value;

    public static TeamDescription CreateTeamDescription(string value)
        => TeamDescription.Create(value).Value;

    public static TaskTitle CreateTaskTitle(string value)
        => TaskTitle.Create(value).Value;

    public static TaskDescription CreateTaskDescription(string value)
        => TaskDescription.Create(value).Value;

    /// <summary>
    /// تیم + لیدر
    /// </summary>
    public static (Team Team, TeamMember Leader) CreateTeamWithLeader
        (Guid? leaderUserId = null,
         string teamName = "تیم تست",
         string leaderTitle = "لیدر") {

        var result = Team.Create
            (leaderUserId ?? Guid.NewGuid(),
             CreateName(teamName),
             CreateTeamDescription("توضیحات تیم"),
             CreateName(leaderTitle));

        result.IsSuccess.Should().BeTrue
            (result.IsFailure ? result.Error
            .Message : "ساخت تیم باید موفق باشد.");

        return (result.Value, result.Value.CurrentMembers.Single());
    }

    /// <summary>
    /// عضو را از مسیر واقعی دامنه، یعنی ارسال و پذیرش دعوت‌نامه، به تیم اضافه می‌کند
    /// </summary>
    public static TeamMember AddActiveMember
        (Team team,
         TeamMember inviter,
         TeamMemberRole role = TeamMemberRole.Member,
         string title = "عضو") {

        var userId = Guid.NewGuid();

        var inviteResult = team.SendInvite
            (inviter.Id,
             userId,
             role,
             CreateName(title));

        inviteResult.IsSuccess.Should().BeTrue();

        var acceptResult = team.AcceptInvite(inviteResult.Value.Id, userId);
        acceptResult.IsSuccess.Should().BeTrue();

        return acceptResult.Value;
    }

    /// <summary>
    /// تیم + لیدر + یک عضو عادی
    /// </summary>
    public static (Team Team, TeamMember Leader, TeamMember Member) CreateTeamWithMember() {

        var (team, leader) = CreateTeamWithLeader();
        var member = AddActiveMember(team, leader);
        return (team, leader, member);
    }

    /// <summary>
    /// وظیفه ای باز ایجاد میکند بدون تغییر وضعیت اولیه
    /// </summary>
    public static TeamTask CreateOpenTask
        (Team team,
         TeamMember creator,
         TeamMember assignee,
         string title = "وظیفه تست",
         DateTime? deadlineUtc = null) {

        var result = TeamTask.Create
            (team.Id,
             creator.AsActor(),
             assignee.AsActor(),
             CreateTaskTitle(title),
             CreateTaskDescription("توضیحات وظیفه تست"),
             deadlineUtc);

        result.IsSuccess.Should().BeTrue
            ( result.IsFailure
            ? result.Error.Message
            : "ساخت وظیفه باید موفق باشد.");

        return result.Value;
    }
}
