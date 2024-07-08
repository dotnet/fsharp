




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
    .field assembly int32 X@
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldc.i4.0
      IL_000a:  stfld      int32 assembly/Foo::X@
      IL_000f:  ret
    } 

    .method public hidebysig specialname instance int32  get_X() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/Foo::X@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance void  set_X(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/Foo::X@
      IL_0007:  ret
    } 

    .property instance int32 X()
    {
      .set instance void assembly/Foo::set_X(int32)
      .get instance int32 assembly/Foo::get_X()
    } 
  } 

  .class auto ansi serializable nested public FooExt
         extends [runtime]System.Object
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public static class assembly/Foo 
            X(class assembly/Foo f,
              int32 i) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/Foo::X@
      IL_0007:  ldarg.0
      IL_0008:  ret
    } 

  } 

  .method public specialname static class assembly/Foo get_f() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/Foo '<StartupCode$assembly>'.$assembly$fsx::f@12
    IL_0005:  ret
  } 

  .method assembly specialname static class assembly/Foo get_f@9() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/Foo '<StartupCode$assembly>'.$assembly$fsx::'f@9-1'
    IL_0005:  ret
  } 

  .method assembly specialname static class assembly/Foo 'get_f@9-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/Foo '<StartupCode$assembly>'.$assembly$fsx::'f@9-2'
    IL_0005:  ret
  } 

  .property class assembly/Foo
          f()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class assembly/Foo assembly::get_f()
  } 
  .property class assembly/Foo
          f@9()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class assembly/Foo assembly::get_f@9()
  } 
  .property class assembly/Foo
          'f@9-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class assembly/Foo assembly::'get_f@9-1'()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .field static assembly initonly class assembly/Foo f@12
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly initonly class assembly/Foo 'f@9-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly initonly class assembly/Foo 'f@9-2'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  4
    IL_0000:  newobj     instance void assembly/Foo::.ctor()
    IL_0005:  stsfld     class assembly/Foo '<StartupCode$assembly>'.$assembly$fsx::f@12
    IL_000a:  call       class assembly/Foo assembly::get_f()
    IL_000f:  ldc.i4.1
    IL_0010:  stfld      int32 assembly/Foo::X@
    IL_0015:  call       class assembly/Foo assembly::get_f()
    IL_001a:  pop
    IL_001b:  call       class assembly/Foo assembly::get_f()
    IL_0020:  ldfld      int32 assembly/Foo::X@
    IL_0025:  ldc.i4.1
    IL_0026:  beq.s      IL_0030

    IL_0028:  ldc.i4.1
    IL_0029:  call       void [runtime]System.Environment::Exit(int32)
    IL_002e:  br.s       IL_0030

    IL_0030:  call       class assembly/Foo assembly::get_f()
    IL_0035:  ldc.i4.1
    IL_0036:  stfld      int32 assembly/Foo::X@
    IL_003b:  call       class assembly/Foo assembly::get_f()
    IL_0040:  stsfld     class assembly/Foo '<StartupCode$assembly>'.$assembly$fsx::'f@9-2'
    IL_0045:  call       class assembly/Foo assembly::'get_f@9-1'()
    IL_004a:  ldc.i4.2
    IL_004b:  stfld      int32 assembly/Foo::X@
    IL_0050:  call       class assembly/Foo assembly::'get_f@9-1'()
    IL_0055:  stsfld     class assembly/Foo '<StartupCode$assembly>'.$assembly$fsx::'f@9-1'
    IL_005a:  call       class assembly/Foo assembly::get_f@9()
    IL_005f:  ldc.i4.3
    IL_0060:  stfld      int32 assembly/Foo::X@
    IL_0065:  call       class assembly/Foo assembly::get_f@9()
    IL_006a:  pop
    IL_006b:  call       class assembly/Foo assembly::get_f()
    IL_0070:  ldfld      int32 assembly/Foo::X@
    IL_0075:  ldc.i4.3
    IL_0076:  beq.s      IL_0080

    IL_0078:  ldc.i4.2
    IL_0079:  call       void [runtime]System.Environment::Exit(int32)
    IL_007e:  br.s       IL_0080

    IL_0080:  ret
  } 

} 






