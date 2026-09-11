// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.IO
open System.Text.RegularExpressions
open System.Xml.Linq

open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text

/// Guards the shipped `.snippet` catalog: the files are content, so nothing else would notice a
/// malformed one until it silently failed to show up in Visual Studio.
module SnippetCatalog =

    let private ns =
        XNamespace.Get "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet"

    let directory = Path.Combine(AppContext.BaseDirectory, "Snippets", "1033", "FSharp")

    let indexPath =
        Path.Combine(AppContext.BaseDirectory, "Snippets", "1033", "SnippetsIndex.xml")

    let files = Directory.GetFiles(directory, "*.snippet") |> Array.sort

    type Snippet =
        {
            Name: string
            Title: string
            Shortcut: string
            Types: string list
            Literals: (string * string) list
            Code: string
        }

        member this.IsSurroundsWith = this.Types |> List.contains "SurroundsWith"

    let load path =
        let document = XDocument.Load(path: string)
        let snippet = document.Descendants(ns + "CodeSnippet") |> Seq.exactlyOne
        let header = snippet.Element(ns + "Header")
        let body = snippet.Element(ns + "Snippet")

        {
            Name = Path.GetFileNameWithoutExtension path
            Title = header.Element(ns + "Title").Value
            Shortcut = header.Element(ns + "Shortcut").Value
            Types = [ for element in header.Descendants(ns + "SnippetType") -> element.Value ]
            Literals =
                [
                    for literal in body.Descendants(ns + "Literal") ->
                        literal.Element(ns + "ID").Value, literal.Element(ns + "Default").Value
                ]
            Code = body.Element(ns + "Code").Value
        }

    /// The snippet as the user first sees it: every literal at its default, the surrounded text
    /// absent, and `()` parked where the caret ends up. `$end$` always occupies a whole expression
    /// position, which is what makes that substitution meaningful.
    let expand snippet =
        let withDefaults =
            snippet.Literals
            |> List.fold (fun (code: string) (id, dflt) -> code.Replace($"$%s{id}$", dflt)) snippet.Code

        withDefaults.Replace("$selected$", "").Replace("$end$", "do ()").Replace("$$", "$")

    let private checker = FSharpChecker.Create()

    let private indent (by: int) (text: string) =
        let pad = String(' ', by)

        text.Split '\n'
        |> Seq.map (fun line ->
            let line = line.TrimEnd '\r'
            if line.Trim() = "" then line else pad + line)
        |> String.concat "\n"

    /// Where a snippet body can legally appear. A body is a fragment, so it only parses inside the
    /// right kind of host.
    let private hosts =
        [
            "whole file", id
            "module level", (fun code -> $"module TestHost\n\n%s{code}\n")
            "type body", (fun code -> $"module TestHost\n\ntype Host() =\n%s{indent 4 code}\n")
            "function body", (fun code -> $"module TestHost\n\nlet f () =\n%s{indent 4 code}\n")
        ]

    let private parseErrors source =
        let options =
            { FSharpParsingOptions.Default with
                SourceFiles = [| "Test.fs" |]
            }

        checker.ParseFile("Test.fs", SourceText.ofString source, options)
        |> Async.RunSynchronously
        |> _.Diagnostics
        |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

    /// The host the expanded body parses in, if any.
    let tryParseInSomeHost code =
        hosts
        |> List.tryPick (fun (name, host) ->
            match parseErrors (host code) with
            | [||] -> Some name
            | _ -> None)

    let firstParseError code =
        hosts
        |> Seq.map (fun (name, host) ->
            let message =
                parseErrors (host code)
                |> Seq.truncate 1
                |> Seq.map _.Message
                |> String.concat ""

            $"%s{name}: %s{message}")
        |> String.concat "; "

