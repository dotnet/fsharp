// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open FSharp.Test.Compiler
open Xunit

module StaticPropertyResolution =

    // Regression test for static property accessors (getter/setter) resolution.
    // Related to https://github.com/dotnet/fsharp/issues/19797
    //
    // When a static extension 'set' or 'get' accessor is defined in a *different* module 
    // than the generic type it extends, and the corresponding property has the other intrinsic 
    // static accessor 
    // (i.e. the intrinsic property has a 'get' and the extension has a 'set', or vice versa)
    //
    // For instance, resolving the assignment via explicit-type-argument syntax, 
    // where the 'set' accessor is an extension and the 'get' accessor is intrinsic,
    // Type<TArg>.Property <- value previously failed with FS0810.
    //
    // The non-generic dotted form `Type.Member`
    // resolved correctly, so any regression test that omits the explicit type arguments
    // does not actually exercise the bug. See the discussion at
    // https://github.com/dotnet/fsharp/issues/19675#issuecomment-4373059900.
    [<Fact>]
    let ``Static property on generic type resolves the extension accessor when the other is intrinsic``() =
        Fsx """
module Lib =
    
    type Label<'T> =
        static member Text with get() = "Static Intrinsic"
        
    [<AutoOpen>]
    module Utils =
        type Label<'T> with
            static member Text with set (v) = printfn "Extension"

module Program =
    open Lib

    Label<int>.Text <- "Text" // regressed: extension setter, see issue 19797 (FS0810)
        """
        |> typecheck
        |> shouldSucceed