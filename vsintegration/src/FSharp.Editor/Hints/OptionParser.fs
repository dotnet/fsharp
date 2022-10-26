// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open System.Collections.Generic
open Microsoft.VisualStudio.FSharp.Editor
open Hints

module OptionParser =

    let getHintKinds options =
        let hintKinds = new HashSet<HintKind>()

        if options.IsInlineTypeHintsEnabled
        then hintKinds.Add HintKind.TypeHint |> ignore

        if options.IsInlineParameterNameHintsEnabled
        then hintKinds.Add HintKind.ParameterNameHint |> ignore

        Set hintKinds