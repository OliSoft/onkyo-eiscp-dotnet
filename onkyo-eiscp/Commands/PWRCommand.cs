using System.Collections;
using System.Collections.Specialized;

namespace Eiscp.Core.Commands
{
    public class PWRCommand : OnOffCommand
    {
        public override string Key => "PWR";
        public override OrderedDictionary Value =>
            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
            {
                {
                    "name",
                    "system-power"
                },
                {
                    "description",
                    "System Power Command"

                },
                {
                    "values",
                    new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                    {
                        {
                           "00",
                            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                            {
                                {
                                    "name",
                                    "standby"
                                },
                                {
                                    "description",
                                    "sets System Standby"
                                }
                            }
                        },
                        {
                            "01",
                            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                            {
                                {
                                   "name",
                                    "on"
                                },
                                {
                                    "description",
                                    "sets System On"
                                }
                            }
                        },
                        {
                            "QSTN",
                            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                            {
                                {
                                    "name",
                                    "query"
                                },
                                {
                                    "description",
                                    "gets the System Power Status"
                                }
                            }
                        }
                    }
                }
            };
    }
}
