using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.EfTest
{
    [Configuration]
    public class DbConfig
    {
        [Bean]
        public virtual string ConnectionString()
        {
            return $"Data source={Guid.NewGuid()}.db";
        }
    }

    [Component]
    [Scope(ScopeType.Transient)]
    public class TestDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        [Dependency("ConnectionString")]
        public string ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TestDbContext).Assembly);
        }
    }

    public class Item
    {
        public Item()
        {
        }

        public Item(string name)
        {
            Name = name;
        }

        [Key]
        public long ItemId { get; set; }

        public string Name { get; set; }
    }

    public class TestDbContextConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(k => k.ItemId);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(128);
        }
    }

}
