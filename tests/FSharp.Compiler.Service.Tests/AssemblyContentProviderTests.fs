module FSharp.Compiler.Service.Tests.AssemblyContentProviderTests

open System
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols
open FSharp.Test
open Xunit

let private filePath = Path.Combine(Path.GetTempPath(), "test.fs")

let private projectOptions : FSharpProjectOptions =
    { ProjectFileName = Path.ChangeExtension(filePath, ".fsproj")
      ProjectId = None
      SourceFiles =  [| filePath |]
      ReferencedProjects = [| |]
      OtherOptions = mkProjectCommandLineArgsSilent ("test.dll", [])
      IsIncompleteTypeCheckEnvironment = true
      UseScriptResolutionRules = false
      LoadTime = DateTime.MaxValue
      OriginalLoadReferences = []
      UnresolvedReferences = None
      Stamp = None }

let private checker = FSharpChecker.Create(useTransparentCompiler = CompilerAssertHelpers.UseTransparentCompiler)

let private assertAreEqual (expected, actual) =
    if actual <> expected then
        failwithf "\n\nExpected\n\n%A\n\nbut was\n\n%A" expected actual

let private checkFile (source: string) =
    let _, checkFileAnswer =
        checker.ParseAndCheckFileInProject(filePath, 0, FSharp.Compiler.Text.SourceText.ofString source, projectOptions)
        |> Async.RunSynchronouslyImmediate

    match checkFileAnswer with
    | FSharpCheckFileAnswer.Aborted -> failwithf "ParseAndCheckFileInProject aborted"
    | FSharpCheckFileAnswer.Succeeded checkFileResults -> checkFileResults

let private getCleanedFullName (symbol: AssemblySymbol) =
    symbol.CleanedIdents |> String.concat "."

let private getTopRequireQualifiedAccessParentName (symbol: AssemblySymbol) =
    symbol.TopRequireQualifiedAccessParent
    |> Option.defaultValue [||]
    |> String.concat "."

let private (=>) (source: string) (expected: string list) =
    let checkFileResults = checkFile source

    let actual =
        checkFileResults.PartialAssemblySignature
        |> AssemblyContent.GetAssemblySignatureContent AssemblyContentType.Full
        |> List.map getCleanedFullName

    assertAreEqual (List.sort expected, List.sort actual)

