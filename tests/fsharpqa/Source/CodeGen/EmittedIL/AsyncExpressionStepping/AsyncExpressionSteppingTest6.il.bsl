
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.16657
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
.assembly AsyncExpressionSteppingTest6
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.AsyncExpressionSteppingTest6
{
  // Offset: 0x00000000 Length: 0x0000027D
}
.mresource public FSharpOptimizationData.AsyncExpressionSteppingTest6
{
  // Offset: 0x00000288 Length: 0x000000BE
}
.module AsyncExpressionSteppingTest6.dll
// MVID: {4D0EDE86-6394-4FAD-A745-038386DE0E4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00520000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed AsyncExpressionSteppingTest6
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public AsyncExpressionSteppingTest6
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable nested assembly beforefieldinit f2@5
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f2@5::builder@
        IL_000d:  ret
      } // end of method f2@5::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        // Code size       58 (0x3a)
        .maxstack  6
        .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
                 [1] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y,
                 [2] int32 z)
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 5,5 : 17,30 
        IL_0000:  nop
        IL_0001:  ldc.i4.0
        IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0007:  stloc.0
        .line 6,6 : 17,23 
        IL_0008:  ldloc.0
        IL_0009:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_000e:  nop
        .line 7,7 : 17,30 
        IL_000f:  ldc.i4.0
        IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0015:  stloc.1
        .line 8,8 : 17,23 
        IL_0016:  ldloc.1
        IL_0017:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_001c:  nop
        .line 9,9 : 17,32 
        IL_001d:  ldloc.0
        IL_001e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0023:  ldloc.1
        IL_0024:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0029:  add
        IL_002a:  stloc.2
        .line 10,10 : 17,25 
        IL_002b:  ldarg.0
        IL_002c:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f2@5::builder@
        IL_0031:  ldloc.2
        IL_0032:  tail.
        IL_0034:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Return<int32>(!!0)
        IL_0039:  ret
      } // end of method f2@5::Invoke

    } // end of class f2@5

    .class auto ansi serializable nested assembly beforefieldinit 'f3@19-4'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .field public int32 x1
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@,
                                   int32 x1,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y) cil managed
      {
        // Code size       28 (0x1c)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::builder@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::x1
        IL_0014:  ldarg.0
        IL_0015:  ldarg.3
        IL_0016:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::y
        IL_001b:  ret
      } // end of method 'f3@19-4'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(int32 _arg) cil managed
      {
        // Code size       39 (0x27)
        .maxstack  6
        .locals init ([0] int32 x4,
                 [1] int32 z)
        .line 18,18 : 17,31 
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  stloc.0
        .line 19,19 : 17,37 
        IL_0003:  ldarg.0
        IL_0004:  ldfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::x1
        IL_0009:  ldarg.0
        IL_000a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::y
        IL_000f:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
        IL_0014:  add
        IL_0015:  ldloc.0
        IL_0016:  add
        IL_0017:  stloc.1
        .line 20,20 : 17,25 
        IL_0018:  ldarg.0
        IL_0019:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::builder@
        IL_001e:  ldloc.1
        IL_001f:  tail.
        IL_0021:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Return<int32>(!!0)
        IL_0026:  ret
      } // end of method 'f3@19-4'::Invoke

    } // end of class 'f3@19-4'

    .class auto ansi serializable nested assembly beforefieldinit 'f3@16-3'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .field public int32 x1
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@,
                                   int32 x1) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::builder@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::x1
        IL_0014:  ret
      } // end of method 'f3@16-3'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(int32 _arg) cil managed
      {
        // Code size       54 (0x36)
        .maxstack  9
        .locals init ([0] int32 x3,
                 [1] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y)
        .line 15,15 : 17,31 
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  stloc.0
        .line 16,16 : 17,30 
        IL_0003:  ldc.i4.0
        IL_0004:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0009:  stloc.1
        .line 17,17 : 17,23 
        IL_000a:  ldloc.1
        IL_000b:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0010:  nop
        .line 18,18 : 17,31 
        IL_0011:  ldarg.0
        IL_0012:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::builder@
        IL_0017:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f2()
        IL_001c:  ldarg.0
        IL_001d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::builder@
        IL_0022:  ldarg.0
        IL_0023:  ldfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::x1
        IL_0028:  ldloc.1
        IL_0029:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                      int32,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_002e:  tail.
        IL_0030:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Bind<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>>)
        IL_0035:  ret
      } // end of method 'f3@16-3'::Invoke

    } // end of class 'f3@16-3'

    .class auto ansi serializable nested assembly beforefieldinit 'f3@15-2'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .field public int32 x1
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@,
                                   int32 x1) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::builder@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::x1
        IL_0014:  ret
      } // end of method 'f3@15-2'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(int32 _arg) cil managed
      {
        // Code size       39 (0x27)
        .maxstack  8
        .locals init ([0] int32 x2)
        .line 14,14 : 17,31 
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  stloc.0
        .line 15,15 : 17,31 
        IL_0003:  ldarg.0
        IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::builder@
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f2()
        IL_000e:  ldarg.0
        IL_000f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::builder@
        IL_0014:  ldarg.0
        IL_0015:  ldfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::x1
        IL_001a:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                      int32)
        IL_001f:  tail.
        IL_0021:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Bind<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>>)
        IL_0026:  ret
      } // end of method 'f3@15-2'::Invoke

    } // end of class 'f3@15-2'

    .class auto ansi serializable nested assembly beforefieldinit 'f3@14-1'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-1'::builder@
        IL_000d:  ret
      } // end of method 'f3@14-1'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(int32 _arg) cil managed
      {
        // Code size       34 (0x22)
        .maxstack  8
        .locals init ([0] int32 x1)
        .line 13,13 : 17,31 
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  stloc.0
        .line 14,14 : 17,31 
        IL_0003:  ldarg.0
        IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-1'::builder@
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f2()
        IL_000e:  ldarg.0
        IL_000f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-1'::builder@
        IL_0014:  ldloc.0
        IL_0015:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                      int32)
        IL_001a:  tail.
        IL_001c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Bind<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>>)
        IL_0021:  ret
      } // end of method 'f3@14-1'::Invoke

    } // end of class 'f3@14-1'

    .class auto ansi serializable nested assembly beforefieldinit f3@13
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f3@13::builder@
        IL_000d:  ret
      } // end of method f3@13::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        // Code size       31 (0x1f)
        .maxstack  7
        .line 13,13 : 17,31 
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f3@13::builder@
        IL_0007:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f2()
        IL_000c:  ldarg.0
        IL_000d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f3@13::builder@
        IL_0012:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
        IL_0017:  tail.
        IL_0019:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Bind<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>>)
        IL_001e:  ret
      } // end of method f3@13::Invoke

    } // end of class f3@13

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
            f2() cil managed
    {
      // Code size       22 (0x16)
      .maxstack  4
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@)
      .line 5,5 : 9,14 
      IL_0000:  nop
      IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldloc.0
      IL_0009:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f2@5::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
      IL_000e:  tail.
      IL_0010:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
      IL_0015:  ret
    } // end of method AsyncExpressionSteppingTest6::f2

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
            f3() cil managed
    {
      // Code size       22 (0x16)
      .maxstack  4
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@)
      .line 13,13 : 9,14 
      IL_0000:  nop
      IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldloc.0
      IL_0009:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f3@13::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
      IL_000e:  tail.
      IL_0010:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
      IL_0015:  ret
    } // end of method AsyncExpressionSteppingTest6::f3

  } // end of class AsyncExpressionSteppingTest6

} // end of class AsyncExpressionSteppingTest6

.class private abstract auto ansi sealed '<StartupCode$AsyncExpressionSteppingTest6>'.$AsyncExpressionSteppingTest6
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
    .line 22,22 : 13,43 
    IL_0000:  nop
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f3()
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
  } // end of method $AsyncExpressionSteppingTest6::.cctor

} // end of class '<StartupCode$AsyncExpressionSteppingTest6>'.$AsyncExpressionSteppingTest6


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
