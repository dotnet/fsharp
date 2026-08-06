// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Miscellaneous.XmlDoc

open System.IO
open Xunit
open FSharp.Compiler.Xml
open FSharp.Compiler.Symbols
open FSharp.Test.Compiler
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
    let private assertMember input expectedTypePath expectedName expectedArity (expectedKind: string) =
        match XmlDocSigParser.parseDocCommentId input with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(expectedTypePath, typePath)
            Assert.Equal(expectedName, memberName)
            Assert.Equal(expectedArity, genericArity)
            Assert.Equal(expectedKind, string kind)
        | other -> failwith $"Expected Member, got {other}"

    let memberReferenceData: obj array array =
        [| [| "M:System.String.IndexOf"; [ "System"; "String" ]; "IndexOf"; 0; "Method" |]
           [| "M:System.String.IndexOf(System.String)"; [ "System"; "String" ]; "IndexOf"; 0; "Method" |]
           [| "M:System.Linq.Enumerable.Select``1"; [ "System"; "Linq"; "Enumerable" ]; "Select"; 1; "Method" |]
           [| "P:System.String.Length"; [ "System"; "String" ]; "Length"; 0; "Property" |]
           [| "E:System.Windows.Forms.Control.Click"; [ "System"; "Windows"; "Forms"; "Control" ]; "Click"; 0; "Event" |]
           [| "M:System.String.#ctor"; [ "System"; "String" ]; ".ctor"; 0; "Method" |] |]

    [<Theory>]
    [<MemberData(nameof memberReferenceData)>]
    let ``Parse member reference`` (input: string, expectedTypePath: string list, expectedName: string, expectedArity: int, expectedKind: string) =
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
// Compile-time emission: <inheritdoc> is written verbatim (IDE expands it, not the compiler)
// ============================================================================

module VerbatimEmissionTests =

    [<Fact>]
    let ``inheritdoc is emitted verbatim into the generated xml doc file`` () =
        let outDir = createTemporaryDirectory ()
        let xmlPath = Path.Combine(outDir.FullName, "test.xml")

        FSharp """
module Test

/// <summary>Base summary</summary>
type Base() = class end

/// <inheritdoc cref="T:Test.Base"/>
type Derived() =
    inherit Base()
"""
        |> withOutputDirectory (Some outDir)
        |> withOptions [ $"--doc:{xmlPath}" ]
        |> compile
        |> shouldSucceed
        |> ignore

        let generated = File.ReadAllText xmlPath
        // The compiler must NOT expand <inheritdoc> at compile time (that is <include>'s job);
        // the cref tag is written verbatim and resolved later by the IDE/FCS tooling layer.
        // (Base's own <summary> is present as Base's own member entry; that is unrelated to expansion.)
        Assert.Contains("<inheritdoc cref=\"T:Test.Base\"", generated)
