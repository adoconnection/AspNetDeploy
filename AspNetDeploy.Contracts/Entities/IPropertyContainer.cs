using System;

namespace AspNetDeploy.Contracts.Entities
{
    public interface IPropertyContainer
    {
        string GetPropertyString(string key, string defaultValue = null);
        int? GetPropertyInt(string key, int? defaultValue = null);
        DateTime? GetPropertyDateTime(string key, DateTime? defaultValue = null);
    }
}