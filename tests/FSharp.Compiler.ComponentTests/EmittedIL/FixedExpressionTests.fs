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
    
let p = { X = 10; Y = 20 }
let xCopy = pinIt p
if xCopy <> p.X then failwith "xCopy was not equal to X"
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
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
        
let p = Point(10,20)
let xCopy = p.PinIt()
if xCopy <> p.X then failwith "xCopy was not equal to X"
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
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
    use ptr = fixed &arr[0]
    NativePtr.get ptr 0
    
let x = [|'a';'b';'c'|]
let y = pinIt x
if y <> 'a' then failwithf "y did not equal first element of x"
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static char  pinIt(char[] arr) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             char& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldelema    [runtime]System.Char
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  conv.i
    IL_000e:  sizeof     [runtime]System.Char
    IL_0014:  mul
    IL_0015:  add
    IL_0016:  ldobj      [runtime]System.Char
    IL_001b:  ret
  } """ ]
        
module ExtendedFixedExpressions =
    [<Fact>]
    let ``Pin int byref of parameter`` () = 
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop

let pinIt (thing: byref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0
    
let mutable x = 42
let xCopy = pinIt &x
if x <> xCopy then failwith "xCopy was not the same as x" 
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static int32  pinIt(int32& thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.0
    IL_0003:  conv.i
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  conv.i
    IL_0008:  sizeof     [runtime]System.Int32
    IL_000e:  mul
    IL_000f:  add
    IL_0010:  ldobj      [runtime]System.Int32
    IL_0015:  ret
  }  """]
        
    [<Fact>]
    let ``Pin int byref of local variable`` () = 
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop

let pinIt (x: int) =
    let mutable thing = x + 1
    use ptr = fixed &thing
    let thingCopy = NativePtr.get ptr 0
    if thingCopy <> thing then failwith "thingCopy was not the same as thing"
    
pinIt 100
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static void  pinIt(int32 x) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             native int V_1,
             int32& pinned V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  add
    IL_0003:  stloc.0
    IL_0004:  ldloca.s   V_0
    IL_0006:  stloc.2
    IL_0007:  ldloca.s   V_0
    IL_0009:  conv.i
    IL_000a:  stloc.1
    IL_000b:  ldloc.1
    IL_000c:  ldc.i4.0
    IL_000d:  conv.i
    IL_000e:  sizeof     [runtime]System.Int32
    IL_0014:  mul
    IL_0015:  add
    IL_0016:  ldobj      [runtime]System.Int32
    IL_001b:  stloc.3
    IL_001c:  ldloc.3
    IL_001d:  ldloc.0
    IL_001e:  beq.s      IL_002b

    IL_0020:  ldstr      "thingCopy was not the same as thing"
    IL_0025:  call       class [runtime]System.Exception [FSharp.Core]Microsoft.FSharp.Core.Operators::Failure(string)
    IL_002a:  throw

    IL_002b:  ret
  } """ ]
