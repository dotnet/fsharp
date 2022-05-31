// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.
// This file contains public types related to the "file system hook" of the FCS API which are used throughout the F# compiler.
namespace FSharp.Compiler.IO

open System
open System.IO
open System.IO.MemoryMappedFiles
open System.Reflection
open System.Text
open System.Runtime.CompilerServices

exception internal IllegalFileNameChar of string * char

module internal Bytes =
    /// returned int will be 0 <= x <= 255
    val get: byte[] -> int -> int

    val zeroCreate: int -> byte[]

    /// each int must be 0 <= x <= 255
    val ofInt32Array: int[] -> byte[]

    /// each int will be 0 <= x <= 255
    val blit: byte[] -> int -> byte[] -> int -> int -> unit

    val stringAsUnicodeNullTerminated: string -> byte[]

    val stringAsUtf8NullTerminated: string -> byte[]

/// A view over bytes.
/// May be backed by managed or unmanaged memory, or memory mapped file.
[<AbstractClass>]
type public ByteMemory =

    abstract Item: int -> byte with get

    abstract Length: int

    abstract ReadAllBytes: unit -> byte[]

    abstract ReadBytes: pos: int * count: int -> byte[]

    abstract ReadInt32: pos: int -> int

    abstract ReadUInt16: pos: int -> uint16

    abstract ReadUtf8String: pos: int * count: int -> string

    abstract Slice: pos: int * count: int -> ByteMemory

    abstract CopyTo: Stream -> unit

    abstract Copy: srcOffset: int * dest: byte[] * destOffset: int * count: int -> unit

    abstract ToArray: unit -> byte[]

    /// Get a stream representation of the backing memory.
    /// Disposing this will not free up any of the backing memory.
    abstract AsStream: unit -> Stream

    /// Get a stream representation of the backing memory.
    /// Disposing this will not free up any of the backing memory.
    /// Stream cannot be written to.
    abstract AsReadOnlyStream: unit -> Stream

[<Struct; NoEquality; NoComparison>]
type internal ReadOnlyByteMemory =

    new: ByteMemory -> ReadOnlyByteMemory

    member Item: int -> byte with get

    member Length: int

    member ReadAllBytes: unit -> byte[]

    member ReadBytes: pos: int * count: int -> byte[]

    member ReadInt32: pos: int -> int

    member ReadUInt16: pos: int -> uint16

    member ReadUtf8String: pos: int * count: int -> string

    member Slice: pos: int * count: int -> ReadOnlyByteMemory

    member CopyTo: Stream -> unit

    member Copy: srcOffset: int * dest: byte[] * destOffset: int * count: int -> unit

    member ToArray: unit -> byte[]

    member AsStream: unit -> Stream

/// MemoryMapped extensions
module internal MemoryMappedFileExtensions =
    type MemoryMappedFile with

        static member TryFromByteMemory: bytes: ReadOnlyByteMemory -> MemoryMappedFile option

        static member TryFromMemory: bytes: ReadOnlyMemory<byte> -> MemoryMappedFile option

/// Filesystem helpers
module internal FileSystemUtils =
    val checkPathForIllegalChars: (string -> unit)

    /// <c>checkSuffix f s</c> returns True if file name "f" ends in suffix "s",
    /// e.g. checkSuffix "abc.fs" ".fs" returns true.
    /// Disregards casing, e.g. checkSuffix "abc.Fs" ".fs" returns true.
    val checkSuffix: path: string -> suffix: string -> bool

    /// <c>chopExtension f</c> removes the extension from the given
    /// file name. Raises <c>ArgumentException</c> if no extension is present.
    val chopExtension: path: string -> string

    /// Return True if the path has a "." extension.
    val hasExtension: path: string -> bool

    /// Get the file name of the given path.
    val fileNameOfPath: path: string -> string

    /// Get the file name without extension of the given path.
    val fileNameWithoutExtensionWithValidate: validate: bool -> path: string -> string

    val fileNameWithoutExtension: path: string -> string

    /// Trim the quotes and spaces from either end of a string
    val trimQuotes: path: string -> string

    /// Checks whether file is dll (ends in .dll)
    val isDll: fileName: string -> bool

/// Type which we use to load assemblies.
type public IAssemblyLoader =
    /// Used to load a dependency for F# Interactive and in an unused corner-case of type provider loading
    abstract AssemblyLoad: assemblyName: AssemblyName -> Assembly

    /// Used to load type providers and located assemblies in F# Interactive
    abstract AssemblyLoadFrom: fileName: string -> Assembly

/// Default implementation for IAssemblyLoader
type DefaultAssemblyLoader =
    new: unit -> DefaultAssemblyLoader
    interface IAssemblyLoader

