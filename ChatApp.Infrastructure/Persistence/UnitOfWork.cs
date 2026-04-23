using ChatApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatDbContext _context;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(ChatDbContext context) => _context = context;

        public async Task<int> SaveChangesAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct)
            => _currentTransaction = await _context.Database.BeginTransactionAsync(ct);

        public async Task CommitTransactionAsync(CancellationToken ct)
        {
            await _currentTransaction?.CommitAsync(ct)!;
        }

        public async Task RollbackTransactionAsync(CancellationToken ct)
        {
            if (_currentTransaction != null) await _currentTransaction.RollbackAsync(ct);
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
        }
    }
}
