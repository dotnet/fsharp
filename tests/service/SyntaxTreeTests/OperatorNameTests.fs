module FSharp.Compiler.Service.Tests.SyntaxTreeTests.OperatorNameTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

[<Test>]
let ``operator as function`` () =
    let ast = """
(+) 3 4
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr (expr = SynExpr.App(funcExpr = SynExpr.App(funcExpr =
                SynExpr.LongIdent(longDotId = SynLongIdent([ident], _, [Some (IdentTrivia.OriginalNotationWithParen(lpr, "+", rpr))])))))
            ])
        ])) ->
        assertRange (2, 0) (2, 1) lpr
        Assert.AreEqual("op_Addition", ident.idText)
        assertRange (2, 2) (2, 3) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``active pattern as function `` () =
    let ast = """
(|Odd|Even|) 4
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr (expr = SynExpr.App(funcExpr =
                SynExpr.LongIdent(false, SynLongIdent([ ident ], _, [ Some(IdentTrivia.HasParenthesis(lpr, rpr)) ]), None, pr)))
            ])
        ])) ->
        assertRange (2, 0) (2, 1) lpr
        Assert.AreEqual("|Odd|Even|", ident.idText)
        assertRange (2, 11) (2, 12) rpr
        assertRange (2, 0) (2, 12) pr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``partial active pattern as function `` () =
    let ast = """
(|Odd|_|) 4
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr (expr = SynExpr.App(funcExpr =
                SynExpr.LongIdent(false, SynLongIdent([ ident ], _, [ Some(IdentTrivia.HasParenthesis(lpr, rpr)) ]), None, pr)))
            ])
        ])) ->
        assertRange (2, 0) (2, 1) lpr
        Assert.AreEqual("|Odd|_|", ident.idText)
        assertRange (2, 8) (2, 9) rpr
        assertRange (2, 0) (2, 9) pr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``custom operator definition`` () =
    let ast = """
let (+) a b = a + b
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [SynBinding(headPat=
                SynPat.LongIdent(longDotId = SynLongIdent([ ident ],_, [ Some (IdentTrivia.OriginalNotationWithParen(lpr, "+", rpr)) ])))
            ])
        ])])) ->
        assertRange (2, 4) (2,5) lpr
        Assert.AreEqual("op_Addition", ident.idText)
        assertRange (2, 6) (2, 7) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``active pattern definition`` () =
    let ast = """
let (|Odd|Even|) (a: int) = if a % 2 = 0 then Even else Odd
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [SynBinding(headPat=
                SynPat.LongIdent(longDotId = SynLongIdent([ident], _, [Some (IdentTrivia.HasParenthesis(lpr, rpr))])))
            ])
        ])])) ->
        assertRange (2, 4) (2, 5) lpr
        Assert.AreEqual("|Odd|Even|", ident.idText)
        assertRange (2, 15) (2, 16) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``partial active pattern definition`` () =
    let ast = """
let (|Int32Const|_|) (a: SynConst) = match a with SynConst.Int32 _ -> Some a | _ -> None
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [SynBinding(headPat=
                SynPat.LongIdent(longDotId = SynLongIdent([ident], _, [Some (IdentTrivia.HasParenthesis(lpr, rpr))])))
            ])
        ])])) ->
         assertRange (2, 4) (2, 5) lpr
         Assert.AreEqual("|Int32Const|_|", ident.idText)
         assertRange (2, 19) (2, 20) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``partial active pattern definition without parameters`` () =
    let ast = """
let (|Boolean|_|) = Boolean.parse
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [SynBinding(headPat=
                SynPat.Named(ident = SynIdent(ident, Some (IdentTrivia.HasParenthesis(lpr, rpr)))))
            ])
        ])])) ->
         assertRange (2, 4) (2, 5) lpr
         Assert.AreEqual("|Boolean|_|", ident.idText)
         assertRange (2, 16) (2, 17) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"


[<Test>]
let ``operator name in SynValSig`` () =
    let ast = """
module IntrinsicOperators
val (&): e1: bool -> e2: bool -> bool
"""
                    |> getParseResultsOfSignatureFile

    match ast with
    | ParsedInput.SigFile(ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Val(valSig = SynValSig(ident = SynIdent(ident, Some (IdentTrivia.OriginalNotationWithParen(lpr, "&", rpr)))
            ))])
        ])) ->
        assertRange (3, 4) (3, 5) lpr
        Assert.AreEqual("op_Amp", ident.idText)
        assertRange (3, 6) (3, 7) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``operator name in val constraint`` () =
    let ast =
        getParseResultsOfSignatureFile """
    [<AutoOpen>]
    module Operators
    /// <summary>Overloaded unary negation.</summary>
    ///
    /// <param name="n">The value to negate.</param>
    ///
    /// <returns>The result of the operation.</returns>
    /// 
    /// <example-tbd></example-tbd>
    /// 
    val inline (~-): n: ^T -> ^T when ^T: (static member ( ~- ): ^T -> ^T) and default ^T: int
