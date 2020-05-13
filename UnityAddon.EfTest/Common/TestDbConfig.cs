using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Context;
using UnityAddon.Ef.Transaction;

namespace UnityAddon.EfTest.Common
{
    public class TestDbConfig<TDbContext> where TDbContext : DbContext
    {
        private readonly string _uuidContext = Guid.NewGuid().ToString();

        [Bean]
        [Scope(ScopeType.Transient)]
        public virtual TDbContext TestDbContext()
        {
            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), new[] { $"Data source={_uuidContext}.db" });
        }
    }

    public abstract class AbstractDbContext : DbContext
    {
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
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }

    public class TestDbContext : AbstractDbContext
    {
        public DbSet<Item> Items { get; set; }

        public TestDbContext(string connectionString) : base(connectionString)
        {
        }
    }

    public class TestDbContext2 : AbstractDbContext
    {
        public DbSet<Item2> Items { get; set; }

        public TestDbContext2(string connectionString) : base(connectionString)
        {
        }
    }
}
