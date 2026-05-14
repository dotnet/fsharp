




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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
{
  
  
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public A
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
            F(class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> x1,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> x2) cil managed
    {
      .param [1]
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.OptionalArgumentAttribute::.ctor() = ( 01 00 00 00 ) 
      .param [2]
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.OptionalArgumentAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               class [runtime]System.Collections.Generic.List`1<class assembly/A> V_2,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_3,
               class assembly/A V_4)
      IL_0000:  nop
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  brfalse.s  IL_0007

      IL_0005:  br.s       IL_000b

      IL_0007:  ldc.i4.0
      IL_0008:  nop
      IL_0009:  br.s       IL_000d

      IL_000b:  ldc.i4.1
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  nop
      IL_000f:  ldarg.1
      IL_0010:  brfalse.s  IL_0014

      IL_0012:  br.s       IL_0018

      IL_0014:  ldloc.0
      IL_0015:  nop
      IL_0016:  br.s       IL_001c

      IL_0018:  ldloc.0
      IL_0019:  ldc.i4.1
      IL_001a:  add
      IL_001b:  nop
      IL_001c:  stloc.1
      IL_001d:  ldloc.1
      IL_001e:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
      IL_0023:  stloc.2
      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  brfalse.s  IL_002a

      IL_0028:  br.s       IL_002e

      IL_002a:  nop
      IL_002b:  nop
      IL_002c:  br.s       IL_0041

      IL_002e:  ldarg.0
      IL_002f:  stloc.3
      IL_0030:  ldloc.3
      IL_0031:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
      IL_0036:  stloc.s    V_4
      IL_0038:  ldloc.2
      IL_0039:  ldloc.s    V_4
      IL_003b:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
      IL_0040:  nop
      IL_0041:  nop
      IL_0042:  ldarg.1
      IL_0043:  brfalse.s  IL_0047

      IL_0045:  br.s       IL_004b

      IL_0047:  nop
      IL_0048:  nop
      IL_0049:  br.s       IL_005e

      IL_004b:  ldarg.1
      IL_004c:  stloc.3
      IL_004d:  ldloc.3
      IL_004e:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
      IL_0053:  stloc.s    V_4
      IL_0055:  ldloc.2
      IL_0056:  ldloc.s    V_4
      IL_0058:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
      IL_005d:  nop
      IL_005e:  ldloc.2
      IL_005f:  ret
    } 

  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
          test() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
    IL_0006:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
          test2() cil managed
  {
    
    .maxstack  4
    .locals init (class assembly/A V_0,
             class [runtime]System.Collections.Generic.List`1<class assembly/A> V_1)
    IL_0000:  newobj     instance void assembly/A::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.1
    IL_0007:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldloc.0
    IL_000f:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_0014:  ldloc.1
    IL_0015:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
          test3() cil managed
  {
    
    .maxstack  4
    .locals init (class assembly/A V_0,
             class [runtime]System.Collections.Generic.List`1<class assembly/A> V_1)
    IL_0000:  newobj     instance void assembly/A::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.1
    IL_0007:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldloc.0
    IL_000f:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_0014:  ldloc.1
    IL_0015:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
          test4() cil managed
  {
    
    .maxstack  4
    .locals init (class assembly/A V_0,
             class assembly/A V_1,
             class [runtime]System.Collections.Generic.List`1<class assembly/A> V_2)
    IL_0000:  newobj     instance void assembly/A::.ctor()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void assembly/A::.ctor()
    IL_000b:  stloc.1
    IL_000c:  ldc.i4.2
    IL_000d:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
    IL_0012:  stloc.2
    IL_0013:  ldloc.2
    IL_0014:  ldloc.0
    IL_0015:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_001a:  ldloc.2
    IL_001b:  ldloc.1
    IL_001c:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_0021:  ldloc.2
    IL_0022:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






