
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
.assembly NonTrivialBranchingBindingInEnd05
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.NonTrivialBranchingBindingInEnd05
{
  // Offset: 0x00000000 Length: 0x00000276
  // WARNING: managed resource file FSharpSignatureData.NonTrivialBranchingBindingInEnd05 created
}
.mresource public FSharpOptimizationData.NonTrivialBranchingBindingInEnd05
{
  // Offset: 0x00000280 Length: 0x000000A6
  // WARNING: managed resource file FSharpOptimizationData.NonTrivialBranchingBindingInEnd05 created
}
.module NonTrivialBranchingBindingInEnd05.exe
// MVID: {624FA9CC-164C-A7F7-A745-0383CCA94F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E00000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed NonTrivialBranchingBindingInEnd05
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static int32[] 
          get_r() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$NonTrivialBranchingBindingInEnd05>'.$NonTrivialBranchingBindingInEnd05::r@6
    IL_0005:  ret
  } // end of method NonTrivialBranchingBindingInEnd05::get_r

  .method public specialname static int32[] 
          get_w() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$NonTrivialBranchingBindingInEnd05>'.$NonTrivialBranchingBindingInEnd05::w@7
    IL_0005:  ret
  } // end of method NonTrivialBranchingBindingInEnd05::get_w

  .property int32[] r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] NonTrivialBranchingBindingInEnd05::get_r()
  } // end of property NonTrivialBranchingBindingInEnd05::r
  .property int32[] w()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] NonTrivialBranchingBindingInEnd05::get_w()
  } // end of property NonTrivialBranchingBindingInEnd05::w
} // end of class NonTrivialBranchingBindingInEnd05

.class private abstract auto ansi sealed '<StartupCode$NonTrivialBranchingBindingInEnd05>'.$NonTrivialBranchingBindingInEnd05
       extends [mscorlib]System.Object
{
  .field static assembly int32[] r@6
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] w@7
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       184 (0xb8)
    .maxstack  7
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5)
    IL_0000:  ldc.i4.8
    IL_0001:  ldc.i4.1
    IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0007:  stsfld     int32[] '<StartupCode$NonTrivialBranchingBindingInEnd05>'.$NonTrivialBranchingBindingInEnd05::r@6
    IL_000c:  ldc.i4.5
    IL_000d:  ldc.i4.2
    IL_000e:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0013:  stsfld     int32[] '<StartupCode$NonTrivialBranchingBindingInEnd05>'.$NonTrivialBranchingBindingInEnd05::w@7
    IL_0018:  ldc.i4.0
    IL_0019:  stloc.1
    IL_001a:  call       int32[] NonTrivialBranchingBindingInEnd05::get_r()
    IL_001f:  ldlen
    IL_0020:  conv.i4
    IL_0021:  stloc.2
    IL_0022:  call       int32[] NonTrivialBranchingBindingInEnd05::get_w()
    IL_0027:  ldlen
    IL_0028:  conv.i4
    IL_0029:  stloc.3
    IL_002a:  ldloc.2
    IL_002b:  ldloc.3
    IL_002c:  bge.s      IL_0032

    IL_002e:  ldloc.2
    IL_002f:  nop
    IL_0030:  br.s       IL_0034

    IL_0032:  ldloc.3
    IL_0033:  nop
    IL_0034:  ldc.i4.1
    IL_0035:  sub
    IL_0036:  stloc.0
    IL_0037:  ldloc.0
    IL_0038:  ldloc.1
    IL_0039:  blt.s      IL_009a

    IL_003b:  ldc.i4.0
    IL_003c:  stloc.3
    IL_003d:  call       int32[] NonTrivialBranchingBindingInEnd05::get_r()
    IL_0042:  ldlen
    IL_0043:  conv.i4
    IL_0044:  stloc.s    V_4
    IL_0046:  call       int32[] NonTrivialBranchingBindingInEnd05::get_w()
    IL_004b:  ldlen
    IL_004c:  conv.i4
    IL_004d:  stloc.s    V_5
    IL_004f:  ldloc.s    V_4
    IL_0051:  ldloc.s    V_5
    IL_0053:  bge.s      IL_005a

    IL_0055:  ldloc.s    V_4
    IL_0057:  nop
    IL_0058:  br.s       IL_005d

    IL_005a:  ldloc.s    V_5
    IL_005c:  nop
    IL_005d:  ldc.i4.1
    IL_005e:  sub
    IL_005f:  stloc.2
    IL_0060:  ldloc.2
    IL_0061:  ldloc.3
    IL_0062:  blt.s      IL_0090

    IL_0064:  call       int32[] NonTrivialBranchingBindingInEnd05::get_r()
    IL_0069:  ldloc.3
    IL_006a:  call       int32[] NonTrivialBranchingBindingInEnd05::get_r()
    IL_006f:  ldloc.3
    IL_0070:  ldelem     [mscorlib]System.Int32
    IL_0075:  call       int32[] NonTrivialBranchingBindingInEnd05::get_w()
    IL_007a:  ldloc.3
    IL_007b:  ldelem     [mscorlib]System.Int32
    IL_0080:  add
    IL_0081:  stelem     [mscorlib]System.Int32
    IL_0086:  ldloc.3
    IL_0087:  ldc.i4.1
    IL_0088:  add
    IL_0089:  stloc.3
    IL_008a:  ldloc.3
    IL_008b:  ldloc.2
    IL_008c:  ldc.i4.1
    IL_008d:  add
    IL_008e:  bne.un.s   IL_0064

    IL_0090:  ldloc.1
    IL_0091:  ldc.i4.1
    IL_0092:  add
    IL_0093:  stloc.1
    IL_0094:  ldloc.1
    IL_0095:  ldloc.0
    IL_0096:  ldc.i4.1
    IL_0097:  add
    IL_0098:  bne.un.s   IL_003b

    IL_009a:  nop
    IL_009b:  nop
    IL_009c:  call       int32[] NonTrivialBranchingBindingInEnd05::get_r()
    IL_00a1:  ldc.i4.0
    IL_00a2:  ldelem     [mscorlib]System.Int32
    IL_00a7:  ldc.i4.s   11
    IL_00a9:  bne.un.s   IL_00af

    IL_00ab:  ldc.i4.0
    IL_00ac:  nop
    IL_00ad:  br.s       IL_00b1

    IL_00af:  ldc.i4.1
    IL_00b0:  nop
    IL_00b1:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_00b6:  pop
    IL_00b7:  ret
  } // end of method $NonTrivialBranchingBindingInEnd05::main@

} // end of class '<StartupCode$NonTrivialBranchingBindingInEnd05>'.$NonTrivialBranchingBindingInEnd05


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\release\net472\tests\EmittedIL\ForLoop\NonTrivialBranchingBindingInEnd05_fs_opt\NonTrivialBranchingBindingInEnd05.res
