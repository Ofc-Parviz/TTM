using TeamTaskManagement.Application.Teams.Responses;
using TeamTaskManagement.Shared.Enums;

namespace TeamTaskManagement.Application.TeamTasks.Responses;

/// <summary>
/// ریسپانس وظایف مربوط به یک تیم
/// </summary>
public sealed record TasksForTeamResponse
    (Guid TeamId,
     string TeamDisplayname,
     TeamTaskStatusFilter AppliedStatusFilter,
     MemberRoleResponse CurrentUserAccessLevelInTeam,
     IReadOnlyList<TeamTaskResponse> TeamTasks);
