using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.EfTest.Common
{
    public class Item2
    {
        public Item2()
        {
        }

        public Item2(string name)
        {
            Name = name;
        }

        [Key]
        public long ItemId { get; set; }

        public string Name { get; set; }
    }

    public class TestDbContext2Configuration : IEntityTypeConfiguration<Item2>
    {
        public void Configure(EntityTypeBuilder<Item2> builder)
        {
            builder.HasKey(k => k.ItemId);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(128);
        }
    }

}
