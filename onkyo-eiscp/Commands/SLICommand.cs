using Eiscp.Core.Helper;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace Eiscp.Core.Commands
{
    /// <summary>
    /// Input Selector Command 
    /// </summary>
    public class SLICommand : UpDownCommand
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Test ???
        /// </summary>
        public int Test { get; }
        /// <summary>
        /// New Input Selector Command
        /// </summary>
        public SLICommand() : this(string.Empty)
        {}
        /// <summary>
        /// New Input Selector Command
        /// </summary>
        /// <param name="name"></param>
        public SLICommand(string name)
        {
            Name = name;
            if (string.IsNullOrEmpty(Name))
                return;

            var e = Enumerable
                .Range(0, 100)
                .Where(i => name.Equals(Utils.Nav(Value, "values", string.Format("{0:D2}", i), "name")));
            Test = e.FirstOrDefault();
        }
        /// <summary>
        /// Key
        /// </summary>
        public override string Key => "SLI";
        /// <summary>
        /// Value
        /// </summary>
        public override OrderedDictionary Value =>
            new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
            {
                {
                    "name",
                    "input-selector"
                },
                {
                    "description",
                    "Input Selector Command"
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
                                new object[]
                                {
                                    "video1",
                                    "vcr",
                                    "dvr"
                                }
                            },
                            {
                                "description",
                                "sets VIDEO1, VCR/DVR"
                            }
                        }
                    },
                    {
                        "01",
                        new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                                "name",
                                new object[]
                                {
                                    "video2",
                                    "cbl",
                                    "sat"
                                }
                            },
                            {
                                "description",
                                "sets VIDEO2, CBL/SAT"
                            }
                        }
                    },
                    {
                        "02",
                        new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                                "name",
                                new object[]
                                {
                                    "video3",
                                    "game"
                                }
                            },
                            {
                                "description",
                                "sets VIDEO3, GAME/TV, GAME"
                            }
                        }
                    },
                    {
                        "03",
                        EiscpTemplates.GetValue( "Video4", "Video4")
                    },
                    {
                        "04",
                        EiscpTemplates.GetValue( "Video5", "Video5")
                    },
                    {
                        "05",
                        EiscpTemplates.GetValue( "PC", "PC")
                    },
                    {
                        "06",
                        EiscpTemplates.GetValue( "Video7", "Video7")
                    },
                    {
                        "07",
                        EiscpTemplates.GetValue( "07", "Hidden1")
                    },
                    {
                       "08",
                       EiscpTemplates.GetValue( "08", "Hidden2")
                    },
                    {
                        "09",
                        EiscpTemplates.GetValue( "09", "Hidden3")
                    },
                    {
                        "10",
                        EiscpTemplates.GetValue( "DVD", "DVD")
                    },
                    {
                        "12",
                        EiscpTemplates.GetValue( "TV", "TV")
                    },
                    {
                        "20",
                        new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                                "name",
                                new object[]
                                {
                                    "tape-1",
                                    "tv",
                                    "tape"
                                }
                            },
                            {
                                "description",
                                "sets TAPE(1), TV/TAPE"
                            }
                        }
                    },
                    {
                         "21",
                         new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                                "name",
                                "tape2"
                            },
                            {
                                "description",
                                "sets TAPE2"
                            }
                        }
                    },
                    {
                        "22",
                        EiscpTemplates.GetValue( "Phono", "Phono")
                    },
                    {
                        "23",
                        EiscpTemplates.GetValue( "CD", "CD")
                    },
                    {
                        "24",
                        EiscpTemplates.GetValue( "FM", "FM")
                    },
                    {
                        "25",
                        EiscpTemplates.GetValue( "AM", "AM")
                    },
                    {
                        "26",
                        EiscpTemplates.GetValue( "Tuner", "Tuner")
                    },
                    {
                        "27",
                        new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                               "name",
                                new object[]
                                {
                                    "music-server",
                                    "p4s",
                                    "dlna"
                                }
                            },
                            {
                                "description",
                                "sets MUSIC SERVER, P4S, DLNA"
                            }
                        }
                    },
                    {
                        "28",
                        new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                                "name",
                                new object[]
                                {
                                    "internet-radio",
                                    "iradio-favorite"
                                }
                            },
                            {
                                "description",
                                "sets INTERNET RADIO, iRadio Favorite"
                            }
                        }
                    },
                    {
                        "29",
                        EiscpTemplates.GetValue( "USB (front)", "USB (front)")
                    },
                    {
                        "2A",
                        EiscpTemplates.GetValue( "USB (rear)", "DVD (rear)")
                    },
                    {
                        "2B",
                        EiscpTemplates.GetValue( "NET", "NET")
                    },
                    {
                        "2C",
                        EiscpTemplates.GetValue( "USB", "USB (toggle)")
                    },
                    {
                        "40",
                        EiscpTemplates.GetValue( "Universal-Port", "Universal-Port")
                    },
                    {
                        "30",
                        EiscpTemplates.GetValue("Multi-Ch", "Multi-Ch")
                    },
                    {
                        "31",
                        new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                                "name",
                                "xm"
                            },
                            {
                                "description",
                                "sets XM"
                            }
                        }
                    },
                    {
                        "32",
                        new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                                "name",
                                "sirius"
                            },
                            {
                                "description",
                                "sets SIRIUS"
                            }
                        }
                    },
                    {
                        "UP",
                        new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                                "name",
                                "up"
                            },
                            {
                                "description",
                                "sets Selector Position Wrap-Around Up"
                            }
                        }
                    },
                    {
                        "DOWN",
                        new OrderedDictionary(StructuralComparisons.StructuralEqualityComparer)
                        {
                            {
                               "name",
                                "down"
                            },
                            {
                                "description",
                                "sets Selector Position Wrap-Around Down"
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
                                "gets The Selector Position"
                            }
                        }
                    }
                }
            }
        };
        /// <summary>
        /// Select
        /// </summary>
        /// <param name="seletor">selector</param>
        /// <returns></returns>
        public string Select(int seletor)=> GetCommandString($"{seletor}");
    }
}
