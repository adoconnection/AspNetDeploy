using ObjectFactory;
using UnityIocContainer;

namespace InnovativeManagementSystems.UnityIocContainer
{
    using Microsoft.Practices.Unity;

    public abstract class UnityTypeResolver : ITypeResolver
    {
        protected IUnityContainer Container { get; private set; }
        
        protected UnityTypeResolver()
        {
            Container = new UnityContainer();
        }

        public T GetInstance<T>()
        {
            return Container.Resolve<T>();
        }

        public abstract void Initialize();

        protected void MapRegistry(Registry registry)
        {
            foreach (var item in registry.GetTypeMappings())
            {
                this.AddRegistryItemToContainer(item.Value);
            }

            foreach (var item in registry.GetTypeFunctionMappings())
            {
                this.Container.RegisterInstance(item.Key, item.Value());
            }
        }

        private void AddRegistryItemToContainer(RegistryItem item)
        {
            switch (item.Lifecycle)
            {
                case LifecycleType.Application:
                    this.Container.RegisterType(item.FromType, item.ToType, new ContainerControlledLifetimeManager());
                    break;

                case LifecycleType.HttpContext:
                    this.Container.RegisterType(item.FromType, item.ToType, new HttpContextLifetimeManager());
                    break;

                default:
                    this.Container.RegisterType(item.FromType, item.ToType);
                    break;
            }
        }
    }
}