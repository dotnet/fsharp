#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.AssemblyContentProviderTests
#endif

open System
open NUnit.Framework
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Service.Tests.Common

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

let private checker = FSharpChecker.Create()

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

[<Test>]
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
        
[<Test>]
let ``Module suffix added by an explicitly applied ModuleSuffix attribute is removed``() =
    """
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module MyType =
    let func123 x = x
"""
    => [ "Test"
         "Test.MyType"
         "Test.MyType.func123" ]

[<Test>]
let ``Property getters and setters are removed``() =
    """
    type MyType() =
        static member val MyProperty = 0 with get,set
"""
    => [ "Test"
         "Test.MyType"
         "Test.MyType.MyProperty" ]

[<Test>]
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
