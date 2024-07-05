




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
  .method public specialname static native int get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     native int '<StartupCode$assembly>'.$assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(native int 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     native int '<StartupCode$assembly>'.$assembly::c@1
    IL_0006:  ret
  } 

  .method public static void  f0() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0xa
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0x0
    IL_0042:  conv.i
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f00() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0xa
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0x0
    IL_0042:  conv.i
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f1() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0x1
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0xa
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f2(native int start) cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             bool V_1,
             native int V_2,
             native int V_3,
             native int V_4)
    IL_0000:  ldc.i8     0xa
    IL_0009:  conv.i
    IL_000a:  ldarg.0
    IL_000b:  bge.s      IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.i
    IL_0017:  nop
    IL_0018:  br.s       IL_0027

    IL_001a:  ldc.i8     0xa
    IL_0023:  conv.i
    IL_0024:  ldarg.0
    IL_0025:  sub
    IL_0026:  nop
    IL_0027:  stloc.0
    IL_0028:  sizeof     [runtime]System.IntPtr
    IL_002e:  ldc.i4.4
    IL_002f:  bne.un.s   IL_0041

    IL_0031:  ldloc.0
    IL_0032:  ldc.i8     0xffffffff
    IL_003b:  conv.u
    IL_003c:  ceq
    IL_003e:  nop
    IL_003f:  br.s       IL_004f

    IL_0041:  ldloc.0
    IL_0042:  ldc.i8     0xffffffffffffffff
    IL_004b:  conv.u
    IL_004c:  ceq
    IL_004e:  nop
    IL_004f:  brfalse.s  IL_0094

    IL_0051:  ldc.i4.1
    IL_0052:  stloc.1
    IL_0053:  ldc.i8     0x0
    IL_005c:  conv.i
    IL_005d:  stloc.2
    IL_005e:  ldarg.0
    IL_005f:  stloc.3
    IL_0060:  br.s       IL_0090

    IL_0062:  ldloc.3
    IL_0063:  call       void assembly::set_c(native int)
    IL_0068:  ldloc.3
    IL_0069:  ldc.i8     0x1
    IL_0072:  conv.i
    IL_0073:  add
    IL_0074:  stloc.3
    IL_0075:  ldloc.2
    IL_0076:  ldc.i8     0x1
    IL_007f:  conv.i
    IL_0080:  add
    IL_0081:  stloc.2
    IL_0082:  ldloc.2
    IL_0083:  ldc.i8     0x0
    IL_008c:  conv.i
    IL_008d:  cgt.un
    IL_008f:  stloc.1
    IL_0090:  ldloc.1
    IL_0091:  brtrue.s   IL_0062

    IL_0093:  ret

    IL_0094:  ldc.i8     0xa
    IL_009d:  conv.i
    IL_009e:  ldarg.0
    IL_009f:  bge.s      IL_00ae

    IL_00a1:  ldc.i8     0x0
    IL_00aa:  conv.i
    IL_00ab:  nop
    IL_00ac:  br.s       IL_00c6

    IL_00ae:  ldc.i8     0xa
    IL_00b7:  conv.i
    IL_00b8:  ldarg.0
    IL_00b9:  sub
    IL_00ba:  ldc.i8     0x1
    IL_00c3:  conv.i
    IL_00c4:  add.ovf.un
    IL_00c5:  nop
    IL_00c6:  stloc.2
    IL_00c7:  ldc.i8     0x0
    IL_00d0:  conv.i
    IL_00d1:  stloc.3
    IL_00d2:  ldarg.0
    IL_00d3:  stloc.s    V_4
    IL_00d5:  br.s       IL_00fa

    IL_00d7:  ldloc.s    V_4
    IL_00d9:  call       void assembly::set_c(native int)
    IL_00de:  ldloc.s    V_4
    IL_00e0:  ldc.i8     0x1
    IL_00e9:  conv.i
    IL_00ea:  add
    IL_00eb:  stloc.s    V_4
    IL_00ed:  ldloc.3
    IL_00ee:  ldc.i8     0x1
    IL_00f7:  conv.i
    IL_00f8:  add
    IL_00f9:  stloc.3
    IL_00fa:  ldloc.3
    IL_00fb:  ldloc.2
    IL_00fc:  blt.un.s   IL_00d7

    IL_00fe:  ret
  } 

  .method public static void  f3(native int finish) cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             bool V_1,
             native int V_2,
             native int V_3,
             native int V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x1
    IL_000a:  conv.i
    IL_000b:  bge.s      IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.i
    IL_0017:  nop
    IL_0018:  br.s       IL_0027

    IL_001a:  ldarg.0
    IL_001b:  ldc.i8     0x1
    IL_0024:  conv.i
    IL_0025:  sub
    IL_0026:  nop
    IL_0027:  stloc.0
    IL_0028:  sizeof     [runtime]System.IntPtr
    IL_002e:  ldc.i4.4
    IL_002f:  bne.un.s   IL_0041

    IL_0031:  ldloc.0
    IL_0032:  ldc.i8     0xffffffff
    IL_003b:  conv.u
    IL_003c:  ceq
    IL_003e:  nop
    IL_003f:  br.s       IL_004f

    IL_0041:  ldloc.0
    IL_0042:  ldc.i8     0xffffffffffffffff
    IL_004b:  conv.u
    IL_004c:  ceq
    IL_004e:  nop
    IL_004f:  brfalse.s  IL_009d

    IL_0051:  ldc.i4.1
    IL_0052:  stloc.1
    IL_0053:  ldc.i8     0x0
    IL_005c:  conv.i
    IL_005d:  stloc.2
    IL_005e:  ldc.i8     0x1
    IL_0067:  conv.i
    IL_0068:  stloc.3
    IL_0069:  br.s       IL_0099

    IL_006b:  ldloc.3
    IL_006c:  call       void assembly::set_c(native int)
    IL_0071:  ldloc.3
    IL_0072:  ldc.i8     0x1
    IL_007b:  conv.i
    IL_007c:  add
    IL_007d:  stloc.3
    IL_007e:  ldloc.2
    IL_007f:  ldc.i8     0x1
    IL_0088:  conv.i
    IL_0089:  add
    IL_008a:  stloc.2
    IL_008b:  ldloc.2
    IL_008c:  ldc.i8     0x0
    IL_0095:  conv.i
    IL_0096:  cgt.un
    IL_0098:  stloc.1
    IL_0099:  ldloc.1
    IL_009a:  brtrue.s   IL_006b

    IL_009c:  ret

    IL_009d:  ldarg.0
    IL_009e:  ldc.i8     0x1
    IL_00a7:  conv.i
    IL_00a8:  bge.s      IL_00b7

    IL_00aa:  ldc.i8     0x0
    IL_00b3:  conv.i
    IL_00b4:  nop
    IL_00b5:  br.s       IL_00cf

    IL_00b7:  ldarg.0
    IL_00b8:  ldc.i8     0x1
    IL_00c1:  conv.i
    IL_00c2:  sub
    IL_00c3:  ldc.i8     0x1
    IL_00cc:  conv.i
    IL_00cd:  add.ovf.un
    IL_00ce:  nop
    IL_00cf:  stloc.2
    IL_00d0:  ldc.i8     0x0
    IL_00d9:  conv.i
    IL_00da:  stloc.3
    IL_00db:  ldc.i8     0x1
    IL_00e4:  conv.i
    IL_00e5:  stloc.s    V_4
    IL_00e7:  br.s       IL_010c

    IL_00e9:  ldloc.s    V_4
    IL_00eb:  call       void assembly::set_c(native int)
    IL_00f0:  ldloc.s    V_4
    IL_00f2:  ldc.i8     0x1
    IL_00fb:  conv.i
    IL_00fc:  add
    IL_00fd:  stloc.s    V_4
    IL_00ff:  ldloc.3
    IL_0100:  ldc.i8     0x1
    IL_0109:  conv.i
    IL_010a:  add
    IL_010b:  stloc.3
    IL_010c:  ldloc.3
    IL_010d:  ldloc.2
    IL_010e:  blt.un.s   IL_00e9

    IL_0110:  ret
  } 

  .method public static void  f4(native int start,
                                 native int finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (native int V_0,
             bool V_1,
             native int V_2,
             native int V_3,
             native int V_4)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0011

    IL_0004:  ldc.i8     0x0
    IL_000d:  conv.i
    IL_000e:  nop
    IL_000f:  br.s       IL_0015

    IL_0011:  ldarg.1
    IL_0012:  ldarg.0
    IL_0013:  sub
    IL_0014:  nop
    IL_0015:  stloc.0
    IL_0016:  sizeof     [runtime]System.IntPtr
    IL_001c:  ldc.i4.4
    IL_001d:  bne.un.s   IL_002f

    IL_001f:  ldloc.0
    IL_0020:  ldc.i8     0xffffffff
    IL_0029:  conv.u
    IL_002a:  ceq
    IL_002c:  nop
    IL_002d:  br.s       IL_003d

    IL_002f:  ldloc.0
    IL_0030:  ldc.i8     0xffffffffffffffff
    IL_0039:  conv.u
    IL_003a:  ceq
    IL_003c:  nop
    IL_003d:  brfalse.s  IL_0082

    IL_003f:  ldc.i4.1
    IL_0040:  stloc.1
    IL_0041:  ldc.i8     0x0
    IL_004a:  conv.i
    IL_004b:  stloc.2
    IL_004c:  ldarg.0
    IL_004d:  stloc.3
    IL_004e:  br.s       IL_007e

    IL_0050:  ldloc.3
    IL_0051:  call       void assembly::set_c(native int)
    IL_0056:  ldloc.3
    IL_0057:  ldc.i8     0x1
    IL_0060:  conv.i
    IL_0061:  add
    IL_0062:  stloc.3
    IL_0063:  ldloc.2
    IL_0064:  ldc.i8     0x1
    IL_006d:  conv.i
    IL_006e:  add
    IL_006f:  stloc.2
    IL_0070:  ldloc.2
    IL_0071:  ldc.i8     0x0
    IL_007a:  conv.i
    IL_007b:  cgt.un
    IL_007d:  stloc.1
    IL_007e:  ldloc.1
    IL_007f:  brtrue.s   IL_0050

    IL_0081:  ret

    IL_0082:  ldarg.1
    IL_0083:  ldarg.0
    IL_0084:  bge.s      IL_0093

    IL_0086:  ldc.i8     0x0
    IL_008f:  conv.i
    IL_0090:  nop
    IL_0091:  br.s       IL_00a2

    IL_0093:  ldarg.1
    IL_0094:  ldarg.0
    IL_0095:  sub
    IL_0096:  ldc.i8     0x1
    IL_009f:  conv.i
    IL_00a0:  add.ovf.un
    IL_00a1:  nop
    IL_00a2:  stloc.2
    IL_00a3:  ldc.i8     0x0
    IL_00ac:  conv.i
    IL_00ad:  stloc.3
    IL_00ae:  ldarg.0
    IL_00af:  stloc.s    V_4
    IL_00b1:  br.s       IL_00d6

    IL_00b3:  ldloc.s    V_4
    IL_00b5:  call       void assembly::set_c(native int)
    IL_00ba:  ldloc.s    V_4
    IL_00bc:  ldc.i8     0x1
    IL_00c5:  conv.i
    IL_00c6:  add
    IL_00c7:  stloc.s    V_4
    IL_00c9:  ldloc.3
    IL_00ca:  ldc.i8     0x1
    IL_00d3:  conv.i
    IL_00d4:  add
    IL_00d5:  stloc.3
    IL_00d6:  ldloc.3
    IL_00d7:  ldloc.2
    IL_00d8:  blt.un.s   IL_00b3

    IL_00da:  ret
  } 

  .method public static void  f5() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0x1
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0xa
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f6() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0x1
    IL_0014:  conv.i
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native int)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x2
    IL_0028:  conv.i
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.i
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0x5
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f7(native int start) cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1,
             native int V_2)
    IL_0000:  ldc.i8     0xa
    IL_0009:  conv.i
    IL_000a:  ldarg.0
    IL_000b:  bge.s      IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.i
    IL_0017:  nop
    IL_0018:  br.s       IL_003d

    IL_001a:  ldc.i8     0xa
    IL_0023:  conv.i
    IL_0024:  ldarg.0
    IL_0025:  sub
    IL_0026:  ldc.i8     0x2
    IL_002f:  conv.i
    IL_0030:  div.un
    IL_0031:  ldc.i8     0x1
    IL_003a:  conv.i
    IL_003b:  add.ovf.un
    IL_003c:  nop
    IL_003d:  stloc.0
    IL_003e:  ldc.i8     0x0
    IL_0047:  conv.i
    IL_0048:  stloc.1
    IL_0049:  ldarg.0
    IL_004a:  stloc.2
    IL_004b:  br.s       IL_006d

    IL_004d:  ldloc.2
    IL_004e:  call       void assembly::set_c(native int)
    IL_0053:  ldloc.2
    IL_0054:  ldc.i8     0x2
    IL_005d:  conv.i
    IL_005e:  add
    IL_005f:  stloc.2
    IL_0060:  ldloc.1
    IL_0061:  ldc.i8     0x1
    IL_006a:  conv.i
    IL_006b:  add
    IL_006c:  stloc.1
    IL_006d:  ldloc.1
    IL_006e:  ldloc.0
    IL_006f:  blt.un.s   IL_004d

    IL_0071:  ret
  } 

  .method public static void  f8(native int step) cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             bool V_1,
             native int V_2,
             native int V_3,
             native int V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x0
    IL_000a:  conv.i
    IL_000b:  bne.un.s   IL_002b

    IL_000d:  ldc.i8     0x1
    IL_0016:  conv.i
    IL_0017:  ldarg.0
    IL_0018:  ldc.i8     0xa
    IL_0021:  conv.i
    IL_0022:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_0027:  pop
    IL_0028:  nop
    IL_0029:  br.s       IL_002c

    IL_002b:  nop
    IL_002c:  ldc.i8     0x0
    IL_0035:  conv.i
    IL_0036:  ldarg.0
    IL_0037:  bge.s      IL_0076

    IL_0039:  ldc.i8     0xa
    IL_0042:  conv.i
    IL_0043:  ldc.i8     0x1
    IL_004c:  conv.i
    IL_004d:  bge.s      IL_005c

    IL_004f:  ldc.i8     0x0
    IL_0058:  conv.i
    IL_0059:  nop
    IL_005a:  br.s       IL_00bd

    IL_005c:  ldc.i8     0xa
    IL_0065:  conv.i
    IL_0066:  ldc.i8     0x1
    IL_006f:  conv.i
    IL_0070:  sub
    IL_0071:  ldarg.0
    IL_0072:  div.un
    IL_0073:  nop
    IL_0074:  br.s       IL_00bd

    IL_0076:  ldc.i8     0x1
    IL_007f:  conv.i
    IL_0080:  ldc.i8     0xa
    IL_0089:  conv.i
    IL_008a:  bge.s      IL_0099

    IL_008c:  ldc.i8     0x0
    IL_0095:  conv.i
    IL_0096:  nop
    IL_0097:  br.s       IL_00bd

    IL_0099:  ldc.i8     0x1
    IL_00a2:  conv.i
    IL_00a3:  ldc.i8     0xa
    IL_00ac:  conv.i
    IL_00ad:  sub
    IL_00ae:  ldarg.0
    IL_00af:  not
    IL_00b0:  ldc.i8     0x1
    IL_00b9:  conv.i
    IL_00ba:  add
    IL_00bb:  div.un
    IL_00bc:  nop
    IL_00bd:  stloc.0
    IL_00be:  sizeof     [runtime]System.IntPtr
    IL_00c4:  ldc.i4.4
    IL_00c5:  bne.un.s   IL_00d7

    IL_00c7:  ldloc.0
    IL_00c8:  ldc.i8     0xffffffff
    IL_00d1:  conv.u
    IL_00d2:  ceq
    IL_00d4:  nop
    IL_00d5:  br.s       IL_00e5

    IL_00d7:  ldloc.0
    IL_00d8:  ldc.i8     0xffffffffffffffff
    IL_00e1:  conv.u
    IL_00e2:  ceq
    IL_00e4:  nop
    IL_00e5:  brfalse.s  IL_012a

    IL_00e7:  ldc.i4.1
    IL_00e8:  stloc.1
    IL_00e9:  ldc.i8     0x0
    IL_00f2:  conv.i
    IL_00f3:  stloc.2
    IL_00f4:  ldc.i8     0x1
    IL_00fd:  conv.i
    IL_00fe:  stloc.3
    IL_00ff:  br.s       IL_0126

    IL_0101:  ldloc.3
    IL_0102:  call       void assembly::set_c(native int)
    IL_0107:  ldloc.3
    IL_0108:  ldarg.0
    IL_0109:  add
    IL_010a:  stloc.3
    IL_010b:  ldloc.2
    IL_010c:  ldc.i8     0x1
    IL_0115:  conv.i
    IL_0116:  add
    IL_0117:  stloc.2
    IL_0118:  ldloc.2
    IL_0119:  ldc.i8     0x0
    IL_0122:  conv.i
    IL_0123:  cgt.un
    IL_0125:  stloc.1
    IL_0126:  ldloc.1
    IL_0127:  brtrue.s   IL_0101

    IL_0129:  ret

    IL_012a:  ldc.i8     0x0
    IL_0133:  conv.i
    IL_0134:  ldarg.0
    IL_0135:  bge.s      IL_0182

    IL_0137:  ldc.i8     0xa
    IL_0140:  conv.i
    IL_0141:  ldc.i8     0x1
    IL_014a:  conv.i
    IL_014b:  bge.s      IL_015d

    IL_014d:  ldc.i8     0x0
    IL_0156:  conv.i
    IL_0157:  nop
    IL_0158:  br         IL_01d4

    IL_015d:  ldc.i8     0xa
    IL_0166:  conv.i
    IL_0167:  ldc.i8     0x1
    IL_0170:  conv.i
    IL_0171:  sub
    IL_0172:  ldarg.0
    IL_0173:  div.un
    IL_0174:  ldc.i8     0x1
    IL_017d:  conv.i
    IL_017e:  add.ovf.un
    IL_017f:  nop
    IL_0180:  br.s       IL_01d4

    IL_0182:  ldc.i8     0x1
    IL_018b:  conv.i
    IL_018c:  ldc.i8     0xa
    IL_0195:  conv.i
    IL_0196:  bge.s      IL_01a5

    IL_0198:  ldc.i8     0x0
    IL_01a1:  conv.i
    IL_01a2:  nop
    IL_01a3:  br.s       IL_01d4

    IL_01a5:  ldc.i8     0x1
    IL_01ae:  conv.i
    IL_01af:  ldc.i8     0xa
    IL_01b8:  conv.i
    IL_01b9:  sub
    IL_01ba:  ldarg.0
    IL_01bb:  not
    IL_01bc:  ldc.i8     0x1
    IL_01c5:  conv.i
    IL_01c6:  add
    IL_01c7:  div.un
    IL_01c8:  ldc.i8     0x1
    IL_01d1:  conv.i
    IL_01d2:  add.ovf.un
    IL_01d3:  nop
    IL_01d4:  stloc.2
    IL_01d5:  ldc.i8     0x0
    IL_01de:  conv.i
    IL_01df:  stloc.3
    IL_01e0:  ldc.i8     0x1
    IL_01e9:  conv.i
    IL_01ea:  stloc.s    V_4
    IL_01ec:  br.s       IL_0208

    IL_01ee:  ldloc.s    V_4
    IL_01f0:  call       void assembly::set_c(native int)
    IL_01f5:  ldloc.s    V_4
    IL_01f7:  ldarg.0
    IL_01f8:  add
    IL_01f9:  stloc.s    V_4
    IL_01fb:  ldloc.3
    IL_01fc:  ldc.i8     0x1
    IL_0205:  conv.i
    IL_0206:  add
    IL_0207:  stloc.3
    IL_0208:  ldloc.3
    IL_0209:  ldloc.2
    IL_020a:  blt.un.s   IL_01ee

    IL_020c:  ret
  } 

  .method public static void  f9(native int finish) cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             native int V_1,
             native int V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x1
    IL_000a:  conv.i
    IL_000b:  bge.s      IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.i
    IL_0017:  nop
    IL_0018:  br.s       IL_003d

    IL_001a:  ldarg.0
    IL_001b:  ldc.i8     0x1
    IL_0024:  conv.i
    IL_0025:  sub
    IL_0026:  ldc.i8     0x2
    IL_002f:  conv.i
    IL_0030:  div.un
    IL_0031:  ldc.i8     0x1
    IL_003a:  conv.i
    IL_003b:  add.ovf.un
    IL_003c:  nop
    IL_003d:  stloc.0
    IL_003e:  ldc.i8     0x0
    IL_0047:  conv.i
    IL_0048:  stloc.1
    IL_0049:  ldc.i8     0x1
    IL_0052:  conv.i
    IL_0053:  stloc.2
    IL_0054:  br.s       IL_0076

    IL_0056:  ldloc.2
    IL_0057:  call       void assembly::set_c(native int)
    IL_005c:  ldloc.2
    IL_005d:  ldc.i8     0x2
    IL_0066:  conv.i
    IL_0067:  add
    IL_0068:  stloc.2
    IL_0069:  ldloc.1
    IL_006a:  ldc.i8     0x1
    IL_0073:  conv.i
    IL_0074:  add
    IL_0075:  stloc.1
    IL_0076:  ldloc.1
    IL_0077:  ldloc.0
    IL_0078:  blt.un.s   IL_0056

    IL_007a:  ret
  } 

  .method public static void  f10(native int start,
                                  native int step,
                                  native int finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (native int V_0,
             bool V_1,
             native int V_2,
             native int V_3,
             native int V_4)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i8     0x0
    IL_000a:  conv.i
    IL_000b:  bne.un.s   IL_0019

    IL_000d:  ldarg.2
    IL_000e:  ldarg.1
    IL_000f:  ldarg.2
    IL_0010:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_0015:  pop
    IL_0016:  nop
    IL_0017:  br.s       IL_001a

    IL_0019:  nop
    IL_001a:  ldc.i8     0x0
    IL_0023:  conv.i
    IL_0024:  ldarg.1
    IL_0025:  bge.s      IL_0040

    IL_0027:  ldarg.2
    IL_0028:  ldarg.2
    IL_0029:  bge.s      IL_0038

    IL_002b:  ldc.i8     0x0
    IL_0034:  conv.i
    IL_0035:  nop
    IL_0036:  br.s       IL_0063

    IL_0038:  ldarg.2
    IL_0039:  ldarg.2
    IL_003a:  sub
    IL_003b:  ldarg.1
    IL_003c:  div.un
    IL_003d:  nop
    IL_003e:  br.s       IL_0063

    IL_0040:  ldarg.2
    IL_0041:  ldarg.2
    IL_0042:  bge.s      IL_0051

    IL_0044:  ldc.i8     0x0
    IL_004d:  conv.i
    IL_004e:  nop
    IL_004f:  br.s       IL_0063

    IL_0051:  ldarg.2
    IL_0052:  ldarg.2
    IL_0053:  sub
    IL_0054:  ldarg.1
    IL_0055:  not
    IL_0056:  ldc.i8     0x1
    IL_005f:  conv.i
    IL_0060:  add
    IL_0061:  div.un
    IL_0062:  nop
    IL_0063:  stloc.0
    IL_0064:  sizeof     [runtime]System.IntPtr
    IL_006a:  ldc.i4.4
    IL_006b:  bne.un.s   IL_007d

    IL_006d:  ldloc.0
    IL_006e:  ldc.i8     0xffffffff
    IL_0077:  conv.u
    IL_0078:  ceq
    IL_007a:  nop
    IL_007b:  br.s       IL_008b

    IL_007d:  ldloc.0
    IL_007e:  ldc.i8     0xffffffffffffffff
    IL_0087:  conv.u
    IL_0088:  ceq
    IL_008a:  nop
    IL_008b:  brfalse.s  IL_00c7

    IL_008d:  ldc.i4.1
    IL_008e:  stloc.1
    IL_008f:  ldc.i8     0x0
    IL_0098:  conv.i
    IL_0099:  stloc.2
    IL_009a:  ldarg.2
    IL_009b:  stloc.3
    IL_009c:  br.s       IL_00c3

    IL_009e:  ldloc.3
    IL_009f:  call       void assembly::set_c(native int)
    IL_00a4:  ldloc.3
    IL_00a5:  ldarg.1
    IL_00a6:  add
    IL_00a7:  stloc.3
    IL_00a8:  ldloc.2
    IL_00a9:  ldc.i8     0x1
    IL_00b2:  conv.i
    IL_00b3:  add
    IL_00b4:  stloc.2
    IL_00b5:  ldloc.2
    IL_00b6:  ldc.i8     0x0
    IL_00bf:  conv.i
    IL_00c0:  cgt.un
    IL_00c2:  stloc.1
    IL_00c3:  ldloc.1
    IL_00c4:  brtrue.s   IL_009e

    IL_00c6:  ret

    IL_00c7:  ldc.i8     0x0
    IL_00d0:  conv.i
    IL_00d1:  ldarg.1
    IL_00d2:  bge.s      IL_00f8

    IL_00d4:  ldarg.2
    IL_00d5:  ldarg.2
    IL_00d6:  bge.s      IL_00e5

    IL_00d8:  ldc.i8     0x0
    IL_00e1:  conv.i
    IL_00e2:  nop
    IL_00e3:  br.s       IL_0126

    IL_00e5:  ldarg.2
    IL_00e6:  ldarg.2
    IL_00e7:  sub
    IL_00e8:  ldarg.1
    IL_00e9:  div.un
    IL_00ea:  ldc.i8     0x1
    IL_00f3:  conv.i
    IL_00f4:  add.ovf.un
    IL_00f5:  nop
    IL_00f6:  br.s       IL_0126

    IL_00f8:  ldarg.2
    IL_00f9:  ldarg.2
    IL_00fa:  bge.s      IL_0109

    IL_00fc:  ldc.i8     0x0
    IL_0105:  conv.i
    IL_0106:  nop
    IL_0107:  br.s       IL_0126

    IL_0109:  ldarg.2
    IL_010a:  ldarg.2
    IL_010b:  sub
    IL_010c:  ldarg.1
    IL_010d:  not
    IL_010e:  ldc.i8     0x1
    IL_0117:  conv.i
    IL_0118:  add
    IL_0119:  div.un
    IL_011a:  ldc.i8     0x1
    IL_0123:  conv.i
    IL_0124:  add.ovf.un
    IL_0125:  nop
    IL_0126:  stloc.2
    IL_0127:  ldc.i8     0x0
    IL_0130:  conv.i
    IL_0131:  stloc.3
    IL_0132:  ldarg.2
    IL_0133:  stloc.s    V_4
    IL_0135:  br.s       IL_0151

    IL_0137:  ldloc.s    V_4
    IL_0139:  call       void assembly::set_c(native int)
    IL_013e:  ldloc.s    V_4
    IL_0140:  ldarg.1
    IL_0141:  add
    IL_0142:  stloc.s    V_4
    IL_0144:  ldloc.3
    IL_0145:  ldc.i8     0x1
    IL_014e:  conv.i
    IL_014f:  add
    IL_0150:  stloc.3
    IL_0151:  ldloc.3
    IL_0152:  ldloc.2
    IL_0153:  blt.un.s   IL_0137

    IL_0155:  ret
  } 

  .method public static void  f11(native int start,
                                  native int finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (native int V_0,
             native int V_1,
             native int V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x0
    IL_000a:  conv.i
    IL_000b:  ldarg.1
    IL_000c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_0011:  pop
    IL_0012:  ldc.i8     0x0
    IL_001b:  conv.i
    IL_001c:  stloc.0
    IL_001d:  ldc.i8     0x0
    IL_0026:  conv.i
    IL_0027:  stloc.1
    IL_0028:  ldarg.0
    IL_0029:  stloc.2
    IL_002a:  br.s       IL_004c

    IL_002c:  ldloc.2
    IL_002d:  call       void assembly::set_c(native int)
    IL_0032:  ldloc.2
    IL_0033:  ldc.i8     0x0
    IL_003c:  conv.i
    IL_003d:  add
    IL_003e:  stloc.2
    IL_003f:  ldloc.1
    IL_0040:  ldc.i8     0x1
    IL_0049:  conv.i
    IL_004a:  add
    IL_004b:  stloc.1
    IL_004c:  ldloc.1
    IL_004d:  ldloc.0
    IL_004e:  blt.un.s   IL_002c

    IL_0050:  ret
  } 

  .method public static void  f12() cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             native int V_1,
             native int V_2)
    IL_0000:  ldc.i8     0x1
    IL_0009:  conv.i
    IL_000a:  ldc.i8     0x0
    IL_0013:  conv.i
    IL_0014:  ldc.i8     0xa
    IL_001d:  conv.i
    IL_001e:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_0023:  pop
    IL_0024:  ldc.i8     0x0
    IL_002d:  conv.i
    IL_002e:  stloc.0
    IL_002f:  ldc.i8     0x0
    IL_0038:  conv.i
    IL_0039:  stloc.1
    IL_003a:  ldc.i8     0x1
    IL_0043:  conv.i
    IL_0044:  stloc.2
    IL_0045:  br.s       IL_0067

    IL_0047:  ldloc.2
    IL_0048:  call       void assembly::set_c(native int)
    IL_004d:  ldloc.2
    IL_004e:  ldc.i8     0x0
    IL_0057:  conv.i
    IL_0058:  add
    IL_0059:  stloc.2
    IL_005a:  ldloc.1
    IL_005b:  ldc.i8     0x1
    IL_0064:  conv.i
    IL_0065:  add
    IL_0066:  stloc.1
    IL_0067:  ldloc.1
    IL_0068:  ldloc.0
    IL_0069:  blt.un.s   IL_0047

    IL_006b:  ret
  } 

  .method public static void  f13() cil managed
  {
    
    .maxstack  4
    .locals init (native int V_0,
             bool V_1,
             native int V_2,
             native int V_3,
             native int V_4)
    IL_0000:  ldc.i8     0xa
    IL_0009:  conv.i
    IL_000a:  ldc.i8     0x1
    IL_0013:  conv.i
    IL_0014:  bge.s      IL_0023

    IL_0016:  ldc.i8     0x0
    IL_001f:  conv.i
    IL_0020:  nop
    IL_0021:  br.s       IL_0039

    IL_0023:  ldc.i8     0xa
    IL_002c:  conv.i
    IL_002d:  ldc.i8     0x1
    IL_0036:  conv.i
    IL_0037:  sub
    IL_0038:  nop
    IL_0039:  stloc.0
    IL_003a:  sizeof     [runtime]System.IntPtr
    IL_0040:  ldc.i4.4
    IL_0041:  bne.un.s   IL_0053

    IL_0043:  ldloc.0
    IL_0044:  ldc.i8     0xffffffff
    IL_004d:  conv.u
    IL_004e:  ceq
    IL_0050:  nop
    IL_0051:  br.s       IL_0061

    IL_0053:  ldloc.0
    IL_0054:  ldc.i8     0xffffffffffffffff
    IL_005d:  conv.u
    IL_005e:  ceq
    IL_0060:  nop
    IL_0061:  brfalse.s  IL_00af

    IL_0063:  ldc.i4.1
    IL_0064:  stloc.1
    IL_0065:  ldc.i8     0x0
    IL_006e:  conv.i
    IL_006f:  stloc.2
    IL_0070:  ldc.i8     0xa
    IL_0079:  conv.i
    IL_007a:  stloc.3
    IL_007b:  br.s       IL_00ab

    IL_007d:  ldloc.3
    IL_007e:  call       void assembly::set_c(native int)
    IL_0083:  ldloc.3
    IL_0084:  ldc.i8     0xffffffffffffffff
    IL_008d:  conv.i
    IL_008e:  add
    IL_008f:  stloc.3
    IL_0090:  ldloc.2
    IL_0091:  ldc.i8     0x1
    IL_009a:  conv.i
    IL_009b:  add
    IL_009c:  stloc.2
    IL_009d:  ldloc.2
    IL_009e:  ldc.i8     0x0
    IL_00a7:  conv.i
    IL_00a8:  cgt.un
    IL_00aa:  stloc.1
    IL_00ab:  ldloc.1
    IL_00ac:  brtrue.s   IL_007d

    IL_00ae:  ret

    IL_00af:  ldc.i8     0xa
    IL_00b8:  conv.i
    IL_00b9:  ldc.i8     0x1
    IL_00c2:  conv.i
    IL_00c3:  bge.s      IL_00d2

    IL_00c5:  ldc.i8     0x0
    IL_00ce:  conv.i
    IL_00cf:  nop
    IL_00d0:  br.s       IL_00f3

    IL_00d2:  ldc.i8     0xa
    IL_00db:  conv.i
    IL_00dc:  ldc.i8     0x1
    IL_00e5:  conv.i
    IL_00e6:  sub
    IL_00e7:  ldc.i8     0x1
    IL_00f0:  conv.i
    IL_00f1:  add.ovf.un
    IL_00f2:  nop
    IL_00f3:  stloc.2
    IL_00f4:  ldc.i8     0x0
    IL_00fd:  conv.i
    IL_00fe:  stloc.3
    IL_00ff:  ldc.i8     0xa
    IL_0108:  conv.i
    IL_0109:  stloc.s    V_4
    IL_010b:  br.s       IL_0130

    IL_010d:  ldloc.s    V_4
    IL_010f:  call       void assembly::set_c(native int)
    IL_0114:  ldloc.s    V_4
    IL_0116:  ldc.i8     0xffffffffffffffff
    IL_011f:  conv.i
    IL_0120:  add
    IL_0121:  stloc.s    V_4
    IL_0123:  ldloc.3
    IL_0124:  ldc.i8     0x1
    IL_012d:  conv.i
    IL_012e:  add
    IL_012f:  stloc.3
    IL_0130:  ldloc.3
    IL_0131:  ldloc.2
    IL_0132:  blt.un.s   IL_010d

    IL_0134:  ret
  } 

  .method public static void  f14() cil managed
  {
    
    .maxstack  5
    .locals init (native int V_0,
             bool V_1,
             native int V_2,
             native int V_3,
             native int V_4)
    IL_0000:  ldc.i8     0xfffffffffffffffe
    IL_0009:  conv.i
    IL_000a:  ldc.i8     0x0
    IL_0013:  conv.i
    IL_0014:  bne.un.s   IL_003d

    IL_0016:  ldc.i8     0xa
    IL_001f:  conv.i
    IL_0020:  ldc.i8     0xfffffffffffffffe
    IL_0029:  conv.i
    IL_002a:  ldc.i8     0x1
    IL_0033:  conv.i
    IL_0034:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native int> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeIntPtr(native int,
                                                                                                                                                                                 native int,
                                                                                                                                                                                 native int)
    IL_0039:  pop
    IL_003a:  nop
    IL_003b:  br.s       IL_003e

    IL_003d:  nop
    IL_003e:  ldc.i8     0x0
    IL_0047:  conv.i
    IL_0048:  ldc.i8     0xfffffffffffffffe
    IL_0051:  conv.i
    IL_0052:  bge.s      IL_009a

    IL_0054:  ldc.i8     0x1
    IL_005d:  conv.i
    IL_005e:  ldc.i8     0xa
    IL_0067:  conv.i
    IL_0068:  bge.s      IL_0077

    IL_006a:  ldc.i8     0x0
    IL_0073:  conv.i
    IL_0074:  nop
    IL_0075:  br.s       IL_00ea

    IL_0077:  ldc.i8     0x1
    IL_0080:  conv.i
    IL_0081:  ldc.i8     0xa
    IL_008a:  conv.i
    IL_008b:  sub
    IL_008c:  ldc.i8     0xfffffffffffffffe
    IL_0095:  conv.i
    IL_0096:  div.un
    IL_0097:  nop
    IL_0098:  br.s       IL_00ea

    IL_009a:  ldc.i8     0xa
    IL_00a3:  conv.i
    IL_00a4:  ldc.i8     0x1
    IL_00ad:  conv.i
    IL_00ae:  bge.s      IL_00bd

    IL_00b0:  ldc.i8     0x0
    IL_00b9:  conv.i
    IL_00ba:  nop
    IL_00bb:  br.s       IL_00ea

    IL_00bd:  ldc.i8     0xa
    IL_00c6:  conv.i
    IL_00c7:  ldc.i8     0x1
    IL_00d0:  conv.i
    IL_00d1:  sub
    IL_00d2:  ldc.i8     0xfffffffffffffffe
    IL_00db:  conv.i
    IL_00dc:  not
    IL_00dd:  ldc.i8     0x1
    IL_00e6:  conv.i
    IL_00e7:  add
    IL_00e8:  div.un
    IL_00e9:  nop
    IL_00ea:  stloc.0
    IL_00eb:  sizeof     [runtime]System.IntPtr
    IL_00f1:  ldc.i4.4
    IL_00f2:  bne.un.s   IL_0104

    IL_00f4:  ldloc.0
    IL_00f5:  ldc.i8     0xffffffff
    IL_00fe:  conv.u
    IL_00ff:  ceq
    IL_0101:  nop
    IL_0102:  br.s       IL_0112

    IL_0104:  ldloc.0
    IL_0105:  ldc.i8     0xffffffffffffffff
    IL_010e:  conv.u
    IL_010f:  ceq
    IL_0111:  nop
    IL_0112:  brfalse.s  IL_0160

    IL_0114:  ldc.i4.1
    IL_0115:  stloc.1
    IL_0116:  ldc.i8     0x0
    IL_011f:  conv.i
    IL_0120:  stloc.2
    IL_0121:  ldc.i8     0xa
    IL_012a:  conv.i
    IL_012b:  stloc.3
    IL_012c:  br.s       IL_015c

    IL_012e:  ldloc.3
    IL_012f:  call       void assembly::set_c(native int)
    IL_0134:  ldloc.3
    IL_0135:  ldc.i8     0xfffffffffffffffe
    IL_013e:  conv.i
    IL_013f:  add
    IL_0140:  stloc.3
    IL_0141:  ldloc.2
    IL_0142:  ldc.i8     0x1
    IL_014b:  conv.i
    IL_014c:  add
    IL_014d:  stloc.2
    IL_014e:  ldloc.2
    IL_014f:  ldc.i8     0x0
    IL_0158:  conv.i
    IL_0159:  cgt.un
    IL_015b:  stloc.1
    IL_015c:  ldloc.1
    IL_015d:  brtrue.s   IL_012e

    IL_015f:  ret

    IL_0160:  ldc.i8     0x0
    IL_0169:  conv.i
    IL_016a:  ldc.i8     0xfffffffffffffffe
    IL_0173:  conv.i
    IL_0174:  bge.s      IL_01ca

    IL_0176:  ldc.i8     0x1
    IL_017f:  conv.i
    IL_0180:  ldc.i8     0xa
    IL_0189:  conv.i
    IL_018a:  bge.s      IL_019c

    IL_018c:  ldc.i8     0x0
    IL_0195:  conv.i
    IL_0196:  nop
    IL_0197:  br         IL_0225

    IL_019c:  ldc.i8     0x1
    IL_01a5:  conv.i
    IL_01a6:  ldc.i8     0xa
    IL_01af:  conv.i
    IL_01b0:  sub
    IL_01b1:  ldc.i8     0xfffffffffffffffe
    IL_01ba:  conv.i
    IL_01bb:  div.un
    IL_01bc:  ldc.i8     0x1
    IL_01c5:  conv.i
    IL_01c6:  add.ovf.un
    IL_01c7:  nop
    IL_01c8:  br.s       IL_0225

    IL_01ca:  ldc.i8     0xa
    IL_01d3:  conv.i
    IL_01d4:  ldc.i8     0x1
    IL_01dd:  conv.i
    IL_01de:  bge.s      IL_01ed

    IL_01e0:  ldc.i8     0x0
    IL_01e9:  conv.i
    IL_01ea:  nop
    IL_01eb:  br.s       IL_0225

    IL_01ed:  ldc.i8     0xa
    IL_01f6:  conv.i
    IL_01f7:  ldc.i8     0x1
    IL_0200:  conv.i
    IL_0201:  sub
    IL_0202:  ldc.i8     0xfffffffffffffffe
    IL_020b:  conv.i
    IL_020c:  not
    IL_020d:  ldc.i8     0x1
    IL_0216:  conv.i
    IL_0217:  add
    IL_0218:  div.un
    IL_0219:  ldc.i8     0x1
    IL_0222:  conv.i
    IL_0223:  add.ovf.un
    IL_0224:  nop
    IL_0225:  stloc.2
    IL_0226:  ldc.i8     0x0
    IL_022f:  conv.i
    IL_0230:  stloc.3
    IL_0231:  ldc.i8     0xa
    IL_023a:  conv.i
    IL_023b:  stloc.s    V_4
    IL_023d:  br.s       IL_0262

    IL_023f:  ldloc.s    V_4
    IL_0241:  call       void assembly::set_c(native int)
    IL_0246:  ldloc.s    V_4
    IL_0248:  ldc.i8     0xfffffffffffffffe
    IL_0251:  conv.i
    IL_0252:  add
    IL_0253:  stloc.s    V_4
    IL_0255:  ldloc.3
    IL_0256:  ldc.i8     0x1
    IL_025f:  conv.i
    IL_0260:  add
    IL_0261:  stloc.3
    IL_0262:  ldloc.3
    IL_0263:  ldloc.2
    IL_0264:  blt.un.s   IL_023f

    IL_0266:  ret
  } 

  .property native int c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(native int)
    .get native int assembly::get_c()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly native int c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.i
    IL_000a:  stsfld     native int '<StartupCode$assembly>'.$assembly::c@1
    IL_000f:  ret
  } 

} 






