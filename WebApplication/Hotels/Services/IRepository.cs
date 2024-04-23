namespace Hotels.Services
{
    public interface IRepository<T>
    {
        IQueryable<T> All();
        T Get(string id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
