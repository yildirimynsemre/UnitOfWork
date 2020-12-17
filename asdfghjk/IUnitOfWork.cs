using System;
using System.Collections.Generic;
using System.Text;

namespace asdfghjk
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        int SaveChanges();
    }
}
