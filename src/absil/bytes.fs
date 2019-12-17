// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Byte arrays
namespace FSharp.Compiler.AbstractIL.Internal

open System
open System.IO
open System.IO.MemoryMappedFiles
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open FSharp.Compiler.AbstractIL.Internal.Library 

#nowarn "9"

module internal Bytes = 
    let b0 n =  (n &&& 0xFF)
    let b1 n =  ((n >>> 8) &&& 0xFF)
    let b2 n =  ((n >>> 16) &&& 0xFF)
    let b3 n =  ((n >>> 24) &&& 0xFF)

    let dWw1 n = int32 ((n >>> 32) &&& 0xFFFFFFFFL)
    let dWw0 n = int32 (n          &&& 0xFFFFFFFFL)

    let get (b:byte[]) n = int32 (Array.get b n)  
    let zeroCreate n : byte[] = Array.zeroCreate n      

    let sub ( b:byte[]) s l = Array.sub b s l   
    let blit (a:byte[]) b c d e = Array.blit a b c d e 

    let ofInt32Array (arr:int[]) = Array.init arr.Length (fun i -> byte arr.[i]) 

    let stringAsUtf8NullTerminated (s:string) = 
        Array.append (System.Text.Encoding.UTF8.GetBytes s) (ofInt32Array [| 0x0 |]) 

    let stringAsUnicodeNullTerminated (s:string) = 
        Array.append (System.Text.Encoding.Unicode.GetBytes s) (ofInt32Array [| 0x0;0x0 |])

type ChunkedArrayForEachDelegate<'T> = delegate of Span<'T> -> unit

