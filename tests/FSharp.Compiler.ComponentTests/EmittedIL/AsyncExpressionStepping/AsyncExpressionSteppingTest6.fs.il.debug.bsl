




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.assembly
{
  
  
}
.mresource public FSharpOptimizationData.assembly
{
  
  
}
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public assembly
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit 'f2@10-1'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public int32 res
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 res) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 assembly/assembly/'f2@10-1'::res
        IL_000d:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 assembly/assembly/'f2@10-1'::res
        IL_0007:  tail.
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>::Success(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!0>,
                                                                                                                                                                       !0)
        IL_000e:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit f2@5
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/f2@5::builder@
        IL_000d:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  7
        .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_0,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_1,
                 int32 V_2,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_3,
                 int32 V_4,
                 int32 V_5)
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
        IL_003b:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/f2@5::builder@
        IL_0040:  stloc.3
        IL_0041:  ldloc.2
        IL_0042:  stloc.s    V_4
        IL_0044:  ldloc.s    V_4
        IL_0046:  stloc.s    V_5
        IL_0048:  ldloc.s    V_5
        IL_004a:  newobj     instance void assembly/assembly/'f2@10-1'::.ctor(int32)
        IL_004f:  tail.
        IL_0051:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_0056:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@20-5'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public int32 res
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 res) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 assembly/assembly/'f3@20-5'::res
        IL_000d:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 assembly/assembly/'f3@20-5'::res
        IL_0007:  tail.
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>::Success(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!0>,
                                                                                                                                                                       !0)
        IL_000e:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@19-4'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 x1
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@,
                                   int32 x1,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@19-4'::builder@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 assembly/assembly/'f3@19-4'::x1
        IL_0014:  ldarg.0
        IL_0015:  ldarg.3
        IL_0016:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> assembly/assembly/'f3@19-4'::y
        IL_001b:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(int32 _arg4) cil managed
      {
        
        .maxstack  6
        .locals init (int32 V_0,
                 int32 V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_2,
                 int32 V_3,
                 int32 V_4)
        IL_0000:  ldarg.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 assembly/assembly/'f3@19-4'::x1
        IL_0008:  ldarg.0
        IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> assembly/assembly/'f3@19-4'::y
        IL_000e:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_0013:  add
        IL_0014:  ldloc.0
        IL_0015:  add
        IL_0016:  stloc.1
        IL_0017:  ldarg.0
        IL_0018:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@19-4'::builder@
        IL_001d:  stloc.2
        IL_001e:  ldloc.1
        IL_001f:  stloc.3
        IL_0020:  ldloc.3
        IL_0021:  stloc.s    V_4
        IL_0023:  ldloc.s    V_4
        IL_0025:  newobj     instance void assembly/assembly/'f3@20-5'::.ctor(int32)
        IL_002a:  tail.
        IL_002c:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_0031:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@18-6'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> part1
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> part2
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> part1,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> part2) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f3@18-6'::part1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> assembly/assembly/'f3@18-6'::part2
        IL_0014:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f3@18-6'::part1
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> assembly/assembly/'f3@18-6'::part2
        IL_000d:  tail.
        IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::Bind<int32,int32>(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_0014:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@16-3'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 x1
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@,
                                   int32 x1) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@16-3'::builder@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 assembly/assembly/'f3@16-3'::x1
        IL_0014:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(int32 _arg3) cil managed
      {
        
        .maxstack  7
        .locals init (int32 V_0,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_2,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_3,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_4,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_5,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_6)
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
        IL_0019:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@16-3'::builder@
        IL_001e:  stloc.2
        IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly::f2()
        IL_0024:  stloc.3
        IL_0025:  ldarg.0
        IL_0026:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@16-3'::builder@
        IL_002b:  ldarg.0
        IL_002c:  ldfld      int32 assembly/assembly/'f3@16-3'::x1
        IL_0031:  ldloc.1
        IL_0032:  newobj     instance void assembly/assembly/'f3@19-4'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                      int32,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0037:  stloc.s    V_4
        IL_0039:  ldloc.3
        IL_003a:  stloc.s    V_5
        IL_003c:  ldloc.s    V_4
        IL_003e:  stloc.s    V_6
        IL_0040:  ldloc.s    V_5
        IL_0042:  ldloc.s    V_6
        IL_0044:  newobj     instance void assembly/assembly/'f3@18-6'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>)
        IL_0049:  tail.
        IL_004b:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_0050:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@15-7'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> part1
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> part2
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> part1,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> part2) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f3@15-7'::part1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> assembly/assembly/'f3@15-7'::part2
        IL_0014:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f3@15-7'::part1
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> assembly/assembly/'f3@15-7'::part2
        IL_000d:  tail.
        IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::Bind<int32,int32>(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_0014:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@15-2'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 x1
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@,
                                   int32 x1) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@15-2'::builder@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 assembly/assembly/'f3@15-2'::x1
        IL_0014:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(int32 _arg2) cil managed
      {
        
        .maxstack  6
        .locals init (int32 V_0,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_2,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_3,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_4,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_5)
        IL_0000:  ldarg.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@15-2'::builder@
        IL_0008:  stloc.1
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly::f2()
        IL_000e:  stloc.2
        IL_000f:  ldarg.0
        IL_0010:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@15-2'::builder@
        IL_0015:  ldarg.0
        IL_0016:  ldfld      int32 assembly/assembly/'f3@15-2'::x1
        IL_001b:  newobj     instance void assembly/assembly/'f3@16-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                      int32)
        IL_0020:  stloc.3
        IL_0021:  ldloc.2
        IL_0022:  stloc.s    V_4
        IL_0024:  ldloc.3
        IL_0025:  stloc.s    V_5
        IL_0027:  ldloc.s    V_4
        IL_0029:  ldloc.s    V_5
        IL_002b:  newobj     instance void assembly/assembly/'f3@15-7'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>)
        IL_0030:  tail.
        IL_0032:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_0037:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@14-8'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> part1
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> part2
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> part1,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> part2) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f3@14-8'::part1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> assembly/assembly/'f3@14-8'::part2
        IL_0014:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f3@14-8'::part1
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> assembly/assembly/'f3@14-8'::part2
        IL_000d:  tail.
        IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::Bind<int32,int32>(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_0014:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@14-1'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@14-1'::builder@
        IL_000d:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(int32 _arg1) cil managed
      {
        
        .maxstack  6
        .locals init (int32 V_0,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_2,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_3,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_4,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_5)
        IL_0000:  ldarg.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@14-1'::builder@
        IL_0008:  stloc.1
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly::f2()
        IL_000e:  stloc.2
        IL_000f:  ldarg.0
        IL_0010:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f3@14-1'::builder@
        IL_0015:  ldloc.0
        IL_0016:  newobj     instance void assembly/assembly/'f3@15-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                      int32)
        IL_001b:  stloc.3
        IL_001c:  ldloc.2
        IL_001d:  stloc.s    V_4
        IL_001f:  ldloc.3
        IL_0020:  stloc.s    V_5
        IL_0022:  ldloc.s    V_4
        IL_0024:  ldloc.s    V_5
        IL_0026:  newobj     instance void assembly/assembly/'f3@14-8'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>)
        IL_002b:  tail.
        IL_002d:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_0032:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f3@13-9'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> part1
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> part2
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> part1,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> part2) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f3@13-9'::part1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> assembly/assembly/'f3@13-9'::part2
        IL_0014:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn 
              Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f3@13-9'::part1
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> assembly/assembly/'f3@13-9'::part2
        IL_000d:  tail.
        IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::Bind<int32,int32>(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!1>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_0014:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit f3@13
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/f3@13::builder@
        IL_000d:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
              Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  6
        .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_0,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_1,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_2,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_3,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>> V_4)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/f3@13::builder@
        IL_0006:  stloc.0
        IL_0007:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly::f2()
        IL_000c:  stloc.1
        IL_000d:  ldarg.0
        IL_000e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/f3@13::builder@
        IL_0013:  newobj     instance void assembly/assembly/'f3@14-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
        IL_0018:  stloc.2
        IL_0019:  ldloc.1
        IL_001a:  stloc.3
        IL_001b:  ldloc.2
        IL_001c:  stloc.s    V_4
        IL_001e:  ldloc.3
        IL_001f:  ldloc.s    V_4
        IL_0021:  newobj     instance void assembly/assembly/'f3@13-9'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>,
                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>)
        IL_0026:  tail.
        IL_0028:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_002d:  ret
      } 

    } 

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
            f2() cil managed
    {
      
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_0)
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.AsyncGlobalOperators::get_DefaultAsyncBuilder()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldloc.0
      IL_0008:  newobj     instance void assembly/assembly/f2@5::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
      IL_000d:  tail.
      IL_000f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
      IL_0014:  ret
    } 

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> 
            f3() cil managed
    {
      
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_0)
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.AsyncGlobalOperators::get_DefaultAsyncBuilder()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldloc.0
      IL_0008:  newobj     instance void assembly/assembly/f3@13::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
      IL_000d:  tail.
      IL_000f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
      IL_0014:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_1)
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly::f3()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  ldnull
    IL_000a:  ldnull
    IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync::RunSynchronously<int32>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>,
                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<valuetype [runtime]System.Threading.CancellationToken>)
    IL_0010:  pop
    IL_0011:  ret
  } 

} 






