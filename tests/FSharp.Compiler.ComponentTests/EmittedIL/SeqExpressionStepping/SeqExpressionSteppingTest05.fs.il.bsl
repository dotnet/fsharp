
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
.assembly SeqExpressionSteppingTest05
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest05
{
  // Offset: 0x00000000 Length: 0x000002BF
  // WARNING: managed resource file FSharpSignatureData.SeqExpressionSteppingTest05 created
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest05
{
  // Offset: 0x000002C8 Length: 0x000000AE
  // WARNING: managed resource file FSharpOptimizationData.SeqExpressionSteppingTest05 created
}
.module SeqExpressionSteppingTest05.exe
// MVID: {624BD0CC-3271-79F0-A745-0383CCD04B62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00CD0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest5
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest5
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f4@6
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
        IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    current
        IL_0018:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_001d:  ldarg.0
        IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0023:  ret
      } // end of method f4@6::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       251 (0xfb)
        .maxstack  7
        .locals init (int32 V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001f,
                              IL_0025,
                              IL_0028,
                              IL_002e)
        IL_001d:  br.s       IL_0034

        IL_001f:  nop
        IL_0020:  br         IL_00b5

        IL_0025:  nop
        IL_0026:  br.s       IL_0086

        IL_0028:  nop
        IL_0029:  br         IL_00ae

        IL_002e:  nop
        IL_002f:  br         IL_00f2

        IL_0034:  nop
        IL_0035:  ldarg.0
        IL_0036:  ldc.i4.0
        IL_0037:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
        IL_003c:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.1
        IL_0043:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0048:  ldarg.0
        IL_0049:  ldc.i4.0
        IL_004a:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
        IL_004f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_005a:  ldarg.0
        IL_005b:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_0060:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_0065:  ldc.i4.1
        IL_0066:  add
        IL_0067:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
        IL_006c:  ldarg.0
        IL_006d:  ldc.i4.2
        IL_006e:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0073:  ldarg.0
        IL_0074:  ldarg.0
        IL_0075:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_007a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_007f:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_0084:  ldc.i4.1
        IL_0085:  ret

        IL_0086:  ldarg.0
        IL_0087:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_008c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_0091:  ldarg.0
        IL_0092:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_0097:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_009c:  add
        IL_009d:  stloc.0
        IL_009e:  ldarg.0
        IL_009f:  ldc.i4.3
        IL_00a0:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_00a5:  ldarg.0
        IL_00a6:  ldloc.0
        IL_00a7:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_00ac:  ldc.i4.1
        IL_00ad:  ret

        IL_00ae:  ldarg.0
        IL_00af:  ldnull
        IL_00b0:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_00b5:  ldarg.0
        IL_00b6:  ldc.i4.4
        IL_00b7:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_00bc:  ldarg.0
        IL_00bd:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_00c2:  ldarg.0
        IL_00c3:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_00c8:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_00cd:  ldc.i4.1
        IL_00ce:  add
        IL_00cf:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
        IL_00d4:  ldstr      "done"
        IL_00d9:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_00de:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_00e3:  pop
        IL_00e4:  ldarg.0
        IL_00e5:  ldnull
        IL_00e6:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_00eb:  ldarg.0
        IL_00ec:  ldc.i4.4
        IL_00ed:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_00f2:  ldarg.0
        IL_00f3:  ldc.i4.0
        IL_00f4:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_00f9:  ldc.i4.0
        IL_00fa:  ret
      } // end of method f4@6::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       165 (0xa5)
        .maxstack  7
        .locals init (class [mscorlib]System.Exception V_0,
                 class [mscorlib]System.Exception V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0006:  ldc.i4.4
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0013)
        IL_0011:  br.s       IL_0019

        IL_0013:  nop
        IL_0014:  br         IL_009f

        IL_0019:  nop
        .try
        {
          IL_001a:  ldarg.0
          IL_001b:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
          IL_0020:  switch     ( 
                                IL_003b,
                                IL_003e,
                                IL_0041,
                                IL_0044,
                                IL_0047)
          IL_0039:  br.s       IL_004a

          IL_003b:  nop
          IL_003c:  br.s       IL_007f

          IL_003e:  nop
          IL_003f:  br.s       IL_004f

          IL_0041:  nop
          IL_0042:  br.s       IL_004e

          IL_0044:  nop
          IL_0045:  br.s       IL_004b

          IL_0047:  nop
          IL_0048:  br.s       IL_007f

          IL_004a:  nop
          IL_004b:  nop
          IL_004c:  br.s       IL_004f

          IL_004e:  nop
          IL_004f:  ldarg.0
          IL_0050:  ldc.i4.4
          IL_0051:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
          IL_0056:  ldarg.0
          IL_0057:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
          IL_005c:  ldarg.0
          IL_005d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
          IL_0062:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
          IL_0067:  ldc.i4.1
          IL_0068:  add
          IL_0069:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
          IL_006e:  ldstr      "done"
          IL_0073:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
          IL_0078:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
          IL_007d:  pop
          IL_007e:  nop
          IL_007f:  ldarg.0
          IL_0080:  ldc.i4.4
          IL_0081:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
          IL_0086:  ldarg.0
          IL_0087:  ldc.i4.0
          IL_0088:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
          IL_008d:  leave.s    IL_0099

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_008f:  castclass  [mscorlib]System.Exception
          IL_0094:  stloc.1
          IL_0095:  ldloc.1
          IL_0096:  stloc.0
          IL_0097:  leave.s    IL_0099

        }  // end handler
        IL_0099:  nop
        IL_009a:  br         IL_0000

        IL_009f:  ldloc.0
        IL_00a0:  brfalse.s  IL_00a4

        IL_00a2:  ldloc.0
        IL_00a3:  throw

        IL_00a4:  ret
      } // end of method f4@6::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       57 (0x39)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0006:  switch     ( 
                              IL_0021,
                              IL_0024,
                              IL_0027,
                              IL_002a,
                              IL_002d)
        IL_001f:  br.s       IL_0030

        IL_0021:  nop
        IL_0022:  br.s       IL_0037

        IL_0024:  nop
        IL_0025:  br.s       IL_0035

        IL_0027:  nop
        IL_0028:  br.s       IL_0033

        IL_002a:  nop
        IL_002b:  br.s       IL_0031

        IL_002d:  nop
        IL_002e:  br.s       IL_0037

        IL_0030:  nop
        IL_0031:  ldc.i4.1
        IL_0032:  ret

        IL_0033:  ldc.i4.1
        IL_0034:  ret

        IL_0035:  ldc.i4.1
        IL_0036:  ret

        IL_0037:  ldc.i4.0
        IL_0038:  ret
      } // end of method f4@6::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_0006:  ret
      } // end of method f4@6::get_LastGenerated

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
        IL_0004:  newobj     instance void SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
        IL_0009:  ret
      } // end of method f4@6::GetFreshEnumerator

    } // end of class f4@6

    .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            f4() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  newobj     instance void SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           int32,
                                                                                                           int32)
      IL_0009:  ret
    } // end of method SeqExpressionSteppingTest5::f4

  } // end of class SeqExpressionSteppingTest5

} // end of class SeqExpressionSteppingTest5

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest05>'.$SeqExpressionSteppingTest5
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
    IL_0000:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5::f4()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  pop
    IL_000d:  ret
  } // end of method $SeqExpressionSteppingTest5::main@

} // end of class '<StartupCode$SeqExpressionSteppingTest05>'.$SeqExpressionSteppingTest5


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\SeqExpressionStepping\SeqExpressionSteppingTest05_fs\SeqExpressionSteppingTest05.res
