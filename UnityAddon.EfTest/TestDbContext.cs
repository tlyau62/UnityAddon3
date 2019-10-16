using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UnityAddon.Attributes;

namespace UnityAddon.EfTest
{
    [Component]
    [Scope(ScopeType.Transient)]
    public class TestDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source=test.db");
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
