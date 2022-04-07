
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly Match02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Match02
{
  // Offset: 0x00000000 Length: 0x000004C0
  // WARNING: managed resource file FSharpSignatureData.Match02 created
}
.mresource public FSharpOptimizationData.Match02
{
  // Offset: 0x000004C8 Length: 0x000002EE
  // WARNING: managed resource file FSharpOptimizationData.Match02 created
}
.module Match02.exe
// MVID: {624E944C-47B1-958A-A745-03834C944E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x009D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Match02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class sequential ansi serializable sealed nested public S
         extends [mscorlib]System.ValueType
  {
    .pack 0
    .size 1
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname static int32 
            op_Addition<a>(!!a _arg1,
                           valuetype Match02/S _arg2) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } // end of method S::op_Addition

    .method public specialname static int32 
            op_Multiply<a>(!!a _arg3,
                           valuetype Match02/S _arg4) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ret
    } // end of method S::op_Multiply

    .method public specialname static int32 
            op_Addition<a>(valuetype Match02/S _arg5,
                           !!a _arg6) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.2
      IL_0001:  ret
    } // end of method S::op_Addition

    .method public specialname static int32 
            op_Multiply<a>(valuetype Match02/S _arg7,
                           !!a _arg8) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.3
      IL_0001:  ret
    } // end of method S::op_Multiply

  } // end of class S

  .method public static int32  testmethod() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  nop
    IL_0003:  nop
    IL_0004:  nop
    IL_0005:  nop
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  ldc.i4.s   12
    IL_000a:  ret
  } // end of method Match02::testmethod

} // end of class Match02

.class private abstract auto ansi sealed '<StartupCode$Match02>'.$Match02
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Match02::main@

} // end of class '<StartupCode$Match02>'.$Match02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\Inlining\Match02_fs\Match02.res
