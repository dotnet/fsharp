#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.ServiceUntypedParseTests
#endif

open System.IO
open FsUnit
open FSharp.Compiler.Ast
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Service.Tests.Common
open NUnit.Framework

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



[<Test>]
let ``Attribute lists`` () =
    let source = """
[<A>]
let foo1 = ()

[<A>]
[<B;C>]
let foo2 = ()

[<A>] [<B;C>]
let foo3 = ()

[<A
let foo4 = ()

[<A;
let foo5 = ()

[<
let foo6 = ()

[<>]
let foo7 = ()
"""
    match parseSourceCode ("test", source) with
    | Some (ParsedInput.ImplFile (ParsedImplFileInput (_,_,_,_,_,[SynModuleOrNamespace (_,_,_,decls,_,_,_,_)],_))) ->
        decls |> List.map (fun decl ->
            match decl with
            | SynModuleDecl.Let (_,[Binding(_,_,_,_,attributeLists,_,_,_,_,_,_,_)],_) ->
                attributeLists |> List.map (fun list ->
                    let r = list.Range

                    list.Attributes.Length,
                    ((r.StartLine, r.StartColumn), (r.EndLine, r.EndColumn)))

            | _ -> failwith "Could not get binding")
        |> shouldEqual
            [ [ (1, ((2,  0),  (2, 5))) ]
              [ (1, ((5,  0),  (5, 5))); (2, ((6, 0), (6, 7))) ]
              [ (1, ((9,  0),  (9, 5))); (2, ((9, 6), (9, 13))) ]
              [ (1, ((12, 0), (13, 0))) ]
              [ (1, ((15, 0), (15, 4))) ]
              [ (0, ((18, 0), (18, 2))) ]
              [ (0, ((21, 0), (21, 4))) ] ]

    | _ -> failwith "Could not get module decls"
