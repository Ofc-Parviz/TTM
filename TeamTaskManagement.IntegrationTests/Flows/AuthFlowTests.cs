using System.Net;
using FluentAssertions;
using TeamTaskManagement.Web.Contracts.Auth;
using TeamTaskManagement.IntegrationTests.Infrastructure;

namespace TeamTaskManagement.IntegrationTests.Flows;

public sealed class AuthFlowTests
    : IClassFixture<WebAppFactory>, IDisposable {

    private readonly ApiCallsHelper _api;

    public AuthFlowTests(WebAppFactory factory)
        => _api = new ApiCallsHelper(factory.CreateClient());

    [Fact]
    public async Task Register_With_DuplicateEmail_Returns_BadRequest() {

        var user = await _api.RegisterAndLoginAsync();

        var response = await _api.RegisterAsync(new RegisterRequest
            (user.Email, user.Password, "کاربر دیگر", "نام", "خانوادگی"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_With_WrongPassword_Returns_Unauthorized_Or_BadRequest() {

        var user = await _api
            .RegisterAndLoginAsync();

        var response = await _api
            .LoginAsync(new LoginRequest(user.Email, "wrong-password"));

        response.StatusCode.Should().BeOneOf
            (HttpStatusCode.Unauthorized,
             HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProfile_WithoutToken_Returns_Unauthorized() {

        _api.ClearToken();

        var response = await _api.GetMyProfileAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_With_ValidToken_Returns_Ok() {

        var user = await _api.RegisterAndLoginAsync();

        _api.UseToken(user.AccessToken);

        var response = await _api.GetMyProfileAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public void Dispose() => _api.Dispose();
}
