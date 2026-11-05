using FluentAssertions;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Shared;
using static TeamTaskManagement.Domain.Tests.Entities.DomainTestHelper;

namespace TeamTaskManagement.Domain.Tests.Entities;

/// <summary>
/// تست های اگریگیت وظیفه تیم
/// </summary>
public sealed class TeamTaskTests {

    #region Arrangements

    /// <summary>
    /// سناریوی وظیفه باز - ایجاد تیمی که مالک 
    /// و یه عضو معمولی و یه وظیفه باز داره
    /// </summary>
    private static (Team Team, TeamMember Leader,
        TeamMember Member, TeamTask Task) CreateTaskScenario() {

        var (team, leader, member) = CreateTeamWithMember();

        var task = CreateOpenTask(team, leader, member);

        return (team, leader, member, task);
    }

    /// <summary>
    /// سناریوی وظیفه ای که مسئول شروع کرده، سپس 
    /// اعلام شکست کرده و بعد مدیر وظیفه دستور تلاش مجدد داده
    /// </summary>
    private static TeamTask CreateTaskAwaitingRetry
        (Team team,
         TeamMember leader,
         TeamMember member) {

        var task = CreateOpenTask(team, leader, member);

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();
        task.ClaimFailure(member.AsActor()).IsSuccess.Should().BeTrue();
        task.OrderRetry(leader.AsActor()).IsSuccess.Should().BeTrue();

        return task;
    }

    /// <summary>
    /// سناریوی وظیفه ای که مسئول شروع کرده، اعلام 
    /// تکمیل کرده، و مدیر وظیفه با تایید بعنوان موفق بستتش
    /// </summary>
    private static TeamTask CreateCompletedTask
        (Team team,
         TeamMember leader,
         TeamMember member) {

        var task = CreateOpenTask(team, leader, member);

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();
        task.ClaimComplete(member.AsActor()).IsSuccess.Should().BeTrue();
        task.CloseAsComplete(leader.AsActor()).IsSuccess.Should().BeTrue();

        return task;
    }

    #endregion

    #region Create

    [Fact]
    public void Create_Should_Initialize_Open_Task_Correctly() {

        var (team, leader, member, task) = CreateTaskScenario();

        task.TeamId.Should().Be(team.Id);
        task.CreatedByMemberId.Should().Be(leader.Id);
        task.AppointedToMemberId.Should().Be(member.Id);
        task.Title.Value.Should().Be("وظیفه تست");
        task.Status.ItsOpenTaskStatus().Should().BeTrue();
    }

    #endregion

    #region Update Details / Deadline

    [Fact]
    public void UpdateDetails_ByCreator_Succeeds() {

        var (_, leader, _, task) = CreateTaskScenario();

        var result = task.UpdateDetails
            (actor: leader.AsActor(),
             CreateTaskTitle("عنوان جدید"),
             CreateTaskDescription("توضیح جدید"));

        result.IsSuccess.Should().BeTrue();
        task.Title.Value.Should().Be("عنوان جدید");
        task.Description.Value.Should().Be("توضیح جدید");
    }

    [Fact]
    public void UpdateDeadline_Changes_Deadline() {

        var (_, leader, _, task) = CreateTaskScenario();
        var deadline = DateTime.UtcNow.AddDays(3);

        var result = task.UpdateDeadline(leader.AsActor(), deadline);

        result.IsSuccess.Should().BeTrue();
        task.DeadlineUtc.Should().Be(deadline);
    }

    #endregion

    #region Rotate

    [Fact]
    public void RotateAssignee_ChangesAppointedMember() {

        var (team, leader, member, task) = CreateTaskScenario();

        var anotherMember = AddActiveMember(team, leader, title: "عضو دوم");

        var result = task.RotateAssignee
            (actor: leader.AsActor(),
             newAssignee: anotherMember.AsActor());

        result.IsSuccess.Should().BeTrue();
        task.AppointedToMemberId.Should().Be(anotherMember.Id);
    }

    [Fact]
    public void RotateCreator_ChangesCreatedByMember() {

        var (_, leader, member, task) = CreateTaskScenario();

        var result = task.RotateCreator
            (actor: leader.AsActor(),
             newCreator: member.AsActor());

        result.IsSuccess.Should().BeTrue();
        task.CreatedByMemberId.Should().Be(member.Id);
    }

    #endregion

    #region Status Lifecycle

