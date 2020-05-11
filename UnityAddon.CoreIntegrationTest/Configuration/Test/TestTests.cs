using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.CoreIntegrationTest.Configuration.Test
{
    [AttributeUsage(AttributeTargets.All)]
    public class ImportAttribute : Attribute
    {
        public Type[] ConfigTypes { get; set; }

        public ImportAttribute(params Type[] configTypes)
        {
            ConfigTypes = configTypes;
        }
    }

    [Configuration]
    [Import(typeof(RepoConfig))]
    public class GenericProfileRepositoryConfig // add this config somewhere in the unit test
    {
        // register the beans needed by repo config
        [Bean]
        public virtual IGenericProfileRepository GenericProfileRepository()
        {
            return new GenericProfileRepository();
        }
    }

    [Configuration]
    public class RepoConfig // general repo beans
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [OptionalDependency]
        public ObjectClassModel ObjectClassModel { get; set; }

        public virtual IBeanDefinitionCollection RepoBean()
        {
            if (ObjectClassModel == null)
                ObjectClassModel = new ObjectClassModel();

            IBeanDefinitionCollection defs = new BeanDefinitionCollection();

            defs.AddSingleton(ObjectClassModel);
            defs.AddTransient<DbEntities, DbEntities>();

            // ...

            return defs;
        }
    }

    internal class DbEntities
    {
    }

    public class ObjectClassModel
    {
    }

    

    public class GenericProfileRepository : IGenericProfileRepository
    {
    }

    public interface IGenericProfileRepository
    {
    }

    public class TestTests
    {

    }
}
