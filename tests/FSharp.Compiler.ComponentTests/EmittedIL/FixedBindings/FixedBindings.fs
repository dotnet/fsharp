namespace EmittedIL.FixedBindings

open System.Reflection
open Microsoft.FSharp.NativeInterop
open Xunit
open FSharp.Compiler.Diagnostics
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Test.Compiler

module Legacy =
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin naked string`` langVersion =
        let runtimeSupportsStringGetPinnableReference =
            typeof<string>.GetMethods()
            |> Seq.exists (fun m -> m.Name = "GetPinnableReference")
        
// Sanity check precondition: if .Net Framework were to ever get GetPinnableReference, we'll know here
#if NETCOREAPP3_0_OR_GREATER
        Assert.True(runtimeSupportsStringGetPinnableReference)
#else
        Assert.False(runtimeSupportsStringGetPinnableReference)
#endif
        
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedString.fs")
        |> withLangVersion langVersion
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
   } """ ]

    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked array`` langVersion = 
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedArray.fs")
        |> withLangVersion langVersion
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

    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin address of record field`` langVersion = 
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinAddressOfRecordField.fs")
        |> withLangVersion langVersion
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> verifyIL ["""
  .method public static int32  pinIt(class FixedBindings/Point thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldflda     int32 FixedBindings/Point::X@
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

    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin address of explicit field on this`` langVersion = 
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinAddressOfExplicitFieldOnThis.fs")
        |> withLangVersion langVersion
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
      IL_0001:  ldflda     int32 FixedBindings/Point::X
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
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin address of array element`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinAddressOfArrayElement.fs")
        |> withLangVersion langVersion
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
        
module ExtendedFixedBindings =
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin naked string`` langVersion =
        let runtimeSupportsStringGetPinnableReference =
            typeof<string>.GetMethods()
            |> Seq.exists (fun m -> m.Name = "GetPinnableReference")
        
// Sanity check precondition: if .Net Framework were to ever get GetPinnableReference, we'll know here
#if NETCOREAPP3_0_OR_GREATER
        Assert.True(runtimeSupportsStringGetPinnableReference)
#else
        Assert.False(runtimeSupportsStringGetPinnableReference)
#endif
        
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedString.fs")
        |> withLangVersion langVersion
        |> withOptions ["--nowarn:9"]
        |> compile
        |>  if runtimeSupportsStringGetPinnableReference then
                (fun comp ->
                    comp
                    |> verifyIL ["""
  .method public static char  pinIt(string str) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             char& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000e

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance char& modreq([runtime]System.Runtime.InteropServices.InAttribute) [runtime]System.String::GetPinnableReference()
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  conv.i
    IL_000c:  br.s       IL_000f

    IL_000e:  ldarg.0
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  conv.i
    IL_0013:  sizeof     [runtime]System.Char
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldobj      [runtime]System.Char
    IL_0020:  ret
  } """ ])
            else
                (fun comp ->
                    comp
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
   } """ ])
    
    [<Theory; InlineData("preview")>]
    let ``Pin int byref of parameter`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntByrefOfParameter.fs")
        |> withLangVersion langVersion
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
  }  """ ]
        
    [<Theory; InlineData("preview")>]
    let ``Pin int byref of local variable`` langVersion = 
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntByrefOfLocalVariable.fs")
        |> withLangVersion langVersion
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
    IL_001e:  beq.s      IL_0027

    IL_0020:  call       !!0 FixedBindings::fail<class [FSharp.Core]Microsoft.FSharp.Core.Unit>()
    IL_0025:  pop
    IL_0026:  ret

    IL_0027:  ret
  } """ ]
        
