// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Blobs of bytes, cross-compiling 
namespace FSharp.Compiler.AbstractIL.Internal

open System
open System.IO

module internal Bytes = 
    /// returned int will be 0 <= x <= 255
    val get: byte[] -> int -> int    
    val zeroCreate: int -> byte[]
    /// each int must be 0 <= x <= 255 
    val ofInt32Array: int[] ->  byte[] 
    /// each int will be 0 <= x <= 255 

    val blit: byte[] -> int -> byte[] -> int -> int -> unit

    val stringAsUnicodeNullTerminated: string -> byte[]
    val stringAsUtf8NullTerminated: string -> byte[]

type internal ChunkedArrayForEachDelegate<'T> = delegate of Span<'T> -> unit

[<Struct;NoEquality;NoComparison>]
type internal ChunkedArray<'T> =

    member Length: int

    member ForEachChunk: ChunkedArrayForEachDelegate<'T> -> unit

    member ToArray: unit -> 'T[]

    member IsEmpty: bool

    static member Empty: ChunkedArray<'T>

/// Not thread safe.
[<Sealed>]
type internal ChunkedArrayBuilder<'T> =

    member Reserve: length: int -> Span<'T>

    member AddSpan: data: ReadOnlySpan<'T> -> unit

    member Add: data: 'T -> unit

    member RemoveAll: predicate: ('T -> bool) -> unit

    member ForEachChunk: ChunkedArrayForEachDelegate<'T> -> unit

    /// When the builder is turned into a chunked array, it is considered frozen.
    member ToChunkedArray: unit -> ChunkedArray<'T>

    static member Create: minChunkSize: int * startingCapacity: int -> ChunkedArrayBuilder<'T>

[<RequireQualifiedAccess>]
module internal ChunkedArray =

    val iter: ('T -> unit) -> ChunkedArray<'T> -> unit

    val map: ('T -> 'U) -> ChunkedArray<'T> -> ChunkedArray<'U>

    val filter: ('T -> bool) -> ChunkedArray<'T> -> ChunkedArray<'T>

    val choose: ('T -> 'U option) -> ChunkedArray<'T> -> ChunkedArray<'U>

    val distinctBy: ('T -> 'Key) -> ChunkedArray<'T> -> ChunkedArray<'T> when 'Key : equality

    val concat: ChunkedArray<'T> seq -> ChunkedArray<'T>

    val toArray: ChunkedArray<'T> -> 'T[]

    val toReversedList: ChunkedArray<'T> -> 'T list

/// May be backed by managed or unmanaged memory, or memory mapped file.
[<AbstractClass>]
type internal ByteMemory =

    abstract Item: int -> byte with get

    abstract Length: int

    abstract ReadBytes: pos: int * count: int -> byte[]

    abstract ReadInt32: pos: int -> int

    abstract ReadUInt16: pos: int -> uint16

    abstract ReadUtf8String: pos: int * count: int -> string

    abstract Slice: pos: int * count: int -> ByteMemory

    abstract CopyTo: Stream -> unit

    abstract CopyTo: Span<byte> -> unit

    abstract Copy: srcOffset: int * dest: byte[] * destOffset: int * count: int -> unit

    abstract ToArray: unit -> byte[]

    /// Get a stream representation of the backing memory.
    /// Disposing this will not free up any of the backing memory.
    abstract AsStream: unit -> Stream

    /// Get a stream representation of the backing memory.
    /// Disposing this will not free up any of the backing memory.
    /// Stream cannot be written to.
    abstract AsReadOnlyStream: unit -> Stream

[<Struct;NoEquality;NoComparison>]
type internal ReadOnlyByteMemory =

    new: ByteMemory -> ReadOnlyByteMemory

    member Item: int -> byte with get

    member Length: int

    member ReadBytes: pos: int * count: int -> byte[]

    member ReadInt32: pos: int -> int

    member ReadUInt16: pos: int -> uint16

    member ReadUtf8String: pos: int * count: int -> string

    member Slice: pos: int * count: int -> ReadOnlyByteMemory

    member CopyTo: Stream -> unit

    member CopyTo: Span<byte> -> unit

    member Copy: srcOffset: int * dest: byte[] * destOffset: int * count: int -> unit

    member ToArray: unit -> byte[]

    /// Get a stream representation of the backing memory.
    /// Disposing this will not free up any of the backing memory.
    /// Stream cannot be written to.
    member AsStream: unit -> Stream

type internal ByteMemory with

    member AsReadOnly: unit -> ReadOnlyByteMemory

    /// Create another ByteMemory object that has a backing memory mapped file based on another ByteMemory's contents.
    static member CreateMemoryMappedFile: ReadOnlyByteMemory -> ByteMemory

    /// Creates a ByteMemory object that has a backing memory mapped file from a file on-disk.
    static member FromFile: path: string * FileAccess * ?canShadowCopy: bool -> ByteMemory

    /// Creates a ByteMemory object that is backed by a raw pointer.
    /// Use with care.
    static member FromUnsafePointer: addr: nativeint * length: int * hold: obj -> ByteMemory

    /// Creates a ByteMemory object that is backed by a byte array with the specified offset and length.
    static member FromArray: bytes: byte[] * offset: int * length: int -> ByteMemory

    /// Creates a ByteMemory object that is backed by a byte array.
    static member FromArray: bytes: byte[] -> ByteMemory

/// Imperative buffers and streams of byte[]
[<Sealed>]
type internal ByteBuffer = 
    member Reserve : int -> Span<byte>
    member Close : unit -> byte[] 
    member EmitIntAsByte : int -> unit
    member EmitIntsAsBytes : int[] -> unit
    member EmitInt32s : int32[] -> unit
    member EmitByte : byte -> unit
    member EmitBytes : byte[] -> unit
    member EmitByteMemory : ReadOnlyByteMemory -> unit
    member EmitInt32 : int32 -> unit
    member EmitInt64 : int64 -> unit
    member EmitInt32AsUInt16 : int32 -> unit
    member EmitBoolAsByte : bool -> unit
    member EmitUInt16 : uint16 -> unit
    member Position : int
    static member Create : startingCapacity: int -> ByteBuffer


[<Sealed>]
type internal ByteStream =
    member ReadByte : unit -> byte
    member ReadBytes : int -> ReadOnlyByteMemory
    member ReadUtf8String : int -> string
    member Position : int 
    static member FromBytes : ReadOnlyByteMemory * start:int * length:int -> ByteStream
    
#if LAZY_UNPICKLE
    member CloneAndSeek : int -> ByteStream
    member Skip : int -> unit
#endif
