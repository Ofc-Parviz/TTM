namespace TeamTaskManagement.Shared.Enums;

/// <summary>
/// فیلتر وظایف اعضا
/// </summary>
public enum MemberTasksFilter {

    /// <summary>
    /// وظایفی که عضو فقط مدیر یا سازندشونه
    /// </summary>
    CreatedTasks = 0,

    /// <summary>
    /// وظایفی که عضو فقط مسئول انجامشونه
    /// </summary>
    AssignedTasks = 1,

    /// <summary>
    /// وظایفی که عضو هم مدیرشونه هم مسئول انجامشون
    /// </summary>
    SelfTasks = 2,

    /// <summary>
    /// همه وظایف مربوط به عضو، چه مسئول و چه مدیر و چه هردو
    /// </summary>
    AllTasks = 3,
}