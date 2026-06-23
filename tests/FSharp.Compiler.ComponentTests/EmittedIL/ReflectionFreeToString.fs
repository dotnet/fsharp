// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module ``ReflectionFreeToString`` =

    // Under --reflectionfree the reflective sprintf "%+A" ToString is replaced by a structurally
    // generated one. These tests lock in the emitted IL: a match/field-read that boxes each field,
    // renders it through Operators.ToString (the `string` operator) with a null guard, and joins the
    // parts with String.Concat. No PrintfFormat is constructed.

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
.method public strict virtual instance string ToString() cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

.maxstack  6
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
IL_0027:  ldloc.0
IL_0028:  call       object [FSharp.Core]Microsoft.FSharp.Core.Operators::Box<int32>(!!0)
IL_002d:  brfalse.s  IL_0037

IL_002f:  ldloc.0
IL_0030:  call       string [FSharp.Core]Microsoft.FSharp.Core.Operators::ToString<int32>(!!0)
IL_0035:  br.s       IL_003c

IL_0037:  ldstr      "null"
IL_003c:  stelem     [runtime]System.String
IL_0041:  dup
IL_0042:  ldc.i4.3
IL_0043:  ldstr      "; "
IL_0048:  stelem     [runtime]System.String
IL_004d:  dup
IL_004e:  ldc.i4.4
IL_004f:  ldstr      "Y = "
IL_0054:  stelem     [runtime]System.String
IL_0059:  dup
IL_005a:  ldc.i4.5
IL_005b:  ldarg.0
IL_005c:  ldfld      int32 ReflectionFreeToString/Point::Y@
IL_0061:  stloc.0
IL_0062:  ldloc.0
IL_0063:  call       object [FSharp.Core]Microsoft.FSharp.Core.Operators::Box<int32>(!!0)
IL_0068:  brfalse.s  IL_0072

IL_006a:  ldloc.0
IL_006b:  call       string [FSharp.Core]Microsoft.FSharp.Core.Operators::ToString<int32>(!!0)
IL_0070:  br.s       IL_0077

IL_0072:  ldstr      "null"
IL_0077:  stelem     [runtime]System.String
IL_007c:  dup
IL_007d:  ldc.i4.6
IL_007e:  ldstr      " }"
IL_0083:  stelem     [runtime]System.String
IL_0088:  call       string [runtime]System.String::Concat(string[])
IL_008d:  ret
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
.method public strict virtual instance string ToString() cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

.maxstack  5
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
IL_0021:  ldloc.1
IL_0022:  call       object [FSharp.Core]Microsoft.FSharp.Core.Operators::Box<int32>(!!0)
IL_0027:  brfalse.s  IL_0031

IL_0029:  ldloc.1
IL_002a:  call       string [FSharp.Core]Microsoft.FSharp.Core.Operators::ToString<int32>(!!0)
IL_002f:  br.s       IL_0036

IL_0031:  ldstr      "null"
IL_0036:  ldstr      ")"
IL_003b:  call       string [runtime]System.String::Concat(string,
string,
string)
IL_0040:  ret
}"""]
