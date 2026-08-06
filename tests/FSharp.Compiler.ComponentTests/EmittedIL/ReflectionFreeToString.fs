// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module ``ReflectionFreeToString`` =

    // Under --reflectionfree, records and unions get a structural ToString (fields joined with String.Concat,
    // value-type fields rendered via a direct allocation-free ToString, no PrintfFormat) instead of sprintf "%+A".

    [<Fact>]
    let ``Record ToString is generated structurally without printf`` () =
        FSharp """
module ReflectionFreeToString
type Point = { X: int; Y: int }
        """
        |> withOptions [ "--reflectionfree" ]
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method public hidebysig virtual final instance string  ToString() cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

.maxstack  8
.locals init (int32 V_0)
IL_0000:  ldc.i4.7
IL_0001:  newarr     [runtime]System.String
IL_0006:  dup
IL_0007:  ldc.i4.0
IL_0008:  ldstr      "{ "
IL_000d:  stelem     [runtime]System.String
IL_0012:  dup
IL_0013:  ldc.i4.1
IL_0014:  ldstr      "X = "
IL_0019:  stelem     [runtime]System.String
IL_001e:  dup
IL_001f:  ldc.i4.2
IL_0020:  ldarg.0
IL_0021:  ldfld      int32 ReflectionFreeToString/Point::X@
IL_0026:  stloc.0
IL_0027:  ldloca.s   V_0
IL_0029:  ldnull
IL_002a:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
IL_002f:  call       instance string [netstandard]System.Int32::ToString(string,
class [netstandard]System.IFormatProvider)
IL_0034:  stelem     [runtime]System.String
IL_0039:  dup
IL_003a:  ldc.i4.3
IL_003b:  ldstr      "; "
IL_0040:  stelem     [runtime]System.String
IL_0045:  dup
IL_0046:  ldc.i4.4
IL_0047:  ldstr      "Y = "
IL_004c:  stelem     [runtime]System.String
IL_0051:  dup
IL_0052:  ldc.i4.5
IL_0053:  ldarg.0
IL_0054:  ldfld      int32 ReflectionFreeToString/Point::Y@
IL_0059:  stloc.0
IL_005a:  ldloca.s   V_0
IL_005c:  ldnull
IL_005d:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
IL_0062:  call       instance string [netstandard]System.Int32::ToString(string,
class [netstandard]System.IFormatProvider)
IL_0067:  stelem     [runtime]System.String
IL_006c:  dup
IL_006d:  ldc.i4.6
IL_006e:  ldstr      " }"
IL_0073:  stelem     [runtime]System.String
IL_0078:  call       string [runtime]System.String::Concat(string[])
IL_007d:  ret
}"""]

    [<Fact>]
    let ``Union ToString is generated structurally without printf`` () =
        FSharp """
module ReflectionFreeToString
type Color = | Red | Custom of int
        """
        |> withOptions [ "--reflectionfree" ]
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method public hidebysig virtual final instance string  ToString() cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

.maxstack  6
.locals init (class ReflectionFreeToString/Color/Custom V_0,
int32 V_1)
IL_0000:  ldarg.0
IL_0001:  isinst     ReflectionFreeToString/Color/_Red
IL_0006:  brfalse.s  IL_000e

IL_0008:  ldstr      "Red"
IL_000d:  ret

IL_000e:  ldarg.0
IL_000f:  castclass  ReflectionFreeToString/Color/Custom
IL_0014:  stloc.0
IL_0015:  ldstr      "Custom("
IL_001a:  ldloc.0
IL_001b:  ldfld      int32 ReflectionFreeToString/Color/Custom::item
IL_0020:  stloc.1
IL_0021:  ldloca.s   V_1
IL_0023:  ldnull
IL_0024:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
IL_0029:  call       instance string [netstandard]System.Int32::ToString(string,
class [netstandard]System.IFormatProvider)
IL_002e:  ldstr      ")"
IL_0033:  call       string [runtime]System.String::Concat(string,
string,
string)
IL_0038:  ret
}"""]

    [<Fact>]
    let ``Struct record ToString reads its fields directly off the this pointer`` () =
        FSharp """
module ReflectionFreeToString
[<Struct>] type SPoint = { SX: int; SY: int }
        """
        |> withOptions [ "--reflectionfree" ]
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method public hidebysig virtual final instance string  ToString() cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

