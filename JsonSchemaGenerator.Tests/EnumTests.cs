// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
public class EnumTests : TestBase
{
    private static String[][] _enumNames =
        [
            ["FirstValue", "SecondValue", "ThirdValue"],
            ["Alpha", "Beta", "Gamma", "Delta", "Epsilon"],
            ["North", "East", "South", "West"]
        ];
    private static String[] _backingTypes =
        [
            "byte",
            "sbyte",
            "short",
            "ushort",
            "int",
            "uint",
            "long",
            "ulong"
        ];
    private static Dictionary<String, Int64[][]> _backingValues = new()
    {
        ["sbyte"] = [
            [7,61,-124,-68,2,-113,86,75,-119,18],
            [-124,-106,92,-12,-75,-22,-13,38,96,-84],
            [116,-25,-15,33,41,31,-34,-86,46,-21],
            [71,-37,52,-107,76,0,-45,29,-31,89],
            [29,-30,-103,-17,38,-30,7,57,-84,66]
        ],
        ["short"] = [
            [29072,32087,12948,25381,-31250,8086,13216,-25804,-2426,-11854],
            [-19827,-17198,-14626,-23045,14971,2788,32669,25159,12518,18258],
            [-5277,30282,11781,908,-27055,10759,26752,20409,5413,23696],
            [-1823,4456,-8497,7919,29159,-6423,32090,-4403,-13432,-11787],
            [-23957,17041,-6864,-17638,-8859,6179,5937,26608,-2607,10944]
        ],
        ["int"] = [
            [1613214318,12129728,1207976673,-248138157,-2010219275,1569083814,777790615,-696666094,2127947781,1978538124],
            [727133575,1988753147,-2018505393,1564004513,1842838173,-958556382,1423447360,-170016292,-1700772819,-1698055605],
            [-472469674,-2080321323,466060468,-1432367983,546136448,-630596869,-413317875,1268838748,589132294,1296216794],
            [-168061618,938387602,-369142410,1929717777,1765899436,-1410313134,1251913876,-580387759,-1875410101,-670198596],
            [-46181944,-47247073,484091968,491896441,27754578,-1619333727,1722797141,-1022670986,1900585747,-1399268045]
        ],
        ["long"] = [
            [495389958603470840,8447152716115548513,4076947326000842512,-5479946906683008702,-6261398387516321561,-6234439237982546410,-7763756555914354885,-2921162275753988,-5265319875354811978,5937704451502015803],
            [-3678953368049770471,-9182618665140555875,-4711107453777003156,-2693251647694472038,-1690942789069429077,-2250582046792997830,6950913194119754604,4460344824565131315,771921778339440785,-2235137444732605076],
            [-6886443169019256175,5346352320447286247,8691261109976116692,-1437156821975304676,-1843864286762302475,-8187225095092195711,6538693043440362744,5124723316310544881,-7659966091633248925,3440951494749412320],
            [-5834532563896315819,7578824214286900436,-6901796846311834429,4700370522546455104,-5815311281243572061,-8248835607728872295,8121697370496614704,5492458820722793746,-8825351281791169735,8069895335539612024],
            [1380781554477892003,5007049859318738593,8135039634451451191,-5130122129127775636,-8345327029106387263,2086799114850886913,-4422763600302490190,8506097865248862454,7045051659855894624,6195371062880437671]
        ],
        ["byte"] = [
            [12,113,81,227,196,211,17,217,250,168],
            [5,10,134,89,152,79,224,112,213,181],
            [149,226,81,87,98,53,173,29,12,196],
            [148,179,228,17,230,222,19,3,213,81],
            [51,87,240,140,214,72,229,179,133,186]
        ],
        ["ushort"] = [
            [9639,49045,39980,59270,10910,32251,54661,6885,39176,60327],
            [40721,516,65340,16443,18898,6049,25282,22084,52238,34607],
            [12551,7755,28334,39034,3972,37495,35054,24475,19402,36031],
            [60289,6348,3021,32699,56417,63956,57187,40288,30949,21145],
            [30458,18076,38895,55144,1764,31314,65509,57373,22943,58765]
        ],
        ["uint"] = [
            [3938632090,2439817866,458715170,1164696510,4107158376,3239060903,3367180297,28153192,1519656750,3304531653],
            [3787012445,2072576096,3414771423,4163419501,398109072,3748731961,4253620060,3594138995,702743338,830889087],
            [3145520667,2744182138,156561620,1496167159,2189894541,448901503,2200178795,2948588216,3274443748,3801561828],
            [1770021667,2591291104,2529775304,2120231418,1217905809,1180803023,1804007503,3398908706,23511469,1915542443],
            [3754926045,149841562,1922975842,1680471646,1274518059,3188380987,3550918944,145084917,1706150537,2772310354]
        ],
        ["ulong"] = [
            [6691236691083761964,6955183930736980709,8111697362173471917,2874999693067119149,6396203050639019889,1325933397858801856,3981981388008945679,1530864952540314256,979527057049333753,5894809989625553641],
            [8830651669050158071,5403138706018044016,7254092947794718173,6854293650711634559,8043044437292611092,7960216899491170382,5058270798566682887,9141635603604733325,7751894405611982958,2604093282185344101],
            [1984167343686588111,6497347327798373003,2163842141691382706,4134328923782959321,1288808377247624934,8313269355162954991,317931964236246973,4584214137271230749,1095174360143517519,3135942262068016948],
            [3292858928160937800,5086052968281212388,1098595150536946310,6730170900371772351,3927452594717479151,2484135242839712064,404716722781270061,3473512752873978604,4289682095942437270,1208482148757636708],
            [1250764728262178485,1669937843595247109,5051287328590007400,4258560733112941488,436561614246524692,8158513699569642826,5074315733366131897,4455992086151602491,7302366524584314963,5924312509077559369]
        ]
    };

