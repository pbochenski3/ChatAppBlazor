using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppChat.Tests
{

    public class DbHelpers
    {
        private readonly ChatDbContext _context;
        public DbHelpers(ChatDbContext context)
        {
            _context = context;
        }
        public async Task ResetAllData()
        {
            var rawSql = @"
        -- Wyłącz wszystkie klucze obce
        EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

        -- Usuń dane ze wszystkich tabel
        EXEC sp_MSforeachtable 'DELETE FROM ?';

        -- Włącz klucze obce z powrotem
        EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';";

            await _context.Database.ExecuteSqlRawAsync(rawSql);
        }
    }
}
