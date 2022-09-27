module FSharp.Compiler.Service.Tests.SyntaxTreeTests.MeasureTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``Measure contains the range of the constant`` () =
    let parseResults = 
        getParseResults
            """
let n = 1.0m<cm>
let m = 7.000<cm>
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [ SynBinding.SynBinding(expr = SynExpr.Const(SynConst.Measure(constantRange = r1), _)) ])
        SynModuleDecl.Let(bindings = [ SynBinding.SynBinding(expr = SynExpr.Const(SynConst.Measure(constantRange = r2), _)) ])
    ]) ])) ->
        assertRange (2, 8) (2, 12) r1
        assertRange (3, 8) (3, 13) r2
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynMeasure.Paren has correct range`` () =
    let parseResults = 
        getParseResults
            """
40u<hr / (staff weeks)>
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Const(SynConst.Measure(SynConst.UInt32 _, _, SynMeasure.Divide(
                        SynMeasure.Seq([ SynMeasure.Named([ hrIdent ], _) ], _),
                        SynMeasure.Seq([ SynMeasure.Paren(SynMeasure.Seq([
                            SynMeasure.Named([ staffIdent ], _)
                            SynMeasure.Named([ weeksIdent ], _)
                        ], _) , mParen) ], _),
                        _)
                ), _))
    ]) ])) ->
        Assert.AreEqual("hr", hrIdent.idText)
        Assert.AreEqual("staff", staffIdent.idText)
        Assert.AreEqual("weeks", weeksIdent.idText)
        assertRange (2, 9) (2, 22) mParen
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

let private (|TypeName|_|) t =
    match t with
    | SynType.LongIdent(SynLongIdent([ident], _, _)) -> Some ident.idText
    | _ -> None

[<Test>]
let ``SynType.Tuple in measure type with no slashes`` () =
    let parseResults = 
        getParseResults
            """
[<Measure>] type X = Y * Z
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [
            SynTypeDefn(typeRepr =
                SynTypeDefnRepr.Simple(simpleRepr =
                    SynTypeDefnSimpleRepr.TypeAbbrev(rhsType =
                        SynType.Tuple(false, [ SynTupleTypeSegment.Type (TypeName "Y")
                                               SynTupleTypeSegment.Star mStar
                                               SynTupleTypeSegment.Type (TypeName "Z") ], mTuple))))
        ])
    ]) ])) ->
        assertRange (2, 23) (2, 24) mStar
        assertRange (2, 21) (2, 26) mTuple
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``SynType.Tuple in measure type with leading slash`` () =
    let parseResults = 
        getParseResults
            """
[<Measure>] type X = / second
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [
            SynTypeDefn(typeRepr =
                SynTypeDefnRepr.Simple(simpleRepr =
                    SynTypeDefnSimpleRepr.TypeAbbrev(rhsType =
                        SynType.Tuple(false, [ SynTupleTypeSegment.Slash mSlash
                                               SynTupleTypeSegment.Type (TypeName "second") ], mTuple))))
        ])
    ]) ])) ->
        assertRange (2, 21) (2, 22) mSlash
        assertRange (2, 21) (2, 29) mTuple
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
    
[<Test>]
let ``SynType.Tuple in measure type with start and slash`` () =
    let parseResults = 
        getParseResults
            """
[<Measure>] type R = X * Y / Z
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [
            SynTypeDefn(typeRepr =
                SynTypeDefnRepr.Simple(simpleRepr =
                    SynTypeDefnSimpleRepr.TypeAbbrev(rhsType =
                        SynType.Tuple(false, [ SynTupleTypeSegment.Type (TypeName "X")
                                               SynTupleTypeSegment.Star msStar
                                               SynTupleTypeSegment.Type (TypeName "Y")
                                               SynTupleTypeSegment.Slash msSlash
                                               SynTupleTypeSegment.Type (TypeName "Z") ], mTuple))))
        ])
    ]) ])) ->
        assertRange (2, 23) (2, 24) msStar
        assertRange (2, 21) (2, 30) mTuple
        assertRange (2, 27) (2, 28) msSlash
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
