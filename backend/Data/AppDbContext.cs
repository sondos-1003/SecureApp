using Microsoft.EntityFrameworkCore;
using SecureWebApp.backend.Models;
using SecureWebApp.Models;
using System.Collections.Generic;

namespace SecureWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserMfaSecret> UserMfaSecrets => Set<UserMfaSecret>();

    }
}