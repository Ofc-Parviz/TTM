using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TeamTaskManagement.Web.Contracts.Auth;
using TeamTaskManagement.Web.Contracts.Teams;
using TeamTaskManagement.Web.Contracts.TeamTasks;
using TeamTaskManagement.Web.Contracts.Users;

namespace TeamTaskManagement.IntegrationTests.Infrastructure;

/// <summary>
/// تنها محل ارسال مستقیم درخواست‌های اچ-تی-تی-پی در تست‌های یکپارچگی
/// </summary>
public sealed class ApiCallsHelper : IDisposable {

    #region Injections

    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions
        JsonOptions = new(JsonSerializerDefaults.Web);

    public ApiCallsHelper(HttpClient httpClient)
        => _httpClient = httpClient ?? throw new
        ArgumentNullException(nameof(httpClient));

    #endregion

    #region Authentication

    public Task<ApiCallResult> RegisterAsync(RegisterRequest request)
        => SendJsonAsync(HttpMethod.Post, "/api/auth/register", request);

    public Task<ApiCallResult<ApiLoginResponseDto>> LoginAsync(LoginRequest request)
        => SendJsonAsync<ApiLoginResponseDto, LoginRequest>(HttpMethod.Post, "/api/auth/login", request);

    public async Task<TestUser> RegisterAndLoginAsync
        (string? email = null,
         string password = "123456mm",
         bool authenticateClient = false) {

        email ??= $"user_{Guid.NewGuid():N}@gmail.com";

        var registerResult = await RegisterAsync(new RegisterRequest(
            email,
            password,
            "کاربر تست",
            "نام",
            "خانوادگی"));

        registerResult.EnsureSuccessStatusCode();

        var loginResult = await LoginAsync
            (new LoginRequest(email, password));

        var login = loginResult.GetRequiredContent();

        if (string.IsNullOrWhiteSpace(login.AccessToken))
            throw new InvalidOperationException("پاسخ ورود، توکن دسترسی معتبری نداشت.");

        var user = new TestUser(login.UserId,
            email, password, login.AccessToken);

        if (authenticateClient)
            UseToken(user.AccessToken);

        return user;
    }

    public void UseToken(string accessToken) {

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("توکن دسترسی نمی‌تواند خالی باشد", nameof(accessToken));

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public void ClearToken()
        => _httpClient.DefaultRequestHeaders.Authorization = null;

    #endregion

    #region Profile

    public Task<ApiCallResult> GetMyProfileAsync()
        => SendAsync(HttpMethod.Get, "/api/profile");

    public Task<ApiCallResult> UpdateMyProfileAsync(UpdateUserProfileRequest request)
        => SendJsonAsync(HttpMethod.Put, "/api/profile", request);

    public Task<ApiCallResult> ChangeMyPasswordAsync(ChangeUserPasswordRequest request)
        => SendJsonAsync(HttpMethod.Post, "/api/profile/new-password", request);

    #endregion

    #region Teams

    public Task<ApiCallResult<List<ApiTeamDto>>> GetMyTeamsAsync()
        => SendAsync<List<ApiTeamDto>>(HttpMethod.Get, "/api/teams");

    public Task<ApiCallResult<ApiTeamDetailsDto>> GetTeamByIdAsync(Guid teamId)
        => SendAsync<ApiTeamDetailsDto>(HttpMethod.Get, $"/api/teams/{teamId}");

    public Task<ApiCallResult> CreateTeamAsync(CreateTeamRequest request)
        => SendJsonAsync(HttpMethod.Post, "/api/teams", request);

    public Task<ApiCallResult> UpdateTeamAsync(UpdateTeamRequest request)
        => SendJsonAsync(HttpMethod.Put, "/api/teams", request);

    public Task<ApiCallResult> LeaveTeamAsync(Guid teamId)
        => SendAsync(HttpMethod.Delete, $"/api/teams/{teamId}/leave");

    public Task<ApiCallResult> RemoveTeamMemberAsync(Guid teamId, Guid memberId)
        => SendAsync(HttpMethod.Delete, $"/api/teams/{teamId}/members/{memberId}");

    public Task<ApiCallResult> ChangeMemberRoleAsync
        (Guid teamId,
         Guid memberId,
         ChangeMemberRoleRequest request) => SendJsonAsync(HttpMethod
             .Put, $"/api/teams/{teamId}/members/{memberId}/role", request);

    #endregion

    #region Invites

    public Task<ApiCallResult<List<ApiInviteDto>>> GetMyInvitesAsync
        (string directionFilter = "All",
         string statusFilter = "All",
         Guid? teamId = null) {

        var queryParameters = new List<string>();

        if (teamId.HasValue)
            queryParameters.Add($"TeamId={Uri.EscapeDataString(teamId.Value.ToString())}");

        queryParameters.Add($"DirectionFilter={Uri.EscapeDataString(directionFilter)}");
        queryParameters.Add($"StatusFilter={Uri.EscapeDataString(statusFilter)}");

        var queryString = string.Join("&", queryParameters);
        return SendAsync<List<ApiInviteDto>>(HttpMethod.Get, $"/api/invites?{queryString}");
    }

    public Task<ApiCallResult> SendInviteAsync(SendTeamInviteRequest request)
        => SendJsonAsync(HttpMethod.Post, "/api/invites", request);

    public Task<ApiCallResult> AcceptInviteAsync(AcceptInviteRequest request)
        => SendJsonAsync(HttpMethod.Post, "/api/invites/accept", request);

    public Task<ApiCallResult> RejectInviteAsync(RejectInviteRequest request)
        => SendJsonAsync(HttpMethod.Put, "/api/invites/reject", request);

    public Task<ApiCallResult> CancelInviteAsync(CancelInviteRequest request) {

        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/invites") {
            Content = JsonContent.Create(request)
        };

        return SendAsync(httpRequest);
    }

