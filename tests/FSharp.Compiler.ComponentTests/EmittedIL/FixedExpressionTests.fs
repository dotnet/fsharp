namespace EmittedIL.FixedExpressionTests

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

let fail () = failwith "thingCopy was not the same as thing" |> ignore

let pinIt (x: int) =
    let mutable thing = x + 1
    use ptr = fixed &thing
    let thingCopy = NativePtr.get ptr 0
    if thingCopy <> thing then fail ()
    
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
             int32 V_3,
             object V_4)
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
    IL_001e:  beq.s      IL_0039

    IL_0020:  ldc.i4.0
    IL_0021:  brfalse.s  IL_002b

    IL_0023:  ldnull
    IL_0024:  unbox.any  [runtime]System.Object
    IL_0029:  br.s       IL_0036

    IL_002b:  ldstr      "thingCopy was not the same as thing"
    IL_0030:  call       class [runtime]System.Exception [FSharp.Core]Microsoft.FSharp.Core.Operators::Failure(string)
    IL_0035:  throw

    IL_0036:  stloc.s    V_4
    IL_0038:  ret

    IL_0039:  ret
  } """ ]
        
    [<Fact>]
    let ``Pin Span via manual GetPinnableReference call`` () =
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop
open System

let pinIt (thing: Span<char>) =
    use ptr = fixed &thing.GetPinnableReference()
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let span = Span("The quick brown fox jumped over the lazy dog".ToCharArray())
    let x = pinIt span
    if x <> 'T' then failwith "x did not equal the first char of the span"
    0
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static char  pinIt(valuetype [runtime]System.Span`1<char> thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             char& pinned V_1)
    IL_0000:  ldarga.s   thing
    IL_0002:  call       instance !0& valuetype [runtime]System.Span`1<char>::GetPinnableReference()
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
        
    [<Fact>]
    let ``Pin Span`` () =
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop
open System

let pinIt (thing: Span<char>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let span = Span("The quick brown fox jumped over the lazy dog".ToCharArray())
    let x = pinIt span
    if x <> 'T' then failwith "x did not equal the first char of the span"
    0
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static char  pinIt(valuetype [runtime]System.Span`1<char> thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             char& pinned V_1)
    IL_0000:  ldarga.s   thing
    IL_0002:  call       instance !0& valuetype [runtime]System.Span`1<char>::GetPinnableReference()
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
        
    [<Fact>]
    let ``Pin generic ReadOnlySpan`` () =
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop
open System

let pinIt (thing: ReadOnlySpan<'a>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let span = ReadOnlySpan("The quick brown fox jumped over the lazy dog".ToCharArray())
    let x = pinIt span
    if x <> 'T' then failwith "x did not equal the first char of the span"
    0
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static !!a  pinIt<a>(valuetype [runtime]System.ReadOnlySpan`1<!!a> thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             !!a& pinned V_1)
    IL_0000:  ldarga.s   thing
    IL_0002:  call       instance !0& modreq([runtime]System.Runtime.InteropServices.InAttribute) valuetype [runtime]System.ReadOnlySpan`1<!!a>::GetPinnableReference()
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  conv.i
    IL_000e:  sizeof     !!a
    IL_0014:  mul
    IL_0015:  add
    IL_0016:  ldobj      !!a
    IL_001b:  ret
  } """ ]
        
    [<Fact>]
    let ``Pin type with method GetPinnableReference : unit -> byref<T>`` () = 
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop
open System

type RefField<'T>(_value) =
    let mutable _value = _value
    member this.Value = _value
    member this.GetPinnableReference () : byref<'T> = &_value

