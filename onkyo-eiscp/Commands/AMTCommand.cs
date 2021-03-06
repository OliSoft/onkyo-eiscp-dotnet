﻿using System.Collections;
using System.Collections.Specialized;

namespace Eiscp.Core.Commands
{
    /// <summary>
    /// Audio Muting Command
    /// </summary>
    public class AMTCommand : OnOffCommand
    {
        /// <summary>
        /// Key
        /// </summary>
        public override string Key => "AMT";
        /// <summary>
        /// Value
        /// </summary>
        public override OrderedDictionary Value =>
            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
            {
                {
                    "name",
                    "audio-muting"
                },
                {
                    "description",
                    "Audio Muting Command"
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
                                    "off"
                                },
                                {
                                    "description",
                                    "sets Audio Muting Off"
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
                                    "sets Audio Muting On"
                                }
                            }
                        },
                        {
                            "TG",
                            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                            {
                                {
                                    "name",
                                    "toggle"
                                },
                                {
                                    "description",
                                    "sets Audio Muting Wrap-Around"
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
                                    "gets the Audio Muting State"
                                }
                            }
                        }
                    }
                }
            };

        /// <summary>
        /// Toggle
        /// </summary>
        public string Toggle => GetCommandString("TG");
    }
}
