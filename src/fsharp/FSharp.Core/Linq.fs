// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#nowarn "1204"

namespace Microsoft.FSharp.Linq


open Microsoft.FSharp
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

module NullableOperators =
    open System
    let (?>=) (x : Nullable<'T>) (y: 'T) = x.HasValue && x.Value >= y

    let (?>) (x : Nullable<'T>) (y: 'T) = x.HasValue && x.Value > y

    let (?<=) (x : Nullable<'T>) (y: 'T) = x.HasValue && x.Value <= y

    let (?<) (x : Nullable<'T>) (y: 'T) = x.HasValue && x.Value < y

    let (?=) (x : Nullable<'T>) (y: 'T) = x.HasValue && x.Value = y

    let (?<>) (x : Nullable<'T>) (y: 'T) = not (x ?= y)

    let (>=?) (x : 'T) (y: Nullable<'T>) = y.HasValue && x >= y.Value

    let (>?) (x : 'T) (y: Nullable<'T>) = y.HasValue && x > y.Value

    let (<=?) (x : 'T) (y: Nullable<'T>) = y.HasValue && x <= y.Value

    let (<?) (x : 'T) (y: Nullable<'T>) = y.HasValue && x < y.Value

    let (=?) (x : 'T) (y: Nullable<'T>) = y.HasValue && x = y.Value

    let (<>?) (x : 'T) (y: Nullable<'T>) = not (x =? y)

    let (?>=?) (x : Nullable<'T>) (y: Nullable<'T>) = (x.HasValue && y.HasValue && x.Value >= y.Value)

    let (?>?) (x : Nullable<'T>) (y: Nullable<'T>) = (x.HasValue && y.HasValue && x.Value > y.Value)

    let (?<=?) (x : Nullable<'T>) (y: Nullable<'T>) = (x.HasValue && y.HasValue && x.Value <= y.Value)

    let (?<?) (x : Nullable<'T>) (y: Nullable<'T>) = (x.HasValue && y.HasValue && x.Value < y.Value)

    let (?=?) (x : Nullable<'T>) (y: Nullable<'T>) = (not x.HasValue && not y.HasValue) || (x.HasValue && y.HasValue && x.Value = y.Value)

    let (?<>?) (x : Nullable<'T>) (y: Nullable<'T>) = not (x ?=? y)

    let inline (?+) (x : Nullable<_>) y = if x.HasValue then Nullable(x.Value + y) else Nullable()

    let inline (+?) x (y: Nullable<_>) = if y.HasValue then Nullable(x + y.Value) else Nullable()

    let inline (?+?) (x : Nullable<_>) (y: Nullable<_>) = if x.HasValue && y.HasValue then Nullable(x.Value + y.Value) else Nullable()

    let inline (?-) (x : Nullable<_>) y = if x.HasValue then Nullable(x.Value - y) else Nullable()

    let inline (-?) x (y: Nullable<_>) = if y.HasValue then Nullable(x - y.Value) else Nullable()

    let inline (?-?) (x : Nullable<_>) (y: Nullable<_>) = if x.HasValue && y.HasValue then Nullable(x.Value - y.Value) else Nullable()

    let inline ( ?*  ) (x : Nullable<_>) y = if x.HasValue then Nullable(x.Value * y) else Nullable()

    let inline ( *?  ) x (y: Nullable<_>) = if y.HasValue then Nullable(x * y.Value) else Nullable()

    let inline ( ?*? ) (x : Nullable<_>) (y: Nullable<_>) = if x.HasValue && y.HasValue then Nullable(x.Value * y.Value) else Nullable()

    let inline ( ?%  ) (x : Nullable<_>) y = if x.HasValue then Nullable(x.Value % y) else Nullable()

    let inline ( %?  ) x (y: Nullable<_>) = if y.HasValue then Nullable(x % y.Value) else Nullable()

    let inline ( ?%? ) (x : Nullable<_>) (y: Nullable<_>) = if x.HasValue && y.HasValue then Nullable(x.Value % y.Value) else Nullable()

    let inline ( ?/  ) (x : Nullable<_>) y = if x.HasValue then Nullable(x.Value / y) else Nullable()

    let inline ( /?  ) x (y: Nullable<_>) = if y.HasValue then Nullable(x / y.Value) else Nullable()

    let inline ( ?/? ) (x : Nullable<_>) (y: Nullable<_>) = if x.HasValue && y.HasValue then Nullable(x.Value / y.Value) else Nullable()

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Nullable =

    open System

    [<CompiledName("ToUInt8")>]
    let inline uint8 (value:Nullable<_>) = if value.HasValue then Nullable(Operators.byte value.Value) else Nullable()

    [<CompiledName("ToInt8")>]
    let inline int8 (value:Nullable<_>) = if value.HasValue then Nullable(Operators.sbyte value.Value) else Nullable()

    [<CompiledName("ToByte")>]
    let inline byte (value:Nullable<_>) = if value.HasValue then Nullable(Operators.byte value.Value) else Nullable()

    [<CompiledName("ToSByte")>]
    let inline sbyte (value:Nullable<_>) = if value.HasValue then Nullable(Operators.sbyte value.Value) else Nullable()

    [<CompiledName("ToInt16")>]
    let inline int16 (value:Nullable<_>) = if value.HasValue then Nullable(Operators.int16 value.Value) else Nullable()

    [<CompiledName("ToUInt16")>]
    let inline uint16 (value:Nullable<_>) = if value.HasValue then Nullable(Operators.uint16 value.Value) else Nullable()

    [<CompiledName("ToInt")>]
    let inline int (value:Nullable<_>) = if value.HasValue then Nullable(Operators.int value.Value) else Nullable()

    [<CompiledName("ToEnum")>]
    let inline enum (value:Nullable< int32 >) = if value.HasValue then Nullable(Operators.enum value.Value) else Nullable()

    [<CompiledName("ToInt32")>]
    let inline int32 (value:Nullable<_>) = if value.HasValue then Nullable(Operators.int32 value.Value) else Nullable()

    [<CompiledName("ToUInt32")>]
    let inline uint32 (value:Nullable<_>) = if value.HasValue then Nullable(Operators.uint32 value.Value) else Nullable()

    [<CompiledName("ToInt64")>]
    let inline int64 (value:Nullable<_>) = if value.HasValue then Nullable(Operators.int64 value.Value) else Nullable()

    [<CompiledName("ToUInt64")>]
    let inline uint64 (value:Nullable<_>) = if value.HasValue then Nullable(Operators.uint64 value.Value) else Nullable()

    [<CompiledName("ToFloat32")>]
    let inline float32 (value:Nullable<_>) = if value.HasValue then Nullable(Operators.float32 value.Value) else Nullable()

    [<CompiledName("ToFloat")>]
    let inline float (value:Nullable<_>) = if value.HasValue then Nullable(Operators.float value.Value) else Nullable()

    [<CompiledName("ToSingle")>]
    let inline single (value:Nullable<_>) = if value.HasValue then Nullable(Operators.float32 value.Value) else Nullable()

    [<CompiledName("ToDouble")>]
    let inline double (value:Nullable<_>) = if value.HasValue then Nullable(Operators.float value.Value) else Nullable()

    [<CompiledName("ToIntPtr")>]
    let inline nativeint (value:Nullable<_>) = if value.HasValue then Nullable(Operators.nativeint value.Value) else Nullable()

    [<CompiledName("ToUIntPtr")>]
    let inline unativeint (value:Nullable<_>) = if value.HasValue then Nullable(Operators.unativeint value.Value) else Nullable()

    [<CompiledName("ToDecimal")>]
    let inline decimal (value:Nullable<_>) = if value.HasValue then Nullable(Operators.decimal value.Value) else Nullable()

    [<CompiledName("ToChar")>]
    let inline char (value:Nullable<_>) = if value.HasValue then Nullable(Operators.char value.Value) else Nullable()

