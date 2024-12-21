




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
  .field static assembly native uint c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static native uint get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     native uint assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(native uint 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     native uint assembly::c@1
    IL_0006:  ret
  } 

  .method public static void  f0() cil managed
  {
    
    .maxstack  4
    .locals init (native uint V_0,
             native uint V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.u
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0xa
    IL_0014:  conv.u
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native uint)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.u
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.u
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0x0
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f00() cil managed
  {
    
    .maxstack  4
    .locals init (native uint V_0,
             native uint V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.u
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0xa
    IL_0014:  conv.u
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native uint)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.u
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.u
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0x0
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f1() cil managed
  {
    
    .maxstack  4
    .locals init (native uint V_0,
             native uint V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.u
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0x1
    IL_0014:  conv.u
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native uint)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.u
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.u
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0xa
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f2(native uint start) cil managed
  {
    
    .maxstack  4
    .locals init (native uint V_0,
             bool V_1,
             native uint V_2,
             native uint V_3,
             native uint V_4)
    IL_0000:  ldc.i8     0xa
    IL_0009:  conv.u
    IL_000a:  ldarg.0
    IL_000b:  bge.un.s   IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.u
    IL_0017:  nop
    IL_0018:  br.s       IL_0027

    IL_001a:  ldc.i8     0xa
    IL_0023:  conv.u
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
    IL_005c:  conv.u
    IL_005d:  stloc.2
    IL_005e:  ldarg.0
    IL_005f:  stloc.3
    IL_0060:  br.s       IL_0090

    IL_0062:  ldloc.3
    IL_0063:  call       void assembly::set_c(native uint)
    IL_0068:  ldloc.3
    IL_0069:  ldc.i8     0x1
    IL_0072:  conv.u
    IL_0073:  add
    IL_0074:  stloc.3
    IL_0075:  ldloc.2
    IL_0076:  ldc.i8     0x1
    IL_007f:  conv.u
    IL_0080:  add
    IL_0081:  stloc.2
    IL_0082:  ldloc.2
    IL_0083:  ldc.i8     0x0
    IL_008c:  conv.u
    IL_008d:  cgt.un
    IL_008f:  stloc.1
    IL_0090:  ldloc.1
    IL_0091:  brtrue.s   IL_0062

    IL_0093:  ret

    IL_0094:  ldc.i8     0xa
    IL_009d:  conv.u
    IL_009e:  ldarg.0
    IL_009f:  bge.un.s   IL_00ae

    IL_00a1:  ldc.i8     0x0
    IL_00aa:  conv.u
    IL_00ab:  nop
    IL_00ac:  br.s       IL_00c6

    IL_00ae:  ldc.i8     0xa
    IL_00b7:  conv.u
    IL_00b8:  ldarg.0
    IL_00b9:  sub
    IL_00ba:  ldc.i8     0x1
    IL_00c3:  conv.u
    IL_00c4:  add.ovf.un
    IL_00c5:  nop
    IL_00c6:  stloc.2
    IL_00c7:  ldc.i8     0x0
    IL_00d0:  conv.u
    IL_00d1:  stloc.3
    IL_00d2:  ldarg.0
    IL_00d3:  stloc.s    V_4
    IL_00d5:  br.s       IL_00fa

    IL_00d7:  ldloc.s    V_4
    IL_00d9:  call       void assembly::set_c(native uint)
    IL_00de:  ldloc.s    V_4
    IL_00e0:  ldc.i8     0x1
    IL_00e9:  conv.u
    IL_00ea:  add
    IL_00eb:  stloc.s    V_4
    IL_00ed:  ldloc.3
    IL_00ee:  ldc.i8     0x1
    IL_00f7:  conv.u
    IL_00f8:  add
    IL_00f9:  stloc.3
    IL_00fa:  ldloc.3
    IL_00fb:  ldloc.2
    IL_00fc:  blt.un.s   IL_00d7

    IL_00fe:  ret
  } 

  .method public static void  f3(native uint finish) cil managed
  {
    
    .maxstack  4
    .locals init (native uint V_0,
             bool V_1,
             native uint V_2,
             native uint V_3,
             native uint V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x1
    IL_000a:  conv.u
    IL_000b:  bge.un.s   IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.u
    IL_0017:  nop
    IL_0018:  br.s       IL_0027

    IL_001a:  ldarg.0
    IL_001b:  ldc.i8     0x1
    IL_0024:  conv.u
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
    IL_005c:  conv.u
    IL_005d:  stloc.2
    IL_005e:  ldc.i8     0x1
    IL_0067:  conv.u
    IL_0068:  stloc.3
    IL_0069:  br.s       IL_0099

    IL_006b:  ldloc.3
    IL_006c:  call       void assembly::set_c(native uint)
    IL_0071:  ldloc.3
    IL_0072:  ldc.i8     0x1
    IL_007b:  conv.u
    IL_007c:  add
    IL_007d:  stloc.3
    IL_007e:  ldloc.2
    IL_007f:  ldc.i8     0x1
    IL_0088:  conv.u
    IL_0089:  add
    IL_008a:  stloc.2
    IL_008b:  ldloc.2
    IL_008c:  ldc.i8     0x0
    IL_0095:  conv.u
    IL_0096:  cgt.un
    IL_0098:  stloc.1
    IL_0099:  ldloc.1
    IL_009a:  brtrue.s   IL_006b

    IL_009c:  ret

    IL_009d:  ldarg.0
    IL_009e:  ldc.i8     0x1
    IL_00a7:  conv.u
    IL_00a8:  bge.un.s   IL_00b7

    IL_00aa:  ldc.i8     0x0
    IL_00b3:  conv.u
    IL_00b4:  nop
    IL_00b5:  br.s       IL_00cf

    IL_00b7:  ldarg.0
    IL_00b8:  ldc.i8     0x1
    IL_00c1:  conv.u
    IL_00c2:  sub
    IL_00c3:  ldc.i8     0x1
    IL_00cc:  conv.u
    IL_00cd:  add.ovf.un
    IL_00ce:  nop
    IL_00cf:  stloc.2
    IL_00d0:  ldc.i8     0x0
    IL_00d9:  conv.u
    IL_00da:  stloc.3
    IL_00db:  ldc.i8     0x1
    IL_00e4:  conv.u
    IL_00e5:  stloc.s    V_4
    IL_00e7:  br.s       IL_010c

    IL_00e9:  ldloc.s    V_4
    IL_00eb:  call       void assembly::set_c(native uint)
    IL_00f0:  ldloc.s    V_4
    IL_00f2:  ldc.i8     0x1
    IL_00fb:  conv.u
    IL_00fc:  add
    IL_00fd:  stloc.s    V_4
    IL_00ff:  ldloc.3
    IL_0100:  ldc.i8     0x1
    IL_0109:  conv.u
    IL_010a:  add
    IL_010b:  stloc.3
    IL_010c:  ldloc.3
    IL_010d:  ldloc.2
    IL_010e:  blt.un.s   IL_00e9

    IL_0110:  ret
  } 

  .method public static void  f4(native uint start,
                                 native uint finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (native uint V_0,
             bool V_1,
             native uint V_2,
             native uint V_3,
             native uint V_4)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.un.s   IL_0011

    IL_0004:  ldc.i8     0x0
    IL_000d:  conv.u
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
    IL_004a:  conv.u
    IL_004b:  stloc.2
    IL_004c:  ldarg.0
    IL_004d:  stloc.3
    IL_004e:  br.s       IL_007e

    IL_0050:  ldloc.3
    IL_0051:  call       void assembly::set_c(native uint)
    IL_0056:  ldloc.3
    IL_0057:  ldc.i8     0x1
    IL_0060:  conv.u
    IL_0061:  add
    IL_0062:  stloc.3
    IL_0063:  ldloc.2
    IL_0064:  ldc.i8     0x1
    IL_006d:  conv.u
    IL_006e:  add
    IL_006f:  stloc.2
    IL_0070:  ldloc.2
    IL_0071:  ldc.i8     0x0
    IL_007a:  conv.u
    IL_007b:  cgt.un
    IL_007d:  stloc.1
    IL_007e:  ldloc.1
    IL_007f:  brtrue.s   IL_0050

    IL_0081:  ret

    IL_0082:  ldarg.1
    IL_0083:  ldarg.0
    IL_0084:  bge.un.s   IL_0093

    IL_0086:  ldc.i8     0x0
    IL_008f:  conv.u
    IL_0090:  nop
    IL_0091:  br.s       IL_00a2

    IL_0093:  ldarg.1
    IL_0094:  ldarg.0
    IL_0095:  sub
    IL_0096:  ldc.i8     0x1
    IL_009f:  conv.u
    IL_00a0:  add.ovf.un
    IL_00a1:  nop
    IL_00a2:  stloc.2
    IL_00a3:  ldc.i8     0x0
    IL_00ac:  conv.u
    IL_00ad:  stloc.3
    IL_00ae:  ldarg.0
    IL_00af:  stloc.s    V_4
    IL_00b1:  br.s       IL_00d6

    IL_00b3:  ldloc.s    V_4
    IL_00b5:  call       void assembly::set_c(native uint)
    IL_00ba:  ldloc.s    V_4
    IL_00bc:  ldc.i8     0x1
    IL_00c5:  conv.u
    IL_00c6:  add
    IL_00c7:  stloc.s    V_4
    IL_00c9:  ldloc.3
    IL_00ca:  ldc.i8     0x1
    IL_00d3:  conv.u
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
    .locals init (native uint V_0,
             native uint V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.u
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0x1
    IL_0014:  conv.u
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native uint)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x1
    IL_0028:  conv.u
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.u
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
    .locals init (native uint V_0,
             native uint V_1)
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.u
    IL_000a:  stloc.0
    IL_000b:  ldc.i8     0x1
    IL_0014:  conv.u
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0038

    IL_0018:  ldloc.1
    IL_0019:  call       void assembly::set_c(native uint)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i8     0x2
    IL_0028:  conv.u
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldc.i8     0x1
    IL_0035:  conv.u
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i8     0x5
    IL_0042:  conv.u
    IL_0043:  blt.un.s   IL_0018

    IL_0045:  ret
  } 

  .method public static void  f7(native uint start) cil managed
  {
    
    .maxstack  4
    .locals init (native uint V_0,
             native uint V_1,
             native uint V_2)
    IL_0000:  ldc.i8     0xa
    IL_0009:  conv.u
    IL_000a:  ldarg.0
    IL_000b:  bge.un.s   IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.u
    IL_0017:  nop
    IL_0018:  br.s       IL_003d

    IL_001a:  ldc.i8     0xa
    IL_0023:  conv.u
    IL_0024:  ldarg.0
    IL_0025:  sub
    IL_0026:  ldc.i8     0x2
    IL_002f:  conv.u
    IL_0030:  div.un
    IL_0031:  ldc.i8     0x1
    IL_003a:  conv.u
    IL_003b:  add.ovf.un
    IL_003c:  nop
    IL_003d:  stloc.0
    IL_003e:  ldc.i8     0x0
    IL_0047:  conv.u
    IL_0048:  stloc.1
    IL_0049:  ldarg.0
    IL_004a:  stloc.2
    IL_004b:  br.s       IL_006d

    IL_004d:  ldloc.2
    IL_004e:  call       void assembly::set_c(native uint)
    IL_0053:  ldloc.2
    IL_0054:  ldc.i8     0x2
    IL_005d:  conv.u
    IL_005e:  add
    IL_005f:  stloc.2
    IL_0060:  ldloc.1
    IL_0061:  ldc.i8     0x1
    IL_006a:  conv.u
    IL_006b:  add
    IL_006c:  stloc.1
    IL_006d:  ldloc.1
    IL_006e:  ldloc.0
    IL_006f:  blt.un.s   IL_004d

    IL_0071:  ret
  } 

  .method public static void  f8(native uint step) cil managed
  {
    
    .maxstack  5
    .locals init (native uint V_0,
             bool V_1,
             native uint V_2,
             native uint V_3,
             native uint V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x0
    IL_000a:  conv.u
    IL_000b:  bne.un.s   IL_002b

    IL_000d:  ldc.i8     0x1
    IL_0016:  conv.u
    IL_0017:  ldarg.0
    IL_0018:  ldc.i8     0xa
    IL_0021:  conv.u
    IL_0022:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native uint> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUIntPtr(native uint,
                                                                                                                                                                                   native uint,
                                                                                                                                                                                   native uint)
    IL_0027:  pop
    IL_0028:  nop
    IL_0029:  br.s       IL_002c

    IL_002b:  nop
    IL_002c:  ldc.i8     0xa
    IL_0035:  conv.u
    IL_0036:  ldc.i8     0x1
    IL_003f:  conv.u
    IL_0040:  bge.un.s   IL_004f

    IL_0042:  ldc.i8     0x0
    IL_004b:  conv.u
    IL_004c:  nop
    IL_004d:  br.s       IL_0067

    IL_004f:  ldc.i8     0xa
    IL_0058:  conv.u
    IL_0059:  ldc.i8     0x1
    IL_0062:  conv.u
    IL_0063:  sub
    IL_0064:  ldarg.0
    IL_0065:  div.un
    IL_0066:  nop
    IL_0067:  stloc.0
    IL_0068:  sizeof     [runtime]System.IntPtr
    IL_006e:  ldc.i4.4
    IL_006f:  bne.un.s   IL_0081

    IL_0071:  ldloc.0
    IL_0072:  ldc.i8     0xffffffff
    IL_007b:  conv.u
    IL_007c:  ceq
    IL_007e:  nop
    IL_007f:  br.s       IL_008f

    IL_0081:  ldloc.0
    IL_0082:  ldc.i8     0xffffffffffffffff
    IL_008b:  conv.u
    IL_008c:  ceq
    IL_008e:  nop
    IL_008f:  brfalse.s  IL_00d4

    IL_0091:  ldc.i4.1
    IL_0092:  stloc.1
    IL_0093:  ldc.i8     0x0
    IL_009c:  conv.u
    IL_009d:  stloc.2
    IL_009e:  ldc.i8     0x1
    IL_00a7:  conv.u
    IL_00a8:  stloc.3
    IL_00a9:  br.s       IL_00d0

    IL_00ab:  ldloc.3
    IL_00ac:  call       void assembly::set_c(native uint)
    IL_00b1:  ldloc.3
    IL_00b2:  ldarg.0
    IL_00b3:  add
    IL_00b4:  stloc.3
    IL_00b5:  ldloc.2
    IL_00b6:  ldc.i8     0x1
    IL_00bf:  conv.u
    IL_00c0:  add
    IL_00c1:  stloc.2
    IL_00c2:  ldloc.2
    IL_00c3:  ldc.i8     0x0
    IL_00cc:  conv.u
    IL_00cd:  cgt.un
    IL_00cf:  stloc.1
    IL_00d0:  ldloc.1
    IL_00d1:  brtrue.s   IL_00ab

    IL_00d3:  ret

    IL_00d4:  ldc.i8     0xa
    IL_00dd:  conv.u
    IL_00de:  ldc.i8     0x1
    IL_00e7:  conv.u
    IL_00e8:  bge.un.s   IL_00f7

    IL_00ea:  ldc.i8     0x0
    IL_00f3:  conv.u
    IL_00f4:  nop
    IL_00f5:  br.s       IL_011a

    IL_00f7:  ldc.i8     0xa
    IL_0100:  conv.u
    IL_0101:  ldc.i8     0x1
    IL_010a:  conv.u
    IL_010b:  sub
    IL_010c:  ldarg.0
    IL_010d:  div.un
    IL_010e:  ldc.i8     0x1
    IL_0117:  conv.u
    IL_0118:  add.ovf.un
    IL_0119:  nop
    IL_011a:  stloc.2
    IL_011b:  ldc.i8     0x0
    IL_0124:  conv.u
    IL_0125:  stloc.3
    IL_0126:  ldc.i8     0x1
    IL_012f:  conv.u
    IL_0130:  stloc.s    V_4
    IL_0132:  br.s       IL_014e

    IL_0134:  ldloc.s    V_4
    IL_0136:  call       void assembly::set_c(native uint)
    IL_013b:  ldloc.s    V_4
    IL_013d:  ldarg.0
    IL_013e:  add
    IL_013f:  stloc.s    V_4
    IL_0141:  ldloc.3
    IL_0142:  ldc.i8     0x1
    IL_014b:  conv.u
    IL_014c:  add
    IL_014d:  stloc.3
    IL_014e:  ldloc.3
    IL_014f:  ldloc.2
    IL_0150:  blt.un.s   IL_0134

    IL_0152:  ret
  } 

  .method public static void  f9(native uint finish) cil managed
  {
    
    .maxstack  4
    .locals init (native uint V_0,
             native uint V_1,
             native uint V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x1
    IL_000a:  conv.u
    IL_000b:  bge.un.s   IL_001a

    IL_000d:  ldc.i8     0x0
    IL_0016:  conv.u
    IL_0017:  nop
    IL_0018:  br.s       IL_003d

    IL_001a:  ldarg.0
    IL_001b:  ldc.i8     0x1
    IL_0024:  conv.u
    IL_0025:  sub
    IL_0026:  ldc.i8     0x2
    IL_002f:  conv.u
    IL_0030:  div.un
    IL_0031:  ldc.i8     0x1
    IL_003a:  conv.u
    IL_003b:  add.ovf.un
    IL_003c:  nop
    IL_003d:  stloc.0
    IL_003e:  ldc.i8     0x0
    IL_0047:  conv.u
    IL_0048:  stloc.1
    IL_0049:  ldc.i8     0x1
    IL_0052:  conv.u
    IL_0053:  stloc.2
    IL_0054:  br.s       IL_0076

    IL_0056:  ldloc.2
    IL_0057:  call       void assembly::set_c(native uint)
    IL_005c:  ldloc.2
    IL_005d:  ldc.i8     0x2
    IL_0066:  conv.u
    IL_0067:  add
    IL_0068:  stloc.2
    IL_0069:  ldloc.1
    IL_006a:  ldc.i8     0x1
    IL_0073:  conv.u
    IL_0074:  add
    IL_0075:  stloc.1
    IL_0076:  ldloc.1
    IL_0077:  ldloc.0
    IL_0078:  blt.un.s   IL_0056

    IL_007a:  ret
  } 

  .method public static void  f10(native uint start,
                                  native uint step,
                                  native uint finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (native uint V_0,
             bool V_1,
             native uint V_2,
             native uint V_3,
             native uint V_4)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i8     0x0
    IL_000a:  conv.u
    IL_000b:  bne.un.s   IL_0019

    IL_000d:  ldarg.2
    IL_000e:  ldarg.1
    IL_000f:  ldarg.2
    IL_0010:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native uint> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUIntPtr(native uint,
                                                                                                                                                                                   native uint,
                                                                                                                                                                                   native uint)
    IL_0015:  pop
    IL_0016:  nop
    IL_0017:  br.s       IL_001a

    IL_0019:  nop
    IL_001a:  ldarg.2
    IL_001b:  ldarg.2
    IL_001c:  bge.un.s   IL_002b

    IL_001e:  ldc.i8     0x0
    IL_0027:  conv.u
    IL_0028:  nop
    IL_0029:  br.s       IL_0031

    IL_002b:  ldarg.2
    IL_002c:  ldarg.2
    IL_002d:  sub
    IL_002e:  ldarg.1
    IL_002f:  div.un
    IL_0030:  nop
    IL_0031:  stloc.0
    IL_0032:  sizeof     [runtime]System.IntPtr
    IL_0038:  ldc.i4.4
    IL_0039:  bne.un.s   IL_004b

    IL_003b:  ldloc.0
    IL_003c:  ldc.i8     0xffffffff
    IL_0045:  conv.u
    IL_0046:  ceq
    IL_0048:  nop
    IL_0049:  br.s       IL_0059

    IL_004b:  ldloc.0
    IL_004c:  ldc.i8     0xffffffffffffffff
    IL_0055:  conv.u
    IL_0056:  ceq
    IL_0058:  nop
    IL_0059:  brfalse.s  IL_0095

    IL_005b:  ldc.i4.1
    IL_005c:  stloc.1
    IL_005d:  ldc.i8     0x0
    IL_0066:  conv.u
    IL_0067:  stloc.2
    IL_0068:  ldarg.2
    IL_0069:  stloc.3
    IL_006a:  br.s       IL_0091

    IL_006c:  ldloc.3
    IL_006d:  call       void assembly::set_c(native uint)
    IL_0072:  ldloc.3
    IL_0073:  ldarg.1
    IL_0074:  add
    IL_0075:  stloc.3
    IL_0076:  ldloc.2
    IL_0077:  ldc.i8     0x1
    IL_0080:  conv.u
    IL_0081:  add
    IL_0082:  stloc.2
    IL_0083:  ldloc.2
    IL_0084:  ldc.i8     0x0
    IL_008d:  conv.u
    IL_008e:  cgt.un
    IL_0090:  stloc.1
    IL_0091:  ldloc.1
    IL_0092:  brtrue.s   IL_006c

    IL_0094:  ret

    IL_0095:  ldarg.2
    IL_0096:  ldarg.2
    IL_0097:  bge.un.s   IL_00a6

    IL_0099:  ldc.i8     0x0
    IL_00a2:  conv.u
    IL_00a3:  nop
    IL_00a4:  br.s       IL_00b7

    IL_00a6:  ldarg.2
    IL_00a7:  ldarg.2
    IL_00a8:  sub
    IL_00a9:  ldarg.1
    IL_00aa:  div.un
    IL_00ab:  ldc.i8     0x1
    IL_00b4:  conv.u
    IL_00b5:  add.ovf.un
    IL_00b6:  nop
    IL_00b7:  stloc.2
    IL_00b8:  ldc.i8     0x0
    IL_00c1:  conv.u
    IL_00c2:  stloc.3
    IL_00c3:  ldarg.2
    IL_00c4:  stloc.s    V_4
    IL_00c6:  br.s       IL_00e2

    IL_00c8:  ldloc.s    V_4
    IL_00ca:  call       void assembly::set_c(native uint)
    IL_00cf:  ldloc.s    V_4
    IL_00d1:  ldarg.1
    IL_00d2:  add
    IL_00d3:  stloc.s    V_4
    IL_00d5:  ldloc.3
    IL_00d6:  ldc.i8     0x1
    IL_00df:  conv.u
    IL_00e0:  add
    IL_00e1:  stloc.3
    IL_00e2:  ldloc.3
    IL_00e3:  ldloc.2
    IL_00e4:  blt.un.s   IL_00c8

    IL_00e6:  ret
  } 

  .method public static void  f11(native uint start,
                                  native uint finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (native uint V_0,
             native uint V_1,
             native uint V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i8     0x0
    IL_000a:  conv.u
    IL_000b:  ldarg.1
    IL_000c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native uint> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUIntPtr(native uint,
                                                                                                                                                                                   native uint,
                                                                                                                                                                                   native uint)
    IL_0011:  pop
    IL_0012:  ldc.i8     0x0
    IL_001b:  conv.u
    IL_001c:  stloc.0
    IL_001d:  ldc.i8     0x0
    IL_0026:  conv.u
    IL_0027:  stloc.1
    IL_0028:  ldarg.0
    IL_0029:  stloc.2
    IL_002a:  br.s       IL_004c

    IL_002c:  ldloc.2
    IL_002d:  call       void assembly::set_c(native uint)
    IL_0032:  ldloc.2
    IL_0033:  ldc.i8     0x0
    IL_003c:  conv.u
    IL_003d:  add
    IL_003e:  stloc.2
    IL_003f:  ldloc.1
    IL_0040:  ldc.i8     0x1
    IL_0049:  conv.u
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
    .locals init (native uint V_0,
             native uint V_1,
             native uint V_2)
    IL_0000:  ldc.i8     0x1
    IL_0009:  conv.u
    IL_000a:  ldc.i8     0x0
    IL_0013:  conv.u
    IL_0014:  ldc.i8     0xa
    IL_001d:  conv.u
    IL_001e:  call       class [runtime]System.Collections.Generic.IEnumerable`1<native uint> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUIntPtr(native uint,
                                                                                                                                                                                   native uint,
                                                                                                                                                                                   native uint)
    IL_0023:  pop
    IL_0024:  ldc.i8     0x0
    IL_002d:  conv.u
    IL_002e:  stloc.0
    IL_002f:  ldc.i8     0x0
    IL_0038:  conv.u
    IL_0039:  stloc.1
    IL_003a:  ldc.i8     0x1
    IL_0043:  conv.u
    IL_0044:  stloc.2
    IL_0045:  br.s       IL_0067

    IL_0047:  ldloc.2
    IL_0048:  call       void assembly::set_c(native uint)
    IL_004d:  ldloc.2
    IL_004e:  ldc.i8     0x0
    IL_0057:  conv.u
    IL_0058:  add
    IL_0059:  stloc.2
    IL_005a:  ldloc.1
    IL_005b:  ldc.i8     0x1
    IL_0064:  conv.u
    IL_0065:  add
    IL_0066:  stloc.1
    IL_0067:  ldloc.1
    IL_0068:  ldloc.0
    IL_0069:  blt.un.s   IL_0047

    IL_006b:  ret
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
    IL_0000:  ldc.i8     0x0
    IL_0009:  conv.u
    IL_000a:  stsfld     native uint assembly::c@1
    IL_000f:  ret
  } 

  .property native uint c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(native uint)
    .get native uint assembly::get_c()
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






