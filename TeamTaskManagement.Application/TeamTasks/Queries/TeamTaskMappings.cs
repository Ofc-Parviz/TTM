using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Application.Teams.Queries;
using TeamTaskManagement.Application.Teams.Responses;
using TeamTaskManagement.Application.TeamTasks.Responses;
using TeamTaskManagement.Application.TeamTasks.Queries.GetMyTasks;
using static TeamTaskManagement.Shared.Classes.Constants;
using TeamTaskManagement.Domain.Shared;
using TeamTaskManagement.Shared.Enums;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Queries {
    /// <summary>
    /// مپ کردن ریسپانس های مربوط به وظیفه تیم
    /// </summary>
    internal static class TeamTaskMappings {

        #region Map All Tasks

        /// <summary>
        /// تبدیل اطلاعات خام لیست وظایف تیم به لیستی از ریسپانس
        /// های مناسب - دریافت ایمیل ها و نام های نمایشی از دیکشنری
        /// </summary>
        public static List<TeamTaskResponse> MapAllTasks
            (Guid currentUserId,
             IReadOnlyCollection<TeamTask> tasks,
             ICollection<Team> teams,
             ICollection<TeamMember> allMemberships,
             IReadOnlyDictionary<Guid, (string, string)> dict) {

            var response = new List<TeamTaskResponse>();

            foreach (var task in tasks) {

                var team = teams.FirstOrDefault(t => t.Id == task.TeamId);

                var assignee = allMemberships
                    .FirstOrDefault(member => member.Id == task.AppointedToMemberId);

                var creator = allMemberships
                    .FirstOrDefault(member => member.Id == task.CreatedByMemberId);

                var currentUserMembership
                    = allMemberships.FirstOrDefault(member
                    => member.UserId == currentUserId
                    && member.TeamId == task.TeamId
                    && member.Status == TeamMemberStatus.Active);

                var currentUserAccessLevelInCurrentTeam = currentUserMembership?
                    .AccessLevel.MapMemberRoleResponse() ?? MemberRoleResponse.Member;

                var taskResp = MapTeamTaskResponse(task,
                    team, creator, assignee, currentUserId,
                    currentUserAccessLevelInCurrentTeam, dict);

                response.Add(taskResp);
            }

            response = response
                .OrderByDescending(task => task.CreatedAtUtc).ToList();

            return response;
        }

        #endregion

        #region Map Task Response - Simple

        /// <summary>
        /// تبدیل اطلاعات خام وظیفه تیم به ریسپانس مناسب
        /// </summary>
        public static TeamTaskResponse MapTeamTaskResponse
            (TeamTask? task,
             Team? team,
             MemberResponse creator,
             MemberResponse assignee,
             MyTaskPositionResponse myPosition,
             MemberRoleResponse currentUserAccessLevelInTeam) {

            return new TeamTaskResponse
                (task?.Id ?? Guid.Empty,
                 task?.TeamId ?? Guid.Empty,
                 team?.TeamDisplayname.Value ?? NotAvailable,
                 task?.Title.Value ?? NotAvailable,
                 task?.Description.Value ?? NotAvailable,
                 task?.Status ?? (TeamTaskStatus)(-1),
                 task?.Status.TeamTaskStatusToString() ?? NotAvailable,
                 task?.DeadlineUtc ?? null,
                 task?.CreatedAtUtc ?? DateTime.MinValue,
                 task?.LastModifiedAtUtc ?? DateTime.MinValue,
                 creator,
                 assignee,
                 myPosition,
                 currentUserAccessLevelInTeam);
        }

        #endregion

        #region Map Task Response - 2

        /// <summary>
        /// تبدیل اطلاعات خام وظیفه تیم به ریسپانس
        /// مناسب - دریافت ایمیل و نام نمایشی از دیکشنری
        /// </summary>
        public static TeamTaskResponse MapTeamTaskResponse
            (TeamTask? task,
             Team? team,
             TeamMember? creator,
             TeamMember? assignee,
             Guid currentUserId,
             MemberRoleResponse currentUserAccessLevelInTeam,
             IReadOnlyDictionary<Guid, (string, string)> dict) {

            var creatorResp = TeamMappings.MapMemberResponse(creator, dict);

            var assigneeResp = TeamMappings.MapMemberResponse(assignee, dict);

            var positionResp = MapMyTaskPositionResponse(creator, assignee, currentUserId);

            return MapTeamTaskResponse(task, team, creatorResp,
                assigneeResp, positionResp, currentUserAccessLevelInTeam);
        }

        #endregion

        #region Map Task Position Response

        /// <summary>
        /// مپ کردن اینکه کاربر فعلی چه نقشی در وظیفه تعیین شده دارد
        /// </summary>
        /// <param name="creator">
        /// اطلاعات عضویت مدیر وظیفه
        /// </param>
        /// <param name="assignee">
        /// اطلاعات عضویت مسئول انجام وظیفه
        /// </param>
        /// <param name="currentUserId">
        /// شناسه کاربری مربوط به کاربر فعلی
        /// </param>
        /// <returns>
        /// اگر اطلاعات عضویت ها ناقص باشد، «نامشخص» بازخواهدگشت - 
        /// اگر کاربر هم مدیر و هم مسئول باشد، «هردو» بازخواهدگشت - 
        /// اگر کاربر فقط مدیر وظیفه باشد، «مدیر» بازخواهدگشت - 
        /// اگر کاربر فقط مسئول انجام وظیفه باشد، «مسئول» بازخواهدگشت - 
        /// و در نهایت اگر کاربر هیچکاره باشد، «بدون نقش» بازخواهدگشت
        /// </returns>
        public static MyTaskPositionResponse MapMyTaskPositionResponse
            (TeamMember? creator, TeamMember? assignee, Guid currentUserId) {

            // اگر اطلاعات مدیر یا مسئول انجام نال باشند نمیتوان
            // فهمید که نقش کاربر فعلی در این وظیفه چیست، چون
            // برای فهمیدن آن به اطلاعات هردو عضو درگیر احتیاج داریم
            if (assignee is null || creator is null)
                // پس در این حالت میگوییم که نقش
                // کاربر در این وظیفه نامشخص است
                return MyTaskPositionResponse.Unknown;

            // اگر اطلاعات مدیر وظیفه با اطلاعات مسئول
            // انجام آن وظیفه یکسان بود، یعنی با یک
            // وظیفه انفرادی طرفیم که کلا مال یه نفره
            if (creator == assignee) {

                // حالا اگر این وظیفه انفرادی
                // مربوط به کاربر فعلی باشه
                if(creator?.UserId == currentUserId)
                    // میگوییم کاربر در این
                    // وظیفه، هر دو نقش رو بعهده داره
                    return MyTaskPositionResponse.Both;

                // و در غیر اینصورت، میگوییم کاربر
                // فعلی هیچ نقشی در این وظیفه نداره
                else return MyTaskPositionResponse.None;
            }

            // اگر شروط قبلی صدق نکرد، یعنی با یه وظیفه تیمی
            // طرفیم که مدیر و مسئولش دو عضو متفاوتن، پس باید
            // حالت های باقیمونده رو بررسی کنیم

            // اگر شناسه کاربری مدیر وظیفه، با
            // شناسه کاربر فعلی برابر باشد
            if (creator?.UserId == currentUserId)
                // میگوییم کاربر فعلی، مدیر این وظیفه است
                return MyTaskPositionResponse.Creator;

            // یا اگر شناسه کاربری مسئول وظیفه، با
            // شناسه کاربر فعلی برابر باشد
            if (assignee?.UserId == currentUserId)
                // میگوییم کاربر فعلی، مسئول انجام دادن این وظیفه است
                return MyTaskPositionResponse.Assignee;

            // اگر باز هم هیچ شرطی صدق نکند
            // کاربر فعلی قطعا هیچ نقشی در این وظیفه ندارد
            return MyTaskPositionResponse.None;
        }

        #endregion

        #region Map Task Query Filter into Repository Variation

        /// <summary>
        /// ترجمه فیلتر کوئری وظایف من، به فیلتر معادل در ریپوزیتوری
        /// </summary>
        public static MemberTasksFilter MapRepositoryMemberTasksFilter
            (this MyTasksQueryFilter filter) {

            switch (filter) {

                case MyTasksQueryFilter.MyCreatedTasks:
                    return MemberTasksFilter.CreatedTasks;

                case MyTasksQueryFilter.MyAssignedTasks:
                    return MemberTasksFilter.AssignedTasks;

                case MyTasksQueryFilter.MySelfTasks:
                    return MemberTasksFilter.SelfTasks;

                default: return MemberTasksFilter.AllTasks;
            }
        }

        #endregion
    }
}
#region Obsoleted Commented Codes

