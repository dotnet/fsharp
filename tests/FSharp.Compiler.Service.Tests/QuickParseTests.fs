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

// QuickParse.GetCompleteIdentifierIsland tolerateJustAfter line index -> (identifier, endColumn, isQuoted) option.
[<Theory>]
[<InlineData(true, "", -1, null, -1)>]
[<InlineData(false, "", -1, null, -1)>]
[<InlineData(true, "", 0, null, -1)>]
[<InlineData(false, "", 0, null, -1)>]
[<InlineData(true, null, 0, null, -1)>]
[<InlineData(false, null, 0, null, -1)>]
[<InlineData(false, "identifier", 0, "identifier", 10)>]
[<InlineData(false, "identifier", 8, "identifier", 10)>]
[<InlineData(true, "identifier", 0, "identifier", 10)>]
[<InlineData(true, "identifier", 8, "identifier", 10)>]
[<InlineData(false, "identifier", 10, null, -1)>]
[<InlineData(true, "identifier", 10, "identifier", 10)>]
[<InlineData(true, "identifier", 11, null, -1)>]
[<InlineData(false, "identifier", 11, null, -1)>]
[<InlineData(false, "|Identifier|", 0, "|Identifier|", 12)>]
[<InlineData(true, "|Identifier|", 0, "|Identifier|", 12)>]
[<InlineData(false, "|Identifier|", 12, null, -1)>]
[<InlineData(true, "|Identifier|", 12, "|Identifier|", 12)>]
[<InlineData(false, "|Identifier|", 13, null, -1)>]
[<InlineData(true, "|Identifier|", 13, null, -1)>]
[<InlineData(false, "``Space Man``", 0, "``Space Man``", 13)>]
[<InlineData(true, "``Space Man``", 0, "``Space Man``", 13)>]
[<InlineData(false, "``Space Man``", 10, "``Space Man``", 13)>]
[<InlineData(true, "``Space Man``", 10, "``Space Man``", 13)>]
[<InlineData(false, "``Space Man``", 11, "``Space Man``", 13)>]
[<InlineData(false, "``Space Man``", 12, "``Space Man``", 13)>]
[<InlineData(true, "``Space Man``", 12, "``Space Man``", 13)>]
[<InlineData(false, "``Space Man``", 13, null, -1)>]
[<InlineData(true, "``Space Man``", 13, "``Space Man``", 13)>]
[<InlineData(false, "``Space Man``", 14, null, -1)>]
[<InlineData(true, "``Space Man``", 14, null, -1)>]
[<InlineData(true, "``msft-prices.csv``", 14, "``msft-prices.csv``", 19)>]
[<InlineData(true, "[|abc;def|]", 2, "abc", 5)>]
[<InlineData(true, "[|abc;def|]", 4, "abc", 5)>]
[<InlineData(true, "[|abc;def|]", 5, "abc", 5)>]
[<InlineData(true, "[|abc;def|]", 6, "def", 9)>]
[<InlineData(true, "[|abc;def|]", 8, "def", 9)>]
[<InlineData(true, "[|abc;def|]", 9, "def", 9)>]
[<InlineData(false, "identifier(*boo*)", 0, "identifier", 10)>]
[<InlineData(true, "identifier(*boo*)", 0, "identifier", 10)>]
[<InlineData(false, "identifier(*boo*)", 10, null, -1)>]
[<InlineData(true, "identifier(*boo*)", 10, "identifier", 10)>]
[<InlineData(false, "identifier(*boo*)", 11, null, -1)>]
[<InlineData(true, "identifier(*boo*)", 11, null, -1)>]
[<InlineData(false, "``Space Man (*boo*)``", 13, "``Space Man (*boo*)``", 21)>]
[<InlineData(true, "``Space Man (*boo*)``", 13, "``Space Man (*boo*)``", 21)>]
[<InlineData(false, "(*boo*)identifier", 11, "identifier", 17)>]
[<InlineData(true, "(*boo*)identifier", 11, "identifier", 17)>]
[<InlineData(false, "identifier(*(*  *)boo*)", 0, "identifier", 10)>]
[<InlineData(true, "identifier(*(*  *)boo*)", 0, "identifier", 10)>]
let ``QuickParse GetCompleteIdentifierIsland``
    (tolerateJustAfter: bool)
    (line: string)
    (index: int)
    (expectedIdent: string)
    (expectedEndCol: int)
    =
    let actual =
        match QuickParse.GetCompleteIdentifierIsland tolerateJustAfter line index with
        | Some(ident, endCol, _) -> Some(ident, endCol)
        | None -> None

    let expected =
        if isNull expectedIdent then
            None
        else
            Some(expectedIdent, expectedEndCol)

    Assert.Equal<(string * int) option>(expected, actual)

