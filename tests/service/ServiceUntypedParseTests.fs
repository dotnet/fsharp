#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.ServiceUntypedParseTests
#endif

open System
open System.IO
open System.Text
open NUnit.Framework
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Service.Tests.Common
open Tests.Service

let [<Literal>] private Marker = "(* marker *)"

let private (=>) (source: string) (expected: CompletionContext option) =

    let lines =
        use reader = new StringReader(source)
        [| let line = ref (reader.ReadLine())
           while not (isNull !line) do
              yield !line
              line := reader.ReadLine()
           if source.EndsWith "\n" then
              yield "" |]
        
    let markerPos = 
        lines 
        |> Array.mapi (fun i x -> i, x)
        |> Array.tryPick (fun (lineIdx, line) -> 
            match line.IndexOf Marker with 
            | -1 -> None 
            | idx -> Some (mkPos (Line.fromZ lineIdx) idx))
        
    match markerPos with
    | None -> failwithf "Marker '%s' was not found in the source code" Marker
    | Some markerPos ->
        match parseSourceCode("C:\\test.fs", source) with
        | None -> failwith "No parse tree"
        | Some parseTree ->
            let actual = UntypedParseImpl.TryGetCompletionContext(markerPos, parseTree, lines.[Line.toZ markerPos.Line])
            try Assert.AreEqual(expected, actual)
            with e ->
                printfn "ParseTree: %A" parseTree
                reraise()

module AttributeCompletion =
    [<Test>]
    let ``at [<|, applied to nothing``() =
        """
[<(* marker *)
"""  
     => Some CompletionContext.AttributeApplication

    [<TestCase ("[<(* marker *)", true)>]
    [<TestCase ("[<AnAttr(* marker *)", true)>]
    [<TestCase ("[<type:(* marker *)", true)>]
    [<TestCase ("[<type:AnAttr(* marker *)", true)>]
    [<TestCase ("[< (* marker *)", true)>]
    [<TestCase ("[<AnAttribute;(* marker *)", true)>]
    [<TestCase ("[<AnAttribute; (* marker *)", true)>]
    [<TestCase ("[<AnAttribute>][<(* marker *)", true)>]
    [<TestCase ("[<AnAttribute>][< (* marker *)", true)>]
    [<TestCase ("[<AnAttribute((* marker *)", false)>]
    [<TestCase ("[<AnAttribute( (* marker *)", false)>]
    [<TestCase ("[<AnAttribute (* marker *)", false)>]
    [<TestCase ("[<AnAttribute>][<AnAttribute((* marker *)", false)>]
    [<TestCase ("[<AnAttribute; AnAttribute((* marker *)", false)>]
    let ``incomplete``(lineStr: string, expectAttributeApplicationContext: bool) =
        (sprintf """
%s
type T =
    { F: int }
""" lineStr)  => (if expectAttributeApplicationContext then Some CompletionContext.AttributeApplication else None)

    [<TestCase ("[<(* marker *)>]", true)>]
    [<TestCase ("[<AnAttr(* marker *)>]", true)>]
    [<TestCase ("[<type:(* marker *)>]", true)>]
    [<TestCase ("[<type:AnAttr(* marker *)>]", true)>]
    [<TestCase ("[< (* marker *)>]", true)>]
    [<TestCase ("[<AnAttribute>][<(* marker *)>]", true)>]
    [<TestCase ("[<AnAttribute>][< (* marker *)>]", true)>]
    [<TestCase ("[<AnAttribute;(* marker *)>]", true)>]
    [<TestCase ("[<AnAttribute; (* marker *) >]", true)>]
    [<TestCase ("[<AnAttribute>][<AnAttribute;(* marker *)>]", true)>]
    [<TestCase ("[<AnAttribute((* marker *)>]", false)>]
    [<TestCase ("[<AnAttribute (* marker *) >]", false)>]
    [<TestCase ("[<AnAttribute>][<AnAttribute((* marker *)>]", false)>]
    [<TestCase ("[<AnAttribute; AnAttribute((* marker *)>]", false)>]
    [<TestCase ("[<AnAttribute; AnAttribute( (* marker *)>]", false)>]
    [<TestCase ("[<AnAttribute>][<AnAttribute; AnAttribute((* marker *)>]", false)>]
    let ``complete``(lineStr: string, expectAttributeApplicationContext: bool) =
        (sprintf """
%s
type T =
    { F: int }
""" lineStr)  => (if expectAttributeApplicationContext then Some CompletionContext.AttributeApplication else None)