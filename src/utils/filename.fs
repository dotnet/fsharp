// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Internal.Utilities.Filename

open System.IO
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 

exception IllegalFileNameChar of string * char

let illegalPathChars = Path.GetInvalidPathChars()

let checkPathForIllegalChars (path:string) = 
    for c in path do
        if illegalPathChars |> Array.exists(fun c1->c1=c) then 
            raise(IllegalFileNameChar(path,c))

// Case sensitive (original behaviour preserved).
let checkSuffix (x:string) (y:string) = x.EndsWith(y,System.StringComparison.Ordinal) 

let hasExtension (s:string) = 
    checkPathForIllegalChars s
    (s.Length >= 1 && s.[s.Length - 1] = '.' && s <> ".." && s <> ".") 
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
