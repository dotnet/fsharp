




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
    IL_0002:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0007:  brfalse.s  IL_000e

    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_0014

    IL_000c:  br.s       IL_001a

    IL_000e:  ldstr      "empty"
    IL_0013:  ret

    IL_0014:  ldstr      "null"
    IL_0019:  ret

    IL_001a:  ldstr      "other"
    IL_001f:  ret
  } 

  .method public static int32  testEmptyStringOnly(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0007:  brtrue.s   IL_000b

    IL_0009:  ldc.i4.1
    IL_000a:  ret

    IL_000b:  ldc.i4.0
    IL_000c:  ret
  } 

  .method public static int32  testBundledNullAndEmpty(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_000c

    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  brtrue.s   IL_000e

    IL_000c:  ldc.i4.0
    IL_000d:  ret

    IL_000e:  ldc.i4.1
    IL_000f:  ret
  } 

  .method public static int32  testBundledEmptyAndNull(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0007:  brfalse.s  IL_000c

    IL_0009:  ldarg.0
    IL_000a:  brtrue.s   IL_000e

    IL_000c:  ldc.i4.0
    IL_000d:  ret

    IL_000e:  ldc.i4.1
    IL_000f:  ret
  } 

  .method public static string  useClassifyString(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0007:  brfalse.s  IL_000e

    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_0014

    IL_000c:  br.s       IL_001a

    IL_000e:  ldstr      "empty"
    IL_0013:  ret

    IL_0014:  ldstr      "null"
    IL_0019:  ret

    IL_001a:  ldstr      "other"
    IL_001f:  ret
  } 

  .method public static int32  useTestEmptyStringOnly(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0007:  brtrue.s   IL_000b

    IL_0009:  ldc.i4.1
    IL_000a:  ret

    IL_000b:  ldc.i4.0
    IL_000c:  ret
  } 

  .method public static int32  useBundledNullAndEmpty(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_000c

    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  brtrue.s   IL_000e

    IL_000c:  ldc.i4.0
    IL_000d:  ret

    IL_000e:  ldc.i4.1
    IL_000f:  ret
  } 

  .method public static int32  useBundledEmptyAndNull(string s) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0007:  brfalse.s  IL_000c

    IL_0009:  ldarg.0
    IL_000a:  brtrue.s   IL_000e

    IL_000c:  ldc.i4.0
    IL_000d:  ret

    IL_000e:  ldc.i4.1
    IL_000f:  ret
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





