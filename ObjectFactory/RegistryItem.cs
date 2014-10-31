using System;

namespace ObjectFactory
{
    public class RegistryItem
    {
        public Type FromType { get; set; }
        public Type ToType { get; set; }
        public LifecycleType Lifecycle { get; set; }
    }
}