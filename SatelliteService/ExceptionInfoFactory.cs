using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SatelliteService
{
    public class ExceptionInfoFactory
    {
        private static readonly IList<string> IgnoredProperties = new[]
            {
                    "Source", "Message", "HelpLink", "InnerException", "StackTrace", "Data"
            };

        public ExceptionInfo Create(Exception exception)
        {
            ExceptionInfo exceptionInfo = new ExceptionInfo()
            {
                TypeName = exception.GetType().FullName,
                AssemblyQualifiedTypeName = exception.GetType().AssemblyQualifiedName,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Source = exception.Source,
                ExceptionData = this.GetExceptionData(exception).Union(GetReflectionInfo(exception)).ToList()
            };

            return exceptionInfo;
        }

        private IEnumerable<ExceptionDataInfo> GetExceptionData(Exception exception)
        {
            return exception.Data
                    .Cast<DictionaryEntry>()
                    .Select(
                            entry => new ExceptionDataInfo
                            {
                                Name = entry.Key.ToString(),
                                Value = entry.Value != null ? entry.Value.ToString() : null
                            })
                    .ToList();
        }

        private IEnumerable<ExceptionDataInfo> GetReflectionInfo(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            IList<ExceptionDataInfo> result = new List<ExceptionDataInfo>();

            Type type = exception.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            object value;

            foreach (PropertyInfo property in properties)
            {
                if (!property.CanRead || IgnoredProperties.IndexOf(property.Name) != -1 || property.GetIndexParameters().Length != 0)
                {
                    continue;
                }

                try
                {
                    value = property.GetValue(exception, null);
                }
                catch (TargetInvocationException)
                {
                    value = string.Empty;
                }

                AddPropertyInfo(result, property.Name, value);
            }

            foreach (FieldInfo field in fields)
            {
                try
                {
                    value = field.GetValue(exception);
                }
                catch (TargetInvocationException)
                {
                    value = string.Empty;
                }

                AddPropertyInfo(result, field.Name, value);
            }

            return result;
        }

        private static void AddPropertyInfo(ICollection<ExceptionDataInfo> result, string name, object value)
        {
            result.Add(new ExceptionDataInfo
            {
                IsProperty = true,
                Name = name,
                Value = value != null ? value.ToString() : null

            });
        }
    }
}