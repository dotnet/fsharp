// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open CancellableTasks

type IFSharpCodeFix =
    abstract member GetChangesAsync: document: Document -> span: TextSpan -> CancellableTask<string * TextChange list>
