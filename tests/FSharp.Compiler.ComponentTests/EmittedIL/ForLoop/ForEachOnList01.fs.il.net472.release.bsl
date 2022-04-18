
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:0:0:0
}
.assembly ForEachOnList01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForEachOnList01
{
  // Offset: 0x00000000 Length: 0x00000341
  // WARNING: managed resource file FSharpSignatureData.ForEachOnList01 created
}
.mresource public FSharpOptimizationData.ForEachOnList01
{
  // Offset: 0x00000348 Length: 0x000000DB
  // WARNING: managed resource file FSharpOptimizationData.ForEachOnList01 created
}
.module ForEachOnList01.exe
// MVID: {625078DB-3D6B-3F80-A745-0383DB785062}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02EF0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForEachOnList01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit test6@38
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class ForEachOnList01/test6@38 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method test6@38::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.1
      IL_0002:  add
      IL_0003:  ret
    } // end of method test6@38::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void ForEachOnList01/test6@38::.ctor()
      IL_0005:  stsfld     class ForEachOnList01/test6@38 ForEachOnList01/test6@38::@_instance
      IL_000a:  ret
    } // end of method test6@38::.cctor

  } // end of class test6@38

  .class auto ansi serializable sealed nested assembly beforefieldinit test7@47
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class ForEachOnList01/test7@47 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method test7@47::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.1
      IL_0002:  add
      IL_0003:  ret
    } // end of method test7@47::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void ForEachOnList01/test7@47::.ctor()
      IL_0005:  stsfld     class ForEachOnList01/test7@47 ForEachOnList01/test7@47::@_instance
      IL_000a:  ret
    } // end of method test7@47::.cctor

  } // end of class test7@47

  .method public static void  test1(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> lst) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  4
    .locals init (int32 V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             int32 V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  stloc.1
    IL_0004:  ldloc.1
    IL_0005:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_000a:  stloc.2
    IL_000b:  br.s       IL_0021

    IL_000d:  ldloc.1
    IL_000e:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0013:  stloc.3
    IL_0014:  ldloc.0
    IL_0015:  ldloc.3
    IL_0016:  add
    IL_0017:  stloc.0
    IL_0018:  ldloc.2
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0020:  stloc.2
    IL_0021:  ldloc.2
    IL_0022:  brtrue.s   IL_000d

    IL_0024:  ret
  } // end of method ForEachOnList01::test1

  .method public static void  test2() cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  6
    .locals init (int32 V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             int32 V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.2
    IL_0004:  ldc.i4.3
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0020:  stloc.2
    IL_0021:  br.s       IL_0037

    IL_0023:  ldloc.1
    IL_0024:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0029:  stloc.3
    IL_002a:  ldloc.0
    IL_002b:  ldloc.3
    IL_002c:  add
    IL_002d:  stloc.0
    IL_002e:  ldloc.2
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0036:  stloc.2
    IL_0037:  ldloc.2
    IL_0038:  brtrue.s   IL_0023

    IL_003a:  ret
  } // end of method ForEachOnList01::test2

  .method public static void  test3() cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  6
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             int32 V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_3,
             int32 V_4)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  stloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  stloc.1
    IL_001a:  ldloc.0
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0022:  stloc.3
    IL_0023:  br.s       IL_003b

    IL_0025:  ldloc.2
    IL_0026:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_002b:  stloc.s    V_4
    IL_002d:  ldloc.1
    IL_002e:  ldloc.s    V_4
    IL_0030:  add
    IL_0031:  stloc.1
    IL_0032:  ldloc.3
    IL_0033:  stloc.2
    IL_0034:  ldloc.2
    IL_0035:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003a:  stloc.3
    IL_003b:  ldloc.3
    IL_003c:  brtrue.s   IL_0025

    IL_003e:  ret
  } // end of method ForEachOnList01::test3

  .method public static void  test4() cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  6
    .locals init (int32 V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_3,
             int32 V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.2
    IL_0004:  ldc.i4.3
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0022:  stloc.3
    IL_0023:  br.s       IL_003b

    IL_0025:  ldloc.2
    IL_0026:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_002b:  stloc.s    V_4
    IL_002d:  ldloc.0
    IL_002e:  ldloc.s    V_4
    IL_0030:  add
    IL_0031:  stloc.0
    IL_0032:  ldloc.3
    IL_0033:  stloc.2
    IL_0034:  ldloc.2
    IL_0035:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003a:  stloc.3
    IL_003b:  ldloc.3
    IL_003c:  brtrue.s   IL_0025

    IL_003e:  ret
  } // end of method ForEachOnList01::test4

  .method public static void  test5() cil managed
  {
    // Code size       86 (0x56)
    .maxstack  6
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             int32 V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_4)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0020:  stloc.2
    IL_0021:  br.s       IL_0052

    IL_0023:  ldloc.1
    IL_0024:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0029:  stloc.3
    IL_002a:  ldstr      "%A"
    IL_002f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0034:  stloc.s    V_4
    IL_0036:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_003b:  ldloc.s    V_4
    IL_003d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0042:  ldloc.3
    IL_0043:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0048:  pop
    IL_0049:  ldloc.2
    IL_004a:  stloc.1
    IL_004b:  ldloc.1
    IL_004c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0051:  stloc.2
    IL_0052:  ldloc.2
    IL_0053:  brtrue.s   IL_0023

    IL_0055:  ret
  } // end of method ForEachOnList01::test5

  .method public static void  test6() cil managed
  {
    // Code size       98 (0x62)
    .maxstack  8
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             int32 V_2,
             class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_3)
    IL_0000:  ldsfld     class ForEachOnList01/test6@38 ForEachOnList01/test6@38::@_instance
    IL_0005:  ldc.i4.1
    IL_0006:  ldc.i4.2
    IL_0007:  ldc.i4.3
    IL_0008:  ldc.i4.4
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0013:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0018:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0022:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_002e:  stloc.1
    IL_002f:  br.s       IL_005e

    IL_0031:  ldloc.0
    IL_0032:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0037:  stloc.2
    IL_0038:  ldstr      "%O"
    IL_003d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0042:  stloc.3
    IL_0043:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0048:  ldloc.3
    IL_0049:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_004e:  ldloc.2
    IL_004f:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0054:  pop
    IL_0055:  ldloc.1
    IL_0056:  stloc.0
    IL_0057:  ldloc.0
    IL_0058:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_005d:  stloc.1
    IL_005e:  ldloc.1
    IL_005f:  brtrue.s   IL_0031

    IL_0061:  ret
  } // end of method ForEachOnList01::test6

  .method public static void  test7() cil managed
  {
    // Code size       104 (0x68)
    .maxstack  8
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             int32 V_2,
             int32 V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_4)
    IL_0000:  ldsfld     class ForEachOnList01/test7@47 ForEachOnList01/test7@47::@_instance
    IL_0005:  ldc.i4.1
    IL_0006:  ldc.i4.2
    IL_0007:  ldc.i4.3
    IL_0008:  ldc.i4.4
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0013:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0018:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0022:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_002e:  stloc.1
    IL_002f:  br.s       IL_0064

    IL_0031:  ldloc.0
    IL_0032:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0037:  stloc.2
    IL_0038:  ldloc.2
    IL_0039:  ldc.i4.1
    IL_003a:  add
    IL_003b:  stloc.3
    IL_003c:  ldstr      "%O"
    IL_0041:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0046:  stloc.s    V_4
    IL_0048:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_004d:  ldloc.s    V_4
    IL_004f:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0054:  ldloc.3
    IL_0055:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_005a:  pop
    IL_005b:  ldloc.1
    IL_005c:  stloc.0
    IL_005d:  ldloc.0
    IL_005e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0063:  stloc.1
    IL_0064:  ldloc.1
    IL_0065:  brtrue.s   IL_0031

    IL_0067:  ret
  } // end of method ForEachOnList01::test7

} // end of class ForEachOnList01

.class private abstract auto ansi sealed '<StartupCode$ForEachOnList01>'.$ForEachOnList01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $ForEachOnList01::main@

} // end of class '<StartupCode$ForEachOnList01>'.$ForEachOnList01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net472\tests\EmittedIL\ForLoop\ForEachOnList01_fs\ForEachOnList01.res
