// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Internal.Utilities.Filename

open System.IO
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 

exception IllegalFileNameChar of string * char

let checkPathForIllegalChars  =
    let chars = new System.Collections.Generic.HashSet<_>(Path.GetInvalidPathChars())
    (fun (path:string) -> 
        for c in path do
            if chars.Contains c then raise(IllegalFileNameChar(path, c)))

// Case sensitive (original behaviour preserved).
let checkSuffix (x:string) (y:string) = x.EndsWithOrdinal(y) 

let hasExtensionWithValidate (validate:bool) (s:string) = 
    if validate then (checkPathForIllegalChars s) |> ignore
    let sLen = s.Length
    (sLen >= 1 && s.[sLen - 1] = '.' && s <> ".." && s <> ".") 
    || Path.HasExtension(s)

let hasExtension (s:string) = hasExtensionWithValidate true s

let chopExtension (s:string) =
    checkPathForIllegalChars s
    if s = "." then "" else // for OCaml compatibility
    if not (hasExtensionWithValidate false s) then 
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

let fileNameWithoutExtensionWithValidate (validate:bool) s = 
    if validate then checkPathForIllegalChars s |> ignore
    Path.GetFileNameWithoutExtension(s)

let fileNameWithoutExtension s = fileNameWithoutExtensionWithValidate true s

let trimQuotes (s:string) =
    s.Trim( [|' '; '\"'|] )
