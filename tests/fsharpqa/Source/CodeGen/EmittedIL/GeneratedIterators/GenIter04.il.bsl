
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
.assembly GenIter04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenIter04
{
  // Offset: 0x00000000 Length: 0x000001E4
}
.mresource public FSharpOptimizationData.GenIter04
{
  // Offset: 0x000001E8 Length: 0x0000007B
}
.module GenIter04.exe
// MVID: {60B68B7E-F79D-DC98-A745-03837E8BB660}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06CC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenIter04
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname squaresOfOneToTenD@4
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
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter04/squaresOfOneToTenD@4::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 GenIter04/squaresOfOneToTenD@4::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 GenIter04/squaresOfOneToTenD@4::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method squaresOfOneToTenD@4::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       152 (0x98)
      .maxstack  8
      .locals init ([0] int32 x)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\GeneratedIterators\\GenIter04.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter04/squaresOfOneToTenD@4::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_006e

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_008f

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 4,4 : 28,53 ''
      IL_0025:  ldarg.0
      IL_0026:  ldc.i4.0
      IL_0027:  ldc.i4.1
      IL_0028:  ldc.i4.s   10
      IL_002a:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_002f:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0034:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter04/squaresOfOneToTenD@4::'enum'
      IL_0039:  ldarg.0
      IL_003a:  ldc.i4.1
      IL_003b:  stfld      int32 GenIter04/squaresOfOneToTenD@4::pc
      .line 4,4 : 28,53 ''
      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter04/squaresOfOneToTenD@4::'enum'
      IL_0046:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_004b:  brfalse.s  IL_006e

      IL_004d:  ldarg.0
      IL_004e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter04/squaresOfOneToTenD@4::'enum'
      IL_0053:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0058:  stloc.0
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 GenIter04/squaresOfOneToTenD@4::pc
      .line 4,4 : 48,53 ''
      IL_0060:  ldarg.0
      IL_0061:  ldloc.0
      IL_0062:  ldloc.0
      IL_0063:  mul
      IL_0064:  stfld      int32 GenIter04/squaresOfOneToTenD@4::current
      IL_0069:  ldc.i4.1
      IL_006a:  ret

      .line 100001,100001 : 0,0 ''
      IL_006b:  nop
      IL_006c:  br.s       IL_0040

      IL_006e:  ldarg.0
      IL_006f:  ldc.i4.3
      IL_0070:  stfld      int32 GenIter04/squaresOfOneToTenD@4::pc
      .line 4,4 : 28,53 ''
      IL_0075:  ldarg.0
      IL_0076:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter04/squaresOfOneToTenD@4::'enum'
      IL_007b:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0080:  nop
      IL_0081:  ldarg.0
      IL_0082:  ldnull
      IL_0083:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter04/squaresOfOneToTenD@4::'enum'
      IL_0088:  ldarg.0
      IL_0089:  ldc.i4.3
      IL_008a:  stfld      int32 GenIter04/squaresOfOneToTenD@4::pc
      IL_008f:  ldarg.0
      IL_0090:  ldc.i4.0
      IL_0091:  stfld      int32 GenIter04/squaresOfOneToTenD@4::current
      IL_0096:  ldc.i4.0
      IL_0097:  ret
    } // end of method squaresOfOneToTenD@4::GenerateNext

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
      IL_0001:  ldfld      int32 GenIter04/squaresOfOneToTenD@4::pc
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
        IL_0018:  ldfld      int32 GenIter04/squaresOfOneToTenD@4::pc
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
        IL_0044:  stfld      int32 GenIter04/squaresOfOneToTenD@4::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter04/squaresOfOneToTenD@4::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 GenIter04/squaresOfOneToTenD@4::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 GenIter04/squaresOfOneToTenD@4::current
        IL_0064:  ldnull
        IL_0065:  stloc.1
        IL_0066:  leave.s    IL_0074

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0068:  castclass  [mscorlib]System.Exception
        IL_006d:  stloc.2
        .line 4,4 : 28,53 ''
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
    } // end of method squaresOfOneToTenD@4::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter04/squaresOfOneToTenD@4::pc
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
    } // end of method squaresOfOneToTenD@4::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter04/squaresOfOneToTenD@4::current
      IL_0006:  ret
    } // end of method squaresOfOneToTenD@4::get_LastGenerated

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
      IL_0003:  newobj     instance void GenIter04/squaresOfOneToTenD@4::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                               int32,
                                                                               int32)
      IL_0008:  ret
    } // end of method squaresOfOneToTenD@4::GetFreshEnumerator

  } // end of class squaresOfOneToTenD@4

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_squaresOfOneToTenD() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$GenIter04>'.$GenIter04::squaresOfOneToTenD@4
    IL_0005:  ret
  } // end of method GenIter04::get_squaresOfOneToTenD

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          squaresOfOneToTenD()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> GenIter04::get_squaresOfOneToTenD()
  } // end of property GenIter04::squaresOfOneToTenD
} // end of class GenIter04

.class private abstract auto ansi sealed '<StartupCode$GenIter04>'.$GenIter04
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> squaresOfOneToTenD@4
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       21 (0x15)
    .maxstack  5
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> squaresOfOneToTenD)
    .line 4,4 : 1,55 ''
    IL_0000:  ldnull
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  newobj     instance void GenIter04/squaresOfOneToTenD@4::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                             int32,
                                                                             int32)
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000d:  dup
    IL_000e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$GenIter04>'.$GenIter04::squaresOfOneToTenD@4
    IL_0013:  stloc.0
    IL_0014:  ret
  } // end of method $GenIter04::main@

} // end of class '<StartupCode$GenIter04>'.$GenIter04


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
