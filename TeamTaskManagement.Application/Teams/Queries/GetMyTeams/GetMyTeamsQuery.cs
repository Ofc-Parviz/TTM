using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.Queries.GetMyTeams;

/// <summary>
/// کوئری دریافت لیست تیم های کاربر فعلی
/// </summary>
public sealed record GetMyTeamsQuery
    (bool OnlyCurrentActiveTeams = true)
    : IQuery<IReadOnlyList<MyTeamResponse>>;

public sealed record MyTeamResponse
    (Guid TeamId,
     string TeamDisplayname,
     string TeamDescription,
     string MyRoleTitle,
     string MyAccessLevel,
     int TeamCurrentMembersCount,
     int MyOpenAssignedTasksCountInThisTeam, // وظایفی که به کاربر فعلی محول شده
     int MyOpenCreatedTasksCountInThisTeam, // وظایفی که کاربر فعلی مدیرشونه
     int MyOpenSelfTasksCountInThisTeam, // وظایفی که کاربر هم مدیرشونه هم انجام دهنده
     IReadOnlyCollection<MyTeamMateResponse> Teammates);

public sealed record MyTeamMateResponse
    (string UserEmail,
     string UserDisplayname,
     string RoleDisplayname,
     string TeamAccessLevel);
