




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
  .method public specialname static int32[] get_r() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::r@6
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_w() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$assembly::w@7
    IL_0005:  ret
  } 

  .property int32[] r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] assembly::get_r()
  } 
  .property int32[] w()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] assembly::get_w()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32[] r@6
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] w@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  7
    .locals init (int32[] V_0,
             int32[] V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6,
             int32 V_7,
             int32 V_8,
             int32 V_9,
             int32 V_10)
    IL_0000:  ldc.i4.8
    IL_0001:  ldc.i4.1
    IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0007:  dup
    IL_0008:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::r@6
    IL_000d:  stloc.0
    IL_000e:  ldc.i4.5
    IL_000f:  ldc.i4.2
    IL_0010:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0015:  dup
    IL_0016:  stsfld     int32[] '<StartupCode$assembly>'.$assembly::w@7
    IL_001b:  stloc.1
    IL_001c:  ldc.i4.0
    IL_001d:  stloc.3
    IL_001e:  call       int32[] assembly::get_r()
    IL_0023:  ldlen
    IL_0024:  conv.i4
    IL_0025:  stloc.s    V_4
    IL_0027:  call       int32[] assembly::get_w()
    IL_002c:  ldlen
    IL_002d:  conv.i4
    IL_002e:  stloc.s    V_5
    IL_0030:  ldloc.s    V_4
    IL_0032:  ldloc.s    V_5
    IL_0034:  bge.s      IL_003b

    IL_0036:  ldloc.s    V_4
    IL_0038:  nop
    IL_0039:  br.s       IL_003e

    IL_003b:  ldloc.s    V_5
    IL_003d:  nop
    IL_003e:  ldc.i4.1
    IL_003f:  sub
    IL_0040:  stloc.2
    IL_0041:  ldloc.2
    IL_0042:  ldloc.3
    IL_0043:  blt.s      IL_00a3

    IL_0045:  ldc.i4.0
    IL_0046:  stloc.s    V_7
    IL_0048:  call       int32[] assembly::get_r()
    IL_004d:  ldlen
    IL_004e:  conv.i4
    IL_004f:  stloc.s    V_8
    IL_0051:  call       int32[] assembly::get_w()
    IL_0056:  ldlen
    IL_0057:  conv.i4
    IL_0058:  stloc.s    V_9
    IL_005a:  ldloc.s    V_8
    IL_005c:  ldloc.s    V_9
    IL_005e:  bge.s      IL_0065

    IL_0060:  ldloc.s    V_8
    IL_0062:  nop
    IL_0063:  br.s       IL_0068

    IL_0065:  ldloc.s    V_9
    IL_0067:  nop
    IL_0068:  ldc.i4.1
    IL_0069:  sub
    IL_006a:  stloc.s    V_6
    IL_006c:  ldloc.s    V_6
    IL_006e:  ldloc.s    V_7
    IL_0070:  blt.s      IL_0099

    IL_0072:  call       int32[] assembly::get_r()
    IL_0077:  ldloc.s    V_7
    IL_0079:  call       int32[] assembly::get_r()
    IL_007e:  ldloc.s    V_7
    IL_0080:  ldelem.i4
    IL_0081:  call       int32[] assembly::get_w()
    IL_0086:  ldloc.s    V_7
    IL_0088:  ldelem.i4
    IL_0089:  add
    IL_008a:  stelem.i4
    IL_008b:  ldloc.s    V_7
    IL_008d:  ldc.i4.1
    IL_008e:  add
    IL_008f:  stloc.s    V_7
    IL_0091:  ldloc.s    V_7
    IL_0093:  ldloc.s    V_6
    IL_0095:  ldc.i4.1
    IL_0096:  add
    IL_0097:  bne.un.s   IL_0072

    IL_0099:  ldloc.3
    IL_009a:  ldc.i4.1
    IL_009b:  add
    IL_009c:  stloc.3
    IL_009d:  ldloc.3
    IL_009e:  ldloc.2
    IL_009f:  ldc.i4.1
    IL_00a0:  add
    IL_00a1:  bne.un.s   IL_0045

    IL_00a3:  nop
    IL_00a4:  nop
    IL_00a5:  call       int32[] assembly::get_r()
    IL_00aa:  ldc.i4.0
    IL_00ab:  ldelem.i4
    IL_00ac:  ldc.i4.s   11
    IL_00ae:  bne.un.s   IL_00b4

    IL_00b0:  ldc.i4.0
    IL_00b1:  nop
    IL_00b2:  br.s       IL_00b6

    IL_00b4:  ldc.i4.1
    IL_00b5:  nop
    IL_00b6:  stloc.s    V_10
    IL_00b8:  ldloc.s    V_10
    IL_00ba:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_00bf:  pop
    IL_00c0:  ret
  } 

} 






