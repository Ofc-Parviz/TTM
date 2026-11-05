using FluentAssertions;
using TeamTaskManagement.IntegrationTests.Infrastructure;
using TeamTaskManagement.Web.Contracts.TeamTasks;
using Xunit;

namespace TeamTaskManagement.IntegrationTests.Flows;

public sealed class TaskRetryFlowTests
    : IClassFixture<WebAppFactory> {

    private readonly WebAppFactory _factory;

    public TaskRetryFlowTests
        (WebAppFactory factory) => _factory = factory;

    [Fact]
    public async Task AssigneeReportsFailure_LeaderOrdersRetry_AssigneeAcceptsIt() {

        using var leaderApi = new ApiCallsHelper(_factory.CreateClient());
        using var memberApi = new ApiCallsHelper(_factory.CreateClient());

        var leader = await leaderApi.RegisterAndLoginAsync(authenticateClient: true);
        var member = await memberApi.RegisterAndLoginAsync(authenticateClient: true);

        var membership = await TestScenarioHelper.CreateTeamWithMemberAsync(
            leaderApi,
            memberApi,
            leader.UserId,
            member.UserId);

        var createTaskResult = await leaderApi.CreateTaskAsync(new CreateTeamTaskRequest(
            membership.TeamId,
            membership.MemberMemberId,
            "وظیفه با امکان تلاش مجدد",
            "شرح وظیفه برای آزمون چرخهٔ تلاش مجدد",
            DateTime.UtcNow.AddDays(2)));

        createTaskResult.EnsureSuccessStatusCode();

        var myTasks = (await memberApi.GetMyTasksAsync()).GetRequiredContent();
        var task = myTasks.Should().ContainSingle().Which;

        (await memberApi.StartTaskAsync(task.TaskId)).EnsureSuccessStatusCode();
        (await memberApi.ClaimTaskFailureAsync(task.TaskId)).EnsureSuccessStatusCode();
        (await leaderApi.OrderTaskRetryAsync(task.TaskId)).EnsureSuccessStatusCode();
        (await memberApi.AcceptTaskRetryAsync(task.TaskId)).EnsureSuccessStatusCode();

        var taskAfterRetry = (await leaderApi.GetTaskByIdAsync(task.TaskId)).GetRequiredContent();
        taskAfterRetry.StatusText.Should().NotBeNullOrWhiteSpace();
    }
}
