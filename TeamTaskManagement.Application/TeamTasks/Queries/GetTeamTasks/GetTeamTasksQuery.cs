using TeamTaskManagement.Application.TeamTasks.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Enums;

namespace TeamTaskManagement.Application.TeamTasks.Queries.GetTeamTasks;

/// <summary>
/// کوئری دریافت وظایف یک تیم
/// </summary>
public sealed record GetTeamTasksQuery
    (Guid TeamId,
     TeamTaskStatusFilter StatusFilter = TeamTaskStatusFilter.All)
: IQuery<TasksForTeamResponse>;
