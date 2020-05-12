using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;

namespace UnityAddon.EfTest.Common
{
    [Configuration]
    public class TestDbConfig
    {
        [Bean]
        [Scope(ScopeType.Transient)]
        public virtual TestDbContext TestDbContext()
        {
            return new TestDbContext($"Data source={Guid.NewGuid()}.db");
        }

        [Bean]
        [Scope(ScopeType.Transient)]
        public virtual DbContext TestDbContextPrimary()
        {
            return TestDbContext();
        }

        [Bean]
        [Scope(ScopeType.Transient)]
        public virtual TestDbContext2 TestDbContext2()
        {
            return new TestDbContext2($"Data source={Guid.NewGuid()}.db");
        }
    }

    public abstract class AbstractDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        private readonly string _connectionString;

        public AbstractDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TestDbContext).Assembly);
        }
    }

    public class TestDbContext : AbstractDbContext
    {
        public TestDbContext(string connectionString) : base(connectionString)
        {
        }
    }

    public class TestDbContext2 : AbstractDbContext
    {
        public TestDbContext2(string connectionString) : base(connectionString)
        {
        }
    }
}
