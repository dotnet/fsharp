
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
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
.assembly Int64
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Int64
{
  // Offset: 0x00000000 Length: 0x0000020D
  // WARNING: managed resource file FSharpSignatureData.Int64 created
}
.mresource public FSharpOptimizationData.Int64
{
  // Offset: 0x00000218 Length: 0x000000D4
  // WARNING: managed resource file FSharpOptimizationData.Int64 created
}
.module Int64.exe
// MVID: {59C696CC-77F4-40B2-A745-0383CC96C659}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02640000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Int64
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static int64 
          get_a() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.2
    IL_0001:  conv.i8
    IL_0002:  ret
  } // end of method Int64::get_a

  .method public specialname static int64 
          get_b() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldc.i4.s   20
    IL_0002:  conv.i8
    IL_0003:  ret
  } // end of method Int64::get_b

  .method public specialname static int64 
          get_c() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4     0x80000001
    IL_0005:  conv.u8
    IL_0006:  ret
  } // end of method Int64::get_c

  .method public specialname static int64 
          get_d() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldc.i8     0xd90851d401
    IL_0009:  ret
  } // end of method Int64::get_d

  .property int64 a()
  {
    .get int64 Int64::get_a()
  } // end of property Int64::a
  .property int64 b()
  {
    .get int64 Int64::get_b()
  } // end of property Int64::b
  .property int64 c()
  {
    .get int64 Int64::get_c()
  } // end of property Int64::c
  .property int64 d()
  {
    .get int64 Int64::get_d()
  } // end of property Int64::d
} // end of class Int64

.class private abstract auto ansi sealed '<StartupCode$Int64>'.$Int64
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Int64::main@

} // end of class '<StartupCode$Int64>'.$Int64


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file Int64.res
