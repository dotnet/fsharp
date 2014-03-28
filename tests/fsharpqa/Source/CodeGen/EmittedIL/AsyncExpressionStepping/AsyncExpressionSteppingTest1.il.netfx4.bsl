
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.16774
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
  .ver 4:0:0:0
}
.assembly AsyncExpressionSteppingTest1
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.AsyncExpressionSteppingTest1
{
  // Offset: 0x00000000 Length: 0x00000289
}
.mresource public FSharpOptimizationData.AsyncExpressionSteppingTest1
{
  // Offset: 0x00000290 Length: 0x000000B1
}
.module AsyncExpressionSteppingTest1.dll
// MVID: {4DABFC7F-6394-B5D4-A745-03837FFCAB4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000280000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed AsyncExpressionSteppingTest1
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public AsyncExpressionSteppingTest1
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable nested assembly beforefieldinit f1@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest1/AsyncExpressionSteppingTest1/f1@6::builder@
        IL_000d:  ret
      } // end of method f1@6::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        // Code size       63 (0x3f)
        .maxstack  8
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 6,6 : 17,32 
        IL_0000:  nop
        IL_0001:  ldstr      "hello"
        IL_0006:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0010:  pop
        .line 7,7 : 17,46 
        IL_0011:  ldstr      "stuck in the middle"
        IL_0016:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_001b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0020:  pop
        .line 8,8 : 17,34 
        IL_0021:  ldstr      "goodbye"
        IL_0026:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_002b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0030:  pop
        IL_0031:  ldarg.0
        IL_0032:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest1/AsyncExpressionSteppingTest1/f1@6::builder@
        IL_0037:  tail.
        IL_0039:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Zero()
        IL_003e:  ret
      } // end of method f1@6::Invoke

    } // end of class f1@6

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
            f1() cil managed
    {
      // Code size       22 (0x16)
      .maxstack  4
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@)
      .line 6,6 : 9,14 
      IL_0000:  nop
      IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldloc.0
      IL_0009:  newobj     instance void AsyncExpressionSteppingTest1/AsyncExpressionSteppingTest1/f1@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
      IL_000e:  tail.
      IL_0010:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
      IL_0015:  ret
    } // end of method AsyncExpressionSteppingTest1::f1

  } // end of class AsyncExpressionSteppingTest1

} // end of class AsyncExpressionSteppingTest1

.class private abstract auto ansi sealed '<StartupCode$AsyncExpressionSteppingTest1>'.$AsyncExpressionSteppingTest1
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       19 (0x13)
    .maxstack  5
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_1)
    .line 10,10 : 13,43 
    IL_0000:  nop
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> AsyncExpressionSteppingTest1/AsyncExpressionSteppingTest1::f1()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldnull
    IL_000b:  ldnull
    IL_000c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync::RunSynchronously<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>,
                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<valuetype [mscorlib]System.Threading.CancellationToken>)
    IL_0011:  pop
    IL_0012:  ret
  } // end of method $AsyncExpressionSteppingTest1::.cctor

} // end of class '<StartupCode$AsyncExpressionSteppingTest1>'.$AsyncExpressionSteppingTest1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