    #endregion

    #region Tasks

    public Task<ApiCallResult<List<ApiTaskDto>>> GetMyTasksAsync()
        => SendAsync<List<ApiTaskDto>>(HttpMethod.Get, "/api/tasks/for-me");

    public Task<ApiCallResult<List<ApiTaskDto>>> GetTeamTasksAsync(GetTeamTasksRequest request) {

        var query = $"TeamId={Uri.EscapeDataString(request.TeamId.ToString())}" +
                    $"&StatusFilter={Uri.EscapeDataString(request.StatusFilter.ToString())}";

        return SendAsync<List<ApiTaskDto>>(HttpMethod.Get, $"/api/tasks/for-team?{query}");
    }

    public Task<ApiCallResult<ApiTaskDto>> GetTaskByIdAsync(Guid taskId)
        => SendAsync<ApiTaskDto>(HttpMethod.Get, $"/api/tasks/{taskId}");

    public Task<ApiCallResult> CreateTaskAsync(CreateTeamTaskRequest request)
        => SendJsonAsync(HttpMethod.Post, "/api/tasks", request);

    public Task<ApiCallResult> UpdateTaskDetailsAsync(UpdateTeamTaskDetailsRequest request)
        => SendJsonAsync(HttpMethod.Put, "/api/tasks/details", request);

    public Task<ApiCallResult> UpdateTaskDeadlineAsync(UpdateTeamTaskDeadlineRequest request)
        => SendJsonAsync(HttpMethod.Put, "/api/tasks/deadline", request);

    public Task<ApiCallResult> RotateTaskAssigneeAsync(RotateTeamTaskAssigneeRequest request)
        => SendJsonAsync(HttpMethod.Put, "/api/tasks/assignee", request);

    public Task<ApiCallResult> RotateTaskCreatorAsync(RotateTeamTaskCreatorRequest request)
        => SendJsonAsync(HttpMethod.Put, "/api/tasks/creator", request);

    public Task<ApiCallResult> StartTaskAsync(Guid taskId)
        => SendAsync(HttpMethod.Post, $"/api/tasks/{taskId}/start");

    public Task<ApiCallResult> ClaimTaskCompletionAsync(Guid taskId)
        => SendAsync(HttpMethod.Post, $"/api/tasks/{taskId}/claim-complete");