namespace Microsoft.FSharp.Linq.RuntimeHelpers

open System
open System.Collections.Generic
open System.Linq.Expressions
open System.Reflection
open Microsoft.FSharp
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core
open Microsoft.FSharp.Linq
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

#if FX_RESHAPED_REFLECTION
open PrimReflectionAdapters
open ReflectionAdapters
#endif

module LeafExpressionConverter =

    // The following is recognized as a LINQ 'member initialization pattern' in a quotation.
    let MemberInitializationHelper (_x:'T) : 'T =  raise (NotSupportedException "This function should not be called directly")

    // The following is recognized as a LINQ 'member initialization pattern' in a quotation.
    let NewAnonymousObjectHelper (_x:'T) : 'T =  raise (NotSupportedException "This function should not be called directly")

    // This is used to mark expressions inserted to satisfy C#'s design where, inside C#-compiler generated
    // LINQ expressions, they pass an argument or type T to an argument expecting Expression<T>.
    let ImplicitExpressionConversionHelper (_x:'T) : Expression<'T> = raise (NotSupportedException "This function should not be called directly")

    [<NoEquality; NoComparison>]
    type ConvEnv =
        {   varEnv : Map<Var, Expression> }
    let asExpr x = (x :> Expression)

    let bindingFlags = BindingFlags.Public ||| BindingFlags.NonPublic
    let instanceBindingFlags = BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.DeclaredOnly

    let isNamedType(typ:Type) = not (typ.IsArray || typ.IsByRef || typ.IsPointer)

    let equivHeadTypes (ty1:Type) (ty2:Type) =
        isNamedType(ty1) &&
        if ty1.IsGenericType then
            ty2.IsGenericType && (ty1.GetGenericTypeDefinition()).Equals(ty2.GetGenericTypeDefinition())
        else
            ty1.Equals(ty2)

    let isFunctionType typ = equivHeadTypes typ (typeof<(int -> int)>)

    let getFunctionType typ =
        if not (isFunctionType typ) then invalidArg "typ" "cannot convert recursion except for function types"
        let tyargs = typ.GetGenericArguments()
        tyargs.[0], tyargs.[1]

    let GetGenericMethodDefinition (methInfo:MethodInfo) =
        if methInfo.IsGenericMethod then methInfo.GetGenericMethodDefinition() else methInfo

    let StringConcat =
       methodhandleof (fun (x:obj, y:obj) -> String.Concat (x, y))
       |> System.Reflection.MethodInfo.GetMethodFromHandle
       :?> MethodInfo

    let SubstHelperRaw (q:Expr, x:Var[], y:obj[]) : Expr =
        let d = Map.ofArray (Array.zip x y)
        q.Substitute(fun v -> v |> d.TryFind |> Option.map (fun x -> Expr.Value (x, v.Type)))

    let SubstHelper<'T> (q:Expr, x:Var[], y:obj[]) : Expr<'T> =
        SubstHelperRaw(q, x, y) |> Expr.Cast

    let showAll =
#if FX_RESHAPED_REFLECTION
        true
#else
        BindingFlags.Public ||| BindingFlags.NonPublic
#endif

    let NullableConstructor =
        typedefof<Nullable<int>>.GetConstructors().[0]

    let SpecificCallToMethodInfo (minfo: System.Reflection.MethodInfo) =
        let isg1 = minfo.IsGenericMethod
        let gmd = if isg1 then minfo.GetGenericMethodDefinition() else null
        (fun tm ->
            match tm with
            | Call(obj, minfo2, args)
                when (
#if !FX_NO_REFLECTION_METADATA_TOKENS
                        minfo.MetadataToken = minfo2.MetadataToken &&
#endif
                        if isg1 then minfo2.IsGenericMethod && gmd = minfo2.GetGenericMethodDefinition()
                        else minfo = minfo2
                     ) ->
                Some (obj, (minfo2.GetGenericArguments() |> Array.toList), args)
            | _ -> None)


    let (|SpecificCallToMethod|_|) (mhandle: System.RuntimeMethodHandle) =
        let minfo = (System.Reflection.MethodInfo.GetMethodFromHandle mhandle) :?> MethodInfo
        SpecificCallToMethodInfo minfo

    let (|GenericEqualityQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> LanguagePrimitives.GenericEquality x y))
    let (|EqualsQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x = y))
    let (|GreaterQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x > y))
    let (|GreaterEqQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x >= y))
    let (|LessQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x <  y))
    let (|LessEqQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x <= y))
    let (|NotEqQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x <> y))

    let (|StaticEqualsQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x:int, y:int) -> NonStructuralComparison.(=) x y))
    let (|StaticGreaterQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x:int, y:int) -> NonStructuralComparison.(>) x y))
    let (|StaticGreaterEqQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x:int, y:int) -> NonStructuralComparison.(>=) x y))
    let (|StaticLessQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x:int, y:int) -> NonStructuralComparison.(<) x y))
    let (|StaticLessEqQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x:int, y:int) -> NonStructuralComparison.(<=) x y))
    let (|StaticNotEqQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x:int, y:int) -> NonStructuralComparison.(<>) x y))

    let (|NullableEqualsQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?=  ) x y))
    let (|NullableNotEqQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?<> ) x y))
    let (|NullableGreaterQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?>  ) x y))
    let (|NullableGreaterEqQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?>= ) x y))
    let (|NullableLessQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?<  ) x y))
    let (|NullableLessEqQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?<= ) x y))

    let (|NullableEqualsNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?=?  ) x y))
    let (|NullableNotEqNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?<>? ) x y))
    let (|NullableGreaterNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?>?  ) x y))
    let (|NullableGreaterEqNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?>=? ) x y))
    let (|NullableLessNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?<?  ) x y))
    let (|NullableLessEqNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?<=? ) x y))

    let (|EqualsNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( =?  ) x y))
    let (|NotEqNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( <>? ) x y))
    let (|GreaterNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( >?  ) x y))
    let (|GreaterEqNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( >=? ) x y))
    let (|LessNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( <?  ) x y))
    let (|LessEqNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( <=? ) x y))

    let (|MakeDecimalQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (a1, a2, a3, a4, a5) -> LanguagePrimitives.IntrinsicFunctions.MakeDecimal a1 a2 a3 a4 a5))

    let (|NullablePlusQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?+ ) x y))
    let (|NullablePlusNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?+? ) x y))
    let (|PlusNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( +? ) x y))

    let (|NullableMinusQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?- ) x y))
    let (|NullableMinusNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?-? ) x y))
    let (|MinusNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( -? ) x y))

    let (|NullableMultiplyQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?* ) x y))
    let (|NullableMultiplyNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?*? ) x y))
    let (|MultiplyNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( *? ) x y))

    let (|NullableDivideQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?/ ) x y))
    let (|NullableDivideNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?/? ) x y))
    let (|DivideNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( /? ) x y))

    let (|NullableModuloQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?% ) x y))
    let (|NullableModuloNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( ?%? ) x y))
    let (|ModuloNullableQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> NullableOperators.( %? ) x y))

    let (|NotQ|_|) =  (|SpecificCallToMethod|_|) (methodhandleof (fun x -> not x))
    let (|NegQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x:int) -> -x))
    let (|PlusQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x + y))
    let (|DivideQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x / y))
    let (|MinusQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x - y))
    let (|MultiplyQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x * y))
    let (|ModuloQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x % y))
    let (|ShiftLeftQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x <<< y))
    let (|ShiftRightQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x >>> y))
    let (|BitwiseAndQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x &&& y))
    let (|BitwiseOrQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x ||| y))
    let (|BitwiseXorQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> x ^^^ y))
    let (|BitwiseNotQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> ~~~ x))
    let (|CheckedNeg|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.( ~-) x))
    let (|CheckedPlusQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> Checked.( + ) x y))
    let (|CheckedMinusQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> Checked.( - ) x y))
    let (|CheckedMultiplyQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> Checked.( * ) x y))

    let (|ConvCharQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.char x))
    let (|ConvDecimalQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.decimal x))
    let (|ConvFloatQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.float x))
    let (|ConvFloat32Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.float32 x))
    let (|ConvSByteQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.sbyte x))

    let (|ConvInt16Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.int16 x))
    let (|ConvInt32Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.int32 x))
    let (|ConvIntQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.int x))
    let (|ConvInt64Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.int64 x))
    let (|ConvByteQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.byte x))
    let (|ConvUInt16Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.uint16 x))
    let (|ConvUInt32Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.uint32 x))
    let (|ConvUInt64Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Operators.uint64 x))

    let (|ConvInt8Q|_|) = SpecificCallToMethodInfo (typeof<ConvEnv>.Assembly.GetType("Microsoft.FSharp.Core.ExtraTopLevelOperators").GetMethod("ToSByte"))
    let (|ConvUInt8Q|_|) = SpecificCallToMethodInfo (typeof<ConvEnv>.Assembly.GetType("Microsoft.FSharp.Core.ExtraTopLevelOperators").GetMethod("ToByte"))
    let (|ConvDoubleQ|_|) = SpecificCallToMethodInfo (typeof<ConvEnv>.Assembly.GetType("Microsoft.FSharp.Core.ExtraTopLevelOperators").GetMethod("ToDouble"))
    let (|ConvSingleQ|_|) = SpecificCallToMethodInfo (typeof<ConvEnv>.Assembly.GetType("Microsoft.FSharp.Core.ExtraTopLevelOperators").GetMethod("ToSingle"))

    let (|ConvNullableCharQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.char x))
    let (|ConvNullableDecimalQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.decimal x))
    let (|ConvNullableFloatQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.float x))
    let (|ConvNullableDoubleQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.double x))
    let (|ConvNullableFloat32Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.float32 x))
    let (|ConvNullableSingleQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.single x))
    let (|ConvNullableSByteQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.sbyte x))
    let (|ConvNullableInt8Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.int8 x))
    let (|ConvNullableInt16Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.int16 x))
    let (|ConvNullableInt32Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.int32 x))
    let (|ConvNullableIntQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.int x))
    let (|ConvNullableInt64Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.int64 x))
    let (|ConvNullableByteQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.byte x))
    let (|ConvNullableUInt8Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.uint8 x))
    let (|ConvNullableUInt16Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.uint16 x))
    let (|ConvNullableUInt32Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.uint32 x))
    let (|ConvNullableUInt64Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.uint64 x))
    // LINQ expressions can't do native integer operations, so we don't convert these
    //let (|ConvNullableIntPtrQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.nativeint x))
    //let (|ConvNullableUIntPtrQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Nullable.unativeint x))


    let (|UnboxGeneric|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> LanguagePrimitives.IntrinsicFunctions.UnboxGeneric x))
    let (|TypeTestGeneric|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> LanguagePrimitives.IntrinsicFunctions.TypeTestGeneric x))
    let (|CheckedConvCharQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.char x))
    let (|CheckedConvSByteQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.sbyte x))
    let (|CheckedConvInt8Q|_|) = SpecificCallToMethodInfo (typeof<ConvEnv>.Assembly.GetType("Microsoft.FSharp.Core.ExtraTopLevelOperators+Checked").GetMethod("ToSByte"))
    let (|CheckedConvUInt8Q|_|) = SpecificCallToMethodInfo (typeof<ConvEnv>.Assembly.GetType("Microsoft.FSharp.Core.ExtraTopLevelOperators+Checked").GetMethod("ToByte"))
    let (|CheckedConvInt16Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.int16 x))
    let (|CheckedConvInt32Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.int32 x))
    let (|CheckedConvInt64Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.int64 x))
    let (|CheckedConvByteQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.byte x))
    let (|CheckedConvUInt16Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.uint16 x))
    let (|CheckedConvUInt32Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.uint32 x))
    let (|CheckedConvUInt64Q|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> Checked.uint64 x))
    let (|ImplicitExpressionConversionHelperQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> ImplicitExpressionConversionHelper x))
    let (|MemberInitializationHelperQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> MemberInitializationHelper x))
    let (|NewAnonymousObjectHelperQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun x -> NewAnonymousObjectHelper x))
    let (|ArrayLookupQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun (x, y) -> LanguagePrimitives.IntrinsicFunctions.GetArray x y))

    //let (|ArrayAssignQ|_|) = (|SpecificCallToMethod|_|) (methodhandleof (fun -> LanguagePrimitives.IntrinsicFunctions.SetArray : int[] -> int -> int -> unit))
    //let (|ArrayTypeQ|_|) (ty:System.Type) = if ty.IsArray && ty.GetArrayRank() = 1 then Some (ty.GetElementType()) else None
    let substHelperMeth =
        methodhandleof (fun (x:Expr, y:Var[], z:obj[]) -> SubstHelper<obj> (x, y, z))
        |> System.Reflection.MethodInfo.GetMethodFromHandle
        :?> MethodInfo

    let substHelperRawMeth =
        methodhandleof (fun (x:Expr, y:Var[], z:obj[]) -> SubstHelperRaw (x, y, z))
        |> System.Reflection.MethodInfo.GetMethodFromHandle
        :?> MethodInfo

    let (-->) ty1 ty2 = Reflection.FSharpType.MakeFunctionType(ty1, ty2)

    /// Extract member initialization expression stored in 'MemberInitializationHelper' (by QueryExtensions.fs)
    let rec (|Sequentials|) = function
        | Patterns.Sequential(a, Sequentials (b, c)) -> (a :: b, c)
        | a -> [], a

    let (|MemberInitializationQ|_|) = function
        | MemberInitializationHelperQ (None, _, [  Sequentials (propSets, init) ]) -> Some (init, propSets)
        | _ -> None

    /// Extract construction of anonymous object noted by use of in 'NewAnonymousObjectHelper' (by QueryExtensions.fs)
    let (|NewAnonymousObjectQ|_|) = function
        | NewAnonymousObjectHelperQ (None, _, [ Patterns.NewObject(ctor, args) ]) -> Some (ctor, args)
        | _ -> None

    /// Extract nullable constructions
    let (|NullableConstruction|_|) = function
      | NewObject(c, [arg]) when equivHeadTypes c.DeclaringType (typeof<Nullable<int>>) -> Some arg
      | _ -> None

    /// Convert F# quotations to LINQ expression trees.
    /// A more polished LINQ-Quotation translator will be published
    /// concert with later versions of LINQ.
    let rec ConvExprToLinqInContext (env:ConvEnv) (inp:Expr) =
        //printf "ConvExprToLinqInContext : %A\n" inp
        match inp with

        // Generic cases
        | Patterns.Var v ->
            try
                Map.find v env.varEnv
            with
            |   :? KeyNotFoundException -> invalidOp ("The variable '"+ v.Name + "' was not found in the translation context'")

        | DerivedPatterns.AndAlso(x1, x2) ->
            Expression.AndAlso(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr

        | DerivedPatterns.OrElse(x1, x2) ->
            Expression.OrElse(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr

        | Patterns.Value(x, ty) ->
            Expression.Constant(x, ty) |> asExpr

        | UnboxGeneric(_, [toTy], [x])
        | Patterns.Coerce(x, toTy) ->
            let converted = ConvExprToLinqInContext env x

            // Most of conversion scenarios in C# are covered by Expression.Convert
            if x.Type.Equals toTy then converted // source and target types match - do nothing
            elif not (x.Type.IsValueType || toTy.IsValueType) && toTy.IsAssignableFrom x.Type then converted // converting reference type to supertype - do nothing
            else Expression.Convert(converted, toTy) |> asExpr // emit Expression.Convert

        | Patterns.TypeTest(x, toTy) ->
            Expression.TypeIs(ConvExprToLinqInContext env x, toTy) |> asExpr

        | TypeTestGeneric(_, [toTy], [x]) ->
            Expression.TypeIs(ConvExprToLinqInContext env x, toTy) |> asExpr

        // Expr.*Get
        | Patterns.FieldGet(objOpt, fieldInfo) ->
            Expression.Field(ConvObjArg env objOpt None, fieldInfo) |> asExpr

        | Patterns.TupleGet(arg, n) ->
             let argP = ConvExprToLinqInContext env arg
             let rec build ty argP n =
                 match Reflection.FSharpValue.PreComputeTuplePropertyInfo(ty, n) with
                 | propInfo, None ->
                     Expression.Property(argP, propInfo) |> asExpr
                 | propInfo, Some (nestedTy, n2) ->
                     build nestedTy (Expression.Property(argP, propInfo) |> asExpr) n2
             build arg.Type argP n

        | Patterns.PropertyGet(objOpt, propInfo, args) ->
            let coerceTo =
                if objOpt.IsSome && FSharpType.IsUnion propInfo.DeclaringType && FSharpType.IsUnion propInfo.DeclaringType.BaseType then
                    Some propInfo.DeclaringType
                else
                    None
            match args with
            | [] ->
                Expression.Property(ConvObjArg env objOpt coerceTo, propInfo) |> asExpr
            | _ ->
                let argsP = ConvExprsToLinq env args
                Expression.Call(ConvObjArg env objOpt coerceTo, propInfo.GetGetMethod(true), argsP) |> asExpr

        // Expr.(Call, Application)
        | Patterns.Call(objOpt, minfo, args) ->

            match inp with
            // Special cases for this translation

            // Object initialization generated by LinqQueries
            | MemberInitializationQ(ctor, propInfos) ->
                let bindings =
                  [| for p in propInfos ->
                      match p with
                      | Patterns.PropertySet(_, pinfo, args, assign) ->
                          if args <> [] then raise (NotSupportedException "Parameterized properties not supported in member initialization.")
                          Expression.Bind(pinfo, ConvExprToLinqInContext env assign) :> MemberBinding
                      | _ ->
                          raise (NotSupportedException "Expected PropertySet in member initialization") |]
                match ConvExprToLinqInContext env ctor with
                | :? NewExpression as converted -> Expression.MemberInit(converted, bindings) |> asExpr
                | _ -> raise (NotSupportedException "Expected Constructor call in member initialization")

            // Anonymous type initialization generated by LinqQueries
            | NewAnonymousObjectQ(ctor, args) ->
                let argsR = ConvExprsToLinq env args
                let props = ctor.DeclaringType.GetProperties()
                Expression.New(ctor, argsR, [| for p in props -> (p :> MemberInfo) |]) |> asExpr


            // Do the same thing as C# compiler for string addition
            | PlusQ (_, [ty1; ty2; ty3], [x1; x2]) when (ty1 = typeof<string>) && (ty2 = typeof<string>) && (ty3 = typeof<string>) ->
                 Expression.Add(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2, StringConcat) |> asExpr

            | GenericEqualityQ (_, _, [x1; x2])
            | EqualsQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.Equal
            | NotEqQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.NotEqual
            | GreaterQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.GreaterThan
            | GreaterEqQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.GreaterThanOrEqual
            | LessQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.LessThan
            | LessEqQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.LessThanOrEqual
            | NotQ (_, _, [x1]) -> Expression.Not(ConvExprToLinqInContext env x1) |> asExpr

            | StaticEqualsQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.Equal
            | StaticNotEqQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.NotEqual
            | StaticGreaterQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.GreaterThan
            | StaticGreaterEqQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.GreaterThanOrEqual
            | StaticLessQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.LessThan
            | StaticLessEqQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.LessThanOrEqual

            | NullableEqualsQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.Equal
            | NullableNotEqQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.NotEqual
            | NullableGreaterQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.GreaterThan
            | NullableGreaterEqQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.GreaterThanOrEqual
            | NullableLessQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.LessThan
            | NullableLessEqQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.LessThanOrEqual

            | EqualsNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.Equal
            | NotEqNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.NotEqual
            | GreaterNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.GreaterThan
            | GreaterEqNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.GreaterThanOrEqual
            | LessNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.LessThan
            | LessEqNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.LessThanOrEqual

            | NullableEqualsNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.Equal
            | NullableNotEqNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.NotEqual
            | NullableGreaterNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.GreaterThan
            | NullableGreaterEqNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.GreaterThanOrEqual
            | NullableLessNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.LessThan
            | NullableLessEqNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.LessThanOrEqual

            // Detect the F# quotation encoding of decimal literals
            | MakeDecimalQ (_, _, [Int32 lo; Int32 med; Int32 hi; Bool isNegative; Byte scale]) ->
                Expression.Constant (new System.Decimal(lo, med, hi, isNegative, scale)) |> asExpr

            | NegQ (_, _, [x1]) -> Expression.Negate(ConvExprToLinqInContext env x1) |> asExpr
            | PlusQ (_, _, [x1; x2]) -> Expression.Add(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | DivideQ (_, _, [x1; x2]) -> Expression.Divide (ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | MinusQ (_, _, [x1; x2]) -> Expression.Subtract(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | MultiplyQ (_, _, [x1; x2]) -> Expression.Multiply(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | ModuloQ (_, _, [x1; x2]) -> Expression.Modulo (ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr

            | ShiftLeftQ (_, _, [x1; x2]) -> Expression.LeftShift(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | ShiftRightQ (_, _, [x1; x2]) -> Expression.RightShift(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | BitwiseAndQ (_, _, [x1; x2]) -> Expression.And(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | BitwiseOrQ (_, _, [x1; x2]) -> Expression.Or(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | BitwiseXorQ (_, _, [x1; x2]) -> Expression.ExclusiveOr(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | BitwiseNotQ (_, _, [x1]) -> Expression.Not(ConvExprToLinqInContext env x1) |> asExpr

            | CheckedNeg (_, _, [x1]) -> Expression.NegateChecked(ConvExprToLinqInContext env x1) |> asExpr
            | CheckedPlusQ (_, _, [x1; x2]) -> Expression.AddChecked(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | CheckedMinusQ (_, _, [x1; x2]) -> Expression.SubtractChecked(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr
            | CheckedMultiplyQ (_, _, [x1; x2]) -> Expression.MultiplyChecked(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr


            | NullablePlusQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.Add
            | PlusNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.Add
            | NullablePlusNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.Add

            | NullableMinusQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.Subtract
            | MinusNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.Subtract
            | NullableMinusNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.Subtract

            | NullableMultiplyQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.Multiply
            | MultiplyNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.Multiply
            | NullableMultiplyNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.Multiply

            | NullableDivideQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.Divide
            | DivideNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.Divide
            | NullableDivideNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.Divide

            | NullableModuloQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 true Expression.Modulo
            | ModuloNullableQ (_, _, [x1; x2]) -> transBinOp env true x1 x2 false Expression.Modulo
            | NullableModuloNullableQ (_, _, [x1; x2]) -> transBinOp env false x1 x2 false Expression.Modulo

            | ConvNullableCharQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<char>>) |> asExpr
            | ConvNullableDecimalQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<decimal>>) |> asExpr
            | ConvNullableFloatQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<float>>) |> asExpr
            | ConvNullableDoubleQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<double>>) |> asExpr
            | ConvNullableFloat32Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<float32>>) |> asExpr
            | ConvNullableSingleQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<float32>>) |> asExpr
            | ConvNullableSByteQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<sbyte>>) |> asExpr
            | ConvNullableInt8Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<sbyte>>) |> asExpr
            | ConvNullableInt16Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<int16>>) |> asExpr
            | ConvNullableInt32Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<int32>>) |> asExpr
            | ConvNullableIntQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<int32>>) |> asExpr
            | ConvNullableInt64Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<int64>>) |> asExpr
            | ConvNullableByteQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<byte>>) |> asExpr
            | ConvNullableUInt8Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<byte>>) |> asExpr
            | ConvNullableUInt16Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<uint16>>) |> asExpr
            | ConvNullableUInt32Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<uint32>>) |> asExpr
            | ConvNullableUInt64Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<uint64>>) |> asExpr
            // LINQ expressions can't do native integer operations, so we don't convert these
            //| ConvNullableIntPtrQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<nativeint>>) |> asExpr
            //| ConvNullableUIntPtrQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<Nullable<unativeint>>) |> asExpr

            | ConvCharQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<char>) |> asExpr
            | ConvDecimalQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<decimal>) |> asExpr
            | ConvFloatQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<float>) |> asExpr
            | ConvDoubleQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<double>) |> asExpr
            | ConvFloat32Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<float32>) |> asExpr
            | ConvSingleQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<float32>) |> asExpr
            | ConvSByteQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<sbyte>) |> asExpr
            | ConvInt8Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<sbyte>) |> asExpr
            | ConvInt16Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<int16>) |> asExpr
            | ConvInt32Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<int32>) |> asExpr
            | ConvIntQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<int32>) |> asExpr
            | ConvInt64Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<int64>) |> asExpr
            | ConvByteQ (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<byte>) |> asExpr
            | ConvUInt8Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<byte>) |> asExpr
            | ConvUInt16Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<uint16>) |> asExpr
            | ConvUInt32Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<uint32>) |> asExpr
            | ConvUInt64Q (_, _, [x1]) -> Expression.Convert(ConvExprToLinqInContext env x1, typeof<uint64>) |> asExpr

            | CheckedConvCharQ (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<char>) |> asExpr
            | CheckedConvSByteQ (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<sbyte>) |> asExpr
            | CheckedConvInt8Q (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<sbyte>) |> asExpr
            | CheckedConvInt16Q (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<int16>) |> asExpr
            | CheckedConvInt32Q (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<int32>) |> asExpr
            | CheckedConvInt64Q (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<int64>) |> asExpr
            | CheckedConvByteQ (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<byte>) |> asExpr
            | CheckedConvUInt8Q (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<byte>) |> asExpr
            | CheckedConvUInt16Q (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<uint16>) |> asExpr
            | CheckedConvUInt32Q (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<uint32>) |> asExpr
            | CheckedConvUInt64Q (_, _, [x1]) -> Expression.ConvertChecked(ConvExprToLinqInContext env x1, typeof<uint64>) |> asExpr
            | ArrayLookupQ (_, [_; _; _], [x1; x2]) ->
                Expression.ArrayIndex(ConvExprToLinqInContext env x1, ConvExprToLinqInContext env x2) |> asExpr

            // Throw away markers inserted to satisfy C#'s design where they pass an argument
            // or type T to an argument expecting Expression<T>.
            | ImplicitExpressionConversionHelperQ (_, [_], [x1]) -> ConvExprToLinqInContext env x1

            | _ ->
                let argsP = ConvExprsToLinq env args
                Expression.Call(ConvObjArg env objOpt None, minfo, argsP) |> asExpr

