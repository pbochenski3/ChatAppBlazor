using ChatApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(ChatDbContext context,ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct)
        {
            if (_context.Database.CurrentTransaction != null) return;

            _currentTransaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct)
        {
            await _currentTransaction?.CommitAsync(ct)!;
        }

        public async Task RollbackTransactionAsync(CancellationToken ct)
        {
            if (_currentTransaction == null) return;

            try
            {
                if (_currentTransaction.GetDbTransaction().Connection != null)
                {
                    await _currentTransaction.RollbackAsync(ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wycofywania transakcji");
            }
            finally
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
        }
    }
}