    public Task<ApiCallResult> ClaimTaskFailureAsync(Guid taskId)
        => SendAsync(HttpMethod.Post, $"/api/tasks/{taskId}/claim-failure");

    public Task<ApiCallResult> OrderTaskRetryAsync(Guid taskId)
        => SendAsync(HttpMethod.Post, $"/api/tasks/{taskId}/order-retry");

    public Task<ApiCallResult> AcceptTaskRetryAsync(Guid taskId)
        => SendAsync(HttpMethod.Post, $"/api/tasks/{taskId}/accept-retry");

    public Task<ApiCallResult> RejectTaskRetryAsync(Guid taskId)
        => SendAsync(HttpMethod.Post, $"/api/tasks/{taskId}/reject-retry");

    public Task<ApiCallResult> CloseTaskAsCompleteAsync(Guid taskId)
        => SendAsync(HttpMethod.Post, $"/api/tasks/{taskId}/close-complete");

    public Task<ApiCallResult> CloseTaskAsFailureAsync(Guid taskId)
        => SendAsync(HttpMethod.Post, $"/api/tasks/{taskId}/close-failure");

    #endregion

    #region Private-Methods: Send Request & Get Response

    private Task<ApiCallResult> SendAsync(HttpMethod method, string requestUri)
        => SendAsync(new HttpRequestMessage(method, requestUri));

    private Task<ApiCallResult<T>> SendAsync<T>(HttpMethod method, string requestUri)
        => SendAsync<T>(new HttpRequestMessage(method, requestUri));

    private Task<ApiCallResult> SendJsonAsync<TRequest>
        (HttpMethod method, string requestUri, TRequest request)
        => SendAsync(new HttpRequestMessage(method, requestUri) {
            Content = JsonContent.Create(request)
        });

    private Task<ApiCallResult<TResponse>> SendJsonAsync
        <TResponse, TRequest>
        (HttpMethod method,
         string requestUri,
         TRequest request)
        => SendAsync<TResponse>(new HttpRequestMessage(method, requestUri) {
            Content = JsonContent.Create(request)
        });

    private async Task<ApiCallResult> SendAsync(HttpRequestMessage request) {

        using (request)
        using (var response = await _httpClient.SendAsync(request)) {
            var responseBody = await response.Content.ReadAsStringAsync();
            return new ApiCallResult(response.StatusCode, responseBody);
        }
    }

    private async Task<ApiCallResult<TResponse>> SendAsync<TResponse>(HttpRequestMessage request) {

        using (request)
        using (var response = await _httpClient.SendAsync(request)) {

            var responseBody = await response.Content.ReadAsStringAsync();

            var content = !response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(responseBody)
                ? default
                : JsonSerializer.Deserialize<TResponse>(responseBody, JsonOptions);

            return new ApiCallResult<TResponse>(response.StatusCode, content, responseBody);
        }
    }

    public void Dispose() => _httpClient.Dispose();

    #endregion
}

#region Used Records

public sealed record TestUser(Guid UserId, string Email, string Password, string AccessToken);

public sealed record ApiCallResult(HttpStatusCode StatusCode, string ResponseBody) {
    public void EnsureSuccessStatusCode() {

        if ((int)StatusCode is < 200 or >= 300)
            throw new HttpRequestException(
                $"درخواست API با وضعیت {(int)StatusCode} ({StatusCode}) ناموفق بود. پاسخ: {ResponseBody}",
                inner: null,
                statusCode: StatusCode);
    }
}

public sealed record ApiCallResult<T>
    (HttpStatusCode StatusCode,
     T? Content,
     string ResponseBody) {

    public T GetRequiredContent() {

        if ((int)StatusCode is < 200 or >= 300)
            throw new HttpRequestException
                ($"درخواست API با وضعیت {(int)StatusCode} ({StatusCode}) ناموفق بود. پاسخ: {ResponseBody}",
                 inner: null,
                 statusCode: StatusCode);

        return Content ?? throw new InvalidOperationException
            ($"پاسخ API با وضعیت {StatusCode} خالی بود، درحالی‌که محتوای JSON انتظار می‌رفت.");
    }
}

#endregion
