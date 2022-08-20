module FSharp.Compiler.Service.Tests.SyntaxTreeTests.ExpressionTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

open Xunit
[<Fact>]
let ``Thomas`` () =
    let ast = """_.ToString.ToString "b" """ |> getParseResults
    Assert.Fail (ast.ToString())
    
[<Test>]
let ``SynExpr.Do contains the range of the do keyword`` () =
    let ast = """let a =
    do
        foobar
    do!
        foobarBang
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Let(bindings = [
                        SynBinding(expr = SynExpr.Sequential(expr1 = SynExpr.Do(_, doRange) ; expr2 = SynExpr.DoBang(_, doBangRange)))
                    ])
                ])
            ])) ->
        assertRange (2, 4) (3, 14) doRange
        assertRange (4, 4) (5, 18) doBangRange
    | _ ->
        Assert.Fail "Could not find SynExpr.Do"

[<Test>]
let ``SynExpr.LetOrUseBang contains the range of the equals sign`` () =
    let ast =
        """
comp {
    let! x = y
    and! z = someFunction ()
    return ()
}
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr = SynExpr.App(argExpr =
                        SynExpr.ComputationExpr(expr =
                            SynExpr.LetOrUseBang(trivia = { EqualsRange = Some mLetBangEquals }
                                                 andBangs = [ SynExprAndBang(trivia= { EqualsRange = mAndBangEquals }) ]))))
                ])
            ])) ->
        assertRange (3, 11) (3, 12) mLetBangEquals
        assertRange (4, 11) (4, 12) mAndBangEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.Record contains the range of the equals sign in SynExprRecordField`` () =
    let ast =
        """
{ V = v
  X      =   // some comment
                someLongFunctionCall
                    a
                    b
                    c }
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Record(recordFields = [
                            SynExprRecordField(equalsRange = Some mEqualsV)
                            SynExprRecordField(equalsRange = Some mEqualsX)
                        ]))
                ])
            ])) ->
        assertRange (2, 4) (2, 5) mEqualsV
        assertRange (3, 9) (3, 10) mEqualsX
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``inherit SynExpr.Record contains the range of the equals sign in SynExprRecordField`` () =
    let ast =
        """
{ inherit Exception(msg); X = 1; }
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Record(baseInfo = Some _ ; recordFields = [
                            SynExprRecordField(equalsRange = Some mEquals)
                        ]))
                ])
            ])) ->
        assertRange (2, 28) (2, 29) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``copy SynExpr.Record contains the range of the equals sign in SynExprRecordField`` () =
    let ast =
        """
{ foo with
        X
            =
                12 }
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Record(copyInfo = Some _ ; recordFields = [
                            SynExprRecordField(equalsRange = Some mEquals)
                        ]))
                ])
            ])) ->
        assertRange (4, 12) (4, 13) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.AnonRecord contains the range of the equals sign in the fields`` () =
    let ast =
        """
{| X = 5
   Y    = 6
   Z        = 7 |}
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.AnonRecd(recordFields = [
                            (_, Some mEqualsX, _)
                            (_, Some mEqualsY, _)
                            (_, Some mEqualsZ, _)
                        ]))
                ])
            ])) ->
        assertRange (2, 5) (2, 6) mEqualsX
        assertRange (3, 8) (3, 9) mEqualsY
        assertRange (4, 12) (4, 13) mEqualsZ
    | _ -> Assert.Fail "Could not get valid AST"
    
[<Test>]
let ``SynExpr.For contains the range of the equals sign`` () =
    let ast =
        """
for i = 1 to 10 do
printf "%d " i
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.For(equalsRange = Some mEquals))
                ])
            ])) ->
        assertRange (2, 6) (2, 7) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.TryWith contains the range of the try and with keyword`` () =
    let ast =
        """
try
x
with
| ex -> y
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.TryWith(trivia={ TryKeyword = mTry; WithKeyword = mWith }))
                ])
            ])) ->
        assertRange (2, 0) (2, 3) mTry
        assertRange (4, 0) (4, 4) mWith
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.TryFinally contains the range of the try and with keyword`` () =
    let ast =
        """
try
x
finally
()
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.TryFinally(trivia={ TryKeyword = mTry; FinallyKeyword = mFinally }))
                ])
            ])) ->
        assertRange (2, 0) (2, 3) mTry
        assertRange (4, 0) (4, 7) mFinally
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.Match contains the range of the match and with keyword`` () =
    let ast =
        """