let private getSymbolMap (getSymbolProperty: AssemblySymbol -> 'a) (source: string) =
    let checkFileResults = checkFile source

    checkFileResults.PartialAssemblySignature
    |> AssemblyContent.GetAssemblySignatureContent AssemblyContentType.Full
    |> List.map (fun s -> getCleanedFullName s, getSymbolProperty s)
    |> Map.ofList

[<Fact>]
let ``implicitly added Module suffix is removed``() =
    """
type MyType = { F: int }

module MyType =
    let func123 x = x
"""
    => ["Test"
        "Test.MyType"
        "Test.MyType"
        "Test.MyType.func123"]

[<Fact>]
let ``Module suffix added by an explicitly applied ModuleSuffix attribute is removed``() =
    """
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module MyType =
    let func123 x = x
"""
    => [ "Test"
         "Test.MyType"
         "Test.MyType.func123" ]

[<Fact>]
let ``Property getters and setters are removed``() =
    """
    type MyType() =
        static member val MyProperty = 0 with get,set
"""
    => [ "Test"
         "Test.MyType"
         "Test.MyType.MyProperty" ]

[<Fact>]
let ``TopRequireQualifiedAccessParent property should be valid``() =
    let source = """
        module M1 = 
            let v1 = 1

            module M11 = 
                let v11 = 1

                module M111 = 
                    let v111 = 1

            [<RequireQualifiedAccess>]
            module M12 = 
                let v12 = 1

                module M121 = 
                    let v121 = 1

                    [<RequireQualifiedAccess>]
                    module M1211 = 
                        let v1211 = 1
    """

    let expectedResult =
        [
            "Test", "";
            "Test.M1", "";
            "Test.M1.v1", "";
            "Test.M1.M11", "";
            "Test.M1.M11.v11", "";
            "Test.M1.M11.M111", "";
            "Test.M1.M11.M111.v111", "";
            "Test.M1.M12", "";
            "Test.M1.M12.v12", "Test.M1.M12";
            "Test.M1.M12.M121", "Test.M1.M12";
            "Test.M1.M12.M121.v121", "Test.M1.M12";
            "Test.M1.M12.M121.M1211", "Test.M1.M12";
            "Test.M1.M12.M121.M1211.v1211", "Test.M1.M12";
        ]
        |> Map.ofList

    let actual = source |> getSymbolMap getTopRequireQualifiedAccessParentName

    assertAreEqual (expectedResult, actual)


[<Fact>]
let ``Check Unresolved Symbols``() =
    let source = """
namespace ``1 2 3``

module Test =
    module M1 = 
        let v1 = 1

        module M11 = 
            let v11 = 1

            module M111 = 
                let v111 = 1

        [<RequireQualifiedAccess>]
        module M12 = 
            let v12 = 1

            module M121 = 
                let v121 = 1

                [<RequireQualifiedAccess>]
                module M1211 = 
                    let v1211 = 1

        type A = 
            static member val B = 0
            static member C() = ()
            static member (++) s s2 = s + "/" + s2

        type B =
            abstract D: int -> int

        let ``a.b.c`` = "999"

        type E = { x: int; y: int }
        type F =
            | A = 1
            | B = 2
        type G =
            | A of int
            | B of string
        
        let (|Is1|_|) x = x = 1
        let (++) s s2 = s + "/" + s2
    """

    let expectedResult =
        [
            "1 2 3.Test", "open ``1 2 3`` - Test";
            "1 2 3.Test.M1", "open ``1 2 3`` - Test.M1";
            "1 2 3.Test.M1.(++)", "open ``1 2 3`` - Test.M1.``(++)``";
            "1 2 3.Test.M1.A", "open ``1 2 3`` - Test.M1.A";
            "1 2 3.Test.M1.A.(++)", "open ``1 2 3`` - Test.M1.A.``(++)``";
            "1 2 3.Test.M1.A.B", "open ``1 2 3`` - Test.M1.A.B";
            "1 2 3.Test.M1.A.C", "open ``1 2 3`` - Test.M1.A.C";
            "1 2 3.Test.M1.A.op_PlusPlus", "open ``1 2 3`` - Test.M1.A.op_PlusPlus";
            "1 2 3.Test.M1.(|Is1|_|)", "open ``1 2 3`` - Test.M1.``(|Is1|_|)``"
            "1 2 3.Test.M1.B", "open ``1 2 3`` - Test.M1.B";
            "1 2 3.Test.M1.E", "open ``1 2 3`` - Test.M1.E";
            "1 2 3.Test.M1.F", "open ``1 2 3`` - Test.M1.F";
            "1 2 3.Test.M1.G", "open ``1 2 3`` - Test.M1.G";
            "1 2 3.Test.M1.Is1", "open ``1 2 3``.Test.M1 - Is1";
            "1 2 3.Test.M1.M11", "open ``1 2 3`` - Test.M1.M11";
            "1 2 3.Test.M1.M11.M111", "open ``1 2 3`` - Test.M1.M11.M111";
            "1 2 3.Test.M1.M11.M111.v111", "open ``1 2 3`` - Test.M1.M11.M111.v111";
            "1 2 3.Test.M1.M11.v11", "open ``1 2 3`` - Test.M1.M11.v11";
            "1 2 3.Test.M1.M12", "open ``1 2 3`` - Test.M1.M12";
            "1 2 3.Test.M1.M12.M121", "open ``1 2 3``.Test.M1 - M12.M121";
            "1 2 3.Test.M1.M12.M121.M1211", "open ``1 2 3``.Test.M1 - M12.M121.M1211";
            "1 2 3.Test.M1.M12.M121.M1211.v1211", "open ``1 2 3``.Test.M1 - M12.M121.M1211.v1211";
            "1 2 3.Test.M1.M12.M121.v121", "open ``1 2 3``.Test.M1 - M12.M121.v121";
            "1 2 3.Test.M1.M12.v12", "open ``1 2 3``.Test.M1 - M12.v12";
            "1 2 3.Test.M1.``a.b.c``", "open ``1 2 3`` - Test.M1.``a.b.c``";
            "1 2 3.Test.M1.op_PlusPlus", "open ``1 2 3`` - Test.M1.op_PlusPlus";
            "1 2 3.Test.M1.v1", "open ``1 2 3`` - Test.M1.v1";
        ]
        |> Map.ofList

    let actual = source |> getSymbolMap (fun i ->
        let ns = i.UnresolvedSymbol.Namespace |> String.concat "."
        $"open {ns} - {i.UnresolvedSymbol.DisplayName}")

    assertAreEqual (expectedResult, actual)

let private activePatternSource = """
namespace Catalogue

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Patterns =
    let (|Even|Odd|) value = if value % 2 = 0 then Even else Odd
    let (|Positive|_|) value = if value > 0 then Some value else None
    let (|Above|_|) threshold value = if value > threshold then Some value else None
    let (|Case|CASE|) value = if value then Case else CASE
    let (|``Has space``|_|) value = if value > 0 then Some value else None
    let private (|Private|_|) value = if value > 0 then Some value else None
    let internal (|Internal|_|) value = if value > 0 then Some value else None

    [<AutoOpen>]
    module Nested =
        let (|Even|_|) value = if value % 2 = 0 then Some value else None

    module private Hidden =
        let (|HiddenCase|_|) value = if value > 0 then Some value else None
"""

let private checkSources sources =
    let options = createProjectOptionsFromNamedSources sources []
    let filePath = Array.last options.SourceFiles
    let _, results = parseAndCheckFile filePath (snd (List.last sources)) options
    Assert.Empty results.Diagnostics
    options, results

let private activePatternCases symbols =
    symbols
    |> List.filter (fun symbol -> symbol.Symbol :? FSharpActivePatternCase)

[<Theory>]
[<InlineData("Even", 0)>]
[<InlineData("Odd", 1)>]
[<InlineData("Positive", 0)>]
[<InlineData("Above", 0)>]
[<InlineData("Case", 0)>]
[<InlineData("CASE", 1)>]
[<InlineData("Has space", 0)>]
let ``active pattern catalogue preserves case identity and source names`` (caseName, index) =
    let _, results = checkSources [ "Patterns.fs", activePatternSource ]
    let symbols = AssemblyContent.GetAssemblySignatureContent AssemblyContentType.Full results.PartialAssemblySignature
    let symbol =
        activePatternCases symbols
        |> List.find (fun symbol -> symbol.CleanedIdents = [| "Catalogue"; "Patterns"; caseName |])
    let case = Assert.IsType<FSharpActivePatternCase> symbol.Symbol
    Assert.Equal(caseName, case.Name)
    Assert.Equal(index, case.Index)
    Assert.Equal(case.FullName, symbol.FullName)
    Assert.NotEqual(getCleanedFullName symbol, symbol.FullName)
    Assert.Equal(Some [| "Catalogue" |], symbol.Namespace)
    Assert.Equal(Some [| "Catalogue"; "Patterns" |], symbol.NearestRequireQualifiedAccessParent)
    Assert.Equal(Some [| "Catalogue"; "Patterns" |], symbol.TopRequireQualifiedAccessParent)
    Assert.Equal(None, symbol.AutoOpenParent)
    Assert.Equal(symbol.FullName, symbol.UnresolvedSymbol.FullName)
    Assert.Equal([| "Catalogue" |], symbol.UnresolvedSymbol.Namespace)
    let sourceName = FSharp.Compiler.Syntax.PrettyNaming.NormalizeIdentifierBackticks caseName
    Assert.Equal($"Patterns.{sourceName}", symbol.UnresolvedSymbol.DisplayName)
    Assert.Equal(EntityKind.FunctionOrValue true, symbol.Kind LookupType.Precise)
    Assert.Contains(symbols, fun symbol ->
        match symbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as value -> value.IsActivePattern && case.Group.Name = Some value.LogicalName
        | _ -> false)

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``active pattern catalogue respects defining value and container visibility`` publicOnly =
    let project, results = checkSources [ "Patterns.fs", activePatternSource ]
    let contentType = if publicOnly then AssemblyContentType.Public else AssemblyContentType.Full
    let expected =
        [ "Catalogue.Patterns.Above"; "Catalogue.Patterns.CASE"; "Catalogue.Patterns.Case"
          "Catalogue.Patterns.Even"; "Catalogue.Patterns.Has space"; "Catalogue.Patterns.Nested.Even"
          "Catalogue.Patterns.Odd"; "Catalogue.Patterns.Positive"
          if not publicOnly then
              "Catalogue.Patterns.Hidden.HiddenCase"
              "Catalogue.Patterns.Internal"
              "Catalogue.Patterns.Private" ]
        |> List.sort
    let output = Path.ChangeExtension(project.ProjectFileName, ".dll")
    let source = """
module Consumer
let classified = match 2 with | Catalogue.Patterns.Even -> true | Catalogue.Patterns.Odd -> false
"""
    let options = createProjectOptionsFromNamedSources [ "Consumer.fs", source ] [ $"-r:{output}" ]
    let options = { options with ReferencedProjects = [| FSharpReferencedProject.FSharpReference(output, project) |] }
    let _, consumer = parseAndCheckFile options.SourceFiles[0] source options
    Assert.Empty consumer.Diagnostics
    let assemblies =
        consumer.ProjectContext.GetReferencedAssemblies()
        |> List.filter (fun assembly -> assembly.SimpleName = Path.GetFileNameWithoutExtension output)
    Assert.Single assemblies |> ignore
    let catalogue = AssemblyContent.GetAssemblySignatureContent contentType results.PartialAssemblySignature
    let actual = activePatternCases catalogue |> List.map getCleanedFullName |> List.sort
    Assert.Equal<string list>(expected, actual)
    let nested = catalogue |> List.find (fun symbol -> getCleanedFullName symbol = "Catalogue.Patterns.Nested.Even")
    Assert.Equal(Some [| "Catalogue"; "Patterns"; "Nested" |], nested.AutoOpenParent)
    let cache = EntityCache()
    let cacheKey = Some project.SourceFiles[0]
    AssemblyContent.GetAssemblyContent cache.Locking AssemblyContentType.Full cacheKey assemblies |> ignore
    let cached =
        AssemblyContent.GetAssemblyContent cache.Locking contentType cacheKey assemblies
        |> activePatternCases
        |> List.map getCleanedFullName
        |> List.sort
    Assert.Equal<string list>(expected, cached)

[<Fact>]
let ``active pattern catalogue respects signature visibility`` () =
    let signature = """
namespace Catalogue
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Patterns =
    val (|Even|Odd|): int -> Choice<unit, unit>
"""
    let _, results = checkSources [ "Patterns.fsi", signature; "Patterns.fs", activePatternSource ]
    let actual =
        AssemblyContent.GetAssemblySignatureContent AssemblyContentType.Full results.PartialAssemblySignature
        |> activePatternCases
        |> List.map getCleanedFullName
        |> List.sort
    Assert.Equal<string list>([ "Catalogue.Patterns.Even"; "Catalogue.Patterns.Odd" ], actual)
