module FSharp.Compiler.Service.Tests.SyntaxTreeTests.SynTypeTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``SynType.Tuple does include leading parameter name`` () =
    let parseResults =
        getParseResultsOfSignatureFile
             """
type T =
    member M: p1: a * p2: b -> int
 """

    match parseResults with
    | ParsedInput.SigFile(ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                    SynMemberSig.Member(memberSig = SynValSig(synType =
                        SynType.Fun(argType = SynType.Tuple(_, _, mTuple))))
                ]))
            ])
        ])
      ])) ->
        assertRange (3, 14) (3, 27) mTuple
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``SynType.Tuple does include leading parameter attributes`` () =
    let parseResults =
        getParseResultsOfSignatureFile
             """
type T =
    member M: [<SomeAttribute>] a * [<OtherAttribute>] b -> int
 """

    match parseResults with
    | ParsedInput.SigFile(ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                    SynMemberSig.Member(memberSig = SynValSig(synType =
                        SynType.Fun(argType = SynType.Tuple(_, _, mTuple))))
                ]))
            ])
        ])
      ])) ->
        assertRange (3, 14) (3, 56) mTuple
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``SynType.Or inside SynExpr.TraitCall`` () =
    let parseResults =
        getParseResults
             """
let inline (!!) (x: ^a) : ^b = ((^a or ^b): (static member op_Implicit: ^a -> ^b) x)
 """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [
            SynBinding(expr =
                SynExpr.Typed(expr =
                    SynExpr.Paren(expr =
                        SynExpr.TraitCall(supportTys =
                            SynType.Paren(
                                SynType.Or(
                                    SynType.Var(range = mVar1),
                                    SynType.Var(range = mVar2),
                                    mOrType,
                                    { OrKeyword = mOrWord }),
                            mParen))
                    )))
        ]) ]) ])) ->
        assertRange (2, 33) (2, 35) mVar1
        assertRange (2, 36) (2, 38) mOrWord
        assertRange (2, 39) (2, 41) mVar2
        assertRange (2, 33) (2, 41) mOrType
        assertRange (2, 32) (2, 42) mParen
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``SynType.Or inside SynTypeConstraint.WhereTyparSupportsMember`` () =
    let parseResults =
        getParseResults
             """
let inline f_StaticMethod<'T1, 'T2 when ('T1 or 'T2) : (static member StaticMethod: int -> int) >() : int = 
    ()
 """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [
            SynBinding(headPat =
                SynPat.LongIdent(typarDecls = Some (SynValTyparDecls(typars = Some (SynTyparDecls.PostfixList(constraints = [
                    SynTypeConstraint.WhereTyparSupportsMember(typars = SynType.Paren(
                        SynType.Or(
                            SynType.Var(range = mVar1),
                            SynType.Var(range = mVar2),
                            mOrType,
                            { OrKeyword = mOrWord }),
                        mParen
                    ))
                ]))))))
        ]) ]) ])) ->
        assertRange (2, 41) (2, 44) mVar1
        assertRange (2, 45) (2, 47) mOrWord
        assertRange (2, 48) (2, 51) mVar2
        assertRange (2, 41) (2, 51) mOrType
        assertRange (2, 40) (2, 52) mParen
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``Nested SynType.Or inside SynExpr.TraitCall`` () =
    let parseResults =
        getParseResults
             """
let inline (!!) (x: ^a * ^b) : ^c = ((^a or ^b or ^c): (static member op_Implicit: ^a * ^b -> ^c) x)
 """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [
            SynBinding(expr =
                SynExpr.Typed(expr =
                    SynExpr.Paren(expr =
                        SynExpr.TraitCall(supportTys =
                            SynType.Paren(
                                SynType.Or(
                                    SynType.Or(
                                        SynType.Var(range = mVarA),
                                        SynType.Var(range = mVarB),
                                        mOrType1,
                                        { OrKeyword = mOrWord1 }
                                    ),
                                    SynType.Var(range = mVarC),
                                    mOrType2,
                                    { OrKeyword = mOrWord2 }),
                            mParen))
                    )))
        ]) ]) ])) ->
        assertRange (2, 38) (2, 40) mVarA
        assertRange (2, 41) (2, 43) mOrWord1
        assertRange (2, 44) (2, 46) mVarB
        assertRange (2, 38) (2, 46) mOrType1
        assertRange (2, 47) (2, 49) mOrWord2
        assertRange (2, 50) (2, 52) mVarC
        assertRange (2, 38) (2, 52) mOrType2
        assertRange (2, 37) (2, 53) mParen
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
    
[<Test>]
let ``Single SynType inside SynExpr.TraitCall`` () =
    let parseResults =
        getParseResults
             """
type X =
    static member inline replace< ^a, ^b, ^c when ^b: (static member replace: ^a * ^b -> ^c)>(a: ^a, f: ^b) =
        (^b : (static member replace: ^a * ^b -> ^c) (a, f))
 """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [
            SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                SynMemberDefn.Member(memberDefn =
                    SynBinding(expr =
                            SynExpr.Paren(expr =
                                SynExpr.TraitCall(supportTys =
                                    SynType.Var(range = mVar))
                    )))
            ]))
        ]) ]) ])) ->
        assertRange (4, 9) (4, 11) mVar
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``SynType.Or with appType on the right hand side`` () =
    let parseResults =
        getParseResults
             """
let inline f (x: 'T) = ((^T or int) : (static member A: int) ())
 """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [
            SynBinding(expr =
                    SynExpr.Paren(expr =
                        SynExpr.TraitCall(supportTys =
                            SynType.Paren(
                                SynType.Or(
                                    SynType.Var(range = mVar),
                                    (SynType.LongIdent _ as appType),
                                    mOrType,
                                    { OrKeyword = mOrWord }),
                            mParen))
                    ))
        ]) ]) ])) ->
        assertRange (2, 25) (2,27) mVar
        assertRange (2, 28) (2, 30) mOrWord
        assertRange (2, 31) (2, 34) appType.Range
        assertRange (2,25) (2, 34) mOrType
        assertRange (2,24) (2, 35) mParen
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
