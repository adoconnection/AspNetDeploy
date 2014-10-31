using System.Collections.Generic;

namespace UnityIocContainer
{
    public static class HttpContextLifetimeManagerController
    {
        private static readonly IList<HttpContextLifetimeManager> managers = new List<HttpContextLifetimeManager>();

        internal static void AddManager(HttpContextLifetimeManager manager)
        {
            managers.Add(manager);
        }

        public static void DoEndRequest()
        {
            foreach (var manager in managers)
            {
                manager.RemoveValue();
            }
        }
    }
}