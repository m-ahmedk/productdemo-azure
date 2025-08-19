using ProductDemo.Helpers;
using System.Net.Http.Json;

namespace ProductDemo.IntegrationTests;

public abstract class TestBase : IClassFixture<CustomWebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;

    protected TestBase(CustomWebApplicationFactory<Program> factory)
    {
        Client = factory.CreateClient();
    }

    protected async Task<string> GetJwtAsync(string email, string password)
    {
        var login = new { Email = email, Password = password };
        var response = await Client.PostAsJsonAsync("/api/auth/login", login);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        return content!.Data!;
    }

    // Authorize with role parameter
    protected async Task AuthorizeAsync(string role = "Admin")
    {
        string email, password;

        if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            email = "mahmedvilla@gmail.com";
            password = "ahmed123#";
        }
        else if (role.Equals("User", StringComparison.OrdinalIgnoreCase))
        {
            email = "normaluser@test.com";
            password = "user123#";
        }
        else
        {
            throw new ArgumentException($"Unknown role: {role}");
        }

        var token = await GetJwtAsync(email, password);
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    protected void Unauthorize()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
}
