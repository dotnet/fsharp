module FSharp.Compiler.Service.Tests.Parser.Recovery

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open NUnit.Framework

[<Test>]
let ``Interface impl - No members`` () =
    let parseResults = getParseResults """
type T =
    interface I with
    member x.P2 = ()

let x = ()
"""
    match getSingleModuleMemberDecls parseResults with
    | [ SynModuleDecl.Types ([ SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members = [ _; _ ])) ], _)
        SynModuleDecl.Let _ ] -> ()
    | _ -> failwith "Unexpected tree"


[<Test>]
let ``Union case 01 - of`` () =
    let parseResults = getParseResults """
type U1 =
    | A of

type U2 =
    | B of
    | C

let x = ()
"""
    let (|UnionWithCases|_|) typeDefn =
        match typeDefn with
        | SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (SynTypeDefnSimpleRepr.Union (unionCases = cases), _)) ->
            cases |> List.map (fun (SynUnionCase (ident = ident)) -> ident.idText) |> Some
        | _ -> None

    match getSingleModuleMemberDecls parseResults with
    | [ SynModuleDecl.Types ([ UnionWithCases ["A"]], _)
        SynModuleDecl.Types ([ UnionWithCases ["B"; "C"] ], _)
        SynModuleDecl.Let _ ] -> ()
    | _ -> failwith "Unexpected tree"


[<Test>]
let ``Match clause 01`` () =
    let parseResults = getParseResults """
match () with
| x
"""
    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _) ]) -> ()
    | _ -> failwith "Unexpected tree"


[<Test>]
let ``Match clause 02 - When`` () =
    let parseResults = getParseResults """
match () with
| x when true
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _) ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 03 - When`` () =
    let parseResults = getParseResults """
match () with
| x when true
| _ -> ()
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _); _ ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 04 - Or pat`` () =
    let parseResults = getParseResults """
match () with
| x
| _ -> ()
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (pat=SynPat.Or _;resultExpr=SynExpr.Const _) ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 05 - Missing body`` () =
    let parseResults = getParseResults """
match () with
| x ->
| _ -> ()
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _)
                               SynMatchClause (resultExpr=SynExpr.Const _) ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 06`` () =
    let parseResults = getParseResults """
match () with
| (x
| y -> ()
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (pat=pat) ]) ->
        match pat with
        | SynPat.FromParseError (SynPat.Paren (SynPat.Or (SynPat.Named _, SynPat.Named _, _), _), _) -> ()
        | _ -> failwith "Unexpected pattern"
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 07`` () =
    let parseResults = getParseResults """
match () with
| (x,
| y -> ()
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (pat=pat) ]) ->
        match pat with
        | SynPat.Or
            (SynPat.FromParseError (SynPat.Paren (SynPat.FromParseError (SynPat.Wild _, _), _), _),
             SynPat.Named _, _) -> ()
        | _ -> failwith "Unexpected pattern"
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 08 - Range`` () =
    let parseResults = getParseResults """
match () with
| a
b
"""
    match getSingleModuleMemberDecls parseResults with
    | [ SynModuleDecl.DoExpr (_, (SynExpr.Match _ as m), _); SynModuleDecl.DoExpr (_, (SynExpr.Ident _ as i), _) ] ->
        Assert.True(Position.posLt m.Range.End i.Range.Start)
    | _ -> failwith "Unexpected tree"


[<Test>]
let ``Let - Parameter - Paren 01`` () =
    let parseResults = getParseResults """
let f (x
"""

    match getSingleDeclInModule parseResults with
    | SynModuleDecl.Let (_, [ SynBinding (headPat = headPat) ], _) ->
        match headPat with
        | SynPat.LongIdent (argPats=SynArgPats.Pats [ SynPat.FromParseError (SynPat.Paren (SynPat.Named _, _), _) ]) -> ()
        | _ -> failwith "Unexpected tree"
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Let - Parameter - Paren 02 - Tuple`` () =
    let parseResults = getParseResults """
let f (x, y
"""

    match getSingleDeclInModule parseResults with
    | SynModuleDecl.Let (_, [ SynBinding (headPat = headPat) ], _) ->
        match headPat with
        | SynPat.LongIdent (argPats=SynArgPats.Pats [ SynPat.FromParseError (SynPat.Paren (SynPat.Tuple _, _), _) ]) -> ()
        | _ -> failwith "Unexpected tree"
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Let - Parameter - Paren 03 - Tuple`` () =
    let parseResults = getParseResults """
let f (x,
"""

    match getSingleDeclInModule parseResults with
    | SynModuleDecl.Let (_, [ SynBinding (headPat = SynPat.LongIdent (argPats = SynArgPats.Pats [ pat ])) ], _) ->
        match pat with
        | SynPat.FromParseError (SynPat.Paren (SynPat.FromParseError (SynPat.Wild _, _), _), _) -> ()
        | _ -> failwith "Unexpected tree"
    | _ -> failwith "Unexpected tree"
