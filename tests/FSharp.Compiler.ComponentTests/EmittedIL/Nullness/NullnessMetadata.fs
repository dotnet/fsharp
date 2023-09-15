// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module NullnessMetadata =

    [<Fact>]
    let ``Nullable attribute gets generated``() =    
        FSharp """
module MyTestModule

let nonNullableInputOutputFunc (x:string) = x
let nullableStringInputOutputFunc (x: string | null) = x
let nonNullableIntFunc (x:int) = x
let nullableIntFunc (x:System.Nullable<int>) = x
let genericValueTypeTest (x: struct(string * (string|null) * int * int * int * int)) = x
let genericRefTypeTest (x: string * (string|null) * int * int * int * int) = x
let nestedGenericsTest (x: list<list<string | null> | null> | null) = x
let multiArgumentTest (x:string) (y:string | null) = 42
        """
        |> withLangVersionPreview
        |> withOptions ["--checknulls"]
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method public static string  nonNullableInputOutputFunc(string x) cil managed
{
.param [0]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 01 00 00 ) 
.param [1]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 01 00 00 ) 
    
.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ret
} """;"""
.method public static string  nullableStringInputOutputFunc(string x) cil managed
{
.param [0]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 02 00 00 ) 
.param [1]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 02 00 00 ) 
    
.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ret
} """;"""
.method public static int32  nonNullableIntFunc(int32 x) cil managed
{
    
.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ret
} """;"""
.method public static valuetype [runtime]System.Nullable`1<int32> 
    nullableIntFunc(valuetype [runtime]System.Nullable`1<int32> x) cil managed
{
    
.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ret
} """;"""
.method public static valuetype [runtime]System.ValueTuple`6<string,string,int32,int32,int32,int32> 
    genericValueTypeTest(valuetype [runtime]System.ValueTuple`6<string,string,int32,int32,int32,int32> x) cil managed
{
.param [0]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 03 00 00 00 00 01 02 00 00 ) 
.param [1]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 03 00 00 00 00 01 02 00 00 ) 
    
.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ret
} """;"""
.method public static class [runtime]System.Tuple`6<string,string,int32,int32,int32,int32> 
    genericRefTypeTest(string x_0,
                        string x_1,
                        int32 x_2,
                        int32 x_3,
                        int32 x_4,
                        int32 x_5) cil managed
{
.param [0]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 03 00 00 00 01 01 02 00 00 ) 
.param [1]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 01 00 00 ) 
.param [2]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 02 00 00 ) 
    
.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldarg.1
IL_0002:  ldarg.2
IL_0003:  ldarg.3
IL_0004:  ldarg.s    x_4
IL_0006:  ldarg.s    x_5
IL_0008:  newobj     instance void class [runtime]System.Tuple`6<string,string,int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3,
                                                                                                            !4,
                                                                                                            !5)
IL_000d:  ret
} """;"""
.method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>> 
    nestedGenericsTest(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>> x) cil managed
{
.param [0]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 02 00 00 ) 
.param [1]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 02 00 00 ) 
    
.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ret
} """;"""
.method public static int32  multiArgumentTest(string x,
                                            string y) cil managed
{
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
.param [1]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 01 00 00 ) 
.param [2]
.custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(class [runtime]System.Array<uint8>) = ( 01 00 01 00 00 00 02 00 00 ) 
    
.maxstack  8
IL_0000:  ldc.i4.s   42
IL_0002:  ret
} """;]

  