module FSharp.Compiler.Service.Tests.Parser.Recovery

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FsUnit
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
            cases |> List.map (fun (SynUnionCase (ident = SynIdent(ident,_))) -> ident.idText) |> Some
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
        | SynPat.FromParseError (SynPat.Paren (SynPat.Or (SynPat.Named _, SynPat.Named _, _, _), _), _) -> ()
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
             SynPat.Named _, _, _) -> ()
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
    | [ SynModuleDecl.Expr (expr=(SynExpr.Match _ as m)); SynModuleDecl.Expr (expr=(SynExpr.Ident _ as i)) ] ->
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

let assertIsBefore (f: _ -> range) (a, b) =
    let r1 = f a
    let r2 = f b
    Position.posGeq r2.Start r1.End |> shouldEqual true

let checkExprOrder exprs =
    exprs
    |> List.pairwise
    |> List.iter (assertIsBefore getRange)

let checkRangeCountAndOrder commas =
    commas
    |> List.iter (fun (commas, length) ->
        List.length commas |> shouldEqual length

        commas
        |> List.pairwise
        |> List.iter (assertIsBefore id))

[<Test>]
let ``Expr - Tuple 01`` () =
    let parseResults = getParseResults """
(,)
(,,)
"""
    let exprs = getSingleModuleMemberDecls parseResults |> List.map getSingleParenInnerExpr
    match exprs with
    | [ SynExpr.ArbitraryAfterError _
        SynExpr.ArbitraryAfterError _ ] -> ()

    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Expr - Tuple 02`` () =
    let parseResults = getParseResults """
(1,)
(,1)
(1,1)
"""
    let exprs = getSingleModuleMemberDecls parseResults |> List.map getSingleParenInnerExpr    
    match exprs with
    | [ SynExpr.Tuple(_, [SynExpr.Const _ as e11; SynExpr.ArbitraryAfterError _ as e12], c1, _)
        SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _  as e21; SynExpr.Const _  as e22], c2, _)
        SynExpr.Tuple(_, [SynExpr.Const _  as e31; SynExpr.Const _  as e32], c3, _) ] ->
            [ e11; e12; e21; e22; e31; e32 ] |> checkExprOrder
            [ c1, 1; c2, 1; c3, 1 ] |> checkRangeCountAndOrder
    
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Expr - Tuple 03`` () =
    let parseResults = getParseResults """
(1,,) // two items are produced
(,1,)
(,,1)

(1,1,)
(,1,1)
(1,,1)

(1,1,1)
"""
    let exprs = getSingleModuleMemberDecls parseResults |> List.map getSingleParenInnerExpr    
    match exprs with
    | [ SynExpr.Tuple(_, [SynExpr.Const _ as e11; SynExpr.ArbitraryAfterError _  as e12], c1, _)
        SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _ as e21; SynExpr.Const _ as e22; SynExpr.ArbitraryAfterError _ as e23], c2, _)
        SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _ as e31; SynExpr.ArbitraryAfterError _ as e32; SynExpr.Const _ as e33], c3, _)

        SynExpr.Tuple(_, [SynExpr.Const _ as e41; SynExpr.Const _ as e42; SynExpr.ArbitraryAfterError _ as e43], c4, _)
        SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _ as e51; SynExpr.Const _ as e52; SynExpr.Const _ as e53], c5, _)
        SynExpr.Tuple(_, [SynExpr.Const _ as e61; SynExpr.ArbitraryAfterError _ as e62; SynExpr.Const _ as e63], c6, _)

        SynExpr.Tuple(_, [SynExpr.Const _ as e71; SynExpr.Const _ as e72; SynExpr.Const _ as e73], c7, _) ] ->
            [ e11; e12; e21; e22; e23; e31; e32; e33
              e41; e42; e43; e51; e52; e53; e61; e62; e63
              e71; e72; e73 ]
            |> checkExprOrder

            [ c1, 1; c2, 2; c3, 2
              c4, 2; c5, 2; c6, 2
              c7, 2 ]
            |> checkRangeCountAndOrder

    | _ -> failwith "Unexpected tree"


[<Test>]
let ``Expr - Tuple 04`` () =
    let parseResults = getParseResults """
(,1,,2,3,,4,)
"""
    let exprs = getSingleModuleMemberDecls parseResults |> List.map getSingleParenInnerExpr
    match exprs with
    | [ SynExpr.Tuple(_, [ SynExpr.ArbitraryAfterError _ as e1
                           SynExpr.Const _ as e2
                           SynExpr.ArbitraryAfterError _ as e3
                           SynExpr.Const _ as e4
                           SynExpr.Const _ as e5
                           SynExpr.ArbitraryAfterError _  as e6
                           SynExpr.Const _ as e7
                           SynExpr.ArbitraryAfterError _  as e8 ], c, _) ] ->
            [ e1; e2; e3; e4; e5; e6; e7; e8 ]
            |> checkExprOrder

            [ c, 7 ] |> checkRangeCountAndOrder

    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Expr - Tuple 05`` () =
    let parseResults = getParseResults """
(1,
"""
    match getSingleModuleMemberDecls parseResults |> List.map getSingleParenInnerExpr with
    | [ SynExpr.FromParseError(SynExpr.Tuple(_, [SynExpr.Const _; SynExpr.ArbitraryAfterError _], _, _), _) ] -> ()
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Expr - Tuple 06`` () =
    let parseResults = getParseResults """
(1,,,2)
"""
    let synExprs = getSingleModuleMemberDecls parseResults |> List.map getSingleParenInnerExpr
    match synExprs with
    | [ SynExpr.Tuple(_, [ SynExpr.Const _
                           SynExpr.ArbitraryAfterError _
                           SynExpr.ArbitraryAfterError _
                           SynExpr.Const _ ], _, _) ] -> ()
    | _ -> failwith "Unexpected tree"