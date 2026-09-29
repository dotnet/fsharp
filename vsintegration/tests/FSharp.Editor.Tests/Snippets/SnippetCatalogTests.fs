// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.SnippetCatalogTests

open System
open System.IO
open System.Text.RegularExpressions
open System.Xml.Linq

open Xunit

open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.Diagnostics
open FSharp.Test

type private Snippet =
    {
        Title: string
        Shortcut: string
        Types: string list
        Literals: (string * string) list
        Code: string
    }

    member this.IsSurroundsWith = List.contains "SurroundsWith" this.Types

let private xmlns =
    XNamespace.Get "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet"

let private catalog = Path.Combine(AppContext.BaseDirectory, "Snippets", "1033")
let private indexPath = Path.Combine(catalog, "SnippetsIndex.xml")

let private names =
    Directory.GetFiles(Path.Combine(catalog, "FSharp"), "*.snippet")
    |> Array.map Path.GetFileNameWithoutExtension
    |> Array.sort

let private pathOf name =
    Path.Combine(catalog, "FSharp", $"%s{name}.snippet")

let private load name =
    let snippet =
        XDocument.Load(pathOf name).Descendants(xmlns + "CodeSnippet") |> Seq.exactlyOne

    let header = snippet.Element(xmlns + "Header")
    let body = snippet.Element(xmlns + "Snippet")

    {
        Title = header.Element(xmlns + "Title").Value
        Shortcut = header.Element(xmlns + "Shortcut").Value
        Types = [ for element in header.Descendants(xmlns + "SnippetType") -> element.Value ]
        Literals =
            [
                for literal in body.Descendants(xmlns + "Literal") ->
                    literal.Element(xmlns + "ID").Value, literal.Element(xmlns + "Default").Value
            ]
        Code = body.Element(xmlns + "Code").Value
    }

let private mentionsSelected (text: string) =
    text.IndexOf("$selected$", StringComparison.Ordinal) >= 0

/// `$end$` always sits in expression position, so `do ()` stands in for it.
let private expand snippet =
    snippet.Literals
    |> List.fold (fun (code: string) (id, dflt) -> code.Replace($"$%s{id}$", dflt)) snippet.Code
    |> _.Replace("$selected$", "").Replace("$end$", "do ()").Replace("$$", "$")

let private indented (code: string) = "    " + code.Replace("\n", "\n    ")

/// A body is a fragment, so it only parses inside the right kind of host.
let private hosts =
    [
        "whole file", id
        "module level", (fun code -> $"module TestHost\n\n%s{code}\n")
        "type body", (fun code -> $"module TestHost\n\ntype Host() =\n%s{indented code}\n")
        "function body", (fun code -> $"module TestHost\n\nlet f () =\n%s{indented code}\n")
    ]

let private parseErrors source =
    CompilerAssert.Parse(source, fileName = "Test.fs")
    |> _.Diagnostics
    |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

let snippetNames: obj[][] = [| for name in names -> [| name |] |]

[<Fact>]
let ``The catalog ships the snippets the registration promises`` () =
    Assert.Equal(41, names.Length)
    Assert.True(File.Exists indexPath, $"missing {indexPath}")

[<Fact>]
let ``Shortcuts and titles are unique`` () =
    let snippets = names |> Array.map load

    let duplicatesBy key =
        snippets |> Seq.countBy key |> Seq.filter (fun (_, count) -> count > 1)

    Assert.Empty(duplicatesBy _.Shortcut)
    Assert.Empty(duplicatesBy _.Title)

[<Theory; MemberData(nameof snippetNames)>]
let ``Snippet is an Expansion under its own name, authored at column zero with spaces`` (name: string) =
    let snippet = load name
    // `pp_if` follows C#, which cannot name a file `#if`.
    let shortcut = if name = "pp_if" then "#if" else name

    Assert.Contains("Expansion", snippet.Types)
    Assert.Equal(shortcut, snippet.Shortcut)
    Assert.Equal(snippet.Shortcut, snippet.Title)
    Assert.DoesNotContain("\t", snippet.Code)
    Assert.False(snippet.Code.StartsWith(" ", StringComparison.Ordinal), "body must start at column 0")

[<Theory; MemberData(nameof snippetNames)>]
let ``Snippet literals are all declared and all used`` (name: string) =
    let snippet = load name

    let referenced =
        Regex.Matches(snippet.Code, @"\$([A-Za-z][A-Za-z0-9]*)\$")
        |> Seq.cast<Match>
        |> Seq.map _.Groups[1].Value
        |> Seq.filter (fun id -> id <> "end" && id <> "selected")
        |> Set.ofSeq

    Assert.Equal<Set<string>>(snippet.Literals |> List.map fst |> Set.ofList, referenced)

[<Theory; MemberData(nameof snippetNames)>]
let ``Snippet marks the caret and a surround field the expansion client can read back`` (name: string) =
    let snippet = load name
    let layout = tryReadSelectedFieldLayout (pathOf name)

    Assert.Contains("$end$", snippet.Code)
    Assert.Equal(snippet.IsSurroundsWith, mentionsSelected snippet.Code)

    if snippet.IsSurroundsWith then
        let lines = snippet.Code.Replace("\r\n", "\n").Split '\n'
        let fieldLine = lines |> Array.findIndex mentionsSelected
        let field = lines[fieldLine]

        // The engine indents wrapped text by the field's column, so nothing but whitespace may precede it.
        Assert.Equal(ValueSome(fieldLine, field.Length - field.TrimStart().Length), layout)
    else
        Assert.Equal(ValueNone, layout)

[<Theory; MemberData(nameof snippetNames)>]
let ``Snippet expands to F# that parses`` (name: string) =
    let code = expand (load name)
    let errorsByHost = [ for host, wrap in hosts -> host, parseErrors (wrap code) ]

    if errorsByHost |> List.forall (snd >> Array.isEmpty >> not) then
        let firstErrors =
            errorsByHost
            |> List.map (fun (host, errors) -> $"%s{host}: %s{errors[0].Message}")
            |> String.concat "; "

        Assert.Fail($"%s{name} does not parse in any host: %s{firstErrors}\n---\n%s{code}")
