
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
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
  .ver 4:7:0:0
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
  // Offset: 0x00000000 Length: 0x0000018C
}
.mresource public FSharpOptimizationData.comparison_decimal01
{
  // Offset: 0x00000190 Length: 0x0000005B
}
.module comparison_decimal01.exe
// MVID: {5EE40368-76D8-7EE3-A745-03836803E45E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06B30000


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
    // Code size       233 (0xe9)
    .maxstack  8
    .locals init ([0] valuetype [mscorlib]System.Decimal V_0,
             [1] valuetype [mscorlib]System.Decimal V_1)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 4,4 : 9,20 'C:\\Users\\phcart\\source\\repos\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\Operators\\comparison_decimal01.fs'
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  ldc.i4.0
    IL_0005:  ldc.i4.1
    IL_0006:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_000b:  ldc.i4.s   20
    IL_000d:  ldc.i4.0
    IL_000e:  ldc.i4.0
    IL_000f:  ldc.i4.0
    IL_0010:  ldc.i4.1
    IL_0011:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0016:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_001b:  pop
    .line 5,5 : 9,21 ''
    IL_001c:  ldc.i4.s   10
    IL_001e:  ldc.i4.0
    IL_001f:  ldc.i4.0
    IL_0020:  ldc.i4.0
    IL_0021:  ldc.i4.1
    IL_0022:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0027:  ldc.i4.s   20
    IL_0029:  ldc.i4.0
    IL_002a:  ldc.i4.0
    IL_002b:  ldc.i4.0
    IL_002c:  ldc.i4.1
    IL_002d:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0032:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_0037:  pop
    .line 6,6 : 9,20 ''
    IL_0038:  ldc.i4.s   10
    IL_003a:  ldc.i4.0
    IL_003b:  ldc.i4.0
    IL_003c:  ldc.i4.0
    IL_003d:  ldc.i4.1
    IL_003e:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0043:  ldc.i4.s   20
    IL_0045:  ldc.i4.0
    IL_0046:  ldc.i4.0
    IL_0047:  ldc.i4.0
    IL_0048:  ldc.i4.1
    IL_0049:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_004e:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0053:  pop
    .line 7,7 : 9,21 ''
    IL_0054:  ldc.i4.s   10
    IL_0056:  ldc.i4.0
    IL_0057:  ldc.i4.0
    IL_0058:  ldc.i4.0
    IL_0059:  ldc.i4.1
    IL_005a:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_005f:  ldc.i4.s   20
    IL_0061:  ldc.i4.0
    IL_0062:  ldc.i4.0
    IL_0063:  ldc.i4.0
    IL_0064:  ldc.i4.1
    IL_0065:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_006a:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                              valuetype [mscorlib]System.Decimal)
    IL_006f:  pop
    .line 8,8 : 9,20 ''
    IL_0070:  ldc.i4.s   10
    IL_0072:  ldc.i4.0
    IL_0073:  ldc.i4.0
    IL_0074:  ldc.i4.0
    IL_0075:  ldc.i4.1
    IL_0076:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_007b:  ldc.i4.s   20
    IL_007d:  ldc.i4.0
    IL_007e:  ldc.i4.0
    IL_007f:  ldc.i4.0
    IL_0080:  ldc.i4.1
    IL_0081:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0086:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_008b:  pop
    .line 9,9 : 9,21 ''
    IL_008c:  ldc.i4.s   10
    IL_008e:  ldc.i4.0
    IL_008f:  ldc.i4.0
    IL_0090:  ldc.i4.0
    IL_0091:  ldc.i4.1
    IL_0092:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0097:  stloc.0
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
    IL_00a3:  stloc.1
    IL_00a4:  ldloc.0
    IL_00a5:  ldloc.1
    IL_00a6:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00ab:  ldc.i4.0
    IL_00ac:  ceq
    IL_00ae:  pop
    .line 100001,100001 : 0,0 ''
    IL_00af:  nop
    .line 10,10 : 9,20 ''
    IL_00b0:  ldc.i4.s   10
    IL_00b2:  ldc.i4.0
    IL_00b3:  ldc.i4.0
    IL_00b4:  ldc.i4.0
    IL_00b5:  ldc.i4.1
    IL_00b6:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_00bb:  ldc.i4.s   20
    IL_00bd:  ldc.i4.0
    IL_00be:  ldc.i4.0
    IL_00bf:  ldc.i4.0
    IL_00c0:  ldc.i4.1
    IL_00c1:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_00c6:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00cb:  pop
    .line 11,11 : 9,26 ''
    IL_00cc:  ldc.i4.s   10
    IL_00ce:  ldc.i4.0
    IL_00cf:  ldc.i4.0
    IL_00d0:  ldc.i4.0
    IL_00d1:  ldc.i4.1
    IL_00d2:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_00d7:  ldc.i4.s   20
    IL_00d9:  ldc.i4.0
    IL_00da:  ldc.i4.0
    IL_00db:  ldc.i4.0
    IL_00dc:  ldc.i4.1
    IL_00dd:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_00e2:  call       int32 [mscorlib]System.Decimal::Compare(valuetype [mscorlib]System.Decimal,
                                                                 valuetype [mscorlib]System.Decimal)
    IL_00e7:  pop
    IL_00e8:  ret
  } // end of method $Comparison_decimal01::main@

} // end of class '<StartupCode$comparison_decimal01>'.$Comparison_decimal01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
