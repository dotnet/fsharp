
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
.assembly NonTrivialBranchingBindingInEnd01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.NonTrivialBranchingBindingInEnd01
{
  // Offset: 0x00000000 Length: 0x00000276
  // WARNING: managed resource file FSharpSignatureData.NonTrivialBranchingBindingInEnd01 created
}
.mresource public FSharpOptimizationData.NonTrivialBranchingBindingInEnd01
{
  // Offset: 0x00000280 Length: 0x000000A6
  // WARNING: managed resource file FSharpOptimizationData.NonTrivialBranchingBindingInEnd01 created
}
.module NonTrivialBranchingBindingInEnd01.exe
// MVID: {624FA9CC-16D8-A7F7-A745-0383CCA94F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03DC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed NonTrivialBranchingBindingInEnd01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static int32[] 
          get_r() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$NonTrivialBranchingBindingInEnd01>'.$NonTrivialBranchingBindingInEnd01::r@6
    IL_0005:  ret
  } // end of method NonTrivialBranchingBindingInEnd01::get_r

  .method public specialname static int32[] 
          get_w() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$NonTrivialBranchingBindingInEnd01>'.$NonTrivialBranchingBindingInEnd01::w@7
    IL_0005:  ret
  } // end of method NonTrivialBranchingBindingInEnd01::get_w

  .property int32[] r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] NonTrivialBranchingBindingInEnd01::get_r()
  } // end of property NonTrivialBranchingBindingInEnd01::r
  .property int32[] w()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] NonTrivialBranchingBindingInEnd01::get_w()
  } // end of property NonTrivialBranchingBindingInEnd01::w
} // end of class NonTrivialBranchingBindingInEnd01

.class private abstract auto ansi sealed '<StartupCode$NonTrivialBranchingBindingInEnd01>'.$NonTrivialBranchingBindingInEnd01
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
    // Code size       132 (0x84)
    .maxstack  7
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldc.i4.8
    IL_0001:  ldc.i4.1
    IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0007:  stsfld     int32[] '<StartupCode$NonTrivialBranchingBindingInEnd01>'.$NonTrivialBranchingBindingInEnd01::r@6
    IL_000c:  ldc.i4.5
    IL_000d:  ldc.i4.2
    IL_000e:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0013:  stsfld     int32[] '<StartupCode$NonTrivialBranchingBindingInEnd01>'.$NonTrivialBranchingBindingInEnd01::w@7
    IL_0018:  ldc.i4.0
    IL_0019:  stloc.1
    IL_001a:  call       int32[] NonTrivialBranchingBindingInEnd01::get_r()
    IL_001f:  ldlen
    IL_0020:  conv.i4
    IL_0021:  stloc.2
    IL_0022:  call       int32[] NonTrivialBranchingBindingInEnd01::get_w()
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
    IL_0039:  blt.s      IL_0067

    IL_003b:  call       int32[] NonTrivialBranchingBindingInEnd01::get_r()
    IL_0040:  ldloc.1
    IL_0041:  call       int32[] NonTrivialBranchingBindingInEnd01::get_r()
    IL_0046:  ldloc.1
    IL_0047:  ldelem     [mscorlib]System.Int32
    IL_004c:  call       int32[] NonTrivialBranchingBindingInEnd01::get_w()
    IL_0051:  ldloc.1
    IL_0052:  ldelem     [mscorlib]System.Int32
    IL_0057:  add
    IL_0058:  stelem     [mscorlib]System.Int32
    IL_005d:  ldloc.1
    IL_005e:  ldc.i4.1
    IL_005f:  add
    IL_0060:  stloc.1
    IL_0061:  ldloc.1
    IL_0062:  ldloc.0
    IL_0063:  ldc.i4.1
    IL_0064:  add
    IL_0065:  bne.un.s   IL_003b

    IL_0067:  nop
    IL_0068:  nop
    IL_0069:  call       int32[] NonTrivialBranchingBindingInEnd01::get_r()
    IL_006e:  ldc.i4.0
    IL_006f:  ldelem     [mscorlib]System.Int32
    IL_0074:  ldc.i4.3
    IL_0075:  bne.un.s   IL_007b

    IL_0077:  ldc.i4.0
    IL_0078:  nop
    IL_0079:  br.s       IL_007d

    IL_007b:  ldc.i4.1
    IL_007c:  nop
    IL_007d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_0082:  pop
    IL_0083:  ret
  } // end of method $NonTrivialBranchingBindingInEnd01::main@

} // end of class '<StartupCode$NonTrivialBranchingBindingInEnd01>'.$NonTrivialBranchingBindingInEnd01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\release\net472\tests\EmittedIL\ForLoop\NonTrivialBranchingBindingInEnd01_fs_opt\NonTrivialBranchingBindingInEnd01.res
