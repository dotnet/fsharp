
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
.assembly Seq_for_all01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Seq_for_all01
{
  // Offset: 0x00000000 Length: 0x0000021E
  // WARNING: managed resource file FSharpSignatureData.Seq_for_all01 created
}
.mresource public FSharpOptimizationData.Seq_for_all01
{
  // Offset: 0x00000228 Length: 0x00000072
  // WARNING: managed resource file FSharpOptimizationData.Seq_for_all01 created
}
.module Seq_for_all01.exe
// MVID: {624E1220-D30D-BA80-A745-038320124E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03940000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Seq_for_all01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit q@4
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Seq_for_all01/q@4 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } // end of method q@4::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 s) cil managed
    {
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  ldc.i4.0
      IL_0003:  ceq
      IL_0005:  nop
      IL_0006:  brfalse.s  IL_000c

      IL_0008:  nop
      IL_0009:  nop
      IL_000a:  br.s       IL_000d

      IL_000c:  nop
      IL_000d:  ldc.i4.1
      IL_000e:  ret
    } // end of method q@4::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Seq_for_all01/q@4::.ctor()
      IL_0005:  stsfld     class Seq_for_all01/q@4 Seq_for_all01/q@4::@_instance
      IL_000a:  ret
    } // end of method q@4::.cctor

  } // end of class q@4

  .method public specialname static bool 
          get_q() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     bool '<StartupCode$Seq_for_all01>'.$Seq_for_all01::q@4
    IL_0005:  ret
  } // end of method Seq_for_all01::get_q

  .property bool q()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get bool Seq_for_all01::get_q()
  } // end of property Seq_for_all01::q
} // end of class Seq_for_all01

.class private abstract auto ansi sealed '<StartupCode$Seq_for_all01>'.$Seq_for_all01
       extends [mscorlib]System.Object
{
  .field static assembly bool q@4
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       29 (0x1d)
    .maxstack  5
    .locals init (bool V_0)
    IL_0000:  ldsfld     class Seq_for_all01/q@4 Seq_for_all01/q@4::@_instance
    IL_0005:  ldc.i4.1
    IL_0006:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0010:  call       bool [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ForAll<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>,
                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0015:  dup
    IL_0016:  stsfld     bool '<StartupCode$Seq_for_all01>'.$Seq_for_all01::q@4
    IL_001b:  stloc.0
    IL_001c:  ret
  } // end of method $Seq_for_all01::main@

} // end of class '<StartupCode$Seq_for_all01>'.$Seq_for_all01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\kevinransom\fsharp\tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\Seq_for_all01_fs\Seq_for_all01.res
