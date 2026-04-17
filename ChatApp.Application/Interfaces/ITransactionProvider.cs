using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface ITransactionProvider
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
