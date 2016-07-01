
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.81.0
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
.assembly Lock01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Lock01
{
  // Offset: 0x00000000 Length: 0x00000184
}
.mresource public FSharpOptimizationData.Lock01
{
  // Offset: 0x00000188 Length: 0x00000064
}
.module Lock01.exe
// MVID: {5775BA45-2BCA-B308-A745-038345BA7557}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01160000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Lock01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit clo@20
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } // end of method clo@20::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar0) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 20,20 : 19,21 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\Misc\\Lock01.fs'
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  ret
    } // end of method clo@20::Invoke

  } // end of class clo@20

  .method public specialname static object 
          get_o() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     object '<StartupCode$Lock01>'.$Lock01::o@19
    IL_0005:  ret
  } // end of method Lock01::get_o

  .property object o()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get object Lock01::get_o()
  } // end of property Lock01::o
} // end of class Lock01

.class private abstract auto ansi sealed '<StartupCode$Lock01>'.$Lock01
       extends [mscorlib]System.Object
{
  .field static assembly object o@19
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       69 (0x45)
    .maxstack  4
    .locals init ([0] object o,
             [1] object V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_2,
             [3] bool V_3,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_4)
    .line 19,19 : 1,28 ''
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_0006:  dup
    IL_0007:  stsfld     object '<StartupCode$Lock01>'.$Lock01::o@19
    IL_000c:  stloc.0
    .line 20,20 : 1,23 ''
    IL_000d:  call       object Lock01::get_o()
    IL_0012:  stloc.1
    IL_0013:  newobj     instance void Lock01/clo@20::.ctor()
    IL_0018:  stloc.2
    IL_0019:  ldc.i4.0
    IL_001a:  stloc.3
    .try
    {
      IL_001b:  ldloc.1
      IL_001c:  ldloca.s   V_3
      IL_001e:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_0023:  ldloc.2
      IL_0024:  ldnull
      IL_0025:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_002a:  stloc.s    V_4
      IL_002c:  leave.s    IL_0041

    }  // end .try
    finally
    {
      IL_002e:  ldloc.3
      IL_002f:  brfalse.s  IL_0033

      IL_0031:  br.s       IL_0035

      IL_0033:  br.s       IL_003e

      .line 100001,100001 : 0,0 ''
      IL_0035:  ldloc.1
      IL_0036:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_003b:  ldnull
      IL_003c:  pop
      IL_003d:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_003e:  ldnull
      IL_003f:  pop
      IL_0040:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0041:  ldloc.s    V_4
    IL_0043:  pop
    IL_0044:  ret
  } // end of method $Lock01::main@

} // end of class '<StartupCode$Lock01>'.$Lock01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
