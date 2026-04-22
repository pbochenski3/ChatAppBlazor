using ChatApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContextFactory<ChatDbContext> _contextFactory;
        private ChatDbContext? _context;

        public UnitOfWork(IDbContextFactory<ChatDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ChatDbContext Context => _context
            ?? throw new InvalidOperationException("UnitOfWork not started. Call Begin() first.");

        public void Begin()
        {
            if (_context != null) return; 
            _context = _contextFactory.CreateDbContext();
        }

        public async Task CommitAsync()
        {
            if (_context != null)
            {
                await _context.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
            _context = null;
        }
    }
}
