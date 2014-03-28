
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17376
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
  .ver 4:3:0:0
}
.assembly DoNotBoxStruct_MDArray_FSInterface_NoExtMeth
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.DoNotBoxStruct_MDArray_FSInterface_NoExtMeth
{
  // Offset: 0x00000000 Length: 0x000002A3
}
.mresource public FSharpOptimizationData.DoNotBoxStruct_MDArray_FSInterface_NoExtMeth
{
  // Offset: 0x000002A8 Length: 0x000000B0
}
.module DoNotBoxStruct_MDArray_FSInterface_NoExtMeth.exe
// MVID: {4F20DD3B-A67D-867A-A745-03833BDD204F}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000000518AC20000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed DoNotBoxStruct_MDArray_FSInterface_NoExtMeth
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname F@6
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method F@6::.ctor

    .method assembly hidebysig instance void 
            Invoke(object sender,
                   int32 args) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  5
      .locals init ([0] int32 V_0)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 6,6 : 77,79 
      IL_0000:  ldarg.2
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  ret
    } // end of method F@6::Invoke

  } // end of class F@6

  .method public static void  F<(class [FSharp.Core]Microsoft.FSharp.Control.IEvent`2<class [FSharp.Core]Microsoft.FSharp.Control.FSharpHandler`1<int32>,int32>) T>(!!T[0...,0...] x) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  8
    .line 6,6 : 47,80 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  readonly.
    IL_0006:  call       instance !!T& !!T[0...,0...]::Address(int32,
                                                               int32)
    IL_000b:  newobj     instance void DoNotBoxStruct_MDArray_FSInterface_NoExtMeth/F@6::.ctor()
    IL_0010:  ldftn      instance void DoNotBoxStruct_MDArray_FSInterface_NoExtMeth/F@6::Invoke(object,
                                                                                                int32)
    IL_0016:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Control.FSharpHandler`1<int32>::.ctor(object,
                                                                                                                 native int)
    IL_001b:  constrained. !!T
    IL_0021:  callvirt   instance void class [FSharp.Core]Microsoft.FSharp.Control.IDelegateEvent`1<class [FSharp.Core]Microsoft.FSharp.Control.FSharpHandler`1<int32>>::AddHandler(!0)
    IL_0026:  nop
    IL_0027:  ret
  } // end of method DoNotBoxStruct_MDArray_FSInterface_NoExtMeth::F

} // end of class DoNotBoxStruct_MDArray_FSInterface_NoExtMeth

.class private abstract auto ansi sealed '<StartupCode$DoNotBoxStruct_MDArray_FSInterface_NoExtMeth>'.$DoNotBoxStruct_MDArray_FSInterface_NoExtMeth
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $DoNotBoxStruct_MDArray_FSInterface_NoExtMeth::main@

} // end of class '<StartupCode$DoNotBoxStruct_MDArray_FSInterface_NoExtMeth>'.$DoNotBoxStruct_MDArray_FSInterface_NoExtMeth


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
