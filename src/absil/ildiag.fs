// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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

//---------------------------------------------------------------------
// Library: ReportTime
//---------------------------------------------------------------------

let reportTime =
    let tFirst = ref None     
    let tPrev = ref None     
    fun showTimes descr ->
        if showTimes then 
            let t = System.Diagnostics.Process.GetCurrentProcess().UserProcessorTime.TotalSeconds
            let prev = match !tPrev with None -> 0.0 | Some t -> t
            let first = match !tFirst with None -> (tFirst := Some t; t) | Some t -> t
            dprintf "ilwrite: TIME %10.3f (total)   %10.3f (delta) - %s\n" (t - first) (t - prev) descr
            tPrev := Some t