let pinIt (thing: RefField<int>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let x = RefField(42)
    let y = pinIt x
    if y <> x.Value then failwith "y did not equal x value"
    0
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static int32  pinIt(class FixedExpressions/RefField`1<int32> thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldflda     !0 class FixedExpressions/RefField`1<int32>::_value@7
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
    let ``Pin type with method GetPinnableReference : unit -> inref<T>`` () = 
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop
open System

type ReadonlyRefField<'T>(_value) =
    let mutable _value = _value
    member this.Value = _value
    member this.GetPinnableReference () : inref<'T> = &_value

let pinIt (thing: ReadonlyRefField<int>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let x = ReadonlyRefField(42)
    let y = pinIt x
    if y <> x.Value then failwith "y did not equal x value"
    0
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static int32  pinIt(class FixedExpressions/ReadonlyRefField`1<int32> thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldflda     !0 class FixedExpressions/ReadonlyRefField`1<int32>::_value@7
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
    let ``Pin C# type with method GetPinnableReference : unit -> byref<T>`` () =
        let csLib =
            CSharp """
namespace CSharpLib
{
    public class PinnableReference<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set => _value = value;
        }

        public PinnableReference(T value)
        {
            this._value = value;
        }

        public ref T GetPinnableReference()
        {
            return ref _value;
        }
    }
}
"""         |> withName "CsLib"
        
        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop
open System
open CSharpLib

let pinIt (thing: PinnableReference<int>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let x = PinnableReference(42)
    let y = pinIt x
    if y <> x.Value then failwith "y did not equal x value"
    0
"""
        |> withReferences [csLib]
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static int32  pinIt(class [CsLib]CSharpLib.PinnableReference`1<int32> thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0& class [CsLib]CSharpLib.PinnableReference`1<int32>::GetPinnableReference()
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
    let ``Pin C# byref struct type with method GetPinnableReference : unit -> byref<T>`` () =
        // TODO: Could be a good idea to test a version of this type written in F# too once we get ref fields in byref-like structs:
        // https://github.com/fsharp/fslang-suggestions/issues/1143
        let csLib =
            CSharp """
namespace CsLib
{
    public ref struct RefField<T>
    {
        private readonly ref T _value;

        public T Value
        {
            get => _value;
            set => _value = value;
        }

        public RefField(ref T value)
        {
            this._value = ref value;
        }

        public ref T GetPinnableReference() => ref _value;
    }
}

"""         |> withName "CsLib" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module FixedExpressions
open Microsoft.FSharp.NativeInterop
open CsLib

let pinIt (refField: RefField<int>) =
    use ptr = fixed refField
    NativePtr.get ptr 0

[<EntryPoint>]
let main _ =
    let mutable x = 42
    let refToX = new RefField<_>(&x)
    let y = pinIt refToX
    if y <> x then failwith "y did not equal x"
    0
"""
        |> withReferences [csLib]
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static int32  pinIt(valuetype [CsLib]CsLib.RefField`1<int32> refField) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarga.s   refField
    IL_0002:  call       instance !0& valuetype [CsLib]CsLib.RefField`1<int32>::GetPinnableReference()
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  conv.i
    IL_000e:  sizeof     [runtime]System.Int32
    IL_0014:  mul
    IL_0015:  add
    IL_0016:  ldobj      [runtime]System.Int32
    IL_001b:  ret
  } """ ]
        
    [<Fact>]
    let ``Pin type with extension method GetPinnableReference : unit -> byref<T>`` () =
        Fsx """
module FixedExpressions
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop

type RefField<'T> = { mutable _value: 'T }

[<Extension>]
type RefFieldExtensions =
    [<Extension>]
    static member GetPinnableReference(refField: RefField<'T>) : byref<'T> = &refField._value 

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let mutable x = 42
    let refToX = { _value = x }
    let y = pinIt refToX
    if y <> x then failwith "y did not equal x"
    0
"""
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static !!a  pinIt<T,a>(class FixedExpressions/RefField`1<!!T> thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             !!a& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldflda     !0 class FixedExpressions/RefField`1<!!a>::_value@
    IL_0006:  stloc.1
    IL_0007:  ldloc.1
    IL_0008:  conv.i
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.0
    IL_000c:  conv.i
    IL_000d:  sizeof     !!a
    IL_0013:  mul
    IL_0014:  add
    IL_0015:  ldobj      !!a
    IL_001a:  ret
  } """ ]
