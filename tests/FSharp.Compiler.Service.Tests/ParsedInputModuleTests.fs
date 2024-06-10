module FSharp.Compiler.Service.Tests.ParsedInputModuleTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text.Position
open Xunit

[<Fact>]
let ``tryPick type test`` () =
    let source = "123 :? int"
    let parseTree = parseSourceCode ("C:\\test.fs", source)

    (mkPos 1 11, parseTree)
    ||> ParsedInput.tryPick (fun _path node -> match node with SyntaxNode.SynType _ -> Some() | _ -> None)
    |> Option.defaultWith (fun _ -> failwith "Did not visit type")

    (mkPos 1 3, parseTree)
    ||> ParsedInput.tryPick (fun _path node -> match node with SyntaxNode.SynType _ -> Some() | _ -> None)
    |> Option.iter (fun _ -> failwith "Should not visit type")

[<Fact>]
let ``tryPick record definition test`` () =
    let source = "type R = { A: int; B: string }"
    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let fields =
        (pos0, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynTypeDefn(SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFields = fields), _))) -> Some fields
            | _ -> None)

    match fields with
    | Some [ SynField (idOpt = Some id1); SynField (idOpt = Some id2) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit record definition"

[<Fact>]
let ``tryPick union definition test`` () =
    let source = "type U = A | B of string"
    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let cases =
        (pos0, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynTypeDefn(SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Union(unionCases = cases), _))) -> Some cases
            | _ -> None)

    match cases with
    | Some [ SynUnionCase (ident = SynIdent(id1,_)); SynUnionCase (ident = SynIdent(id2,_)) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit union definition"

[<Fact>]
let ``tryPick enum definition test`` () =
    let source = "type E = A = 0 | B = 1"
    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let cases =
        (pos0, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynTypeDefn(SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Enum(cases = cases), _))) -> Some cases
            | _ -> None)

    match cases with
    | Some [ SynEnumCase (ident = SynIdent (id1, _)); SynEnumCase (ident = SynIdent (id2, _)) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit enum definition"

[<Fact>]
let ``tryPick recursive let binding`` () =
    let source = "let rec fib n = if n < 2 then n else fib (n - 1) + fib (n - 2) in fib 10"
    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let bindings =
        (pos0, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynExpr(SynExpr.LetOrUse(isRecursive = false)) -> failwith "isRecursive should be true"
            | SyntaxNode.SynExpr(SynExpr.LetOrUse(isRecursive = true; bindings = bindings)) -> Some bindings
            | _ -> None)

    match bindings with
    | Some [ SynBinding(valData = SynValData(valInfo = SynValInfo(curriedArgInfos = [ [ SynArgInfo(ident = Some id) ] ]))) ] when id.idText = "n" -> ()
    | _ -> failwith "Did not visit recursive let binding"

[<Fact>]
let ``tryPick ValSig`` () =
    let source = """
module X

val y: int -> int
"""

    let parseTree = parseSourceCode ("C:\\test.fsi", source)

    let ident =
        (pos0, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynValSig(SynValSig(ident = SynIdent(ident = ident))) -> Some ident.idText
            | _ -> None)

    match ident with
    | Some "y" -> ()
    | _ -> failwith "Did not visit SynValSig"

[<Fact>]
let ``tryPick nested ValSig`` () =
    let source = """
module X

module Y =
    val z: int -> int
"""

    let parseTree = parseSourceCode ("C:\\test.fsi", source)

    let ident =
        (pos0, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynValSig(SynValSig(ident = SynIdent(ident = ident))) -> Some ident.idText
            | _ -> None)

    match ident with
    | Some "z" -> ()
    | _ -> failwith "Did not visit SynValSig"

[<Fact>]
let ``tryPick Record in SynTypeDefnSig`` () =
    let source = """
module X

type Y =
    {
        A: int
        B: char
        C: string
    }
"""

    let parseTree = parseSourceCode ("C:\\test.fsi", source)

    let ident =
        (pos0, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynTypeDefnSig(SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFields = fields), _))) ->
                fields
                |> List.choose (function SynField(idOpt = Some ident) -> Some ident.idText | _ -> None)
                |> String.concat ","
                |> Some
            | _ -> None)

    match ident with
    | Some "A,B,C" -> ()
    | _ -> failwith "Did not visit SynTypeDefnSimpleRepr.Record in SynTypeDefnSig"

[<Fact>]
let ``tryPick SynValSig in SynMemberSig`` () =
    let source = """
module Lib

type Meh =
    new: unit -> Meh
    member Foo: y: int -> int
"""

    let parseTree = parseSourceCode ("C:\\test.fsi", source)
    let pos = mkPos 6 4

    let ident =
        (pos, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynValSig(SynValSig(ident = SynIdent(ident = valIdent))) -> Some valIdent.idText
            | _ -> None)

    match ident with
    | Some "Foo" -> ()
    | _ -> failwith "Did not visit SynValSig in SynMemberSig.Member"

