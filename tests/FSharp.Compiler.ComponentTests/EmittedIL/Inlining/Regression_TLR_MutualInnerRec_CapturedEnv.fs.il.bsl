




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
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  brtrue.s   IL_0006

    IL_0004:  ldarg.0
    IL_0005:  ret

    IL_0006:  nop
    IL_0007:  ldarg.2
    IL_0008:  ldc.i4.2
    IL_0009:  rem
    IL_000a:  brtrue.s   IL_0019

    IL_000c:  ldarg.0
    IL_000d:  ldarg.1
    IL_000e:  ldarg.2
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  tail.
    IL_0013:  call       int32 assembly::b@8(int32,
                                                                              int32,
                                                                              int32)
    IL_0018:  ret

    IL_0019:  ldarg.0
    IL_001a:  ldarg.1
    IL_001b:  ldarg.2
    IL_001c:  ldarg.1
    IL_001d:  sub
    IL_001e:  starg.s    n
    IL_0020:  starg.s    factor
    IL_0022:  starg.s    threshold
    IL_0024:  br.s       IL_0000
  } 

  .method assembly static int32  b@8(int32 threshold,
                                     int32 factor,
                                     int32 n) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  brtrue.s   IL_0008

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  ret

    IL_0008:  nop
    IL_0009:  ldarg.2
    IL_000a:  ldc.i4.2
    IL_000b:  rem
    IL_000c:  brtrue.s   IL_001b

    IL_000e:  ldarg.0
    IL_000f:  ldarg.1
    IL_0010:  ldarg.2
    IL_0011:  ldarg.1
    IL_0012:  sub
    IL_0013:  starg.s    n
    IL_0015:  starg.s    factor
    IL_0017:  starg.s    threshold
    IL_0019:  br.s       IL_0000

    IL_001b:  ldarg.0
    IL_001c:  ldarg.1
    IL_001d:  ldarg.2
    IL_001e:  ldc.i4.1
    IL_001f:  sub
    IL_0020:  tail.
    IL_0022:  call       int32 assembly::a@4(int32,
                                                                              int32,
                                                                              int32)
    IL_0027:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
} 





