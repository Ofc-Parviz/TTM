namespace TeamTaskManagement.IntegrationTests.Infrastructure;

// مدل‌های پاسخ موردنیاز تست‌ها - مستقل از مدل‌های داخلی برنامه

public sealed class ApiLoginResponseDto {
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
}

public sealed class ApiTeamDto {
    public Guid TeamId { get; set; }
    public string TeamDisplayname { get; set; } = string.Empty;
}

public sealed class ApiInviteDto {
    public Guid InviteId { get; set; }
    public Guid TeamId { get; set; }
}

public sealed class ApiTeamDetailsDto {
    public List<ApiTeamMemberDto> Members { get; set; } = new();
}

public sealed class ApiTeamMemberDto {
    public Guid MemberId { get; set; }
    public Guid UserId { get; set; }
}

public sealed class ApiTaskDto {
    public Guid TaskId { get; set; }
    public string StatusText { get; set; } = string.Empty;
}
