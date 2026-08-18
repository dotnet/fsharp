




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
  .method public static int32  outer(int32 threshold,
                                     int32 factor) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldarg.0
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.s   100
    IL_0008:  tail.
    IL_000a:  call       int32 assembly::a@4(int32,
                                                                              int32,
                                                                              int32)
    IL_000f:  ret
  } 

  .method public static int32  main(string[] _argv) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.7
    IL_0001:  ldc.i4.1
    IL_0002:  tail.
    IL_0004:  call       int32 assembly::outer(int32,
                                                                                int32)
    IL_0009:  ret
  } 

  .method assembly static int32  a@4(int32 threshold,
                                     int32 factor,
                                     int32 n) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.2
    IL_0001:  brtrue.s   IL_0005

    IL_0003:  ldarg.0
    IL_0004:  ret

    IL_0005:  ldarg.2
    IL_0006:  ldc.i4.2
    IL_0007:  rem
    IL_0008:  brtrue.s   IL_0017

    IL_000a:  ldarg.0
    IL_000b:  ldarg.1
    IL_000c:  ldarg.2
    IL_000d:  ldc.i4.1
    IL_000e:  sub
    IL_000f:  tail.
    IL_0011:  call       int32 assembly::b@8(int32,
                                                                              int32,
                                                                              int32)
    IL_0016:  ret

    IL_0017:  ldarg.0
    IL_0018:  ldarg.1
    IL_0019:  ldarg.2
    IL_001a:  ldarg.1
    IL_001b:  sub
    IL_001c:  starg.s    n
    IL_001e:  starg.s    factor
    IL_0020:  starg.s    threshold
    IL_0022:  br.s       IL_0000
  } 

  .method assembly static int32  b@8(int32 threshold,
                                     int32 factor,
                                     int32 n) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.2
    IL_0001:  brtrue.s   IL_0007

    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.1
    IL_0005:  add
    IL_0006:  ret

    IL_0007:  ldarg.2
    IL_0008:  ldc.i4.2
    IL_0009:  rem
    IL_000a:  brtrue.s   IL_0019

    IL_000c:  ldarg.0
    IL_000d:  ldarg.1
    IL_000e:  ldarg.2
    IL_000f:  ldarg.1
    IL_0010:  sub
    IL_0011:  starg.s    n
    IL_0013:  starg.s    factor
    IL_0015:  starg.s    threshold
    IL_0017:  br.s       IL_0000

    IL_0019:  ldarg.0
    IL_001a:  ldarg.1
    IL_001b:  ldarg.2
    IL_001c:  ldc.i4.1
    IL_001d:  sub
    IL_001e:  tail.
    IL_0020:  call       int32 assembly::a@4(int32,
                                                                              int32,
                                                                              int32)
    IL_0025:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
} 






