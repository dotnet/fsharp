
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
.assembly ListExpressionSteppingTest2
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionSteppingTest2
{
  // Offset: 0x00000000 Length: 0x0000026D
}
.mresource public FSharpOptimizationData.ListExpressionSteppingTest2
{
  // Offset: 0x00000278 Length: 0x000000AF
}
.module ListExpressionSteppingTest2.exe
// MVID: {59B1920C-D3DE-B780-A745-03830C92B159}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00690000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest2
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest2
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f1@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public int32 pc
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(int32 pc,
                                   int32 current) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::pc
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::current
        IL_000e:  ldarg.0
        IL_000f:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0014:  ret
      } // end of method f1@6::.ctor

      .method public strict virtual instance int32 
              GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        // Code size       123 (0x7b)
        .maxstack  6
        .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
        .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\ListExpressionStepping\\ListExpressionSteppingTest2.fs'
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001b,
                              IL_001d,
                              IL_001f)
        IL_0019:  br.s       IL_002a

        IL_001b:  br.s       IL_0021

        IL_001d:  br.s       IL_0024

        IL_001f:  br.s       IL_0027

        .line 100001,100001 : 0,0 ''
        IL_0021:  nop
        IL_0022:  br.s       IL_004b

        .line 100001,100001 : 0,0 ''
        IL_0024:  nop
        IL_0025:  br.s       IL_006b

        .line 100001,100001 : 0,0 ''
        IL_0027:  nop
        IL_0028:  br.s       IL_0072

        .line 100001,100001 : 0,0 ''
        IL_002a:  nop
        .line 6,6 : 11,26 ''
        IL_002b:  ldstr      "hello"
        IL_0030:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0035:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_003a:  pop
        IL_003b:  ldarg.0
        IL_003c:  ldc.i4.1
        IL_003d:  stfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::pc
        .line 7,7 : 11,18 ''
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.1
        IL_0044:  stfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::current
        IL_0049:  ldc.i4.1
        IL_004a:  ret

        .line 8,8 : 11,28 ''
        IL_004b:  ldstr      "goodbye"
        IL_0050:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0055:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_005a:  pop
        IL_005b:  ldarg.0
        IL_005c:  ldc.i4.2
        IL_005d:  stfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::pc
        .line 9,9 : 11,18 ''
        IL_0062:  ldarg.0
        IL_0063:  ldc.i4.2
        IL_0064:  stfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::current
        IL_0069:  ldc.i4.1
        IL_006a:  ret

        IL_006b:  ldarg.0
        IL_006c:  ldc.i4.3
        IL_006d:  stfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::pc
        IL_0072:  ldarg.0
        IL_0073:  ldc.i4.0
        IL_0074:  stfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::current
        IL_0079:  ldc.i4.0
        IL_007a:  ret
      } // end of method f1@6::GenerateNext

      .method public strict virtual instance void 
              Close() cil managed
      {
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.3
        IL_0002:  stfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::pc
        IL_0007:  ret
      } // end of method f1@6::Close

      .method public strict virtual instance bool 
              get_CheckClose() cil managed
      {
        // Code size       56 (0x38)
        .maxstack  8
        .line 100001,100001 : 0,0 ''
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::pc
        IL_0006:  switch     ( 
                              IL_001d,
                              IL_001f,
                              IL_0021,
                              IL_0023)
        IL_001b:  br.s       IL_0031

        IL_001d:  br.s       IL_0025

        IL_001f:  br.s       IL_0028

        IL_0021:  br.s       IL_002b

        IL_0023:  br.s       IL_002e

        .line 100001,100001 : 0,0 ''
        IL_0025:  nop
        IL_0026:  br.s       IL_0036

        .line 100001,100001 : 0,0 ''
        IL_0028:  nop
        IL_0029:  br.s       IL_0034

        .line 100001,100001 : 0,0 ''
        IL_002b:  nop
        IL_002c:  br.s       IL_0032

        .line 100001,100001 : 0,0 ''
        IL_002e:  nop
        IL_002f:  br.s       IL_0036

        .line 100001,100001 : 0,0 ''
        IL_0031:  nop
        IL_0032:  ldc.i4.0
        IL_0033:  ret

        IL_0034:  ldc.i4.0
        IL_0035:  ret

        IL_0036:  ldc.i4.0
        IL_0037:  ret
      } // end of method f1@6::get_CheckClose

      .method public strict virtual instance int32 
              get_LastGenerated() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::current
        IL_0006:  ret
      } // end of method f1@6::get_LastGenerated

      .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
              GetFreshEnumerator() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldc.i4.0
        IL_0001:  ldc.i4.0
        IL_0002:  newobj     instance void ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::.ctor(int32,
                                                                                                               int32)
        IL_0007:  ret
      } // end of method f1@6::GetFreshEnumerator

    } // end of class f1@6

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f1() cil managed
    {
      // Code size       15 (0xf)
      .maxstack  8
      .line 6,9 : 9,19 ''
      IL_0000:  ldc.i4.0
      IL_0001:  ldc.i4.0
      IL_0002:  newobj     instance void ListExpressionSteppingTest2/ListExpressionSteppingTest2/f1@6::.ctor(int32,
                                                                                                             int32)
      IL_0007:  tail.
      IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000e:  ret
    } // end of method ListExpressionSteppingTest2::f1

  } // end of class ListExpressionSteppingTest2

} // end of class ListExpressionSteppingTest2

.class private abstract auto ansi sealed '<StartupCode$ListExpressionSteppingTest2>'.$ListExpressionSteppingTest2
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       7 (0x7)
    .maxstack  8
    .line 11,11 : 13,17 ''
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest2/ListExpressionSteppingTest2::f1()
    IL_0005:  pop
    IL_0006:  ret
  } // end of method $ListExpressionSteppingTest2::main@

} // end of class '<StartupCode$ListExpressionSteppingTest2>'.$ListExpressionSteppingTest2


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
