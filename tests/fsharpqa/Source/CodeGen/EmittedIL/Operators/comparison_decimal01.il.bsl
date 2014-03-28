
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17310
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:3:0:0
}
.assembly comparison_decimal01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.comparison_decimal01
{
  // Offset: 0x00000000 Length: 0x0000019E
}
.mresource public FSharpOptimizationData.comparison_decimal01
{
  // Offset: 0x000001A8 Length: 0x0000005B
}
.module comparison_decimal01.exe
// MVID: {4E86C244-76D8-7EE3-A745-038344C2864E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000000007D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Comparison_decimal01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class Comparison_decimal01

.class private abstract auto ansi sealed '<StartupCode$comparison_decimal01>'.$Comparison_decimal01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       229 (0xe5)
    .maxstack  8
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 4,4 : 9,20 
    IL_0000:  nop
    IL_0001:  ldc.i4.s   10
    IL_0003:  ldc.i4.0
    IL_0004:  ldc.i4.0
    IL_0005:  ldc.i4.0
    IL_0006:  ldc.i4.1
    IL_0007:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_000c:  ldc.i4.s   20
    IL_000e:  ldc.i4.0
    IL_000f:  ldc.i4.0
    IL_0010:  ldc.i4.0
    IL_0011:  ldc.i4.1
    IL_0012:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0017:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_001c:  pop
    .line 5,5 : 9,21 
    IL_001d:  ldc.i4.s   10
    IL_001f:  ldc.i4.0
    IL_0020:  ldc.i4.0
    IL_0021:  ldc.i4.0
    IL_0022:  ldc.i4.1
    IL_0023:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0028:  ldc.i4.s   20
    IL_002a:  ldc.i4.0
    IL_002b:  ldc.i4.0
    IL_002c:  ldc.i4.0
    IL_002d:  ldc.i4.1
    IL_002e:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0033:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_0038:  pop
    .line 6,6 : 9,20 
    IL_0039:  ldc.i4.s   10
    IL_003b:  ldc.i4.0
    IL_003c:  ldc.i4.0
    IL_003d:  ldc.i4.0
    IL_003e:  ldc.i4.1
    IL_003f:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0044:  ldc.i4.s   20
    IL_0046:  ldc.i4.0
    IL_0047:  ldc.i4.0
    IL_0048:  ldc.i4.0
    IL_0049:  ldc.i4.1
    IL_004a:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_004f:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0054:  pop
    .line 7,7 : 9,21 
    IL_0055:  ldc.i4.s   10
    IL_0057:  ldc.i4.0
    IL_0058:  ldc.i4.0
    IL_0059:  ldc.i4.0
    IL_005a:  ldc.i4.1
    IL_005b:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0060:  ldc.i4.s   20
    IL_0062:  ldc.i4.0
    IL_0063:  ldc.i4.0
    IL_0064:  ldc.i4.0
    IL_0065:  ldc.i4.1
    IL_0066:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_006b:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                              valuetype [mscorlib]System.Decimal)
    IL_0070:  pop
    .line 8,8 : 9,20 
    IL_0071:  ldc.i4.s   10
    IL_0073:  ldc.i4.0
    IL_0074:  ldc.i4.0
    IL_0075:  ldc.i4.0
    IL_0076:  ldc.i4.1
    IL_0077:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_007c:  ldc.i4.s   20
    IL_007e:  ldc.i4.0
    IL_007f:  ldc.i4.0
    IL_0080:  ldc.i4.0
    IL_0081:  ldc.i4.1
    IL_0082:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0087:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_008c:  pop
    .line 9,9 : 9,21 
    IL_008d:  ldc.i4.s   10
    IL_008f:  ldc.i4.0
    IL_0090:  ldc.i4.0
    IL_0091:  ldc.i4.0
    IL_0092:  ldc.i4.1
    IL_0093:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0098:  ldc.i4.s   20
    IL_009a:  ldc.i4.0
    IL_009b:  ldc.i4.0
    IL_009c:  ldc.i4.0
    IL_009d:  ldc.i4.1
    IL_009e:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_00a3:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00a8:  ldc.i4.0
    IL_00a9:  ceq
    IL_00ab:  pop
    .line 10,10 : 9,20 
    IL_00ac:  ldc.i4.s   10
    IL_00ae:  ldc.i4.0
    IL_00af:  ldc.i4.0
    IL_00b0:  ldc.i4.0
    IL_00b1:  ldc.i4.1
    IL_00b2:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_00b7:  ldc.i4.s   20
    IL_00b9:  ldc.i4.0
    IL_00ba:  ldc.i4.0
    IL_00bb:  ldc.i4.0
    IL_00bc:  ldc.i4.1
    IL_00bd:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_00c2:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00c7:  pop
    .line 11,11 : 9,26 
    IL_00c8:  ldc.i4.s   10
    IL_00ca:  ldc.i4.0
    IL_00cb:  ldc.i4.0
    IL_00cc:  ldc.i4.0
    IL_00cd:  ldc.i4.1
    IL_00ce:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_00d3:  ldc.i4.s   20
    IL_00d5:  ldc.i4.0
    IL_00d6:  ldc.i4.0
    IL_00d7:  ldc.i4.0
    IL_00d8:  ldc.i4.1
    IL_00d9:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_00de:  call       int32 [mscorlib]System.Decimal::Compare(valuetype [mscorlib]System.Decimal,
                                                                 valuetype [mscorlib]System.Decimal)
    IL_00e3:  pop
    IL_00e4:  ret
  } // end of method $Comparison_decimal01::main@

} // end of class '<StartupCode$comparison_decimal01>'.$Comparison_decimal01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
