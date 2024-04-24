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
            return All().FirstOrDefault(x => x.Code == id);
        }

        public IQueryable<Room> GetAllByHotel(Hotel hotel)
        {
            return All().Where(x => x.IdHotel == hotel.IdHotel);
        }

        public void Update(Room entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
    }
}
