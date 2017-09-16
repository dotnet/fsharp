
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
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
  .ver 4:4:1:0
}
.assembly CompiledNameAttribute03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.CompiledNameAttribute03
{
  // Offset: 0x00000000 Length: 0x0000027D
}
.mresource public FSharpOptimizationData.CompiledNameAttribute03
{
  // Offset: 0x00000288 Length: 0x00000086
}
.module CompiledNameAttribute03.exe
// MVID: {59B1923F-2CE4-60B9-A745-03833F92B159}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03120000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed CompiledNameAttribute03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static !!a  SomeCompiledName<a>(int32 x) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationSourceNameAttribute::.ctor(string) = ( 01 00 01 66 00 00 )                               // ...f..
    // Code size       7 (0x7)
    .maxstack  8
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 4,4 : 15,25 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\CompiledNameAttribute\\CompiledNameAttribute03.fs'
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  sub
    IL_0003:  starg.s    x
    IL_0005:  br.s       IL_0000
  } // end of method CompiledNameAttribute03::SomeCompiledName

} // end of class CompiledNameAttribute03

.class private abstract auto ansi sealed '<StartupCode$CompiledNameAttribute03>'.$CompiledNameAttribute03
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $CompiledNameAttribute03::main@

} // end of class '<StartupCode$CompiledNameAttribute03>'.$CompiledNameAttribute03


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
