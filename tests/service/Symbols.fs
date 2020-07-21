#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.Symbols
#endif

open FSharp.Compiler.Service.Tests.Common
open FsUnit
open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

module ActivePatterns =

    let completePatternInput = """
let (|True|False|) = function
    | true -> True
    | false -> False

match true with
| True | False -> ()
"""

    let partialPatternInput = """
let (|String|_|) = function
    | :? String -> Some ()
    | _ -> None

match "foo" with
| String
| _ -> ()
"""

    let getCaseUsages source line =
         let fileName, options = mkTestFileAndOptions source [| |]
         let _, checkResults = parseAndCheckFile fileName source options
          
         checkResults.GetAllUsesOfAllSymbolsInFile()
         |> Async.RunSynchronously
         |> Array.filter (fun su -> su.RangeAlternate.StartLine = line && su.Symbol :? FSharpActivePatternCase)
         |> Array.map (fun su -> su.Symbol :?> FSharpActivePatternCase)

    [<Test>]
    let ``Active pattern case indices`` () =
        let getIndices = Array.map (fun (case: FSharpActivePatternCase) -> case.Index)

        getCaseUsages completePatternInput 7 |> getIndices |> shouldEqual [| 0; 1 |]
        getCaseUsages partialPatternInput 7 |> getIndices |> shouldEqual [| 0 |]

    [<Test>]
    let ``Active pattern group names`` () =
        let getGroupName (case: FSharpActivePatternCase) = case.Group.Name.Value

        getCaseUsages completePatternInput 7 |> Array.head |> getGroupName |> shouldEqual "|True|False|"
        getCaseUsages partialPatternInput 7 |> Array.head |> getGroupName |> shouldEqual "|String|_|"


module XmlDocSig =

    [<Test>]
    let ``XmlDocSig of modules in namespace`` () =
        let source = """
namespace Ns1
module Mod1 =
    let val1 = 1
    module Mod2 =
       let func2 () = ()
"""
        let fileName, options = mkTestFileAndOptions source [| |]
        let _, checkResults = parseAndCheckFile fileName source options  

        let mod1 = checkResults.PartialAssemblySignature.FindEntityByPath ["Ns1"; "Mod1"] |> Option.get
        let mod2 = checkResults.PartialAssemblySignature.FindEntityByPath ["Ns1"; "Mod1"; "Mod2"] |> Option.get
        let mod1val1 = mod1.MembersFunctionsAndValues |> Seq.find (fun m -> m.DisplayName = "val1")
        let mod2func2 = mod2.MembersFunctionsAndValues |> Seq.find (fun m -> m.DisplayName = "func2")
        mod1.XmlDocSig |> shouldEqual "T:Ns1.Mod1"
        mod2.XmlDocSig |> shouldEqual "T:Ns1.Mod1.Mod2"
        mod1val1.XmlDocSig |> shouldEqual "P:Ns1.Mod1.val1"
        mod2func2.XmlDocSig |> shouldEqual "M:Ns1.Mod1.Mod2.func2"

    [<Test>]
    let ``XmlDocSig of modules`` () =
         let source = """
module Mod1 
let val1 = 1
module Mod2 =
    let func2 () = ()
"""
         let fileName, options = mkTestFileAndOptions source [| |]
         let _, checkResults = parseAndCheckFile fileName source options  

         let mod1 = checkResults.PartialAssemblySignature.FindEntityByPath ["Mod1"] |> Option.get
         let mod2 = checkResults.PartialAssemblySignature.FindEntityByPath ["Mod1"; "Mod2"] |> Option.get
         let mod1val1 = mod1.MembersFunctionsAndValues |> Seq.find (fun m -> m.DisplayName = "val1")
         let mod2func2 = mod2.MembersFunctionsAndValues |> Seq.find (fun m -> m.DisplayName = "func2")
         mod1.XmlDocSig |> shouldEqual "T:Mod1"
         mod2.XmlDocSig |> shouldEqual "T:Mod1.Mod2"
         mod1val1.XmlDocSig |> shouldEqual "P:Mod1.val1"
         mod2func2.XmlDocSig |> shouldEqual "M:Mod1.Mod2.func2"


module Attributes =
    [<Test>]
    let ``Emit conditional attributes`` () =
        let source = """
open System
open System.Diagnostics

[<Conditional("Bar")>]
type FooAttribute() =
    inherit Attribute()

[<Foo>]
let x = 123
"""
        let fileName, options = mkTestFileAndOptions source [| "--noconditionalerasure" |]
        let _, checkResults = parseAndCheckFile fileName source options

        checkResults.GetAllUsesOfAllSymbolsInFile()
         |> Async.RunSynchronously
         |> Array.tryFind (fun su -> su.Symbol.DisplayName = "x")
         |> Option.orElseWith (fun _ -> failwith "Could not get symbol")
         |> Option.map (fun su -> su.Symbol :?> FSharpMemberOrFunctionOrValue)
         |> Option.iter (fun symbol -> symbol.Attributes.Count |> shouldEqual 1)
