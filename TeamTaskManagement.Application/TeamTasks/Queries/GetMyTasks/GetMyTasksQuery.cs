using TeamTaskManagement.Application.TeamTasks.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Queries.GetMyTasks;

/// <summary>
/// کوئری دریافت وظایف کاربر لاگین شده
/// </summary>
public sealed record GetMyTasksQuery
    (bool OnlyActiveMemberships = true,
     MyTasksQueryFilter Filter = MyTasksQueryFilter.MyAllTasks,
     Guid? TeamId = null) : IQuery<IReadOnlyList<TeamTaskResponse>>;
