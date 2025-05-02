




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
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'f1@12-1'
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 x
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ComputationExpressions.Program/'f1@12-1'::x
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(class [runtime]System.Collections.Generic.List`1<int32>& sm) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 ComputationExpressions.Program/'f1@12-1'::x
      IL_000c:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
      IL_0011:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'f1@12-3'
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 x
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ComputationExpressions.Program/'f1@12-3'::x
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(class [runtime]System.Collections.Generic.List`1<int32>& sm) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 ComputationExpressions.Program/'f1@12-3'::x
      IL_000c:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
      IL_0011:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'f1@12-4'
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/'f1@12-4'::builder@
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(class [runtime]System.Collections.Generic.List`1<int32>& sm) cil managed
    {
      
      .maxstack  6
      .locals init (class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_0,
               int32 V_1,
               class [runtime]System.Collections.Generic.List`1<int32>& V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/'f1@12-4'::builder@
      IL_0006:  stloc.0
      IL_0007:  ldc.i4.3
      IL_0008:  stloc.1
      IL_0009:  ldarg.1
      IL_000a:  stloc.2
      IL_000b:  ldloc.2
      IL_000c:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
      IL_0011:  ldloc.1
      IL_0012:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
      IL_0017:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'f1@12-2'
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/'f1@12-2'::builder@
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(class [runtime]System.Collections.Generic.List`1<int32>& sm) cil managed
    {
      
      .maxstack  6
      .locals init (class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_0,
               class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>> V_1,
               class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_2,
               int32 V_3,
               class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>> V_4,
               class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_5,
               class [runtime]System.Collections.Generic.List`1<int32>& V_6)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/'f1@12-2'::builder@
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/'f1@12-2'::builder@
      IL_000d:  stloc.2
      IL_000e:  ldc.i4.2
      IL_000f:  stloc.3
      IL_0010:  ldloc.3
      IL_0011:  newobj     instance void ComputationExpressions.Program/'f1@12-3'::.ctor(int32)
      IL_0016:  ldftn      instance void ComputationExpressions.Program/'f1@12-3'::Invoke(class [runtime]System.Collections.Generic.List`1<int32>&)
      IL_001c:  newobj     instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::.ctor(object,
                                                                                                                                                                                                                      native int)
      IL_0021:  stloc.1
      IL_0022:  ldarg.0
      IL_0023:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/'f1@12-2'::builder@
      IL_0028:  stloc.s    V_5
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/'f1@12-2'::builder@
      IL_0030:  newobj     instance void ComputationExpressions.Program/'f1@12-4'::.ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32>)
      IL_0035:  ldftn      instance void ComputationExpressions.Program/'f1@12-4'::Invoke(class [runtime]System.Collections.Generic.List`1<int32>&)
      IL_003b:  newobj     instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::.ctor(object,
                                                                                                                                                                                                                      native int)
      IL_0040:  stloc.s    V_4
      IL_0042:  ldarg.1
      IL_0043:  stloc.s    V_6
      IL_0045:  ldloc.1
      IL_0046:  ldloc.s    V_6
      IL_0048:  callvirt   instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::Invoke(!0&)
      IL_004d:  nop
      IL_004e:  ldloc.s    V_4
      IL_0050:  ldloc.s    V_6
      IL_0052:  callvirt   instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::Invoke(!0&)
      IL_0057:  nop
      IL_0058:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname f1@12
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f1@12::builder@
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(class [runtime]System.Collections.Generic.List`1<int32>& sm) cil managed
    {
      
      .maxstack  6
      .locals init (class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_0,
               class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>> V_1,
               class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_2,
               int32 V_3,
               class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>> V_4,
               class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_5,
               class [runtime]System.Collections.Generic.List`1<int32>& V_6)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f1@12::builder@
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f1@12::builder@
      IL_000d:  stloc.2
      IL_000e:  ldc.i4.1
      IL_000f:  stloc.3
      IL_0010:  ldloc.3
      IL_0011:  newobj     instance void ComputationExpressions.Program/'f1@12-1'::.ctor(int32)
      IL_0016:  ldftn      instance void ComputationExpressions.Program/'f1@12-1'::Invoke(class [runtime]System.Collections.Generic.List`1<int32>&)
      IL_001c:  newobj     instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::.ctor(object,
                                                                                                                                                                                                                      native int)
      IL_0021:  stloc.1
      IL_0022:  ldarg.0
      IL_0023:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f1@12::builder@
      IL_0028:  stloc.s    V_5
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f1@12::builder@
      IL_0030:  newobj     instance void ComputationExpressions.Program/'f1@12-2'::.ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32>)
      IL_0035:  ldftn      instance void ComputationExpressions.Program/'f1@12-2'::Invoke(class [runtime]System.Collections.Generic.List`1<int32>&)
      IL_003b:  newobj     instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::.ctor(object,
                                                                                                                                                                                                                      native int)
      IL_0040:  stloc.s    V_4
      IL_0042:  ldarg.1
      IL_0043:  stloc.s    V_6
      IL_0045:  ldloc.1
      IL_0046:  ldloc.s    V_6
      IL_0048:  callvirt   instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::Invoke(!0&)
      IL_004d:  nop
      IL_004e:  ldloc.s    V_4
      IL_0050:  ldloc.s    V_6
      IL_0052:  callvirt   instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::Invoke(!0&)
      IL_0057:  nop
      IL_0058:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname f2@13
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f2@13::builder@
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(class [runtime]System.Collections.Generic.List`1<int32>& sm) cil managed
    {
      
      .maxstack  6
      .locals init (class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_0,
               class [runtime]System.Collections.Generic.List`1<int32> V_1,
               class [runtime]System.Collections.Generic.List`1<int32>& V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f2@13::builder@
      IL_0006:  stloc.0
      IL_0007:  call       class [runtime]System.Collections.Generic.List`1<int32> ComputationExpressions.Program::get_xs()
      IL_000c:  stloc.1
      IL_000d:  ldarg.1
      IL_000e:  stloc.2
      IL_000f:  ldloc.2
      IL_0010:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
      IL_0015:  ldloc.1
      IL_0016:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::AddRange(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_001b:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname f3@14
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> builder@) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f3@14::builder@
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(class [runtime]System.Collections.Generic.List`1<int32>& sm) cil managed
    {
      
      .maxstack  7
      .locals init (class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_0,
               class [runtime]System.Collections.Generic.List`1<int32> V_1,
               class [runtime]System.Collections.Generic.List`1<int32>& V_2,
               int32 V_3,
               int32 V_4,
               int32 V_5,
               int32 V_6,
               class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_7,
               int32 V_8,
               class [runtime]System.Collections.Generic.List`1<int32>& V_9)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f3@14::builder@
      IL_0006:  stloc.0
      IL_0007:  call       class [runtime]System.Collections.Generic.List`1<int32> ComputationExpressions.Program::get_xs()
      IL_000c:  stloc.1
      IL_000d:  ldarg.1
      IL_000e:  stloc.2
      IL_000f:  ldc.i4.0
      IL_0010:  stloc.s    V_4
      IL_0012:  ldloc.1
      IL_0013:  callvirt   instance int32 class [runtime]System.Collections.Generic.List`1<int32>::get_Count()
      IL_0018:  ldc.i4.1
      IL_0019:  sub
      IL_001a:  stloc.3
      IL_001b:  ldloc.3
      IL_001c:  ldloc.s    V_4
      IL_001e:  blt.s      IL_005b

      IL_0020:  ldloc.1
      IL_0021:  ldloc.s    V_4
      IL_0023:  callvirt   instance !0 class [runtime]System.Collections.Generic.List`1<int32>::get_Item(int32)
      IL_0028:  stloc.s    V_5
      IL_002a:  ldloc.s    V_5
      IL_002c:  stloc.s    V_6
      IL_002e:  ldarg.0
      IL_002f:  ldfld      class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> ComputationExpressions.Program/f3@14::builder@
      IL_0034:  stloc.s    V_7
      IL_0036:  ldloc.s    V_6
      IL_0038:  ldloc.s    V_6
      IL_003a:  mul
      IL_003b:  stloc.s    V_8
      IL_003d:  ldloc.2
      IL_003e:  stloc.s    V_9
      IL_0040:  ldloc.s    V_9
      IL_0042:  ldobj      class [runtime]System.Collections.Generic.List`1<int32>
      IL_0047:  ldloc.s    V_8
      IL_0049:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<int32>::Add(!0)
      IL_004e:  ldloc.s    V_4
      IL_0050:  ldc.i4.1
      IL_0051:  add
      IL_0052:  stloc.s    V_4
      IL_0054:  ldloc.s    V_4
      IL_0056:  ldloc.3
      IL_0057:  ldc.i4.1
      IL_0058:  add
      IL_0059:  bne.un.s   IL_0020

      IL_005b:  ret
    } 

  } 

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
             class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> V_1,
             class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>> V_2,
             class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_3,
             class [runtime]System.Collections.Generic.List`1<int32> V_4)
    IL_0000:  call       class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<!!0> [assemblyLibrary]ComputationExpressions.Library::resizeArray<int32>()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    IL_0008:  ldloc.0
    IL_0009:  stloc.3
    IL_000a:  ldloc.0
    IL_000b:  newobj     instance void ComputationExpressions.Program/f1@12::.ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32>)
    IL_0010:  ldftn      instance void ComputationExpressions.Program/f1@12::Invoke(class [runtime]System.Collections.Generic.List`1<int32>&)
    IL_0016:  newobj     instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::.ctor(object,
                                                                                                                                                                                                                    native int)
    IL_001b:  stloc.2
    IL_001c:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0021:  stloc.s    V_4
    IL_0023:  ldloc.2
    IL_0024:  ldloca.s   V_4
    IL_0026:  callvirt   instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::Invoke(!0&)
    IL_002b:  nop
    IL_002c:  ldloc.s    V_4
    IL_002e:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<int32> f2() cil managed
  {
    
    .maxstack  4
    .locals init (class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> V_0,
             class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> V_1,
             class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>> V_2,
             class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_3,
             class [runtime]System.Collections.Generic.List`1<int32> V_4)
    IL_0000:  call       class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<!!0> [assemblyLibrary]ComputationExpressions.Library::resizeArray<int32>()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    IL_0008:  ldloc.0
    IL_0009:  stloc.3
    IL_000a:  ldloc.0
    IL_000b:  newobj     instance void ComputationExpressions.Program/f2@13::.ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32>)
    IL_0010:  ldftn      instance void ComputationExpressions.Program/f2@13::Invoke(class [runtime]System.Collections.Generic.List`1<int32>&)
    IL_0016:  newobj     instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::.ctor(object,
                                                                                                                                                                                                                    native int)
    IL_001b:  stloc.2
    IL_001c:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0021:  stloc.s    V_4
    IL_0023:  ldloc.2
    IL_0024:  ldloca.s   V_4
    IL_0026:  callvirt   instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::Invoke(!0&)
    IL_002b:  nop
    IL_002c:  ldloc.s    V_4
    IL_002e:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<int32> f3() cil managed
  {
    
    .maxstack  4
    .locals init (class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> V_0,
             class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32> V_1,
             class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>> V_2,
             class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilder V_3,
             class [runtime]System.Collections.Generic.List`1<int32> V_4)
    IL_0000:  call       class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<!!0> [assemblyLibrary]ComputationExpressions.Library::resizeArray<int32>()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    IL_0008:  ldloc.0
    IL_0009:  stloc.3
    IL_000a:  ldloc.0
    IL_000b:  newobj     instance void ComputationExpressions.Program/f3@14::.ctor(class [assemblyLibrary]ComputationExpressions.Library/ResizeArrayBuilder`1<int32>)
    IL_0010:  ldftn      instance void ComputationExpressions.Program/f3@14::Invoke(class [runtime]System.Collections.Generic.List`1<int32>&)
    IL_0016:  newobj     instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::.ctor(object,
                                                                                                                                                                                                                    native int)
    IL_001b:  stloc.2
    IL_001c:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0021:  stloc.s    V_4
    IL_0023:  ldloc.2
    IL_0024:  ldloca.s   V_4
    IL_0026:  callvirt   instance void class [assemblyLibrary]ComputationExpressions.Library/CollectionBuilderCode`1<class [runtime]System.Collections.Generic.List`1<int32>>::Invoke(!0&)
    IL_002b:  nop
    IL_002c:  ldloc.s    V_4
    IL_002e:  ret
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






