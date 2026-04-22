using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Begin();
        Task CommitAsync();
    }
}
