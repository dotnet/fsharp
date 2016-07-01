
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.81.0
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
  .ver 4:4:1:0
}
.assembly AsyncExpressionSteppingTest3
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.AsyncExpressionSteppingTest3
{
  // Offset: 0x00000000 Length: 0x0000026F
}
.mresource public FSharpOptimizationData.AsyncExpressionSteppingTest3
{
  // Offset: 0x00000278 Length: 0x000000B1
}
.module AsyncExpressionSteppingTest3.dll
// MVID: {5775B145-6394-F35E-A745-038345B17557}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00D30000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed AsyncExpressionSteppingTest3
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public AsyncExpressionSteppingTest3
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable nested assembly beforefieldinit f3@5
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest3/AsyncExpressionSteppingTest3/f3@5::builder@
        IL_000d:  ret
      } // end of method f3@5::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        // Code size       58 (0x3a)
        .maxstack  6
        .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
                 [1] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y,
                 [2] int32 z)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 5,5 : 17,30 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\AsyncExpressionStepping\\AsyncExpressionSteppingTest3.fs'
        IL_0000:  nop
        IL_0001:  ldc.i4.0
        IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0007:  stloc.0
        .line 6,6 : 17,23 ''
        IL_0008:  ldloc.0
        IL_0009:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_000e:  nop
        .line 7,7 : 17,30 ''
        IL_000f:  ldc.i4.0
        IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0015:  stloc.1
        .line 8,8 : 17,23 ''
        IL_0016:  ldloc.1
        IL_0017:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_001c:  nop
        .line 9,9 : 17,32 ''
        IL_001d:  ldloc.0
        IL_001e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0023:  ldloc.1
        IL_0024:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0029:  add
        IL_002a:  stloc.2
        .line 10,10 : 17,25 ''
        IL_002b:  ldarg.0
        IL_002c:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest3/AsyncExpressionSteppingTest3/f3@5::builder@
        IL_0031:  ldloc.2
        IL_0032:  tail.
        IL_0034:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Return<int32>(!!0)
        IL_0039:  ret
      } // end of method f3@5::Invoke

    } // end of class f3@5

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
            f3() cil managed
    {
      // Code size       22 (0x16)
      .maxstack  4
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@)
      .line 5,5 : 9,14 ''
      IL_0000:  nop
      IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldloc.0
      IL_0009:  newobj     instance void AsyncExpressionSteppingTest3/AsyncExpressionSteppingTest3/f3@5::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
      IL_000e:  tail.
      IL_0010:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
      IL_0015:  ret
    } // end of method AsyncExpressionSteppingTest3::f3

  } // end of class AsyncExpressionSteppingTest3

} // end of class AsyncExpressionSteppingTest3

.class private abstract auto ansi sealed '<StartupCode$AsyncExpressionSteppingTest3>'.$AsyncExpressionSteppingTest3
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
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_1)
    .line 12,12 : 13,43 ''
    IL_0000:  nop
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest3/AsyncExpressionSteppingTest3::f3()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldnull
    IL_000b:  ldnull
    IL_000c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync::RunSynchronously<int32>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>,
                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<valuetype [mscorlib]System.Threading.CancellationToken>)
    IL_0011:  pop
    IL_0012:  ret
  } // end of method $AsyncExpressionSteppingTest3::.cctor

} // end of class '<StartupCode$AsyncExpressionSteppingTest3>'.$AsyncExpressionSteppingTest3


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
