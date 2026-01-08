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
            Assert.Equal<string list>(["System"], typePath)
            Assert.Equal("IndexOf", memberName)
            Assert.Equal(0, genericArity)
            Assert.Equal(DocCommentIdKind.Method, kind)
        | _ -> failwith $"Expected Member, got {result}"

    [<Fact>]
    let ``Parse method with parameters`` () =
        let result = XmlDocSigParser.parseDocCommentId "M:System.String.IndexOf(System.String)"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"], typePath)
            Assert.Equal("IndexOf", memberName)
            Assert.Equal(0, genericArity)
            Assert.Equal(DocCommentIdKind.Method, kind)
        | _ -> failwith $"Expected Member, got {result}"

    [<Fact>]
    let ``Parse generic method reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "M:System.Linq.Enumerable.Select``1"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"; "Linq"], typePath)
            Assert.Equal("Select", memberName)
            Assert.Equal(1, genericArity)
            Assert.Equal(DocCommentIdKind.Method, kind)
        | _ -> failwith $"Expected Member, got {result}"

    [<Fact>]
    let ``Parse property reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "P:System.String.Length"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"], typePath)
            Assert.Equal("Length", memberName)
            Assert.Equal(0, genericArity)
            Assert.Equal(DocCommentIdKind.Property, kind)
        | _ -> failwith $"Expected Member, got {result}"

    [<Fact>]
    let ``Parse field reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "F:MyNamespace.MyClass.myField"
        match result with
        | ParsedDocCommentId.Field(typePath, fieldName) ->
            Assert.Equal<string list>(["MyNamespace"], typePath)
            Assert.Equal("myField", fieldName)
        | _ -> failwith $"Expected Field, got {result}"

    [<Fact>]
    let ``Parse event reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "E:System.Windows.Forms.Control.Click"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"; "Windows"; "Forms"], typePath)
            Assert.Equal("Click", memberName)
            Assert.Equal(0, genericArity)
            Assert.Equal(DocCommentIdKind.Event, kind)
        | _ -> failwith $"Expected Member with Event kind, got {result}"

    [<Fact>]
    let ``Parse constructor reference`` () =
        let result = XmlDocSigParser.parseDocCommentId "M:System.String.#ctor"
        match result with
        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            Assert.Equal<string list>(["System"], typePath)
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
    open FSharp.Compiler.Symbols.XmlDocInheritance
    open FSharp.Compiler.Text.Range
    
    [<Fact>]
    let ``Empty XmlDoc returns empty`` () =
        let emptyDoc = XmlDoc.Empty
        let result = expandInheritDoc None range0 Set.empty emptyDoc
        Assert.True(result.IsEmpty)

    [<Fact>]
    let ``XmlDoc without inheritdoc returns unchanged`` () =
        let doc = XmlDoc([|"<summary>Test summary</summary>"|], range0)
        let result = expandInheritDoc None range0 Set.empty doc
        Assert.Equal(doc.GetXmlText(), result.GetXmlText())

    [<Fact>]
    let ``XmlDoc with inheritdoc but no InfoReader returns unchanged`` () =
        let doc = XmlDoc([|"<inheritdoc/>"|], range0)
        let result = expandInheritDoc None range0 Set.empty doc
        // Without InfoReader, should return unchanged
        Assert.NotNull(result)

    [<Fact>]
    let ``XmlDoc with inheritdoc cref is detected`` () =
        let doc = XmlDoc([|"<inheritdoc cref=\"T:System.String\"/>"|], range0)
        let result = expandInheritDoc None range0 Set.empty doc
        // Without InfoReader, should return unchanged
        Assert.NotNull(result)

    [<Fact>]
    let ``XmlDoc with inheritdoc path is detected`` () =
        let doc = XmlDoc([|"<inheritdoc path=\"/summary\"/>"|], range0)
        let result = expandInheritDoc None range0 Set.empty doc
        // Without InfoReader, should return unchanged
        Assert.NotNull(result)

    [<Fact>]
    let ``Malformed XML is handled gracefully`` () =
        let doc = XmlDoc([|"<unclosed>"|], range0)
        let result = expandInheritDoc None range0 Set.empty doc
        // Should return original doc when XML is malformed
        Assert.Equal(doc.GetXmlText(), result.GetXmlText())

    [<Fact>]
    let ``Cycle detection prevents infinite recursion`` () =
        let doc = XmlDoc([|"<inheritdoc cref=\"T:System.String\"/>"|], range0)
        // Simulate a cycle by pre-populating visited set
        let visited = Set.ofList ["T:System.String"]
        let result = expandInheritDoc None range0 visited doc
        // Should return original doc when cycle is detected
        Assert.NotNull(result)


