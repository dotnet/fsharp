// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Miscellaneous.XmlDoc

open System.IO
open Xunit
open FSharp.Compiler.Xml
open FSharp.Compiler.Symbols
open TestFramework


let memberDoc = "<summary>Summary</summary>"

let xmlFileContents signature = $"""<?xml version="1.0" encoding="utf-8"?>
<doc>
  <assembly>
    <name>FSharp.Core</name>
  </assembly>
  <members>
    <member name="T:Microsoft.FSharp.Collections.list`1">
      <summary>The type of immutable singly-linked lists. </summary>
  </member>
  <member name="{signature}">
    {memberDoc}
  </member>
</members>
</doc>
"""

[<Theory>]
[<InlineData("P:Microsoft.FSharp.Collections.FSharpList`1.Length")>]
[<InlineData("P:Microsoft.FSharp.Collections.FSharpList`1.Length'")>]
let ``Can extract XML docs from a file for a signature`` signature =
    let xmlFileName = getTemporaryFileName () + ".xml"

    try
        File.WriteAllText(xmlFileName, xmlFileContents signature)

        let docInfo =
            XmlDocumentationInfo.TryCreateFromFile(xmlFileName)
            |> Option.defaultWith (fun () -> failwith "Couldn't create XmlDoc from file")

        match docInfo.TryGetXmlDocBySig(signature) with
        | None -> failwith "Got no doc"
        | Some doc -> Assert.Equal(memberDoc, doc.UnprocessedLines |> String.concat "\n")

    finally
        File.Delete xmlFileName


// ============================================================================
// XmlDocSigParser Tests
// ============================================================================

module XmlDocSigParserTests =

    // Type reference parsing - parameterized
    [<Theory>]
    [<InlineData("T:System.String", "System;String")>]
    [<InlineData("T:MyNamespace.OuterClass.InnerClass", "MyNamespace;OuterClass;InnerClass")>]
    [<InlineData("T:System.Collections.Generic.List`1", "System;Collections;Generic;List`1")>]
    let ``Parse type reference`` (input: string, expectedPathStr: string) =
        let expectedPath = expectedPathStr.Split(';') |> Array.toList

        match XmlDocSigParser.parseDocCommentId input with
        | ParsedDocCommentId.Type path -> Assert.Equal<string list>(expectedPath, path)
        | other -> failwith $"Expected Type, got {other}"

    // Member reference parsing - parameterized via MemberData
    let private assertMember input expectedTypePath expectedName expectedArity expectedKind =
        match XmlDocSigParser.parseDocCommentId input with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(expectedTypePath, typePath)
            Assert.Equal(expectedName, memberName)
            Assert.Equal(expectedArity, genericArity)
            Assert.Equal(expectedKind, kind)
        | other -> failwith $"Expected Member, got {other}"

    let memberReferenceData: obj array array =
        [| [| "M:System.String.IndexOf"; [ "System"; "String" ]; "IndexOf"; 0; DocCommentIdKind.Method |]
           [| "M:System.String.IndexOf(System.String)"; [ "System"; "String" ]; "IndexOf"; 0; DocCommentIdKind.Method |]
           [| "M:System.Linq.Enumerable.Select``1"; [ "System"; "Linq"; "Enumerable" ]; "Select"; 1; DocCommentIdKind.Method |]
           [| "P:System.String.Length"; [ "System"; "String" ]; "Length"; 0; DocCommentIdKind.Property |]
           [| "E:System.Windows.Forms.Control.Click"; [ "System"; "Windows"; "Forms"; "Control" ]; "Click"; 0; DocCommentIdKind.Event |]
           [| "M:System.String.#ctor"; [ "System"; "String" ]; ".ctor"; 0; DocCommentIdKind.Method |] |]

    [<Theory>]
    [<MemberData(nameof memberReferenceData)>]
    let ``Parse member reference`` (input: string, expectedTypePath: string list, expectedName: string, expectedArity: int, expectedKind: DocCommentIdKind) =
        assertMember input expectedTypePath expectedName expectedArity expectedKind

    [<Fact>]
    let ``Parse field reference`` () =
        match XmlDocSigParser.parseDocCommentId "F:MyNamespace.MyClass.myField" with
        | ParsedDocCommentId.Field(typePath, fieldName) ->
            Assert.Equal<string list>([ "MyNamespace"; "MyClass" ], typePath)
            Assert.Equal("myField", fieldName)
        | other -> failwith $"Expected Field, got {other}"

    // Invalid input parsing - parameterized
    [<Theory>]
    [<InlineData("InvalidFormat")>]
    [<InlineData("M:SinglePart")>]
    let ``Parse invalid doc comment ID returns None`` (input: string) =
        match XmlDocSigParser.parseDocCommentId input with
        | ParsedDocCommentId.None -> ()
        | other -> failwith $"Expected None, got {other}"


