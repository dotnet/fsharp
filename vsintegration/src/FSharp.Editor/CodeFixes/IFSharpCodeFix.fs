// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open CancellableTasks

type FSharpCodeFix =
    {
        Name: string
        Message: string
        Changes: TextChange list
    }

type IFSharpCodeFixProvider =
    abstract member GetCodeFixIfAppliesAsync: document: Document -> span: TextSpan -> CancellableTask<FSharpCodeFix option>
