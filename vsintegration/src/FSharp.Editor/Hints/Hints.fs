// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open System.Threading
open Microsoft.CodeAnalysis
open FSharp.Compiler.Text

module Hints =

    type HintKind =
        | TypeHint
        | ParameterNameHint
        | ReturnTypeHint

    // Relatively convenient for testing
    type NativeHint =
        {
            Kind: HintKind
            Range: range
            Parts: TaggedText list
            GetToolTip: Document -> CancellationToken -> Async<TaggedText list>
        }

    let serialize kind =
        match kind with
        | TypeHint -> "type"
        | ParameterNameHint -> "parameterName"
        | ReturnTypeHint -> "returnType"
