using ChatApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Transactions;

namespace ChatApp.Infrastructure.Providers
{
    public class TransactionProvider : ITransactionProvider
    {
        private readonly TransactionOptions _options;
        public TransactionProvider()
        {
            _options = new TransactionOptions()
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.DefaultTimeout
            };
        }
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            _options,
            TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var result = await action();
                scope.Complete();
                return result;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await ExecuteAsync<bool>(async () =>
            {
                await action();
                return true; 
            });
        }
        }
}
