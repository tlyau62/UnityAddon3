using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.BeanDefinition.MemberBean;
using UnityAddon.Core.BeanDefinition.ServiceBean;
using Unity.Lifetime;
using UnityAddon.Core.Bean;
using System.Collections;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinitionCollection : IList<IBeanDefinition>, IBeanRegistry
    {
        void AddRange(IEnumerable<IBeanDefinition> beanDefinitions);
    }

    public class BeanDefinitionCollection : BeanRegistry, IBeanDefinitionCollection
    {
        private readonly List<IBeanDefinition> _list = new List<IBeanDefinition>();

        public IBeanDefinition this[int index] { get => _list[index]; set => _list[index] = value; }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public override void Add(IBeanDefinition beanDefinition)
        {
            _list.Add(beanDefinition);
        }

        public void AddRange(IEnumerable<IBeanDefinition> beanDefinitions)
        {
            _list.AddRange(beanDefinitions);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(IBeanDefinition item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(IBeanDefinition[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IBeanDefinition> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(IBeanDefinition item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, IBeanDefinition item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(IBeanDefinition item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