[<Fact(Skip = "This is probably not what the user wanted; not enforcing this test.")>]
let ``QuickParse GetCompleteIdentifierIsland tolerates one char after a quoted identifier (legacy CheckIsland25, not enforced)``
    ()
    =
    let actual =
        match QuickParse.GetCompleteIdentifierIsland true "``Space Man``" 11 with
        | Some(ident, endCol, _) -> Some(ident, endCol)
        | None -> None

    Assert.Equal<(string * int) option>(Some("Man", 11), actual)

// tuple (QualifyingIdents, PartialIdent, LastDotPos). Encoding for the [<InlineData>] primitives:
//   quals:   null -> [] (empty list); "" -> [""] (one empty qualifier); else ';'-split into a list.
//   lastDot: -1 -> None; else Some lastDot.
[<Theory>]
[<InlineData("let y = List.", "List", "", 12)>]
[<InlineData("let y = List.conc", "List", "conc", 12)>]
[<InlineData("let y = S", null, "S", -1)>]
[<InlineData("S", null, "S", -1)>]
[<InlineData("let y=", null, "", -1)>]
[<InlineData("Console.Wr", "Console", "Wr", 7)>]
[<InlineData(" .", "", "", 1)>]
[<InlineData(".", "", "", 0)>]
[<InlineData("System.Console.Wr", "System;Console", "Wr", 14)>]
[<InlineData("let y=f'", null, "f'", -1)>]
[<InlineData("let y=SomeModule.f'", "SomeModule", "f'", 16)>]
[<InlineData("let y=Some.OtherModule.f'", "Some;OtherModule", "f'", 22)>]
[<InlineData("let y=f'g", null, "f'g", -1)>]
[<InlineData("let y=SomeModule.f'g", "SomeModule", "f'g", 16)>]
[<InlineData("let y=Some.OtherModule.f'g", "Some;OtherModule", "f'g", 22)>]
[<InlineData("let y=FSharp.Data.File.``msft-prices.csv``", null, "", -1)>]
[<InlineData("let y=FSharp.Data.File.``msft-prices.csv", "FSharp;Data;File", "msft-prices.csv", 22)>]
[<InlineData("let y=SomeModule.  f", "SomeModule", "f", 16)>]
[<InlineData("let y=SomeModule  .f", "SomeModule", "f", 18)>]
[<InlineData("let y=SomeModule  .  f", "SomeModule", "f", 18)>]
[<InlineData("let y=SomeModule  .", "SomeModule", "", 18)>]
[<InlineData("let y=SomeModule  .  ", "SomeModule", "", 18)>]
let ``QuickParse GetPartialLongNameEx``
    (line: string)
    (quals: string)
    (partialIdent: string)
    (lastDot: int)
    =
    let actual = QuickParse.GetPartialLongNameEx(line, line.Length - 1)

    let expectedQuals =
        if isNull quals then [] else quals.Split(';') |> List.ofArray

    let expectedLastDot = if lastDot < 0 then None else Some lastDot
    let expected = (expectedQuals, partialIdent, expectedLastDot)
    let actualTuple = (actual.QualifyingIdents, actual.PartialIdent, actual.LastDotPos)
    Assert.Equal<string list * string * int option>(expected, actualTuple)