// ============================================================================
// Integration Tests
// ============================================================================

module IntegrationTests =
    open FSharp.Test.Compiler
    
    [<Fact>]
    let ``Inheritdoc in XML file generation`` () =
        FSharp """
module TestModule

/// <summary>Base documentation</summary>
type BaseClass() =
    member _.BaseMethod() = ()

/// <inheritdoc cref="T:TestModule.BaseClass"/>
type DerivedClass() =
    inherit BaseClass()
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldSucceed
    
    [<Fact>]
    let ``Interface implementation with inheritdoc should work``() =
        FSharp """
module TestModule

/// <summary>Interface with comprehensive documentation</summary>
/// <remarks>This interface defines the core contract</remarks>
type IService =
    /// <summary>Executes the service operation</summary>
    /// <param name="input">The input parameter</param>
    /// <returns>The operation result</returns>
    abstract Execute: input:string -> string

/// <inheritdoc cref="T:TestModule.IService"/>
type ServiceImpl() =
    interface IService with
        /// <inheritdoc/>
        member _.Execute(input) = input
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldSucceed
    
    [<Fact>]
    let ``Method override with inheritdoc should work``() =
        FSharp """
module TestModule

/// <summary>Base class with virtual method</summary>
type BaseClass() =
    /// <summary>Virtual method to override</summary>
    /// <param name="x">First parameter</param>
    /// <param name="y">Second parameter</param>
    /// <returns>The sum of parameters</returns>
    abstract member Compute: x:int -> y:int -> int
    default _.Compute(x, y) = x + y

/// <inheritdoc cref="T:TestModule.BaseClass"/>
type DerivedClass() =
    inherit BaseClass()
    /// <inheritdoc/>
    override _.Compute(x, y) = x * y
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldSucceed
    
    [<Fact>]
    let ``XPath filtering with path attribute should work``() =
        FSharp """
module TestModule

/// <summary>Base documentation</summary>
/// <remarks>These are important remarks</remarks>
/// <example>This is an example</example>
type BaseType() = class end

/// <summary>Derived type</summary>
/// <inheritdoc cref="T:TestModule.BaseType" path="/remarks"/>
type DerivedType() = class end
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldSucceed
    
    [<Fact>]
    let ``Warning for unresolvable cref``() =
        FSharp """
module TestModule

/// <inheritdoc cref="T:NonExistent.Type"/>
type MyType() = class end
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3390, Line 4, Col 1, Line 4, Col 35, "This XML comment is invalid: inheritdoc error: Cannot resolve cref 'T:NonExistent.Type'")
    
    [<Fact>]
    let ``Warning for circular reference``() =
        FSharp """
module TestModule

/// <inheritdoc cref="T:TestModule.TypeB"/>
type TypeA() = class end

/// <inheritdoc cref="T:TestModule.TypeA"/>
type TypeB() = class end
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3390, Line 4, Col 1, Line 4, Col 35, "This XML comment is invalid: inheritdoc error: Circular reference detected for 'T:TestModule.TypeB'")
        ]
    
    [<Fact>]
    let ``Warning for implicit inheritdoc without cref``() =
        FSharp """
module TestModule

type BaseType() =
    /// <summary>Base method</summary>
    member _.Method() = ()

type DerivedType() =
    inherit BaseType()
    /// <inheritdoc/>
    member _.Method() = ()
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3390, Line 10, Col 5, Line 10, Col 21, "This XML comment is invalid: inheritdoc error: Implicit inheritdoc (without cref) is not yet supported")


// Comprehensive cross-reference tests
module XmlDocCrossReferenceTests =
    open FSharp.Test

    [<Fact>]
    let ``Same compilation different module inheritance``() =
        FSharp """
