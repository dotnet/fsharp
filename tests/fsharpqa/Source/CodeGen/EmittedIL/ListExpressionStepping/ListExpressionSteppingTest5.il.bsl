
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
  .ver 6:0:0:0
}
.assembly ListExpressionSteppingTest5
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionSteppingTest5
{
  // Offset: 0x00000000 Length: 0x00000269
}
.mresource public FSharpOptimizationData.ListExpressionSteppingTest5
{
  // Offset: 0x00000270 Length: 0x000000AF
}
.module ListExpressionSteppingTest5.exe
// MVID: {61F02896-CBE3-BFEA-A745-03839628F061}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05020000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest5
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest5
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f4() cil managed
    {
      // Code size       101 (0x65)
      .maxstack  4
      .locals init ([0] valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
               [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_2,
               [3] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y,
               [4] int32 z)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 6,15 : 9,30 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\ListExpressionStepping\\ListExpressionSteppingTest5.fs'
      IL_0000:  nop
      .line 6,6 : 11,24 ''
      IL_0001:  ldc.i4.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
      IL_0007:  stloc.1
      .line 7,7 : 11,14 ''
      .try
      {
        IL_0008:  nop
        .line 8,8 : 15,28 ''
        IL_0009:  ldc.i4.0
        IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_000f:  stloc.3
        .line 9,9 : 15,21 ''
        IL_0010:  ldloc.3
        IL_0011:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0016:  nop
        IL_0017:  ldloca.s   V_0
        .line 10,10 : 21,23 ''
        IL_0019:  ldloc.1
        IL_001a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_001f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
        IL_0024:  nop
        .line 11,11 : 15,30 ''
        IL_0025:  ldloc.1
        IL_0026:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_002b:  ldloc.3
        IL_002c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0031:  add
        IL_0032:  stloc.s    z
        IL_0034:  ldloca.s   V_0
        .line 12,12 : 21,22 ''
        IL_0036:  ldloc.s    z
        IL_0038:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
        IL_003d:  nop
        IL_003e:  ldnull
        IL_003f:  stloc.2
        IL_0040:  leave.s    IL_005b

        .line 13,13 : 11,18 ''
      }  // end .try
      finally
      {
        IL_0042:  nop
        .line 14,14 : 14,20 ''
        IL_0043:  ldloc.1
        IL_0044:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0049:  nop
        .line 15,15 : 14,28 ''
        IL_004a:  ldstr      "done"
        IL_004f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0054:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0059:  pop
        IL_005a:  endfinally
        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_005b:  ldloc.2
      IL_005c:  pop
      IL_005d:  ldloca.s   V_0
      IL_005f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
      IL_0064:  ret
    } // end of method ListExpressionSteppingTest5::f4

  } // end of class ListExpressionSteppingTest5

} // end of class ListExpressionSteppingTest5

.class private abstract auto ansi sealed '<StartupCode$ListExpressionSteppingTest5>'.$ListExpressionSteppingTest5
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
    .line 17,17 : 13,17 ''
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest5/ListExpressionSteppingTest5::f4()
    IL_0005:  pop
    IL_0006:  ret
  } // end of method $ListExpressionSteppingTest5::main@

} // end of class '<StartupCode$ListExpressionSteppingTest5>'.$ListExpressionSteppingTest5


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
