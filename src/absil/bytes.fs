// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Byte arrays
namespace FSharp.Compiler.AbstractIL.Internal

open System
open System.IO
open System.IO.MemoryMappedFiles
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop

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

[<AbstractClass>]
type ByteMemory () =

    abstract Item: int -> byte with get, set

    abstract Length: int

    abstract GetBytes: pos: int * count: int -> byte[]

    abstract GetInt32: pos: int -> int

    abstract GetUInt16: pos: int -> uint16

    abstract GetUtf8String: pos: int * count: int -> string

    abstract Slice: pos: int * count: int -> ByteMemory

    abstract CopyTo: Stream -> unit

    abstract Copy: srcOffset: int * dest: byte[] * destOffset: int * count: int -> unit

    abstract ToArray: unit -> byte[]

    abstract AsStream: unit -> Stream

[<Sealed>]
type ByteArrayMemory(bytes: byte[], offset, length) =
    inherit ByteMemory()

    do
        if offset < 0 then
            failwith "offset is less than zero"

        if length <= 0 then
            failwith "length is less than or equal to zero"

        if (offset + length) > bytes.Length then
            failwith "span larger than byte array"
    
    override _.Item 
        with get i = 
            bytes.[offset + i]
        and set i v =
            bytes.[offset + i] <- v

    override _.Length = length

    override _.GetBytes(pos, count) = 
        Array.sub bytes (offset + pos) count

    override _.GetInt32 pos =
        BitConverter.ToInt32(bytes, pos)

    override _.GetUInt16 pos =
        BitConverter.ToUInt16(bytes, pos)

    override _.GetUtf8String(pos, count) =
        System.Text.Encoding.UTF8.GetString(bytes, pos, count)

    override _.Slice(pos, count) =
        ByteArrayMemory(bytes, offset + pos, count) :> ByteMemory

    override _.CopyTo stream =
        stream.Write(bytes, offset, length)

    override _.Copy(srcOffset, dest, destOffset, count) =
        Array.blit bytes (offset + srcOffset) dest destOffset count

    override _.ToArray() =
        Array.sub bytes offset length

    override _.AsStream() =
        new MemoryStream(bytes, offset, length) :> Stream

[<Sealed>]
type RawByteMemory(addr: nativeptr<byte>, length: int, hold: obj) =
    inherit ByteMemory ()

    let check i =
        if i < 0 || i >= length then 
            failwith "out of range"

    do
        if length <= 0 then
            failwith "length is less than or equal to zero"

    override _.Item 
        with get i = 
            check i
            NativePtr.add addr i
            |> NativePtr.read 
        and set i v =
            check i
            NativePtr.set addr i v

    override _.Length = length

    override _.GetUtf8String(pos, count) =
        check pos
        check (pos + count - 1)
        System.Text.Encoding.UTF8.GetString(NativePtr.add addr pos, count)

    override _.GetBytes(pos, count) = 
        check pos
        check (pos + count - 1)
        let res = Bytes.zeroCreate count
        Marshal.Copy(NativePtr.toNativeInt addr + nativeint pos, res, 0, count)
        res

    override _.GetInt32 pos =
        check pos
        check (pos + 3)
        Marshal.ReadInt32(NativePtr.toNativeInt addr + nativeint pos)

    override _.GetUInt16 pos =
        check pos
        check (pos + 1)
        uint16(Marshal.ReadInt16(NativePtr.toNativeInt addr + nativeint pos))

    override _.Slice(pos, count) =
        check pos
        check (pos + count - 1)
        RawByteMemory(NativePtr.add addr pos, count, hold) :> ByteMemory

    override x.CopyTo stream =
        use stream2 = x.AsStream()
        stream2.CopyTo stream

    override x.Copy(srcOffset, dest, destOffset, count) =
        check srcOffset
        Marshal.Copy(NativePtr.toNativeInt addr + nativeint srcOffset, dest, destOffset, count)

    override _.ToArray() =
        let res = Array.zeroCreate<byte> length
        Marshal.Copy(NativePtr.toNativeInt addr, res, 0, res.Length)
        res

    override _.AsStream() =
        new UnmanagedMemoryStream(addr, int64 length) :> Stream