#if !NO_CURRIED_FUNCTION_OPTIMIZATIONS
        // f x1 x2 x3 x4 --> InvokeFast4
        | Patterns.Application(Patterns.Application(Patterns.Application(Patterns.Application(f, arg1), arg2), arg3), arg4) ->
            // TODO: amortize this computation based on f.Type
            let meth =
                let domainTy1, rangeTy = getFunctionType f.Type
                let domainTy2, rangeTy = getFunctionType rangeTy
                let domainTy3, rangeTy = getFunctionType rangeTy
                let domainTy4, rangeTy = getFunctionType rangeTy
                let ty = domainTy1 --> domainTy2
                (ty.GetMethods() |> Array.find (fun minfo -> minfo.Name = "InvokeFast" && minfo.GetParameters().Length = 5)).MakeGenericMethod [| domainTy3; domainTy4; rangeTy |]
            let argsP = ConvExprsToLinq env [f; arg1; arg2; arg3; arg4]
            Expression.Call((null:Expression), meth, argsP) |> asExpr

        // f x1 x2 x3 --> InvokeFast3
        | Patterns.Application(Patterns.Application(Patterns.Application(f, arg1), arg2), arg3) ->
            // TODO: amortize this computation based on f.Type
            let meth =
                let domainTy1, rangeTy = getFunctionType f.Type
                let domainTy2, rangeTy = getFunctionType rangeTy
                let domainTy3, rangeTy = getFunctionType rangeTy
                let ty = domainTy1 --> domainTy2
                (ty.GetMethods() |> Array.find (fun minfo -> minfo.Name = "InvokeFast" && minfo.GetParameters().Length = 4)).MakeGenericMethod [| domainTy3; rangeTy |]
            let argsP = ConvExprsToLinq env [f; arg1; arg2; arg3]
            Expression.Call((null:Expression), meth, argsP) |> asExpr

        // f x1 x2 --> InvokeFast2
        | Patterns.Application(Patterns.Application(f, arg1), arg2) ->
            // TODO: amortize this computation based on f.Type
            let meth =
                let domainTy1, rangeTy = getFunctionType f.Type
                let domainTy2, rangeTy = getFunctionType rangeTy
                let ty = domainTy1 --> domainTy2
                (ty.GetMethods() |> Array.find (fun minfo -> minfo.Name = "InvokeFast" && minfo.GetParameters().Length = 3)).MakeGenericMethod [| rangeTy |]
            let argsP = ConvExprsToLinq env [f; arg1; arg2]
            Expression.Call((null:Expression), meth, argsP) |> asExpr
