
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
.assembly extern ComputationExprLibrary
{
  .ver 0:0:0:0
}
.assembly ComputationExpr05
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ComputationExpr05
{
  // Offset: 0x00000000 Length: 0x00000212
}
.mresource public FSharpOptimizationData.ComputationExpr05
{
  // Offset: 0x00000218 Length: 0x0000007D
}
.module ComputationExpr05.exe
// MVID: {5775B6CC-3687-E566-A745-0383CCB67557}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x011D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ComputationExpr05
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'res5@9-1'
         extends [mscorlib]System.Object
         implements [mscorlib]System.IDisposable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0007:  ldarg.0
      IL_0008:  pop
      IL_0009:  ret
    } // end of method 'res5@9-1'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  'System-IDisposable-Dispose'() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       2 (0x2)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 9,9 : 68,70 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\ComputationExpressions\\ComputationExpr05.fs'
      IL_0000:  nop
      IL_0001:  ret
    } // end of method 'res5@9-1'::'System-IDisposable-Dispose'

  } // end of class 'res5@9-1'

  .class auto ansi serializable nested assembly beforefieldinit 'res5@10-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.IDisposable,class [ComputationExprLibrary]Library.Eventually`1<int32>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.IDisposable,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr05/'res5@10-2'::builder@
      IL_000d:  ret
    } // end of method 'res5@10-2'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [mscorlib]System.IDisposable _arg1) cil managed
    {
      // Code size       46 (0x2e)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.IDisposable x,
               [1] int32 V_1)
      .line 10,10 : 9,50
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 10,10 : 9,50
      IL_0003:  nop
      .line 10,10 : 18,33
      IL_0004:  ldstr      "hello"
      IL_0009:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0013:  pop
      .line 10,10 : 35,49
      IL_0014:  ldstr      "hello"
      IL_0019:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_001e:  stloc.1
      .line 11,11 : 9,17
      IL_001f:  ldarg.0
      IL_0020:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr05/'res5@10-2'::builder@
      IL_0025:  ldc.i4.1
      IL_0026:  tail.
      IL_0028:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_002d:  ret
    } // end of method 'res5@10-2'::Invoke

  } // end of class 'res5@10-2'

  .class auto ansi serializable nested assembly beforefieldinit res5@8
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
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr05/res5@8::builder@
      IL_000d:  ret
    } // end of method res5@8::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       59 (0x3b)
      .maxstack  7
      .locals init ([0] int32 x)
      .line 8,8 : 9,50
      IL_0000:  nop
      IL_0001:  nop
      .line 8,8 : 18,33
      IL_0002:  ldstr      "hello"
      IL_0007:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0011:  pop
      .line 8,8 : 35,49
      IL_0012:  ldstr      "hello"
      IL_0017:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_001c:  stloc.0
      .line 9,9 : 17,72
      IL_001d:  ldarg.0
      IL_001e:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr05/res5@8::builder@
      IL_0023:  newobj     instance void ComputationExpr05/'res5@9-1'::.ctor()
      IL_0028:  ldarg.0
      IL_0029:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder ComputationExpr05/res5@8::builder@
      IL_002e:  newobj     instance void ComputationExpr05/'res5@10-2'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0033:  tail.
      IL_0035:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Using<int32>(class [mscorlib]System.IDisposable,
                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.IDisposable,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_003a:  ret
    } // end of method res5@8::Invoke

  } // end of class res5@8

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res5() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr05>'.$ComputationExpr05::res5@6
    IL_0005:  ret
  } // end of method ComputationExpr05::get_res5

  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res5()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr05::get_res5()
  } // end of property ComputationExpr05::res5
} // end of class ComputationExpr05

.class private abstract auto ansi sealed '<StartupCode$ComputationExpr05>'.$ComputationExpr05
       extends [mscorlib]System.Object
{
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res5@6
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
    .locals init ([0] class [ComputationExprLibrary]Library.Eventually`1<int32> res5,
             [1] class [ComputationExprLibrary]Library.EventuallyBuilder builder@)
    .line 13,13 : 1,25
    IL_0000:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldloc.1
    IL_0008:  newobj     instance void ComputationExpr05/res5@8::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_000d:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_0012:  dup
    IL_0013:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr05>'.$ComputationExpr05::res5@6
    IL_0018:  stloc.0
    IL_0019:  nop
    IL_001a:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> ComputationExpr05::get_res5()
    IL_001f:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0024:  pop
    IL_0025:  ret
  } // end of method $ComputationExpr05::main@

} // end of class '<StartupCode$ComputationExpr05>'.$ComputationExpr05


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
