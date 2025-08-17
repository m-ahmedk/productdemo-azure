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

    protected async Task<string> GetJwtAsync(string email = "mahmedvilla@gmail.com", string password = "ahmed123#")
    {
        var login = new { Email = email, Password = password };
        var response = await Client.PostAsJsonAsync("/api/auth/login", login);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        return content!.Data!;
    }

    protected async Task AuthorizeAsync()
    {
        var token = await GetJwtAsync();
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    protected void Unauthorize()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
}
