using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Cache;
using UnityAddon.Core.Attributes;

namespace UnityAddon.CacheTest
{
    public class MyEntity
    {
        public int ID { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
    }

    public interface IMyRepository
    {
        MyEntity GetByID(int id);
        MyEntity GetByFullName(string @namespace, string name);
    }

    [Component]
    public class MyRepository : IMyRepository
    {
        public static int GetByIDCount = 0;
        public static int GetByFullNameCount = 0;

        [Cachable]
        public MyEntity GetByID(int id)
        {
            GetByIDCount++;
            if (id == 1)
                return new MyEntity
                {
                    ID = 1,
                    Namespace = "n",
                    Name = "x"
                };
            else if (id == 2)
                return new MyEntity
                {
                    ID = 2,
                    Namespace = "m",
                    Name = "s"
                };
            else
                return null;
        }

        [Cachable]
        public MyEntity GetByFullName(string @namespace, string name)
        {
            GetByFullNameCount++;
            if (@namespace == "n" && name == "x")
                return new MyEntity
                {
                    ID = 1,
                    Namespace = "n",
                    Name = "x"
                };
            else if (@namespace == "m" && name == "s")
                return new MyEntity
                {
                    ID = 2,
                    Namespace = "m",
                    Name = "s"
                };
            else
                return null;
        }
    }

    public interface IMyService
    {
        void Update();
        void DoSomething();
    }

    [Component]
    public class MyService : IMyService
    {
        [InvalidateCache(typeof(MyRepository))]
        public void Update()
        {
        }

        [InvalidateCache(typeof(MyService))]
        public void DoSomething()
        {
        }
    }
}
