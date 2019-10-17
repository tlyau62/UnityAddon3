using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Test
{
    //[Component]
    //[Scope(ScopeType.Transient)]
    //public class TestDbContext : DbContext
    //{
    //    public DbSet<Item> Items { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        optionsBuilder.UseSqlite("Data source=test.db");
    //    }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TestDbContext).Assembly);
    //    }
    //}

    //public class Item
    //{
    //    public Item()
    //    {
    //    }

    //    public Item(string name)
    //    {
    //        Name = name;
    //    }

    //    [Key]
    //    public long ItemId { get; set; }

    //    public string Name { get; set; }
    //}

    //public class TestDbContextConfiguration : IEntityTypeConfiguration<Item>
    //{
    //    public void Configure(EntityTypeBuilder<Item> builder)
    //    {
    //        builder.HasKey(k => k.ItemId);
    //        builder.Property(p => p.Name).IsRequired().HasMaxLength(128);
    //    }
    //}

    //public interface IService
    //{
    //    int CountItems();
    //    void InsertItem(Item item);
    //}

    //[Component]
    //public class Service : IService
    //{
    //    [Dependency]
    //    public IDbContextTemplate<TestDbContext> DbContextTemplate { get; set; }

    //    private DbSet<Item> _item => DbContextTemplate.GetEntity<Item>();

    //    [RequireDbContext]
    //    public int CountItems()
    //    {
    //        return _item.Count();
    //    }

    //    [RequireDbContext(Transactional = true)]
    //    public void InsertItem(Item item)
    //    {
    //        _item.Add(item);
    //    }
    //}

    class Program
    {
        static void Main(string[] args)
        {
            //var container = new UnityContainer();
            //var appContext = new ApplicationContext(container);

            //var db = appContext.Resolve<IDbContextFactory>();

            //db.Open().Database.EnsureCreated();
            //db.Close();

            //var a = appContext.Resolve<IService>();

            //a.InsertItem(new Item("test"));

            //var b = a.CountItems();

            //db.Open().Database.EnsureDeleted();
            //db.Close();

            //Console.WriteLine("Hello World!");
        }
    }
}