    public static Object[][] Data =>
        _backingTypes.SelectMany(t =>
        {
            var values = _backingValues[t];
            var result = new Object[values.Length * _enumNames.Length][];

            for(var i = 0; i < _enumNames.Length; i++)
                for(var j = 0; j < values.Length; j++)
                    result[i * values.Length + j] = [_enumNames[i].Zip(values[j]).ToArray(), t];

            return result;
        }).ToArray();

    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_EnumNamesOrValuesOrIntegerForEnumType((String name, Int64 value)[] constants, String backingType)
    {
        TestSchema(
            $$"""
            enum EnumerationType: {{backingType}}
            {
                {{String.Join(",\n\t", constants.Select(t => $"{t.name} = {t.value}"))}}
            }

            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public EnumerationType Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        anyOf = new Object[]
                        {
                            new { type = new[] { "integer" } },
                            new { @enum = constants.Select(t=>(Object)t.name).Concat(constants.Select(t=>(Object)t.value)).ToArray() }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_EnumNamesOrValuesOrIntegerForMultipleEnumTypeProperties((String name, Int64 value)[] constants, String backingType)
    {
        TestSchema(
            $$"""
            enum EnumerationType: {{backingType}}
            {
                {{String.Join(",\n\t", constants.Select(t => $"{t.name} = {t.value}"))}}
            }

            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public EnumerationType Prop1 { get; set; }
                public EnumerationType Prop2 { get; set; }
                public EnumerationType Prop3 { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop1 = new
                    {
                        anyOf = new Object[]
                        {
                            new { type = new[] { "integer" } },
                            new { @enum = constants.Select(t=>(Object)t.name).Concat(constants.Select(t=>(Object)t.value)).ToArray() }
                        }
                    },
                    Prop2 = new
                    {
                        anyOf = new Object[]
                        {
                            new { type = new[] { "integer" } },
                            new { @enum = constants.Select(t=>(Object)t.name).Concat(constants.Select(t=>(Object)t.value)).ToArray() }
                        }
                    },
                    Prop3 = new
                    {
                        anyOf = new Object[]
                        {
                            new { type = new[] { "integer" } },
                            new { @enum = constants.Select(t=>(Object)t.name).Concat(constants.Select(t=>(Object)t.value)).ToArray() }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_EnumNamesOrValuesOrIntegerOrNullForNullableEnumType((String name, Int64 value)[] constants, String backingType)
    {
        TestSchema(
            $$"""
            enum EnumerationType: {{backingType}}
            {
                {{String.Join(",\n\t", constants.Select(t => $"{t.name} = {t.value}"))}}
            }

            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public EnumerationType? Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        anyOf = new Object[]
                        {
                            new { type = new[] { "integer", "null" } },
                            new { @enum = constants.Select(t=>(Object)t.name).Concat(constants.Select(t=>(Object)t.value)).ToArray() }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_EnumNamesOrValuesOrIntegerOrNullForMultipleNullableEnumTypeProperties((String name, Int64 value)[] constants, String backingType)
    {
        TestSchema(
            $$"""
            enum EnumerationType: {{backingType}}
            {
                {{String.Join(",\n\t", constants.Select(t => $"{t.name} = {t.value}"))}}
            }

            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public EnumerationType? Prop1 { get; set; }
                public EnumerationType? Prop2 { get; set; }
                public EnumerationType? Prop3 { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop1 = new
                    {
                        anyOf = new Object[]
                        {
                            new { type = new[] { "integer", "null" } },
                            new { @enum = constants.Select(t=>(Object)t.name).Concat(constants.Select(t=>(Object)t.value)).ToArray() }
                        }
                    },
                    Prop2 = new
                    {
                        anyOf = new Object[]
                        {
                            new { type = new[] { "integer", "null" } },
                            new { @enum = constants.Select(t=>(Object)t.name).Concat(constants.Select(t=>(Object)t.value)).ToArray() }
                        }
                    },
                    Prop3 = new
                    {
                        anyOf = new Object[]
                        {
                            new { type = new[] { "integer", "null" } },
                            new { @enum = constants.Select(t=>(Object)t.name).Concat(constants.Select(t=>(Object)t.value)).ToArray() }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
}