[<Fact>]
let ``tryPick picks the first matching node`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let ``module`` =
        (mkPos 6 28, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                Some(longIdent |> List.map (fun ident -> ident.idText))
            | _ -> None)

    Assert.Equal(Some ["N"], ``module``)

[<Fact>]
let ``tryPick falls back to the nearest matching node to the left if pos is out of range`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let ``module`` =
        (mkPos 7 30, parseTree)
        ||> ParsedInput.tryPick (fun _path node ->
            match node with
            | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                Some(longIdent |> List.map (fun ident -> ident.idText))
            | _ -> None)

    Assert.Equal(Some ["N"], ``module``)

[<Fact>]
let ``tryPickLast picks the last matching node`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let ``module`` =
        (mkPos 6 28, parseTree)
        ||> ParsedInput.tryPickLast (fun _path node ->
            match node with
            | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                Some(longIdent |> List.map (fun ident -> ident.idText))
            | _ -> None)

    Assert.Equal(Some ["P"], ``module``)

[<Fact>]
let ``tryPickLast falls back to the nearest matching node to the left if pos is out of range`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let ``module`` =
        (mkPos 7 30, parseTree)
        ||> ParsedInput.tryPickLast (fun _path node ->
            match node with
            | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                Some(longIdent |> List.map (fun ident -> ident.idText))
            | _ -> None)

    Assert.Equal(Some ["P"], ``module``)

[<Fact>]
let ``exists returns true for the first matching node`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let mutable start = 0, 0

    let found =
        (mkPos 6 28, parseTree)
        ||> ParsedInput.exists (fun _path node ->
            match node with
            | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                start <- node.Range.StartLine, node.Range.StartColumn
                true
            | _ -> false)

    Assert.True found
    Assert.Equal((4, 0), start)

[<Fact>]
let ``exists falls back to the nearest matching node to the left if pos is out of range`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let mutable start = 0, 0

    let found =
        (mkPos 7 30, parseTree)
        ||> ParsedInput.exists (fun _path node ->
            match node with
            | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                start <- node.Range.StartLine, node.Range.StartColumn
                true
            | _ -> false)

    Assert.True found
    Assert.Equal((4, 0), start)

[<Fact>]
let ``tryNode picks the last node containing the given position`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let ``module`` =
        parseTree
        |> ParsedInput.tryNode (mkPos 6 28)
        |> Option.bind (fun (node, _path) ->
            match node with
            | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                Some(longIdent |> List.map (fun ident -> ident.idText))
            | _ -> None)

    Assert.Equal(Some ["P"], ``module``)

[<Fact>]
let ``tryNode returns None if no node contains the given position`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let ``module`` = parseTree |> ParsedInput.tryNode (mkPos 6 30)

    Assert.Equal(None, ``module``)

[<Fact>]
let ``fold traverses nodes in order`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end

module Q =
    module R =
        module S = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let modules =
        ([], parseTree)
        ||> ParsedInput.fold (fun acc _path node ->
            match node with
            | SyntaxNode.SynModuleOrNamespace(SynModuleOrNamespace(longId = longIdent))
            | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                (longIdent |> List.map (fun ident -> ident.idText)) :: acc
            | _ -> acc)

    Assert.Equal<string list list>(
        [["M"]; ["N"]; ["O"]; ["P"]; ["Q"]; ["R"]; ["S"]],
        List.rev modules)

[<Fact>]
let ``foldWhile traverses nodes in order`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end

module Q =
    module R =
        module S = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let modules =
        ([], parseTree)
        ||> ParsedInput.foldWhile (fun acc _path node ->
            match node with
            | SyntaxNode.SynModuleOrNamespace(SynModuleOrNamespace(longId = longIdent))
            | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                Some((longIdent |> List.map (fun ident -> ident.idText)) :: acc)
            | _ -> Some acc)

    Assert.Equal<string list list>(
        [["M"]; ["N"]; ["O"]; ["P"]; ["Q"]; ["R"]; ["S"]],
        List.rev modules)

[<Fact>]
let ``foldWhile traverses nodes in order until the folder returns None`` () =
    let source = """
module M

module N =
    module O =
        module P = begin end

module Q =
    module R =
        module S = begin end
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    let modules =
        ([], parseTree)
        ||> ParsedInput.foldWhile (fun acc _path node ->
            if posGt node.Range.Start (mkPos 7 0) then None
            else
                match node with
                | SyntaxNode.SynModuleOrNamespace(SynModuleOrNamespace(longId = longIdent))
                | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longIdent))) ->
                    Some((longIdent |> List.map (fun ident -> ident.idText)) :: acc)
                | _ -> Some acc)

    Assert.Equal<string list list>(
        [["M"]; ["N"]; ["O"]; ["P"]],
        List.rev modules)
