using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.Logging.DatabaseLogger
{
    public class DatabaseLoggingService : ILoggingService
    {
        private static readonly IList<string> IgnoredProperties = new[]
            {
                    "Source", "Message", "HelpLink", "InnerException", "StackTrace", "Data"
            };

        public void Log(Exception exception, int? userId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            AspNetDeployExceptionEntry aspNetDeployExceptionEntry = new AspNetDeployExceptionEntry();
            aspNetDeployExceptionEntry.TimeStamp = DateTime.UtcNow;
            aspNetDeployExceptionEntry.UserId = userId;
            aspNetDeployExceptionEntry.ExceptionEntry = this.CreateAndSaveExceptionEntryRecursive(entities, null, exception);
            entities.AspNetDeployExceptionEntry.Add(aspNetDeployExceptionEntry);

            entities.SaveChanges();
        }

        private ExceptionEntry CreateAndSaveExceptionEntryRecursive(AspNetDeployEntities entities, ExceptionEntry parentException, Exception exception)
        {
            ExceptionEntry exceptionEntry = new ExceptionEntry();
            exceptionEntry.Message = exception.Message;
            exceptionEntry.Source = exception.Source;
            exceptionEntry.StackTrace = exception.StackTrace;
            exceptionEntry.TypeName = exception.GetType().FullName;
            entities.ExceptionEntry.Add(exceptionEntry);

            if (parentException != null)
            {
                parentException.InnerExceptionEntry = exceptionEntry;
            }

            foreach (ExceptionEntryData entryData in this.GetExceptionData(exception).Union(GetReflectionInfo(exception)).ToList())
            {
                entryData.ExceptionEntry = exceptionEntry;
                entities.ExceptionEntryData.Add(entryData);
            }

            if (exception.InnerException != null)
            {
                this.CreateAndSaveExceptionEntryRecursive(entities, exceptionEntry, exception.InnerException);
            }

            return exceptionEntry;
        }

        private IEnumerable<ExceptionEntryData> GetExceptionData(Exception exception)
        {
            return exception.Data
                    .Cast<DictionaryEntry>()
                    .Select(
                            entry => new ExceptionEntryData
                            {
                                Name = entry.Key.ToString(),
                                Value = entry.Value != null ? entry.Value.ToString() : null
                            })
                    .ToList();
        }

        private IEnumerable<ExceptionEntryData> GetReflectionInfo(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            IList<ExceptionEntryData> result = new List<ExceptionEntryData>();

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

        private static void AddPropertyInfo(ICollection<ExceptionEntryData> result, string name, object value)
        {
            result.Add(new ExceptionEntryData
            {
                IsProperty = true,
                Name = name,
                Value = value != null ? value.ToString() : null

            });
        } 
    }
}