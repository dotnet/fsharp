// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

module ``Literals`` =

    [<Fact>]
    let ``Literal attribute generates literal static field``() =
        FSharp """
module LiteralValue

[<Literal>]
let x = 7

[<EntryPoint>]
let main _ =
    0
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
.field public static literal int32 x = int32(0x00000007)
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.LiteralAttribute::.ctor() = ( 01 00 00 00 )"""]


    [<Fact>]
    let ``Arithmetic in integer literals is evaluated at compile time``() =
        FSharp """
module LiteralArithmetic

let [<Literal>] bytesInMegabyte = 1024L * 1024L

let [<Literal>] bytesInKilobyte = bytesInMegabyte >>> 10

let [<Literal>] bytesInKilobyte2 = bytesInMegabyte / 1024L

let [<Literal>] secondsInDayPlusThree = 3 + (60 * 60 * 24)

let [<Literal>] bitwise = 1us &&& (3us ||| 4us)

let [<Literal>] bitwise2 = 1y ^^^ (3y + ~~~4y)
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal int64 bytesInMegabyte = int64(0x100000)"""
            """.field public static literal int64 bytesInKilobyte = int64(0x400)"""
            """.field public static literal int64 bytesInKilobyte2 = int64(0x400)"""
            """.field public static literal int32 secondsInDayPlusThree = int32(0x00015183)"""
            """.field public static literal uint16 bitwise = uint16(0x0001)"""
            """.field public static literal int8 bitwise2 = int8(0xFF)"""
        ]

    [<Fact>]
    let ``Arithmetic in char and floating point literals is evaluated at compile time``() =
        // on Linux and Mac floats with no decimal parts are printed without the decimal point (unlike Windows)
        // let's add some fractions so that the tests are consistent
        FSharp """
module LiteralArithmetic

let [<Literal>] bytesInMegabyte = 1024. * 1024. + 0.1

let [<Literal>] bytesInMegabyte' = 1024f ** 2f

let [<Literal>] bytesInKilobyte = bytesInMegabyte / 1024. + 0.1

let [<Literal>] secondsInDayPlusThree = 3.1f + (60f * 60f * 24f)

let [<Literal>] chars = 'a' + 'b' - 'a'
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal float64 bytesInMegabyte = float64(1048576.1000000001)"""
            if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then
                """.field public static literal float32 'bytesInMegabyte\'' = float32(1048576.)"""
            else
                """.field public static literal float32 'bytesInMegabyte\'' = float32(1048576)"""
            """.field public static literal float64 bytesInKilobyte = float64(1024.10009765625)"""
            """.field public static literal float32 secondsInDayPlusThree = float32(86403.102)"""
            """.field public static literal char chars = char(0x0062)"""
        ]

    [<Fact>]
    let ``Logical operations on booleans are evaluated at compile time``() =
        FSharp """
module LiteralArithmetic

let [<Literal>] flag = true

let [<Literal>] flippedFlag = not flag

let [<Literal>] simple1 = flippedFlag || false

let [<Literal>] simple2 = true && not true

let [<Literal>] complex1 = false || (flag && not flippedFlag)

let [<Literal>] complex2 = false || (flag && flippedFlag)

let [<Literal>] complex3 = true || (flag && not flippedFlag)
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal bool flag = bool(true)"""
            """.field public static literal bool flippedFlag = bool(false)"""
            """.field public static literal bool simple1 = bool(false)"""
            """.field public static literal bool simple2 = bool(false)"""
            """.field public static literal bool complex1 = bool(true)"""
            """.field public static literal bool complex2 = bool(false)"""
            """.field public static literal bool complex3 = bool(true)"""
        ]

    [<Fact>]
    let ``Arithmetic can be used for constructing enum literals``() =
        FSharp """
module LiteralArithmetic

type E =
    | A = 1
    | B = 2
    
let [<Literal>] x = enum<E> (1 + 1)
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal valuetype LiteralArithmetic/E x = int32(0x00000002)"""
        ]

    [<Fact>]
    let ``Arithmetic can be used for constructing literals in attributes``() =
        FSharp """
module LiteralArithmetic

open System.Runtime.CompilerServices

// 256 = AggressiveInlining
[<MethodImpl(enum -(-1 <<< 8))>]
let x () =
    3
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.method public static int32  x() cil managed aggressiveinlining"""
        ]

    [<Fact>]
    let ``Compilation fails when addition in literal overflows``() =
        FSharp """
module LiteralArithmetic

let [<Literal>] x = System.Int32.MaxValue + 1
        """
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResult {
            Error = Error 3177
            Range = { StartLine = 4
                      StartColumn = 21
                      EndLine = 4
                      EndColumn = 46 }
            Message = "This literal expression or attribute argument results in an arithmetic overflow."
        }

    [<Fact>]
    let ``Decimal literals are properly initialized``() =
        FSharp """
module DecimalInit

[<Literal>]
let x = 5.5m
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """
.class public abstract auto ansi sealed DecimalInit
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .field public static initonly valuetype [runtime]System.Decimal x
  .custom instance void [runtime]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                        uint8,
                                                                                                        int32,
                                                                                                        int32,
                                                                                                        int32) = ( 01 00 01 00 00 00 00 00 00 00 00 00 37 00 00 00   
                                                                                                                   00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$DecimalInit::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$DecimalInit::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   55
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  ldc.i4.0
    IL_0005:  ldc.i4.1
    IL_0006:  newobj     instance void [runtime]System.Decimal::.ctor(int32,
                                                                             int32,
                                                                             int32,
                                                                             bool,
                                                                             uint8)
    IL_000b:  stsfld     valuetype [runtime]System.Decimal DecimalInit::x
    IL_0010:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$DecimalInit
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void DecimalInit::staticInitialization@()
    IL_0005:  ret
  } 

}
"""
        ]

    [<Fact>]
    let ``Arithmetic can be used for constructing decimal literals``() =
        FSharp """