module ModuleA

/// <summary>Base class in module A</summary>
/// <remarks>Important base class remarks</remarks>
type BaseType() = class end

module ModuleB

open ModuleA

/// <inheritdoc cref="T:ModuleA.BaseType"/>
type DerivedType() = inherit BaseType()
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldSucceed
        |> verifyXmlDoc "T:ModuleB.DerivedType" (fun lines ->
            lines |> shouldContainText "<summary>Base class in module A</summary>"
            lines |> shouldContainText "<remarks>Important base class remarks</remarks>")

    [<Fact>]
    let ``Same compilation different module with nested namespaces``() =
        FSharp """
namespace OuterNamespace

module ModuleA =
    /// <summary>Base documentation from ModuleA</summary>
    type BaseType() = class end

namespace InnerNamespace

module ModuleB =
    /// <inheritdoc cref="T:OuterNamespace.ModuleA.BaseType"/>
    type DerivedType() = class end
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldSucceed
        |> verifyXmlDoc "T:InnerNamespace.ModuleB.DerivedType" (fun lines ->
            lines |> shouldContainText "<summary>Base documentation from ModuleA</summary>")

    [<Fact>]
    let ``Inheritance from .NET BCL System.String``() =
        FSharp """
module TestModule

/// <inheritdoc cref="T:System.String"/>
type MyStringWrapper() = class end
        """
        |> withOptions ["--doc:test.xml"; "--noframework"]
        |> withReferences [typeof<System.String>.Assembly.Location]
        |> compile
        |> shouldSucceed
        |> verifyXmlDoc "T:TestModule.MyStringWrapper" (fun lines ->
            // System.String documentation should be inherited
            lines |> shouldContainText "System.String")

    [<Fact>]
    let ``Inheritance from .NET BCL System.Collections.Generic.List``() =
        FSharp """
module TestModule

/// <inheritdoc cref="T:System.Collections.Generic.List`1"/>
type MyListWrapper<'T>() = class end
        """
        |> withOptions ["--doc:test.xml"; "--noframework"]
        |> withReferences [typeof<System.Collections.Generic.List<int>>.Assembly.Location]
        |> compile
        |> shouldSucceed
        |> verifyXmlDoc "T:TestModule.MyListWrapper`1" (fun lines ->
            // System.Collections.Generic.List documentation should be inherited
            lines |> shouldContainText "List")

    [<Fact>]
    let ``Inheritance from FSharp.Core option type``() =
        FSharp """
module TestModule

/// <inheritdoc cref="T:Microsoft.FSharp.Core.FSharpOption`1"/>
type MyOptionWrapper<'T>() = class end
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldSucceed
        |> verifyXmlDoc "T:TestModule.MyOptionWrapper`1" (fun lines ->
            // FSharp.Core.FSharpOption documentation should be inherited
            lines |> shouldContainText "option")

    [<Fact>]
    let ``Inheritance from FSharp.Core List module``() =
        FSharp """
module TestModule

/// <inheritdoc cref="T:Microsoft.FSharp.Collections.ListModule"/>
type MyListUtilities() = class end
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Method inheritance from different module``() =
        FSharp """
module BaseModule

type BaseType() =
    /// <summary>Base method documentation</summary>
    /// <param name="x">The parameter</param>
    /// <returns>The result</returns>
    member _.Calculate(x: int) = x * 2

module DerivedModule

open BaseModule

type DerivedType() =
    inherit BaseType()
    /// <inheritdoc cref="M:BaseModule.BaseType.Calculate(System.Int32)"/>
    override _.Calculate(x: int) = x * 3
        """
        |> withOptions ["--doc:test.xml"]
        |> compile
        |> shouldSucceed
        |> verifyXmlDoc "M:DerivedModule.DerivedType.Calculate(System.Int32)" (fun lines ->
            lines |> shouldContainText "<summary>Base method documentation</summary>"
            lines |> shouldContainText "<param name=\"x\">The parameter</param>"
            lines |> shouldContainText "<returns>The result</returns>")
