using System;
using System.Web;
using Microsoft.Practices.Unity;

namespace UnityIocContainer
{
    public class HttpContextLifetimeManager : LifetimeManager, IDisposable
    {
        private readonly string itemName = Guid.NewGuid().ToString();

        public HttpContextLifetimeManager()
        {
            HttpContextLifetimeManagerController.AddManager(this);
        }

        public override object GetValue()
        {
            return HttpContext.Current.Items[this.itemName];
        }

        public override void RemoveValue()
        {
            var disposable = this.GetValue() as IDisposable;

            HttpContext.Current.Items.Remove(this.itemName);

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Items[this.itemName] = newValue;
        }

        public void Dispose()
        {
            this.RemoveValue();
        }
    }
}