// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO

module PathCompletionUtilities =

    let GetPathThroughLastSlash(quotedPath: string, quotedPathStart: int, position: int) =
        let quoteLength = "\"".Length
        let positionInQuotedPath = position - quotedPathStart
        let path = quotedPath.Substring(quoteLength, positionInQuotedPath - quoteLength).Trim()
        let afterLastSlashIndex = 
            let position = Math.Min(path.Length, path.Length - 1)
            let index = path.LastIndexOf(Path.DirectorySeparatorChar, position)
            if index >= 0 then index + 1 else -1
        if afterLastSlashIndex >= 0 then path.Substring(0, afterLastSlashIndex) else path

    let EndsWithQuote(quotedPath: string) =
        quotedPath.Length >= 2 && quotedPath.[quotedPath.Length - 1] = '"'
