using System.Collections;
using System.Collections.Specialized;

namespace Eiscp.Core.Commands
{
    public static class EiscpTemplates
    {
        public static object GetKey(object key) => key;
        public static object GetValue(object name, string description) =>
            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                {
                    {
                        "name",
                        name
                    },
                    {
                        "description",
                        description
                    }
                };
    }
}
