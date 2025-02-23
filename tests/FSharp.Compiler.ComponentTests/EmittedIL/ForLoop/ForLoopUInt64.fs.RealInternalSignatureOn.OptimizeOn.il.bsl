




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
  .class abstract auto ansi sealed nested public Up
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  constEmpty() cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               uint64 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  conv.i8
      IL_0002:  stloc.0
      IL_0003:  ldc.i4.s   10
      IL_0005:  conv.i8
      IL_0006:  stloc.1
      IL_0007:  br.s       IL_0019

      IL_0009:  ldloc.1
      IL_000a:  call       void assembly::set_c(uint64)
      IL_000f:  ldloc.1
      IL_0010:  ldc.i4.1
      IL_0011:  conv.i8
      IL_0012:  add
      IL_0013:  stloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.1
      IL_0016:  conv.i8
      IL_0017:  add
      IL_0018:  stloc.0
      IL_0019:  ldloc.0
      IL_001a:  ldc.i4.0
      IL_001b:  conv.i8
      IL_001c:  blt.un.s   IL_0009

      IL_001e:  ret
    } 

    .method public static void  constNonEmpty() cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               uint64 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  conv.i8
      IL_0002:  stloc.0
      IL_0003:  ldc.i4.1
      IL_0004:  conv.i8
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_0018

      IL_0008:  ldloc.1
      IL_0009:  call       void assembly::set_c(uint64)
      IL_000e:  ldloc.1
      IL_000f:  ldc.i4.1
      IL_0010:  conv.i8
      IL_0011:  add
      IL_0012:  stloc.1
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.1
      IL_0015:  conv.i8
      IL_0016:  add
      IL_0017:  stloc.0
      IL_0018:  ldloc.0
      IL_0019:  ldc.i4.s   10
      IL_001b:  conv.i8
      IL_001c:  blt.un.s   IL_0008

      IL_001e:  ret
    } 

    .method public static void  constFinish(uint64 start) cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               uint64 V_1,
               uint64 V_2)
      IL_0000:  ldc.i4.s   10
      IL_0002:  conv.i8
      IL_0003:  ldarg.0
      IL_0004:  bge.un.s   IL_000b

      IL_0006:  ldc.i4.0
      IL_0007:  conv.i8
      IL_0008:  nop
      IL_0009:  br.s       IL_0014

      IL_000b:  ldc.i4.s   10
      IL_000d:  conv.i8
      IL_000e:  ldarg.0
      IL_000f:  sub
      IL_0010:  ldc.i4.1
      IL_0011:  conv.i8
      IL_0012:  add.ovf.un
      IL_0013:  nop
      IL_0014:  stloc.0
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.1
      IL_0018:  ldarg.0
      IL_0019:  stloc.2
      IL_001a:  br.s       IL_002c

      IL_001c:  ldloc.2
      IL_001d:  call       void assembly::set_c(uint64)
      IL_0022:  ldloc.2
      IL_0023:  ldc.i4.1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.2
      IL_0027:  ldloc.1
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.1
      IL_002c:  ldloc.1
      IL_002d:  ldloc.0
      IL_002e:  blt.un.s   IL_001c

      IL_0030:  ret
    } 

    .method public static void  constStart(uint64 finish) cil managed
    {
      
      .maxstack  4
      .locals init (uint64 V_0,
               uint64 V_1,
               uint64 V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  conv.i8
      IL_0003:  bge.un.s   IL_000a

      IL_0005:  ldc.i4.0
      IL_0006:  conv.i8
      IL_0007:  nop
      IL_0008:  br.s       IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldc.i4.1
      IL_000c:  conv.i8
      IL_000d:  sub
      IL_000e:  ldc.i4.1
      IL_000f:  conv.i8
      IL_0010:  add.ovf.un
      IL_0011:  nop
      IL_0012:  stloc.0
      IL_0013:  ldc.i4.0
      IL_0014:  conv.i8
      IL_0015:  stloc.1
      IL_0016:  ldc.i4.1
      IL_0017:  conv.i8
      IL_0018:  stloc.2
      IL_0019:  br.s       IL_002b

      IL_001b:  ldloc.2
      IL_001c:  call       void assembly::set_c(uint64)
      IL_0021:  ldloc.2
      IL_0022:  ldc.i4.1
      IL_0023:  conv.i8
      IL_0024:  add
      IL_0025:  stloc.2
      IL_0026:  ldloc.1
      IL_0027:  ldc.i4.1
      IL_0028:  conv.i8
      IL_0029:  add
      IL_002a:  stloc.1
      IL_002b:  ldloc.1
      IL_002c:  ldloc.0
      IL_002d:  blt.un.s   IL_001b

      IL_002f:  ret
    } 

    .method public static void  annotatedStart(uint64 start,
                                               uint64 finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint64 V_0,
               bool V_1,
               uint64 V_2,
               uint64 V_3,
               uint64 V_4)
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  bge.un.s   IL_0009

      IL_0004:  ldc.i4.0
      IL_0005:  conv.i8
      IL_0006:  nop
      IL_0007:  br.s       IL_000d

      IL_0009:  ldarg.1
      IL_000a:  ldarg.0
      IL_000b:  sub
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  bne.un.s   IL_0036

      IL_0013:  ldc.i4.1
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.2
      IL_0018:  ldarg.0
      IL_0019:  stloc.3
      IL_001a:  br.s       IL_0032

      IL_001c:  ldloc.3
      IL_001d:  call       void assembly::set_c(uint64)
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4.1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  conv.i8
      IL_002f:  cgt.un
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brtrue.s   IL_001c

      IL_0035:  ret

      IL_0036:  ldarg.1
      IL_0037:  ldarg.0
      IL_0038:  bge.un.s   IL_003f

      IL_003a:  ldc.i4.0
      IL_003b:  conv.i8
      IL_003c:  nop
      IL_003d:  br.s       IL_0046

      IL_003f:  ldarg.1
      IL_0040:  ldarg.0
      IL_0041:  sub
      IL_0042:  ldc.i4.1
      IL_0043:  conv.i8
      IL_0044:  add.ovf.un
      IL_0045:  nop
      IL_0046:  stloc.2
      IL_0047:  ldc.i4.0
      IL_0048:  conv.i8
      IL_0049:  stloc.3
      IL_004a:  ldarg.0
      IL_004b:  stloc.s    V_4
      IL_004d:  br.s       IL_0062

      IL_004f:  ldloc.s    V_4
      IL_0051:  call       void assembly::set_c(uint64)
      IL_0056:  ldloc.s    V_4
      IL_0058:  ldc.i4.1
      IL_0059:  conv.i8
      IL_005a:  add
      IL_005b:  stloc.s    V_4
      IL_005d:  ldloc.3
      IL_005e:  ldc.i4.1
      IL_005f:  conv.i8
      IL_0060:  add
      IL_0061:  stloc.3
      IL_0062:  ldloc.3
      IL_0063:  ldloc.2
      IL_0064:  blt.un.s   IL_004f

      IL_0066:  ret
    } 

    .method public static void  annotatedFinish(uint64 start,
                                                uint64 finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint64 V_0,
               bool V_1,
               uint64 V_2,
               uint64 V_3,
               uint64 V_4)
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  bge.un.s   IL_0009

      IL_0004:  ldc.i4.0
      IL_0005:  conv.i8
      IL_0006:  nop
      IL_0007:  br.s       IL_000d

      IL_0009:  ldarg.1
      IL_000a:  ldarg.0
      IL_000b:  sub
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  bne.un.s   IL_0036

      IL_0013:  ldc.i4.1
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.2
      IL_0018:  ldarg.0
      IL_0019:  stloc.3
      IL_001a:  br.s       IL_0032

      IL_001c:  ldloc.3
      IL_001d:  call       void assembly::set_c(uint64)
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4.1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  conv.i8
      IL_002f:  cgt.un
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brtrue.s   IL_001c

      IL_0035:  ret

      IL_0036:  ldarg.1
      IL_0037:  ldarg.0
      IL_0038:  bge.un.s   IL_003f

      IL_003a:  ldc.i4.0
      IL_003b:  conv.i8
      IL_003c:  nop
      IL_003d:  br.s       IL_0046

      IL_003f:  ldarg.1
      IL_0040:  ldarg.0
      IL_0041:  sub
      IL_0042:  ldc.i4.1
      IL_0043:  conv.i8
      IL_0044:  add.ovf.un
      IL_0045:  nop
      IL_0046:  stloc.2
      IL_0047:  ldc.i4.0
      IL_0048:  conv.i8
      IL_0049:  stloc.3
      IL_004a:  ldarg.0
      IL_004b:  stloc.s    V_4
      IL_004d:  br.s       IL_0062

      IL_004f:  ldloc.s    V_4
      IL_0051:  call       void assembly::set_c(uint64)
      IL_0056:  ldloc.s    V_4
      IL_0058:  ldc.i4.1
      IL_0059:  conv.i8
      IL_005a:  add
      IL_005b:  stloc.s    V_4
      IL_005d:  ldloc.3
      IL_005e:  ldc.i4.1
      IL_005f:  conv.i8
      IL_0060:  add
      IL_0061:  stloc.3
      IL_0062:  ldloc.3
      IL_0063:  ldloc.2
      IL_0064:  blt.un.s   IL_004f

      IL_0066:  ret
    } 

    .method public static void  inferredStartAndFinish(uint64 start,
                                                       uint64 finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint64 V_0,
               bool V_1,
               uint64 V_2,
               uint64 V_3,
               uint64 V_4)
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  bge.un.s   IL_0009

      IL_0004:  ldc.i4.0
      IL_0005:  conv.i8
      IL_0006:  nop
      IL_0007:  br.s       IL_000d

      IL_0009:  ldarg.1
      IL_000a:  ldarg.0
      IL_000b:  sub
      IL_000c:  nop
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.m1
      IL_0010:  conv.i8
      IL_0011:  bne.un.s   IL_0036

      IL_0013:  ldc.i4.1
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  conv.i8
      IL_0017:  stloc.2
      IL_0018:  ldarg.0
      IL_0019:  stloc.3
      IL_001a:  br.s       IL_0032

      IL_001c:  ldloc.3
      IL_001d:  call       void assembly::set_c(uint64)
      IL_0022:  ldloc.3
      IL_0023:  ldc.i4.1
      IL_0024:  conv.i8
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.2
      IL_0028:  ldc.i4.1
      IL_0029:  conv.i8
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  conv.i8
      IL_002f:  cgt.un
      IL_0031:  stloc.1
      IL_0032:  ldloc.1
      IL_0033:  brtrue.s   IL_001c

      IL_0035:  ret

      IL_0036:  ldarg.1
      IL_0037:  ldarg.0
      IL_0038:  bge.un.s   IL_003f

      IL_003a:  ldc.i4.0
      IL_003b:  conv.i8
      IL_003c:  nop
      IL_003d:  br.s       IL_0046

      IL_003f:  ldarg.1
      IL_0040:  ldarg.0
      IL_0041:  sub
      IL_0042:  ldc.i4.1
      IL_0043:  conv.i8
      IL_0044:  add.ovf.un
      IL_0045:  nop
      IL_0046:  stloc.2
      IL_0047:  ldc.i4.0
      IL_0048:  conv.i8
      IL_0049:  stloc.3
      IL_004a:  ldarg.0
      IL_004b:  stloc.s    V_4
      IL_004d:  br.s       IL_0062

      IL_004f:  ldloc.s    V_4
      IL_0051:  call       void assembly::set_c(uint64)
      IL_0056:  ldloc.s    V_4
      IL_0058:  ldc.i4.1
      IL_0059:  conv.i8
      IL_005a:  add
      IL_005b:  stloc.s    V_4
      IL_005d:  ldloc.3
      IL_005e:  ldc.i4.1
      IL_005f:  conv.i8
      IL_0060:  add
      IL_0061:  stloc.3
      IL_0062:  ldloc.3
      IL_0063:  ldloc.2
      IL_0064:  blt.un.s   IL_004f

      IL_0066:  ret
    } 

  } 

  .field static assembly uint64 c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static uint64 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     uint64 assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(uint64 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     uint64 assembly::c@1
    IL_0006:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stsfld     uint64 assembly::c@1
    IL_0007:  ret
  } 

  .property uint64 c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(uint64)
    .get uint64 assembly::get_c()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






