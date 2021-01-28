// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

// This file contains public types related to the "file system hook" of the FCS API which are used throughout the F# compiler.
namespace FSharp.Compiler.IO

open System
open System.IO

/// Represents a shim for the file system
type public IFileSystem =

    /// Used to load a dependency for F# Interactive and in an unused corner-case of type provider loading
    abstract member AssemblyLoad: assemblyName:System.Reflection.AssemblyName -> System.Reflection.Assembly

    /// Used to load type providers and located assemblies in F# Interactive
    abstract member AssemblyLoadFrom: fileName:string -> System.Reflection.Assembly

    /// A shim over File.Delete
    abstract member FileDelete: fileName:string -> unit
    abstract member FileStreamCreateShim: fileName:string -> Stream

    /// A shim over FileStream with FileMode.Open, FileAccess.Read, FileShare.ReadWrite
    abstract member FileStreamReadShim: fileName:string -> Stream

    /// A shim over FileStream with FileMode.Open, FileAccess.Write, FileShare.Read
    abstract member FileStreamWriteExistingShim: fileName:string -> Stream

    /// Take in a filename with an absolute path, and return the same filename
    /// but canonicalized with respect to extra path separators (e.g. C:\\\\foo.txt) 
    /// and '..' portions
    abstract member GetFullPathShim: fileName:string -> string

    /// Utc time of the last modification
    abstract member GetLastWriteTimeShim: fileName:string -> DateTime

    /// A shim over Path.GetTempPath
    abstract member GetTempPathShim: unit -> string

    /// A shim over Path.IsInvalidPath
    abstract member IsInvalidPathShim: filename:string -> bool

    /// A shim over Path.IsPathRooted
    abstract member IsPathRootedShim: path:string -> bool

    /// Used to determine if a file will not be subject to deletion during the lifetime of a typical client process.
    abstract member IsStableFileHeuristic: fileName:string -> bool

    /// A shim over File.ReadAllBytes
    abstract member ReadAllBytesShim: fileName:string -> byte []

    /// A shim over File.Exists
    abstract member SafeExists: fileName:string -> bool

/// Represents a default implementation of the file system
type DefaultFileSystem =
    /// Create a default implementation of the file system
    new: unit -> DefaultFileSystem
    interface IFileSystem

[<AutoOpen>]
module public FileSystemAutoOpens =
    /// The global hook into the file system
    val mutable FileSystem: IFileSystem

    type System.IO.File with
        static member internal ReadBinaryChunk: fileName:string * start:int * len:int -> byte []

        static member internal OpenReaderAndRetry: filename:string * codepage:int option * retryLocked:bool -> System.IO.StreamReader


