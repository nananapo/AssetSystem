using System.Collections.Generic;

namespace AssetSystem.Reference
{
    public interface IPropertyReference
    {
        public string GetPropertyType();

        public IDictionary<string, string> GetProperties();
    }
}