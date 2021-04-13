// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.
// This file contains public types related to the "file system hook" of the FCS API which are used throughout the F# compiler.
namespace FSharp.Compiler.IO

open System
open System.IO
open System.Reflection

open Internal.Utilities.Library

exception IllegalFileNameChar of string * char

/// Filesystem helpers
module internal FileSystemUtils =
    val checkPathForIllegalChars: (string -> unit)

    /// <c>checkSuffix f s</c> returns True if filename "f" ends in suffix "s",
    /// e.g. checkSuffix "abc.fs" ".fs" returns true.
    val checkSuffix: string -> string -> bool

    /// <c>chopExtension f</c> removes the extension from the given
    /// filename. Raises <c>ArgumentException</c> if no extension is present.
    val chopExtension: string -> string

    /// Return True if the filename has a "." extension.
    val hasExtension: string -> bool

    /// Get the filename of the given path.
    val fileNameOfPath: string -> string

    /// Get the filename without extension of the given path.
    val fileNameWithoutExtensionWithValidate: bool -> string -> string
    val fileNameWithoutExtension: string -> string

    /// Trim the quotes and spaces from either end of a string
    val trimQuotes: string -> string

    /// Checks whether filename ends in suffidx, ignoring case.
    val hasSuffixCaseInsensitive: string -> string -> bool

    /// Checks whether file is dll (ends in .dll)
    val isDll: string -> bool

    // Reads all data from Stream.
    val readAllFromStream: Stream -> string

    // Yields content line by line from stream
    val readLinesFromStream: Stream -> string seq

    // Writes data to a stream
    val inline writeToStream: Stream ->  ^a -> unit


/// Represents a shim for the file system
type public IFileSystem =
    /// Used to load a dependency for F# Interactive and in an unused corner-case of type provider loading
    abstract member AssemblyLoad: assemblyName:System.Reflection.AssemblyName -> System.Reflection.Assembly

    /// Used to load type providers and located assemblies in F# Interactive
    abstract member AssemblyLoadFrom: fileName:string -> System.Reflection.Assembly

    /// Open the file, returns ByteMemory, which can then be read from or written to (depending on mode it was open with).
    abstract member OpenFileShim: filePath: string * access: FileAccess * ?shadowCopy: bool -> ByteMemory

    /// Take in a filename with an absolute path, and return the same filename
    /// but canonicalized with respect to extra path separators (e.g. C:\\\\foo.txt)
    /// and '..' portions
    abstract member GetFullPathShim: fileName:string -> string

    /// Take in a directory, filename, and return canonicalized path to the filename in directory.
    /// If filename path is rooted, ignores directory and returns filename path.
    /// Otherwise, combines directory with filename and gets full path via GetFullPathShim(string).
    abstract member GetFullFilePathInDirectoryShim: dir: string -> fileName: string -> string

    /// Utc time of the last modification
    abstract member GetLastWriteTimeShim: fileName:string -> DateTime

    // Utc time of creation
    abstract member GetCreationTimeShim: path: string -> DateTime

    /// A shim over Path.GetTempPath
    abstract member GetTempPathShim: unit -> string

    /// A shim for getting directory name from path
    abstract member GetDirectoryNameShim: path: string -> string

    /// A shim over Path.IsInvalidPath
    abstract member IsInvalidPathShim: filename:string -> bool

    /// A shim over Path.IsPathRooted
    abstract member IsPathRootedShim: path:string -> bool

    /// Removes relative parts from any full paths
    abstract member NormalizePathShim: path: string -> string

    /// Used to determine if a file will not be subject to deletion during the lifetime of a typical client process.
    abstract member IsStableFileHeuristic: fileName:string -> bool

    // A shim over file copying.
    abstract member CopyShim: src: string * dest: string * overwrite: bool -> unit

    /// A shim over File.Exists
    abstract member FileExistsShim: fileName: string -> bool

    /// A shim over File.Delete
    abstract member FileDeleteShim: fileName: string -> unit

    /// A shim over Directory.Exists
    abstract member DirectoryExistsShim: path: string -> bool

    /// A shim over Directory.Delete
    abstract member DirectoryDeleteShim: path: string -> unit

    /// A shim over Directory.EnumerateFiles
    abstract member EnumerateFilesShim: path: string * pattern: string -> string seq


/// Represents a default (memory-mapped) implementation of the file system
type MemoryMappedFileSystem =
    /// Create a default implementation of the file system
    new: unit -> MemoryMappedFileSystem
    interface IFileSystem

[<AutoOpen>]
module public FileSystemAutoOpens =
    /// The global hook into the file system
    val mutable FileSystem: IFileSystem