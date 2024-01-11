module Tests.Service.AstModuleTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text.Position
open Xunit

[<Fact>]
let ``tryPick type test`` () =
    let source = "123 :? int"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    parseTree.Contents
    |> Ast.tryPick (mkPos 1 11) (fun _path node -> match node with SyntaxNode.SynType _ -> Some() | _ -> None)
    |> Option.defaultWith (fun _ -> failwith "Did not visit type")

    parseTree.Contents
    |> Ast.tryPick (mkPos 1 3) (fun _path node -> match node with SyntaxNode.SynType _ -> Some() | _ -> None)
    |> Option.iter (fun _ -> failwith "Should not visit type")

[<Fact>]
let ``tryPick record definition test`` () =
    let source = "type R = { A: int; B: string }"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    let fields =
        parseTree.Contents
        |> Ast.tryPick pos0 (fun _path node ->
            match node with
            | SyntaxNode.SynTypeDefn(SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFields = fields), _))) -> Some fields
            | _ -> None)

    match fields with
    | Some [ SynField (idOpt = Some id1); SynField (idOpt = Some id2) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit record definition"

[<Fact>]
let ``tryPick union definition test`` () =
    let source = "type U = A | B of string"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    let cases =
        parseTree.Contents
        |> Ast.tryPick pos0 (fun _path node ->
            match node with
            | SyntaxNode.SynTypeDefn(SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Union(unionCases = cases), _))) -> Some cases
            | _ -> None)

    match cases with
    | Some [ SynUnionCase (ident = SynIdent(id1,_)); SynUnionCase (ident = SynIdent(id2,_)) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit union definition"

[<Fact>]
let ``tryPick enum definition test`` () =
    let source = "type E = A = 0 | B = 1"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    let cases =
        parseTree.Contents
        |> Ast.tryPick pos0 (fun _path node ->
            match node with
            | SyntaxNode.SynTypeDefn(SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Enum(cases = cases), _))) -> Some cases
            | _ -> None)

    match cases with
    | Some [ SynEnumCase (ident = SynIdent (id1, _)); SynEnumCase (ident = SynIdent (id2, _)) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit enum definition"

[<Fact>]
let ``tryPick recursive let binding`` () =
    let source = "let rec fib n = if n < 2 then n else fib (n - 1) + fib (n - 2) in fib 10"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    let bindings =
        parseTree.Contents
        |> Ast.tryPick pos0 (fun _path node ->
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

    let parseTree = parseSourceCode("C:\\test.fsi", source)

    let ident =
        parseTree.Contents
        |> Ast.tryPick pos0 (fun _path node ->
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

    let parseTree = parseSourceCode("C:\\test.fsi", source)

    let ident =
        parseTree.Contents
        |> Ast.tryPick pos0 (fun _path node ->
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

    let parseTree = parseSourceCode("C:\\test.fsi", source)

    let ident =
        parseTree.Contents
        |> Ast.tryPick pos0 (fun _path node ->
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

    let parseTree = parseSourceCode("C:\\test.fsi", source)
    let pos = mkPos 6 4

    let ident =
        parseTree.Contents
        |> Ast.tryPick pos (fun _path node ->
            match node with
            | SyntaxNode.SynValSig(SynValSig(ident = SynIdent(ident = valIdent))) -> Some valIdent.idText
            | _ -> None)

    match ident with
    | Some "Foo" -> ()
    | _ -> failwith "Did not visit SynValSig in SynMemberSig.Member"
