




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
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_2,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_3,
               int32 V_4,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_5,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_6,
               class [runtime]System.Collections.Generic.List`1<class assembly/A> V_7,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_8,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_9,
               class assembly/A V_10,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_11,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_12,
               class assembly/A V_13)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  stloc.2
      IL_0004:  ldloc.2
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000d

      IL_0009:  ldloc.0
      IL_000a:  nop
      IL_000b:  br.s       IL_0013

      IL_000d:  ldloc.2
      IL_000e:  stloc.3
      IL_000f:  ldloc.0
      IL_0010:  ldc.i4.1
      IL_0011:  add
      IL_0012:  nop
      IL_0013:  stloc.1
      IL_0014:  ldarg.1
      IL_0015:  stloc.s    V_5
      IL_0017:  ldloc.s    V_5
      IL_0019:  brfalse.s  IL_001d

      IL_001b:  br.s       IL_0021

      IL_001d:  ldloc.1
      IL_001e:  nop
      IL_001f:  br.s       IL_0029

      IL_0021:  ldloc.s    V_5
      IL_0023:  stloc.s    V_6
      IL_0025:  ldloc.1
      IL_0026:  ldc.i4.1
      IL_0027:  add
      IL_0028:  nop
      IL_0029:  stloc.s    V_4
      IL_002b:  ldloc.s    V_4
      IL_002d:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
      IL_0032:  stloc.s    V_7
      IL_0034:  ldarg.0
      IL_0035:  stloc.s    V_8
      IL_0037:  ldloc.s    V_8
      IL_0039:  brfalse.s  IL_003d

      IL_003b:  br.s       IL_0041

      IL_003d:  nop
      IL_003e:  nop
      IL_003f:  br.s       IL_0058

      IL_0041:  ldloc.s    V_8
      IL_0043:  stloc.s    V_9
      IL_0045:  ldloc.s    V_9
      IL_0047:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
      IL_004c:  stloc.s    V_10
      IL_004e:  ldloc.s    V_7
      IL_0050:  ldloc.s    V_10
      IL_0052:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
      IL_0057:  nop
      IL_0058:  ldarg.1
      IL_0059:  stloc.s    V_11
      IL_005b:  ldloc.s    V_11
      IL_005d:  brfalse.s  IL_0061

      IL_005f:  br.s       IL_0065

      IL_0061:  nop
      IL_0062:  nop
      IL_0063:  br.s       IL_007c

      IL_0065:  ldloc.s    V_11
      IL_0067:  stloc.s    V_12
      IL_0069:  ldloc.s    V_12
      IL_006b:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
      IL_0070:  stloc.s    V_13
      IL_0072:  ldloc.s    V_7
      IL_0074:  ldloc.s    V_13
      IL_0076:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
      IL_007b:  nop
      IL_007c:  ldloc.s    V_7
      IL_007e:  ret
    } 

  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> test() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldnull
    IL_0002:  call       class [runtime]System.Collections.Generic.List`1<class assembly/A> assembly/C::F(class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>,
                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>)
    IL_0007:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> test2() cil managed
  {
    
    .maxstack  8
    IL_0000:  newobj     instance void assembly/A::.ctor()
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::Some(!0)
    IL_000a:  ldnull
    IL_000b:  call       class [runtime]System.Collections.Generic.List`1<class assembly/A> assembly/C::F(class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>,
                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>)
    IL_0010:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> test3() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  newobj     instance void assembly/A::.ctor()
    IL_0006:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::Some(!0)
    IL_000b:  call       class [runtime]System.Collections.Generic.List`1<class assembly/A> assembly/C::F(class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>,
                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>)
    IL_0010:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> test4() cil managed
  {
    
    .maxstack  8
    IL_0000:  newobj     instance void assembly/A::.ctor()
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::Some(!0)
    IL_000a:  newobj     instance void assembly/A::.ctor()
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::Some(!0)
    IL_0014:  call       class [runtime]System.Collections.Generic.List`1<class assembly/A> assembly/C::F(class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>,
                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>)
    IL_0019:  ret
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





