
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
.assembly Marshal
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Marshal
{
  // Offset: 0x00000000 Length: 0x00000578
  // WARNING: managed resource file FSharpSignatureData.Marshal created
}
.mresource public FSharpOptimizationData.Marshal
{
  // Offset: 0x00000580 Length: 0x0000004E
  // WARNING: managed resource file FSharpOptimizationData.Marshal created
}
.module Marshal.exe
// MVID: {624E1220-7500-369C-A745-038320124E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x037E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Marshal
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested public Reader
         extends [mscorlib]System.MulticastDelegate
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(object 'object',
                                 native int 'method') runtime managed
    {
    } // end of method Reader::.ctor

    .method public hidebysig strict virtual 
            instance int32  Invoke([out] uint8[]  marshal([ + 1]) data,
                                   [out] int32 length) runtime managed
    {
    } // end of method Reader::Invoke

    .method public hidebysig strict virtual 
            instance class [mscorlib]System.IAsyncResult 
            BeginInvoke([out] uint8[]  marshal([ + 1]) data,
                        [out] int32 length,
                        class [mscorlib]System.AsyncCallback callback,
                        object objects) runtime managed
    {
    } // end of method Reader::BeginInvoke

    .method public hidebysig strict virtual 
            instance int32  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
    {
    } // end of method Reader::EndInvoke

  } // end of class Reader

} // end of class Marshal

.class private abstract auto ansi sealed '<StartupCode$Marshal>'.$Marshal
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Marshal::main@

} // end of class '<StartupCode$Marshal>'.$Marshal


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\kevinransom\fsharp\tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\Marshal_fs\Marshal.res
