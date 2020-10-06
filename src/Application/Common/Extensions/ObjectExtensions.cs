using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Application.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(
            this TSource source,
            string fields)
        {
            if (source == null)
            {
                throw new Exception("Source can not be null");
            }

            var returnObject = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>) returnObject).Add(
                        propertyInfo.Name,
                        propertyValue);
                }

                return returnObject;
            }

            foreach (var field in fields.Split(','))
            {
                var propertyName = field.Trim();

                var propertyInfo = typeof(TSource)
                    .GetProperty(
                        propertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    throw new Exception($"Property {propertyName} not found");
                }

                var propertyValue = propertyInfo.GetValue(source);

                ((IDictionary<string, object>) returnObject).Add(
                    propertyInfo.Name,
                    propertyValue);
            }

            return returnObject;
        }
    }
}