#endif

        // f x1 --> Invoke
        | Patterns.Application(f, arg) ->
            let fP = ConvExprToLinqInContext env f
            let argP = ConvExprToLinqInContext env arg
            // TODO: amortize this computation based on f.Type
            let meth = f.Type.GetMethod("Invoke")
            Expression.Call(fP, meth, [| argP |]) |> asExpr

        // Expr.New*
        | Patterns.NewRecord(recdTy, args) ->
            let ctorInfo = Reflection.FSharpValue.PreComputeRecordConstructorInfo(recdTy, showAll)
            Expression.New(ctorInfo, ConvExprsToLinq env args) |> asExpr

        | Patterns.NewArray(ty, args) ->
            Expression.NewArrayInit(ty, ConvExprsToLinq env args) |> asExpr

        | Patterns.DefaultValue ty ->
            Expression.New ty |> asExpr

        | Patterns.NewUnionCase(unionCaseInfo, args) ->
            let methInfo = Reflection.FSharpValue.PreComputeUnionConstructorInfo(unionCaseInfo, showAll)
            let argsR = ConvExprsToLinq env args
            Expression.Call((null:Expression), methInfo, argsR) |> asExpr

#if !NO_PATTERN_MATCHING_IN_INPUT_LANGUAGE
        | Patterns.UnionCaseTest(e, unionCaseInfo) ->
            let methInfo = Reflection.FSharpValue.PreComputeUnionTagMemberInfo(unionCaseInfo.DeclaringType, showAll)
            let obj = ConvExprToLinqInContext env e
            let tagE =
                match methInfo with
                | :? PropertyInfo as p ->
                    Expression.Property(obj, p) |> asExpr
                | :? MethodInfo as m ->
                    Expression.Call((null:Expression), m, [| obj |]) |> asExpr
                | _ -> failwith "unreachable case"
            Expression.Equal(tagE, Expression.Constant(unionCaseInfo.Tag)) |> asExpr
