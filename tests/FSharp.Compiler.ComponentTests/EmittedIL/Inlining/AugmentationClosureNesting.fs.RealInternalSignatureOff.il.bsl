




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





.class public abstract auto ansi sealed Sample
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly int32 backing
    .field static assembly int32 init@2
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public static void  Set(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 Sample/C::init@2
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0013

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  nop
      IL_0010:  nop
      IL_0011:  br.s       IL_0014

      IL_0013:  nop
      IL_0014:  ldarg.0
      IL_0015:  stsfld     int32 Sample/C::backing
      IL_001a:  ret
    } 

    .method assembly static int32  Secret() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 Sample/C::init@2
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0013

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  nop
      IL_0010:  nop
      IL_0011:  br.s       IL_0014

      IL_0013:  nop
      IL_0014:  ldsfld     int32 Sample/C::backing
      IL_0019:  ldc.i4.1
      IL_001a:  add
      IL_001b:  ret
    } 

    .method public hidebysig instance int32 Run() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.5
      IL_0001:  call       int32 Sample::h@9(int32)
      IL_0006:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Sample::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Sample::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

  } 

  .method public static int32  main(string[] _arg1) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Sample::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Sample::init@
    IL_000b:  pop
    IL_000c:  ldc.i4.s   41
    IL_000e:  call       void Sample/C::Set(int32)
    IL_0013:  nop
    IL_0014:  newobj     instance void Sample/C::.ctor()
    IL_0019:  callvirt   instance int32 Sample/C::Run()
    IL_001e:  ldc.i4.s   42
    IL_0020:  bne.un.s   IL_0024

    IL_0022:  ldc.i4.0
    IL_0023:  ret

    IL_0024:  ldc.i4.1
    IL_0025:  ret
  } 

  .method assembly static int32  h@9(int32 n) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0009

    IL_0003:  call       int32 Sample/C::Secret()
    IL_0008:  ret

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  starg.s    n
    IL_000e:  br.s       IL_0000
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Sample
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 Sample/C::backing
    IL_0006:  ldc.i4.1
    IL_0007:  volatile.
    IL_0009:  stsfld     int32 Sample/C::init@2
    IL_000e:  ret
  } 

} 






