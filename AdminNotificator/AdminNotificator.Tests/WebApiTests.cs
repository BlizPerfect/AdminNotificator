using System.Net;
using System.Net.Http.Json;
using AdminNotificator.Core;
using AdminNotificator.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace AdminNotificator.Tests;

[TestFixture]
public class WebApiTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddDbContext<AdminNotificatorDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb-" + Guid.NewGuid());
                    });
                });
            });

        _client = _factory.CreateClient();
    }
    
    [TearDown]
    public void TearDown()
    {
        _factory.Dispose();
        _client.Dispose();
    }

    [Test]
    public async Task GetAll_ShouldReturnEmptyListInitially()
    {
        var response = await _client.GetAsync("/notifications");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<EmailType>>();
        items.Should().BeEmpty();
    }

    [Test]
    public async Task Post_ShouldCreateEmailType()
    {
        var newEmail = new EmailType
        {
            Id = "abc",
            EmailTitle = "Test",
            BodyName = "Body",
            SenderEmail = "test@example.com"
        };

        var response = await _client.PostAsJsonAsync("/notifications", newEmail);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<EmailType>();
        created.Should().NotBeNull();
        created!.Id.Should().Be("abc");
    }

    [Test]
    public async Task GetById_ShouldReturnCorrectItem()
    {
        await Post_ShouldCreateEmailType();

        var response = await _client.GetAsync("/notifications/abc");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<EmailType>();
        item!.Id.Should().Be("abc");
    }

    [Test]
    public async Task Put_ShouldUpdateEmailType()
    {
        await Post_ShouldCreateEmailType();

        var updated = new EmailType
        {
            Id = "abc",
            EmailTitle = "Updated",
            BodyName = "NewBody",
            SenderEmail = "updated@example.com"
        };

        var putResponse = await _client.PutAsJsonAsync("/notifications/abc", updated);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync("/notifications/abc");
        var item = await getResponse.Content.ReadFromJsonAsync<EmailType>();
        item!.EmailTitle.Should().Be("Updated");
    }

    [Test]
    public async Task Delete_ShouldRemoveItem()
    {
        await Post_ShouldCreateEmailType();

        var deleteResponse = await _client.DeleteAsync("/notifications/abc");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync("/notifications/abc");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}