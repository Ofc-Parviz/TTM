using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManagement.Application.Users.Responses;

/// <summary>
/// ریسپانس آمار کاربر
/// </summary>
public sealed record UserStatisticsResponse
    (UserTaskStatisticItemResponse AssignedTasks,
     UserTaskStatisticItemResponse CreatedTasks,
     UserTaskStatisticItemResponse SelfTasks,
     UserMembershipStatisticsResponse Memberships);

public sealed record UserTaskStatisticItemResponse
    (int TotalCount,
     int OpenCount,
     int ClosedCount,
     int SuccessEndedCount,
     int FailureEndedCount,
     int DeadlineFailureEndedCount);

public sealed record UserMembershipStatisticsResponse
    (int TotalCount,
     int CurrentlyActiveCount,
     int LeavedCount,
     int KickedOutCount);