match x with
| y -> z
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Match(trivia = { MatchKeyword = mMatch; WithKeyword = mWith }))
                ])
            ])) ->
        assertRange (2, 0) (2, 5) mMatch
        assertRange (2, 8) (2, 12) mWith
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.MatchBang contains the range of the match and with keyword`` () =
    let ast =
        """
match! x with
| y -> z
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.MatchBang(trivia = { MatchBangKeyword = mMatch; WithKeyword = mWith }))
                ])
            ])) ->
        assertRange (2, 0) (2, 6) mMatch
        assertRange (2, 9) (2, 13) mWith
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.ObjExpr contains the range of with keyword`` () =
    let ast =
        """
{ new obj() with
    member x.ToString() = "INotifyEnumerableInternal"
  interface INotifyEnumerableInternal<'T>
  interface IEnumerable<_> with
    member x.GetEnumerator() = null }
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.ObjExpr(withKeyword=Some mWithObjExpr; extraImpls=[ SynInterfaceImpl(withKeyword=None); SynInterfaceImpl(withKeyword=Some mWithSynInterfaceImpl) ]))
                ])
            ])) ->
        assertRange (2, 12) (2, 16) mWithObjExpr
        assertRange (5, 27) (5, 31) mWithSynInterfaceImpl
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.LetOrUse contains the range of in keyword`` () =
    let ast =
        getParseResults "let x = 1 in ()"

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.LetOrUse(trivia={ InKeyword = Some mIn }))
                ])
            ])) ->
        assertRange (1, 10) (1, 12) mIn
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.LetOrUse with recursive binding contains the range of in keyword`` () =
    let ast =
        getParseResults """
do
    let rec f = ()
    and g = () in
    ()
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Do(expr = SynExpr.LetOrUse(bindings=[_;_]; trivia={ InKeyword = Some mIn })))
                ])
            ])) ->
        assertRange (4, 15) (4, 17) mIn
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``nested SynExpr.LetOrUse contains the range of in keyword`` () =
    let ast =
        getParseResults """
let f () =
    let x = 1 in // the "in" keyword is available in F#
    let y = 2 in
    x + y
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Let(bindings = [
                      SynBinding(expr =
                          SynExpr.LetOrUse(bindings=[_]; trivia={ InKeyword = Some mIn }; body=SynExpr.LetOrUse(trivia={ InKeyword = Some mInnerIn })))
                    ])
                ])
            ])) ->
        assertRange (3, 14) (3, 16) mIn
        assertRange (4, 14) (4, 16) mInnerIn
    | _ -> Assert.Fail "Could not get valid AST"    

[<Test>]
let ``SynExpr.LetOrUse does not contain the range of in keyword`` () =
    let ast =
        getParseResults """
do
let x = 1     
()
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Do(expr = SynExpr.LetOrUse(trivia={ InKeyword = None })))
                ])
            ])) ->
        Assert.Pass()
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.LetOrUse where body expr starts with token of two characters does not contain the range of in keyword`` () =
    let ast =
        getParseResults """
do
let e1 = e :?> Collections.DictionaryEntry
e1.Key, e1.Value
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Do(expr = SynExpr.LetOrUse(trivia={ InKeyword = None })))
                ])
            ])) ->
        Assert.Pass()
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``global keyword as SynExpr`` () =
    let ast =
        getParseResults """
global
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.LongIdent(longDotId = SynLongIdent([mangledGlobal], [], [Some (IdentTrivia.OriginalNotation "global")]))
                )])
            ])) ->
        Assert.AreEqual("`global`", mangledGlobal.idText)
        Assert.Pass()
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExprRecordFields contain correct amount of trivia`` () =
    let ast =
        getParseResults """
    { JobType = EsriBoundaryImport
      FileToImport = filePath
      State = state
      DryRun = args.DryRun }
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Record(recordFields = [
                            SynExprRecordField(fieldName = (synLongIdent, _))
                            _; _; _
                        ]))
                ])
            ])) ->
        match synLongIdent.IdentsWithTrivia with
        | [ _ ] -> Assert.Pass()
        | idents -> Assert.Fail $"Expected a single SynIdent, got {idents}"
    | _ -> Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``SynExpr.Dynamic does contain ident`` () =
    let ast =
        getParseResults "x?k"

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr = SynExpr.Dynamic (_, _, SynExpr.Ident(idK) ,mDynamicExpr))
                ])
            ])) ->
        Assert.AreEqual("k", idK.idText)
        assertRange (1,0) (1, 3) mDynamicExpr
    | _ -> Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``SynExpr.Dynamic does contain parentheses`` () =
    let ast =
        getParseResults "x?(g)"

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Dynamic (_, _, SynExpr.Paren(SynExpr.Ident(idG), lpr, Some rpr, mParen) ,mDynamicExpr))
                ])
            ])) ->
        Assert.AreEqual("g", idG.idText)
        assertRange (1, 2) (1,3) lpr
        assertRange (1, 4) (1,5) rpr
        assertRange (1, 2) (1,5) mParen
        assertRange (1,0) (1, 5) mDynamicExpr
    | _ -> Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``SynExpr.Set with SynExpr.Dynamic`` () =
    let ast =
        getParseResults "x?v <- 2"

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr = SynExpr.Set(
                        SynExpr.Dynamic (_, _, SynExpr.Ident(idV) ,mDynamicExpr),
                        SynExpr.Const _,
                        mSetExpr
                    ))
                ])
            ])) ->
        Assert.AreEqual("v", idV.idText)
        assertRange (1,0) (1, 3) mDynamicExpr
        assertRange (1,0) (1, 8) mSetExpr
    | _ -> Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``SynExpr.Obj with setter`` () =
    let ast =
        getParseResults """
[<AbstractClass>]
type CFoo() =
    abstract AbstractClassPropertySet: string with set

{ new CFoo() with
    override this.AbstractClassPropertySet with set (v:string) = () }
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types _
                    SynModuleDecl.Expr(expr = SynExpr.ObjExpr(members = [
                        SynMemberDefn.GetSetMember(None, Some _, m, { WithKeyword = mWith; SetKeyword = Some mSet })
                    ]))
                ])
            ])) ->
        assertRange (7, 43) (7, 47) mWith
        assertRange (7, 48) (7, 51) mSet
        assertRange (7,4) (7, 67) m
    | _ -> Assert.Fail $"Could not get valid AST, got {ast}"

