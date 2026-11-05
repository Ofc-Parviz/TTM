namespace TeamTaskManagement.Shared.Enums;

/// <summary>
/// ورودی نقش و سطح دسترسی عضو تیم
/// </summary>
public enum TeamMemberRoleInput {

    /// <summary>
    /// لیدر - معادل مالک تیم - اختیارات کامل
    /// </summary>
    Leader = 1,

    /// <summary>
    /// مدیرمیانی - منصوب شده توسط لیدر - اختیارات: 
    /// دعوت دیگران - تعریف وظایف برای خود و دیگران بجز 
    /// لیدر - مشاهده و تکمیل وظایف محوله و ارسال گزارش به لیدر
    /// </summary>
    Manager = 2,

    /// <summary>
    /// عضو معمولی - اختیارات: مشاهده و تکمیل
    /// وظایف محوله و ارسال گزارش به وظیفه دهنده
    /// </summary>
    Member = 3
}
