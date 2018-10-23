
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
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
  .ver 4:5:0:0
}
.assembly GenIter03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenIter03
{
  // Offset: 0x00000000 Length: 0x00000200
}
.mresource public FSharpOptimizationData.GenIter03
{
  // Offset: 0x00000208 Length: 0x0000007B
}
.module GenIter03.exe
// MVID: {5B9A6329-F77C-DC98-A745-038329639A5B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x026B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenIter03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname squaresOfOneToTenC@4
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
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter03/squaresOfOneToTenC@4::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 GenIter03/squaresOfOneToTenC@4::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 GenIter03/squaresOfOneToTenC@4::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method squaresOfOneToTenC@4::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       158 (0x9e)
      .maxstack  8
      .locals init ([0] int32 x)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\GeneratedIterators\\GenIter03.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter03/squaresOfOneToTenC@4::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002a

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0074

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0071

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      .line 4,4 : 30,55 ''
      IL_002b:  ldarg.0
      IL_002c:  ldc.i4.0
      IL_002d:  ldc.i4.1
      IL_002e:  ldc.i4.s   10
      IL_0030:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0035:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter03/squaresOfOneToTenC@4::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 GenIter03/squaresOfOneToTenC@4::pc
      .line 4,4 : 30,55 ''
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter03/squaresOfOneToTenC@4::'enum'
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0074

      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter03/squaresOfOneToTenC@4::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stloc.0
      IL_005f:  ldarg.0
      IL_0060:  ldc.i4.2
      IL_0061:  stfld      int32 GenIter03/squaresOfOneToTenC@4::pc
      .line 4,4 : 50,55 ''
      IL_0066:  ldarg.0
      IL_0067:  ldloc.0
      IL_0068:  ldloc.0
      IL_0069:  mul
      IL_006a:  stfld      int32 GenIter03/squaresOfOneToTenC@4::current
      IL_006f:  ldc.i4.1
      IL_0070:  ret

      .line 100001,100001 : 0,0 ''
      IL_0071:  nop
      IL_0072:  br.s       IL_0046

      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.3
      IL_0076:  stfld      int32 GenIter03/squaresOfOneToTenC@4::pc
      .line 4,4 : 30,55 ''
      IL_007b:  ldarg.0
      IL_007c:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter03/squaresOfOneToTenC@4::'enum'
      IL_0081:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0086:  nop
      IL_0087:  ldarg.0
      IL_0088:  ldnull
      IL_0089:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter03/squaresOfOneToTenC@4::'enum'
      IL_008e:  ldarg.0
      IL_008f:  ldc.i4.3
      IL_0090:  stfld      int32 GenIter03/squaresOfOneToTenC@4::pc
      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.0
      IL_0097:  stfld      int32 GenIter03/squaresOfOneToTenC@4::current
      IL_009c:  ldc.i4.0
      IL_009d:  ret
    } // end of method squaresOfOneToTenC@4::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter03/squaresOfOneToTenC@4::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0019

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br         IL_0087

      .line 100001,100001 : 0,0 ''
      IL_0019:  nop
      .try
      {
        IL_001a:  ldarg.0
        IL_001b:  ldfld      int32 GenIter03/squaresOfOneToTenC@4::pc
        IL_0020:  switch     ( 
                              IL_0037,
                              IL_0039,
                              IL_003b,
                              IL_003d)
        IL_0035:  br.s       IL_004b

        IL_0037:  br.s       IL_003f

        IL_0039:  br.s       IL_0042

        IL_003b:  br.s       IL_0045

        IL_003d:  br.s       IL_0048

        .line 100001,100001 : 0,0 ''
        IL_003f:  nop
        IL_0040:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_0042:  nop
        IL_0043:  br.s       IL_004d

        .line 100001,100001 : 0,0 ''
        IL_0045:  nop
        IL_0046:  br.s       IL_004c

        .line 100001,100001 : 0,0 ''
        IL_0048:  nop
        IL_0049:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_004b:  nop
        .line 100001,100001 : 0,0 ''
        IL_004c:  nop
        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.3
        IL_004f:  stfld      int32 GenIter03/squaresOfOneToTenC@4::pc
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GenIter03/squaresOfOneToTenC@4::'enum'
        IL_005a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_005f:  nop
        .line 100001,100001 : 0,0 ''
        IL_0060:  nop
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.3
        IL_0063:  stfld      int32 GenIter03/squaresOfOneToTenC@4::pc
        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.0
        IL_006a:  stfld      int32 GenIter03/squaresOfOneToTenC@4::current
        IL_006f:  ldnull
        IL_0070:  stloc.1
        IL_0071:  leave.s    IL_007f

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0073:  castclass  [mscorlib]System.Exception
        IL_0078:  stloc.2
        .line 4,4 : 30,55 ''
        IL_0079:  ldloc.2
        IL_007a:  stloc.0
        IL_007b:  ldnull
        IL_007c:  stloc.1
        IL_007d:  leave.s    IL_007f

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_007f:  ldloc.1
      IL_0080:  pop
      .line 100001,100001 : 0,0 ''
      IL_0081:  nop
      IL_0082:  br         IL_0000

      IL_0087:  ldloc.0
      IL_0088:  ldnull
      IL_0089:  cgt.un
      IL_008b:  brfalse.s  IL_008f

      IL_008d:  br.s       IL_0091

      IL_008f:  br.s       IL_0093

      .line 100001,100001 : 0,0 ''
      IL_0091:  ldloc.0
      IL_0092:  throw

      .line 100001,100001 : 0,0 ''
      IL_0093:  ret
    } // end of method squaresOfOneToTenC@4::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter03/squaresOfOneToTenC@4::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001b:  br.s       IL_0031

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0028

      IL_0021:  br.s       IL_002b

      IL_0023:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0025:  nop
      IL_0026:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0028:  nop
      IL_0029:  br.s       IL_0034

      .line 100001,100001 : 0,0 ''
      IL_002b:  nop
      IL_002c:  br.s       IL_0032

      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0036

      .line 100001,100001 : 0,0 ''
      IL_0031:  nop
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method squaresOfOneToTenC@4::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 GenIter03/squaresOfOneToTenC@4::current
      IL_0006:  ret
    } // end of method squaresOfOneToTenC@4::get_LastGenerated

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
      IL_0003:  newobj     instance void GenIter03/squaresOfOneToTenC@4::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                               int32,
                                                                               int32)
      IL_0008:  ret
    } // end of method squaresOfOneToTenC@4::GetFreshEnumerator

  } // end of class squaresOfOneToTenC@4

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          squaresOfOneToTenC() cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    .line 4,4 : 28,57 ''
    IL_0000:  ldnull
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  newobj     instance void GenIter03/squaresOfOneToTenC@4::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                             int32,
                                                                             int32)
    IL_0008:  tail.
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000f:  ret
  } // end of method GenIter03::squaresOfOneToTenC

} // end of class GenIter03

.class private abstract auto ansi sealed '<StartupCode$GenIter03>'.$GenIter03
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $GenIter03::main@

} // end of class '<StartupCode$GenIter03>'.$GenIter03


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
