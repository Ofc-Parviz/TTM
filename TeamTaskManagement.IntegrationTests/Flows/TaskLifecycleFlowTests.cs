using FluentAssertions;
using TeamTaskManagement.IntegrationTests.Infrastructure;
using TeamTaskManagement.Web.Contracts.TeamTasks;
using Xunit;

namespace TeamTaskManagement.IntegrationTests.Flows;

public sealed class TaskLifecycleFlowTests
    : IClassFixture<WebAppFactory> {

    private readonly WebAppFactory _factory;

    public TaskLifecycleFlowTests
        (WebAppFactory factory) => _factory = factory;

    [Fact]
    public async Task AssigneeCompletesTask_LeaderClosesIt() {

        using var leaderApi = new ApiCallsHelper(_factory.CreateClient());
        using var memberApi = new ApiCallsHelper(_factory.CreateClient());

        var leader = await leaderApi.RegisterAndLoginAsync(authenticateClient: true);
        var member = await memberApi.RegisterAndLoginAsync(authenticateClient: true);

        var membership = await
            TestScenarioHelper.CreateTeamWithMemberAsync
            (leaderApi, memberApi, leader.UserId, member.UserId);

        var createTaskResult = await leaderApi
            .CreateTaskAsync(new CreateTeamTaskRequest
            (membership.TeamId,
             membership.MemberMemberId,
             "پیاده‌سازی تست",
             "توضیحات کامل وظیفه",
             DateTime.UtcNow.AddDays(2)));

        createTaskResult.EnsureSuccessStatusCode();

        var myTasks = (await memberApi
            .GetMyTasksAsync()).GetRequiredContent();

        var task = myTasks.Should().ContainSingle().Which;

        (await memberApi.StartTaskAsync(task.TaskId)).EnsureSuccessStatusCode();
        (await memberApi.ClaimTaskCompletionAsync(task.TaskId)).EnsureSuccessStatusCode();
        (await leaderApi.CloseTaskAsCompleteAsync(task.TaskId)).EnsureSuccessStatusCode();

        var finalTask = (await leaderApi.GetTaskByIdAsync(task.TaskId)).GetRequiredContent();
        finalTask.StatusText.Should().NotBeNullOrWhiteSpace();
    }
}