    [Fact]
    public void Start_ByAssignee_Succeeds() {

        var (_, _, member, task) = CreateTaskScenario();

        var result = task.Start(member.AsActor());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ClaimComplete_ByAssignee_AfterStart_Succeeds() {

        var (_, _, member, task) = CreateTaskScenario();

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.ClaimComplete(member.AsActor());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ClaimFailure_ByAssignee_AfterStart_Succeeds() {

        var (_, _, member, task) = CreateTaskScenario();
        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.ClaimFailure(member.AsActor());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void OrderRetry_ByCreator_AfterFailure_Succeeds() {

        var (_, leader, member, task) = CreateTaskScenario();

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        task.ClaimFailure(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.OrderRetry(leader.AsActor());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void AcceptRetry_ByAssignee_Succeeds() {

        var (team, leader, member) = CreateTeamWithMember();

        var task = CreateTaskAwaitingRetry(team, leader, member);

        var result = task.AcceptRetry(member.AsActor());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void RejectRetry_ByAssignee_Succeeds() {

        var (team, leader, member) = CreateTeamWithMember();

        var task = CreateTaskAwaitingRetry(team, leader, member);

        var result = task.RejectRetry(member.AsActor());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CloseAsComplete_ByCreator_AfterClaimComplete_Succeeds() {

        var (_, leader, member, task) = CreateTaskScenario();

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        task.ClaimComplete(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.CloseAsComplete(leader.AsActor());

        result.IsSuccess.Should().BeTrue();

        task.Status.ItsClosedTaskStatus().Should().BeTrue();
    }

    [Fact]
    public void CloseAsFailure_ByCreator_AfterClaimFailure_Succeeds() {

        var (_, leader, member, task) = CreateTaskScenario();

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        task.ClaimFailure(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.CloseAsFailure(leader.AsActor());

        result.IsSuccess.Should().BeTrue();
        task.Status.ItsClosedTaskStatus().Should().BeTrue();
    }

    #endregion

    #region Negative Permission / Invalid Transition

    [Fact]
    public void Start_ByNonAssignee_Fails() {

        var (_, leader, _, task) = CreateTaskScenario();

        var result = task.Start(leader.AsActor());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ClaimComplete_BeforeStart_Fails() {

        var (_, _, member, task) = CreateTaskScenario();

        var result = task.ClaimComplete(member.AsActor());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ClosedTask_RejectsDetailsUpdate() {

        var (team, leader, member) = CreateTeamWithMember();

        var task = CreateCompletedTask(team, leader, member);

        var result = task.UpdateDetails
            (leader.AsActor(),
             CreateTaskTitle("عنوان پس از بستن"),
             CreateTaskDescription("تغییر غیرمجاز"));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void CloseAsComplete_ByAssignee_Fails() {

        var (_, _, member, task) = CreateTaskScenario();

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        task.ClaimComplete(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.CloseAsComplete(member.AsActor());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void OrderRetry_BeforeClaimFailure_Fails() {

        var (_, leader, member, task) = CreateTaskScenario();

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.OrderRetry(leader.AsActor());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AcceptRetry_BeforeRetryIsOrdered_Fails() {

        var (_, _, member, task) = CreateTaskScenario();

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        task.ClaimFailure(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.AcceptRetry(member.AsActor());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void RotateAssignee_OnClosedTask_Fails() {

        var (team, leader, member) = CreateTeamWithMember();

        var anotherMember = AddActiveMember(team, leader, title: "عضو دوم");

        var task = CreateCompletedTask(team, leader, member);

        var result = task.RotateAssignee(leader.AsActor(), anotherMember.AsActor());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateDeadline_OnClosedTask_Fails() {

        var (team, leader, member) = CreateTeamWithMember();

        var task = CreateCompletedTask(team, leader, member);

        var result = task.UpdateDeadline(leader.AsActor(), DateTime.UtcNow.AddDays(1));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ClaimComplete_ByCreator_Fails() {

        var (_, leader, member, task) = CreateTaskScenario();

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.ClaimComplete(leader.AsActor());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Start_Twice_Fails() {

        var (_, _, member, task) = CreateTaskScenario();

        task.Start(member.AsActor()).IsSuccess.Should().BeTrue();

        var result = task.Start(member.AsActor());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void RejectRetry_ByCreator_Fails() {

        var (team, leader, member) = CreateTeamWithMember();

        var task = CreateTaskAwaitingRetry(team, leader, member);

        var result = task.RejectRetry(leader.AsActor());

        result.IsFailure.Should().BeTrue();
    }

    #endregion
}