namespace Microsoft.FSharp.Core

open Microsoft.FSharp.Collections

[<AutoOpen>]
module private UnitsHelpers =
    let inline retype (x: 'T) : 'U =
        (# "" x: 'U #)

type Units =
    static member inline Add<[<Measure>] 'Measure>(input: byte) : byte<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: float) : float<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: int16) : int16<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: int) : int<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: int64) : int64<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: sbyte) : sbyte<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: nativeint) : nativeint<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: uint16) : uint16<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: uint) : uint<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: uint64) : uint64<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: decimal) : decimal<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: float32) : float32<'Measure> =
        retype input

    static member inline Add<[<Measure>] 'Measure>(input: unativeint) : unativeint<'Measure> =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: byte<'Measure>) : byte =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: float<'Measure>) : float =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: int16<'Measure>) : int16 =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: int<'Measure>) : int =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: int64<'Measure>) : int64 =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: sbyte<'Measure>) : sbyte =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: nativeint<'Measure>) : nativeint =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: uint16<'Measure>) : uint16 =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: uint<'Measure>) : uint =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: uint64<'Measure>) : uint64 =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: decimal<'Measure>) : decimal =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: float32<'Measure>) : float32 =
        retype input

    static member inline Remove<[<Measure>] 'Measure>(input: unativeint<'Measure>) : unativeint =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: byte<'MeasureIn>)
        : byte<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: float<'MeasureIn>)
        : float<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: int16<'MeasureIn>)
        : int16<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: int<'MeasureIn>)
        : int<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: int64<'MeasureIn>)
        : int64<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: sbyte<'MeasureIn>)
        : sbyte<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: nativeint<'MeasureIn>)
        : nativeint<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: uint16<'MeasureIn>)
        : uint16<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: uint<'MeasureIn>)
        : uint<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: uint64<'MeasureIn>)
        : uint64<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: decimal<'MeasureIn>)
        : decimal<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: float32<'MeasureIn>)
        : float32<'MeasureOut> =
        retype input

    static member inline Cast<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: unativeint<'MeasureIn>)
        : unativeint<'MeasureOut> =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: byte[]) : byte<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: float[]) : float<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: int16[]) : int16<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: int[]) : int<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: int64[]) : int64<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: sbyte[]) : sbyte<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: nativeint[]) : nativeint<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: uint16[]) : uint16<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: uint[]) : uint<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: uint64[]) : uint64<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: decimal[]) : decimal<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: float32[]) : float32<'Measure>[] =
        retype input

    static member inline AddArray<[<Measure>] 'Measure>(input: unativeint[]) : unativeint<'Measure>[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: byte<'Measure>[]) : byte[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: float<'Measure>[]) : float[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: int16<'Measure>[]) : int16[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: int<'Measure>[]) : int[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: int64<'Measure>[]) : int64[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: sbyte<'Measure>[]) : sbyte[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: nativeint<'Measure>[]) : nativeint[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: uint16<'Measure>[]) : uint16[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: uint<'Measure>[]) : uint[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: uint64<'Measure>[]) : uint64[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: decimal<'Measure>[]) : decimal[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: float32<'Measure>[]) : float32[] =
        retype input

    static member inline RemoveArray<[<Measure>] 'Measure>(input: unativeint<'Measure>[]) : unativeint[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: byte<'MeasureIn>[])
        : byte<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: float<'MeasureIn>[])
        : float<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: int16<'MeasureIn>[])
        : int16<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: int<'MeasureIn>[])
        : int<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: int64<'MeasureIn>[])
        : int64<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: sbyte<'MeasureIn>[])
        : sbyte<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: uint16<'MeasureIn>[])
        : uint16<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: uint<'MeasureIn>[])
        : uint<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: uint64<'MeasureIn>[])
        : uint64<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: decimal<'MeasureIn>[])
        : decimal<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: float32<'MeasureIn>[])
        : float32<'MeasureOut>[] =
        retype input

    static member inline CastArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: unativeint<'MeasureIn>[])
        : unativeint<'MeasureOut>[] =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>(input: ResizeArray<byte>) : ResizeArray<byte<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<float>)
        : ResizeArray<float<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<int16>)
        : ResizeArray<int16<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>(input: ResizeArray<int>) : ResizeArray<int<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<int64>)
        : ResizeArray<int64<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<sbyte>)
        : ResizeArray<sbyte<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<nativeint>)
        : ResizeArray<nativeint<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<uint16>)
        : ResizeArray<uint16<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>(input: ResizeArray<uint>) : ResizeArray<uint<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<uint64>)
        : ResizeArray<uint64<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<decimal>)
        : ResizeArray<decimal<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<float32>)
        : ResizeArray<float32<'Measure>> =
        retype input

    static member inline AddResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<unativeint>)
        : ResizeArray<unativeint<'Measure>> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<byte<'Measure>>)
        : ResizeArray<byte> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<float<'Measure>>)
        : ResizeArray<float> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<int16<'Measure>>)
        : ResizeArray<int16> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>(input: ResizeArray<int<'Measure>>) : ResizeArray<int> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<int64<'Measure>>)
        : ResizeArray<int64> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<sbyte<'Measure>>)
        : ResizeArray<sbyte> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<nativeint<'Measure>>)
        : ResizeArray<nativeint> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<uint16<'Measure>>)
        : ResizeArray<uint16> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<uint<'Measure>>)
        : ResizeArray<uint> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<uint64<'Measure>>)
        : ResizeArray<uint64> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<decimal<'Measure>>)
        : ResizeArray<decimal> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<float32<'Measure>>)
        : ResizeArray<float32> =
        retype input

    static member inline RemoveResizeArray<[<Measure>] 'Measure>
        (input: ResizeArray<unativeint<'Measure>>)
        : ResizeArray<unativeint> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<byte<'MeasureIn>>)
        : ResizeArray<byte<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<float<'MeasureIn>>)
        : ResizeArray<float<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<int16<'MeasureIn>>)
        : ResizeArray<int16<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<int<'MeasureIn>>)
        : ResizeArray<int<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<int64<'MeasureIn>>)
        : ResizeArray<int64<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<sbyte<'MeasureIn>>)
        : ResizeArray<sbyte<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<nativeint<'MeasureIn>>)
        : ResizeArray<nativeint<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<uint16<'MeasureIn>>)
        : ResizeArray<uint16<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<uint<'MeasureIn>>)
        : ResizeArray<uint<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<uint64<'MeasureIn>>)
        : ResizeArray<uint64<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<decimal<'MeasureIn>>)
        : ResizeArray<decimal<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<float32<'MeasureIn>>)
        : ResizeArray<float32<'MeasureOut>> =
        retype input

    static member inline CastResizeArray<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: ResizeArray<unativeint<'MeasureIn>>)
        : ResizeArray<unativeint<'MeasureOut>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<byte>) : list<byte<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<float>) : list<float<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<int16>) : list<int16<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<int>) : list<int<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<int64>) : list<int64<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<sbyte>) : list<sbyte<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<nativeint>) : list<nativeint<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<uint16>) : list<uint16<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<uint>) : list<uint<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<uint64>) : list<uint64<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<decimal>) : list<decimal<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<float32>) : list<float32<'Measure>> =
        retype input

    static member inline AddList<[<Measure>] 'Measure>(input: list<unativeint>) : list<unativeint<'Measure>> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<byte<'Measure>>) : list<byte> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<float<'Measure>>) : list<float> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<int16<'Measure>>) : list<int16> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<int<'Measure>>) : list<int> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<int64<'Measure>>) : list<int64> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<sbyte<'Measure>>) : list<sbyte> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<nativeint<'Measure>>) : list<nativeint> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<uint16<'Measure>>) : list<uint16> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<uint<'Measure>>) : list<uint> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<uint64<'Measure>>) : list<uint64> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<decimal<'Measure>>) : list<decimal> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<float32<'Measure>>) : list<float32> =
        retype input

    static member inline RemoveList<[<Measure>] 'Measure>(input: list<unativeint<'Measure>>) : list<unativeint> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<byte<'MeasureIn>>)
        : list<byte<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<float<'MeasureIn>>)
        : list<float<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<int16<'MeasureIn>>)
        : list<int16<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<int<'MeasureIn>>)
        : list<int<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<int64<'MeasureIn>>)
        : list<int64<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<sbyte<'MeasureIn>>)
        : list<sbyte<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<nativeint<'MeasureIn>>)
        : list<nativeint<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<uint16<'MeasureIn>>)
        : list<uint16<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<uint<'MeasureIn>>)
        : list<uint<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<uint64<'MeasureIn>>)
        : list<uint64<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<decimal<'MeasureIn>>)
        : list<decimal<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<float32<'MeasureIn>>)
        : list<float32<'MeasureOut>> =
        retype input

    static member inline CastList<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: list<unativeint<'MeasureIn>>)
        : list<unativeint<'MeasureOut>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<byte>) : seq<byte<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<float>) : seq<float<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<int16>) : seq<int16<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<int>) : seq<int<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<int64>) : seq<int64<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<sbyte>) : seq<sbyte<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<nativeint>) : seq<nativeint<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<uint16>) : seq<uint16<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<uint>) : seq<uint<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<uint64>) : seq<uint64<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<decimal>) : seq<decimal<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<float32>) : seq<float32<'Measure>> =
        retype input

    static member inline AddSeq<[<Measure>] 'Measure>(input: seq<unativeint>) : seq<unativeint<'Measure>> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<byte<'Measure>>) : seq<byte> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<float<'Measure>>) : seq<float> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<int16<'Measure>>) : seq<int16> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<int<'Measure>>) : seq<int> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<int64<'Measure>>) : seq<int64> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<sbyte<'Measure>>) : seq<sbyte> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<nativeint<'Measure>>) : seq<nativeint> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<uint16<'Measure>>) : seq<uint16> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<uint<'Measure>>) : seq<uint> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<uint64<'Measure>>) : seq<uint64> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<decimal<'Measure>>) : seq<decimal> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<float32<'Measure>>) : seq<float32> =
        retype input

    static member inline RemoveSeq<[<Measure>] 'Measure>(input: seq<unativeint<'Measure>>) : seq<unativeint> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<byte<'MeasureIn>>)
        : seq<byte<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<float<'MeasureIn>>)
        : seq<float<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<int16<'MeasureIn>>)
        : seq<int16<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<int<'MeasureIn>>)
        : seq<int<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<int64<'MeasureIn>>)
        : seq<int64<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<sbyte<'MeasureIn>>)
        : seq<sbyte<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<nativeint<'MeasureIn>>)
        : seq<nativeint<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<uint16<'MeasureIn>>)
        : seq<uint16<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<uint<'MeasureIn>>)
        : seq<uint<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<uint64<'MeasureIn>>)
        : seq<uint64<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<decimal<'MeasureIn>>)
        : seq<decimal<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<float32<'MeasureIn>>)
        : seq<float32<'MeasureOut>> =
        retype input

    static member inline CastSeq<[<Measure>] 'MeasureIn, [<Measure>] 'MeasureOut>
        (input: seq<unativeint<'MeasureIn>>)
        : seq<unativeint<'MeasureOut>> =
        retype input
