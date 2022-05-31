// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Linq

#nowarn "1204"

open System
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

module NullableOperators =
    let (?>=) (x: Nullable<'T>) (y: 'T) =
        x.HasValue && x.Value >= y

    let (?>) (x: Nullable<'T>) (y: 'T) =
        x.HasValue && x.Value > y

    let (?<=) (x: Nullable<'T>) (y: 'T) =
        x.HasValue && x.Value <= y

    let (?<) (x: Nullable<'T>) (y: 'T) =
        x.HasValue && x.Value < y

    let (?=) (x: Nullable<'T>) (y: 'T) =
        x.HasValue && x.Value = y

    let (?<>) (x: Nullable<'T>) (y: 'T) =
        not (x ?= y)

    let (>=?) (x: 'T) (y: Nullable<'T>) =
        y.HasValue && x >= y.Value

    let (>?) (x: 'T) (y: Nullable<'T>) =
        y.HasValue && x > y.Value

    let (<=?) (x: 'T) (y: Nullable<'T>) =
        y.HasValue && x <= y.Value

    let (<?) (x: 'T) (y: Nullable<'T>) =
        y.HasValue && x < y.Value

    let (=?) (x: 'T) (y: Nullable<'T>) =
        y.HasValue && x = y.Value

    let (<>?) (x: 'T) (y: Nullable<'T>) =
        not (x =? y)

    let (?>=?) (x: Nullable<'T>) (y: Nullable<'T>) =
        (x.HasValue && y.HasValue && x.Value >= y.Value)

    let (?>?) (x: Nullable<'T>) (y: Nullable<'T>) =
        (x.HasValue && y.HasValue && x.Value > y.Value)

    let (?<=?) (x: Nullable<'T>) (y: Nullable<'T>) =
        (x.HasValue && y.HasValue && x.Value <= y.Value)

    let (?<?) (x: Nullable<'T>) (y: Nullable<'T>) =
        (x.HasValue && y.HasValue && x.Value < y.Value)

    let (?=?) (x: Nullable<'T>) (y: Nullable<'T>) =
        (not x.HasValue && not y.HasValue)
        || (x.HasValue && y.HasValue && x.Value = y.Value)

    let (?<>?) (x: Nullable<'T>) (y: Nullable<'T>) =
        not (x ?=? y)

    let inline (?+) (x: Nullable<_>) y =
        if x.HasValue then Nullable(x.Value + y) else Nullable()

    let inline (+?) x (y: Nullable<_>) =
        if y.HasValue then Nullable(x + y.Value) else Nullable()

    let inline (?+?) (x: Nullable<_>) (y: Nullable<_>) =
        if x.HasValue && y.HasValue then
            Nullable(x.Value + y.Value)
        else
            Nullable()

    let inline (?-) (x: Nullable<_>) y =
        if x.HasValue then Nullable(x.Value - y) else Nullable()

    let inline (-?) x (y: Nullable<_>) =
        if y.HasValue then Nullable(x - y.Value) else Nullable()

    let inline (?-?) (x: Nullable<_>) (y: Nullable<_>) =
        if x.HasValue && y.HasValue then
            Nullable(x.Value - y.Value)
        else
            Nullable()

    let inline (?*) (x: Nullable<_>) y =
        if x.HasValue then Nullable(x.Value * y) else Nullable()

    let inline ( *? ) x (y: Nullable<_>) =
        if y.HasValue then Nullable(x * y.Value) else Nullable()

    let inline (?*?) (x: Nullable<_>) (y: Nullable<_>) =
        if x.HasValue && y.HasValue then
            Nullable(x.Value * y.Value)
        else
            Nullable()

    let inline (?%) (x: Nullable<_>) y =
        if x.HasValue then Nullable(x.Value % y) else Nullable()

    let inline (%?) x (y: Nullable<_>) =
        if y.HasValue then Nullable(x % y.Value) else Nullable()

    let inline (?%?) (x: Nullable<_>) (y: Nullable<_>) =
        if x.HasValue && y.HasValue then
            Nullable(x.Value % y.Value)
        else
            Nullable()

    let inline (?/) (x: Nullable<_>) y =
        if x.HasValue then Nullable(x.Value / y) else Nullable()

    let inline (/?) x (y: Nullable<_>) =
        if y.HasValue then Nullable(x / y.Value) else Nullable()

    let inline (?/?) (x: Nullable<_>) (y: Nullable<_>) =
        if x.HasValue && y.HasValue then
            Nullable(x.Value / y.Value)
        else
            Nullable()

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Nullable =
    [<CompiledName("ToUInt8")>]
    let inline uint8 (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.byte value.Value)
        else
            Nullable()

    [<CompiledName("ToInt8")>]
    let inline int8 (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.sbyte value.Value)
        else
            Nullable()

    [<CompiledName("ToByte")>]
    let inline byte (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.byte value.Value)
        else
            Nullable()

    [<CompiledName("ToSByte")>]
    let inline sbyte (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.sbyte value.Value)
        else
            Nullable()

    [<CompiledName("ToInt16")>]
    let inline int16 (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.int16 value.Value)
        else
            Nullable()

    [<CompiledName("ToUInt16")>]
    let inline uint16 (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.uint16 value.Value)
        else
            Nullable()

    [<CompiledName("ToInt")>]
    let inline int (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.int value.Value)
        else
            Nullable()

    [<CompiledName("ToUInt")>]
    let inline uint (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.uint value.Value)
        else
            Nullable()

    [<CompiledName("ToEnum")>]
    let inline enum (value: Nullable<int32>) =
        if value.HasValue then
            Nullable(Operators.enum value.Value)
        else
            Nullable()

    [<CompiledName("ToInt32")>]
    let inline int32 (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.int32 value.Value)
        else
            Nullable()

    [<CompiledName("ToUInt32")>]
    let inline uint32 (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.uint32 value.Value)
        else
            Nullable()

    [<CompiledName("ToInt64")>]
    let inline int64 (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.int64 value.Value)
        else
            Nullable()

    [<CompiledName("ToUInt64")>]
    let inline uint64 (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.uint64 value.Value)
        else
            Nullable()

    [<CompiledName("ToFloat32")>]
    let inline float32 (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.float32 value.Value)
        else
            Nullable()

    [<CompiledName("ToFloat")>]
    let inline float (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.float value.Value)
        else
            Nullable()

    [<CompiledName("ToSingle")>]
    let inline single (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.float32 value.Value)
        else
            Nullable()

    [<CompiledName("ToDouble")>]
    let inline double (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.float value.Value)
        else
            Nullable()

    [<CompiledName("ToIntPtr")>]
    let inline nativeint (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.nativeint value.Value)
        else
            Nullable()

    [<CompiledName("ToUIntPtr")>]
    let inline unativeint (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.unativeint value.Value)
        else
            Nullable()

    [<CompiledName("ToDecimal")>]
    let inline decimal (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.decimal value.Value)
        else
            Nullable()

    [<CompiledName("ToChar")>]
    let inline char (value: Nullable<_>) =
        if value.HasValue then
            Nullable(Operators.char value.Value)
        else
            Nullable()
