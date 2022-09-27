module FSharp.Compiler.Service.Tests.SyntaxTreeTests.AttributeTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework
open Tests.Service.Symbols

[<Test>]
let ``range of attribute`` () =
    let ast =
        """
[<MyAttribute(foo ="bar")>]
do ()
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile (ParsedImplFileInput(contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls =
        [ SynModuleDecl.Attributes(attributes = [ { Attributes = [ { Range = mAttribute } ] } ]) ; SynModuleDecl.Expr _ ] ) ]))  ->
        assertRange (2, 2) (2, 25) mAttribute
    | _ -> Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``range of attribute with path`` () =
    let ast =
        """
[<Prefix.MyAttribute(foo ="bar")>]
do ()
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile (ParsedImplFileInput(contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls =
        [ SynModuleDecl.Attributes(attributes = [ { Attributes = [ { Range = mAttribute } ] } ]) ; SynModuleDecl.Expr _ ] ) ]))  ->
        assertRange (2, 2) (2, 32) mAttribute
    | _ -> Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``range of attribute with target`` () =
    let ast =
        """
[<assembly: MyAttribute(foo ="bar")>]
do ()
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile (ParsedImplFileInput(contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls =
        [ SynModuleDecl.Attributes(attributes = [ { Attributes = [ { Range = mAttribute } ] } ]) ; SynModuleDecl.Expr _ ] ) ]))  ->
        assertRange (2, 2) (2, 35) mAttribute
    | _ -> Assert.Fail $"Could not get valid AST, got {ast}"
