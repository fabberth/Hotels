using Hotels.Domain;

namespace Hotels.Services
{
    public interface IRoomRepository : IRepository<Room>
    {
        public IQueryable<Room> GetAllByHotel(Hotel hotel);
    }
}
