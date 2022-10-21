module FSharp.Compiler.Service.Tests.Parser.Recovery

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FsUnit
open NUnit.Framework

[<Test>]
let ``Interface impl - No members`` () =
    use _ = FileIndex.setTestSource """
type T =
    interface I with
    member x.P2 = ()

let x = ()
"""
    match parseTestSource () |> getSingleModuleMemberDecls with
    | [ SynModuleDecl.Types ([ SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members = [ _; _ ])) ], _)
        SynModuleDecl.Let _ ] -> ()
    | _ -> failwith "Unexpected tree"


[<Test>]
let ``Union case 01 - of`` () =
    use _ = FileIndex.setTestSource """
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

    match parseTestSource () |> getSingleModuleMemberDecls with
    | [ SynModuleDecl.Types ([ UnionWithCases ["A"]], _)
        SynModuleDecl.Types ([ UnionWithCases ["B"; "C"] ], _)
        SynModuleDecl.Let _ ] -> ()
    | _ -> failwith "Unexpected tree"


[<Test>]
let ``Match clause 01`` () =
    use _ = FileIndex.setTestSource """
match () with
| x
"""
    match parseTestSource () |> getSingleExprInModule with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _) ]) -> ()
    | _ -> failwith "Unexpected tree"


[<Test>]
let ``Match clause 02 - When`` () =
    use _ = FileIndex.setTestSource """
match () with
| x when true
"""

    match parseTestSource () |> getSingleExprInModule with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _) ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 03 - When`` () =
    use _ = FileIndex.setTestSource """
match () with
| x when true
| _ -> ()
"""

    match parseTestSource () |> getSingleExprInModule with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _); _ ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 04 - Or pat`` () =
    use _ = FileIndex.setTestSource """
match () with
| x
| _ -> ()
"""

    match parseTestSource () |> getSingleExprInModule with
    | SynExpr.Match (clauses=[ SynMatchClause (pat=SynPat.Or _;resultExpr=SynExpr.Const _) ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 05 - Missing body`` () =
    use _ = FileIndex.setTestSource """
match () with
| x ->
| _ -> ()
"""

    match parseTestSource () |> getSingleExprInModule with
    | SynExpr.Match (clauses=[ SynMatchClause (resultExpr=SynExpr.ArbitraryAfterError _)
                               SynMatchClause (resultExpr=SynExpr.Const _) ]) -> ()
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 06`` () =
    use _ = FileIndex.setTestSource """
