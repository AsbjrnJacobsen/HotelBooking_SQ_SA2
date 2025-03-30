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
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:5000"); 
    }

    [Given(@"the hotel has no existing bookings")]
    public async Task GivenTheHotelHasNoExistingBookings()
    {
        var bookings = await _client.GetAsync("/bookings");
        var content = await bookings.Content.ReadAsStringAsync();
        var bookingList = JsonConvert.DeserializeObject<List<dynamic>>(content);

        foreach (var booking in bookingList)
        {
            int id = booking.id;
            await _client.DeleteAsync($"/bookings/{id}");
        }
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

    [When(@"I create a booking from ""(.*)"" to ""(.*)""")]
    public async Task WhenICreateABookingFromTo(string start, string end)
    {
        var booking = new
        {
            StartDate = DateTime.ParseExact(start, "dd/MM/yyyy", null),
            EndDate = DateTime.ParseExact(end, "dd/MM/yyyy", null),
            CustomerId = 1
        };

        var content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");
        _response = await _client.PostAsync("/bookings", content);
    }

    [Then(@"the booking should be created successfully")]
    public void ThenTheBookingShouldBeCreatedSuccessfully()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Then(@"the booking should be rejected")]
    public void ThenTheBookingShouldBeRejected()
    {
        _response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
    }
}