#if NETCOREAPP
    [<Theory; InlineData("preview")>]
    let ``Pin Span via manual GetPinnableReference call`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinSpanWithManualGetPinnableReferenceCall.fs")
        |> withLangVersion langVersion
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
        
    [<Theory; InlineData("preview")>]
    let ``Pin Span`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinSpan.fs")
        |> withLangVersion langVersion
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
        
    [<Theory; InlineData("preview")>]
    let ``Pin generic ReadOnlySpan`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinGenericReadOnlySpan.fs")
        |> withLangVersion langVersion
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static !!a  pinIt<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) a>(valuetype [runtime]System.ReadOnlySpan`1<!!a> thing) cil managed
  {
    .param type a 
      .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
    
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
#endif
        
    [<Theory; InlineData("preview")>]
    let ``Pin type with method GetPinnableReference : unit -> byref<T>`` langVersion = 
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceReturningByref.fs")
        |> withLangVersion langVersion
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
    IL_0001:  brfalse.s  IL_000e

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance !0& class FixedExpressions/RefField`1<int32>::GetPinnableReference()
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  conv.i
    IL_000c:  br.s       IL_000f

    IL_000e:  ldarg.0
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  conv.i
    IL_0013:  sizeof     [runtime]System.Int32
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldobj      [runtime]System.Int32
    IL_0020:  ret
  } """ ]
        
    [<Theory; InlineData("preview")>]
    let ``Pin type with method GetPinnableReference : unit -> inref<T>`` langVersion = 
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceReturningInref.fs")
        |> withLangVersion langVersion
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static int32  pinIt(class FixedBindings/ReadonlyRefField`1<int32> thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000e

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance !0& modreq([runtime]System.Runtime.InteropServices.InAttribute) class FixedBindings/ReadonlyRefField`1<int32>::GetPinnableReference()
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  conv.i
    IL_000c:  br.s       IL_000f

    IL_000e:  ldarg.0
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  conv.i
    IL_0013:  sizeof     [runtime]System.Int32
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldobj      [runtime]System.Int32
    IL_0020:  ret
  } """ ]

    [<Theory; InlineData("preview")>]
    let ``Pin struct type with method GetPinnableReference : unit -> byref<T>`` langVersion =
        // Effectively tests the same thing as the test with Span<T>, but this works on .NET Framework 
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinStructTypeWithGetPinnableReferenceReturningByref.fs")
        |> withLangVersion langVersion
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static !!a  pinIt<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) a>(valuetype FixedBindings/ArrayElementRef`1<!!a> thing) cil managed
  {
    .param type a 
      .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (native int V_0,
             !!a& pinned V_1)
    IL_0000:  ldarga.s   thing
    IL_0002:  call       instance !0& valuetype FixedBindings/ArrayElementRef`1<!!a>::GetPinnableReference()
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
    
    [<Theory; InlineData("preview")>]
    let ``Pin C# type with method GetPinnableReference : unit -> byref<T>`` langVersion =
        let csLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "PinCSharpTypeWithGetPinnableReferenceReturningByref.cs") |> withName "CsLib"
        
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinCSharpTypeWithGetPinnableReferenceReturningByref.fs")
        |> withLangVersion langVersion
        |> withReferences [csLib]
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static int32  pinIt(class [CsLib]PinnableReference`1<int32> thing) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000e

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance !0& class [CsLib]PinnableReference`1<int32>::GetPinnableReference()
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  conv.i
    IL_000c:  br.s       IL_000f

    IL_000e:  ldarg.0
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  conv.i
    IL_0013:  sizeof     [runtime]System.Int32
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldobj      [runtime]System.Int32
    IL_0020:  ret
  } """ ]

#if NETCOREAPP
    [<Theory; InlineData("preview")>]
    let ``Pin C# byref struct type with method GetPinnableReference : unit -> byref<T>`` langVersion =
        // TODO: Could be a good idea to test a version of this type written in F# too once we get ref fields in byref-like structs:
        // https://github.com/fsharp/fslang-suggestions/issues/1143
        let csLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "PinCSharpByrefStructTypeWithGetPinnableReferenceReturningByref.cs")
            |> withName "CsLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinCSharpByrefStructTypeWithGetPinnableReferenceReturningByref.fs")
        |> withLangVersion langVersion
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
#endif
    
    [<Theory; InlineData("preview")>]
    let ``Pin type with extension method GetPinnableReference : unit -> byref<T>`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithExtensionGetPinnableReferenceReturningByref.fs")
        |> withLangVersion langVersion
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static !!a  pinIt<T,valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) a>(class FixedBindings/RefField`1<!!T> thing) cil managed
  {
    .param type a 
      .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (native int V_0,
             !!a& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000e

    IL_0003:  ldarg.0
    IL_0004:  call       !!0& FixedBindings/RefFieldExtensions::GetPinnableReference<!!1>(class FixedBindings/RefField`1<!!0>)
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  conv.i
    IL_000c:  br.s       IL_000f

    IL_000e:  ldarg.0
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  conv.i
    IL_0013:  sizeof     !!a
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldobj      !!a
    IL_0020:  ret
  } """ ]
    
    [<Theory; InlineData("preview")>]
    let ``Pin null value of type with GetPinnableReference`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNullValueOfTypeWithGetPinnableReference.fs")
        |> withLangVersion langVersion
        |> withOptions ["--nowarn:9"]
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method public static void  pinIt(class FixedExpressions/RefField`1<int32> thing) cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             int32& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000e

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance !0& class FixedExpressions/RefField`1<int32>::GetPinnableReference()
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  conv.i
    IL_000c:  br.s       IL_000f

    IL_000e:  ldarg.0
    IL_000f:  stloc.0
    IL_0010:  ldc.i8     0x0
    IL_0019:  conv.i
    IL_001a:  ldloc.0
    IL_001b:  bne.un.s   IL_002e

    IL_001d:  ldstr      "Success - null guard worked"
    IL_0022:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0027:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002c:  pop
    IL_002d:  ret

    IL_002e:  ldstr      "Test failed - null guard did not work"
    IL_0033:  call       class [runtime]System.Exception [FSharp.Core]Microsoft.FSharp.Core.Operators::Failure(string)
    IL_0038:  throw
  } 
"""]
