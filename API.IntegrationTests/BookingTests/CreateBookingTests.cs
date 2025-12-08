using System.Net;
using System.Net.Http.Json;
using API.Dtos.Bookings;
using API.IntegrationTests.Helpers;
using NUnit.Framework;

namespace API.IntegrationTests.BookingTests;

public class CreateBookingTests
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
    public async Task CreateBooking_ShouldReturn201Created()
    {
        // ARRANGE – bokning att skapa
        var dto = new BookingCreateDto
        {
            CompanyId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2),
            Notes = "Test booking",
            ServiceIds = new List<int> { 1 }
        };

        // ACT – skicka request
        var response = await _client.PostAsJsonAsync("/api/bookings", dto);

        // ASSERT – API returnerar Created (201)
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task CreateThenGetBooking_ShouldReturnSameBooking()
    {
        // ARRANGE – skapa en bokning
        var dto = new BookingCreateDto
        {
            CompanyId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2),
            Notes = "Integration booking",
            ServiceIds = new List<int> { 1 }
        };

        // POST /api/bookings
        var createResponse = await _client.PostAsJsonAsync("/api/bookings", dto);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        // Läs ut den skapade bokningen från svaret
        var createdDto =
            await createResponse.Content.ReadFromJsonAsync<BookingDetailDto>();

        Assert.IsNotNull(createdDto);
        Assert.That(createdDto!.Id, Is.GreaterThan(0));

        // ACT – GET /api/bookings/{id}
        var getResponse = await _client.GetAsync($"/api/bookings/{createdDto.Id}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var fetched =
            await getResponse.Content.ReadFromJsonAsync<BookingDetailDto>();

        // ASSERT – samma data
        Assert.IsNotNull(fetched);
        Assert.That(fetched!.Id, Is.EqualTo(createdDto.Id));
        Assert.That(fetched.CustomerId, Is.EqualTo(dto.CustomerId));
        Assert.That(fetched.EmployeeId, Is.EqualTo(dto.EmployeeId));
        Assert.That(fetched.Notes, Is.EqualTo("Integration booking"));
    }

}
