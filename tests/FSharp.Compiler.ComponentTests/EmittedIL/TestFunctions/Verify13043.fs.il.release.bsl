
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly Verify13043
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Verify13043
{
  // Offset: 0x00000000 Length: 0x00000374
  // WARNING: managed resource file FSharpSignatureData.Verify13043 created
}
.mresource public FSharpOptimizationData.Verify13043
{
  // Offset: 0x00000378 Length: 0x000000F1
  // WARNING: managed resource file FSharpOptimizationData.Verify13043 created
}
.module Verify13043.exe
// MVID: {626FF6A4-9119-C695-A745-0383A4F66F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00BF0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Verify13043
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit f@8
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> Verify13043/f@8::condition
      IL_000d:  ret
    } // end of method f@8::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> l) cil managed
    {
      // Code size       62 (0x3e)
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_0012

      IL_000c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0011:  ret

      IL_0012:  ldloc.0
      IL_0013:  stloc.1
      IL_0014:  ldloc.1
      IL_0015:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_001a:  stloc.2
      IL_001b:  ldloc.1
      IL_001c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
      IL_0021:  stloc.3
      IL_0022:  nop
      IL_0023:  ldarg.0
      IL_0024:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> Verify13043/f@8::condition
      IL_0029:  ldloc.3
      IL_002a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::Invoke(!0)
      IL_002f:  brfalse.s  IL_0036

      IL_0031:  ldloc.2
      IL_0032:  starg.s    l
      IL_0034:  br.s       IL_0000

      IL_0036:  ldloc.3
      IL_0037:  ldloc.2
      IL_0038:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_003d:  ret
    } // end of method f@8::Invoke

  } // end of class f@8

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f@27-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> Verify13043/'f@27-1'::condition
      IL_000d:  ret
    } // end of method 'f@27-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> _arg1) cil managed
    {
      // Code size       62 (0x3e)
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_0012

      IL_000c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0011:  ret

      IL_0012:  ldloc.0
      IL_0013:  stloc.1
      IL_0014:  ldloc.1
      IL_0015:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_001a:  stloc.2
      IL_001b:  ldloc.1
      IL_001c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
      IL_0021:  stloc.3
      IL_0022:  nop
      IL_0023:  ldarg.0
      IL_0024:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> Verify13043/'f@27-1'::condition
      IL_0029:  ldloc.3
      IL_002a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::Invoke(!0)
      IL_002f:  brfalse.s  IL_0036

      IL_0031:  ldloc.2
      IL_0032:  starg.s    _arg1
      IL_0034:  br.s       IL_0000

      IL_0036:  ldloc.3
      IL_0037:  ldloc.2
      IL_0038:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_003d:  ret
    } // end of method 'f@27-1'::Invoke

  } // end of class 'f@27-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit matchResult@38
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Verify13043/matchResult@38 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } // end of method matchResult@38::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  call       bool Verify13043::condition(int32)
      IL_0006:  ret
    } // end of method matchResult@38::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Verify13043/matchResult@38::.ctor()
      IL_0005:  stsfld     class Verify13043/matchResult@38 Verify13043/matchResult@38::@_instance
      IL_000a:  ret
    } // end of method matchResult@38::.cctor

  } // end of class matchResult@38

  .class auto ansi serializable sealed nested assembly beforefieldinit functionResult@43
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Verify13043/functionResult@43 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } // end of method functionResult@43::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  call       bool Verify13043::condition(int32)
      IL_0006:  ret
    } // end of method functionResult@43::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Verify13043/functionResult@43::.ctor()
      IL_0005:  stsfld     class Verify13043/functionResult@43 Verify13043/functionResult@43::@_instance
      IL_000a:  ret
    } // end of method functionResult@43::.cctor

  } // end of class functionResult@43

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_list() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Verify13043>'.$Verify13043::list@3
    IL_0005:  ret
  } // end of method Verify13043::get_list

  .method public static bool  condition(int32 n) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.3
    IL_0002:  clt
    IL_0004:  ret
  } // end of method Verify13043::condition

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          dropWhileWithMatch(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition,
                             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       17 (0x11)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>> V_0)
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Verify13043/f@8::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.1
    IL_0009:  tail.
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::Invoke(!0)
    IL_0010:  ret
  } // end of method Verify13043::dropWhileWithMatch

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          dropWhileWithFunction(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition,
                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       17 (0x11)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>> V_0)
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Verify13043/'f@27-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.1
    IL_0009:  tail.
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::Invoke(!0)
    IL_0010:  ret
  } // end of method Verify13043::dropWhileWithFunction

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_matchResult() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Verify13043>'.$Verify13043::matchResult@38
    IL_0005:  ret
  } // end of method Verify13043::get_matchResult

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_functionResult() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Verify13043>'.$Verify13043::functionResult@43
    IL_0005:  ret
  } // end of method Verify13043::get_functionResult

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          list()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_list()
  } // end of property Verify13043::list
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          matchResult()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_matchResult()
  } // end of property Verify13043::matchResult
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          functionResult()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_functionResult()
  } // end of property Verify13043::functionResult
} // end of class Verify13043

.class private abstract auto ansi sealed '<StartupCode$Verify13043>'.$Verify13043
       extends [System.Runtime]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list@3
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> matchResult@38
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> functionResult@43
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       127 (0x7f)
    .maxstack  6
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2)
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
    IL_0017:  dup
    IL_0018:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Verify13043>'.$Verify13043::list@3
    IL_001d:  stloc.0
    IL_001e:  ldsfld     class Verify13043/matchResult@38 Verify13043/matchResult@38::@_instance
    IL_0023:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_list()
    IL_0028:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::dropWhileWithMatch(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,
                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_002d:  dup
    IL_002e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Verify13043>'.$Verify13043::matchResult@38
    IL_0033:  stloc.1
    IL_0034:  ldstr      "Match: %A"
    IL_0039:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor(string)
    IL_003e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0043:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_matchResult()
    IL_0048:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_004d:  pop
    IL_004e:  ldsfld     class Verify13043/functionResult@43 Verify13043/functionResult@43::@_instance
    IL_0053:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_list()
    IL_0058:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::dropWhileWithFunction(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,
                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_005d:  dup
    IL_005e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Verify13043>'.$Verify13043::functionResult@43
    IL_0063:  stloc.2
    IL_0064:  ldstr      "Function: %A"
    IL_0069:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor(string)
    IL_006e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0073:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_functionResult()
    IL_0078:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_007d:  pop
    IL_007e:  ret
  } // end of method $Verify13043::main@

} // end of class '<StartupCode$Verify13043>'.$Verify13043


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\GitHub\dsyme\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net6.0\tests\EmittedIL\TestFunctions\Verify13043_il\Verify13043.res
