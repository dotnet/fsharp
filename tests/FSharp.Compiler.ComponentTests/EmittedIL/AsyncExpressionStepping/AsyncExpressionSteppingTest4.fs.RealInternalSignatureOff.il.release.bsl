




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
    .class auto ansi serializable sealed nested assembly beforefieldinit 'f4@10-2'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public int32 'value'
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(int32 'value') cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 assembly/assembly/'f4@10-2'::'value'
        IL_000d:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 assembly/assembly/'f4@10-2'::'value'
        IL_0007:  tail.
        IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>::Success(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!0>,
                                                                                                                                                                       !0)
        IL_000e:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f4@7-1'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@, class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f4@7-1'::builder@
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> assembly/assembly/'f4@7-1'::x
        IL_0014:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  6
        .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_0,
                 int32 V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_2,
                 int32 V_3)
        IL_0000:  nop
        IL_0001:  ldc.i4.0
        IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0007:  stloc.0
        IL_0008:  ldloc.0
        IL_0009:  ldloc.0
        IL_000a:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_000f:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_Value(!0)
        IL_0014:  nop
        IL_0015:  ldarg.0
        IL_0016:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> assembly/assembly/'f4@7-1'::x
        IL_001b:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_0020:  ldloc.0
        IL_0021:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_0026:  add
        IL_0027:  stloc.1
        IL_0028:  ldarg.0
        IL_0029:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/'f4@7-1'::builder@
        IL_002e:  stloc.2
        IL_002f:  ldloc.1
        IL_0030:  stloc.3
        IL_0031:  ldloc.3
        IL_0032:  newobj     instance void assembly/assembly/'f4@10-2'::.ctor(int32)
        IL_0037:  tail.
        IL_0039:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_003e:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f4@12-3'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> assembly/assembly/'f4@12-3'::x
        IL_000d:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> assembly/assembly/'f4@12-3'::x
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> assembly/assembly/'f4@12-3'::x
        IL_000d:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_Value()
        IL_0012:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_Value(!0)
        IL_0017:  nop
        IL_0018:  ldstr      "done"
        IL_001d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0022:  tail.
        IL_0024:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0029:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'f4@6-4'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> compensation
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> computation, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> compensation) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f4@6-4'::computation
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/assembly/'f4@6-4'::compensation
        IL_0014:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<int32> ctxt) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly/'f4@6-4'::computation
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/assembly/'f4@6-4'::compensation
        IL_000d:  tail.
        IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::TryFinally<int32>(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0014:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit f4@5
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>
    {
      .field public class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder builder@) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/f4@5::builder@
        IL_000d:  ret
      } 

      .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  7
        .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_0,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_1,
                 class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_2,
                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_3)
        IL_0000:  ldc.i4.0
        IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/f4@5::builder@
        IL_000d:  stloc.1
        IL_000e:  ldarg.0
        IL_000f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/f4@5::builder@
        IL_0014:  ldarg.0
        IL_0015:  ldfld      class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder assembly/assembly/f4@5::builder@
        IL_001a:  ldloc.0
        IL_001b:  newobj     instance void assembly/assembly/'f4@7-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder,
                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_0020:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
        IL_0025:  stloc.2
        IL_0026:  ldloc.0
        IL_0027:  newobj     instance void assembly/assembly/'f4@12-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
        IL_002c:  stloc.3
        IL_002d:  ldloc.2
        IL_002e:  ldloc.3
        IL_002f:  newobj     instance void assembly/assembly/'f4@6-4'::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32>,
                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0034:  tail.
        IL_0036:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
        IL_003b:  ret
      } 

    } 

    .method public static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> f4() cil managed
    {
      
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder V_0)
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldloc.0
      IL_0008:  newobj     instance void assembly/assembly/f4@5::.ctor(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder)
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
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> V_1)
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<int32> assembly/assembly::f4()
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





