
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
.assembly NoBoxingOnDispose01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.NoBoxingOnDispose01
{
  // Offset: 0x00000000 Length: 0x0000026D
  // WARNING: managed resource file FSharpSignatureData.NoBoxingOnDispose01 created
}
.mresource public FSharpOptimizationData.NoBoxingOnDispose01
{
  // Offset: 0x00000278 Length: 0x0000007F
  // WARNING: managed resource file FSharpOptimizationData.NoBoxingOnDispose01 created
}
.module NoBoxingOnDispose01.exe
// MVID: {62506722-4EA9-C934-A745-038322675062}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02FC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed NoBoxingOnDispose01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  f1<T>(class [mscorlib]System.Collections.Generic.List`1<!!T> x) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<!!T> V_0,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!!T> V_1,
             !!T V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<!!T>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_0014

      IL_000b:  ldloca.s   V_1
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!!T>::get_Current()
      IL_0012:  stloc.2
      IL_0013:  nop
      IL_0014:  ldloca.s   V_1
      IL_0016:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!!T>::MoveNext()
      IL_001b:  brtrue.s   IL_000b

      IL_001d:  leave.s    IL_002d

    }  // end .try
    finally
    {
      IL_001f:  ldloca.s   V_1
      IL_0021:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!!T>
      IL_0027:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002c:  endfinally
    }  // end handler
    IL_002d:  ret
  } // end of method NoBoxingOnDispose01::f1

} // end of class NoBoxingOnDispose01

.class private abstract auto ansi sealed '<StartupCode$NoBoxingOnDispose01>'.$NoBoxingOnDispose01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $NoBoxingOnDispose01::main@

} // end of class '<StartupCode$NoBoxingOnDispose01>'.$NoBoxingOnDispose01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\Misc\NoBoxingOnDispose01_fs\NoBoxingOnDispose01.res
