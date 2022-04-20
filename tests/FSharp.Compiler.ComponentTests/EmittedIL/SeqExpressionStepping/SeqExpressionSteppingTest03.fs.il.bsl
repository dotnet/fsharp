
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
.assembly SeqExpressionSteppingTest03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest03
{
  // Offset: 0x00000000 Length: 0x000002D1
  // WARNING: managed resource file FSharpSignatureData.SeqExpressionSteppingTest03 created
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest03
{
  // Offset: 0x000002D8 Length: 0x000000AE
  // WARNING: managed resource file FSharpOptimizationData.SeqExpressionSteppingTest03 created
}
.module SeqExpressionSteppingTest03.exe
// MVID: {624FDC53-73FA-1BE4-A745-038353DC4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000001F9CDAC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest3
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest3
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f2@7
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .field public int32 pc
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
                                   int32 pc,
                                   int32 current) cil managed
      {
        // Code size       28 (0x1c)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::x
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::pc
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::current
        IL_0015:  ldarg.0
        IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_001b:  ret
      } // end of method f2@7::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       128 (0x80)
        .maxstack  7
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0017,
                              IL_001a)
        IL_0015:  br.s       IL_001d

        IL_0017:  nop
        IL_0018:  br.s       IL_0062

        IL_001a:  nop
        IL_001b:  br.s       IL_0077

        IL_001d:  nop
        IL_001e:  br.s       IL_0062

        IL_0020:  ldarg.0
        IL_0021:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::x
        IL_0026:  ldarg.0
        IL_0027:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::x
        IL_002c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_0031:  ldc.i4.1
        IL_0032:  add
        IL_0033:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
        IL_0038:  ldstr      "hello"
        IL_003d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0042:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0047:  pop
        IL_0048:  ldarg.0
        IL_0049:  ldc.i4.1
        IL_004a:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::pc
        IL_004f:  ldarg.0
        IL_0050:  ldarg.0
        IL_0051:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::x
        IL_0056:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_005b:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::current
        IL_0060:  ldc.i4.1
        IL_0061:  ret

        IL_0062:  ldarg.0
        IL_0063:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::x
        IL_0068:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_006d:  ldc.i4.4
        IL_006e:  blt.s      IL_0020

        IL_0070:  ldarg.0
        IL_0071:  ldc.i4.2
        IL_0072:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::pc
        IL_0077:  ldarg.0
        IL_0078:  ldc.i4.0
        IL_0079:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::current
        IL_007e:  ldc.i4.0
        IL_007f:  ret
      } // end of method f2@7::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.2
        IL_0002:  stfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::pc
        IL_0007:  ret
      } // end of method f2@7::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       39 (0x27)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::pc
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
      } // end of method f2@7::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::current
        IL_0006:  ret
      } // end of method f2@7::get_LastGenerated

      .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::x
        IL_0006:  ldc.i4.0
        IL_0007:  ldc.i4.0
        IL_0008:  newobj     instance void SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
        IL_000d:  ret
      } // end of method f2@7::GetFreshEnumerator

    } // end of class f2@7

    .method public static class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> 
            f2() cil managed
    {
      // Code size       16 (0x10)
      .maxstack  5
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.0
      IL_0009:  ldc.i4.0
      IL_000a:  newobj     instance void SeqExpressionSteppingTest3/SeqExpressionSteppingTest3/f2@7::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           int32,
                                                                                                           int32)
      IL_000f:  ret
    } // end of method SeqExpressionSteppingTest3::f2

  } // end of class SeqExpressionSteppingTest3

} // end of class SeqExpressionSteppingTest3

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest03>'.$SeqExpressionSteppingTest3
       extends [System.Runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       14 (0xe)
    .maxstack  3
    .locals init (class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_0)
    IL_0000:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest3/SeqExpressionSteppingTest3::f2()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  pop
    IL_000d:  ret
  } // end of method $SeqExpressionSteppingTest3::main@

} // end of class '<StartupCode$SeqExpressionSteppingTest03>'.$SeqExpressionSteppingTest3


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\SeqExpressionStepping\SeqExpressionSteppingTest03_fs\SeqExpressionSteppingTest03.res
