// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.TypeProviderTests

open NUnit.Framework
open FsUnit
open TestHelper
open Microsoft.FSharp.Compiler.SourceCodeServices.ServiceFormatting.FormatConfig

[<Test>]
let ``type providers``() =
    formatSourceString false """
type Northwind = ODataService<"http://services.odata.org/Northwind/Northwind.svc/">""" config
    |> prepend newline
    |> should equal """
type Northwind = ODataService< "http://services.odata.org/Northwind/Northwind.svc/" >
"""

[<Test>]
let ``should add space before type provider params``() =
    formatSourceString false """
type IntegerRegex = FSharpx.Regex< @"(?<value>\d+)" >""" config
    |> prepend newline
    |> should equal """
type IntegerRegex = FSharpx.Regex< @"(?<value>\d+)" >
"""

[<Test>]
let ``should throw FormatException on unparsed input``() =
    Assert.Throws(fun () -> 
        formatSourceString false """
type GeoResults = JsonProvider<Sample= "A" + "GitHub.json" >""" config 
        |> ignore)

[<Test>]
let ``should handle lines with more than 512 characters``() =
    formatSourceString false """
(new CsvFile<string * decimal * decimal>(new Func<obj, string[], string * decimal * decimal>(fun (parent : obj) (row : string[]) -> CommonRuntime.GetNonOptionalValue("Name", CommonRuntime.ConvertString(TextConversions.AsOption(row.[0])), TextConversions.AsOption(row.[0])), CommonRuntime.GetNonOptionalValue("Distance", CommonRuntime.ConvertDecimal("", TextConversions.AsOption(row.[1])), TextConversions.AsOption(row.[1])), CommonRuntime.GetNonOptionalValue("Time", CommonRuntime.ConvertDecimal("", TextConversions.AsOption(row.[2])), TextConversions.AsOption(row.[2]))), new Func<string * decimal * decimal, string[]>(fun (row : string * decimal * decimal) -> [| CommonRuntime.ConvertStringBack(CommonRuntime.GetOptionalValue((let x, _, _ = row in x))); CommonRuntime.ConvertDecimalBack("", CommonRuntime.GetOptionalValue((let _, x, _ = row in x))); CommonRuntime.ConvertDecimalBack("", CommonRuntime.GetOptionalValue((let _, _, x = row in x))) |]), (ProviderFileSystem.readTextAtRunTimeWithDesignTimeOptions @"C:\Dev\FSharp.Data-master\src\..\tests\FSharp.Data.Tests\Data" "" "SmallTest.csv"), "", '"', true, false)).Cache()
"""  config
    |> prepend newline
    |> should equal """
(new CsvFile<string * decimal * decimal>(new Func<obj, string [], string * decimal * decimal>(fun (parent : obj) (row : string []) -> CommonRuntime.GetNonOptionalValue("Name", CommonRuntime.ConvertString(TextConversions.AsOption(row.[0])), TextConversions.AsOption(row.[0])), CommonRuntime.GetNonOptionalValue("Distance", CommonRuntime.ConvertDecimal("", TextConversions.AsOption(row.[1])), TextConversions.AsOption(row.[1])), CommonRuntime.GetNonOptionalValue("Time", CommonRuntime.ConvertDecimal("", TextConversions.AsOption(row.[2])), TextConversions.AsOption(row.[2]))), 
                                         new Func<string * decimal * decimal, string []>(fun (row : string * decimal * decimal) -> 
                                         [| CommonRuntime.ConvertStringBack(CommonRuntime.GetOptionalValue((let x, _, _ = row
                                                                                                            x)))
                                            CommonRuntime.ConvertDecimalBack("", 
                                                                             CommonRuntime.GetOptionalValue((let _, x, _ = row
                                                                                                             x)))
                                            CommonRuntime.ConvertDecimalBack("", 
                                                                             CommonRuntime.GetOptionalValue((let _, _, x = row
                                                                                                             x))) |]), (ProviderFileSystem.readTextAtRunTimeWithDesignTimeOptions @"C:\Dev\FSharp.Data-master\src\..\tests\FSharp.Data.Tests\Data" "" "SmallTest.csv"), "", '"', true, false))
    .Cache()
"""