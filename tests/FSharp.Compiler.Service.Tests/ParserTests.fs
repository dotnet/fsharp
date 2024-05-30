﻿module FSharp.Compiler.Service.Tests.ParserTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FsUnit
open Xunit

[<Fact>]
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


[<Fact>]
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


[<Fact>]
let ``Match clause 01`` () =
    let parseResults = getParseResults """
match () with
| x
"""
    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _) ]) -> ()
    | _ -> failwith "Unexpected tree"


[<Fact>]
let ``Match clause 02 - When`` () =
    let parseResults = getParseResults """
match () with
| x when true
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _) ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Fact>]
let ``Match clause 03 - When`` () =
    let parseResults = getParseResults """
match () with
| x when true
| _ -> ()
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _); _ ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Fact>]
let ``Match clause 04 - Or pat`` () =
    let parseResults = getParseResults """
match () with
| x
| _ -> ()
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (pat=SynPat.Or _;resultExpr=SynExpr.Const _) ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Fact>]
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

[<Fact>]
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

[<Fact>]
let ``Match clause 07`` () =
    let parseResults = getParseResults """
match () with
| (x,
| y -> ()
"""

    match getSingleExprInModule parseResults with
    | SynExpr.Match (clauses=[ SynMatchClause (pat=pat) ]) ->
        match pat with
        | SynPat.Paren(SynPat.Or(SynPat.Tuple(elementPats = [SynPat.Named _; SynPat.Wild _]), SynPat.Named _, _, _), _) -> ()
        | _ -> failwith "Unexpected pattern"
    | _ -> failwith "Unexpected tree"

[<Fact>]
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


[<Fact>]
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

[<Fact>]
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

[<Fact>]
let ``Let - Parameter - Paren 03 - Tuple`` () =
    let parseResults = getParseResults """
let f (x,
"""

    match getSingleDeclInModule parseResults with
    | SynModuleDecl.Let (_, [ SynBinding (headPat = SynPat.LongIdent (argPats = SynArgPats.Pats [ pat ])) ], _) ->
        match pat with
        | SynPat.FromParseError (SynPat.Paren (SynPat.Tuple(elementPats = [SynPat.Named _; SynPat.Wild _]), _), _) -> ()
        | _ -> failwith "Unexpected tree"
    | _ -> failwith "Unexpected tree"

let assertIsBefore (f: _ -> range) (a, b) =
    let r1 = f a
    let r2 = f b
    Position.posGeq r2.Start r1.End |> shouldEqual true

let inline assertIsEmptyRange node =
    let range = getRange node
    Position.posEq range.Start range.End |> shouldEqual true

let inline checkNodeOrder exprs =
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

[<Fact>]
let ``Expr - Tuple 07`` () =
    let parseResults = getParseResults """
let x = 1,
"""
    match getSingleModuleMemberDecls parseResults with
    | [ SynModuleDecl.Let(_, [ (SynBinding(expr = expr)) ], range) ] ->
        shouldEqual expr.Range.StartLine expr.Range.EndLine
        shouldEqual range.StartLine range.EndLine
    | _ -> failwith "Unexpected tree"

[<Fact>]
let ``Pattern - Head - Tuple 01`` () =
    let parseResults = getParseResults """
let , = ()
let ,, = ()
let ,,, = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map getLetDeclHeadPattern
    match pats with
    | [ SynPat.Tuple(elementPats = [SynPat.Wild _ as p11; SynPat.Wild _ as p12])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p21; SynPat.Wild _ as p22; SynPat.Wild _ as p23])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p31; SynPat.Wild _ as p32; SynPat.Wild _ as p33; SynPat.Wild _ as p34]) ] ->
            [ p11; p12; p21; p22; p23; p31; p32; p33; p34 ] |> checkNodeOrder
            [ p11; p12; p21; p22; p23; p31; p32; p33; p34 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

[<Fact>]
let ``Pattern - Head - Tuple 02`` () =
    let parseResults = getParseResults """
let 1, = ()
let ,1 = ()
let 1,1 = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map getLetDeclHeadPattern
    match pats with
    | [ SynPat.Tuple(elementPats = [SynPat.Const _ as p11; SynPat.Wild _ as p12])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p21; SynPat.Const _ as p22])
        SynPat.Tuple(elementPats = [SynPat.Const _ as p31; SynPat.Const _ as p32]) ] ->
            [ p11; p12; p21; p22; p31; p32 ] |> checkNodeOrder
            [ p12; p21 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

[<Fact>]
let ``Pattern - Head - Tuple 03`` () =
    let parseResults = getParseResults """
let 1,, = ()
let ,1, = ()
let ,,1 = ()

let 1,1, = ()
let ,1,1 = ()
let 1,,1 = ()

let 1,1,1 = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map getLetDeclHeadPattern
    match pats with
    | [ SynPat.Tuple(elementPats = [SynPat.Const _ as p11; SynPat.Wild _ as p12; SynPat.Wild _ as p13])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p21; SynPat.Const _ as p22; SynPat.Wild _ as p23])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p31; SynPat.Wild _ as p32; SynPat.Const _ as p33])

        SynPat.Tuple(elementPats = [SynPat.Const _ as p41; SynPat.Const _ as p42; SynPat.Wild _ as p43])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p51; SynPat.Const _ as p52; SynPat.Const _ as p53])
        SynPat.Tuple(elementPats = [SynPat.Const _ as p61; SynPat.Wild _ as p62; SynPat.Const _ as p63])
        
        SynPat.Tuple(elementPats = [SynPat.Const _ as p71; SynPat.Const _ as p72; SynPat.Const _ as p73]) ] ->
            [ p11; p12; p13; p21; p22; p23; p31; p32; p33
              p41; p42; p43; p51; p52; p53; p61; p62; p63
              p71; p72; p73 ] |> checkNodeOrder
            [ p12; p13; p21; p23; p31; p32; p43; p51; p62 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

let getParenPatInnerPattern pat =
    match pat with
    | SynPat.Paren(pat, _) -> pat
    | _ -> failwith "Unexpected tree"

[<Fact>]
let ``Pattern - Paren - Tuple 01`` () =
    let parseResults = getParseResults """
let (,) = ()
let (,,) = ()
let (,,,) = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map (getLetDeclHeadPattern >> getParenPatInnerPattern)
    match pats with
    | [ SynPat.Tuple(elementPats = [SynPat.Wild _ as p11; SynPat.Wild _ as p12])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p21; SynPat.Wild _ as p22; SynPat.Wild _ as p23])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p31; SynPat.Wild _ as p32; SynPat.Wild _ as p33; SynPat.Wild _ as p34]) ] ->
            [ p11; p12; p21; p22; p23; p31; p32; p33; p34 ] |> checkNodeOrder
            [ p11; p12; p21; p22; p23; p31; p32; p33; p34 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

[<Fact>]
let ``Pattern - Paren - Tuple 02`` () =
    let parseResults = getParseResults """
let (1,) = ()
let (,1) = ()
let (1,1) = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map (getLetDeclHeadPattern >> getParenPatInnerPattern)
    match pats with
    | [ SynPat.Tuple(elementPats = [SynPat.Const _ as p11; SynPat.Wild _ as p12])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p21; SynPat.Const _ as p22])
        SynPat.Tuple(elementPats = [SynPat.Const _ as p31; SynPat.Const _ as p32]) ] ->
            [ p11; p12; p21; p22; p31; p32 ] |> checkNodeOrder
            [ p12; p21 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

[<Fact>]
let ``Pattern - Paren - Tuple 03`` () =
    let parseResults = getParseResults """
let (1,,) = ()
let (,1,) = ()
let (,,1) = ()

let (1,1,) = ()
let (,1,1) = ()
let (1,,1) = ()

let (1,1,1) = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map (getLetDeclHeadPattern >> getParenPatInnerPattern)
    match pats with
    | [ SynPat.Tuple(elementPats = [SynPat.Const _ as p11; SynPat.Wild _ as p12; SynPat.Wild _ as p13])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p21; SynPat.Const _ as p22; SynPat.Wild _ as p23])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p31; SynPat.Wild _ as p32; SynPat.Const _ as p33])

        SynPat.Tuple(elementPats = [SynPat.Const _ as p41; SynPat.Const _ as p42; SynPat.Wild _ as p43])
        SynPat.Tuple(elementPats = [SynPat.Wild _ as p51; SynPat.Const _ as p52; SynPat.Const _ as p53])
        SynPat.Tuple(elementPats = [SynPat.Const _ as p61; SynPat.Wild _ as p62; SynPat.Const _ as p63])
        
        SynPat.Tuple(elementPats = [SynPat.Const _ as p71; SynPat.Const _ as p72; SynPat.Const _ as p73]) ] ->
            [ p11; p12; p13; p21; p22; p23; p31; p32; p33
              p41; p42; p43; p51; p52; p53; p61; p62; p63
              p71; p72; p73 ] |> checkNodeOrder
            [ p12; p13; p21; p23; p31; p32; p43; p51; p62 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"


[<Fact>]
let ``Match - Clause 01`` () =
    let parseResults = getParseResults """
match () with
| _ -> ()
| _, 
    """
    let exprs = getSingleExprInModule parseResults
    match exprs with
    | SynExpr.Match(_, _, [_; _], _, _) -> ()
    | _ -> failwith "Unexpected tree"
