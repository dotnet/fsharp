// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test.Compiler

module AttributeResolutionInRecursiveScopes =

    // https://github.com/dotnet/fsharp/issues/7931
    [<Fact>]
    let ``Extension attribute on module in namespace rec`` () =
        FSharp """
namespace rec Ns

open System.Runtime.CompilerServices

[<Extension>]
module Module =
    [<Extension>]
    let ext1 (x: int) = x.ToString()
        """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/7931
    [<Fact>]
    let ``Extension attribute on type in namespace rec`` () =
        FSharp """
namespace rec Ns

open System.Runtime.CompilerServices

[<Extension>]
type T() =
    class end
        """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/5795 - Custom attribute used on type and let in rec module
    [<Fact>]
    let ``Custom attribute used on type and let in rec module`` () =
        FSharp """
module rec M

type CustomAttribute() =
    inherit System.Attribute()

[<Custom>] type A = | A
[<Custom>] let a = ()
        """
        |> typecheck
        |> shouldSucceed

    // Nested module case: open inside outer module, attribute on inner module
    [<Fact>]
    let ``Open inside nested module resolves for attribute on inner module in namespace rec`` () =
        FSharp """
namespace rec Ns

module Outer =
    open System.Runtime.CompilerServices

    [<Extension>]
    module Inner =
        [<Extension>]
        let ext1 (x: int) = x.ToString()
        """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // Non-recursive baseline: should always work
    [<Fact>]
    let ``Extension attribute works without rec - baseline`` () =
        FSharp """
namespace Ns

open System.Runtime.CompilerServices

[<Extension>]
module Module =
    [<Extension>]
    let ext1 (x: int) = x.ToString()
        """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // Multiple opens in namespace rec
    [<Fact>]
    let ``Multiple opens resolve for attributes in namespace rec`` () =
        FSharp """
namespace rec Ns

open System
open System.Runtime.CompilerServices

[<Extension>]
module Module =
    [<Extension>]
    [<Obsolete("test")>]
    let ext1 (x: int) = x.ToString()
        """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // Open in module rec resolves for module attributes
    [<Fact>]
    let ``Open in module rec resolves for nested module attribute`` () =
        FSharp """
module rec M

open System.Runtime.CompilerServices

[<Extension>]
module Inner =
    [<Extension>]
    let ext1 (x: int) = x.ToString()
        """
        |> typecheck
        |> shouldSucceed

    // Open with Obsolete attribute in namespace rec
    [<Fact>]
    let ``Obsolete attribute resolves via open in namespace rec`` () =
        FSharp """
namespace rec Ns

open System

[<Obsolete("deprecated")>]
module DeprecatedModule =
    let x = 42
        """
        |> asLibrary
        |> typecheck
        |> shouldSucceed