.maxstack  8
.locals init (int32 V_0)
IL_0000:  ldc.i4.7
IL_0001:  newarr     [runtime]System.String
IL_0006:  dup
IL_0007:  ldc.i4.0
IL_0008:  ldstr      "{ "
IL_000d:  stelem     [runtime]System.String
IL_0012:  dup
IL_0013:  ldc.i4.1
IL_0014:  ldstr      "SX = "
IL_0019:  stelem     [runtime]System.String
IL_001e:  dup
IL_001f:  ldc.i4.2
IL_0020:  ldarg.0
IL_0021:  ldfld      int32 ReflectionFreeToString/SPoint::SX@
IL_0026:  stloc.0
IL_0027:  ldloca.s   V_0
IL_0029:  ldnull
IL_002a:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
IL_002f:  call       instance string [netstandard]System.Int32::ToString(string,
class [netstandard]System.IFormatProvider)
IL_0034:  stelem     [runtime]System.String
IL_0039:  dup
IL_003a:  ldc.i4.3
IL_003b:  ldstr      "; "
IL_0040:  stelem     [runtime]System.String
IL_0045:  dup
IL_0046:  ldc.i4.4
IL_0047:  ldstr      "SY = "
IL_004c:  stelem     [runtime]System.String
IL_0051:  dup
IL_0052:  ldc.i4.5
IL_0053:  ldarg.0
IL_0054:  ldfld      int32 ReflectionFreeToString/SPoint::SY@
IL_0059:  stloc.0
IL_005a:  ldloca.s   V_0
IL_005c:  ldnull
IL_005d:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
IL_0062:  call       instance string [netstandard]System.Int32::ToString(string,
class [netstandard]System.IFormatProvider)
IL_0067:  stelem     [runtime]System.String
IL_006c:  dup
IL_006d:  ldc.i4.6
IL_006e:  ldstr      " }"
IL_0073:  stelem     [runtime]System.String
IL_0078:  call       string [runtime]System.String::Concat(string[])
IL_007d:  ret
}"""]

    [<Fact>]
    let ``Struct union ToString switches on the tag rather than the case type`` () =
        FSharp """
module ReflectionFreeToString
[<Struct>] type SColor = | SRed | SCustom of item: int
        """
        |> withOptions [ "--reflectionfree" ]
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method public hidebysig virtual final instance string  ToString() cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

.maxstack  6
.locals init (int32 V_0)
IL_0000:  ldarg.0
IL_0001:  call       instance int32 ReflectionFreeToString/SColor::get_Tag()
IL_0006:  ldc.i4.0
IL_0007:  bne.un.s   IL_000f

IL_0009:  ldstr      "SRed"
IL_000e:  ret

IL_000f:  ldstr      "SCustom("
IL_0014:  ldarg.0
IL_0015:  ldfld      int32 ReflectionFreeToString/SColor::_item
IL_001a:  stloc.0
IL_001b:  ldloca.s   V_0
IL_001d:  ldnull
IL_001e:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
IL_0023:  call       instance string [netstandard]System.Int32::ToString(string,
class [netstandard]System.IFormatProvider)
IL_0028:  ldstr      ")"
IL_002d:  call       string [runtime]System.String::Concat(string,
string,
string)
IL_0032:  ret
}"""]

    // An anonymous record's fields are type parameters, so each renders through the generic box+null guard and
    // the recursion guard is always emitted. The field reads are left out of the baseline: they name the
    // anonymous type, whose mangled name is not stable.
    [<Fact>]
    let ``Anonymous record ToString is generated with a recursion guard`` () =
        FSharp """
module ReflectionFreeToString
let anon (o: obj) = {| A = 1; N = o |}
        """
        |> withOptions [ "--reflectionfree" ]
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method public strict virtual instance string ToString() cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

.maxstack  6
.locals init (!'<A>j__TPar' V_0,
!'<N>j__TPar' V_1)
IL_0000:  call       void [runtime]System.Runtime.CompilerServices.RuntimeHelpers::EnsureSufficientExecutionStack()
IL_0005:  ldc.i4.7
IL_0006:  newarr     [runtime]System.String
IL_000b:  dup
IL_000c:  ldc.i4.0
IL_000d:  ldstr      "{| "
IL_0012:  stelem     [runtime]System.String
IL_0017:  dup
IL_0018:  ldc.i4.1
IL_0019:  ldstr      "A = "
IL_001e:  stelem     [runtime]System.String
IL_0023:  dup
IL_0024:  ldc.i4.2
IL_0025:  ldarg.0"""
                     """
IL_002d:  call       object [FSharp.Core]Microsoft.FSharp.Core.Operators::Box<!'<A>j__TPar'>(!!0)
IL_0032:  brfalse.s  IL_003c

IL_0034:  ldloc.0
IL_0035:  call       string [FSharp.Core]Microsoft.FSharp.Core.Operators::ToString<!'<A>j__TPar'>(!!0)
IL_003a:  br.s       IL_0041

IL_003c:  ldstr      "null"
IL_0041:  stelem     [runtime]System.String
IL_0046:  dup
IL_0047:  ldc.i4.3
IL_0048:  ldstr      "; "
IL_004d:  stelem     [runtime]System.String
IL_0052:  dup
IL_0053:  ldc.i4.4
IL_0054:  ldstr      "N = "
IL_0059:  stelem     [runtime]System.String
IL_005e:  dup
IL_005f:  ldc.i4.5
IL_0060:  ldarg.0"""
                     """
IL_0083:  ldstr      " |}"
IL_0088:  stelem     [runtime]System.String
IL_008d:  call       string [runtime]System.String::Concat(string[])
IL_0092:  ret
}"""]
