module FSharp.Compiler.Service.Tests.SyntaxTreeTests.PatternTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

[<Test>]
let ``SynPat.Record contains the range of the equals sign`` () =
    let parseResults = 
        getParseResults
            """
match x with
| { Foo = bar } -> ()
| _ -> ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Match(clauses = [ SynMatchClause(pat = SynPat.Record(fieldPats = [ (_, mEquals, _) ])) ; _ ])
        )
    ]) ])) ->
        assertRange (3, 8) (3, 9) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynArgPats.NamePatPairs contains the range of the equals sign`` () =
    let parseResults = 
        getParseResults
            """
match x with
| X(Y  = y) -> y
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Match(clauses = [ SynMatchClause(pat = SynPat.LongIdent(argPats = SynArgPats.NamePatPairs(pats = [ _, mEquals ,_ ])))])
        )
    ]) ])) ->
        assertRange (3, 7) (3, 8) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynPat.Or contains the range of the bar`` () =
    let parseResults = 
        getParseResults
            """
match x with
| A
| B -> ()
| _ -> ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Match(clauses = [ SynMatchClause(pat = SynPat.Or(trivia={ BarRange = mBar })) ; _ ])
        )
    ]) ])) ->
        assertRange (4, 0) (4, 1) mBar
    | _ -> Assert.Fail "Could not get valid AST"
    
[<Test>]
let ``:: operator in SynPat.LongIdent`` () =
    let parseResults = 
        getParseResults
            """
let (head::tail) =  [ 1;2;4]
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(
            bindings = [ SynBinding(headPat = SynPat.Paren(SynPat.LongIdent(longDotId = SynLongIdent([ opColonColonIdent ], _, [ Some (IdentTrivia.OriginalNotation "::") ])), _)) ]
        )
    ]) ])) ->
        Assert.AreEqual("op_ColonColon", opColonColonIdent.idText)
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``:: operator in match pattern`` () =
    let parseResults = 
        getParseResults
            """
match x with
| (head) :: (tail) -> ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Match(clauses = [
                SynMatchClause(pat = SynPat.LongIdent(longDotId = SynLongIdent([ opColonColonIdent ], _, [ Some (IdentTrivia.OriginalNotation "::") ])))
            ])
        )
    ]) ])) ->
        Assert.AreEqual("op_ColonColon", opColonColonIdent.idText)
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``Parentheses of SynArgPats.NamePatPairs`` () =
    let parseResults = 
        getParseResults
            """
match data with
| OnePartData( // foo
    part1 = p1
  (* bar *) ) -> p1
| _ -> failwith "todo"
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Match(clauses = [
                SynMatchClause(pat = SynPat.LongIdent(argPats = SynArgPats.NamePatPairs(trivia = trivia)))
                _
            ])
        )
    ]) ])) ->
        assertRange (3, 13) (5, 13) trivia.ParenRange
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