match () with
| (x
| y -> ()
"""

    match parseTestSource () |> getSingleExprInModule with
    | SynExpr.Match (clauses=[ SynMatchClause (pat=pat) ]) ->
        match pat with
        | SynPat.FromParseError (SynPat.Paren (SynPat.Or (SynPat.Named _, SynPat.Named _, _, _), _), _) -> ()
        | _ -> failwith "Unexpected pattern"
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 07`` () =
    use _ = FileIndex.setTestSource """
match () with
| (x,
| y -> ()
"""

    match parseTestSource () |> getSingleExprInModule with
    | SynExpr.Match (clauses=[ SynMatchClause (pat=pat) ]) ->
        match pat with
        | SynPat.Paren(SynPat.Or(SynPat.Tuple(_, [SynPat.Named _; SynPat.Wild _], _), SynPat.Named _, _, _), _) -> ()
        | _ -> failwith "Unexpected pattern"
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Match clause 08 - Range`` () =
    use _ = FileIndex.setTestSource """
match () with
| a
b
"""
    match parseTestSource () |> getSingleModuleMemberDecls with
    | [ SynModuleDecl.Expr (expr=(SynExpr.Match _ as m)); SynModuleDecl.Expr (expr=(SynExpr.Ident _ as i)) ] ->
        Assert.True(Position.posLt m.Range.End i.Range.Start)
    | _ -> failwith "Unexpected tree"


[<Test>]
let ``Let - Parameter - Paren 01`` () =
    use _ = FileIndex.setTestSource """
let f (x
"""

    match parseTestSource () |> getSingleDeclInModule with
    | SynModuleDecl.Let (_, [ SynBinding (headPat = headPat) ], _) ->
        match headPat with
        | SynPat.LongIdent (argPats=SynArgPats.Pats [ SynPat.FromParseError (SynPat.Paren (SynPat.Named _, _), _) ]) -> ()
        | _ -> failwith "Unexpected tree"
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Let - Parameter - Paren 02 - Tuple`` () =
    use _ = FileIndex.setTestSource """
let f (x, y
"""

    match parseTestSource () |> getSingleDeclInModule with
    | SynModuleDecl.Let (_, [ SynBinding (headPat = headPat) ], _) ->
        match headPat with
        | SynPat.LongIdent (argPats=SynArgPats.Pats [ SynPat.FromParseError (SynPat.Paren (SynPat.Tuple _, _), _) ]) -> ()
        | _ -> failwith "Unexpected tree"
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Let - Parameter - Paren 03 - Tuple`` () =
    use _ = FileIndex.setTestSource """
let f (x,
"""

    match parseTestSource () |> getSingleDeclInModule with
    | SynModuleDecl.Let (_, [ SynBinding (headPat = SynPat.LongIdent (argPats = SynArgPats.Pats [ pat ])) ], _) ->
        match pat with
        | SynPat.FromParseError (SynPat.Paren (SynPat.Tuple(_, [SynPat.Named _; SynPat.Wild _], _), _), _) -> ()
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

[<Test>]
let ``Expr - Tuple 01`` () =
    let parseResults = getParseResults """
(,)
(,,)
(,,,)
"""
    let exprs = getSingleModuleMemberDecls parseResults |> List.map getSingleParenInnerExpr
    match exprs with
    | [ SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _ as e11; SynExpr.ArbitraryAfterError _ as e12], c1, _)
        SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _ as e21; SynExpr.ArbitraryAfterError _ as e22; SynExpr.ArbitraryAfterError _ as e23], c2, _)
        SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _ as e31; SynExpr.ArbitraryAfterError _ as e32; SynExpr.ArbitraryAfterError _ as e33; SynExpr.ArbitraryAfterError _ as e34], c3, _) ] ->
            [ e11; e12; e21; e22; e23; e31; e32; e33; e34 ] |> checkNodeOrder
            [ c1, 1; c2, 2; c3, 3 ] |> checkRangeCountAndOrder

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
            [ e11; e12; e21; e22; e31; e32 ] |> checkNodeOrder
            [ c1, 1; c2, 1; c3, 1 ] |> checkRangeCountAndOrder
    
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Expr - Tuple 03`` () =
    let parseResults = getParseResults """
(1,,)
(,1,)
(,,1)

(1,1,)
(,1,1)
(1,,1)

(1,1,1)
"""
    let exprs = getSingleModuleMemberDecls parseResults |> List.map getSingleParenInnerExpr    
    match exprs with
    | [ SynExpr.Tuple(_, [SynExpr.Const _ as e11; SynExpr.ArbitraryAfterError _  as e12; SynExpr.ArbitraryAfterError _  as e13], c1, _)
        SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _ as e21; SynExpr.Const _ as e22; SynExpr.ArbitraryAfterError _ as e23], c2, _)
        SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _ as e31; SynExpr.ArbitraryAfterError _ as e32; SynExpr.Const _ as e33], c3, _)

        SynExpr.Tuple(_, [SynExpr.Const _ as e41; SynExpr.Const _ as e42; SynExpr.ArbitraryAfterError _ as e43], c4, _)
        SynExpr.Tuple(_, [SynExpr.ArbitraryAfterError _ as e51; SynExpr.Const _ as e52; SynExpr.Const _ as e53], c5, _)
        SynExpr.Tuple(_, [SynExpr.Const _ as e61; SynExpr.ArbitraryAfterError _ as e62; SynExpr.Const _ as e63], c6, _)

        SynExpr.Tuple(_, [SynExpr.Const _ as e71; SynExpr.Const _ as e72; SynExpr.Const _ as e73], c7, _) ] ->
            [ e11; e12; e13; e21; e22; e23; e31; e32; e33
              e41; e42; e43; e51; e52; e53; e61; e62; e63
              e71; e72; e73 ]
            |> checkNodeOrder

            [ c1, 2; c2, 2; c3, 2
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
            [ e1; e2; e3; e4; e5; e6; e7; e8 ] |> checkNodeOrder
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

[<Test>]
let ``Expr - Tuple 07`` () =
    let parseResults = getParseResults """
let x = 1,
"""
    match getSingleModuleMemberDecls parseResults with
    | [ SynModuleDecl.Let(_, [ (SynBinding(expr = expr)) ], range) ] ->
        shouldEqual expr.Range.StartLine expr.Range.EndLine
        shouldEqual range.StartLine range.EndLine
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Pattern - Head - Tuple 01`` () =
    let parseResults = getParseResults """
let , = ()
let ,, = ()
let ,,, = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map getLetDeclHeadPattern
    match pats with
    | [ SynPat.Tuple(_, [SynPat.Wild _ as p11; SynPat.Wild _ as p12], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p21; SynPat.Wild _ as p22; SynPat.Wild _ as p23], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p31; SynPat.Wild _ as p32; SynPat.Wild _ as p33; SynPat.Wild _ as p34], _) ] ->
            [ p11; p12; p21; p22; p23; p31; p32; p33; p34 ] |> checkNodeOrder
            [ p11; p12; p21; p22; p23; p31; p32; p33; p34 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Pattern - Head - Tuple 02`` () =
    let parseResults = getParseResults """
let 1, = ()
let ,1 = ()
let 1,1 = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map getLetDeclHeadPattern
    match pats with
    | [ SynPat.Tuple(_, [SynPat.Const _ as p11; SynPat.Wild _ as p12], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p21; SynPat.Const _ as p22], _)
        SynPat.Tuple(_, [SynPat.Const _ as p31; SynPat.Const _ as p32], _) ] ->
            [ p11; p12; p21; p22; p31; p32 ] |> checkNodeOrder
            [ p12; p21 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

[<Test>]
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
    | [ SynPat.Tuple(_, [SynPat.Const _ as p11; SynPat.Wild _ as p12; SynPat.Wild _ as p13], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p21; SynPat.Const _ as p22; SynPat.Wild _ as p23], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p31; SynPat.Wild _ as p32; SynPat.Const _ as p33], _)

        SynPat.Tuple(_, [SynPat.Const _ as p41; SynPat.Const _ as p42; SynPat.Wild _ as p43], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p51; SynPat.Const _ as p52; SynPat.Const _ as p53], _)
        SynPat.Tuple(_, [SynPat.Const _ as p61; SynPat.Wild _ as p62; SynPat.Const _ as p63], _)
        
        SynPat.Tuple(_, [SynPat.Const _ as p71; SynPat.Const _ as p72; SynPat.Const _ as p73], _) ] ->
            [ p11; p12; p13; p21; p22; p23; p31; p32; p33
              p41; p42; p43; p51; p52; p53; p61; p62; p63
              p71; p72; p73 ] |> checkNodeOrder
            [ p12; p13; p21; p23; p31; p32; p43; p51; p62 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

let getParenPatInnerPattern pat =
    match pat with
    | SynPat.Paren(pat, _) -> pat
    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Pattern - Paren - Tuple 01`` () =
    let parseResults = getParseResults """
let (,) = ()
let (,,) = ()
let (,,,) = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map (getLetDeclHeadPattern >> getParenPatInnerPattern)
    match pats with
    | [ SynPat.Tuple(_, [SynPat.Wild _ as p11; SynPat.Wild _ as p12], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p21; SynPat.Wild _ as p22; SynPat.Wild _ as p23], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p31; SynPat.Wild _ as p32; SynPat.Wild _ as p33; SynPat.Wild _ as p34], _) ] ->
            [ p11; p12; p21; p22; p23; p31; p32; p33; p34 ] |> checkNodeOrder
            [ p11; p12; p21; p22; p23; p31; p32; p33; p34 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

[<Test>]
let ``Pattern - Paren - Tuple 02`` () =
    let parseResults = getParseResults """
let (1,) = ()
let (,1) = ()
let (1,1) = ()
"""
    let pats = getSingleModuleMemberDecls parseResults |> List.map (getLetDeclHeadPattern >> getParenPatInnerPattern)
    match pats with
    | [ SynPat.Tuple(_, [SynPat.Const _ as p11; SynPat.Wild _ as p12], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p21; SynPat.Const _ as p22], _)
        SynPat.Tuple(_, [SynPat.Const _ as p31; SynPat.Const _ as p32], _) ] ->
            [ p11; p12; p21; p22; p31; p32 ] |> checkNodeOrder
            [ p12; p21 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"

[<Test>]
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
    | [ SynPat.Tuple(_, [SynPat.Const _ as p11; SynPat.Wild _ as p12; SynPat.Wild _ as p13], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p21; SynPat.Const _ as p22; SynPat.Wild _ as p23], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p31; SynPat.Wild _ as p32; SynPat.Const _ as p33], _)

        SynPat.Tuple(_, [SynPat.Const _ as p41; SynPat.Const _ as p42; SynPat.Wild _ as p43], _)
        SynPat.Tuple(_, [SynPat.Wild _ as p51; SynPat.Const _ as p52; SynPat.Const _ as p53], _)
        SynPat.Tuple(_, [SynPat.Const _ as p61; SynPat.Wild _ as p62; SynPat.Const _ as p63], _)
        
        SynPat.Tuple(_, [SynPat.Const _ as p71; SynPat.Const _ as p72; SynPat.Const _ as p73], _) ] ->
            [ p11; p12; p13; p21; p22; p23; p31; p32; p33
              p41; p42; p43; p51; p52; p53; p61; p62; p63
              p71; p72; p73 ] |> checkNodeOrder
            [ p12; p13; p21; p23; p31; p32; p43; p51; p62 ] |> List.iter assertIsEmptyRange

    | _ -> failwith "Unexpected tree"
