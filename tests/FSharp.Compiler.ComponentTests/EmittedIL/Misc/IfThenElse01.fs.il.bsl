
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
.assembly IfThenElse01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.IfThenElse01
{
  // Offset: 0x00000000 Length: 0x00000273
  // WARNING: managed resource file FSharpSignatureData.IfThenElse01 created
}
.mresource public FSharpOptimizationData.IfThenElse01
{
  // Offset: 0x00000278 Length: 0x00000092
  // WARNING: managed resource file FSharpOptimizationData.IfThenElse01 created
}
.module IfThenElse01.exe
// MVID: {624E1220-A4B0-91C2-A745-038320124E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04FA0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed IfThenElse01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public M
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit f5@5
           extends [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc
    {
      .field static assembly initonly class IfThenElse01/M/f5@5 @_instance
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

      .method private specialname rtspecialname static 
              void  .cctor() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       11 (0xb)
        .maxstack  10
        IL_0000:  newobj     instance void IfThenElse01/M/f5@5::.ctor()
        IL_0005:  stsfld     class IfThenElse01/M/f5@5 IfThenElse01/M/f5@5::@_instance
        IL_000a:  ret
      } // end of method f5@5::.cctor

    } // end of class f5@5

    .class auto ansi serializable sealed nested assembly beforefieldinit f5@5T<a>
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
        // Code size       17 (0x11)
        .maxstack  7
        .locals init (class IfThenElse01/M/f5@5 V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class IfThenElse01/M/f5@5 class IfThenElse01/M/f5@5T<!a>::self0@
        IL_0006:  stloc.0
        IL_0007:  nop
        IL_0008:  ldarg.1
        IL_0009:  ldarg.2
        IL_000a:  ble.s      IL_000e

        IL_000c:  ldarg.3
        IL_000d:  ret

        IL_000e:  ldarg.s    w
        IL_0010:  ret
      } // end of method f5@5T::Invoke

    } // end of class f5@5T

    .method public static char  m() cil managed
    {
      // Code size       43 (0x2b)
      .maxstack  7
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc V_0,
               char V_1,
               char V_2,
               int32 V_3,
               int32 V_4)
      IL_0000:  ldsfld     class IfThenElse01/M/f5@5 IfThenElse01/M/f5@5::@_instance
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldc.i4.s   10
      IL_0009:  ldc.i4.s   10
      IL_000b:  ldc.i4.s   97
      IL_000d:  ldc.i4.s   98
      IL_000f:  stloc.1
      IL_0010:  stloc.2
      IL_0011:  stloc.3
      IL_0012:  stloc.s    V_4
      IL_0014:  callvirt   instance object [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc::Specialize<char>()
      IL_0019:  unbox.any  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>>>>
      IL_001e:  ldloc.s    V_4
      IL_0020:  ldloc.3
      IL_0021:  ldloc.2
      IL_0022:  ldloc.1
      IL_0023:  tail.
      IL_0025:  call       !!2 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::InvokeFast<char,char,char>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,!!2>>>>,
                                                                                                                              !0,
                                                                                                                              !1,
                                                                                                                              !!0,
                                                                                                                              !!1)
      IL_002a:  ret
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
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  call       char IfThenElse01/M::m()
    IL_0005:  pop
    IL_0006:  ret
  } // end of method $IfThenElse01::main@

} // end of class '<StartupCode$IfThenElse01>'.$IfThenElse01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\kevinransom\fsharp\tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\IfThenElse01_fs\IfThenElse01.res
