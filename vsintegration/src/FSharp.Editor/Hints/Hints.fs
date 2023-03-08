// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open FSharp.Compiler.Text

module Hints =
    
    [<RequireQualifiedAccess>]
    type HintKind = 
        | TypeHint
        | ParameterNameHint

    // Relatively convenient for testing
    type NativeHint = {
        Kind: HintKind
        Range: range
        Parts: TaggedText list
    }