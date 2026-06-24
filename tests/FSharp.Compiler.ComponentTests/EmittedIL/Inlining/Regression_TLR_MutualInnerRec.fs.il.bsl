




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
  .method public static int32  fifth() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4     0xf4240
    IL_0005:  ldc.i4     0x26ad7
    IL_000a:  tail.
    IL_000c:  call       int32 assembly::fifthMethodFirstCallee@4(int32,
                                                                                       int32)
    IL_0011:  ret
  } 

  .method public static int32  main(string[] _argv) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  tail.
    IL_0002:  call       int32 assembly::fifth()
    IL_0007:  ret
  } 

  .method assembly static int32  fifthMethodFirstCallee@4(int32 iterationCount,
                                                          int32 firstArg) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0006

    IL_0003:  ldc.i4.s   100
    IL_0005:  ret

    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.2
    IL_0008:  rem
    IL_0009:  brtrue.s   IL_0017

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.1
    IL_000d:  sub
    IL_000e:  ldarg.1
    IL_000f:  tail.
    IL_0011:  call       int32 assembly::fifthMethodSecondCallee@8(int32,
                                                                                        int32)
    IL_0016:  ret

    IL_0017:  ldarg.0
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  ldarg.1
    IL_001b:  starg.s    firstArg
    IL_001d:  starg.s    iterationCount
    IL_001f:  br.s       IL_0000
  } 

  .method assembly static int32  fifthMethodSecondCallee@8(int32 iterationCount,
                                                           int32 firstArg) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0006

    IL_0003:  ldc.i4.s   101
    IL_0005:  ret

    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.2
    IL_0008:  rem
    IL_0009:  brtrue.s   IL_0015

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.1
    IL_000d:  sub
    IL_000e:  ldarg.1
    IL_000f:  starg.s    firstArg
    IL_0011:  starg.s    iterationCount
    IL_0013:  br.s       IL_0000

    IL_0015:  ldarg.0
    IL_0016:  ldc.i4.1
    IL_0017:  sub
    IL_0018:  ldarg.1
    IL_0019:  tail.
    IL_001b:  call       int32 assembly::fifthMethodFirstCallee@4(int32,
                                                                                       int32)
    IL_0020:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
} 





