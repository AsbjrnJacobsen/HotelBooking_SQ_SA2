using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Core;

namespace HotelBooking.UnitTests.Fakes
{
    public class FakeRoomRepository : IRepository<Room>
    {
        // This field is exposed so that a unit test can validate that the
        // Add method was invoked.
        public bool addWasCalled = false;
        private readonly List<Room> rooms;

        public FakeRoomRepository(bool returnEmpty = false)
        {
            rooms = returnEmpty ? new List<Room>() : new List<Room>
            {
                new Room { Id = 1 },
                new Room { Id = 2 },
                new Room { Id = 3 }
            };
        }
        
        public Task AddAsync(Room entity)
        {
            addWasCalled = true;
            return Task.CompletedTask;
        }

        // This field is exposed so that a unit test can validate that the
        // Edit method was invoked.
        public bool editWasCalled = false;

        public Task EditAsync(Room entity)
        {
            editWasCalled = true;
            return Task.CompletedTask;
        }

        public Task<Room> GetAsync(int id)
        {
            Task<Room> roomTask = Task.Factory.StartNew(() => new Room { Id = 1, Description = "A"} );
            return roomTask;
        }

        public Task<IEnumerable<Room>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Room>>(rooms);

        }

        // This field is exposed so that a unit test can validate that the
        // Remove method was invoked.
        public bool removeWasCalled = false;

        public Task RemoveAsync(int id)
        {
            removeWasCalled = true;
            return Task.CompletedTask;
        }
    }
}
