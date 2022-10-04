
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
.assembly SeqExpressionSteppingTest01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest01
{
  // Offset: 0x00000000 Length: 0x000002BF
  // WARNING: managed resource file FSharpSignatureData.SeqExpressionSteppingTest01 created
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest01
{
  // Offset: 0x000002C8 Length: 0x000000AE
  // WARNING: managed resource file FSharpOptimizationData.SeqExpressionSteppingTest01 created
}
.module SeqExpressionSteppingTest01.exe
// MVID: {624BCCE6-AD85-430C-A745-0383E6CC4B62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00C30000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest1
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest1
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f0@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public int32 pc
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(int32 pc,
                                   int32 current) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::pc
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::current
        IL_000e:  ldarg.0
        IL_000f:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0014:  ret
      } // end of method f0@6::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       62 (0x3e)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0017,
                              IL_001a)
        IL_0015:  br.s       IL_001d

        IL_0017:  nop
        IL_0018:  br.s       IL_002e

        IL_001a:  nop
        IL_001b:  br.s       IL_0035

        IL_001d:  nop
        IL_001e:  ldarg.0
        IL_001f:  ldc.i4.1
        IL_0020:  stfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::pc
        IL_0025:  ldarg.0
        IL_0026:  ldc.i4.1
        IL_0027:  stfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::current
        IL_002c:  ldc.i4.1
        IL_002d:  ret

        IL_002e:  ldarg.0
        IL_002f:  ldc.i4.2
        IL_0030:  stfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::pc
        IL_0035:  ldarg.0
        IL_0036:  ldc.i4.0
        IL_0037:  stfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::current
        IL_003c:  ldc.i4.0
        IL_003d:  ret
      } // end of method f0@6::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.2
        IL_0002:  stfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::pc
        IL_0007:  ret
      } // end of method f0@6::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       39 (0x27)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::pc
        IL_0006:  switch     ( 
                              IL_0019,
                              IL_001c,
                              IL_001f)
        IL_0017:  br.s       IL_0022

        IL_0019:  nop
        IL_001a:  br.s       IL_0025

        IL_001c:  nop
        IL_001d:  br.s       IL_0023

        IL_001f:  nop
        IL_0020:  br.s       IL_0025

        IL_0022:  nop
        IL_0023:  ldc.i4.0
        IL_0024:  ret

        IL_0025:  ldc.i4.0
        IL_0026:  ret
      } // end of method f0@6::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::current
        IL_0006:  ret
      } // end of method f0@6::get_LastGenerated

      .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldc.i4.0
        IL_0001:  ldc.i4.0
        IL_0002:  newobj     instance void SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::.ctor(int32,
                                                                                                             int32)
        IL_0007:  ret
      } // end of method f0@6::GetFreshEnumerator

    } // end of class f0@6

    .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            f0() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ldc.i4.0
      IL_0002:  newobj     instance void SeqExpressionSteppingTest1/SeqExpressionSteppingTest1/f0@6::.ctor(int32,
                                                                                                           int32)
      IL_0007:  ret
    } // end of method SeqExpressionSteppingTest1::f0

  } // end of class SeqExpressionSteppingTest1

} // end of class SeqExpressionSteppingTest1

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest01>'.$SeqExpressionSteppingTest1
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       14 (0xe)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_0)
    IL_0000:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest1/SeqExpressionSteppingTest1::f0()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  pop
    IL_000d:  ret
  } // end of method $SeqExpressionSteppingTest1::main@

} // end of class '<StartupCode$SeqExpressionSteppingTest01>'.$SeqExpressionSteppingTest1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\SeqExpressionStepping\SeqExpressionSteppingTest01_fs\SeqExpressionSteppingTest01.res
