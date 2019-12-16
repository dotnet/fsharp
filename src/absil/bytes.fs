// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Byte arrays
namespace FSharp.Compiler.AbstractIL.Internal

open System
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open FSharp.Compiler.AbstractIL.Internal.Library 

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

type internal ByteStream = 
    { bytes: byte[] 
      mutable pos: int 
      max: int }
    member b.ReadByte() = 
        if b.pos >= b.max then failwith "end of stream"
        let res = b.bytes.[b.pos]
        b.pos <- b.pos + 1
        res 
    member b.ReadUtf8String n = 
        let res = System.Text.Encoding.UTF8.GetString(b.bytes,b.pos,n)  
        b.pos <- b.pos + n; res 
      
    static member FromBytes (b:byte[],n,len) = 
        if n < 0 || (n+len) > b.Length then failwith "FromBytes"
        { bytes = b; pos = n; max = n+len }

    member b.ReadBytes n  = 
        if b.pos + n > b.max then failwith "ReadBytes: end of stream"
        let res = Bytes.sub b.bytes b.pos n
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
        buf.bbArray.Write(ReadOnlySpan i)
        buf.bbCurrent <- buf.bbCurrent + i.Length

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


