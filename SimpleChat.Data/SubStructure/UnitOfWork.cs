using Microsoft.Extensions.Logging;
using SimpleChat.Core.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Data.SubStructure
{
    public class UnitOfWork : IDisposable
    {
        private SimpleChatDbContext _con;
        private bool disposed = false;
        private Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public UnitOfWork(SimpleChatDbContext con)
        {
            _con = con;
        }

        public IRepository<D> Repository<D>()
            where D : BaseEntity, IBaseEntity, new()
        {
            if (repositories.Keys.Contains(typeof(D)) == true)
            {
                return repositories[typeof(D)] as IRepository<D>;
            }

            IRepository<D> repository = new Repository<D>(_con);
            repositories.Add(typeof(D), repository);
            return repository;
        }
        public virtual async Task<int> SaveChanges()
        {
            return await _con.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _con.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>();
        Task<int> SaveChanges();
        void Dispose(bool disposing);
        void Dispose();
    }
}
