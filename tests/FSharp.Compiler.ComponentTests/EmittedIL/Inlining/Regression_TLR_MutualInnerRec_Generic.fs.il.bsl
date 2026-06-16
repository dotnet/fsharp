




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
  .method assembly static !!T  a@4<T>(!!T zero,
                                      int32 n,
                                      !!T v) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_0006

    IL_0004:  ldarg.2
    IL_0005:  ret

    IL_0006:  nop
    IL_0007:  ldarg.1
    IL_0008:  ldc.i4.2
    IL_0009:  rem
    IL_000a:  brtrue.s   IL_0019

    IL_000c:  ldarg.0
    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.1
    IL_000f:  sub
    IL_0010:  ldarg.2
    IL_0011:  tail.
    IL_0013:  call       !!0 assembly::b@8<!!0>(!!0,
                                                                             int32,
                                                                             !!0)
    IL_0018:  ret

    IL_0019:  ldarg.0
    IL_001a:  ldarg.1
    IL_001b:  ldc.i4.1
    IL_001c:  sub
    IL_001d:  ldarg.2
    IL_001e:  starg.s    v
    IL_0020:  starg.s    n
    IL_0022:  starg.s    zero
    IL_0024:  br.s       IL_0000
  } 

  .method assembly static !!T  b@8<T>(!!T zero,
                                      int32 n,
                                      !!T v) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_0006

    IL_0004:  ldarg.0
    IL_0005:  ret

    IL_0006:  nop
    IL_0007:  ldarg.1
    IL_0008:  ldc.i4.2
    IL_0009:  rem
    IL_000a:  brtrue.s   IL_0019

    IL_000c:  ldarg.0
    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.1
    IL_000f:  sub
    IL_0010:  ldarg.2
    IL_0011:  starg.s    v
    IL_0013:  starg.s    n
    IL_0015:  starg.s    zero
    IL_0017:  br.s       IL_0000

    IL_0019:  ldarg.0
    IL_001a:  ldarg.1
    IL_001b:  ldc.i4.1
    IL_001c:  sub
    IL_001d:  ldarg.2
    IL_001e:  tail.
    IL_0020:  call       !!0 assembly::a@4<!!0>(!!0,
                                                                             int32,
                                                                             !!0)
    IL_0025:  ret
  } 

  .method public static int32  main(string[] _argv) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.0
    IL_0003:  call       !!0 assembly::outer<int32>(!!0,
                                                                                 !!0)
    IL_0008:  ldc.i4.1
    IL_0009:  bne.un.s   IL_000d

    IL_000b:  ldc.i4.0
    IL_000c:  ret

    IL_000d:  ldc.i4.1
    IL_000e:  ret
  } 

  .method public static !!T  outer<T>(!!T initial,
                                      !!T zero) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (!!T V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  ldc.i4     0x3e8
    IL_0008:  ldarg.0
    IL_0009:  tail.
    IL_000b:  call       !!0 assembly::a@4<!!0>(!!0,
                                                                             int32,
                                                                             !!0)
    IL_0010:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
} 





