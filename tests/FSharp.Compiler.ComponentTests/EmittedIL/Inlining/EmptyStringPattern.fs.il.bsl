




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
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000f

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  nop
    IL_000d:  br.s       IL_0011

    IL_000f:  ldc.i4.0
    IL_0010:  nop
    IL_0011:  brtrue.s   IL_0018

    IL_0013:  ldarg.0
    IL_0014:  brfalse.s  IL_001e

    IL_0016:  br.s       IL_0024

    IL_0018:  ldstr      "empty"
    IL_001d:  ret

    IL_001e:  ldstr      "null"
    IL_0023:  ret

    IL_0024:  ldstr      "other"
    IL_0029:  ret
  } 

  .method public static int32  testEmptyStringOnly(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000d

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0009:  brtrue.s   IL_000d

    IL_000b:  ldc.i4.1
    IL_000c:  ret

    IL_000d:  ldc.i4.0
    IL_000e:  ret
  } 

  .method public static int32  testBundledNullAndEmpty(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0016

    IL_0003:  ldarg.0
    IL_0004:  brfalse.s  IL_0012

    IL_0006:  ldarg.0
    IL_0007:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  nop
    IL_0010:  br.s       IL_0014

    IL_0012:  ldc.i4.0
    IL_0013:  nop
    IL_0014:  brfalse.s  IL_0018

    IL_0016:  ldc.i4.0
    IL_0017:  ret

    IL_0018:  ldc.i4.1
    IL_0019:  ret
  } 

  .method public static int32  testBundledEmptyAndNull(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000f

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  nop
    IL_000d:  br.s       IL_0011

    IL_000f:  ldc.i4.0
    IL_0010:  nop
    IL_0011:  brtrue.s   IL_0016

    IL_0013:  ldarg.0
    IL_0014:  brtrue.s   IL_0018

    IL_0016:  ldc.i4.0
    IL_0017:  ret

    IL_0018:  ldc.i4.1
    IL_0019:  ret
  } 

  .method public static string  useClassifyString(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000f

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  nop
    IL_000d:  br.s       IL_0011

    IL_000f:  ldc.i4.0
    IL_0010:  nop
    IL_0011:  brtrue.s   IL_0018

    IL_0013:  ldarg.0
    IL_0014:  brfalse.s  IL_001e

    IL_0016:  br.s       IL_0024

    IL_0018:  ldstr      "empty"
    IL_001d:  ret

    IL_001e:  ldstr      "null"
    IL_0023:  ret

    IL_0024:  ldstr      "other"
    IL_0029:  ret
  } 

  .method public static int32  useTestEmptyStringOnly(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000d

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0009:  brtrue.s   IL_000d

    IL_000b:  ldc.i4.1
    IL_000c:  ret

    IL_000d:  ldc.i4.0
    IL_000e:  ret
  } 

  .method public static int32  useBundledNullAndEmpty(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0016

    IL_0003:  ldarg.0
    IL_0004:  brfalse.s  IL_0012

    IL_0006:  ldarg.0
    IL_0007:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  nop
    IL_0010:  br.s       IL_0014

    IL_0012:  ldc.i4.0
    IL_0013:  nop
    IL_0014:  brfalse.s  IL_0018

    IL_0016:  ldc.i4.0
    IL_0017:  ret

    IL_0018:  ldc.i4.1
    IL_0019:  ret
  } 

  .method public static int32  useBundledEmptyAndNull(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_000f

    IL_0003:  ldarg.0
    IL_0004:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  nop
    IL_000d:  br.s       IL_0011

    IL_000f:  ldc.i4.0
    IL_0010:  nop
    IL_0011:  brtrue.s   IL_0016

    IL_0013:  ldarg.0
    IL_0014:  brtrue.s   IL_0018

    IL_0016:  ldc.i4.0
    IL_0017:  ret

    IL_0018:  ldc.i4.1
    IL_0019:  ret
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





