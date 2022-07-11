module FSharp.Compiler.Service.Tests.SyntaxTreeTests.SourceIdentifierTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework


[<Test>]
let ``__LINE__`` () =
    let parseResults = 
        getParseResults
            """
__LINE__"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.Const(SynConst.SourceIdentifier("__LINE__", "2", range), _))
    ]) ])) ->
        assertRange (2, 0) (2, 8) range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``__SOURCE_DIRECTORY__`` () =
    let parseResults = 
        getParseResults
            """
__SOURCE_DIRECTORY__"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.Const(SynConst.SourceIdentifier("__SOURCE_DIRECTORY__", _, range), _))
    ]) ])) ->
        assertRange (2, 0) (2, 20) range
    | _ -> Assert.Fail "Could not get valid AST"
    
[<Test>]
let ``__SOURCE_FILE__`` () =
    let parseResults = 
        getParseResults
            """
__SOURCE_FILE__"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.Const(SynConst.SourceIdentifier("__SOURCE_FILE__", _, range), _))
    ]) ])) ->
        assertRange (2, 0) (2, 15) range
    | _ -> Assert.Fail "Could not get valid AST"
