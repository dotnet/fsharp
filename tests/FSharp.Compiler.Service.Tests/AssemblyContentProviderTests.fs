module FSharp.Compiler.Service.Tests.AssemblyContentProviderTests

open System
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Service.Tests.Common
open FSharp.Test

let private filePath = "C:\\test.fs"

let private projectOptions : FSharpProjectOptions = 
    { ProjectFileName = "C:\\test.fsproj"
      ProjectId = None
      SourceFiles =  [| filePath |]
      ReferencedProjects = [| |]
      OtherOptions = [| |]
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
        |> Async.RunImmediate

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

[<FactForDESKTOP>]
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
        
[<FactForDESKTOP>]
let ``Module suffix added by an explicitly applied ModuleSuffix attribute is removed``() =
    """
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module MyType =
    let func123 x = x
"""
    => [ "Test"
         "Test.MyType"
         "Test.MyType.func123" ]

[<FactForDESKTOP>]
let ``Property getters and setters are removed``() =
    """
    type MyType() =
        static member val MyProperty = 0 with get,set
"""
    => [ "Test"
         "Test.MyType"
         "Test.MyType.MyProperty" ]

[<FactForDESKTOP>]
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


[<FactForDESKTOP>]
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
