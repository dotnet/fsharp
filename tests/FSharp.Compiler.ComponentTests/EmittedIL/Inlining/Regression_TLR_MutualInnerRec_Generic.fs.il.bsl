




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

  .method public static int32  main(string[] _argv) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.0
    IL_0002:  call       !!0 assembly::outer<int32>(!!0,
                                                                                 !!0)
    IL_0007:  ldc.i4.1
    IL_0008:  bne.un.s   IL_000c

    IL_000a:  ldc.i4.0
    IL_000b:  ret

    IL_000c:  ldc.i4.1
    IL_000d:  ret
  } 

  .method assembly static !!T  a@4<T>(!!T zero,
                                      int32 n,
                                      !!T v) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_0005

    IL_0003:  ldarg.2
    IL_0004:  ret

    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.2
    IL_0007:  rem
    IL_0008:  brtrue.s   IL_0017

    IL_000a:  ldarg.0
    IL_000b:  ldarg.1
    IL_000c:  ldc.i4.1
    IL_000d:  sub
    IL_000e:  ldarg.2
    IL_000f:  tail.
    IL_0011:  call       !!0 assembly::b@8<!!0>(!!0,
                                                                             int32,
                                                                             !!0)
    IL_0016:  ret

    IL_0017:  ldarg.0
    IL_0018:  ldarg.1
    IL_0019:  ldc.i4.1
    IL_001a:  sub
    IL_001b:  ldarg.2
    IL_001c:  starg.s    v
    IL_001e:  starg.s    n
    IL_0020:  starg.s    zero
    IL_0022:  br.s       IL_0000
  } 

  .method assembly static !!T  b@8<T>(!!T zero,
                                      int32 n,
                                      !!T v) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_0005

    IL_0003:  ldarg.0
    IL_0004:  ret

    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.2
    IL_0007:  rem
    IL_0008:  brtrue.s   IL_0017

    IL_000a:  ldarg.0
    IL_000b:  ldarg.1
    IL_000c:  ldc.i4.1
    IL_000d:  sub
    IL_000e:  ldarg.2
    IL_000f:  starg.s    v
    IL_0011:  starg.s    n
    IL_0013:  starg.s    zero
    IL_0015:  br.s       IL_0000

    IL_0017:  ldarg.0
    IL_0018:  ldarg.1
    IL_0019:  ldc.i4.1
    IL_001a:  sub
    IL_001b:  ldarg.2
    IL_001c:  tail.
    IL_001e:  call       !!0 assembly::a@4<!!0>(!!0,
                                                                             int32,
                                                                             !!0)
    IL_0023:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
} 





