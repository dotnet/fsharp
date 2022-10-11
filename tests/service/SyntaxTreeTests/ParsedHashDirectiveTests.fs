module FSharp.Compiler.Service.Tests.SyntaxTreeTests.ParsedHashDirectiveTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``SourceIdentifier as ParsedHashDirectiveArgument`` () =
    let parseResults = 
        getParseResults
            "#I __SOURCE_DIRECTORY__"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.HashDirective(ParsedHashDirective("I", [ ParsedHashDirectiveArgument.SourceIdentifier(c,_,m) ] , _), _)
    ]) ])) ->
        Assert.AreEqual("__SOURCE_DIRECTORY__", c)
        assertRange (1, 3) (1, 23) m
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Regular String as ParsedHashDirectiveArgument`` () =
    let parseResults = 
        getParseResults
            "#I \"/tmp\""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.HashDirective(ParsedHashDirective("I", [ ParsedHashDirectiveArgument.String(v, SynStringKind.Regular, m) ] , _), _)
    ]) ])) ->
        Assert.AreEqual("/tmp", v)
        assertRange (1, 3) (1, 9) m
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Verbatim String as ParsedHashDirectiveArgument`` () =
    let parseResults = 
        getParseResults
            "#I @\"C:\\Temp\""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.HashDirective(ParsedHashDirective("I", [ ParsedHashDirectiveArgument.String(v, SynStringKind.Verbatim, m) ] , _), _)
    ]) ])) ->
        Assert.AreEqual("C:\\Temp", v)
        assertRange (1, 3) (1, 13) m
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Triple quote String as ParsedHashDirectiveArgument`` () =
    let parseResults = 
        getParseResults
            "#nowarn \"\"\"40\"\"\""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.HashDirective(ParsedHashDirective("nowarn", [ ParsedHashDirectiveArgument.String(v, SynStringKind.TripleQuote, m) ] , _), _)
    ]) ])) ->
        Assert.AreEqual("40", v)
        assertRange (1, 8) (1, 16) m
    | _ -> Assert.Fail "Could not get valid AST"
