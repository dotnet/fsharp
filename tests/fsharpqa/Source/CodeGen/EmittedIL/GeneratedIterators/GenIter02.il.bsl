
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
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
  .ver 5:0:0:0
}
.assembly GenIter02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenIter02
{
  // Offset: 0x00000000 Length: 0x000001F4
}
.mresource public FSharpOptimizationData.GenIter02
{
  // Offset: 0x000001F8 Length: 0x0000007B
}
.module GenIter02.exe
// MVID: {60B68B7E-F857-DC98-A745-03837E8BB660}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06720000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenIter02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname squaresOfOneToTenB@5
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter02/squaresOfOneToTenB@5::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 GenIter02/squaresOfOneToTenB@5::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 GenIter02/squaresOfOneToTenB@5::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method squaresOfOneToTenB@5::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       170 (0xaa)
      .maxstack  8
      .locals init ([0] int32 x)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\GeneratedIterators\\GenIter02.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter02/squaresOfOneToTenB@5::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_0080

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_007d

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br         IL_00a1

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      .line 5,7 : 7,23 ''
      IL_0028:  ldarg.0
      IL_0029:  ldc.i4.0
      IL_002a:  ldc.i4.1
      IL_002b:  ldc.i4.2
      IL_002c:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0031:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0036:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter02/squaresOfOneToTenB@5::'enum'
      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.1
      IL_003d:  stfld      int32 GenIter02/squaresOfOneToTenB@5::pc
      .line 5,7 : 7,23 ''
      IL_0042:  ldarg.0
      IL_0043:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter02/squaresOfOneToTenB@5::'enum'
      IL_0048:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_004d:  brfalse.s  IL_0080

      IL_004f:  ldarg.0
      IL_0050:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter02/squaresOfOneToTenB@5::'enum'
      IL_0055:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005a:  stloc.0
      .line 6,6 : 12,27 ''
      IL_005b:  ldstr      "hello"
      IL_0060:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_0065:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_006a:  pop
      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.2
      IL_006d:  stfld      int32 GenIter02/squaresOfOneToTenB@5::pc
      .line 7,7 : 12,23 ''
      IL_0072:  ldarg.0
      IL_0073:  ldloc.0
      IL_0074:  ldloc.0
      IL_0075:  mul
      IL_0076:  stfld      int32 GenIter02/squaresOfOneToTenB@5::current
      IL_007b:  ldc.i4.1
      IL_007c:  ret

      .line 100001,100001 : 0,0 ''
      IL_007d:  nop
      IL_007e:  br.s       IL_0042

      IL_0080:  ldarg.0
      IL_0081:  ldc.i4.3
      IL_0082:  stfld      int32 GenIter02/squaresOfOneToTenB@5::pc
      .line 5,7 : 7,23 ''
      IL_0087:  ldarg.0
      IL_0088:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter02/squaresOfOneToTenB@5::'enum'
      IL_008d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0092:  nop
      IL_0093:  ldarg.0
      IL_0094:  ldnull
      IL_0095:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter02/squaresOfOneToTenB@5::'enum'
      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 GenIter02/squaresOfOneToTenB@5::pc
      IL_00a1:  ldarg.0
      IL_00a2:  ldc.i4.0
      IL_00a3:  stfld      int32 GenIter02/squaresOfOneToTenB@5::current
      IL_00a8:  ldc.i4.0
      IL_00a9:  ret
    } // end of method squaresOfOneToTenB@5::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       133 (0x85)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter02/squaresOfOneToTenB@5::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br.s       IL_007c

      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 GenIter02/squaresOfOneToTenB@5::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 GenIter02/squaresOfOneToTenB@5::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter02/squaresOfOneToTenB@5::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 GenIter02/squaresOfOneToTenB@5::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 GenIter02/squaresOfOneToTenB@5::current
        IL_0064:  ldnull
        IL_0065:  stloc.1
        IL_0066:  leave.s    IL_0074

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0068:  castclass  [mscorlib]System.Exception
        IL_006d:  stloc.2
        .line 5,7 : 7,23 ''
        IL_006e:  ldloc.2
        IL_006f:  stloc.0
        IL_0070:  ldnull
        IL_0071:  stloc.1
        IL_0072:  leave.s    IL_0074

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0074:  ldloc.1
      IL_0075:  pop
      .line 100001,100001 : 0,0 ''
      IL_0076:  nop
      IL_0077:  br         IL_0000

      IL_007c:  ldloc.0
      IL_007d:  ldnull
      IL_007e:  cgt.un
      IL_0080:  brfalse.s  IL_0084

      .line 100001,100001 : 0,0 ''
      IL_0082:  ldloc.0
      IL_0083:  throw

      .line 100001,100001 : 0,0 ''
      IL_0084:  ret
    } // end of method squaresOfOneToTenB@5::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter02/squaresOfOneToTenB@5::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method squaresOfOneToTenB@5::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter02/squaresOfOneToTenB@5::current
      IL_0006:  ret
    } // end of method squaresOfOneToTenB@5::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void GenIter02/squaresOfOneToTenB@5::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                               int32,
                                                                               int32)
      IL_0008:  ret
    } // end of method squaresOfOneToTenB@5::GetFreshEnumerator

  } // end of class squaresOfOneToTenB@5

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          squaresOfOneToTenB() cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    .line 5,7 : 5,25 ''
    IL_0000:  ldnull
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  newobj     instance void GenIter02/squaresOfOneToTenB@5::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                             int32,
                                                                             int32)
    IL_0008:  tail.
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000f:  ret
  } // end of method GenIter02::squaresOfOneToTenB

} // end of class GenIter02

.class private abstract auto ansi sealed '<StartupCode$GenIter02>'.$GenIter02
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $GenIter02::main@

} // end of class '<StartupCode$GenIter02>'.$GenIter02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
