// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

/// Lines are 1-based and inclusive; 0 and 0 when the file is known but no line is.
[<Struct>]
type internal EditorFocus =
    {
        FilePath: string
        FirstLine: int
        LastLine: int
    }
