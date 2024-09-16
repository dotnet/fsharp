




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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
{
  
  
}
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public Foo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly int32 x
    .field static assembly int32 init@1
    .method private specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public specialname static int32 get_X() cil managed
    {
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 assembly/Foo::init@1
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 assembly/Foo::x
      IL_0016:  ret
    } 

    .method public specialname static void set_X(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 assembly/Foo::init@1
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 assembly/Foo::x
      IL_0017:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly specialname static void staticInitialization@() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 assembly/Foo::x
      IL_0006:  ldc.i4.1
      IL_0007:  volatile.
      IL_0009:  stsfld     int32 assembly/Foo::init@1
      IL_000e:  ret
    } 

    .property int32 X()
    {
      .set void assembly/Foo::set_X(int32)
      .get int32 assembly/Foo::get_X()
    } 
  } 

  .class abstract auto ansi sealed nested public Exts2
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  Foo.X.Static(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.2
      IL_0002:  mul
      IL_0003:  call       void assembly/Foo::set_X(int32)
      IL_0008:  ret
    } 

  } 

  .class abstract auto ansi sealed nested public Exts
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  Foo.X.Static(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       void assembly/Foo::set_X(int32)
      IL_0006:  ret
    } 

  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void assembly/Foo::staticInitialization@()
    IL_0005:  ldc.i4.1
    IL_0006:  call       void assembly/Foo::set_X(int32)
    IL_000b:  call       int32 assembly/Foo::get_X()
    IL_0010:  ldc.i4.1
    IL_0011:  beq.s      IL_001b

    IL_0013:  ldc.i4.1
    IL_0014:  call       void [runtime]System.Environment::Exit(int32)
    IL_0019:  br.s       IL_001b

    IL_001b:  ldc.i4.4
    IL_001c:  call       void assembly/Foo::set_X(int32)
    IL_0021:  call       int32 assembly/Foo::get_X()
    IL_0026:  ldc.i4.4
    IL_0027:  beq.s      IL_0031

    IL_0029:  ldc.i4.2
    IL_002a:  call       void [runtime]System.Environment::Exit(int32)
    IL_002f:  br.s       IL_0031

    IL_0031:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






