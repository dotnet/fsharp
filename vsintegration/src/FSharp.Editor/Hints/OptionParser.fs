// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open System.Collections.Generic
open Microsoft.VisualStudio.FSharp.Editor
open Hints

module OptionParser =
    
    let getHintKinds options =
        Set 
            [ if options.IsInlineTypeHintsEnabled then
                 HintKind.TypeHint

              if options.IsInlineParameterNameHintsEnabled then
                 HintKind.ParameterNameHint ]