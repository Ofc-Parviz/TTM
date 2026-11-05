using FluentAssertions;
using TeamTaskManagement.Domain.ValueObjects;
using Xunit;

namespace TeamTaskManagement.Domain.Tests.ValueObjects;

public sealed class UserEmailTests {

    [Theory]
    [InlineData("test@gmail.com")]
    [InlineData("USER@Gmail.COM")]
    public void Create_With_Valid_Email_Should_Succeed(string email) {

        var result = UserEmail.Create(email);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-an-email")]
    [InlineData("a@")]
    [InlineData("@b.com")]
    public void Create_With_Invalid_Email_Should_Fail(string email) {
        var result = UserEmail.Create(email);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void FromDatabase_Should_Restore_Value() {

        var vo = UserEmail.FromDatabase("stored@test.com");

        vo.Value.Should().Be("stored@test.com");
    }
}

public sealed class PasswordTests {

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public void Create_With_Invalid_Plain_Should_Fail(string plain) {

        var result = Password.ValidatePlainText(plain);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void FromDatabase_Should_Keep_Hash() {

        var hash = "some-hash-from-db";
        var vo = Password.FromDatabase(hash);

        vo.HashedValue.Should().Be(hash);
    }
}

public sealed class TaskTitleTests {
    [Fact]
    public void Create_With_Valid_Title_Should_Succeed() {

        var result = TaskTitle.Create("پیاده‌سازی لاگین");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("پیاده‌سازی لاگین");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_With_Empty_Should_Fail(string title) {

        var result = TaskTitle.Create(title);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_With_Too_Long_Should_Fail() {

        var tooLong = new string('x', TaskTitle.MaxLength + 1);
        var result = TaskTitle.Create(tooLong);

        result.IsFailure.Should().BeTrue();
    }
}