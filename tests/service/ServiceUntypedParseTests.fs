#if INTERACTIVE
#r "../../Debug/fcs/net45/FSharp.Compiler.Service.dll" // note, run 'build fcs debug' to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../packages/NUnit.3.5.0/lib/net45/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.ServiceUntypedParseTests
#endif

open System
open System.IO
open System.Text
open NUnit.Framework
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Service.Tests.Common
open Tests.Service

let [<Literal>] private Marker = "(* marker *)"

let (=>) (source: string) (expected: CompletionContext option) =

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
            let actual = UntypedParseImpl.TryGetCompletionContext(markerPos, Some parseTree, lines.[Line.toZ markerPos.Line])
            try Assert.AreEqual(expected, actual)
            with e ->
                printfn "ParseTree: %A" parseTree
                reraise()

(*** open ended attribute ***)

[<Test>]
let ``AttributeApplication completion context at [<|``() =
    """
[<(* marker *)
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttr|``() =
    """
[<AnAttr(* marker *)
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<type:|``() =
    """
[<type:(* marker *)
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<type:AnAttr|``() =
    """
[<type:AnAttr(* marker *)
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [< |``() =
    """
[< (* marker *)
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttribute;|``() =
    """
[<AnAttribute;(* marker *)
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttribute; |``() =
    """
[<AnAttribute; (* marker *)
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttribute>][<|``() =
    """
[<AnAttribute>][<(* marker *)
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttribute>][< |``() =
    """
[<AnAttribute>][< (* marker *)
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute(|``() =
    """
[<AnAttribute((* marker *)
type T =
    { F: int }
"""
    => None

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute( |``() =
    """
[<AnAttribute( (* marker *)
type T =
    { F: int }
"""
    => None

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute |``() =
    """
[<AnAttribute (* marker *)
type T =
    { F: int }
"""
    => None

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute>][<AnAttribute(|``() =
    """
[<AnAttribute>][<AnAttribute((* marker *)
type T =
    { F: int }
"""
    => None

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute; AnAttribute(|``() =
    """
[<AnAttribute; AnAttribute((* marker *)
type T =
    { F: int }
"""
    => None

(*** closed attribute ***)

[<Test>]
let ``AttributeApplication completion context at [<|>]``() =
    """
[<(* marker *)>]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttr|>]``() =
    """
[<AnAttr(* marker *)>]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<type:|>]``() =
    """
[<type:(* marker *)>]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<type:AnAttr|>]``() =
    """
[<type:AnAttr(* marker *)>]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication


[<Test>]
let ``AttributeApplication completion context at [< |>]``() =
    """
[< (* marker *)>]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttribute>][<|>]``() =
    """
[<AnAttribute>][<(* marker *)>]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttribute>][< |>]``() =
    """
[<AnAttribute>][< (* marker *)>]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttribute;|>]``() =
    """
[<AnAttribute;(* marker *)>]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttribute; | >]``() =
    """
[<AnAttribute; (* marker *) >]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``AttributeApplication completion context at [<AnAttribute>][<AnAttribute; | >]``() =
    """
[<AnAttribute>][<AnAttribute;(* marker *)>]
type T =
    { F: int }
"""
    => Some CompletionContext.AttributeApplication

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute(|>]``() =
    """
[<AnAttribute((* marker *)>]
type T =
    { F: int }
"""
    => None

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute | >]``() =
    """
[<AnAttribute (* marker *) >]
type T =
    { F: int }
"""
    => None

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute>][<AnAttribute( | >]``() =
    """
[<AnAttribute>][<AnAttribute((* marker *)>]
type T =
    { F: int }
"""
    => None

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute; AnAttribute(| >]``() =
    """
[<AnAttribute; AnAttribute((* marker *)>]
type T =
    { F: int }
"""
    => None

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute; AnAttribute( | >]``() =
    """
[<AnAttribute; AnAttribute( (* marker *)>]
type T =
    { F: int }
"""
    => None

[<Test>]
let ``No AttributeApplication completion context at [<AnAttribute>][<AnAttribute; AnAttribute(| >]``() =
    """
[<AnAttribute>][<AnAttribute; AnAttribute((* marker *)>]
type T =
    { F: int }
"""
    => None