// منطق قبلی مربوط به مپینگ هارو که در
// حال حاضر منسوخ شده کامنت کردم

#region My Task Response [Obsoleted]

///// <summary>
///// ریسپانس وظیفه کاربر لاگین شده
///// </summary>
///// <param name="MyTaskmateMembership">
///// هم تیمی وظیفه - اگر کاربر هم مدیر باشد هم مسئول، باید نال باشد
///// </param>
//public sealed record MyTaskResponse
//    (Guid TaskId,
//     Guid TeamId,
//     string TeamDisplayname,
//     string Title,
//     string Description,
//     TeamTaskStatus Status,
//     string StatusText,
//     DateTime? DeadlineUtc,
//     DateTime CreatedAtUtc,
//     MyTaskPositionResponse MyPosition,
//     TaskMemberResponse MyMembership,
//     TaskMemberResponse? MyTaskmateMembership);

#endregion

#region Map My Task Response - 1 [Obsoleted]

//public static MyTaskResponse MapMyTaskResponse
//    (TeamTask? task,
//     Team? team,
//     MyTaskPositionResponse myPosition,
//     TaskMemberResponse myMembership,
//     TaskMemberResponse? myTaskmateMembership) {

//    return new MyTaskResponse
//        (task?.Id ?? Guid.Empty,
//         task?.TeamId ?? Guid.Empty,
//         team?.TeamDisplayname.Value ?? NotAvailable,
//         task?.Title.Value ?? NotAvailable,
//         task?.Description.Value ?? NotAvailable,
//         task?.Status ?? (TeamTaskStatus)(-1),
//         task?.Status.TeamTaskStatusToString() ?? NotAvailable,
//         task?.DeadlineUtc ?? null,
//         task?.CreatedAtUtc ?? DateTime.MinValue,
//         myPosition,
//         myMembership,
//         myTaskmateMembership);
//}

