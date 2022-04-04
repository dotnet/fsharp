
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
.assembly ListExpressionStepping04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionStepping04
{
  // Offset: 0x00000000 Length: 0x000002B6
  // WARNING: managed resource file FSharpSignatureData.ListExpressionStepping04 created
}
.mresource public FSharpOptimizationData.ListExpressionStepping04
{
  // Offset: 0x000002C0 Length: 0x000000AC
  // WARNING: managed resource file FSharpOptimizationData.ListExpressionStepping04 created
}
.module ListExpressionStepping04.exe
// MVID: {62465220-15D3-6E8E-A745-038320524662}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02DF0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest4
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest4
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f3() cil managed
    {
      // Code size       88 (0x58)
      .maxstack  5
      .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_1,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_2,
               int32 V_3)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
      IL_0007:  stloc.1
      IL_0008:  ldloc.1
      IL_0009:  ldloc.1
      IL_000a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_000f:  ldc.i4.1
      IL_0010:  add
      IL_0011:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
      IL_0016:  ldc.i4.0
      IL_0017:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
      IL_001c:  stloc.2
      IL_001d:  ldloc.2
      IL_001e:  ldloc.2
      IL_001f:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0024:  ldc.i4.1
      IL_0025:  add
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
      IL_002b:  ldloca.s   V_0
      IL_002d:  ldloc.1
      IL_002e:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0033:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0038:  nop
      IL_0039:  ldloc.1
      IL_003a:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_003f:  ldloc.2
      IL_0040:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0045:  add
      IL_0046:  stloc.3
      IL_0047:  ldloca.s   V_0
      IL_0049:  ldloc.3
      IL_004a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_004f:  nop
      IL_0050:  ldloca.s   V_0
      IL_0052:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
      IL_0057:  ret
    } // end of method ListExpressionSteppingTest4::f3

  } // end of class ListExpressionSteppingTest4

} // end of class ListExpressionSteppingTest4

.class private abstract auto ansi sealed '<StartupCode$ListExpressionStepping04>'.$ListExpressionSteppingTest4
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest4/ListExpressionSteppingTest4::f3()
    IL_0005:  pop
    IL_0006:  ret
  } // end of method $ListExpressionSteppingTest4::main@

} // end of class '<StartupCode$ListExpressionStepping04>'.$ListExpressionSteppingTest4


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\ListExpressionStepping\ListExpressionStepping04_fs\ListExpressionStepping04.res
