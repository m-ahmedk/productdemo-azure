using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ProductDemo.DTOs.Auth;
using System.Net;
using System.Net.Http.Json;

namespace ProductDemo.IntegrationTests.Controllers.Auth;

public class AuthValidationTests : TestBase
{
    public AuthValidationTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    // REGISTER VALIDATION TESTS

    [Fact]
    public async Task Register_Should_Fail_When_Email_Is_Missing()
    {
        await AuthorizeAsync(); // optional, AuthController might be [AllowAnonymous]

        var dto = new RegisterDto
        {
            Email = "",
            Password = "P@ssword123"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task Register_Should_Fail_When_Password_Is_Missing()
    {
        var dto = new RegisterDto
        {
            Email = "user@test.com",
            Password = ""
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Password");
    }

    // LOGIN VALIDATION TESTS

    [Fact]
    public async Task Login_Should_Fail_When_Email_Is_Missing()
    {
        var dto = new LoginDto
        {
            Email = "",
            Password = "somePass123"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task Login_Should_Fail_When_Password_Is_Missing()
    {
        var dto = new LoginDto
        {
            Email = "user@test.com",
            Password = ""
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Password");
    }
}
