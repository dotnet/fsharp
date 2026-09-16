// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

/// The file the user last edited and the lines the caret or selection covers, 1-based and inclusive;
/// 0 and 0 when the file is known but no line is.
[<Struct>]
type internal EditorFocus =
    {
        FilePath: string
        FirstLine: int
        LastLine: int
    }
