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
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.SyntaxTree
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

[<A;>]
let foo8 = ()
"""
    let (SynModuleOrNamespace (decls = decls)) = parseSourceCodeAndGetModule source
    decls |> List.map (fun decl ->
        match decl with
        | SynModuleDecl.Let (_, [Binding (attributes = attributeLists)], _) ->
            attributeLists |> List.map (fun list -> list.Attributes.Length, getRangeCoords list.Range)
        | _ -> failwith "Could not get binding")
    |> shouldEqual
        [ [ (1, ((2,  0),  (2, 5))) ]
          [ (1, ((5,  0),  (5, 5))); (2, ((6, 0), (6, 7))) ]
          [ (1, ((9,  0),  (9, 5))); (2, ((9, 6), (9, 13))) ]
          [ (1, ((12, 0), (13, 0))) ]
          [ (1, ((15, 0), (15, 4))) ]
          [ (0, ((18, 0), (18, 2))) ]
          [ (0, ((21, 0), (21, 4))) ]
          [ (1, ((24, 0), (24, 6))) ] ]


let rec getParenTypes (synType: SynType): SynType list =
    [ match synType with
      | SynType.Paren (innerType, _) ->
          yield synType
          yield! getParenTypes innerType

      | SynType.Fun (argType, returnType, _) ->
          yield! getParenTypes argType
          yield! getParenTypes returnType

      | SynType.Tuple (_, types, _) ->
          for _, synType in types do
              yield! getParenTypes synType

      | SynType.AnonRecd (_, fields, _) ->
          for _, synType in fields do
              yield! getParenTypes synType

      | SynType.App (typeName = typeName; typeArgs = typeArgs)
      | SynType.LongIdentApp (typeName = typeName; typeArgs = typeArgs) ->
          yield! getParenTypes typeName
          for synType in typeArgs do
              yield! getParenTypes synType

      | _ -> () ]

[<Test>]
let ``SynType.Paren ranges`` () =
    let source = """
((): int * (int * int))
((): (int -> int) -> int)
((): ((int)))
"""
    let (SynModuleOrNamespace (decls = decls)) = parseSourceCodeAndGetModule source
    decls |> List.map (fun decl ->
        match decl with
        | SynModuleDecl.DoExpr (expr = SynExpr.Paren (expr = SynExpr.Typed (_, synType ,_))) ->
            getParenTypes synType
            |> List.map (fun synType -> getRangeCoords synType.Range)
        | _ -> failwith "Could not get binding")
    |> shouldEqual
        [ [ (2, 11), (2, 22) ]
          [ (3, 5), (3, 17) ]
          [ (4, 5), (4, 12); (4, 6), (4, 11) ] ]


module TypeMemberRanges =

    let getTypeMemberRange source =
        let (SynModuleOrNamespace (decls = decls)) = parseSourceCodeAndGetModule source
        match decls with
        | [ SynModuleDecl.Types ([ TypeDefn (_, SynTypeDefnRepr.ObjectModel (_, memberDecls, _), _, _) ], _) ] ->
            memberDecls |> List.map (fun memberDecl -> getRangeCoords memberDecl.Range)
        | _ -> failwith "Could not get member"

    
    [<Test>]
    let ``Member range 01 - Simple``() =
        let source = """
type T =
    member x.Foo() = ()
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (3, 23) ]

    
    [<Test>]
    let ``Member range 02 - Static``() =
        let source = """
type T =
    static member Foo() = ()
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (3, 28) ]


    [<Test>]
    let ``Member range 03 - Attribute``() =
        let source = """
type T =
    [<Foo>]
    static member Foo() = ()
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (4, 28) ]


    [<Test>]
    let ``Member range 04 - Property``() =
        let source = """
type T =
    member x.P = ()
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (3, 19) ]


    [<Test>]
    let ``Member range 05 - Setter only property``() =
        let source = """
type T =
    member x.P with set (value) = v <- value
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (3, 44) ]

    
    [<Test>]
    let ``Member range 06 - Read-write property``() =
        let source = """
type T =
    member this.MyReadWriteProperty
        with get () = x
        and set (value) = x <- value
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (5, 36)
                                                   (3, 4), (5, 36) ]


    [<Test>]
    let ``Member range 07 - Auto property``() =
        let source = """
type T =
    member val Property1 = ""
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (3, 29) ]


    [<Test>]
    let ``Member range 08 - Auto property with setter``() =
        let source = """
type T =
    member val Property1 = "" with get, set
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (3, 29) ]

    
    [<Test>]
    let ``Member range 09 - Abstract slot``() =
        let source = """
type T =
    abstract P: int
    abstract M: unit -> unit
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (3, 19)
                                                   (4, 4), (4, 28) ]

    [<Test>]
    let ``Member range 10 - Val field``() =
        let source = """
type T =
    val x: int
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (3, 14) ]


    [<Test>]
    let ``Member range 11 - Ctor``() =
        let source = """
type T =
    new (x:int) = ()
"""
        getTypeMemberRange source |> shouldEqual [ (3, 4), (3, 20) ]
