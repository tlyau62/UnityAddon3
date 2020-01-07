using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Ef;

namespace UnityAddon.EfTest.Common
{
    public class DbSetupUtility
    {
        public static void CreateDb<T>(IDbContextFactory<T> dbContextFactory) where T : DbContext
        {
            if (!dbContextFactory.IsOpen())
            {
                dbContextFactory.Open();
            }

            dbContextFactory.Get().Database.EnsureCreated();
            dbContextFactory.Close();
        }

        public static void DropDb<T>(IDbContextFactory<T> dbContextFactory) where T : DbContext
        {
            if (!dbContextFactory.IsOpen())
            {
                dbContextFactory.Open();
            }

            dbContextFactory.Get().Database.EnsureDeleted();
            dbContextFactory.Close();
        }
    }
}