module LiteralArithmetic

[<Literal>]
let x = 1m + 2m
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static initonly valuetype [runtime]System.Decimal x
.custom instance void [runtime]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                    uint8,
                                                                                                    int32,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 
                                                                                                                00 00 )"""
            """
.method assembly specialname static void staticInitialization@() cil managed
{

.maxstack  8
IL_0000:  ldc.i4.3
IL_0001:  ldc.i4.0
IL_0002:  ldc.i4.0
IL_0003:  ldc.i4.0
IL_0004:  ldc.i4.0
IL_0005:  newobj     instance void [runtime]System.Decimal::.ctor(int32,
                                                                        int32,
                                                                        int32,
                                                                        bool,
                                                                        uint8)
IL_000a:  stsfld     valuetype [runtime]System.Decimal LiteralArithmetic::x
IL_000f:  ret
}
"""
        ]

    [<Fact>]
    let ``Pattern matching decimal literal``() =
        FSharp """
module PatternMatch

[<Literal>]
let x = 5m

let test () =
    match x with
    | 5m -> 0
    | _ -> 1
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """
.field public static initonly valuetype [runtime]System.Decimal x
.custom instance void [runtime]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                    uint8,
                                                                                                    int32,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 
                                                                                                                00 00 )"""
            """
.method public static int32  test() cil managed
    {

.maxstack  8
.locals init (valuetype [runtime]System.Decimal V_0)
IL_0000:  ldc.i4.5
IL_0001:  ldc.i4.0
IL_0002:  ldc.i4.0
IL_0003:  ldc.i4.0
IL_0004:  ldc.i4.0
IL_0005:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
IL_000a:  stloc.0
IL_000b:  ldloc.0
IL_000c:  ldc.i4.5
IL_000d:  ldc.i4.0
IL_000e:  ldc.i4.0
IL_000f:  ldc.i4.0
IL_0010:  ldc.i4.0
IL_0011:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
IL_0016:  call       bool [netstandard]System.Decimal::op_Equality(valuetype [netstandard]System.Decimal,
                                                                valuetype [netstandard]System.Decimal)
  """
        ]

    [<Fact>]
    let ``Multiple decimals literals can be created``() =
        FSharp """
module DecimalLiterals

[<Literal>]
let x = 41m

[<Literal>]
let y = 42m
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """
.field public static initonly valuetype [runtime]System.Decimal x
.custom instance void [runtime]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                    uint8,
                                                                                                    int32,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 00 00 00 00 00 00 00 00 00 00 29 00 00 00   
                                                                                                                00 00 )
