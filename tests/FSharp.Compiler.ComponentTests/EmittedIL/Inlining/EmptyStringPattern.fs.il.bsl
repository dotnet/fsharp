




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





.class public abstract auto ansi sealed TestLibrary
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static string  classifyString(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_0010

    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  ldc.i4.0
    IL_000b:  ceq
    IL_000d:  nop
    IL_000e:  br.s       IL_0012

    IL_0010:  ldc.i4.0
    IL_0011:  nop
    IL_0012:  brtrue.s   IL_0019

    IL_0014:  ldarg.0
    IL_0015:  brfalse.s  IL_001f

    IL_0017:  br.s       IL_0025

    IL_0019:  ldstr      "empty"
    IL_001e:  ret

    IL_001f:  ldstr      "null"
    IL_0024:  ret

    IL_0025:  ldstr      "other"
    IL_002a:  ret
  } 

  .method public static int32  testEmptyStringOnly(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_000e

    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  brtrue.s   IL_000e

    IL_000c:  ldc.i4.1
    IL_000d:  ret

    IL_000e:  ldc.i4.0
    IL_000f:  ret
  } 

  .method public static int32  testBundledNullAndEmpty(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_0017

    IL_0004:  ldarg.0
    IL_0005:  brfalse.s  IL_0013

    IL_0007:  ldarg.0
    IL_0008:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  nop
    IL_0011:  br.s       IL_0015

    IL_0013:  ldc.i4.0
    IL_0014:  nop
    IL_0015:  brfalse.s  IL_0019

    IL_0017:  ldc.i4.0
    IL_0018:  ret

    IL_0019:  ldc.i4.1
    IL_001a:  ret
  } 

  .method public static int32  testBundledEmptyAndNull(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_0010

    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  ldc.i4.0
    IL_000b:  ceq
    IL_000d:  nop
    IL_000e:  br.s       IL_0012

    IL_0010:  ldc.i4.0
    IL_0011:  nop
    IL_0012:  brtrue.s   IL_0017

    IL_0014:  ldarg.0
    IL_0015:  brtrue.s   IL_0019

    IL_0017:  ldc.i4.0
    IL_0018:  ret

    IL_0019:  ldc.i4.1
    IL_001a:  ret
  } 

  .method public static string  useClassifyString(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_0010

    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  ldc.i4.0
    IL_000b:  ceq
    IL_000d:  nop
    IL_000e:  br.s       IL_0012

    IL_0010:  ldc.i4.0
    IL_0011:  nop
    IL_0012:  brtrue.s   IL_0019

    IL_0014:  ldarg.0
    IL_0015:  brfalse.s  IL_001f

    IL_0017:  br.s       IL_0025

    IL_0019:  ldstr      "empty"
    IL_001e:  ret

    IL_001f:  ldstr      "null"
    IL_0024:  ret

    IL_0025:  ldstr      "other"
    IL_002a:  ret
  } 

  .method public static int32  useTestEmptyStringOnly(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_000e

    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  brtrue.s   IL_000e

    IL_000c:  ldc.i4.1
    IL_000d:  ret

    IL_000e:  ldc.i4.0
    IL_000f:  ret
  } 

  .method public static int32  useBundledNullAndEmpty(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_0017

    IL_0004:  ldarg.0
    IL_0005:  brfalse.s  IL_0013

    IL_0007:  ldarg.0
    IL_0008:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  nop
    IL_0011:  br.s       IL_0015

    IL_0013:  ldc.i4.0
    IL_0014:  nop
    IL_0015:  brfalse.s  IL_0019

    IL_0017:  ldc.i4.0
    IL_0018:  ret

    IL_0019:  ldc.i4.1
    IL_001a:  ret
  } 

  .method public static int32  useBundledEmptyAndNull(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_0010

    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  ldc.i4.0
    IL_000b:  ceq
    IL_000d:  nop
    IL_000e:  br.s       IL_0012

    IL_0010:  ldc.i4.0
    IL_0011:  nop
    IL_0012:  brtrue.s   IL_0017

    IL_0014:  ldarg.0
    IL_0015:  brtrue.s   IL_0019

    IL_0017:  ldc.i4.0
    IL_0018:  ret

    IL_0019:  ldc.i4.1
    IL_001a:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$TestLibrary
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 





