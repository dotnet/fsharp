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
