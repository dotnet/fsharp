// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Data.TypeProviders.Utility
module internal Util =
    open System
    open System.Reflection
    open System.Collections.Generic

    type ProcessResult = { exitCode : int; stdout : string[]; stderr : string[] }
    val executeProcess : workingDirectory: string * exe:string * cmdline:string -> ProcessResult
    /// Throws on failure
    val shell : workingDirectory: string * exe:string * cmdline:string * formatError:(string[] -> string[] -> unit) option -> unit

    val cscExe : unit -> string
    val dataSvcUtilExe : unit -> string
    val edmGenExe : unit -> string
    val svcUtilExe : unit -> string

    val sdkUtil : string -> string

    type FileResource =
        abstract member Path : string
        inherit IDisposable

    val TemporaryFile : extension:string -> FileResource
    val ExistingFile : fileName:string -> FileResource

    type DirectoryResource =
        abstract member Path : string
        inherit IDisposable

    val TemporaryDirectory : unit -> DirectoryResource
    val ExistingDirectory : dirName:string -> DirectoryResource

    type MemoizationTable<'T,'U  when 'T : equality> =
        new : ('T -> 'U) -> MemoizationTable<'T,'U>
        member Apply : 'T -> 'U
        member Contents : IDictionary<'T, 'U>

    val WithExclusiveAccessToFile : string -> (unit -> 'T)-> 'T

