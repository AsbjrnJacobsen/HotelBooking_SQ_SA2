Feature: Create Booking
In order to reserve a hotel room
As a customer
I want to be able to create a valid booking

    Background:
        Given the hotel has no existing bookings

    Scenario Outline: Create booking with different date combinations
        When I create a booking <start> days from today to <end> days from today
        Then the booking should be <result>

        Examples:
          | start | end | result   |
          | 3     | 3   | created  |
          | 15    | 15  | created  |
          | 4     | 4   | rejected |
          | 14    | 14  | created  |