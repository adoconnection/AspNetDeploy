namespace ObjectFactory
{
    public static class Factory
    {
        private static ITypeResolver typeResolver;

        public static T GetInstance<T>()
        {
            return typeResolver.GetInstance<T>();
        }

        public static void SetTypeResolver(ITypeResolver resolver)
        {
            typeResolver = resolver;
            typeResolver.Initialize();
        }
    }
}