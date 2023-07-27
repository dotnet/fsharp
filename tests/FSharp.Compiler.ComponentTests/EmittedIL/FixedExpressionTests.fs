module EmittedIL.FixedExpressionTests

open Xunit
open FSharp.Compiler.Diagnostics
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Test.Compiler

module Legacy =
    [<Fact>]
    let ``Pin naked string``() = 
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop

let pinIt (str: string) =
    use ptr = fixed str
    NativePtr.get ptr 0
"""
        |> withOptions ["--nowarn:9"]
        |> compile
        |> verifyIL ["""
   .method public static char  pinIt(string str) cil managed
   {
     
     .maxstack  5
     .locals init (native int V_0,
              string pinned V_1)
     IL_0000:  ldarg.0
     IL_0001:  stloc.1
     IL_0002:  ldarg.0
     IL_0003:  brfalse.s  IL_000f

     IL_0005:  ldarg.0
     IL_0006:  conv.i
     IL_0007:  call       int32 [runtime]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
     IL_000c:  add
     IL_000d:  br.s       IL_0010

     IL_000f:  ldarg.0
     IL_0010:  stloc.0
     IL_0011:  ldloc.0
     IL_0012:  ldc.i4.0
     IL_0013:  conv.i
     IL_0014:  sizeof     [runtime]System.Char
     IL_001a:  mul
     IL_001b:  add
     IL_001c:  ldobj      [runtime]System.Char
     IL_0021:  ret
   }""" ]

    [<Fact>]
    let ``Pin naked array`` () = 
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop

let pinIt (arr: char[]) =
    use ptr = fixed arr
    NativePtr.get ptr 0
"""
        |> withOptions ["--nowarn:9"]
        |> compile
        |> verifyIL ["""
  .method public static char  pinIt(char[] arr) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             char& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_001b

    IL_0003:  ldarg.0
    IL_0004:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Length<char>(!!0[])
    IL_0009:  brfalse.s  IL_0017

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.0
    IL_000d:  ldelema    [runtime]System.Char
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  conv.i
    IL_0015:  br.s       IL_001d

    IL_0017:  ldc.i4.0
    IL_0018:  conv.i
    IL_0019:  br.s       IL_001d

    IL_001b:  ldc.i4.0
    IL_001c:  conv.i
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.0
    IL_0020:  conv.i
    IL_0021:  sizeof     [runtime]System.Char
    IL_0027:  mul
    IL_0028:  add
    IL_0029:  ldobj      [runtime]System.Char
    IL_002e:  ret
  } """ ]

    [<Fact>]
    let ``Pin address of record field`` () = 
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop

type Point = { mutable X: int; mutable Y: int }

let pinIt (thing: Point) =
    use ptr = fixed &thing.X
    NativePtr.get ptr 0
"""
        |> withOptions ["--nowarn:9"]
        |> compile
        |> verifyIL ["""
  .method public static int32  pinIt(class FixedExpressions/Point thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldflda     int32 FixedExpressions/Point::X@
    IL_0006:  stloc.1
    IL_0007:  ldloc.1
    IL_0008:  conv.i
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.0
    IL_000c:  conv.i
    IL_000d:  sizeof     [runtime]System.Int32
    IL_0013:  mul
    IL_0014:  add
    IL_0015:  ldobj      [runtime]System.Int32
    IL_001a:  ret
  } """ ]

    [<Fact>]
    let ``Pin address of explicit field on this`` () = 
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop

type Point =
    val mutable X: int
    val mutable Y: int
    
    new(x: int, y: int) = { X = x; Y = y }
    
    member this.PinIt() =
        use ptr = fixed &this.X
        NativePtr.get ptr 0
"""
        |> withOptions ["--nowarn:9"]
        |> compile
        |> verifyIL ["""
    .method public hidebysig instance int32 
            PinIt() cil managed
    {
      
      .maxstack  5
      .locals init (native int V_0,
               int32& pinned V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldflda     int32 FixedExpressions/Point::X
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  conv.i
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  ldc.i4.0
      IL_000c:  conv.i
      IL_000d:  sizeof     [System.Runtime]System.Int32
      IL_0013:  mul
      IL_0014:  add
      IL_0015:  ldobj      [System.Runtime]System.Int32
      IL_001a:  ret
    } """ ]
        
    [<Fact>]
    let ``Pin address of array element`` () =
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop

let pinIt (arr: char[]) =
    use ptr = fixed &arr[42]
    NativePtr.get ptr 0
"""
        |> withOptions ["--nowarn:9"]
        |> compile
        |> verifyIL ["""
  .method public static char  pinIt(char[] arr) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             char& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   42
    IL_0003:  ldelema    [runtime]System.Char
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  conv.i
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  conv.i
    IL_000f:  sizeof     [runtime]System.Char
    IL_0015:  mul
    IL_0016:  add
    IL_0017:  ldobj      [runtime]System.Char
    IL_001c:  ret
  } """ ]
        