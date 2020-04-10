using Eiscp.Core.Model;
using System.Collections;
using System.Collections.Specialized;

namespace Eiscp.Core.Commands
{
    /// <summary>
    /// Master Volume Command
    /// </summary>
    public class MVLCommand : UpDownCommand
    {
        /// <summary>
        /// Key
        /// </summary>
        public override string Key => "MVL";
        /// <summary>
        /// Value
        /// </summary>
        public override OrderedDictionary Value => 
            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
            {
                {
                    "name",
                    "master-volume"
                },
                {
                    "description",
                    "Master Volume Command"
                },
                {
                    "values",
                    new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                    {
                        {
                            EiscpTemplates.GetKey("2E"),
                            EiscpTemplates.GetValue( "46", "Volume")
                        },
                               {
                            EiscpTemplates.GetKey("31"),
                            EiscpTemplates.GetValue( "49", "Volume")
                        },
                        {
                            EiscpTemplates.GetKey("32"),
                            EiscpTemplates.GetValue( "50", "Volume")
                        },
                        {
                            EiscpTemplates.GetKey("33"),
                            EiscpTemplates.GetValue( "51", "Volume")
                        },
                        //{
                        //    new object[]
                        //    {
                        //        "0",
                        //        "100"
                        //    },
                        //    new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        //    {
                        //        {
                        //           "name",
                        //            null
                        //        },
                        //        {
                        //            "description",
                        //            "Volume Level 0 – 100 ( In hexadecimal representation)"
                        //        }
                        //    }
                        //},
                        //{
                        //    new object[]
                        //    {
                        //        "0",
                        //        "80"
                        //    },
                        //    new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        //    {
                        //        {
                        //            "name",
                        //            null
                        //        },
                        //        {
                        //            "description",
                        //            "Volume Level 0 – 80 ( In hexadecimal representation)"
                        //        }
                        //    }
                        //},
                        {
                            "UP",
                            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                            {
                                {
                                    "name",
                                    "level-up"
                                },
                                {
                                    "description",
                                    "sets Volume Level Up"
                                }
                            }
                        },
                        {
                            "DOWN",
                            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                            {
                                {
                                    "name",
                                    "level-down"
                                },
                                {
                                    "description",
                                    "sets Volume Level Down"
                                }
                            }
                        },
                        {
                            "UP1",
                            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                            {
                                {
                                    "name",
                                    "level-up-1db-step"
                                },
                                {
                                    "description",
                                    "sets Volume Level Up 1dB Step"
                                }
                            }
                        },
                        {
                            "DOWN1",
                            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                            {
                                {
                                    "name",
                                    "level-down-1db-step"
                                },
                                {
                                    "description",
                                    "sets Volume Level Down 1dB Step"
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
                                    "gets the Volume Level"
                                }
                            }
                        }
                    }
                }
            };

        /// <summary>
        /// Up 1
        /// </summary>
        public string Up1 => GetCommandString("UP1");
        /// <summary>
        /// Down 1
        /// </summary>
        public string Down1 => GetCommandString("DOWN1");

        /// <summary>
        /// Set Volume
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public string SetVolume(int volume)
        {
            var res = new Response($"{Key}{volume}", Value);
            return $"{res.Key}{res.ToHex()}";
        }
    }
}
