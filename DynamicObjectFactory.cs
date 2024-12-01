using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridDynamicTest
{
    public class DynamicObjectFactory
    {
        public static List<dynamic> CreateDynamicObjects(Dictionary<string, List<object>> propertyDict)
        {
            if (propertyDict == null || propertyDict.Count == 0)
                throw new ArgumentException("Property dictionary cannot be null or empty.");

            int objectCount = propertyDict.First().Value.Count;
            if (propertyDict.Values.Any(list => list.Count != objectCount))
                throw new ArgumentException("All lists in the dictionary must have the same number of items.");

            List<dynamic> dynamicObjects = new List<dynamic>();

            for (int i = 0; i < objectCount; i++)
            {
                dynamic obj = new DynamicProperty();

                foreach (var kvp in propertyDict)
                {
                    obj.TrySetMember(new CustomSetMemberBinder(kvp.Key), kvp.Value[i]);
                }

                dynamicObjects.Add(obj);
            }

            return dynamicObjects;
        }
    }
}