type SnippetCatalogTests() =

    static member snippetNames: obj[][] =
        [|
            for path in SnippetCatalog.files -> [| Path.GetFileNameWithoutExtension path |]
        |]

    static member private load name =
        SnippetCatalog.load (Path.Combine(SnippetCatalog.directory, $"%s{name}.snippet"))

    [<Fact>]
    member _.``The catalog ships the snippets the registration promises``() =
        Assert.Equal(40, SnippetCatalog.files.Length)
        Assert.True(File.Exists SnippetCatalog.indexPath, $"missing {SnippetCatalog.indexPath}")

    [<Fact>]
    member _.``Shortcuts and titles are unique``() =
        let snippets = SnippetCatalog.files |> Array.map SnippetCatalog.load

        let duplicatesBy key =
            snippets |> Seq.countBy key |> Seq.filter (fun (_, count) -> count > 1)

        Assert.Empty(duplicatesBy _.Shortcut)
        Assert.Empty(duplicatesBy _.Title)

    [<Theory>]
    [<MemberData(nameof (SnippetCatalogTests.snippetNames))>]
    member _.``Snippet declares an Expansion type and a title matching its shortcut``(name: string) =
        let snippet = SnippetCatalogTests.load name

        Assert.Contains("Expansion", snippet.Types)
        Assert.Equal(snippet.Shortcut, snippet.Title)

        // `pp_if` follows C#, which cannot name a file `#if`.
        if name <> "pp_if" then
            Assert.Equal(name, snippet.Shortcut)

    [<Theory>]
    [<MemberData(nameof (SnippetCatalogTests.snippetNames))>]
    member _.``Snippet literals are all declared and all used``(name: string) =
        let snippet = SnippetCatalogTests.load name

        let referenced =
            Regex.Matches(snippet.Code, @"\$([A-Za-z][A-Za-z0-9]*)\$")
            |> Seq.cast<Match>
            |> Seq.map _.Groups[1].Value
            |> Seq.filter (fun id -> id <> "end" && id <> "selected")
            |> Set.ofSeq

        let declared = snippet.Literals |> List.map fst |> Set.ofList

        Assert.Equal<Set<string>>(declared, referenced)

    [<Theory>]
    [<MemberData(nameof (SnippetCatalogTests.snippetNames))>]
    member _.``Snippet marks the caret position and its surround field``(name: string) =
        let snippet = SnippetCatalogTests.load name

        // An explicit `$end$` is what lets the expansion client skip reading the snippet XML back
        // out of the live session, which is the call that needs Roslyn's IVsExpansionSessionInternal
        // workaround.
        Assert.Contains("$end$", snippet.Code)

        Assert.Equal(snippet.IsSurroundsWith, snippet.Code.IndexOf("$selected$", StringComparison.Ordinal) >= 0)

        if snippet.IsSurroundsWith then
            // The expansion engine indents the substituted text from the column the template put the
            // field at, so anything preceding it on its line would offset the whole wrapped block.
            let selectedLine =
                snippet.Code.Split '\n'
                |> Array.find (fun line -> line.IndexOf("$selected$", StringComparison.Ordinal) >= 0)

            Assert.Equal("$selected$", selectedLine.TrimStart().Substring(0, "$selected$".Length))

    [<Theory>]
    [<MemberData(nameof (SnippetCatalogTests.snippetNames))>]
    member _.``The field layout the expansion client reads back matches the file``(name: string) =
        // Surround With indents the wrapped code by whatever the template indents `$selected$` by, and
        // the live session will not report that, so the client re-reads it from the `.snippet` itself.
        // An unreadable layout silently degrades every wrapped line to the snippet's own column.
        let snippet = SnippetCatalogTests.load name
        let path = Path.Combine(SnippetCatalog.directory, $"%s{name}.snippet")

        let layout =
            Microsoft.VisualStudio.FSharp.Editor.SnippetExpansionHelpers.tryReadSelectedFieldLayout path

        if snippet.IsSurroundsWith then
            let lines = snippet.Code.Replace("\r\n", "\n").Split '\n'

            let fieldLine =
                lines
                |> Array.findIndex (fun line -> line.IndexOf("$selected$", StringComparison.Ordinal) >= 0)

            let fieldIndent = lines[fieldLine].Length - lines[fieldLine].TrimStart().Length

            Assert.Equal(ValueSome(fieldLine, fieldIndent), layout)
        else
            Assert.Equal(ValueNone, layout)

    [<Theory>]
    [<MemberData(nameof (SnippetCatalogTests.snippetNames))>]
    member _.``Snippet body is authored at column zero with spaces``(name: string) =
        let snippet = SnippetCatalogTests.load name

        Assert.DoesNotContain("\t", snippet.Code)

        // Absolute indentation comes from FormatSpan at insertion time, not from the file.
        Assert.False(snippet.Code.StartsWith(" ", StringComparison.Ordinal), "body must start at column 0")

    [<Theory>]
    [<MemberData(nameof (SnippetCatalogTests.snippetNames))>]
    member _.``Snippet expands to F# that parses``(name: string) =
        let snippet = SnippetCatalogTests.load name
        let code = SnippetCatalog.expand snippet

        match SnippetCatalog.tryParseInSomeHost code with
        | Some _ -> ()
        | None -> failwith $"%s{name} does not parse in any host: %s{SnippetCatalog.firstParseError code}\n---\n%s{code}"
