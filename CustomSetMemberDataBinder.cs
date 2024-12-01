using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridDynamicTest
{
    // Helper class to create SetMemberBinder instances
    public class CustomSetMemberBinder : SetMemberBinder
    {
        public CustomSetMemberBinder(string name) : base(name, ignoreCase: false) { }

        public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
        {
            throw new NotImplementedException();
        }
    }
}