#endregion

#region Map My Task Response - 2 [Obsoleted]

//public static MyTaskResponse MapMyTaskResponse
//    (TeamTask? task,
//     Team? team,
//     TeamMember? creator,
//     TeamMember? assignee,
//     Guid currentUserId,
//     IReadOnlyDictionary<Guid, (string, string)> dict) {

//    var creatorResp = MapTaskMemberResponse(creator, dict);

//    var assigneeResp = MapTaskMemberResponse(assignee, dict);

//    var positionResp = MapMyTaskPositionResponse(creator, assignee, currentUserId);

//    if (positionResp is MyTaskPositionResponse.Both)
//        return MapMyTaskResponse(task, team,
//            positionResp, creatorResp, null);

//    if (positionResp is MyTaskPositionResponse.Creator)
//        return MapMyTaskResponse(task, team,
//             myPosition: positionResp,
//             myMembership: creatorResp,
//             myTaskmateMembership: assigneeResp);

//    if (positionResp is MyTaskPositionResponse.Assignee)
//        return MapMyTaskResponse(task, team,
//            myPosition: positionResp,
//            myMembership: assigneeResp,
//            myTaskmateMembership: creatorResp);

//    return MapMyTaskResponse(task, team, positionResp, creatorResp, assigneeResp);
//}

#endregion

// بعدا این اتفاق برای MyTeamResponse و MyTeamMateResponse هم میفته احتمالا

#endregion