"""
            """
.field public static initonly valuetype [runtime]System.Decimal y
.custom instance void [runtime]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                    uint8,
                                                                                                    int32,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 00 00 00 00 00 00 00 00 00 00 2A 00 00 00   
                                                                                                                00 00 )
"""
            """
.method assembly specialname static void staticInitialization@() cil managed
{

.maxstack  8
IL_0000:  ldc.i4.s   41
IL_0002:  ldc.i4.0
IL_0003:  ldc.i4.0
IL_0004:  ldc.i4.0
IL_0005:  ldc.i4.0
IL_0006:  newobj     instance void [runtime]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
IL_000b:  stsfld     valuetype [runtime]System.Decimal DecimalLiterals::x
IL_0010:  ldc.i4.s   42
IL_0012:  ldc.i4.0
IL_0013:  ldc.i4.0
IL_0014:  ldc.i4.0
IL_0015:  ldc.i4.0
IL_0016:  newobj     instance void [runtime]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
IL_001b:  stsfld     valuetype [runtime]System.Decimal DecimalLiterals::y
IL_0020:  ret
} 
"""
            ]

    [<Fact>]
    let ``Compilation fails when using arithmetic with a non-literal in literal``() =
        FSharp """
module LiteralArithmetic

let [<Literal>] x = 1 + System.DateTime.Now.Hour
        """
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
#if !NETCOREAPP
            { Error = Warning 52
              Range = { StartLine = 4
                        StartColumn = 25
                        EndLine = 4
                        EndColumn = 49 }
              Message = "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed" }
#endif
            { Error = Error 267
              Range = { StartLine = 4
                        StartColumn = 25
                        EndLine = 4
                        EndColumn = 49 }
              Message = "This is not a valid constant expression or custom attribute value" }
            { Error = Error 267
              Range = { StartLine = 4
                        StartColumn = 21
                        EndLine = 4
                        EndColumn = 49 }
              Message = "This is not a valid constant expression or custom attribute value" }
        ]

    [<Fact>]
    let ``Arithmetic cannot be used in enums, literals and attributes in lang version70``() =
        FSharp """
module LiteralArithmetic

open System.Runtime.CompilerServices

[<MethodImpl(enum -(-1 <<< 8))>]
let x () = 3

let [<Literal>] lit = 1 <<< (7 * 10)

type E =
    | A = (1 <<< 2)
    | B = 1
    | C = (5 / 3 * 4)
        """
        |> withLangVersion70
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 6, Col 19, Line 6, Col 30, "Feature 'Arithmetic and logical operations in literals, enum definitions and attributes' is not available in F# 7.0. Please use language version 8.0 or greater.")
            (Error 3350, Line 9, Col 23, Line 9, Col 37, "Feature 'Arithmetic and logical operations in literals, enum definitions and attributes' is not available in F# 7.0. Please use language version 8.0 or greater.")
            (Error 3350, Line 12, Col 12, Line 12, Col 19, "Feature 'Arithmetic and logical operations in literals, enum definitions and attributes' is not available in F# 7.0. Please use language version 8.0 or greater.")
            (Error 3350, Line 14, Col 12, Line 14, Col 21, "Feature 'Arithmetic and logical operations in literals, enum definitions and attributes' is not available in F# 7.0. Please use language version 8.0 or greater.")
        ]
