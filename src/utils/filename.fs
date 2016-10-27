// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Internal.Utilities.Filename

open System.IO
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 

exception IllegalFileNameChar of string * char

/// The set of characters which may not be used in a path.
/// This is saved here because Path.GetInvalidPathChars() allocates and returns
/// a new array each time it's called (by necessity, for security reasons).
/// This is only used within `checkPathForIllegalChars`, and is only read from.
let illegalPathChars =
    let chars = Path.GetInvalidPathChars ()
    chars

let checkPathForIllegalChars (path:string) =
    let len = path.Length
    for i = 0 to len - 1 do
        let c = path.[i]
        
        // Determine if this character is disallowed within a path by
        // attempting to find it in the array of illegal path characters.
        for badChar in illegalPathChars do
            if c = badChar then
                raise(IllegalFileNameChar(path, c))

// Case sensitive (original behaviour preserved).
let checkSuffix (x:string) (y:string) = x.EndsWith(y,System.StringComparison.Ordinal) 

let hasExtension (s:string) = 
    checkPathForIllegalChars s
    let sLen = s.Length
    (sLen >= 1 && s.[sLen - 1] = '.' && s <> ".." && s <> ".") 
    || Path.HasExtension(s)

let chopExtension (s:string) =
    checkPathForIllegalChars s
    if s = "." then "" else // for OCaml compatibility
    if not (hasExtension s) then 
        raise (System.ArgumentException("chopExtension")) // message has to be precisely this, for OCaml compatibility, and no argument name can be set
    Path.Combine (Path.GetDirectoryName s,Path.GetFileNameWithoutExtension(s))

let directoryName (s:string) = 
    checkPathForIllegalChars s
    if s = "" then "."
    else 
      match Path.GetDirectoryName(s) with 
      | null -> if FileSystem.IsPathRootedShim(s) then s else "."
      | res -> if res = "" then "." else res

let fileNameOfPath s = 
    checkPathForIllegalChars s
    Path.GetFileName(s)

let fileNameWithoutExtension s = 
    checkPathForIllegalChars s
    Path.GetFileNameWithoutExtension(s)

let trimQuotes (s:string) =
    s.Trim( [|' '; '\"'|] )