[<Struct;NoEquality;NoComparison>]
type ReadOnlyByteMemory(bytes: ByteMemory) =

    member _.Item with [<MethodImpl(MethodImplOptions.AggressiveInlining)>] get i = bytes.[i]

    member _.Length with [<MethodImpl(MethodImplOptions.AggressiveInlining)>] get () = bytes.Length

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.GetBytes(pos, count) = bytes.GetBytes(pos, count)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.GetInt32 pos = bytes.GetInt32 pos

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.GetUInt16 pos = bytes.GetUInt16 pos

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.GetUtf8String(pos, count) = bytes.GetUtf8String(pos, count)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Slice(pos, count) = bytes.Slice(pos, count) |> ReadOnlyByteMemory

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.ToArray() = bytes.ToArray()

type ByteMemory with

    static member CreateMemoryMappedFile(bytes: ByteMemory) =
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
        let res = b.bytes.GetUtf8String(b.pos,n)  
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


type internal ByteBuffer = 
    { mutable bbArray: byte[] 
      mutable bbCurrent: int }

    member buf.Ensure newSize = 
        let oldBufSize = buf.bbArray.Length 
        if newSize > oldBufSize then 
            let old = buf.bbArray 
            buf.bbArray <- Bytes.zeroCreate (max newSize (oldBufSize * 2))
            Bytes.blit old 0 buf.bbArray 0 buf.bbCurrent

    member buf.Close () = Bytes.sub buf.bbArray 0 buf.bbCurrent

    member buf.EmitIntAsByte (i:int) = 
        let newSize = buf.bbCurrent + 1 
        buf.Ensure newSize
        buf.bbArray.[buf.bbCurrent] <- byte i
        buf.bbCurrent <- newSize 

    member buf.EmitByte (b:byte) = buf.EmitIntAsByte (int b)

    member buf.EmitIntsAsBytes (arr:int[]) = 
        let n = arr.Length
        let newSize = buf.bbCurrent + n 
        buf.Ensure newSize
        let bbArr = buf.bbArray
        let bbBase = buf.bbCurrent
        for i = 0 to n - 1 do 
            bbArr.[bbBase + i] <- byte arr.[i] 
        buf.bbCurrent <- newSize 

    member bb.FixupInt32 pos n = 
        bb.bbArray.[pos] <- (Bytes.b0 n |> byte)
        bb.bbArray.[pos + 1] <- (Bytes.b1 n |> byte)
        bb.bbArray.[pos + 2] <- (Bytes.b2 n |> byte)
        bb.bbArray.[pos + 3] <- (Bytes.b3 n |> byte)

    member buf.EmitInt32 n = 
        let newSize = buf.bbCurrent + 4 
        buf.Ensure newSize
        buf.FixupInt32 buf.bbCurrent n
        buf.bbCurrent <- newSize 

    member buf.EmitBytes (i:byte[]) = 
        let n = i.Length 
        let newSize = buf.bbCurrent + n 
        buf.Ensure newSize
        Bytes.blit i 0 buf.bbArray buf.bbCurrent n
        buf.bbCurrent <- newSize 

    member buf.EmitMemory (i:ByteMemory) = 
        let n = i.Length 
        let newSize = buf.bbCurrent + n 
        buf.Ensure newSize
        i.Copy(0, buf.bbArray, buf.bbCurrent, n)
        buf.bbCurrent <- newSize 

    member buf.EmitInt32AsUInt16 n = 
        let newSize = buf.bbCurrent + 2 
        buf.Ensure newSize
        buf.bbArray.[buf.bbCurrent] <- (Bytes.b0 n |> byte)
        buf.bbArray.[buf.bbCurrent + 1] <- (Bytes.b1 n |> byte)
        buf.bbCurrent <- newSize 
    
    member buf.EmitBoolAsByte (b:bool) = buf.EmitIntAsByte (if b then 1 else 0)

    member buf.EmitUInt16 (x:uint16) = buf.EmitInt32AsUInt16 (int32 x)

    member buf.EmitInt64 x = 
        buf.EmitInt32 (Bytes.dWw0 x)
        buf.EmitInt32 (Bytes.dWw1 x)

    member buf.Position = buf.bbCurrent

    static member Create sz = 
        { bbArray=Bytes.zeroCreate sz 
          bbCurrent = 0 }


