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
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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