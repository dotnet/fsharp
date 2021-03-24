// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.Diagnostics
open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Utilities
open FSharp.Test.Utilities.Compiler
open FSharp.Tests

[<TestFixture>]
module SignatureGenerationTests =

    let sigText (checkResults: FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults) =
        match checkResults.GenerateSignatureText() with
        | None -> failwith "Unable to generate signature text."
        | Some text -> text

    [<Test>]
    let ``Generate signature with correct namespace``() =
        let text =
            FSharp """
namespace ANamespaceForSignature
            """
            |> withLangVersion50
            |> typecheckResults
            |> sigText

        let expected =
            """namespace rec ANamespaceForSignature"""
        
        Assert.shouldBeEquivalentTo expected (text.ToString())

    [<Test>]
    let ``Generate signature with correct namespace 2``() =
        let text =
            FSharp """
namespace Test.ANamespaceForSignature
            """
            |> withLangVersion50
            |> typecheckResults
            |> sigText

        let expected =
            """namespace rec Test.ANamespaceForSignature"""
        
        Assert.shouldBeEquivalentTo expected (text.ToString())

    [<Test>]
    let ``Generate signature with correct module``() =
        let text =
            FSharp """
module AModuleForSignature
            """
            |> withLangVersion50
            |> typecheckResults
            |> sigText

        let expected =
            """module rec AModuleForSignature"""
        
        Assert.shouldBeEquivalentTo expected (text.ToString())

    [<Test>]
    let ``Generate signature with correct module 2``() =
        let text =
            FSharp """
module Test.AModuleForSignature
            """
            |> withLangVersion50
            |> typecheckResults
            |> sigText

        let expected =
            """module rec Test.AModuleForSignature"""
        
        Assert.shouldBeEquivalentTo expected (text.ToString())
