
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.81.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:4:1:0
}
.assembly IfThenElse01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.IfThenElse01
{
  // Offset: 0x00000000 Length: 0x00000201
}
.mresource public FSharpOptimizationData.IfThenElse01
{
  // Offset: 0x00000208 Length: 0x00000092
}
.module IfThenElse01.dll
// MVID: {5775B6FF-2D6C-0B5D-A745-0383FFB67557}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00BD0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed IfThenElse01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public M
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable nested assembly beforefieldinit f5@5
           extends [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc
    {
      .method assembly specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc::.ctor()
        IL_0006:  ret
      } // end of method f5@5::.ctor

      .method public strict virtual instance object 
              Specialize<a>() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  newobj     instance void class IfThenElse01/M/f5@5T<!!a>::.ctor(class IfThenElse01/M/f5@5)
        IL_0006:  box        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!a>>>>
        IL_000b:  ret
      } // end of method f5@5::Specialize

    } // end of class f5@5

    .class auto ansi serializable nested assembly beforefieldinit f5@5T<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`5<int32,int32,!a,!a,!a>
    {
      .field public class IfThenElse01/M/f5@5 self0@
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class IfThenElse01/M/f5@5 self0@) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`5<int32,int32,!a,!a,!a>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class IfThenElse01/M/f5@5 class IfThenElse01/M/f5@5T<!a>::self0@
        IL_000d:  ret
      } // end of method f5@5T::.ctor

      .method public strict virtual instance !a 
              Invoke(int32 x,
                     int32 y,
                     !a z,
                     !a w) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  7
        .locals init ([0] class IfThenElse01/M/f5@5 V_0)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 5,5 : 48,63 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\Misc\\IfThenElse01.fs'
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class IfThenElse01/M/f5@5 class IfThenElse01/M/f5@5T<!a>::self0@
        IL_0006:  stloc.0
        IL_0007:  nop
        IL_0008:  ldarg.1
        IL_0009:  ldarg.2
        IL_000a:  ble.s      IL_000e

        IL_000c:  br.s       IL_0010

        IL_000e:  br.s       IL_0012

        .line 5,5 : 64,65
        IL_0010:  ldarg.3
        IL_0011:  ret

        .line 5,5 : 71,72
        IL_0012:  ldarg.s    w
        IL_0014:  ret
      } // end of method f5@5T::Invoke

    } // end of class f5@5T

    .method public static char  m() cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  7
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc f5,
               [1] char V_1,
               [2] char V_2,
               [3] int32 V_3,
               [4] int32 V_4)
      .line 6,6 : 9,25
      IL_0000:  nop
      IL_0001:  newobj     instance void IfThenElse01/M/f5@5::.ctor()
      IL_0006:  stloc.0
      .line 6,6 : 9,25
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   10
      IL_000a:  ldc.i4.s   10
      IL_000c:  ldc.i4.s   97
      IL_000e:  ldc.i4.s   98
      IL_0010:  stloc.1
      IL_0011:  stloc.2
      IL_0012:  stloc.3
      IL_0013:  stloc.s    V_4
      IL_0015:  callvirt   instance object [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc::Specialize<char>()
      IL_001a:  unbox.any  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>>>>
      IL_001f:  ldloc.s    V_4
      IL_0021:  ldloc.3
      IL_0022:  ldloc.2
      IL_0023:  ldloc.1
      IL_0024:  tail.
      IL_0026:  call       !!2 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::InvokeFast<char,char,char>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,!!2>>>>,
                                                                                                                              !0,
                                                                                                                              !1,
                                                                                                                              !!0,
                                                                                                                              !!1)
      IL_002b:  ret
    } // end of method M::m

  } // end of class M

} // end of class IfThenElse01

.class private abstract auto ansi sealed '<StartupCode$IfThenElse01>'.$IfThenElse01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    .line 7,7 : 4,7
    IL_0000:  nop
    IL_0001:  call       char IfThenElse01/M::m()
    IL_0006:  pop
    IL_0007:  ret
  } // end of method $IfThenElse01::.cctor

} // end of class '<StartupCode$IfThenElse01>'.$IfThenElse01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
