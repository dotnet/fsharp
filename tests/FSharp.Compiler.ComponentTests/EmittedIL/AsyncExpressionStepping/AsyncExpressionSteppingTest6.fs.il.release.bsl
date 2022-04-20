
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
.assembly AsyncExpressionSteppingTest6
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.AsyncExpressionSteppingTest6
{
  // Offset: 0x00000000 Length: 0x000002E3
  // WARNING: managed resource file FSharpSignatureData.AsyncExpressionSteppingTest6 created
}
.mresource public FSharpOptimizationData.AsyncExpressionSteppingTest6
{
  // Offset: 0x000002E8 Length: 0x000000BE
  // WARNING: managed resource file FSharpOptimizationData.AsyncExpressionSteppingTest6 created
}
.module AsyncExpressionSteppingTest6.dll
// MVID: {622719B0-6394-4FAD-A745-0383B0192762}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00DB0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed AsyncExpressionSteppingTest6
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public AsyncExpressionSteppingTest6
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit 'f2@10-1'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public int32 'value'
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 'value') cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f2@10-1'::'value'
        IL_000d:  ret
      } // end of method 'f2@10-1'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f2@10-1'::'value'
        IL_0007:  tail.
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>::Success(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!0>,
                                                                                                                                                                       !0)
        IL_000e:  ret
      } // end of method 'f2@10-1'::Invoke

    } // end of class 'f2@10-1'

    .class auto ansi serializable sealed nested assembly beforefieldinit f2@5
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f2@5::builder@
        IL_000d:  ret
      } // end of method f2@5::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        // Code size       83 (0x53)
        .maxstack  7
        .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_0,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_1,
                 int32 V_2,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_3,
                 int32 V_4)
        IL_0000:  ldc.i4.0
        IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  ldloc.0
        IL_0009:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_000e:  ldc.i4.1
        IL_000f:  add
        IL_0010:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_Value(!0)
        IL_0015:  nop
        IL_0016:  ldc.i4.0
        IL_0017:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_001c:  stloc.1
        IL_001d:  ldloc.1
        IL_001e:  ldloc.1
        IL_001f:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_0024:  ldc.i4.1
        IL_0025:  add
        IL_0026:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_Value(!0)
        IL_002b:  nop
        IL_002c:  ldloc.0
        IL_002d:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_0032:  ldloc.1
        IL_0033:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_0038:  add
        IL_0039:  stloc.2
        IL_003a:  ldarg.0
        IL_003b:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f2@5::builder@
        IL_0040:  stloc.3
        IL_0041:  ldloc.2
        IL_0042:  stloc.s    V_4
        IL_0044:  ldloc.s    V_4
        IL_0046:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f2@10-1'::.ctor(int32)
        IL_004b:  tail.
        IL_004d:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_0052:  ret
      } // end of method f2@5::Invoke

    } // end of class f2@5

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@20-5'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public int32 'value'
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 'value') cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@20-5'::'value'
        IL_000d:  ret
      } // end of method 'f3@20-5'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@20-5'::'value'
        IL_0007:  tail.
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>::Success(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!0>,
                                                                                                                                                                       !0)
        IL_000e:  ret
      } // end of method 'f3@20-5'::Invoke

    } // end of class 'f3@20-5'

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@19-4'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 x1
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@,
                                   int32 x1,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
              Invoke(int32 _arg4) cil managed
      {
        // Code size       46 (0x2e)
        .maxstack  6
        .locals init (int32 V_0,
                 int32 V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_2,
                 int32 V_3)
        IL_0000:  ldarg.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::x1
        IL_0008:  ldarg.0
        IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::y
        IL_000e:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_0013:  add
        IL_0014:  ldloc.0
        IL_0015:  add
        IL_0016:  stloc.1
        IL_0017:  ldarg.0
        IL_0018:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::builder@
        IL_001d:  stloc.2
        IL_001e:  ldloc.1
        IL_001f:  stloc.3
        IL_0020:  ldloc.3
        IL_0021:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@20-5'::.ctor(int32)
        IL_0026:  tail.
        IL_0028:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_002d:  ret
      } // end of method 'f3@19-4'::Invoke

    } // end of class 'f3@19-4'

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@18-6'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> binder
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> binder) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@18-6'::computation
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@18-6'::binder
        IL_0014:  ret
      } // end of method 'f3@18-6'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@18-6'::computation
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@18-6'::binder
        IL_000d:  tail.
        IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::Bind<int32,int32>(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_0014:  ret
      } // end of method 'f3@18-6'::Invoke

    } // end of class 'f3@18-6'

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@16-3'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 x1
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@,
                                   int32 x1) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
              Invoke(int32 _arg3) cil managed
      {
        // Code size       73 (0x49)
        .maxstack  7
        .locals init (int32 V_0,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_2,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_3,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_4)
        IL_0000:  ldarg.1
        IL_0001:  stloc.0
        IL_0002:  ldc.i4.0
        IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0008:  stloc.1
        IL_0009:  ldloc.1
        IL_000a:  ldloc.1
        IL_000b:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_0010:  ldc.i4.1
        IL_0011:  add
        IL_0012:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_Value(!0)
        IL_0017:  nop
        IL_0018:  ldarg.0
        IL_0019:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::builder@
        IL_001e:  stloc.2
        IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f2()
        IL_0024:  stloc.3
        IL_0025:  ldarg.0
        IL_0026:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::builder@
        IL_002b:  ldarg.0
        IL_002c:  ldfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::x1
        IL_0031:  ldloc.1
        IL_0032:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@19-4'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                      int32,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0037:  stloc.s    V_4
        IL_0039:  ldloc.3
        IL_003a:  ldloc.s    V_4
        IL_003c:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@18-6'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>)
        IL_0041:  tail.
        IL_0043:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_0048:  ret
      } // end of method 'f3@16-3'::Invoke

    } // end of class 'f3@16-3'

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@15-7'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> binder
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> binder) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-7'::computation
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-7'::binder
        IL_0014:  ret
      } // end of method 'f3@15-7'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-7'::computation
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-7'::binder
        IL_000d:  tail.
        IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::Bind<int32,int32>(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_0014:  ret
      } // end of method 'f3@15-7'::Invoke

    } // end of class 'f3@15-7'

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@15-2'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 x1
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@,
                                   int32 x1) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
              Invoke(int32 _arg2) cil managed
      {
        // Code size       48 (0x30)
        .maxstack  6
        .locals init (int32 V_0,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_2,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_3)
        IL_0000:  ldarg.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::builder@
        IL_0008:  stloc.1
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f2()
        IL_000e:  stloc.2
        IL_000f:  ldarg.0
        IL_0010:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::builder@
        IL_0015:  ldarg.0
        IL_0016:  ldfld      int32 AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::x1
        IL_001b:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@16-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                      int32)
        IL_0020:  stloc.3
        IL_0021:  ldloc.2
        IL_0022:  ldloc.3
        IL_0023:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-7'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>)
        IL_0028:  tail.
        IL_002a:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_002f:  ret
      } // end of method 'f3@15-2'::Invoke

    } // end of class 'f3@15-2'

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@14-8'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> binder
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> binder) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-8'::computation
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-8'::binder
        IL_0014:  ret
      } // end of method 'f3@14-8'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-8'::computation
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-8'::binder
        IL_000d:  tail.
        IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::Bind<int32,int32>(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_0014:  ret
      } // end of method 'f3@14-8'::Invoke

    } // end of class 'f3@14-8'

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@14-1'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
              Invoke(int32 _arg1) cil managed
      {
        // Code size       43 (0x2b)
        .maxstack  6
        .locals init (int32 V_0,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_2,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_3)
        IL_0000:  ldarg.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-1'::builder@
        IL_0008:  stloc.1
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f2()
        IL_000e:  stloc.2
        IL_000f:  ldarg.0
        IL_0010:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-1'::builder@
        IL_0015:  ldloc.0
        IL_0016:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@15-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                      int32)
        IL_001b:  stloc.3
        IL_001c:  ldloc.2
        IL_001d:  ldloc.3
        IL_001e:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-8'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>)
        IL_0023:  tail.
        IL_0025:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_002a:  ret
      } // end of method 'f3@14-1'::Invoke

    } // end of class 'f3@14-1'

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@13-9'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> binder
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> binder) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@13-9'::computation
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@13-9'::binder
        IL_0014:  ret
      } // end of method 'f3@13-9'::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@13-9'::computation
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@13-9'::binder
        IL_000d:  tail.
        IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::Bind<int32,int32>(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_0014:  ret
      } // end of method 'f3@13-9'::Invoke

    } // end of class 'f3@13-9'

    .class auto ansi serializable sealed nested assembly beforefieldinit f3@13
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f3@13::builder@
        IL_000d:  ret
      } // end of method f3@13::.ctor

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        // Code size       40 (0x28)
        .maxstack  6
        .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_0,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_1,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_2)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f3@13::builder@
        IL_0006:  stloc.0
        IL_0007:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f2()
        IL_000c:  stloc.1
        IL_000d:  ldarg.0
        IL_000e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f3@13::builder@
        IL_0013:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@14-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
        IL_0018:  stloc.2
        IL_0019:  ldloc.1
        IL_001a:  ldloc.2
        IL_001b:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/'f3@13-9'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>)
        IL_0020:  tail.
        IL_0022:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_0027:  ret
      } // end of method f3@13::Invoke

    } // end of class f3@13

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
            f2() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_0)
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldloc.0
      IL_0008:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f2@5::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
      IL_000d:  tail.
      IL_000f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
      IL_0014:  ret
    } // end of method AsyncExpressionSteppingTest6::f2

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
            f3() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_0)
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldloc.0
      IL_0008:  newobj     instance void AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6/f3@13::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
      IL_000d:  tail.
      IL_000f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
      IL_0014:  ret
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
    // Code size       18 (0x12)
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_1)
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> AsyncExpressionSteppingTest6/AsyncExpressionSteppingTest6::f3()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  ldnull
    IL_000a:  ldnull
    IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync::RunSynchronously<int32>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>,
                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<valuetype [mscorlib]System.Threading.CancellationToken>)
    IL_0010:  pop
    IL_0011:  ret
  } // end of method $AsyncExpressionSteppingTest6::.cctor

} // end of class '<StartupCode$AsyncExpressionSteppingTest6>'.$AsyncExpressionSteppingTest6


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net472\tests\EmittedIL\AsyncExpressionStepping\AsyncExpressionSteppingTest6_fs\AsyncExpressionSteppingTest6.res
