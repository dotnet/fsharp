
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
.assembly NonEscapingArguments02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.NonEscapingArguments02
{
  // Offset: 0x00000000 Length: 0x000003D5
  // WARNING: managed resource file FSharpSignatureData.NonEscapingArguments02 created
}
.mresource public FSharpOptimizationData.NonEscapingArguments02
{
  // Offset: 0x000003E0 Length: 0x00000212
  // WARNING: managed resource file FSharpOptimizationData.NonEscapingArguments02 created
}
.module NonEscapingArguments02.exe
// MVID: {624E1220-BB6A-DA26-A745-038320124E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03840000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed NonEscapingArguments02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public ListSizeCounter`1<t>
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32 size
    .method public specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!t> somelist) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Length<!t>(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
      IL_000f:  stfld      int32 class NonEscapingArguments02/ListSizeCounter`1<!t>::size
      IL_0014:  ret
    } // end of method ListSizeCounter`1::.ctor

    .method public hidebysig specialname 
            instance int32  get_Size() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class NonEscapingArguments02/ListSizeCounter`1<!t>::size
      IL_0006:  ret
    } // end of method ListSizeCounter`1::get_Size

    .property instance int32 Size()
    {
      .get instance int32 NonEscapingArguments02/ListSizeCounter`1::get_Size()
    } // end of property ListSizeCounter`1::Size
  } // end of class ListSizeCounter`1

} // end of class NonEscapingArguments02

.class private abstract auto ansi sealed '<StartupCode$NonEscapingArguments02>'.$NonEscapingArguments02
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $NonEscapingArguments02::main@

} // end of class '<StartupCode$NonEscapingArguments02>'.$NonEscapingArguments02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\kevinransom\fsharp\tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\NonEscapingArguments02_fs\NonEscapingArguments02.res
