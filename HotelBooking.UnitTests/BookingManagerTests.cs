using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private readonly ITestOutputHelper _output;
        private IBookingManager bookingManager;
        IRepository<Booking> bookingRepository;

        public BookingManagerTests(ITestOutputHelper output){
            _output = output;
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository, _output);
        }

        

        // TC1
        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = await bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }
        
        // TC2
        [Fact]
        public async Task FindAvailableRoom_NoAvailableRoomDueToConflicts_ReturnsMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            DateTime end = DateTime.Today.AddDays(2);
            var bookingRepo = new FakeBookingRepository(date, end);
            var roomRepo = new FakeRoomRepository();
            
            // Act
            
            foreach (var roomId in new[] {1, 2, 3})
            {
                await bookingRepo.AddAsync(new Booking
                {
                    RoomId = roomId,
                    StartDate = date,
                    EndDate = end,
                    IsActive = true
                });
                _output.WriteLine($"Room {roomId} has been removed from the booking repository.");
            }
            
            var manager = new BookingManager(bookingRepo, roomRepo, _output);
            int roomIdResult = await manager.FindAvailableRoom(date, end);
            
            // Assert
            Assert.Equal(-1, roomIdResult);
        }
        
        // TC3
        [Fact]
        public async Task FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Task Result() => bookingManager.FindAvailableRoom(date, date);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(Result);
        }
        
        // TC4
        [Fact]
        public async Task FindAvailableRoom_NoRoomsInRepository_ReturnsMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            DateTime end = DateTime.Today.AddDays(2);
            // Create Fake Repository
            var emptyRoomRepo = new FakeRoomRepository(returnEmpty: true);
            var bookingRepo = new FakeBookingRepository(date, end);
            
            var manager = new BookingManager(bookingRepo, emptyRoomRepo, _output);
            
            // Act
            int result = await manager.FindAvailableRoom(date, end);

            // Assert
            Assert.Equal(-1, result);
        }
        
        // TC5a
        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            DateTime end = DateTime.Today.AddDays(2);

            var bookingRepo = new FakeBookingRepository(date, end);
            var roomRepo = new FakeRoomRepository();
            
            // Manually book room 1 + 2
            await bookingRepo.AddAsync(new Booking
            {
                RoomId = 1,
                StartDate = date,
                EndDate = end,
                IsActive = true
            });

            await bookingRepo.AddAsync(new Booking
            {
                RoomId = 2,
                StartDate = date,
                EndDate = end,
                IsActive = true
            });
            
            var manager = new BookingManager(bookingRepo, roomRepo, _output);
            // Act
            int roomId = await manager.FindAvailableRoom(date, end);

            
            // Assert
            Assert.Equal(3, roomId); 
        }

        //TC5b
        [Fact]
        public async Task FindAvailableRoom_ReturnedRoomHasNoConflictingBookings()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            DateTime end = DateTime.Today.AddDays(2);
            
            var bookingRepo = new FakeBookingRepository(date, end);
            var roomRepo = new FakeRoomRepository();
            var manager = new BookingManager(bookingRepo, roomRepo,_output);
            
            // Act
            int roomId = await manager.FindAvailableRoom(date, end);

            var bookingForReturnedRoomId = (await bookingRepo.GetAllAsync())
                .Where(b => b.RoomId == roomId
                            && b.StartDate == date
                            && b.EndDate == end
                            && b.IsActive);
            // Assert
            Assert.Empty(bookingForReturnedRoomId);
        }
    }
}
