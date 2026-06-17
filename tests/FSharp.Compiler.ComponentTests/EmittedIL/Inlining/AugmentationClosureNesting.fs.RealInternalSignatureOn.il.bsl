




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
    .class auto ansi serializable sealed nested assembly beforefieldinit h@9
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
    {
      .field static assembly initonly class Sample/C/h@9 @_instance
      .method assembly specialname rtspecialname instance void  .ctor() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
        IL_0006:  ret
      } 

      .method public strict virtual instance int32 Invoke(int32 n) cil managed
      {
        
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.1
        IL_0002:  brtrue.s   IL_000a

        IL_0004:  call       int32 Sample/C::Secret()
        IL_0009:  ret

        IL_000a:  ldarg.1
        IL_000b:  ldc.i4.1
        IL_000c:  sub
        IL_000d:  starg.s    n
        IL_000f:  br.s       IL_0000
      } 

      .method private specialname rtspecialname static void  .cctor() cil managed
      {
        
        .maxstack  10
        IL_0000:  newobj     instance void Sample/C/h@9::.ctor()
        IL_0005:  stsfld     class Sample/C/h@9 Sample/C/h@9::@_instance
        IL_000a:  ret
      } 

    } 

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
      IL_0000:  nop
      IL_0001:  volatile.
      IL_0003:  ldsfld     int32 Sample/C::init@2
      IL_0008:  ldc.i4.1
      IL_0009:  bge.s      IL_0014

      IL_000b:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_0010:  nop
      IL_0011:  nop
      IL_0012:  br.s       IL_0015

      IL_0014:  nop
      IL_0015:  ldarg.0
      IL_0016:  stsfld     int32 Sample/C::backing
      IL_001b:  ret
    } 

    .method private static int32  Secret() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  nop
      IL_0001:  volatile.
      IL_0003:  ldsfld     int32 Sample/C::init@2
      IL_0008:  ldc.i4.1
      IL_0009:  bge.s      IL_0014

      IL_000b:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_0010:  nop
      IL_0011:  nop
      IL_0012:  br.s       IL_0015

      IL_0014:  nop
      IL_0015:  ldsfld     int32 Sample/C::backing
      IL_001a:  ldc.i4.1
      IL_001b:  add
      IL_001c:  ret
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

    .method assembly static void  staticInitialization@() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 Sample/C::backing
      IL_0006:  ldc.i4.1
      IL_0007:  volatile.
      IL_0009:  stsfld     int32 Sample/C::init@2
      IL_000e:  ret
    } 

    .method public hidebysig instance int32 Run() cil managed
    {
      
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_0)
      IL_0000:  ldsfld     class Sample/C/h@9 Sample/C/h@9::@_instance
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldc.i4.5
      IL_0008:  tail.
      IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_000f:  ret
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
    IL_0014:  nop
    IL_0015:  newobj     instance void Sample/C::.ctor()
    IL_001a:  callvirt   instance int32 Sample/C::Run()
    IL_001f:  ldc.i4.s   42
    IL_0021:  bne.un.s   IL_0025

    IL_0023:  ldc.i4.0
    IL_0024:  ret

    IL_0025:  ldc.i4.1
    IL_0026:  ret
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

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void Sample/C::staticInitialization@()
    IL_0005:  ret
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
    IL_0000:  call       void Sample::staticInitialization@()
    IL_0005:  ret
  } 

} 






