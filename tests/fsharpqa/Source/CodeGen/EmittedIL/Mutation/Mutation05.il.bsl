
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.33440
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
.assembly Mutation05
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Mutation05
{
  // Offset: 0x00000000 Length: 0x000004C6
}
.mresource public FSharpOptimizationData.Mutation05
{
  // Offset: 0x000004D0 Length: 0x00000127
}
.module Mutation05.exe
// MVID: {54C7CE57-8C6A-2E22-A745-038357CEC754}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x001E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Mutation05
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public C
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32 x
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.VolatileFieldAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 4,4 : 5,22 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\Mutation\\Mutation05.fs'
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  nop
      IL_0009:  ldarg.0
      IL_000a:  ldc.i4.1
      IL_000b:  volatile.
      IL_000d:  stfld      int32 Mutation05/C::x
      .line 2,2 : 6,7 ''
      IL_0012:  ret
    } // end of method C::.ctor

    .method public hidebysig specialname 
            instance int32  get_X() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 6,6 : 32,33 ''
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  volatile.
      IL_0004:  ldfld      int32 Mutation05/C::x
      IL_0009:  ret
    } // end of method C::get_X

    .method public hidebysig specialname 
            instance void  set_X(int32 v) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      .line 6,6 : 46,52 ''
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  volatile.
      IL_0005:  stfld      int32 Mutation05/C::x
      IL_000a:  ret
    } // end of method C::set_X

    .property instance int32 X()
    {
      .set instance void Mutation05/C::set_X(int32)
      .get instance int32 Mutation05/C::get_X()
    } // end of property C::X
  } // end of class C

  .class auto ansi serializable nested public StaticC
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly int32 x
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.VolatileFieldAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly int32 init@9
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 9,9 : 6,13 ''
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  nop
      IL_0009:  ret
    } // end of method StaticC::.ctor

    .method public specialname static int32 
            get_X() cil managed
    {
      // Code size       33 (0x21)
      .maxstack  8
      .line 13,13 : 34,35 ''
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 Mutation05/StaticC::init@9
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_000c

      IL_000a:  br.s       IL_000e

      IL_000c:  br.s       IL_0017

      .line 100001,100001 : 0,0 ''
      IL_000e:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_0013:  nop
      .line 100001,100001 : 0,0 ''
      IL_0014:  nop
      IL_0015:  br.s       IL_0018

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0017:  nop
      IL_0018:  nop
      IL_0019:  volatile.
      IL_001b:  ldsfld     int32 Mutation05/StaticC::x
      IL_0020:  ret
    } // end of method StaticC::get_X

    .method public specialname static void 
            set_X(int32 v) cil managed
    {
      // Code size       34 (0x22)
      .maxstack  8
      .line 13,13 : 48,54 ''
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 Mutation05/StaticC::init@9
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_000c

      IL_000a:  br.s       IL_000e

      IL_000c:  br.s       IL_0017

      .line 100001,100001 : 0,0 ''
      IL_000e:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_0013:  nop
      .line 100001,100001 : 0,0 ''
      IL_0014:  nop
      IL_0015:  br.s       IL_0018

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0017:  nop
      IL_0018:  nop
      IL_0019:  ldarg.0
      IL_001a:  volatile.
      IL_001c:  stsfld     int32 Mutation05/StaticC::x
      IL_0021:  ret
    } // end of method StaticC::set_X

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       13 (0xd)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$Mutation05>'.$Mutation05::init@
      IL_0006:  ldsfld     int32 '<StartupCode$Mutation05>'.$Mutation05::init@
      IL_000b:  pop
      IL_000c:  ret
    } // end of method StaticC::.cctor

    .property int32 X()
    {
      .set void Mutation05/StaticC::set_X(int32)
      .get int32 Mutation05/StaticC::get_X()
    } // end of property StaticC::X
  } // end of class StaticC

} // end of class Mutation05

.class private abstract auto ansi sealed '<StartupCode$Mutation05>'.$Mutation05
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       18 (0x12)
    .maxstack  8
    .line 11,11 : 12,29 ''
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  volatile.
    IL_0004:  stsfld     int32 Mutation05/StaticC::x
    IL_0009:  ldc.i4.1
    IL_000a:  volatile.
    IL_000c:  stsfld     int32 Mutation05/StaticC::init@9
    .line 9,9 : 6,13 ''
    IL_0011:  ret
  } // end of method $Mutation05::main@

} // end of class '<StartupCode$Mutation05>'.$Mutation05


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
