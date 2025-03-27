




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern assemblyLibrary
{
  .ver 0:0:0:0
}
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed ComputationExpressions.Program
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Collections.Generic.List`1<int32> xs@10
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public static class [runtime]System.Collections.Generic.List`1<int32> f0() cil managed
  {
    
    .maxstack  4
    .locals init (class [runtime]System.Collections.Generic.List`1<int32> V_0)
    IL_0000:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.2
    IL_000f:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.3
    IL_0016:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_001b:  ldloc.0
    IL_001c:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.List`1<int32> get_xs() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.List`1<int32> ComputationExpressions.Program::xs@10
    IL_0005:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<int32> f1() cil managed
  {
    
    .maxstack  4
    .locals init (class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> V_0,
             class [runtime]System.Collections.Generic.List`1<int32> V_1,
             class [runtime]System.Collections.Generic.List`1<int32>& V_2)
    IL_0000:  call       class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<!0> class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32>::get_Instance()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000b:  stloc.1
    IL_000c:  ldloca.s   V_1
    IL_000e:  stloc.2
    IL_000f:  ldloc.2
    IL_0010:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
    IL_0015:  ldc.i4.1
    IL_0016:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_001b:  ldloca.s   V_1
    IL_001d:  stloc.2
    IL_001e:  ldloc.2
    IL_001f:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
    IL_0024:  ldc.i4.2
    IL_0025:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_002a:  ldloca.s   V_1
    IL_002c:  stloc.2
    IL_002d:  ldloc.2
    IL_002e:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
    IL_0033:  ldc.i4.3
    IL_0034:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0039:  ldloc.1
    IL_003a:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<int32> f2() cil managed
  {
    
    .maxstack  4
    .locals init (class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> V_0,
             class [runtime]System.Collections.Generic.List`1<int32> V_1,
             class [runtime]System.Collections.Generic.List`1<int32>& V_2)
    IL_0000:  call       class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<!0> class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32>::get_Instance()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000b:  stloc.1
    IL_000c:  ldloca.s   V_1
    IL_000e:  stloc.2
    IL_000f:  ldloc.2
    IL_0010:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
    IL_0015:  call       class [runtime]System.Collections.Generic.List`1<int32> ComputationExpressions.Program::get_xs()
    IL_001a:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::AddRange(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_001f:  ldloc.1
    IL_0020:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<int32> f3() cil managed
  {
    
    .maxstack  5
    .locals init (class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> V_0,
             class [runtime]System.Collections.Generic.List`1<int32> V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             class [runtime]System.Collections.Generic.List`1<int32>& V_6)
    IL_0000:  call       class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<!0> class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32>::get_Instance()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000b:  stloc.1
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.3
    IL_000e:  call       class [runtime]System.Collections.Generic.List`1<int32> ComputationExpressions.Program::get_xs()
    IL_0013:  callvirt   instance int32 class [runtime]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  stloc.2
    IL_001b:  ldloc.2
    IL_001c:  ldloc.3
    IL_001d:  blt.s      IL_004f

    IL_001f:  call       class [runtime]System.Collections.Generic.List`1<int32> ComputationExpressions.Program::get_xs()
    IL_0024:  ldloc.3
    IL_0025:  callvirt   instance !0 class [runtime]System.Collections.Generic.List`1<int32>::get_Item(int32)
    IL_002a:  stloc.s    V_4
    IL_002c:  ldloc.s    V_4
    IL_002e:  ldloc.s    V_4
    IL_0030:  mul
    IL_0031:  stloc.s    V_5
    IL_0033:  ldloca.s   V_1
    IL_0035:  stloc.s    V_6
    IL_0037:  ldloc.s    V_6
    IL_0039:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
    IL_003e:  ldloc.s    V_5
    IL_0040:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0045:  ldloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  add
    IL_0048:  stloc.3
    IL_0049:  ldloc.3
    IL_004a:  ldloc.2
    IL_004b:  ldc.i4.1
    IL_004c:  add
    IL_004d:  bne.un.s   IL_001f

    IL_004f:  ldloc.1
    IL_0050:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>.$ComputationExpressions'.Program::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>.$ComputationExpressions'.Program::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [runtime]System.Collections.Generic.List`1<int32> ComputationExpressions.Program::f0()
    IL_0005:  stsfld     class [runtime]System.Collections.Generic.List`1<int32> ComputationExpressions.Program::xs@10
    IL_000a:  ret
  } 

  .property class [runtime]System.Collections.Generic.List`1<int32>
          xs()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.List`1<int32> ComputationExpressions.Program::get_xs()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>.$ComputationExpressions'.Program
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void ComputationExpressions.Program::staticInitialization@()
    IL_0005:  ret
  } 

} 






