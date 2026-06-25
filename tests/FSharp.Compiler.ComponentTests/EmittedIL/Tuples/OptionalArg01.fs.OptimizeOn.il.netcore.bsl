




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
    .method public specialname rtspecialname instance void  .ctor() cil managed
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
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> F(class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> x1, class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> x2) cil managed
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
      IL_0001:  ldarg.0
      IL_0002:  brfalse.s  IL_0006

      IL_0004:  br.s       IL_000a

      IL_0006:  ldc.i4.0
      IL_0007:  nop
      IL_0008:  br.s       IL_000c

      IL_000a:  ldc.i4.1
      IL_000b:  nop
      IL_000c:  stloc.0
      IL_000d:  ldarg.1
      IL_000e:  brfalse.s  IL_0012

      IL_0010:  br.s       IL_0016

      IL_0012:  ldloc.0
      IL_0013:  nop
      IL_0014:  br.s       IL_001a

      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.1
      IL_0018:  add
      IL_0019:  nop
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
      IL_0021:  stloc.2
      IL_0022:  ldarg.0
      IL_0023:  brfalse.s  IL_0027

      IL_0025:  br.s       IL_002b

      IL_0027:  nop
      IL_0028:  nop
      IL_0029:  br.s       IL_003e

      IL_002b:  ldarg.0
      IL_002c:  stloc.3
      IL_002d:  ldloc.3
      IL_002e:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
      IL_0033:  stloc.s    V_4
      IL_0035:  ldloc.2
      IL_0036:  ldloc.s    V_4
      IL_0038:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
      IL_003d:  nop
      IL_003e:  ldarg.1
      IL_003f:  brfalse.s  IL_0043

      IL_0041:  br.s       IL_0047

      IL_0043:  nop
      IL_0044:  nop
      IL_0045:  br.s       IL_005a

      IL_0047:  ldarg.1
      IL_0048:  stloc.3
      IL_0049:  ldloc.3
      IL_004a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
      IL_004f:  stloc.s    V_4
      IL_0051:  ldloc.2
      IL_0052:  ldloc.s    V_4
      IL_0054:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
      IL_0059:  nop
      IL_005a:  ldloc.2
      IL_005b:  ret
    } 

  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> test() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
    IL_0006:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> test2() cil managed
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

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> test3() cil managed
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

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> test4() cil managed
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