#endif

        | (Patterns.NewObject(ctorInfo, args) as x) ->
            match x with
            // LINQ providers prefer C# "Nullable x" to be "Convert x", since that's what C# uses
            // to construct nullable values.
            | NullableConstruction arg -> Expression.Convert(ConvExprToLinqInContext env arg, x.Type) |> asExpr
            | _ -> Expression.New(ctorInfo, ConvExprsToLinq env args) |> asExpr

        | Patterns.NewDelegate(dty, vs, b) ->
            let vsP = List.map ConvVarToLinq vs
            let env = {env with varEnv = List.foldBack2 (fun (v:Var) vP -> Map.add v (vP |> asExpr)) vs vsP env.varEnv }
            let bodyP = ConvExprToLinqInContext env b
            Expression.Lambda(dty, bodyP, vsP) |> asExpr

        | Patterns.NewTuple args ->
             let tupTy = args |> List.map (fun arg -> arg.Type) |> Array.ofList |> Reflection.FSharpType.MakeTupleType
             let argsP = ConvExprsToLinq env args
             let rec build ty (argsP: Expression[]) =
                 match Reflection.FSharpValue.PreComputeTupleConstructorInfo ty with
                 | ctorInfo, None -> Expression.New(ctorInfo, argsP) |> asExpr
                 | ctorInfo, Some (nestedTy) ->
                     let n = ctorInfo.GetParameters().Length - 1
                     Expression.New(ctorInfo, Array.append argsP.[0..n-1] [| build nestedTy argsP.[n..] |]) |> asExpr
             build tupTy argsP

        | Patterns.IfThenElse(g, t, e) ->
            Expression.Condition(ConvExprToLinqInContext env g, ConvExprToLinqInContext env t, ConvExprToLinqInContext env e) |> asExpr

        | Patterns.QuoteTyped x ->
            let fvs = x.GetFreeVars()

            Expression.Call(substHelperMeth.MakeGenericMethod [| x.Type |],
                            [| (Expression.Constant x) |> asExpr
                               (Expression.NewArrayInit(typeof<Var>, [| for fv in fvs -> Expression.Constant fv |> asExpr |]) |> asExpr)
                               (Expression.NewArrayInit(typeof<obj>, [| for fv in fvs -> Expression.Convert(env.varEnv.[fv], typeof<obj>) |> asExpr |]) |> asExpr) |])
                    |> asExpr

        | Patterns.QuoteRaw x ->
            let fvs = x.GetFreeVars()

            Expression.Call(substHelperRawMeth,
                            [| (Expression.Constant x) |> asExpr
                               (Expression.NewArrayInit(typeof<Var>, [| for fv in fvs -> Expression.Constant fv |> asExpr |]) |> asExpr)
                               (Expression.NewArrayInit(typeof<obj>, [| for fv in fvs -> Expression.Convert(env.varEnv.[fv], typeof<obj>) |> asExpr |]) |> asExpr) |])
                    |> asExpr

        | Patterns.Let (v, e, b) ->
            let vP = ConvVarToLinq v
            let envinner = { env with varEnv = Map.add v (vP |> asExpr) env.varEnv }
            let bodyP = ConvExprToLinqInContext envinner b
            let eP = ConvExprToLinqInContext env e
            let ty = Expression.GetFuncType [| v.Type; b.Type |]
            let lam = Expression.Lambda(ty, bodyP, [| vP |]) |> asExpr
            Expression.Call(lam, ty.GetMethod("Invoke", instanceBindingFlags), [| eP |]) |> asExpr

        | Patterns.Lambda(v, body) ->
            let vP = ConvVarToLinq v
            let env = { env with varEnv = Map.add v (vP |> asExpr) env.varEnv }
            let bodyP = ConvExprToLinqInContext env body
            let lambdaTy, tyargs =
                if bodyP.Type = typeof<System.Void> then
                    let tyargs = [| vP.Type |]
                    typedefof<Action<_>>, tyargs
                else
                    let tyargs = [| vP.Type; bodyP.Type |]
                    typedefof<Func<_, _>>, tyargs
            let convType = lambdaTy.MakeGenericType tyargs
            let convDelegate = Expression.Lambda(convType, bodyP, [| vP |]) |> asExpr
            Expression.Call(typeof<FuncConvert>, "ToFSharpFunc", tyargs, [| convDelegate |]) |> asExpr

        | _ ->
            raise (new NotSupportedException(Printf.sprintf "Could not convert the following F# Quotation to a LINQ Expression Tree\n--------\n%A\n-------------\n" inp))

    and transBinOp env addConvertLeft x1 x2 addConvertRight (exprErasedConstructor : _ * _ -> _) =
        let e1 = ConvExprToLinqInContext env x1
        let e2 = ConvExprToLinqInContext env x2
        let e1 = if addConvertLeft then Expression.Convert(e1, typedefof<Nullable<int>>.MakeGenericType [| e1.Type |]) |> asExpr else e1
        let e2 = if addConvertRight then Expression.Convert(e2, typedefof<Nullable<int>>.MakeGenericType [| e2.Type |]) |> asExpr else e2
        exprErasedConstructor(e1, e2) |> asExpr


    and ConvObjArg env objOpt coerceTo : Expression =
        match objOpt with
        | Some obj ->
            let expr = ConvExprToLinqInContext env obj
            match coerceTo with
            | None -> expr
            | Some ty -> Expression.TypeAs(expr, ty) :> Expression
        | None ->
            null

    and ConvExprsToLinq env es : Expression[] =
        es |> List.map (ConvExprToLinqInContext env) |> Array.ofList

    and ConvVarToLinq (v: Var) =
        //printf "** Expression .Parameter(%a, %a)\n" output_any ty output_any nm
        Expression.Parameter(v.Type, v.Name)

    let ConvExprToLinq (e: Expr) = ConvExprToLinqInContext { varEnv = Map.empty } e

    let QuotationToExpression (e: Microsoft.FSharp.Quotations.Expr) = ConvExprToLinq e
    let QuotationToLambdaExpression (e: Microsoft.FSharp.Quotations.Expr<'T>) =  (ConvExprToLinq e) :?> Expression<'T>

    // This contorted compilation is used because LINQ's "Compile" is only allowed on lambda expressions, and LINQ
    // provides no other way to evaluate the expression.
    //
    // REVIEW: It is possible it is just better to interpret the expression in many common cases, e.g. property-gets, values etc.
    let EvaluateQuotation (e: Microsoft.FSharp.Quotations.Expr) : obj =
#if FX_NO_QUOTATIONS_COMPILE
       raise (new NotSupportedException())
#else
       match e with
       | Value (obj, _) -> obj
       | _ ->
       let ty = e.Type
       let e = Expr.NewDelegate (Expression.GetFuncType([|typeof<unit>; ty |]), [new Var("unit", typeof<unit>)], e)
       let linqExpr = (ConvExprToLinq e:?> LambdaExpression)
       let d = linqExpr.Compile ()
       try
           d.DynamicInvoke [| box () |]
       with :? System.Reflection.TargetInvocationException as exn ->
           raise exn.InnerException
#endif


