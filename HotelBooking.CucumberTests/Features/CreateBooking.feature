Feature: Create Booking
As a hotel guest
I want to book a room for a valid period
So that I can reserve my stay in advance

    Background:
        Given the hotel has no existing bookings

    @valid
    Scenario: Booking with valid start and end dates
        # today + 4 to today + 9
        When I create a booking from "05/04/2025" to "10/04/2025"
        Then the booking should be created successfully

    @invalid_start
    Scenario: Booking with start date today
        # today to today + 9
        When I create a booking from "01/04/2025" to "10/04/2025"
        Then the booking should be rejected

    @invalid_start
    Scenario: Booking with start date in the past
        # today - 1 to today + 9
        When I create a booking from "31/03/2025" to "10/04/2025"
        Then the booking should be rejected

    @invalid_end
    Scenario: Booking with end date before start date
        # today + 4 to today + 3
        When I create a booking from "05/04/2025" to "04/04/2025"
        Then the booking should be rejected

    @invalid_end
    Scenario: Booking with end date equal to start date
        # today + 4 to today + 4
        When I create a booking from "05/04/2025" to "05/04/2025"
        Then the booking should be rejected

    @conflict
    Scenario: Booking overlaps with an existing booking
        Given the hotel already has a booking from "10/04/2025" to "15/04/2025"
        When I create a booking from "09/04/2025" to "12/04/2025"
        Then the booking should be rejected