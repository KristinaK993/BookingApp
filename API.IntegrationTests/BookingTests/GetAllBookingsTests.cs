using System.Net;
using API.IntegrationTests.Helpers;
using NUnit.Framework;

namespace API.IntegrationTests.BookingTests;

public class GetAllBookingsTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetAllBookings_ShouldReturn200OK()
    {
        // ACT
        var response = await _client.GetAsync("/api/bookings");

        // ASSERT
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
