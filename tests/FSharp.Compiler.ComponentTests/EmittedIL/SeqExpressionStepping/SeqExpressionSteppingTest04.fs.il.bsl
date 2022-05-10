
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
.assembly SeqExpressionSteppingTest04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest04
{
  // Offset: 0x00000000 Length: 0x000002BF
  // WARNING: managed resource file FSharpSignatureData.SeqExpressionSteppingTest04 created
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest04
{
  // Offset: 0x000002C8 Length: 0x000000AE
  // WARNING: managed resource file FSharpOptimizationData.SeqExpressionSteppingTest04 created
}
.module SeqExpressionSteppingTest04.exe
// MVID: {624BD089-2A76-825A-A745-038389D04B62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03610000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest4
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest4
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f3@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y
      .field public int32 pc
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y,
                                   int32 pc,
                                   int32 current) cil managed
      {
        // Code size       36 (0x24)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::x
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::y
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::pc
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    current
        IL_0018:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::current
        IL_001d:  ldarg.0
        IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0023:  ret
      } // end of method f3@6::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       211 (0xd3)
        .maxstack  7
        .locals init (int32 V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001b,
                              IL_001e,
                              IL_0024)
        IL_0019:  br.s       IL_002a

        IL_001b:  nop
        IL_001c:  br.s       IL_008d

        IL_001e:  nop
        IL_001f:  br         IL_00b5

        IL_0024:  nop
        IL_0025:  br         IL_00ca

        IL_002a:  nop
        IL_002b:  ldarg.0
        IL_002c:  ldc.i4.0
        IL_002d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
        IL_0032:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::x
        IL_0037:  ldarg.0
        IL_0038:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::x
        IL_003d:  ldarg.0
        IL_003e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::x
        IL_0043:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_0048:  ldc.i4.1
        IL_0049:  add
        IL_004a:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.0
        IL_0051:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
        IL_0056:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::y
        IL_005b:  ldarg.0
        IL_005c:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::y
        IL_0061:  ldarg.0
        IL_0062:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::y
        IL_0067:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_006c:  ldc.i4.1
        IL_006d:  add
        IL_006e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.1
        IL_0075:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::pc
        IL_007a:  ldarg.0
        IL_007b:  ldarg.0
        IL_007c:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::x
        IL_0081:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_0086:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::current
        IL_008b:  ldc.i4.1
        IL_008c:  ret

        IL_008d:  ldarg.0
        IL_008e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::x
        IL_0093:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_0098:  ldarg.0
        IL_0099:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::y
        IL_009e:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_00a3:  add
        IL_00a4:  stloc.0
        IL_00a5:  ldarg.0
        IL_00a6:  ldc.i4.2
        IL_00a7:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::pc
        IL_00ac:  ldarg.0
        IL_00ad:  ldloc.0
        IL_00ae:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::current
        IL_00b3:  ldc.i4.1
        IL_00b4:  ret

        IL_00b5:  ldarg.0
        IL_00b6:  ldnull
        IL_00b7:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::y
        IL_00bc:  ldarg.0
        IL_00bd:  ldnull
        IL_00be:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::x
        IL_00c3:  ldarg.0
        IL_00c4:  ldc.i4.3
        IL_00c5:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::pc
        IL_00ca:  ldarg.0
        IL_00cb:  ldc.i4.0
        IL_00cc:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::current
        IL_00d1:  ldc.i4.0
        IL_00d2:  ret
      } // end of method f3@6::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.3
        IL_0002:  stfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::pc
        IL_0007:  ret
      } // end of method f3@6::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       48 (0x30)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::pc
        IL_0006:  switch     ( 
                              IL_001d,
                              IL_0020,
                              IL_0023,
                              IL_0026)
        IL_001b:  br.s       IL_0029

        IL_001d:  nop
        IL_001e:  br.s       IL_002e

        IL_0020:  nop
        IL_0021:  br.s       IL_002c

        IL_0023:  nop
        IL_0024:  br.s       IL_002a

        IL_0026:  nop
        IL_0027:  br.s       IL_002e

        IL_0029:  nop
        IL_002a:  ldc.i4.0
        IL_002b:  ret

        IL_002c:  ldc.i4.0
        IL_002d:  ret

        IL_002e:  ldc.i4.0
        IL_002f:  ret
      } // end of method f3@6::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::current
        IL_0006:  ret
      } // end of method f3@6::get_LastGenerated

      .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       10 (0xa)
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  ldnull
        IL_0002:  ldc.i4.0
        IL_0003:  ldc.i4.0
        IL_0004:  newobj     instance void SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
        IL_0009:  ret
      } // end of method f3@6::GetFreshEnumerator

    } // end of class f3@6

    .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            f3() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  newobj     instance void SeqExpressionSteppingTest4/SeqExpressionSteppingTest4/f3@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           int32,
                                                                                                           int32)
      IL_0009:  ret
    } // end of method SeqExpressionSteppingTest4::f3

  } // end of class SeqExpressionSteppingTest4

} // end of class SeqExpressionSteppingTest4

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest04>'.$SeqExpressionSteppingTest4
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
    IL_0000:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest4/SeqExpressionSteppingTest4::f3()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  pop
    IL_000d:  ret
  } // end of method $SeqExpressionSteppingTest4::main@

} // end of class '<StartupCode$SeqExpressionSteppingTest04>'.$SeqExpressionSteppingTest4


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\SeqExpressionStepping\SeqExpressionSteppingTest04_fs\SeqExpressionSteppingTest04.res