// ============================================================================
// XmlDocInheritance Tests
// ============================================================================

module XmlDocInheritanceTests =
    open FSharp.Compiler.XmlDocInheritance
    open FSharp.Compiler.Text

    let private noResolver (_cref: string) : string option = None

    let private expandWithNoResolver visited xmlText =
        expandInheritDocFromXmlText noResolver None Range.range0 visited xmlText

    [<Fact>]
    let ``Empty XmlDoc returns empty`` () =
        let result = expandWithNoResolver Set.empty ""
        Assert.Equal("", result)

    [<Fact>]
    let ``XmlDoc without inheritdoc returns unchanged`` () =
        let xmlText = "<summary>Test summary</summary>"
        let result = expandWithNoResolver Set.empty xmlText
        Assert.Equal(xmlText, result)

    // These all pass different inheritdoc variants without resolver - result should be non-null
    [<Theory>]
    [<InlineData("<inheritdoc/>")>]
    [<InlineData("<inheritdoc cref=\"T:System.String\"/>")>]
    [<InlineData("<inheritdoc path=\"/summary\"/>")>]
    let ``XmlDoc with inheritdoc but no resolver returns non-null`` (xmlLine: string) =
        let result = expandWithNoResolver Set.empty xmlLine
        Assert.NotNull(result)

    [<Fact>]
    let ``Malformed XML is handled gracefully`` () =
        let xmlText = "<unclosed>"
        let result = expandWithNoResolver Set.empty xmlText
        Assert.Equal(xmlText, result)

    [<Fact>]
    let ``Cycle detection prevents infinite recursion`` () =
        let xmlText = "<inheritdoc cref=\"T:System.String\"/>"
        let visited = Set.ofList [ "T:System.String" ]
        let result = expandWithNoResolver visited xmlText
        Assert.NotNull(result)

    [<Fact>]
    let ``Resolver-based expansion replaces inheritdoc with resolved content`` () =
        let resolver (cref: string) =
            if cref = "T:Test.BaseType" then
                Some "<summary>Base type summary</summary>"
            else
                None

        let xmlText = "<inheritdoc cref=\"T:Test.BaseType\"/>"
        let result = expandInheritDocFromXmlText resolver None Range.range0 Set.empty xmlText
        Assert.Contains("Base type summary", result)
        Assert.DoesNotContain("<inheritdoc", result)

    [<Fact>]
    let ``Recursive chained resolution expands through multiple levels`` () =
        let resolver (cref: string) =
            match cref with
            | "T:GrandBase" -> Some "<summary>GrandBase documentation</summary>"
            | "T:Base" -> Some "<inheritdoc cref=\"T:GrandBase\"/>"
            | _ -> None

        let xmlText = "<inheritdoc cref=\"T:Base\"/>"
        let result = expandInheritDocFromXmlText resolver None Range.range0 Set.empty xmlText
        Assert.Contains("GrandBase documentation", result)

    [<Fact>]
    let ``Implicit target resolves when no cref is specified`` () =
        let resolver (cref: string) =
            if cref = "T:Test.IService" then
                Some "<summary>Service contract docs</summary>"
            else
                None

        let xmlText = "<inheritdoc/>"

        let result =
            expandInheritDocFromXmlText resolver (Some "T:Test.IService") Range.range0 Set.empty xmlText

        Assert.Contains("Service contract docs", result)

    [<Fact>]
    let ``XPath path filter selects only matching elements`` () =
        let resolver (cref: string) =
            if cref = "T:Test.Base" then
                Some "<summary>Base summary</summary><remarks>Base remarks</remarks>"
            else
                None

        let xmlText = "<inheritdoc cref=\"T:Test.Base\" path=\"/remarks\"/>"
        let result = expandInheritDocFromXmlText resolver None Range.range0 Set.empty xmlText
        Assert.Contains("Base remarks", result)
        Assert.DoesNotContain("Base summary", result)