"""

    match ast with
    | ParsedInput.SigFile(ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Val(valSig = SynValSig(synType=SynType.WithGlobalConstraints(constraints=[
                SynTypeConstraint.WhereTyparSupportsMember(memberSig=SynMemberSig.Member(memberSig=SynValSig(ident =
                    SynIdent(ident, Some (IdentTrivia.OriginalNotationWithParen(lpr, "~-", rpr))))))
                SynTypeConstraint.WhereTyparDefaultsToType _
            ])))
            ])
        ])) ->
        assertRange (12, 57) (12, 58) lpr
        Assert.AreEqual("op_UnaryNegation", ident.idText)
        assertRange (12, 62) (12, 63) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``named parameter`` () =
    let ast = getParseResults """
f(x=4)
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.App(argExpr = SynExpr.Paren(expr = SynExpr.App(funcExpr=
                SynExpr.App(funcExpr= SynExpr.LongIdent(longDotId = SynLongIdent([ident], _, [Some (IdentTrivia.OriginalNotation "=")])))))))
            ])
        ])) ->
        Assert.AreEqual("op_Equality", ident.idText)
        assertRange (2,3) (2,4) ident.idRange
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``infix operation`` () =
    let ast = getParseResults """
1 + 1
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr =
                SynExpr.App(funcExpr = SynExpr.App(isInfix = true
                                                   funcExpr = SynExpr.LongIdent(longDotId = SynLongIdent([ident], _, [Some (IdentTrivia.OriginalNotation "+")]))
                                                   argExpr = SynExpr.Const(SynConst.Int32(1), _))
                            argExpr = SynExpr.Const(SynConst.Int32(1), _)))
            ])
        ])) ->
        Assert.AreEqual("op_Addition", ident.idText)
        assertRange (2,2) (2,3) ident.idRange
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``prefix operation`` () =
    let ast = getParseResults """
+ -86
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr =
                SynExpr.App(isInfix = false
                            funcExpr = SynExpr.LongIdent(longDotId = SynLongIdent([ident], _, [Some (IdentTrivia.OriginalNotation "+")]))
                            argExpr = SynExpr.Const(SynConst.Int32(-86), _)))
            ])
        ])) ->
        Assert.AreEqual("op_UnaryPlus", ident.idText)
        assertRange (2,0) (2,1) ident.idRange
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``prefix operation with two characters`` () =
    let ast = getParseResults """
%%arg
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr =
                SynExpr.App(isInfix = false
                            funcExpr = SynExpr.LongIdent(longDotId = SynLongIdent([ident], _, [Some (IdentTrivia.OriginalNotation "%%")]))
                            argExpr = SynExpr.Ident argIdent))
            ])
        ])) ->
        Assert.AreEqual("op_SpliceUntyped", ident.idText)
        assertRange (2,0) (2,2) ident.idRange
        Assert.AreEqual("arg", argIdent.idText)
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"    

[<Test>]
let ``detect difference between compiled operators`` () =
    let ast = getParseResults """
(+) a b
op_Addition a b
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr =
                SynExpr.App(funcExpr = SynExpr.App(isInfix = false
                                                   funcExpr = SynExpr.LongIdent(longDotId = SynLongIdent([ident], _, [Some (IdentTrivia.OriginalNotationWithParen(lpr, "+", rpr))]))
                                                   argExpr = SynExpr.Ident a1)
                            argExpr = SynExpr.Ident b1))
            SynModuleDecl.Expr(expr =
                SynExpr.App(funcExpr = SynExpr.App(isInfix = false
                                                   funcExpr = SynExpr.Ident op_Addition
                                                   argExpr = SynExpr.Ident a2)
                            argExpr = SynExpr.Ident b2)
                )
            ])
        ])) ->
        assertRange (2,0) (2,1) lpr
        Assert.AreEqual("op_Addition", ident.idText)
        assertRange (2,2) (2,3) rpr
        Assert.AreEqual("a", a1.idText)
        Assert.AreEqual("b", b1.idText)

        Assert.AreEqual("op_Addition", op_Addition.idText)
        Assert.AreEqual("a", a2.idText)
        Assert.AreEqual("b", b2.idText)
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``operator in member definition`` () =
    let ast = getParseResults """
type X with
    member _.(+) a b = a + b
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [
                SynTypeDefn(members = [
                    SynMemberDefn.Member(memberDefn = SynBinding(headPat = SynPat.LongIdent(longDotId =
                        SynLongIdent([ _; operatorIdent ], [ mDot ], [ None; Some (IdentTrivia.OriginalNotationWithParen(lpr, "+", rpr)) ]) as lid)))
                ])
                ]
            )
            ])
        ])) ->
        assertRange (3,12) (3,13) mDot
        assertRange (3,13) (3,14) lpr
        Assert.AreEqual("op_Addition", operatorIdent.idText)
        assertRange (3,15) (3,16) rpr
        assertRange (3,11) (3,15) lid.Range
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``nameof operator`` () =
    let ast = getParseResults """