/// Not thread safe.
/// Loosely based on StringBuilder/BlobBuilder
[<Sealed>]
type ChunkedArrayBuilder<'T> private (minChunkSize: int, buffer: 'T []) =

    member val private Buffer = buffer with get, set
    member val private ChunkLength = 0 with get, set
    member val private NextOrPrevious = Unchecked.defaultof<ChunkedArrayBuilder<'T>> with get, set
    member val private Position = 0 with get, set
    member val private IsFrozen = false with get, set
    member val private ChunkCount = 1 with get, set

    // [1:first]->[2]->[3:last]<-[4:head]
    //     ^_______________|
    member private x.FirstChunk = x.NextOrPrevious.NextOrPrevious
    
    member private x.CheckIsFrozen() =
        if x.IsFrozen then
            invalidOp "Chunked array builder is frozen and cannot add or remove items."

    member x.IsEmpty = x.ChunkCount = 1 && x.ChunkLength = 0

    member x.Reserve length =
        x.CheckIsFrozen()

        if length + x.Position > x.Buffer.Length then
            x.ChunkCount <- x.ChunkCount + 1
            let newChunk = 
                ChunkedArrayBuilder<'T>(
                    minChunkSize, 
                    Array.zeroCreate<'T> (Math.Max(length, minChunkSize)), 
                    IsFrozen = true)
            let newBuffer = newChunk.Buffer

            match box x.NextOrPrevious with
            | null -> ()
            | _ ->
                match box x.FirstChunk with
                | null ->
                    let firstChunk = x.NextOrPrevious
                    firstChunk.NextOrPrevious <- newChunk
                    newChunk.NextOrPrevious <- firstChunk
                | _ ->
                    let firstChunk = x.FirstChunk
                    let lastChunk = x.NextOrPrevious
                    lastChunk.NextOrPrevious <- newChunk
                    newChunk.NextOrPrevious <- firstChunk

            newChunk.ChunkLength <- x.ChunkLength
            newChunk.Buffer <- x.Buffer
            x.Buffer <- newBuffer
            x.NextOrPrevious <- newChunk
            x.ChunkLength <- 0
            x.Position <- 0
        let reserved = Span(x.Buffer, x.Position, length)
        x.ChunkLength <- x.ChunkLength + length
        x.Position <- x.ChunkLength
        reserved

    member x.AddSpan(data: ReadOnlySpan<'T>) =
        let reserved = x.Reserve data.Length
        data.CopyTo(reserved)

    member x.Add(data: 'T) =
        x.AddSpan(ReadOnlySpan(Unsafe.AsPointer(&Unsafe.AsRef(&data)), 1))

    member x.ForEachBuilder f =
        match box x.NextOrPrevious with
        | null -> ()
        | _ ->
            match box x.FirstChunk with
            | null ->
                f x.NextOrPrevious
            | _ ->
                let firstChunk = x.FirstChunk
                f firstChunk
                let mutable chunk = firstChunk.NextOrPrevious
                while chunk <> firstChunk do
                    f chunk
                    chunk <- chunk.NextOrPrevious

        if x.ChunkLength > 0 then
            f x

    member x.ForEachChunk (f: ChunkedArrayForEachDelegate<'T>) =
        x.ForEachBuilder (fun builder -> 
            // Note: This could happen due to removing items. 
            //       It might be better to remove the node itself instead of checking for the length in the for loop.
            if builder.ChunkLength > 0 then
                f.Invoke(Span(builder.Buffer, 0, builder.ChunkLength)))

    member x.RemoveAll predicate =
        x.CheckIsFrozen()
        x.ForEachBuilder(fun builder ->
            let removeQueue = Collections.Generic.Queue()

            for i = 0 to builder.ChunkLength - 1 do
                let item = builder.Buffer.[i]
                if predicate item then
                    removeQueue.Enqueue i

            while removeQueue.Count > 0 do
                let indexToRemove = removeQueue.Dequeue()

                if indexToRemove = builder.ChunkLength - 1 then
                    builder.Buffer.[indexToRemove] <- Unchecked.defaultof<_>
                else
                    for i = indexToRemove to builder.ChunkLength - 2 do
                        builder.Buffer.[i] <- builder.Buffer.[i + 1]
                    builder.Buffer.[builder.ChunkLength - 1] <- Unchecked.defaultof<_>

                builder.ChunkLength <- builder.ChunkLength - 1
              
            assert (builder.ChunkLength >= 0)

            if builder.ChunkLength = 0 then
                x.ChunkCount <- x.ChunkCount - 1)

    static member Create(minChunkSize, startingCapacity) =
        ChunkedArrayBuilder(
            minChunkSize, 
            Array.zeroCreate<'T> (Math.Max(startingCapacity, minChunkSize)))
                
[<Struct;NoEquality;NoComparison>]
type ChunkedArray<'T>(builder: ChunkedArrayBuilder<'T>) =

    member _.Length =
        match box builder with
        | null -> 0
        | _ ->
            let mutable total = 0
            builder.ForEachChunk (fun chunk -> total <- total + chunk.Length)
            total

    member _.ForEachChunk f = 
        match box builder with
        | null -> ()
        | _ ->
            builder.ForEachChunk f

    member x.ToArray() =
        match box builder with
        | null -> [||]
        | _ ->
            let arr = Array.zeroCreate<'T> x.Length
            let mutable total = 0
            builder.ForEachChunk (fun chunk -> 
                chunk.CopyTo(Span(arr, total, chunk.Length))
                total <- total + chunk.Length)
            arr

    member x.IsEmpty =
        match box builder with
        | null -> true
        | _ -> builder.IsEmpty

    static member Empty =
        ChunkedArray<'T> Unchecked.defaultof<_>

type ChunkedArrayBuilder<'T> with

    member x.ToChunkedArray() = 
        x.IsFrozen <- true
        ChunkedArray x

[<RequireQualifiedAccess>]
module ChunkedArray =

    [<Literal>]
    let MinChunkSize = 16

    [<Literal>]
    let DefaultChunkSize = 256

    let iter f (chunkedArr: ChunkedArray<_>) =
        chunkedArr.ForEachChunk(fun chunk ->
            for item in chunk do
                f item)

    let map f (chunkedArr: ChunkedArray<_>) =
        if chunkedArr.IsEmpty then ChunkedArray<_>.Empty
        else
            let res = ChunkedArrayBuilder<_>.Create(MinChunkSize, DefaultChunkSize)
            chunkedArr.ForEachChunk(fun chunk ->
                for item in chunk do
                    res.Add (f item))
            res.ToChunkedArray()

    let filter predicate (chunkedArr: ChunkedArray<_>) =
        if chunkedArr.IsEmpty then ChunkedArray<_>.Empty
        else
            let res = ChunkedArrayBuilder<_>.Create(MinChunkSize, DefaultChunkSize)
            chunkedArr.ForEachChunk(fun chunk ->
                for item in chunk do
                    if predicate item then
                        res.Add item)
            res.ToChunkedArray()

    let choose chooser (chunkedArr: ChunkedArray<_>) =
        if chunkedArr.IsEmpty then ChunkedArray<_>.Empty
        else
            let res = ChunkedArrayBuilder<_>.Create(MinChunkSize, DefaultChunkSize)
            chunkedArr.ForEachChunk(fun chunk ->
                for item in chunk do
                    match chooser item with
                    | Some mappedItem -> res.Add mappedItem
                    | _ -> ())
            res.ToChunkedArray()

    let toArray (chunkedArr: ChunkedArray<_>) =
        chunkedArr.ToArray()

    let toReversedList (chunkedArr: ChunkedArray<_>) =
        let mutable res = []
        chunkedArr
        |> iter (fun item -> res <- item :: res)
        res

[<AbstractClass>]
type ByteMemory () =

    abstract Item: int -> byte with get, set

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

    abstract AsStream: unit -> Stream

    abstract AsReadOnlyStream: unit -> Stream

[<Sealed>]
type ByteArrayMemory(bytes: byte[], offset, length) =
    inherit ByteMemory()

    do
        if length <= 0 || length > bytes.Length then
            raise (ArgumentOutOfRangeException("length"))

        if offset < 0 || (offset + length) > bytes.Length then
            raise (ArgumentOutOfRangeException("offset"))
    
    override _.Item 
        with get i = bytes.[offset + i]
        and set i v = bytes.[offset + i] <- v

    override _.Length = length

    override _.ReadBytes(pos, count) = 
        Array.sub bytes (offset + pos) count

    override _.ReadInt32 pos =
        let finalOffset = offset + pos
        (uint32 bytes.[finalOffset]) |||
        ((uint32 bytes.[finalOffset + 1]) <<< 8) |||
        ((uint32 bytes.[finalOffset + 2]) <<< 16) |||
        ((uint32 bytes.[finalOffset + 3]) <<< 24)
        |> int

    override _.ReadUInt16 pos =
        let finalOffset = offset + pos
        (uint16 bytes.[finalOffset]) |||
        ((uint16 bytes.[finalOffset + 1]) <<< 8)

    override _.ReadUtf8String(pos, count) =
        System.Text.Encoding.UTF8.GetString(bytes, offset + pos, count)

    override _.Slice(pos, count) =
        ByteArrayMemory(bytes, offset + pos, count) :> ByteMemory

    override _.CopyTo(stream: Stream) =
        stream.Write(bytes, offset, length)

    override _.CopyTo(span: Span<byte>) =
        ReadOnlySpan(bytes, offset, length).CopyTo span

    override _.Copy(srcOffset, dest, destOffset, count) =
        Array.blit bytes (offset + srcOffset) dest destOffset count

    override _.ToArray() =
        Array.sub bytes offset length

    override _.AsStream() =
        new MemoryStream(bytes, offset, length) :> Stream

    override _.AsReadOnlyStream() =
        new MemoryStream(bytes, offset, length, false) :> Stream

[<Sealed>]
type RawByteMemory(addr: nativeptr<byte>, length: int, hold: obj) =
    inherit ByteMemory ()

    let check i =
        if i < 0 || i >= length then 
            raise (ArgumentOutOfRangeException("i"))

    do
        if length <= 0 then
            raise (ArgumentOutOfRangeException("length"))

    override _.Item 
        with get i = 
            check i
            NativePtr.add addr i
            |> NativePtr.read 
        and set i v =
            check i
            NativePtr.set addr i v

    override _.Length = length

    override _.ReadUtf8String(pos, count) =
        check pos
        check (pos + count - 1)
        System.Text.Encoding.UTF8.GetString(NativePtr.add addr pos, count)

    override _.ReadBytes(pos, count) = 
        check pos
        check (pos + count - 1)
        let res = Bytes.zeroCreate count
        Marshal.Copy(NativePtr.toNativeInt addr + nativeint pos, res, 0, count)
        res

    override _.ReadInt32 pos =
        check pos
        check (pos + 3)
        Marshal.ReadInt32(NativePtr.toNativeInt addr + nativeint pos)

    override _.ReadUInt16 pos =
        check pos
        check (pos + 1)
        uint16(Marshal.ReadInt16(NativePtr.toNativeInt addr + nativeint pos))

    override _.Slice(pos, count) =
        check pos
        check (pos + count - 1)
        RawByteMemory(NativePtr.add addr pos, count, hold) :> ByteMemory

    override x.CopyTo(stream: Stream) =
        use stream2 = x.AsStream()
        stream2.CopyTo stream

    override x.CopyTo(span: Span<byte>) =
        ReadOnlySpan(NativePtr.toVoidPtr addr, length).CopyTo span

    override x.Copy(srcOffset, dest, destOffset, count) =
        check srcOffset
        Marshal.Copy(NativePtr.toNativeInt addr + nativeint srcOffset, dest, destOffset, count)

    override _.ToArray() =
        let res = Array.zeroCreate<byte> length
        Marshal.Copy(NativePtr.toNativeInt addr, res, 0, res.Length)
        res

    override _.AsStream() =
        new UnmanagedMemoryStream(addr, int64 length) :> Stream

    override _.AsReadOnlyStream() =
        new UnmanagedMemoryStream(addr, int64 length, int64 length, FileAccess.Read) :> Stream

[<Struct;NoEquality;NoComparison>]
type ReadOnlyByteMemory(bytes: ByteMemory) =

    member _.Item with [<MethodImpl(MethodImplOptions.AggressiveInlining)>] get i = bytes.[i]

    member _.Length with [<MethodImpl(MethodImplOptions.AggressiveInlining)>] get () = bytes.Length

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.ReadBytes(pos, count) = bytes.ReadBytes(pos, count)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.ReadInt32 pos = bytes.ReadInt32 pos

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.ReadUInt16 pos = bytes.ReadUInt16 pos

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.ReadUtf8String(pos, count) = bytes.ReadUtf8String(pos, count)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Slice(pos, count) = bytes.Slice(pos, count) |> ReadOnlyByteMemory

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.CopyTo(stream: Stream) = bytes.CopyTo stream

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.CopyTo(span: Span<byte>) = bytes.CopyTo span

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Copy(srcOffset, dest, destOffset, count) = bytes.Copy(srcOffset, dest, destOffset, count)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.ToArray() = bytes.ToArray()

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.AsStream() = bytes.AsReadOnlyStream()

type ByteMemory with

    member x.AsReadOnly() = ReadOnlyByteMemory x

    static member CreateMemoryMappedFile(bytes: ReadOnlyByteMemory) =
        let length = int64 bytes.Length
        let mmf = 
            let mmf =
                MemoryMappedFile.CreateNew(
                    null, 
                    length, 
                    MemoryMappedFileAccess.ReadWrite, 
                    MemoryMappedFileOptions.None, 
                    HandleInheritability.None)
            use stream = mmf.CreateViewStream(0L, length, MemoryMappedFileAccess.ReadWrite)
            bytes.CopyTo stream
            mmf

        let accessor = mmf.CreateViewAccessor(0L, length, MemoryMappedFileAccess.ReadWrite)

        let safeHolder =
            { new obj() with
                override x.Finalize() =
                    (x :?> IDisposable).Dispose()
              interface IDisposable with
                member x.Dispose() =
                    GC.SuppressFinalize x
                    accessor.Dispose()
                    mmf.Dispose() }
        RawByteMemory.FromUnsafePointer(accessor.SafeMemoryMappedViewHandle.DangerousGetHandle(), int length, safeHolder)

    static member FromFile(path, access, ?canShadowCopy: bool) =
        let canShadowCopy = defaultArg canShadowCopy false

        let memoryMappedFileAccess =
            match access with
            | FileAccess.Read -> MemoryMappedFileAccess.Read
            | FileAccess.Write -> MemoryMappedFileAccess.Write
            | _ -> MemoryMappedFileAccess.ReadWrite

        let mmf, accessor, length = 
            let fileStream = File.Open(path, FileMode.Open, access, FileShare.Read)
            let length = fileStream.Length
            let mmf = 
                if canShadowCopy then
                    let mmf = 
                        MemoryMappedFile.CreateNew(
                            null, 
                            length, 
                            MemoryMappedFileAccess.ReadWrite, 
                            MemoryMappedFileOptions.None, 
                            HandleInheritability.None)
                    use stream = mmf.CreateViewStream(0L, length, MemoryMappedFileAccess.ReadWrite)
                    fileStream.CopyTo(stream)
                    fileStream.Dispose()
                    mmf
                else
                    MemoryMappedFile.CreateFromFile(
                        fileStream, 
                        null, 
                        length, 
                        memoryMappedFileAccess, 
                        HandleInheritability.None, 
                        leaveOpen=false)
            mmf, mmf.CreateViewAccessor(0L, length, memoryMappedFileAccess), length

        match access with
        | FileAccess.Read when not accessor.CanRead -> failwith "Cannot read file"
        | FileAccess.Write when not accessor.CanWrite -> failwith "Cannot write file"
        | _ when not accessor.CanRead || not accessor.CanWrite -> failwith "Cannot read or write file"
        | _ -> ()

        let safeHolder =
            { new obj() with
                override x.Finalize() =
                    (x :?> IDisposable).Dispose()
              interface IDisposable with
                member x.Dispose() =
                    GC.SuppressFinalize x
                    accessor.Dispose()
                    mmf.Dispose() }
        RawByteMemory.FromUnsafePointer(accessor.SafeMemoryMappedViewHandle.DangerousGetHandle(), int length, safeHolder)

    static member FromUnsafePointer(addr, length, hold: obj) = 
        RawByteMemory(NativePtr.ofNativeInt addr, length, hold) :> ByteMemory

    static member FromArray(bytes, offset, length) =
        ByteArrayMemory(bytes, offset, length) :> ByteMemory

    static member FromArray bytes =
        ByteArrayMemory.FromArray(bytes, 0, bytes.Length)

type internal ByteStream = 
    { bytes: ReadOnlyByteMemory
      mutable pos: int 
      max: int }
    member b.ReadByte() = 
        if b.pos >= b.max then failwith "end of stream"
        let res = b.bytes.[b.pos]
        b.pos <- b.pos + 1
        res 
    member b.ReadUtf8String n = 
        let res = b.bytes.ReadUtf8String(b.pos,n)  
        b.pos <- b.pos + n; res 
      
    static member FromBytes (b: ReadOnlyByteMemory,n,len) = 
        if n < 0 || (n+len) > b.Length then failwith "FromBytes"
        { bytes = b; pos = n; max = n+len }

    member b.ReadBytes n  = 
        if b.pos + n > b.max then failwith "ReadBytes: end of stream"
        let res = b.bytes.Slice(b.pos, n)
        b.pos <- b.pos + n
        res

    member b.Position = b.pos 
#if LAZY_UNPICKLE
    member b.CloneAndSeek = { bytes=b.bytes; pos=pos; max=b.max }
    member b.Skip = b.pos <- b.pos + n
#endif

[<RequireQualifiedAccess>]
module ByteBufferConstants =

    // From System.Reflection.Metadata.BlobBuilder as the DefaultChunkSize.
    [<Literal>]
    let MaxStartingCapacity = 256

    // From System.Reflection.Metadata.BlobBuilder
    // Reasoning was the smallest atomic data was a Guid.
    // Therefore, it should be a reasonable minimum value.
    [<Literal>]
    let MinChunkSize = 16

type internal ByteBuffer = 
    { mutable bbArray: ChunkedArrayBuilder<byte> 
      mutable bbCurrent: int }

    member buf.Reserve length = 
        buf.bbCurrent <- buf.bbCurrent + length
        buf.bbArray.Reserve length

    member buf.Close () = buf.bbArray.ToChunkedArray().ToArray()

    member buf.EmitIntAsByte (i:int) = 
        buf.EmitByte (byte i)

    member buf.EmitByte (b:byte) =
        (buf.bbArray.Reserve 1).WriteByte (0, b)
        buf.bbCurrent <- buf.bbCurrent + 1

    member buf.EmitIntsAsBytes (arr:int[]) = 
        let n = arr.Length
        for i = 0 to n - 1 do 
            buf.EmitByte (byte arr.[i])

    member buf.EmitInt32s (arr: int32[]) =
        let n = arr.Length
        for i = 0 to n - 1 do 
            buf.EmitInt32 arr.[i]

    member buf.EmitInt32 n = 
        (buf.bbArray.Reserve 4).WriteUInt32(0, uint32 n)
        buf.bbCurrent <- buf.bbCurrent + 4

    member buf.EmitBytes (i:byte[]) = 
        buf.bbArray.AddSpan(ReadOnlySpan i)
        buf.bbCurrent <- buf.bbCurrent + i.Length

    member buf.EmitByteMemory (bytes: ReadOnlyByteMemory) =
        bytes.CopyTo(buf.bbArray.Reserve bytes.Length)
        buf.bbCurrent <- buf.bbCurrent + bytes.Length

    member buf.EmitInt32AsUInt16 n = 
        (buf.bbArray.Reserve 2).WriteInt32AsUInt16 (0, n)
        buf.bbCurrent <- buf.bbCurrent + 2
    
    member buf.EmitBoolAsByte (b:bool) = buf.EmitIntAsByte (if b then 1 else 0)

    member buf.EmitUInt16 (x:uint16) = buf.EmitInt32AsUInt16 (int32 x)

    member buf.EmitInt64 x = 
        buf.EmitInt32 (Bytes.dWw0 x)
        buf.EmitInt32 (Bytes.dWw1 x)

    member buf.Position = buf.bbCurrent

    static member Create startingCapacity = 
        let startingCapacity =
            if startingCapacity > ByteBufferConstants.MaxStartingCapacity then
                ByteBufferConstants.MaxStartingCapacity
            else
                startingCapacity
        { bbArray = ChunkedArrayBuilder.Create(ByteBufferConstants.MinChunkSize, startingCapacity)  
          bbCurrent = 0 }


