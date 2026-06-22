module FSharp.Compiler.Service.Tests.QuickParseTests

open Xunit
open FSharp.Compiler.EditorServices

// QuickParse.GetCompleteIdentifierIsland is used by language service features
// to extract identifier context from source text at cursor positions.
// When it returns None (as it did for '?' before the fix), downstream services
// like semantic classification, completion, and hover can misinterpret the context.
// This impacts Visual Studio's syntax highlighting - see issue #11008753

[<Fact>]
let ``QuickParse handles optional parameter identifier extraction when cursor is on question mark``() =
    let lineStr = "member _.memb(?optional:string) = optional"
    
    // Test when cursor is exactly on the '?' character
    let posOnQuestionMark = 14
    Assert.Equal('?', lineStr[posOnQuestionMark])
    
    let island = QuickParse.GetCompleteIdentifierIsland false lineStr posOnQuestionMark
    
    // We expect to get "optional" as the identifier
    Assert.True(Option.isSome island, "Should extract identifier island when positioned on '?'")
    
    match island with
    | Some(ident, startCol, isQuoted) ->
        Assert.Equal("optional", ident)
        Assert.False(isQuoted)
        // The identifier should start after the '?'
        Assert.True(startCol >= 15, sprintf "Start column %d should be >= 15" startCol)
    | None ->
        Assert.Fail("Expected to find identifier 'optional' when positioned on '?'")

[<Fact>]
let ``QuickParse handles optional parameter identifier extraction when cursor is on identifier``() =
    let lineStr = "member _.memb(?optional:string) = optional"
    
    // Test when cursor is on the identifier "optional" after the '?'
    let posOnOptional = 17
    Assert.Equal('t', lineStr[posOnOptional])
    
    let island = QuickParse.GetCompleteIdentifierIsland false lineStr posOnOptional
    
    // We expect to get "optional" as the identifier
    Assert.True(Option.isSome island, "Should extract identifier island when positioned on identifier")
    
    match island with
    | Some(ident, startCol, isQuoted) ->
        Assert.Equal("optional", ident)
        Assert.False(isQuoted)
    | None ->
        Assert.Fail("Expected to find identifier 'optional'")

[<Fact>]
let ``QuickParse does not treat question mark as identifier in other contexts``() =
    let lineStr = "let x = y ? z"
    
    // Test when cursor is on the '?' in a different context (not optional parameter)
    let posOnQuestionMark = 10
    Assert.Equal('?', lineStr[posOnQuestionMark])
    
    let island = QuickParse.GetCompleteIdentifierIsland false lineStr posOnQuestionMark
    
    // In this context, '?' is followed by space, not an identifier start
    // So we should get None or the next identifier 'z'
    // Let's check what we actually get
    match island with
    | Some(ident, _, _) ->
        // If we get something, it should be 'z' (the next identifier after the space)
        Assert.Equal("z", ident)
    | None ->
        // Or we might get None, which is also acceptable
        ()

// --- Issue #4966 regression tests -----------------------------------------
// QuickParse.GetPartialLongNameEx had a special case for ')' before a trigger
// dot (so 'f(x).Name.' did not feed 'Name' back as a qualifying ident).
// The equivalent case for ']' was missing, so 'a.[0].Data.' wrongly returned
// QualifyingIdents = ["Data"] and the completion engine then resolved 'Data'
// as a module/type prefix. After the fix, ']' must behave like ')': the
// returned PartialLongName must be empty so the completer falls back to
// expression-typings.

[<Theory>]
[<InlineData("a.[0].Data.")>]        // legacy explicit-dot indexer
[<InlineData("a[0].Data.")>]         // modern indexer syntax
[<InlineData("[1;2].Length.")>]      // list literal indexer/tail
[<InlineData("xs.[0].[1].Value.")>]  // nested explicit-dot indexer
[<InlineData("xs[0][1].Value.")>]    // nested modern indexer
let ``GetPartialLongNameEx returns empty for dot after closing bracket`` (lineStr: string) =
    // cursor is right after the trailing '.', so index is the position of that dot
    let index = lineStr.Length - 1
    Assert.Equal('.', lineStr[index])

    let pln = QuickParse.GetPartialLongNameEx(lineStr, index)

    Assert.Equal<string list>([], pln.QualifyingIdents)
    Assert.Equal("", pln.PartialIdent)

// Regression guard: the pre-existing ')' special case must keep working
// exactly as it does on `main`. These cases pass today; they must keep passing
// after the Sprint-02 fix.
[<Theory>]
[<InlineData("f(x).Name.")>]
[<InlineData("(1).ToString.")>]
[<InlineData("(f x).Length.")>]
let ``GetPartialLongNameEx returns empty for dot after closing paren`` (lineStr: string) =
    let index = lineStr.Length - 1
    Assert.Equal('.', lineStr[index])

    let pln = QuickParse.GetPartialLongNameEx(lineStr, index)

    Assert.Equal<string list>([], pln.QualifyingIdents)
    Assert.Equal("", pln.PartialIdent)

// Negative cases: simple long-ident with no bracket/paren tail must STILL be
// returned as a qualifying ident. The fix must not over-throw-away.
[<Theory>]
[<InlineData("Foo.",        "Foo")>]
[<InlineData("System.IO.",  "IO")>]
[<InlineData("xs.Length.",  "Length")>]
let ``GetPartialLongNameEx preserves plain long identifiers`` (lineStr: string, lastQualifier: string) =
    let index = lineStr.Length - 1
    Assert.Equal('.', lineStr[index])

    let pln = QuickParse.GetPartialLongNameEx(lineStr, index)

    Assert.NotEmpty pln.QualifyingIdents
    Assert.Equal(lastQualifier, List.last pln.QualifyingIdents)
    Assert.Equal("", pln.PartialIdent)
