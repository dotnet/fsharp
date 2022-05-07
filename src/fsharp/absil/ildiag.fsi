// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Diagnostics from the AbsIL toolkit. You can reset the diagnostics
/// stream to point elsewhere, or turn it
/// off altogether by setting it to 'None'.  The logging channel initially
/// points to stderr.  All functions call flush() automatically.
///
/// REVIEW: review if we should just switch to System.Diagnostics
module internal FSharp.Compiler.AbstractIL.Diagnostics

open System.IO
open Microsoft.FSharp.Core.Printf

val public setDiagnosticsChannel: TextWriter option -> unit

val public dprintfn: TextWriterFormat<'a> -> 'a
val public dprintf: TextWriterFormat<'a> -> 'a
val public dprintn: string -> unit
