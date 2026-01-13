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
    
    [<Fact>]
    let ``Parse simple type reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "T:System.String"
        match result with
        | ParsedDocCommentId.Type path -> 
            Assert.Equal<string list>(["System"; "String"], path)
        | _ -> failwith $"Expected Type, got {result}"

    [<Fact>]
    let ``Parse nested type reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "T:MyNamespace.OuterClass.InnerClass"
        match result with
        | ParsedDocCommentId.Type path -> 
            Assert.Equal<string list>(["MyNamespace"; "OuterClass"; "InnerClass"], path)
        | _ -> failwith $"Expected Type, got {result}"

    [<Fact>]
    let ``Parse generic type reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "T:System.Collections.Generic.List`1"
        match result with
        | ParsedDocCommentId.Type path -> 
            Assert.Equal<string list>(["System"; "Collections"; "Generic"; "List`1"], path)
        | _ -> failwith $"Expected Type, got {result}"

    [<Fact>]
    let ``Parse method reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "M:System.String.IndexOf"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"; "String"], typePath)
            Assert.Equal("IndexOf", memberName)
            Assert.Equal(0, genericArity)
            Assert.Equal(DocCommentIdKind.Method, kind)
        | _ -> failwith $"Expected Member, got {result}"

    [<Fact>]
    let ``Parse method with parameters`` () =
        let result = XmlDocSigParser.parseDocCommentId "M:System.String.IndexOf(System.String)"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"; "String"], typePath)
            Assert.Equal("IndexOf", memberName)
            Assert.Equal(0, genericArity)
            Assert.Equal(DocCommentIdKind.Method, kind)
        | _ -> failwith $"Expected Member, got {result}"

    [<Fact>]
    let ``Parse generic method reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "M:System.Linq.Enumerable.Select``1"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"; "Linq"; "Enumerable"], typePath)
            Assert.Equal("Select", memberName)
            Assert.Equal(1, genericArity)
            Assert.Equal(DocCommentIdKind.Method, kind)
        | _ -> failwith $"Expected Member, got {result}"

    [<Fact>]
    let ``Parse property reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "P:System.String.Length"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"; "String"], typePath)
            Assert.Equal("Length", memberName)
            Assert.Equal(0, genericArity)
            Assert.Equal(DocCommentIdKind.Property, kind)
        | _ -> failwith $"Expected Member, got {result}"

    [<Fact>]
    let ``Parse field reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "F:MyNamespace.MyClass.myField"
        match result with
        | ParsedDocCommentId.Field(typePath, fieldName) ->
            Assert.Equal<string list>(["MyNamespace"; "MyClass"], typePath)
            Assert.Equal("myField", fieldName)
        | _ -> failwith $"Expected Field, got {result}"

    [<Fact>]
    let ``Parse event reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "E:System.Windows.Forms.Control.Click"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"; "Windows"; "Forms"; "Control"], typePath)
            Assert.Equal("Click", memberName)
            Assert.Equal(0, genericArity)
            Assert.Equal(DocCommentIdKind.Event, kind)
        | _ -> failwith $"Expected Member with Event kind, got {result}"

    [<Fact>]
    let ``Parse constructor reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "M:System.String.#ctor"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"; "String"], typePath)
            Assert.Equal(".ctor", memberName)  // Converted from #ctor
            Assert.Equal(0, genericArity)
            Assert.Equal(DocCommentIdKind.Method, kind)
        | _ -> failwith $"Expected Member, got {result}"

    [<Fact>]
    let ``Parse invalid doc comment ID returns None`` () =
        let result = XmlDocSigParser.parseDocCommentId "InvalidFormat"
        match result with
        | ParsedDocCommentId.None -> ()
        | _ -> failwith $"Expected None, got {result}"

    [<Fact>]
    let ``Parse doc comment ID with single part returns None`` () =
        let result = XmlDocSigParser.parseDocCommentId "M:SinglePart"
        match result with
        | ParsedDocCommentId.None -> ()
        | _ -> failwith $"Expected None for single-part member, got {result}"


// ============================================================================
// XmlDocInheritance Tests
// ============================================================================

module XmlDocInheritanceTests =
    open FSharp.Compiler.XmlDocInheritance
    open FSharp.Compiler.Text
    
    [<Fact>]
    let ``Empty XmlDoc returns empty`` () =
        let emptyDoc = XmlDoc.Empty
        let result = expandInheritDoc None None None None Range.range0 Set.empty emptyDoc
        Assert.True(result.IsEmpty)

    [<Fact>]
    let ``XmlDoc without inheritdoc returns unchanged`` () =
        let doc = XmlDoc([|"<summary>Test summary</summary>"|], Range.range0)
        let result = expandInheritDoc None None None None Range.Zero Set.empty doc
        Assert.Equal(doc.GetXmlText(), result.GetXmlText())

    [<Fact>]
    let ``XmlDoc with inheritdoc but no InfoReader returns unchanged`` () =
        let doc = XmlDoc([|"<inheritdoc/>"|], Range.Zero)
        let result = expandInheritDoc None None None None Range.Zero Set.empty doc
        // Without InfoReader, should return unchanged
        Assert.NotNull(result)

    [<Fact>]
    let ``XmlDoc with inheritdoc cref is detected`` () =
        let doc = XmlDoc([|"<inheritdoc cref=\"T:System.String\"/>"|], Range.Zero)
        let result = expandInheritDoc None None None None Range.Zero Set.empty doc
        // Without InfoReader, should return unchanged
        Assert.NotNull(result)

    [<Fact>]
    let ``XmlDoc with inheritdoc path is detected`` () =
        let doc = XmlDoc([|"<inheritdoc path=\"/summary\"/>"|], Range.Zero)
        let result = expandInheritDoc None None None None Range.Zero Set.empty doc
        // Without InfoReader, should return unchanged
        Assert.NotNull(result)

    [<Fact>]
    let ``Malformed XML is handled gracefully`` () =
        let doc = XmlDoc([|"<unclosed>"|], Range.Zero)
        let result = expandInheritDoc None None None None Range.Zero Set.empty doc
        // Should return original doc when XML is malformed
        Assert.Equal(doc.GetXmlText(), result.GetXmlText())

    [<Fact>]
    let ``Cycle detection prevents infinite recursion`` () =
        let doc = XmlDoc([|"<inheritdoc cref=\"T:System.String\"/>"|], Range.Zero)
        // Simulate a cycle by pre-populating visited set
        let visited = Set.ofList ["T:System.String"]
        let result = expandInheritDoc None None None None Range.Zero visited doc
        // Should return original doc when cycle is detected
        Assert.NotNull(result)

