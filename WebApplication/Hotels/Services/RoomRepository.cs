using Hotels.AppDbContexts;
using Hotels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hotels.Services
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext dbContext;
        public RoomRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public void Add(Room entity)
        {
            dbContext.Rooms.Add(entity);
            dbContext.SaveChanges();
        }

        public void AddReservation(Reserve reserve)
        {
            dbContext.Reserves.Add(reserve);
            dbContext.SaveChanges();
        }

        public IQueryable<Room> All()
        {
            return dbContext.Rooms.AsQueryable();
        }

        public void Delete(Room entity)
        {
            dbContext.Rooms.Remove(entity);
            dbContext.SaveChanges();
        }

        public Room Get(string id)
        {
            if (int.TryParse(id, out var IdBD))
                return All().FirstOrDefault(x => x.IdBD == IdBD);

            return All().FirstOrDefault(x => x.Code == id);
        }

        public IQueryable<Room> GetAllByHotel(Hotel hotel)
        {
            return All().Where(x => x.IdHotel == hotel.IdHotel);
        }

        public IQueryable<Reserve> GetAllReservations()
        {
            return dbContext.Reserves.AsQueryable();
        }

        public Reserve GetReservation(string id)
        {
            if(int.TryParse(id, out var reservationId))
                return dbContext.Reserves.FirstOrDefault(x => x.IdReserve == reservationId);

            return dbContext.Reserves.FirstOrDefault(x=> x.Code == id);
        }

        public IQueryable<Reserve> GetReservationsByRoom(Room habitacion)
        {
            return dbContext.Reserves.AsQueryable().Where(x=> x.Room == habitacion);
        }

        public void Update(Room entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public void UpdateReservation(Reserve reserve)
        {
            dbContext.Entry(reserve).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public IQueryable<Room> GetRoomsByReservationDates(DateTime DateOfAdmission, DateTime DateOfExit)
        {
            var reservations = GetAllReservations().Where(r => (DateOfAdmission >= r.DateOfAdmission && DateOfAdmission <= r.DateOfExit) ||
                             (DateOfExit >= r.DateOfAdmission && DateOfExit <= r.DateOfExit) ||
                             (r.DateOfAdmission >= DateOfAdmission && r.DateOfAdmission <= DateOfExit) ||
                             (r.DateOfExit >= DateOfAdmission && r.DateOfExit <= DateOfExit));


            return reservations.Select(x => x.Room);
        }
    }
}
