namespace ObjectFactory
{
    public interface ITypeResolver
    {
        T GetInstance<T>();
        void Initialize();
    }
}