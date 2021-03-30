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

    let sigShouldBe (expected: string) src =
        let text =
            FSharp src
            |> withLangVersion50
            |> typecheckResults
            |> sigText

        let actual = text.ToString()
        let expected2 = expected.Replace("\r\n", "\n")
        Assert.shouldBeEquivalentTo expected2 actual

    [<Test>]
    let ``Generate signature with correct namespace``() =
        """
namespace ANamespaceForSignature
        """
        |> sigShouldBe """namespace rec ANamespaceForSignature"""

    [<Test>]
    let ``Generate signature with correct namespace 2``() =
        """
namespace Test.ANamespaceForSignature
        """
        |> sigShouldBe """namespace rec Test.ANamespaceForSignature"""

    [<Test>]
    let ``Generate signature with correct namespace 3``() =
        """
namespace Test.ANamespaceForSignature

namespace Test2.ANamespaceForSignature2
        """
        |> sigShouldBe """namespace rec Test.ANamespaceForSignature

namespace rec Test2.ANamespaceForSignature2"""


    [<Test>]
    let ``Generate signature with correct namespace and type``() =
        """
namespace Test.ANamespaceForSignature

type TestType = class end
        """
        |> sigShouldBe """namespace rec Test.ANamespaceForSignature

type TestType"""

    [<Test>]
    let ``Generate signature with correct namespace and private inner record type``() =
        """
namespace Test.ANamespaceForSignature

type TestType = private { x: int }
        """
        |> sigShouldBe """namespace rec Test.ANamespaceForSignature

type TestType =
  private { x: int }"""

    [<Test>]
    let ``Generate signature with correct namespace and private inner record type and module with private inner record type``() =
        """
namespace Test.ANamespaceForSignature

type TestType = private { x: int }

module ModuleA =

    type TestType2 = private { x: float32 }
        """
        |> sigShouldBe """namespace rec Test.ANamespaceForSignature

type TestType =
  private { x: int }

module rec ModuleA =

  type TestType2 =
    private { x: float32 }"""

    [<Test>]
    let ``Generate signature with correct module``() =
        """
module AModuleForSignature
        """
        |> sigShouldBe """module rec AModuleForSignature"""

    [<Test>]
    let ``Generate signature with correct module 2``() =
        """
module Test.AModuleForSignature
        """
        |> sigShouldBe """namespace rec Test

module rec AModuleForSignature = begin end"""

    [<Test>]
    let ``Generate signature with correct module includes attributes``() =
        // TODO: This should trim the "Attribute ()".
        """
[<RequireQualifiedAccess>]
module AModuleForSignature
        """
        |> sigShouldBe """[<RequireQualifiedAccessAttribute ()>]
module rec AModuleForSignature"""
