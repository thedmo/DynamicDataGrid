using System.Reflection;

namespace DataGridDynamicTest
{
    public class ObjectsToDictionaryConverter
    {
        public static Dictionary<string, List<object?>> TransformToDict<T>(List<T> items)
        {
            if (items == null || !items.Any())
                return new Dictionary<string, List<object?>>();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return properties.ToDictionary(
                prop => prop.Name,
                prop => items.Select(item => prop.GetValue(item)).ToList()
            );
        }
    }
}
