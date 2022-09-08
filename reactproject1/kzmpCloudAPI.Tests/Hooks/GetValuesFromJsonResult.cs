using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace kzmpCloudAPI.Tests.Hooks
{
    public static class GetValuesFromJsonResult
    {
        public static object GetReflectedProperty(this object obj, string propertyName)
        {
            if (obj == null || propertyName == null)
            {
                return null;
            }

            PropertyInfo? property = obj.GetType().GetProperty(propertyName);

            if (property == null)
            {
                return null;
            }

            return property.GetValue(obj, null);
        }
    }
}
