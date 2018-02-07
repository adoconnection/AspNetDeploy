using System;
using System.Reflection;
using System.Threading;

namespace VsTestLibrary
{
    public class VsTestRunner
    {
        public VsTestRunResult Run(Type type, string initMethod, string testMethod, string cleanupMethod, Func<string, object> configurationProvider = null)
        {
            object instance;

            try
            {
                instance = this.CreateInstance(type, configurationProvider);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new VsTestLibraryException("unable to create new instance", e);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(initMethod))
                {
                    this.RunMethod(type, initMethod, instance);
                }

                this.RunMethod(type, testMethod, instance);

                if (!string.IsNullOrWhiteSpace(cleanupMethod))
                {
                    this.RunMethod(type, cleanupMethod, instance);
                }

                return new VsTestRunResult
                {
                    IsSuccess = true
                };
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (TargetInvocationException exception)
            {
                return new VsTestRunResult
                {
                    IsSuccess = false,
                    Exception = exception.InnerException is AggregateException 
                        ? exception.InnerException.InnerException
                        : exception.InnerException
                };
            }
            catch (Exception exception)
            {
                return new VsTestRunResult
                {
                    IsSuccess = false,
                    Exception = exception
                };
            }
        }

        private object CreateInstance(Type type, Func<string, object> configuration = null)
        {
            if (configuration != null)
            {
                foreach (ConstructorInfo constructorInfo in type.GetConstructors())
                {
                    ParameterInfo[] parameterInfos = constructorInfo.GetParameters();

                    if (parameterInfos.Length == 1 && parameterInfos[0].ParameterType == typeof(Func<string, object>))
                    {
                        return constructorInfo.Invoke(new object[] {configuration});
                    }
                }
            }

            return Activator.CreateInstance(type);
        }

        private void RunMethod(Type type, string initMethod, object instance)
        {
            MethodInfo methodInfo = type.GetMethod(initMethod);

            if (methodInfo == null)
            {
                throw new VsTestLibraryException("Method not found");
            }

            methodInfo.Invoke(instance, null);
        }
    }
}