/// Represents a shim for the file system
type public IFileSystem =

    // Assembly loader.
    abstract AssemblyLoader: IAssemblyLoader

    /// Open the file for read, returns ByteMemory, uses either FileStream (for smaller files) or MemoryMappedFile (for potentially big files, such as dlls).
    abstract OpenFileForReadShim: filePath: string * ?useMemoryMappedFile: bool * ?shouldShadowCopy: bool -> Stream

    /// Open the file for writing. Returns a Stream.
    abstract OpenFileForWriteShim:
        filePath: string * ?fileMode: FileMode * ?fileAccess: FileAccess * ?fileShare: FileShare -> Stream

    /// Take in a file name with an absolute path, and return the same file name
    /// but canonicalized with respect to extra path separators (e.g. C:\\\\foo.txt)
    /// and '..' portions
    abstract GetFullPathShim: fileName: string -> string

    /// Take in a directory, filename, and return canonicalized path to the file name in directory.
    /// If file name path is rooted, ignores directory and returns file name path.
    /// Otherwise, combines directory with file name and gets full path via GetFullPathShim(string).
    abstract GetFullFilePathInDirectoryShim: dir: string -> fileName: string -> string

    /// A shim over Path.IsPathRooted
    abstract IsPathRootedShim: path: string -> bool

    /// Removes relative parts from any full paths
    abstract NormalizePathShim: path: string -> string

    /// A shim over Path.IsInvalidPath
    abstract IsInvalidPathShim: path: string -> bool

    /// A shim over Path.GetTempPath
    abstract GetTempPathShim: unit -> string

    /// A shim for getting directory name from path
    abstract GetDirectoryNameShim: path: string -> string

    /// Utc time of the last modification
    abstract GetLastWriteTimeShim: fileName: string -> DateTime

    // Utc time of creation
    abstract GetCreationTimeShim: path: string -> DateTime

    // A shim over file copying.
    abstract CopyShim: src: string * dest: string * overwrite: bool -> unit

    /// A shim over File.Exists
    abstract FileExistsShim: fileName: string -> bool

    /// A shim over File.Delete
    abstract FileDeleteShim: fileName: string -> unit

    /// A shim over Directory.Exists, but returns a string, the FullName of the resulting
    /// DirectoryInfo.
    abstract DirectoryCreateShim: path: string -> string

    /// A shim over Directory.Exists
    abstract DirectoryExistsShim: path: string -> bool

    /// A shim over Directory.Delete
    abstract DirectoryDeleteShim: path: string -> unit

    /// A shim over Directory.EnumerateFiles
    abstract EnumerateFilesShim: path: string * pattern: string -> string seq

    /// A shim over Directory.EnumerateDirectories
    abstract EnumerateDirectoriesShim: path: string -> string seq

    /// Used to determine if a file will not be subject to deletion during the lifetime of a typical client process.
    abstract IsStableFileHeuristic: fileName: string -> bool

/// Represents a default (memory-mapped) implementation of the file system
type DefaultFileSystem =
    /// Create a default implementation of the file system
    new: unit -> DefaultFileSystem
    abstract AssemblyLoader: IAssemblyLoader
    override AssemblyLoader: IAssemblyLoader

    abstract OpenFileForReadShim: filePath: string * ?useMemoryMappedFile: bool * ?shouldShadowCopy: bool -> Stream
    override OpenFileForReadShim: filePath: string * ?useMemoryMappedFile: bool * ?shouldShadowCopy: bool -> Stream

    abstract OpenFileForWriteShim:
        filePath: string * ?fileMode: FileMode * ?fileAccess: FileAccess * ?fileShare: FileShare -> Stream
    override OpenFileForWriteShim:
        filePath: string * ?fileMode: FileMode * ?fileAccess: FileAccess * ?fileShare: FileShare -> Stream

    abstract GetFullPathShim: fileName: string -> string
    override GetFullPathShim: fileName: string -> string

    abstract GetFullFilePathInDirectoryShim: dir: string -> fileName: string -> string
    override GetFullFilePathInDirectoryShim: dir: string -> fileName: string -> string

    abstract IsPathRootedShim: path: string -> bool
    override IsPathRootedShim: path: string -> bool

    abstract NormalizePathShim: path: string -> string
    override NormalizePathShim: path: string -> string

    abstract IsInvalidPathShim: path: string -> bool
    override IsInvalidPathShim: path: string -> bool

    abstract GetTempPathShim: unit -> string
    override GetTempPathShim: unit -> string

    abstract GetDirectoryNameShim: path: string -> string
    override GetDirectoryNameShim: path: string -> string

    abstract GetLastWriteTimeShim: fileName: string -> DateTime
    override GetLastWriteTimeShim: fileName: string -> DateTime

    abstract GetCreationTimeShim: path: string -> DateTime
    override GetCreationTimeShim: path: string -> DateTime

    abstract CopyShim: src: string * dest: string * overwrite: bool -> unit
    override CopyShim: src: string * dest: string * overwrite: bool -> unit

    abstract FileExistsShim: fileName: string -> bool
    override FileExistsShim: fileName: string -> bool

    abstract FileDeleteShim: fileName: string -> unit
    override FileDeleteShim: fileName: string -> unit

    abstract DirectoryCreateShim: path: string -> string
    override DirectoryCreateShim: path: string -> string

    abstract DirectoryExistsShim: path: string -> bool
    override DirectoryExistsShim: path: string -> bool

    abstract DirectoryDeleteShim: path: string -> unit
    override DirectoryDeleteShim: path: string -> unit

    abstract EnumerateFilesShim: path: string * pattern: string -> string seq
    override EnumerateFilesShim: path: string * pattern: string -> string seq

    abstract EnumerateDirectoriesShim: path: string -> string seq
    override EnumerateDirectoriesShim: path: string -> string seq

    abstract IsStableFileHeuristic: fileName: string -> bool
    override IsStableFileHeuristic: fileName: string -> bool

    interface IFileSystem

