
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
.assembly extern ComputationExprLibrary
{
  .ver 0:0:0:0
}
.assembly ComputationExpr07
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ComputationExpr07
{
  // Offset: 0x00000000 Length: 0x00000216
}
.mresource public FSharpOptimizationData.ComputationExpr07
{
  // Offset: 0x00000220 Length: 0x0000007D
}
.module ComputationExpr07.exe
// MVID: {590846DC-35BD-E566-A745-0383DC460859}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01600000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ComputationExpr07
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit 'res7@9-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@,
                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/'res7@9-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ComputationExpr07/'res7@9-1'::x
      IL_0014:  ret
    } // end of method 'res7@9-1'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       42 (0x2a)
      .maxstack  7
      .locals init ([0] int32 v)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 9,9 : 9,29 'C:\\src\\manofstick\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\ComputationExpressions\\ComputationExpr07.fs'
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 10,10 : 13,24 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ComputationExpr07/'res7@9-1'::x
      IL_0009:  ldarg.0
      IL_000a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ComputationExpr07/'res7@9-1'::x
      IL_000f:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
      IL_0014:  ldloc.0
      IL_0015:  sub
      IL_0016:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::op_ColonEquals<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>,
                                                                                                    !!0)
      IL_001b:  nop
      IL_001c:  ldarg.0
      IL_001d:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/'res7@9-1'::builder@
      IL_0022:  tail.
      IL_0024:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> [ComputationExprLibrary]Library.EventuallyBuilder::Zero()
      IL_0029:  ret
    } // end of method 'res7@9-1'::Invoke

  } // end of class 'res7@9-1'

  .class auto ansi serializable nested assembly beforefieldinit 'res7@11-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@,
                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/'res7@11-2'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ComputationExpr07/'res7@11-2'::x
      IL_0014:  ret
    } // end of method 'res7@11-2'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       26 (0x1a)
      .maxstack  8
      .line 11,11 : 9,18 ''
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/'res7@11-2'::builder@
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> ComputationExpr07/'res7@11-2'::x
      IL_000d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
      IL_0012:  tail.
      IL_0014:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_0019:  ret
    } // end of method 'res7@11-2'::Invoke

  } // end of class 'res7@11-2'

  .class auto ansi serializable nested assembly beforefieldinit res7@8
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/res7@8::builder@
      IL_000d:  ret
    } // end of method res7@8::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       86 (0x56)
      .maxstack  9
      .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x)
      .line 8,8 : 9,22 ''
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
      IL_0007:  stloc.0
      .line 9,9 : 9,29 ''
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/res7@8::builder@
      IL_000e:  ldarg.0
      IL_000f:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/res7@8::builder@
      IL_0014:  ldc.i4.0
      IL_0015:  ldc.i4.1
      IL_0016:  ldc.i4.3
      IL_0017:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_001c:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0021:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0026:  ldarg.0
      IL_0027:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/res7@8::builder@
      IL_002c:  ldloc.0
      IL_002d:  newobj     instance void ComputationExpr07/'res7@9-1'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder,
                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_0032:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> [ComputationExprLibrary]Library.EventuallyBuilder::For<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>>)
      IL_0037:  ldarg.0
      IL_0038:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/res7@8::builder@
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr07/res7@8::builder@
      IL_0043:  ldloc.0
      IL_0044:  newobj     instance void ComputationExpr07/'res7@11-2'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder,
                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_0049:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_004e:  tail.
      IL_0050:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Combine<int32>(class [ComputationExprLibrary]Library.Eventually`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>,
                                                                                                                                                              class [ComputationExprLibrary]Library.Eventually`1<!!0>)
      IL_0055:  ret
    } // end of method res7@8::Invoke

  } // end of class res7@8

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res7() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr07>'.$ComputationExpr07::res7@6
    IL_0005:  ret
  } // end of method ComputationExpr07::get_res7

  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res7()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr07::get_res7()
  } // end of property ComputationExpr07::res7
} // end of class ComputationExpr07

.class private abstract auto ansi sealed '<StartupCode$ComputationExpr07>'.$ComputationExpr07
       extends [mscorlib]System.Object
{
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res7@6
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       38 (0x26)
    .maxstack  4
    .locals init ([0] class [ComputationExprLibrary]Library.Eventually`1<int32> res7,
             [1] class [ComputationExprLibrary]Library.EventuallyBuilder builder@)
    .line 13,13 : 1,25 ''
    IL_0000:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldloc.1
    IL_0008:  newobj     instance void ComputationExpr07/res7@8::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_000d:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_0012:  dup
    IL_0013:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr07>'.$ComputationExpr07::res7@6
    IL_0018:  stloc.0
    IL_0019:  nop
    IL_001a:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr07::get_res7()
    IL_001f:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0024:  pop
    IL_0025:  ret
  } // end of method $ComputationExpr07::main@

} // end of class '<StartupCode$ComputationExpr07>'.$ComputationExpr07


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