nameof(+)
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr =
                                SynExpr.App(isInfix = false
                                            funcExpr = SynExpr.Ident nameofIdent
                                            argExpr =
                                                SynExpr.LongIdent(longDotId = SynLongIdent([operatorIdent], _, [Some (IdentTrivia.OriginalNotationWithParen(lpr, "+", rpr))]))
                                )
                )
            ])
        ])) ->
        Assert.AreEqual("nameof", nameofIdent.idText)
        assertRange (2,6) (2,7) lpr
        Assert.AreEqual("op_Addition", operatorIdent.idText)
        assertRange (2,8) (2,9) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``optional expression`` () =
    let ast = getParseResults """
f(?x = 7)
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr =
                                SynExpr.App(isInfix = false
                                            funcExpr = SynExpr.Ident f
                                            argExpr = SynExpr.Paren(
                                                SynExpr.App(funcExpr = SynExpr.App(
                                                                isInfix = true
                                                                funcExpr = SynExpr.LongIdent(longDotId = SynLongIdent([eqIdent], _, [Some (IdentTrivia.OriginalNotation "=")]))
                                                                argExpr = SynExpr.LongIdent(true, SynLongIdent([x], [], [None]), _, mOptional)
                                                            )
                                                            argExpr = SynExpr.Const(SynConst.Int32 7, _)), lpr, Some rpr, pr)))
            ])
        ])) ->
        Assert.AreEqual("f", f.idText)
        assertRange (2,1) (2,2) lpr
        Assert.AreEqual("x", x.idText)
        assertRange (2,3) (2, 4) mOptional
        Assert.AreEqual("op_Equality", eqIdent.idText)
        assertRange (2,8) (2,9) rpr
        assertRange (2,1) (2,9) pr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``object model with two members`` () =
    let ast = getParseResults """
type X() =
    let mutable allowInto = 0
    member _.AllowIntoPattern with get() = allowInto and set v = allowInto <- v
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [
                SynTypeDefn.SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members =[
                    SynMemberDefn.ImplicitCtor _
                    SynMemberDefn.LetBindings _
                    SynMemberDefn.GetSetMember(
                        Some (SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ _ ; allowIntoPatternGet ])))),
                        Some (SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ _ ; allowIntoPatternSet ])))),
                        _,
                        { WithKeyword = mWith; AndKeyword = Some mAnd })
                ]))
            ])
        ])
        ])) ->
        Assert.AreEqual("AllowIntoPattern", allowIntoPatternGet.idText)
        assertRange (4, 30) (4, 34) mWith
        Assert.AreEqual("AllowIntoPattern", allowIntoPatternSet.idText)
        assertRange (4, 53) (4, 56) mAnd
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``qualified operator expression`` () =
    let ast = getParseResults """
let PowByte (x:byte) n = Checked.( * ) x
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [
                SynBinding(expr = SynExpr.App(funcExpr =
                    SynExpr.LongIdent(longDotId = SynLongIdent([checkedIdent; operatorIdent], [mDot], [None; Some (IdentTrivia.OriginalNotationWithParen(lpr, "*", rpr))]))))
            ])
        ])
        ])) ->
        Assert.AreEqual("Checked", checkedIdent.idText)
        assertRange (2, 32) (2, 33) mDot
        assertRange (2, 33) (2, 34) lpr
        Assert.AreEqual("op_Multiply", operatorIdent.idText)
        assertRange (2, 37) (2, 38) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``active pattern identifier in private member`` () =
    let ast = getParseResults """
type A() =
    member private _.(|
        A'
    |) = (|
        Lazy
    |)
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [
                SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                    SynMemberDefn.ImplicitCtor _
                    SynMemberDefn.Member(memberDefn = SynBinding(
                        headPat = SynPat.LongIdent(longDotId = SynLongIdent([underscoreIdent; aQuoteIdent], [ _ ], [ None; Some (IdentTrivia.HasParenthesis(lpr, rpr)) ]))
                    ))
                ]))
            ])
        ])
        ])) ->
        ()
        Assert.AreEqual("_", underscoreIdent.idText)
        Assert.AreEqual("|A'|", aQuoteIdent.idText)
        assertRange (3, 21) (3, 22) lpr
        assertRange (5, 5) (5, 6) rpr
    | _ ->
        Assert.Fail $"Could not get valid AST, got {ast}"