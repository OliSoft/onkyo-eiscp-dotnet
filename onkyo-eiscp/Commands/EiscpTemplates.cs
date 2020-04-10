using System.Collections;
using System.Collections.Specialized;

namespace Eiscp.Core.Commands
{
    /// <summary>
    /// Template
    /// </summary>
    public static class EiscpTemplates
    {
        /// <summary>
        /// Get key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetKey(object key) => key;
        /// <summary>
        /// Get value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
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
