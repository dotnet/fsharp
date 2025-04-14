namespace Microsoft.FSharp.Core

open System.ComponentModel

[<Sealed; AbstractClass; EditorBrowsable(EditorBrowsableState.Never)>]
type Measure =
    static member inline Tag(value: byte) : byte<'Measure> =
        LanguagePrimitives.ByteWithMeasure value

    static member inline Tag(value: sbyte) : sbyte<'Measure> =
        LanguagePrimitives.SByteWithMeasure value

    static member inline Tag(value: int16) : int16<'Measure> =
        LanguagePrimitives.Int16WithMeasure value

    static member inline Tag(value: uint16) : uint16<'Measure> =
        LanguagePrimitives.UInt16WithMeasure value

    static member inline Tag(value: int) : int<'Measure> =
        LanguagePrimitives.Int32WithMeasure value

    static member inline Tag(value: uint) : uint<'Measure> =
        LanguagePrimitives.UInt32WithMeasure value

    static member inline Tag(value: int64) : int64<'Measure> =
        LanguagePrimitives.Int64WithMeasure value

    static member inline Tag(value: uint64) : uint64<'Measure> =
        LanguagePrimitives.UInt64WithMeasure value

    static member inline Tag(value: nativeint) : nativeint<'Measure> =
        LanguagePrimitives.IntPtrWithMeasure value

    static member inline Tag(value: unativeint) : unativeint<'Measure> =
        LanguagePrimitives.UIntPtrWithMeasure value

    static member inline Tag(value: float) : float<'Measure> =
        LanguagePrimitives.FloatWithMeasure value

    static member inline Tag(value: float32) : float32<'Measure> =
        LanguagePrimitives.Float32WithMeasure value

    static member inline Tag(value: decimal) : decimal<'Measure> =
        LanguagePrimitives.DecimalWithMeasure value

    static member inline InvokeTag value : '``T<'Measure>`` =
        let inline call_2 (_: ^a, b: ^b) =
            ((^a or ^b): (static member Tag: _ -> _) b)

        call_2 (Unchecked.defaultof<Measure>, value)

    static member inline Untag(value: byte<'Measure>) =
        byte value

    static member inline Untag(value: sbyte<'Measure>) =
        sbyte value

    static member inline Untag(value: int16<'Measure>) =
        int16 value

    static member inline Untag(value: uint16<'Measure>) =
        uint16 value

    static member inline Untag(value: int<'Measure>) =
        int value

    static member inline Untag(value: uint<'Measure>) =
        uint value

    static member inline Untag(value: int64<'Measure>) =
        int64 value

    static member inline Untag(value: uint64<'Measure>) =
        uint64 value

    static member inline Untag(value: nativeint<'Measure>) =
        nativeint value

    static member inline Untag(value: unativeint<'Measure>) =
        unativeint value

    static member inline Untag(value: float<'Measure>) =
        float value

    static member inline Untag(value: float32<'Measure>) =
        float32 value

    static member inline Untag(value: decimal<'Measure>) =
        decimal value

    static member inline InvokeUntag value : 'T =
        let inline call_2 (_: ^a, b: ^b) =
            ((^a or ^b): (static member Untag: _ -> _) b)

        call_2 (Unchecked.defaultof<Measure>, value)

[<RequireQualifiedAccess>]
module Measure =
    /// Tags a value with a unit of measure.
    let inline tag (value: 'T) : '``T<'Measure>`` =
        Measure.InvokeTag value

    /// Removes a unit of measure from a value.
    let inline untag (value: '``T<'Measure>``) : 'T =
        Measure.InvokeUntag value

    /// Tags a value with a new unit of measure.
    let inline retag (value: '``T<'Measure1>``) : '``T<'Measure2>`` =
        tag (untag value: 'T)

    /// Maps a value with a unit of measure to another value with a unit of measure.
    let inline map ([<InlineIfLambda>] mapping: 'T -> 'U) (value: '``T<'Measure1>``) : '``U<'Measure2>`` =
        tag (mapping (untag value))
