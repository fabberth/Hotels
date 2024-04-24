using Hotels.AppDbContexts;
using Hotels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hotels.Services
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext dbContext;
        public HotelRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public void Add(Hotel entity)
        {
            dbContext.Hotels.Add(entity);
            dbContext.SaveChanges();
        }

        public IQueryable<Hotel> All()
        {
            return dbContext.Hotels.AsQueryable();
        }

        public void Delete(Hotel entity)
        {
            dbContext.Hotels.Remove(entity);
            dbContext.SaveChanges();
        }

        public Hotel Get(string Id)
        {
            if(int.TryParse(Id, out int IdHotel))
                return All().FirstOrDefault(x => x.IdHotel == IdHotel);
            return All().FirstOrDefault(x=> x.Code == Id);
        }

        public void Update(Hotel entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
    }
}
