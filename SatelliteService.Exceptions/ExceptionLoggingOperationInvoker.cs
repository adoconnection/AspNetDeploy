using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteService.Exceptions
{
    public class ExceptionLoggingOperationInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker baseInvoker;
        private readonly OperationDescription operationDescription;

        public ExceptionLoggingOperationInvoker(IOperationInvoker baseInvoker, OperationDescription operationDescription)
        {
            this.baseInvoker = baseInvoker;
            this.operationDescription = operationDescription;
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            try
            {
                return baseInvoker.Invoke(instance, inputs, out outputs);
            }
            catch (Exception exception)
            {
                exception.Data.Add("Invoked Method", this.operationDescription.Name);

                IEnumerable<KeyValuePair<string, object>> parameters = this.operationDescription.SyncMethod.GetParameters().Zip(inputs, (parameterInfo, value) => new KeyValuePair<string, object>(parameterInfo.Name, value));

                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    if (parameter.Value != null && !parameter.Value.GetType().IsSerializable)
                    {
                        exception.Data.Add(parameter.Key, "N/A");
                        continue;
                    }

                    exception.Data.Add(parameter.Key, parameter.Value);
                }

                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);

                Exception innerException = exception.InnerException;

                while (innerException != null)
                {
                    Console.WriteLine("Inner exception");
                    Console.WriteLine(exception.Message);
                    Console.WriteLine(exception.StackTrace);

                    innerException = innerException.InnerException;
                }

                throw;
            }
        }

        public object[] AllocateInputs()
        {
            return this.baseInvoker.AllocateInputs();
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return this.baseInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return this.baseInvoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get
            {
                return this.baseInvoker.IsSynchronous;
            }
        }
    }
}