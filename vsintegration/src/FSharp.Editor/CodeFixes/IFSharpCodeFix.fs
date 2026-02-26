// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text

open Internal.Utilities.Library

type FSharpCodeFix =
    {
        Name: string
        Message: string
        Changes: TextChange list
    }

/// Provider can generate at most 1 suggestion.
type IFSharpCodeFixProvider =
    abstract member GetCodeFixIfAppliesAsync: context: CodeFixContext -> Async2<FSharpCodeFix voption>

/// Provider can generate multiple suggestions.
type IFSharpMultiCodeFixProvider =
    abstract member GetCodeFixesAsync: context: CodeFixContext -> Async2<FSharpCodeFix seq>
