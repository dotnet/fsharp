
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (A1 90 89 B1 C7 4D 08 09 )                         // .....M..
  .ver 1:9:7:8
}
.assembly AsyncExpressionSteppingTest2
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.AsyncExpressionSteppingTest2
{
  // Offset: 0x00000000 Length: 0x000002AB
}
.mresource public FSharpOptimizationData.AsyncExpressionSteppingTest2
{
  // Offset: 0x000002B0 Length: 0x000000B1
}
.module AsyncExpressionSteppingTest2.dll
// MVID: {4ACEB5C8-6394-D499-A745-0383C8B5CE4A}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000002730000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed AsyncExpressionSteppingTest2
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public AsyncExpressionSteppingTest2
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable nested assembly beforefieldinit 'f2@5-1'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x) cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/'f2@5-1'::x
        IL_000d:  ret
      } // end of method 'f2@5-1'::.ctor

      .method public strict virtual instance bool 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        // Code size       16 (0x10)
        .maxstack  6
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 5,5 : 23,29 
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/'f2@5-1'::x
        IL_0007:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_000c:  ldc.i4.4
        IL_000d:  clt
        IL_000f:  ret
      } // end of method 'f2@5-1'::Invoke

    } // end of class 'f2@5-1'

    .class auto ansi serializable nested assembly beforefieldinit 'f2@6-2'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
                                   class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/'f2@6-2'::x
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/'f2@6-2'::builder@
        IL_0014:  ret
      } // end of method 'f2@6-2'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        // Code size       43 (0x2b)
        .maxstack  5
        .line 6,6 : 20,26 
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/'f2@6-2'::x
        IL_0007:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_000c:  nop
        .line 7,7 : 20,35 
        IL_000d:  ldstr      "hello"
        IL_0012:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0017:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_001c:  pop
        IL_001d:  ldarg.0
        IL_001e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/'f2@6-2'::builder@
        IL_0023:  tail.
        IL_0025:  call       instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Zero()
        IL_002a:  ret
      } // end of method 'f2@6-2'::Invoke

    } // end of class 'f2@6-2'

    .class auto ansi serializable nested assembly beforefieldinit f2@5
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
                                   class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/f2@5::x
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/f2@5::builder@
        IL_0014:  ret
      } // end of method f2@5::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        // Code size       54 (0x36)
        .maxstack  9
        .line 5,5 : 17,29 
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/f2@5::builder@
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/f2@5::x
        IL_000d:  newobj     instance void AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/'f2@5-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0012:  ldarg.0
        IL_0013:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/f2@5::builder@
        IL_0018:  ldarg.0
        IL_0019:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/f2@5::x
        IL_001e:  ldarg.0
        IL_001f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/f2@5::builder@
        IL_0024:  newobj     instance void AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/'f2@6-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
        IL_0029:  call       instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_002e:  tail.
        IL_0030:  call       instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::While(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>,
                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0035:  ret
      } // end of method f2@5::Invoke

    } // end of class f2@5

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
            f2() cil managed
    {
      // Code size       30 (0x1e)
      .maxstack  5
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
               [1] class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@)
      .line 4,4 : 9,22 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
      IL_0007:  stloc.0
      .line 5,5 : 9,14 
      IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  ldloc.0
      IL_0010:  ldloc.1
      IL_0011:  newobj     instance void AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2/f2@5::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                               class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
      IL_0016:  tail.
      IL_0018:  call       instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
      IL_001d:  ret
    } // end of method AsyncExpressionSteppingTest2::f2

  } // end of class AsyncExpressionSteppingTest2

} // end of class AsyncExpressionSteppingTest2

.class private abstract auto ansi sealed '<StartupCode$AsyncExpressionSteppingTest2>'.$AsyncExpressionSteppingTest2
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
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> AsyncExpressionSteppingTest2/AsyncExpressionSteppingTest2::f2()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldnull
    IL_000b:  ldnull
    IL_000c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync::RunSynchronously<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>,
                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<valuetype [FSharp.Core]System.Threading.CancellationToken>)
    IL_0011:  pop
    IL_0012:  ret
  } // end of method $AsyncExpressionSteppingTest2::.cctor

} // end of class '<StartupCode$AsyncExpressionSteppingTest2>'.$AsyncExpressionSteppingTest2


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
