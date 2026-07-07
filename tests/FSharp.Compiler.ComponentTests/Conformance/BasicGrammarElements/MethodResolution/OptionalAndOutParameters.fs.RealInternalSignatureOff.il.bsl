




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
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed OutOptionalTests
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public Thing
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public static bool  Do([out] int32& o,
                                   [opt] int32 i) cil managed
    {
      .param [2] = int32(0x00000001)
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stobj      [runtime]System.Int32
      IL_0007:  ldarg.1
      IL_0008:  ldc.i4.7
      IL_0009:  ceq
      IL_000b:  ret
    } 

  } 

  .method assembly specialname static class [runtime]System.Tuple`2<bool,int32> get_patternInput@8() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<bool,int32> '<StartupCode$assembly>'.$OutOptionalTests::patternInput@8
    IL_0005:  ret
  } 

  .method assembly specialname static int32 get_outArg@8() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::outArg@8
    IL_0005:  ret
  } 

  .method assembly specialname static void set_outArg@8(int32 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::outArg@8
    IL_0006:  ret
  } 

  .method assembly specialname static class [runtime]System.Tuple`2<bool,int32> 'get_patternInput@9-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<bool,int32> '<StartupCode$assembly>'.$OutOptionalTests::'patternInput@9-1'
    IL_0005:  ret
  } 

  .method assembly specialname static int32 'get_outArg@9-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::'outArg@9-1'
    IL_0005:  ret
  } 

  .method assembly specialname static void 'set_outArg@9-1'(int32 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::'outArg@9-1'
    IL_0006:  ret
  } 

  .property class [runtime]System.Tuple`2<bool,int32>
          patternInput@8()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<bool,int32> OutOptionalTests::get_patternInput@8()
  } 
  .property int32 outArg@8()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void OutOptionalTests::set_outArg@8(int32)
    .get int32 OutOptionalTests::get_outArg@8()
  } 
  .property class [runtime]System.Tuple`2<bool,int32>
          'patternInput@9-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<bool,int32> OutOptionalTests::'get_patternInput@9-1'()
  } 
  .property int32 'outArg@9-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void OutOptionalTests::'set_outArg@9-1'(int32)
    .get int32 OutOptionalTests::'get_outArg@9-1'()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$OutOptionalTests
       extends [runtime]System.Object
{
  .field static assembly initonly class [runtime]System.Tuple`2<bool,int32> patternInput@8
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 outArg@8
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly initonly class [runtime]System.Tuple`2<bool,int32> 'patternInput@9-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 'outArg@9-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  4
    .locals init (int32& V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::outArg@8
    IL_0006:  ldsflda    int32 '<StartupCode$assembly>'.$OutOptionalTests::outArg@8
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.s   42
    IL_000f:  stobj      [runtime]System.Int32
    IL_0014:  ldc.i4.0
    IL_0015:  call       int32 OutOptionalTests::get_outArg@8()
    IL_001a:  newobj     instance void class [runtime]System.Tuple`2<bool,int32>::.ctor(!0,
                                                                                               !1)
    IL_001f:  stsfld     class [runtime]System.Tuple`2<bool,int32> '<StartupCode$assembly>'.$OutOptionalTests::patternInput@8
    IL_0024:  ldc.i4.0
    IL_0025:  stsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::'outArg@9-1'
    IL_002a:  ldsflda    int32 '<StartupCode$assembly>'.$OutOptionalTests::'outArg@9-1'
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.1
    IL_0032:  stobj      [runtime]System.Int32
    IL_0037:  ldc.i4.0
    IL_0038:  call       int32 OutOptionalTests::'get_outArg@9-1'()
    IL_003d:  newobj     instance void class [runtime]System.Tuple`2<bool,int32>::.ctor(!0,
                                                                                               !1)
    IL_0042:  stsfld     class [runtime]System.Tuple`2<bool,int32> '<StartupCode$assembly>'.$OutOptionalTests::'patternInput@9-1'
    IL_0047:  ret
  } 

} 






