using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataGridDynamicTest
{
    public static class DynamicFactory
    {
        public static Dictionary<string, List<object>> TransformToDict<T>(List<T> items)
        {
            if (items == null || !items.Any())
                return new Dictionary<string, List<object>>();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return properties.ToDictionary(
                prop => prop.Name,
                prop => items.Select(item => prop.GetValue(item)).ToList()
            );
        }

        public static List<DynamicModelWrapper> CreateDynamicObjects<T>(List<T> items)
        {
            return items.Select(item => new DynamicModelWrapper(item)).Cast<DynamicModelWrapper>().ToList();
        }
    }
}
