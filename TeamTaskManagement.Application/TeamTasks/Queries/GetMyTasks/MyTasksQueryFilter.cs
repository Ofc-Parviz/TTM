namespace TeamTaskManagement.Application.TeamTasks.Queries.GetMyTasks;

/// <summary>
/// فیلتر کوئری وظایف کاربر
/// </summary>
public enum MyTasksQueryFilter {

    /// <summary>
    /// وظایفی که کاربر فقط مدیر یا سازندشونه
    /// </summary>
    MyCreatedTasks = 0,

    /// <summary>
    /// وظایفی که کاربر فقط مسئول انجامشونه
    /// </summary>
    MyAssignedTasks = 1,

    /// <summary>
    /// وظایفی که کاربر هم مدیرشونه هم مسئول انجامشون
    /// </summary>
    MySelfTasks = 2,

    /// <summary>
    /// همه وظایف مربوط به کاربر، چه مسئول و چه مدیر و چه هردو
    /// </summary>
    MyAllTasks = 3
}
