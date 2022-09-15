module FSharp.Compiler.Service.Tests.SyntaxTreeTests.MatchClauseTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``Range of single SynMatchClause`` () =
    let parseResults = 
        getParseResults
            """
try
    let content = tryDownloadFile url
    Some content
with ex ->
    Infrastructure.ReportWarning ex
    None"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = range) as clause ]))
    ]) ])) ->
        assertRange (5, 5) (7, 8) range
        assertRange (5, 5) (7, 8) clause.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of multiple SynMatchClause`` () =
    let parseResults = 
        getParseResults
            """
try
    let content = tryDownloadFile url
    Some content
with
| ex ->
    Infrastructure.ReportWarning ex
    None
| exx ->
    None"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = r1) as clause1
                                                                SynMatchClause(range = r2) as clause2 ]))
    ]) ])) ->
        assertRange (6, 2) (8, 8) r1
        assertRange (6, 2) (8, 8) clause1.Range
        
        assertRange (9, 2) (10, 8) r2
        assertRange (9, 2) (10, 8) clause2.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of single SynMatchClause followed by bar`` () =
    let parseResults = 
        getParseResults
            """
try
    let content = tryDownloadFile url
    Some content
with
| ex ->
    ()
| """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = range) as clause ]))
    ]) ])) ->
        assertRange (6, 2) (7, 6) range
        assertRange (6, 2) (7, 6) clause.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of single SynMatchClause with missing body`` () =
    let parseResults = 
        getParseResults
            """
try
    let content = tryDownloadFile url
    Some content
with
| ex ->"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = range) as clause ]))
    ]) ])) ->
        assertRange (6, 2) (6, 4) range
        assertRange (6, 2) (6, 4) clause.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of single SynMatchClause with missing body and when expr`` () =
    let parseResults = 
        getParseResults
            """
try
    let content = tryDownloadFile url
    Some content
with
| ex when (isNull ex) ->"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = range) as clause ]))
    ]) ])) ->
        assertRange (6, 2) (6, 21) range
        assertRange (6, 2) (6, 21) clause.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of arrow in SynMatchClause`` () =
    let parseResults = 
        getParseResults
            """
match foo with
| Bar bar -> ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.Match(clauses = [ SynMatchClause(trivia={ ArrowRange = Some mArrow }) ]))
    ]) ])) ->
        assertRange (3, 10) (3, 12) mArrow
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of arrow in SynMatchClause with when clause`` () =
    let parseResults = 
        getParseResults
            """
match foo with
| Bar bar when (someCheck bar) -> ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.Match(clauses = [ SynMatchClause(trivia={ ArrowRange = Some mArrow }) ]))
    ]) ])) ->
        assertRange (3, 31) (3, 33) mArrow
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of bar in a single SynMatchClause in SynExpr.Match`` () =
    let parseResults = 
        getParseResults
            """
match foo with
| Bar bar when (someCheck bar) -> ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.Match(clauses = [ SynMatchClause(trivia={ BarRange = Some mBar }) ]))
    ]) ])) ->
        assertRange (3, 0) (3, 1) mBar
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of bar in multiple SynMatchClauses in SynExpr.Match`` () =
    let parseResults = 
        getParseResults
            """
match foo with
| Bar bar when (someCheck bar) -> ()
| Far too -> near ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.Match(clauses = [ SynMatchClause(trivia={ BarRange = Some mBar1 })
                                                            SynMatchClause(trivia={ BarRange = Some mBar2 }) ]))
    ]) ])) ->
        assertRange (3, 0) (3, 1) mBar1
        assertRange (4, 0) (4, 1) mBar2
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of bar in a single SynMatchClause in SynExpr.TryWith`` () =
    let parseResults = 
        getParseResults
            """
try
    foo ()
with
| exn -> ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(trivia={ BarRange = Some mBar }) ]))
    ]) ])) ->
        assertRange (5, 0) (5, 1) mBar
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``No range of bar in a single SynMatchClause in SynExpr.TryWith`` () =
    let parseResults = 
        getParseResults
            """
try
    foo ()
with exn ->
    // some comment
    ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(trivia={ BarRange = None }) ]))
    ]) ])) ->
        Assert.Pass()
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of bar in a multiple SynMatchClauses in SynExpr.TryWith`` () =
    let parseResults = 
        getParseResults
            """
try
    foo ()
with
| IOException as ioex ->
    // some comment
    ()
| ex -> ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(trivia={ BarRange = Some mBar1 })
                                                                SynMatchClause(trivia={ BarRange = Some mBar2 }) ]))
    ]) ])) ->
        assertRange (5, 0) (5, 1) mBar1
        assertRange (8, 0) (8, 1) mBar2
    | _ -> Assert.Fail "Could not get valid AST"