[<AutoOpen>]
module public StreamExtensions =

    type System.IO.Stream with

        member GetWriter: ?encoding: Encoding -> TextWriter
        member WriteAllLines: contents: string seq * ?encoding: Encoding -> unit
        member Write<'a> : data: 'a -> unit
        member GetReader: codePage: int option * ?retryLocked: bool -> StreamReader
        member ReadBytes: start: int * len: int -> byte[]
        member ReadAllBytes: unit -> byte[]
        member ReadAllText: ?encoding: Encoding -> string
        member ReadLines: ?encoding: Encoding -> string seq
        member ReadAllLines: ?encoding: Encoding -> string array
        member WriteAllText: text: string -> unit
        member AsByteMemory: unit -> ByteMemory

[<AutoOpen>]
module public FileSystemAutoOpens =
    /// The global hook into the file system
    val mutable FileSystem: IFileSystem

type internal ByteMemory with

    member AsReadOnly: unit -> ReadOnlyByteMemory

    /// Empty byte memory.
    static member Empty: ByteMemory

    /// Create a ByteMemory object that has a backing memory mapped file.
    static member FromMemoryMappedFile: MemoryMappedFile -> ByteMemory

    /// Creates a ByteMemory object that is backed by a raw pointer.
    /// Use with care.
    static member FromUnsafePointer: addr: nativeint * length: int * holder: obj -> ByteMemory

    /// Creates a ByteMemory object that is backed by a byte array with the specified offset and length.
    static member FromArray: bytes: byte[] * offset: int * length: int -> ByteMemory

    /// Creates a ByteMemory object that is backed by a byte array.
    static member FromArray: bytes: byte[] -> ByteMemory

[<Sealed>]
type internal ByteStream =
    member ReadByte: unit -> byte
    member ReadBytes: int -> ReadOnlyByteMemory
    member ReadUtf8String: int -> string
    member Position: int
    static member FromBytes: ReadOnlyByteMemory * start: int * length: int -> ByteStream

#if LAZY_UNPICKLE
    member CloneAndSeek: int -> ByteStream
    member Skip: int -> unit
#endif

/// Imperative buffers and streams of byte[]
/// Not thread safe.
[<Sealed>]
type internal ByteBuffer =
    interface IDisposable

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member AsMemory: unit -> ReadOnlyMemory<byte>

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitIntAsByte: int -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitIntsAsBytes: int[] -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitByte: byte -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitBytes: byte[] -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitMemory: ReadOnlyMemory<byte> -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitByteMemory: ReadOnlyByteMemory -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitInt32: int32 -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitInt64: int64 -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member FixupInt32: pos: int -> value: int32 -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitInt32AsUInt16: int32 -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitBoolAsByte: bool -> unit

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member EmitUInt16: uint16 -> unit

    member Position: int
    static member Create: capacity: int * ?useArrayPool: bool -> ByteBuffer

[<Sealed>]
type internal ByteStorage =

    member GetByteMemory: unit -> ReadOnlyByteMemory

    /// Creates a ByteStorage whose backing bytes are the given ByteMemory. Does not make a copy.
    static member FromByteMemory: ReadOnlyByteMemory -> ByteStorage

    /// Creates a ByteStorage whose backing bytes are the given byte array. Does not make a copy.
    static member FromByteArray: byte[] -> ByteStorage

    /// Creates a ByteStorage that has a copy of the given ByteMemory.
    static member FromByteMemoryAndCopy: ReadOnlyByteMemory * useBackingMemoryMappedFile: bool -> ByteStorage

    /// Creates a ByteStorage that has a copy of the given Memory<byte>.
    static member FromMemoryAndCopy: ReadOnlyMemory<byte> * useBackingMemoryMappedFile: bool -> ByteStorage

    /// Creates a ByteStorage that has a copy of the given byte array.
    static member FromByteArrayAndCopy: byte[] * useBackingMemoryMappedFile: bool -> ByteStorage
