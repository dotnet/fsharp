
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
  .ver 5:0:0:0
}
.assembly CompiledNameAttribute02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.CompiledNameAttribute02
{
  // Offset: 0x00000000 Length: 0x000002E4
}
.mresource public FSharpOptimizationData.CompiledNameAttribute02
{
  // Offset: 0x000002E8 Length: 0x000000CD
}
.module CompiledNameAttribute02.exe
// MVID: {6124062C-F755-F3C0-A745-03832C062461}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06A90000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed CompiledNameAttribute02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public T
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public hidebysig instance int32 
            SomeCompiledName(int32 x,
                             int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationSourceNameAttribute::.ctor(string) = ( 01 00 06 4D 65 74 68 6F 64 00 00 )                // ...Method..
      // Code size       6 (0x6)
      .maxstack  4
      .locals init ([0] class CompiledNameAttribute02/T a)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\CompiledNameAttribute\\CompiledNameAttribute02.fs'
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      .line 5,5 : 34,39 ''
      IL_0002:  ldarg.1
      IL_0003:  ldarg.2
      IL_0004:  add
      IL_0005:  ret
    } // end of method T::SomeCompiledName

  } // end of class T

} // end of class CompiledNameAttribute02

.class private abstract auto ansi sealed '<StartupCode$CompiledNameAttribute02>'.$CompiledNameAttribute02
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $CompiledNameAttribute02::main@

} // end of class '<StartupCode$CompiledNameAttribute02>'.$CompiledNameAttribute02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
