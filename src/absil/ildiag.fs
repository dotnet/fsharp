// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Configurable AppDomain-global diagnostics channel for the Abstract IL library
///
/// REVIEW: review if we should just switch to System.Diagnostics
module internal Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 

open Internal.Utilities

open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 

let diagnosticsLog = ref (Some stdout)
let dflushn () = match !diagnosticsLog with None -> () | Some d -> d.WriteLine(); d.Flush()
let dflush () = match !diagnosticsLog with None -> () | Some d -> d.Flush()
let dprintn (s:string) = 
  match !diagnosticsLog with None -> () | Some d -> d.Write s; d.Write "\n"; dflush()

let dprintf (fmt: Format<_,_,_,_>) = 
    Printf.kfprintf dflush (match !diagnosticsLog with None -> System.IO.TextWriter.Null | Some d -> d) fmt

let dprintfn (fmt: Format<_,_,_,_>) = 
    Printf.kfprintf dflushn (match !diagnosticsLog with None -> System.IO.TextWriter.Null | Some d -> d) fmt

let setDiagnosticsChannel s = diagnosticsLog := s
