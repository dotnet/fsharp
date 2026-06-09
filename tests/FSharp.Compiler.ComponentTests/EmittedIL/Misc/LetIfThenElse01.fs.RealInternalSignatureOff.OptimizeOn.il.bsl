




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
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Tuple`4<int32,int32,int32,int32> F<a>(!!a y) cil managed
  {
    
    .maxstack  7
    .locals init (valuetype [runtime]System.DateTime V_0,
             valuetype [runtime]System.DateTime V_1,
             valuetype [runtime]System.DateTime V_2,
             valuetype [runtime]System.DateTime V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0007:  stloc.0
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_000f:  ldc.i4     0x7d0
    IL_0014:  ble.s      IL_001a

    IL_0016:  ldc.i4.1
    IL_0017:  nop
    IL_0018:  br.s       IL_001c

    IL_001a:  ldc.i4.2
    IL_001b:  nop
    IL_001c:  nop
    IL_001d:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0022:  stloc.1
    IL_0023:  ldloca.s   V_1
    IL_0025:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_002a:  ldc.i4     0x7d0
    IL_002f:  ble.s      IL_0035

    IL_0031:  ldc.i4.1
    IL_0032:  nop
    IL_0033:  br.s       IL_0037

    IL_0035:  ldc.i4.2
    IL_0036:  nop
    IL_0037:  nop
    IL_0038:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_003d:  stloc.2
    IL_003e:  ldloca.s   V_2
    IL_0040:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_0045:  ldc.i4     0x7d0
    IL_004a:  bge.s      IL_0050

    IL_004c:  ldc.i4.1
    IL_004d:  nop
    IL_004e:  br.s       IL_0052

    IL_0050:  ldc.i4.2
    IL_0051:  nop
    IL_0052:  nop
    IL_0053:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0058:  stloc.3
    IL_0059:  ldloca.s   V_3
    IL_005b:  call       instance int32 [runtime]System.DateTime::get_Year()
    IL_0060:  ldc.i4     0x7d0
    IL_0065:  bge.s      IL_006b

    IL_0067:  ldc.i4.1
    IL_0068:  nop
    IL_0069:  br.s       IL_006d

    IL_006b:  ldc.i4.2
    IL_006c:  nop
    IL_006d:  newobj     instance void class [runtime]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                            !1,
                                                                                                            !2,
                                                                                                            !3)
    IL_0072:  ret
  } 

  .method assembly specialname static class [runtime]System.Tuple`4<int32,int32,int32,int32> get_arg@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`4<int32,int32,int32,int32> '<StartupCode$assembly>'.$assembly::arg@1
    IL_0005:  ret
  } 

  .property class [runtime]System.Tuple`4<int32,int32,int32,int32>
          arg@1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`4<int32,int32,int32,int32> assembly::get_arg@1()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly class [runtime]System.Tuple`4<int32,int32,int32,int32> arg@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       class [runtime]System.Tuple`4<int32,int32,int32,int32> assembly::F<int32>(!!0)
    IL_0006:  stsfld     class [runtime]System.Tuple`4<int32,int32,int32,int32> '<StartupCode$assembly>'.$assembly::arg@1
    IL_000b:  ret
  } 

} 





