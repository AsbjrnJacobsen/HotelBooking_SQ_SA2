using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Reqnroll;

namespace HotelBooking.CucumberTests.StepDefinitions;

[Binding]
public class BookingStepDefinitions
{
    private HttpClient _client;
    private HttpResponseMessage _response;

    [BeforeScenario]
    public void Setup()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000")
        };
    }

    [Given(@"the hotel has no existing bookings")]
    public async Task GivenTheHotelHasNoExistingBookings()
    {
        var response = await _client.GetAsync("/bookings");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var bookingList = JsonConvert.DeserializeObject<List<dynamic>>(content);

        foreach (var booking in bookingList)
        {
            int id = booking.id;
            await _client.DeleteAsync($"/bookings/{id}");
        }
    }

    [When(@"I create a booking (.*) days from today to (.*) days from today")]
    public async Task WhenICreateABookingFromTodayOffsets(int startOffset, int endOffset)
    {
        var startDate = DateTime.Today.AddDays(startOffset);
        var endDate = DateTime.Today.AddDays(endOffset);

        var booking = new
        {
            StartDate = startDate,
            EndDate = endDate,
            CustomerId = 1
        };

        var content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");
        _response = await _client.PostAsync("/bookings", content);
    }

    [Then(@"the booking should be (.*)")]
    public void ThenTheBookingShouldBe(string result)
    {
        if (result.Trim().ToLower() == "created")
            _response.StatusCode.Should().Be(HttpStatusCode.Created);
        else
            _response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
    }

    [Given(@"the hotel already has a booking from ""(.*)"" to ""(.*)""")]
    public async Task GivenTheHotelAlreadyHasABookingFromTo(string start, string end)
    {
        var booking = new
        {
            StartDate = DateTime.ParseExact(start, "dd/MM/yyyy", null),
            EndDate = DateTime.ParseExact(end, "dd/MM/yyyy", null),
            CustomerId = 1
        };

        var content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");
        await _client.PostAsync("/bookings", content);
    }

    [Given(@"the hotel is fully booked from ""(.*)"" to ""(.*)""")]
    public async Task GivenTheHotelIsFullyBookedFromTo(string start, string end)
    {
        DateTime startDate = DateTime.ParseExact(start, "dd/MM/yyyy", null);
        DateTime endDate = DateTime.ParseExact(end, "dd/MM/yyyy", null);

        for (int roomId = 1; roomId <= 3; roomId++)
        {
            var booking = new
            {
                StartDate = startDate,
                EndDate = endDate,
                CustomerId = roomId,
                RoomId = roomId
            };

            var content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");
            await _client.PostAsync("/bookings", content);
        }
    }

    // Removed pending step: WhenICreateABookingFromTo(string p0, string p1)
}
