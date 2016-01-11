using System;
using System.Threading;

namespace Exceptions
{
    public static class ExceptionExtender
    {
        public static bool IsCritical(this Exception exception)
        {
            if (exception is ThreadAbortException)
            {
                return true;
            }

            if (exception is OutOfMemoryException)
            {
                return true;
            }

            if (exception is AppDomainUnloadedException)
            {
                return true;
            }

            if (exception is BadImageFormatException)
            {
                return true;
            }

            if (exception is CannotUnloadAppDomainException)
            {
                return true;
            }

            if (exception is InvalidProgramException)
            {
                return true;
            }

            return false;
        }
    }
}