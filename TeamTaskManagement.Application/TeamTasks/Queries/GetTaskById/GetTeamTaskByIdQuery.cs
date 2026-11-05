using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.TeamTasks.Responses;

namespace TeamTaskManagement.Application.TeamTasks.Queries.GetTaskById;

/// <summary>
/// کوئری دریافت جزئیات یک وظیفه
/// </summary>
public sealed record GetTeamTaskByIdQuery(Guid TeamTaskId) : IQuery<TeamTaskResponse>;
