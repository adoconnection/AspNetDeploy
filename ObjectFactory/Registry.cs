using System;
using System.Collections.Generic;

namespace ObjectFactory
{
    public class Registry
    {
        private readonly IDictionary<Type, RegistryItem> typeMapping;
        private readonly IDictionary<Type, Func<object>> typeFunctionMapping;

        public Registry()
        {
            this.typeMapping = new Dictionary<Type, RegistryItem>();
            this.typeFunctionMapping = new Dictionary<Type, Func<object>>();
        }

        public void Map<T1, T2>(LifecycleType lifecycleType = LifecycleType.AlwaysNew)
        {
            this.typeMapping.Add(typeof(T1), new RegistryItem { FromType = typeof(T1), ToType = typeof(T2), Lifecycle = lifecycleType });
        }
        public void Map<T1>(Func<object> objectProvider)
        {
            this.typeFunctionMapping.Add(typeof(T1), objectProvider);
        }

        public IDictionary<Type, RegistryItem> GetTypeMappings()
        {
            return this.typeMapping;
        }
        public IDictionary<Type, Func<object>> GetTypeFunctionMappings()
        {
            return this.typeFunctionMapping;
        }
    }
}