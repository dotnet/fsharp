




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:1:0:0
}
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.assembly
{
  
  
}
.mresource public FSharpOptimizationData.assembly
{
  
  
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed Decimal_comparison_RealInternalSignatureOn
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Decimal_comparison_RealInternalSignatureOn::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Decimal_comparison_RealInternalSignatureOn::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  ldc.i4.0
    IL_0005:  ldc.i4.1
    IL_0006:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_000b:  ldc.i4.s   20
    IL_000d:  ldc.i4.0
    IL_000e:  ldc.i4.0
    IL_000f:  ldc.i4.0
    IL_0010:  ldc.i4.1
    IL_0011:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_0016:  call       bool [netstandard]System.Decimal::op_LessThan(valuetype [netstandard]System.Decimal,
                                                                       valuetype [netstandard]System.Decimal)
    IL_001b:  pop
    IL_001c:  ldc.i4.s   10
    IL_001e:  ldc.i4.0
    IL_001f:  ldc.i4.0
    IL_0020:  ldc.i4.0
    IL_0021:  ldc.i4.1
    IL_0022:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_0027:  ldc.i4.s   20
    IL_0029:  ldc.i4.0
    IL_002a:  ldc.i4.0
    IL_002b:  ldc.i4.0
    IL_002c:  ldc.i4.1
    IL_002d:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_0032:  call       bool [netstandard]System.Decimal::op_LessThanOrEqual(valuetype [netstandard]System.Decimal,
                                                                              valuetype [netstandard]System.Decimal)
    IL_0037:  pop
    IL_0038:  ldc.i4.s   10
    IL_003a:  ldc.i4.0
    IL_003b:  ldc.i4.0
    IL_003c:  ldc.i4.0
    IL_003d:  ldc.i4.1
    IL_003e:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_0043:  ldc.i4.s   20
    IL_0045:  ldc.i4.0
    IL_0046:  ldc.i4.0
    IL_0047:  ldc.i4.0
    IL_0048:  ldc.i4.1
    IL_0049:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_004e:  call       bool [netstandard]System.Decimal::op_GreaterThan(valuetype [netstandard]System.Decimal,
                                                                          valuetype [netstandard]System.Decimal)
    IL_0053:  pop
    IL_0054:  ldc.i4.s   10
    IL_0056:  ldc.i4.0
    IL_0057:  ldc.i4.0
    IL_0058:  ldc.i4.0
    IL_0059:  ldc.i4.1
    IL_005a:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_005f:  ldc.i4.s   20
    IL_0061:  ldc.i4.0
    IL_0062:  ldc.i4.0
    IL_0063:  ldc.i4.0
    IL_0064:  ldc.i4.1
    IL_0065:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_006a:  call       bool [netstandard]System.Decimal::op_GreaterThanOrEqual(valuetype [netstandard]System.Decimal,
                                                                                 valuetype [netstandard]System.Decimal)
    IL_006f:  pop
    IL_0070:  ldc.i4.s   10
    IL_0072:  ldc.i4.0
    IL_0073:  ldc.i4.0
    IL_0074:  ldc.i4.0
    IL_0075:  ldc.i4.1
    IL_0076:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_007b:  ldc.i4.s   20
    IL_007d:  ldc.i4.0
    IL_007e:  ldc.i4.0
    IL_007f:  ldc.i4.0
    IL_0080:  ldc.i4.1
    IL_0081:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_0086:  call       bool [netstandard]System.Decimal::op_Equality(valuetype [netstandard]System.Decimal,
                                                                       valuetype [netstandard]System.Decimal)
    IL_008b:  pop
    IL_008c:  ldc.i4.s   10
    IL_008e:  ldc.i4.0
    IL_008f:  ldc.i4.0
    IL_0090:  ldc.i4.0
    IL_0091:  ldc.i4.1
    IL_0092:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_0097:  ldc.i4.s   20
    IL_0099:  ldc.i4.0
    IL_009a:  ldc.i4.0
    IL_009b:  ldc.i4.0
    IL_009c:  ldc.i4.1
    IL_009d:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_00a2:  call       bool [netstandard]System.Decimal::op_Equality(valuetype [netstandard]System.Decimal,
                                                                       valuetype [netstandard]System.Decimal)
    IL_00a7:  ldc.i4.0
    IL_00a8:  ceq
    IL_00aa:  pop
    IL_00ab:  ldc.i4.s   10
    IL_00ad:  ldc.i4.0
    IL_00ae:  ldc.i4.0
    IL_00af:  ldc.i4.0
    IL_00b0:  ldc.i4.1
    IL_00b1:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_00b6:  ldc.i4.s   20
    IL_00b8:  ldc.i4.0
    IL_00b9:  ldc.i4.0
    IL_00ba:  ldc.i4.0
    IL_00bb:  ldc.i4.1
    IL_00bc:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_00c1:  call       bool [netstandard]System.Decimal::op_Equality(valuetype [netstandard]System.Decimal,
                                                                       valuetype [netstandard]System.Decimal)
    IL_00c6:  pop
    IL_00c7:  ldc.i4.s   10
    IL_00c9:  ldc.i4.0
    IL_00ca:  ldc.i4.0
    IL_00cb:  ldc.i4.0
    IL_00cc:  ldc.i4.1
    IL_00cd:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_00d2:  ldc.i4.s   20
    IL_00d4:  ldc.i4.0
    IL_00d5:  ldc.i4.0
    IL_00d6:  ldc.i4.0
    IL_00d7:  ldc.i4.1
    IL_00d8:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                          int32,
                                                                          int32,
                                                                          bool,
                                                                          uint8)
    IL_00dd:  call       int32 [netstandard]System.Decimal::Compare(valuetype [netstandard]System.Decimal,
                                                                    valuetype [netstandard]System.Decimal)
    IL_00e2:  pop
    IL_00e3:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Decimal_comparison_RealInternalSignatureOn
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
    IL_0000:  call       void Decimal_comparison_RealInternalSignatureOn::staticInitialization@()
    IL_0005:  ret
  } 

} 






