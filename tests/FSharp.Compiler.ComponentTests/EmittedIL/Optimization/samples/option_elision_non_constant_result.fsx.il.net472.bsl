




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





.class public abstract auto ansi sealed Option_elision_non_constant_result
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!!b> 
          mapOption<a,b>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!b> mapper,
                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!!a> option) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!!a> V_0,
             !!a V_1)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_0005

    IL_0003:  br.s       IL_001b

    IL_0005:  ldarg.1
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!!a>::get_Value()
    IL_000d:  stloc.1
    IL_000e:  ldarg.0
    IL_000f:  ldloc.1
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!b>::Invoke(!0)
    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!!b>::Some(!0)
    IL_001a:  ret

    IL_001b:  ldnull
    IL_001c:  ret
  } 

  .method public static int32  almostErasedOption() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.5
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>::Some(!0)
    IL_0006:  brtrue.s   IL_000a

    IL_0008:  br.s       IL_000c

    IL_000a:  ldc.i4.5
    IL_000b:  ret

    IL_000c:  ldc.i4.4
    IL_000d:  ret
  } 

  .method public static int32  unnecessaryOption() cil managed
  {
    
    .maxstack  3
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32> V_0)
    IL_0000:  ldc.i4.5
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>::Some(!0)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brtrue.s   IL_000c

    IL_000a:  br.s       IL_0013

    IL_000c:  ldloc.0
    IL_000d:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>::get_Value()
    IL_0012:  ret

    IL_0013:  ldc.i4.4
    IL_0014:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Option_elision_non_constant_result$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Option_elision_non_constant_result$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void [runtime]System.Console::WriteLine()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Option_elision_non_constant_result$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void Option_elision_non_constant_result::staticInitialization@()
    IL_0005:  ret
  } 

} 






