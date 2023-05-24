// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.CodeAnalysis
open FSharp.Compiler.Text
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

module Hints =

    [<RequireQualifiedAccess>]
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
            GetTooltip: Document -> CancellableTask<TaggedText list>
        }

    let inline serialize kind =
        match kind with
        | HintKind.TypeHint -> "type"
        | HintKind.ParameterNameHint -> "parameterName"
        | HintKind.ReturnTypeHint -> "returnType"
