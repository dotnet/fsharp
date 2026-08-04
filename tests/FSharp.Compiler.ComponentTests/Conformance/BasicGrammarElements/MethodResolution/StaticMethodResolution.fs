// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open FSharp.Test.Compiler
open Xunit

module StaticMethodResolution =
    
    // Regression test for https://github.com/dotnet/fsharp/issues/19664
    //
    // When a static extension method is defined in a *different* [<AutoOpen>] module than
    // the generic type it extends, and shares its name with an intrinsic static member,
    // resolving the call via the explicit-type-argument syntax `Type<TArg>.Member(...)`
    // previously failed with FS0505. The non-generic dotted form `Type.Member(...)`
    // resolved correctly, so any regression test that omits the explicit type arguments
    // does not actually exercise the bug. See the discussion at
    // https://github.com/dotnet/fsharp/issues/19675#issuecomment-4373059900.
    [<Fact>]
    let ``Static extension on generic type resolves with explicit type arguments``() =
        Fsx """
module Extensions =

    type StaticGeneric<'T> =
        static member Bar() = ()
        static member Bar(_: int, _: int) = ()

    [<AutoOpen>]
    module StaticGenericExtensions =
        type StaticGeneric<'T> with
            static member Bar(_: int) = ()

module Program =
    open Extensions

    StaticGeneric<int>.Bar()        // intrinsic, 0 args
    StaticGeneric<int>.Bar(42)      // regressed: extension, 1 arg, see issue 19664 (FS0505)
    StaticGeneric<int>.Bar(42, 0)   // intrinsic, 2 args
        """
        |> typecheck
        |> shouldSucceed