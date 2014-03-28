
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
.assembly extern cas
{
  .ver 0:0:0:0
}
.assembly cas
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
  .custom instance void [cas]CustomSecAttr.CustomPermission2Attribute::.ctor(valuetype [mscorlib]System.Security.Permissions.SecurityAction) = ( 01 00 03 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .permissionset assert
             = {[cas]CustomSecAttr.CustomPermission2Attribute = {}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.cas
{
  // Offset: 0x00000000 Length: 0x00000603
}
.mresource public FSharpOptimizationData.cas
{
  // Offset: 0x00000608 Length: 0x000000F3
}
.module cas.exe
// MVID: {4F20DEFF-35EA-18E3-A745-0383FFDE204F}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000000A6EAC30000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Cas
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public AttrTest
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable nested public Foo
           extends [mscorlib]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .permissionset linkcheck
                 = {class 'System.Security.Permissions.SecurityPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' = {property enum class 'System.Security.Permissions.SecurityPermissionFlag, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' 'Flags' = int32(2)}}
      .permissionset demand
                 = {class 'System.Security.Permissions.PrincipalPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' = {property string 'Role' = string('test')}}
      .method public specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       10 (0xa)
        .maxstack  8
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 11,11 : 10,13 
        IL_0000:  ldarg.0
        IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  nop
        IL_0009:  ret
      } // end of method Foo::.ctor

      .method public hidebysig instance int32 
              someMethod() cil managed
      {
        .permissionset demand
                   = {class 'System.Security.Permissions.PrincipalPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' = {property string 'Role' = string('test')}}
        .permissionset assert
                   = {[cas]CustomSecAttr.CustomPermission2Attribute = {property enum class 'CustomSecAttr.SecurityArgType, cas, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' 'SecurityArg' = int32(2)}}
        // Code size       7 (0x7)
        .maxstack  8
        .line 14,14 : 33,37 
        IL_0000:  nop
        IL_0001:  ldc.i4     0x18c0
        IL_0006:  ret
      } // end of method Foo::someMethod

    } // end of class Foo

  } // end of class AttrTest

} // end of class Cas

.class private abstract auto ansi sealed '<StartupCode$cas>'.$Cas
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    .line 18,18 : 7,9 
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $Cas::main@

} // end of class '<StartupCode$cas>'.$Cas


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
