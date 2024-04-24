using Hotels.Domain;

namespace Hotels.Services
{
    public interface IRoomRepository : IRepository<Room>
    {
        public IQueryable<Room> GetAllByHotel(Hotel hotel);
        public IQueryable<Reserve> GetReservationsByRoom(Room habitacion);
        public IQueryable<Reserve> GetAllReservations();
        public Reserve GetReservation(string id);
        public void UpdateReservation(Reserve reserve);
        public void AddReservation(Reserve reserve);
        public IQueryable<Room> GetRoomsByReservationDates(DateTime DateOfAdmission, DateTime DateOfExit);
    }
}
