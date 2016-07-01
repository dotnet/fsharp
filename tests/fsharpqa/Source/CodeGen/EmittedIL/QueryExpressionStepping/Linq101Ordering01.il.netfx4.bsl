
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
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly Linq101Ordering01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Ordering01
{
  // Offset: 0x00000000 Length: 0x000003B2
}
.mresource public FSharpOptimizationData.Linq101Ordering01
{
  // Offset: 0x000003B8 Length: 0x00000134
}
.module Linq101Ordering01.exe
// MVID: {5775B16C-649A-6956-A745-03836CB17557}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x006E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Ordering01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname sortedWords@11
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public string _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string w
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(string _arg1,
                                 string w,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string Linq101Ordering01/sortedWords@11::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      string Linq101Ordering01/sortedWords@11::w
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords@11::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Ordering01/sortedWords@11::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      string Linq101Ordering01/sortedWords@11::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_002b:  ret
    } // end of method sortedWords@11::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Ordering01.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/sortedWords@11::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_002d:  nop
      .line 11,11 : 9,26
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_words()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords@11::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Ordering01/sortedWords@11::pc
      .line 11,11 : 9,26
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords@11::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords@11::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0063:  stfld      string Linq101Ordering01/sortedWords@11::_arg1
      .line 11,11 : 9,26
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      string Linq101Ordering01/sortedWords@11::_arg1
      IL_006f:  stfld      string Linq101Ordering01/sortedWords@11::w
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Ordering01/sortedWords@11::pc
      .line 12,12 : 9,17
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      string Linq101Ordering01/sortedWords@11::w
      IL_0082:  stfld      string Linq101Ordering01/sortedWords@11::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldnull
      IL_008b:  stfld      string Linq101Ordering01/sortedWords@11::w
      .line 11,11 : 9,26
      IL_0090:  ldarg.0
      IL_0091:  ldnull
      IL_0092:  stfld      string Linq101Ordering01/sortedWords@11::_arg1
      .line 100001,100001 : 0,0
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Ordering01/sortedWords@11::pc
      .line 11,11 : 9,26
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords@11::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords@11::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Ordering01/sortedWords@11::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldnull
      IL_00bd:  stfld      string Linq101Ordering01/sortedWords@11::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method sortedWords@11::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Ordering01/sortedWords@11::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Ordering01/sortedWords@11::pc
        IL_0022:  switch     ( 
                              IL_0039,
                              IL_003b,
                              IL_003d,
                              IL_003f)
        IL_0037:  br.s       IL_004d

        IL_0039:  br.s       IL_0041

        IL_003b:  br.s       IL_0044

        IL_003d:  br.s       IL_0047

        IL_003f:  br.s       IL_004a

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_004d:  nop
        .line 100001,100001 : 0,0
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Ordering01/sortedWords@11::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords@11::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Ordering01/sortedWords@11::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      string Linq101Ordering01/sortedWords@11::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 11,11 : 9,26
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0
      IL_0095:  ret
    } // end of method sortedWords@11::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Ordering01/sortedWords@11::pc
      IL_0007:  switch     ( 
                            IL_001e,
                            IL_0020,
                            IL_0022,
                            IL_0024)
      IL_001c:  br.s       IL_0032

      IL_001e:  br.s       IL_0026

      IL_0020:  br.s       IL_0029

      IL_0022:  br.s       IL_002c

      IL_0024:  br.s       IL_002f

      IL_0026:  nop
      IL_0027:  br.s       IL_0037

      IL_0029:  nop
      IL_002a:  br.s       IL_0035

      IL_002c:  nop
      IL_002d:  br.s       IL_0033

      IL_002f:  nop
      IL_0030:  br.s       IL_0037

      IL_0032:  nop
      IL_0033:  ldc.i4.1
      IL_0034:  ret

      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method sortedWords@11::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string Linq101Ordering01/sortedWords@11::current
      IL_0007:  ret
    } // end of method sortedWords@11::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldnull
      IL_0006:  newobj     instance void Linq101Ordering01/sortedWords@11::.ctor(string,
                                                                                 string,
                                                                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                 int32,
                                                                                 string)
      IL_000b:  ret
    } // end of method sortedWords@11::GetFreshEnumerator

  } // end of class sortedWords@11

  .class auto ansi serializable nested assembly beforefieldinit 'sortedWords@12-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedWords@12-1'::.ctor

    .method public strict virtual instance string 
            Invoke(string w) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 12,12 : 16,17
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'sortedWords@12-1'::Invoke

  } // end of class 'sortedWords@12-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname sortedWords2@18
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public string _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string w
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(string _arg1,
                                 string w,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string Linq101Ordering01/sortedWords2@18::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      string Linq101Ordering01/sortedWords2@18::w
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords2@18::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Ordering01/sortedWords2@18::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      string Linq101Ordering01/sortedWords2@18::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_002b:  ret
    } // end of method sortedWords2@18::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .line 100001,100001 : 0,0
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/sortedWords2@18::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_002d:  nop
      .line 18,18 : 9,26
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_words()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords2@18::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Ordering01/sortedWords2@18::pc
      .line 18,18 : 9,26
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords2@18::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords2@18::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0063:  stfld      string Linq101Ordering01/sortedWords2@18::_arg1
      .line 18,18 : 9,26
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      string Linq101Ordering01/sortedWords2@18::_arg1
      IL_006f:  stfld      string Linq101Ordering01/sortedWords2@18::w
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Ordering01/sortedWords2@18::pc
      .line 19,19 : 9,26
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      string Linq101Ordering01/sortedWords2@18::w
      IL_0082:  stfld      string Linq101Ordering01/sortedWords2@18::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldnull
      IL_008b:  stfld      string Linq101Ordering01/sortedWords2@18::w
      .line 18,18 : 9,26
      IL_0090:  ldarg.0
      IL_0091:  ldnull
      IL_0092:  stfld      string Linq101Ordering01/sortedWords2@18::_arg1
      .line 100001,100001 : 0,0
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Ordering01/sortedWords2@18::pc
      .line 18,18 : 9,26
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords2@18::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords2@18::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Ordering01/sortedWords2@18::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldnull
      IL_00bd:  stfld      string Linq101Ordering01/sortedWords2@18::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method sortedWords2@18::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Ordering01/sortedWords2@18::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Ordering01/sortedWords2@18::pc
        IL_0022:  switch     ( 
                              IL_0039,
                              IL_003b,
                              IL_003d,
                              IL_003f)
        IL_0037:  br.s       IL_004d

        IL_0039:  br.s       IL_0041

        IL_003b:  br.s       IL_0044

        IL_003d:  br.s       IL_0047

        IL_003f:  br.s       IL_004a

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_004d:  nop
        .line 100001,100001 : 0,0
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Ordering01/sortedWords2@18::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedWords2@18::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Ordering01/sortedWords2@18::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      string Linq101Ordering01/sortedWords2@18::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 18,18 : 9,26
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0
      IL_0095:  ret
    } // end of method sortedWords2@18::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Ordering01/sortedWords2@18::pc
      IL_0007:  switch     ( 
                            IL_001e,
                            IL_0020,
                            IL_0022,
                            IL_0024)
      IL_001c:  br.s       IL_0032

      IL_001e:  br.s       IL_0026

      IL_0020:  br.s       IL_0029

      IL_0022:  br.s       IL_002c

      IL_0024:  br.s       IL_002f

      IL_0026:  nop
      IL_0027:  br.s       IL_0037

      IL_0029:  nop
      IL_002a:  br.s       IL_0035

      IL_002c:  nop
      IL_002d:  br.s       IL_0033

      IL_002f:  nop
      IL_0030:  br.s       IL_0037

      IL_0032:  nop
      IL_0033:  ldc.i4.1
      IL_0034:  ret

      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method sortedWords2@18::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string Linq101Ordering01/sortedWords2@18::current
      IL_0007:  ret
    } // end of method sortedWords2@18::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldnull
      IL_0006:  newobj     instance void Linq101Ordering01/sortedWords2@18::.ctor(string,
                                                                                  string,
                                                                                  class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
      IL_000b:  ret
    } // end of method sortedWords2@18::GetFreshEnumerator

  } // end of class sortedWords2@18

  .class auto ansi serializable nested assembly beforefieldinit 'sortedWords2@19-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedWords2@19-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string w) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 19,19 : 17,25
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0007:  ret
    } // end of method 'sortedWords2@19-1'::Invoke

  } // end of class 'sortedWords2@19-1'

  .class auto ansi serializable nested assembly beforefieldinit sortedProducts@26
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Ordering01/sortedProducts@26::builder@
      IL_000d:  ret
    } // end of method sortedProducts@26::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 26,26 : 9,29
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 27,27 : 9,29
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Ordering01/sortedProducts@26::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method sortedProducts@26::Invoke

  } // end of class sortedProducts@26

  .class auto ansi serializable nested assembly beforefieldinit 'sortedProducts@27-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedProducts@27-1'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 27,27 : 16,29
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0009:  ret
    } // end of method 'sortedProducts@27-1'::Invoke

  } // end of class 'sortedProducts@27-1'

  .class auto ansi serializable nested assembly beforefieldinit 'sortedProducts@28-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedProducts@28-2'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 28,28 : 16,17
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'sortedProducts@28-2'::Invoke

  } // end of class 'sortedProducts@28-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname sortedProducts2@44
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [Utils]Utils/Product _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product p
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [Utils]Utils/Product _arg1,
                                 class [Utils]Utils/Product p,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::p
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/sortedProducts2@44::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Ordering01/sortedProducts2@44::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_002b:  ret
    } // end of method sortedProducts2@44::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .line 100001,100001 : 0,0
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/sortedProducts2@44::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_002d:  nop
      .line 44,44 : 9,29
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/sortedProducts2@44::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Ordering01/sortedProducts2@44::pc
      .line 44,44 : 9,29
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/sortedProducts2@44::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/sortedProducts2@44::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0063:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::_arg1
      .line 44,44 : 9,29
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::_arg1
      IL_006f:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::p
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Ordering01/sortedProducts2@44::pc
      .line 45,45 : 9,40
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::p
      IL_0082:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldnull
      IL_008b:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::p
      .line 44,44 : 9,29
      IL_0090:  ldarg.0
      IL_0091:  ldnull
      IL_0092:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::_arg1
      .line 100001,100001 : 0,0
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Ordering01/sortedProducts2@44::pc
      .line 44,44 : 9,29
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/sortedProducts2@44::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/sortedProducts2@44::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Ordering01/sortedProducts2@44::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldnull
      IL_00bd:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method sortedProducts2@44::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Ordering01/sortedProducts2@44::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Ordering01/sortedProducts2@44::pc
        IL_0022:  switch     ( 
                              IL_0039,
                              IL_003b,
                              IL_003d,
                              IL_003f)
        IL_0037:  br.s       IL_004d

        IL_0039:  br.s       IL_0041

        IL_003b:  br.s       IL_0044

        IL_003d:  br.s       IL_0047

        IL_003f:  br.s       IL_004a

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_004d:  nop
        .line 100001,100001 : 0,0
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Ordering01/sortedProducts2@44::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/sortedProducts2@44::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Ordering01/sortedProducts2@44::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 44,44 : 9,29
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0
      IL_0095:  ret
    } // end of method sortedProducts2@44::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Ordering01/sortedProducts2@44::pc
      IL_0007:  switch     ( 
                            IL_001e,
                            IL_0020,
                            IL_0022,
                            IL_0024)
      IL_001c:  br.s       IL_0032

      IL_001e:  br.s       IL_0026

      IL_0020:  br.s       IL_0029

      IL_0022:  br.s       IL_002c

      IL_0024:  br.s       IL_002f

      IL_0026:  nop
      IL_0027:  br.s       IL_0037

      IL_0029:  nop
      IL_002a:  br.s       IL_0035

      IL_002c:  nop
      IL_002d:  br.s       IL_0033

      IL_002f:  nop
      IL_0030:  br.s       IL_0037

      IL_0032:  nop
      IL_0033:  ldc.i4.1
      IL_0034:  ret

      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method sortedProducts2@44::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [Utils]Utils/Product Linq101Ordering01/sortedProducts2@44::current
      IL_0007:  ret
    } // end of method sortedProducts2@44::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldnull
      IL_0006:  newobj     instance void Linq101Ordering01/sortedProducts2@44::.ctor(class [Utils]Utils/Product,
                                                                                     class [Utils]Utils/Product,
                                                                                     class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                     int32,
                                                                                     class [Utils]Utils/Product)
      IL_000b:  ret
    } // end of method sortedProducts2@44::GetFreshEnumerator

  } // end of class sortedProducts2@44

  .class auto ansi serializable nested assembly beforefieldinit 'sortedProducts2@45-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedProducts2@45-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 45,45 : 26,40
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0009:  ret
    } // end of method 'sortedProducts2@45-1'::Invoke

  } // end of class 'sortedProducts2@45-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname sortedDigits@52
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public string _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string d
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(string _arg1,
                                 string d,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string Linq101Ordering01/sortedDigits@52::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      string Linq101Ordering01/sortedDigits@52::d
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedDigits@52::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Ordering01/sortedDigits@52::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      string Linq101Ordering01/sortedDigits@52::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_002b:  ret
    } // end of method sortedDigits@52::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       196 (0xc4)
      .maxstack  6
      .line 100001,100001 : 0,0
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/sortedDigits@52::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_002d

      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0024

      IL_001f:  br.s       IL_0027

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0021:  nop
      IL_0022:  br.s       IL_009a

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0024:  nop
      IL_0025:  br.s       IL_0089

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0027:  nop
      IL_0028:  br         IL_00bb

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_002d:  nop
      .line 52,52 : 9,27
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_digits()
      IL_0034:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
      IL_0039:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003e:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedDigits@52::'enum'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 Linq101Ordering01/sortedDigits@52::pc
      .line 52,52 : 9,27
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedDigits@52::'enum'
      IL_0050:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0055:  brfalse.s  IL_009a

      IL_0057:  ldarg.0
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedDigits@52::'enum'
      IL_005e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0063:  stfld      string Linq101Ordering01/sortedDigits@52::_arg1
      .line 52,52 : 9,27
      IL_0068:  ldarg.0
      IL_0069:  ldarg.0
      IL_006a:  ldfld      string Linq101Ordering01/sortedDigits@52::_arg1
      IL_006f:  stfld      string Linq101Ordering01/sortedDigits@52::d
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 Linq101Ordering01/sortedDigits@52::pc
      .line 53,53 : 9,24
      IL_007b:  ldarg.0
      IL_007c:  ldarg.0
      IL_007d:  ldfld      string Linq101Ordering01/sortedDigits@52::d
      IL_0082:  stfld      string Linq101Ordering01/sortedDigits@52::current
      IL_0087:  ldc.i4.1
      IL_0088:  ret

      IL_0089:  ldarg.0
      IL_008a:  ldnull
      IL_008b:  stfld      string Linq101Ordering01/sortedDigits@52::d
      .line 52,52 : 9,27
      IL_0090:  ldarg.0
      IL_0091:  ldnull
      IL_0092:  stfld      string Linq101Ordering01/sortedDigits@52::_arg1
      .line 100001,100001 : 0,0
      IL_0097:  nop
      IL_0098:  br.s       IL_004a

      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101Ordering01/sortedDigits@52::pc
      .line 52,52 : 9,27
      IL_00a1:  ldarg.0
      IL_00a2:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedDigits@52::'enum'
      IL_00a7:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00ac:  nop
      IL_00ad:  ldarg.0
      IL_00ae:  ldnull
      IL_00af:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedDigits@52::'enum'
      IL_00b4:  ldarg.0
      IL_00b5:  ldc.i4.3
      IL_00b6:  stfld      int32 Linq101Ordering01/sortedDigits@52::pc
      IL_00bb:  ldarg.0
      IL_00bc:  ldnull
      IL_00bd:  stfld      string Linq101Ordering01/sortedDigits@52::current
      IL_00c2:  ldc.i4.0
      IL_00c3:  ret
    } // end of method sortedDigits@52::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Ordering01/sortedDigits@52::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0
      .line 100001,100001 : 0,0
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Ordering01/sortedDigits@52::pc
        IL_0022:  switch     ( 
                              IL_0039,
                              IL_003b,
                              IL_003d,
                              IL_003f)
        IL_0037:  br.s       IL_004d

        IL_0039:  br.s       IL_0041

        IL_003b:  br.s       IL_0044

        IL_003d:  br.s       IL_0047

        IL_003f:  br.s       IL_004a

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0
        .line 100001,100001 : 0,0
        IL_004d:  nop
        .line 100001,100001 : 0,0
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Ordering01/sortedDigits@52::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/sortedDigits@52::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Ordering01/sortedDigits@52::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      string Linq101Ordering01/sortedDigits@52::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 52,52 : 9,27
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0
      IL_0095:  ret
    } // end of method sortedDigits@52::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Ordering01/sortedDigits@52::pc
      IL_0007:  switch     ( 
                            IL_001e,
                            IL_0020,
                            IL_0022,
                            IL_0024)
      IL_001c:  br.s       IL_0032

      IL_001e:  br.s       IL_0026

      IL_0020:  br.s       IL_0029

      IL_0022:  br.s       IL_002c

      IL_0024:  br.s       IL_002f

      IL_0026:  nop
      IL_0027:  br.s       IL_0037

      IL_0029:  nop
      IL_002a:  br.s       IL_0035

      IL_002c:  nop
      IL_002d:  br.s       IL_0033

      IL_002f:  nop
      IL_0030:  br.s       IL_0037

      IL_0032:  nop
      IL_0033:  ldc.i4.1
      IL_0034:  ret

      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldc.i4.0
      IL_0038:  ret
    } // end of method sortedDigits@52::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string Linq101Ordering01/sortedDigits@52::current
      IL_0007:  ret
    } // end of method sortedDigits@52::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldnull
      IL_0006:  newobj     instance void Linq101Ordering01/sortedDigits@52::.ctor(string,
                                                                                  string,
                                                                                  class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
      IL_000b:  ret
    } // end of method sortedDigits@52::GetFreshEnumerator

  } // end of class sortedDigits@52

  .class auto ansi serializable nested assembly beforefieldinit 'sortedDigits@53-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedDigits@53-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string d) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 53,53 : 16,24
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0007:  ret
    } // end of method 'sortedDigits@53-1'::Invoke

  } // end of class 'sortedDigits@53-1'

  .class auto ansi serializable nested assembly beforefieldinit 'sortedDigits@54-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedDigits@54-2'::.ctor

    .method public strict virtual instance string 
            Invoke(string d) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 54,54 : 16,17
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'sortedDigits@54-2'::Invoke

  } // end of class 'sortedDigits@54-2'

  .class auto ansi serializable nested assembly beforefieldinit sortedProducts3@60
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Ordering01/sortedProducts3@60::builder@
      IL_000d:  ret
    } // end of method sortedProducts3@60::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 60,60 : 9,29
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 61,61 : 9,26
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Ordering01/sortedProducts3@60::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method sortedProducts3@60::Invoke

  } // end of class sortedProducts3@60

  .class auto ansi serializable nested assembly beforefieldinit 'sortedProducts3@61-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedProducts3@61-1'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 61,61 : 16,26
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'sortedProducts3@61-1'::Invoke

  } // end of class 'sortedProducts3@61-1'

  .class auto ansi serializable nested assembly beforefieldinit 'sortedProducts3@62-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedProducts3@62-2'::.ctor

    .method public strict virtual instance valuetype [mscorlib]System.Decimal 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 62,62 : 26,37
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0009:  ret
    } // end of method 'sortedProducts3@62-2'::Invoke

  } // end of class 'sortedProducts3@62-2'

  .class auto ansi serializable nested assembly beforefieldinit 'sortedProducts3@63-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'sortedProducts3@63-3'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 63,63 : 16,17
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'sortedProducts3@63-3'::Invoke

  } // end of class 'sortedProducts3@63-3'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_words() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::'words@8-2'
    IL_0005:  ret
  } // end of method Linq101Ordering01::get_words

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_sortedWords() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedWords@9
    IL_0005:  ret
  } // end of method Linq101Ordering01::get_sortedWords

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_sortedWords2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedWords2@16
    IL_0005:  ret
  } // end of method Linq101Ordering01::get_sortedWords2

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::'products@23-6'
    IL_0005:  ret
  } // end of method Linq101Ordering01::get_products

  .method public specialname static class [Utils]Utils/Product[] 
          get_sortedProducts() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts@24
    IL_0005:  ret
  } // end of method Linq101Ordering01::get_sortedProducts

  .method public specialname static class [Utils]Utils/Product[] 
          get_sortedProducts2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts2@42
    IL_0005:  ret
  } // end of method Linq101Ordering01::get_sortedProducts2

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_digits() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::'digits@49-2'
    IL_0005:  ret
  } // end of method Linq101Ordering01::get_digits

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_sortedDigits() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedDigits@50
    IL_0005:  ret
  } // end of method Linq101Ordering01::get_sortedDigits

  .method public specialname static class [Utils]Utils/Product[] 
          get_sortedProducts3() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts3@58
    IL_0005:  ret
  } // end of method Linq101Ordering01::get_sortedProducts3

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_words()
  } // end of property Linq101Ordering01::words
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          sortedWords()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_sortedWords()
  } // end of property Linq101Ordering01::sortedWords
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          sortedWords2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_sortedWords2()
  } // end of property Linq101Ordering01::sortedWords2
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
  } // end of property Linq101Ordering01::products
  .property class [Utils]Utils/Product[] sortedProducts()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [Utils]Utils/Product[] Linq101Ordering01::get_sortedProducts()
  } // end of property Linq101Ordering01::sortedProducts
  .property class [Utils]Utils/Product[] sortedProducts2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [Utils]Utils/Product[] Linq101Ordering01::get_sortedProducts2()
  } // end of property Linq101Ordering01::sortedProducts2
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          digits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_digits()
  } // end of property Linq101Ordering01::digits
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          sortedDigits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_sortedDigits()
  } // end of property Linq101Ordering01::sortedDigits
  .property class [Utils]Utils/Product[] sortedProducts3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [Utils]Utils/Product[] Linq101Ordering01::get_sortedProducts3()
  } // end of property Linq101Ordering01::sortedProducts3
} // end of class Linq101Ordering01

.class private abstract auto ansi sealed '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 'words@8-2'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> sortedWords@9
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> sortedWords2@16
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 'products@23-6'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [Utils]Utils/Product[] sortedProducts@24
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [Utils]Utils/Product[] sortedProducts2@42
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 'digits@49-2'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> sortedDigits@50
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [Utils]Utils/Product[] sortedProducts3@58
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       559 (0x22f)
    .maxstack  13
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> sortedWords,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> sortedWords2,
             [3] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products,
             [4] class [Utils]Utils/Product[] sortedProducts,
             [5] class [Utils]Utils/Product[] sortedProducts2,
             [6] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits,
             [7] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> sortedDigits,
             [8] class [Utils]Utils/Product[] sortedProducts3,
             [9] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
             [10] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             [11] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11,
             [12] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             [13] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_13,
             [14] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_14)
    .line 8,8 : 1,45
    IL_0000:  nop
    IL_0001:  ldstr      "cherry"
    IL_0006:  ldstr      "apple"
    IL_000b:  ldstr      "blueberry"
    IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0024:  dup
    IL_0025:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::'words@8-2'
    IL_002a:  stloc.0
    .line 9,13 : 1,20
    IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0030:  stloc.s    builder@
    IL_0032:  ldloc.s    builder@
    IL_0034:  ldnull
    IL_0035:  ldnull
    IL_0036:  ldnull
    IL_0037:  ldc.i4.0
    IL_0038:  ldnull
    IL_0039:  newobj     instance void Linq101Ordering01/sortedWords@11::.ctor(string,
                                                                               string,
                                                                               class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                               int32,
                                                                               string)
    IL_003e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0043:  newobj     instance void Linq101Ordering01/'sortedWords@12-1'::.ctor()
    IL_0048:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<string,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_004d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0052:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0057:  dup
    IL_0058:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedWords@9
    IL_005d:  stloc.1
    .line 16,20 : 1,20
    IL_005e:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0063:  stloc.s    V_10
    IL_0065:  ldloc.s    V_10
    IL_0067:  ldnull
    IL_0068:  ldnull
    IL_0069:  ldnull
    IL_006a:  ldc.i4.0
    IL_006b:  ldnull
    IL_006c:  newobj     instance void Linq101Ordering01/sortedWords2@18::.ctor(string,
                                                                                string,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
    IL_0071:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0076:  newobj     instance void Linq101Ordering01/'sortedWords2@19-1'::.ctor()
    IL_007b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0080:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0085:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_008a:  dup
    IL_008b:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedWords2@16
    IL_0090:  stloc.2
    .line 23,23 : 1,32
    IL_0091:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_0096:  dup
    IL_0097:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::'products@23-6'
    IL_009c:  stloc.3
    .line 24,29 : 1,21
    IL_009d:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a2:  stloc.s    V_11
    IL_00a4:  ldloc.s    V_11
    IL_00a6:  ldloc.s    V_11
    IL_00a8:  ldloc.s    V_11
    IL_00aa:  ldloc.s    V_11
    IL_00ac:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
    IL_00b1:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_00b6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00bb:  ldloc.s    V_11
    IL_00bd:  newobj     instance void Linq101Ordering01/sortedProducts@26::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00c2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00c7:  newobj     instance void Linq101Ordering01/'sortedProducts@27-1'::.ctor()
    IL_00cc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00d1:  newobj     instance void Linq101Ordering01/'sortedProducts@28-2'::.ctor()
    IL_00d6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00db:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00e0:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00e5:  dup
    IL_00e6:  stsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts@24
    IL_00eb:  stloc.s    sortedProducts
    .line 42,46 : 1,21
    IL_00ed:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00f2:  stloc.s    V_12
    IL_00f4:  ldloc.s    V_12
    IL_00f6:  ldnull
    IL_00f7:  ldnull
    IL_00f8:  ldnull
    IL_00f9:  ldc.i4.0
    IL_00fa:  ldnull
    IL_00fb:  newobj     instance void Linq101Ordering01/sortedProducts2@44::.ctor(class [Utils]Utils/Product,
                                                                                   class [Utils]Utils/Product,
                                                                                   class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                   int32,
                                                                                   class [Utils]Utils/Product)
    IL_0100:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0105:  newobj     instance void Linq101Ordering01/'sortedProducts2@45-1'::.ctor()
    IL_010a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortByDescending<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_010f:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0114:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0119:  dup
    IL_011a:  stsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts2@42
    IL_011f:  stloc.s    sortedProducts2
    .line 49,49 : 1,96
    IL_0121:  ldstr      "zero"
    IL_0126:  ldstr      "one"
    IL_012b:  ldstr      "two"
    IL_0130:  ldstr      "three"
    IL_0135:  ldstr      "four"
    IL_013a:  ldstr      "five"
    IL_013f:  ldstr      "six"
    IL_0144:  ldstr      "seven"
    IL_0149:  ldstr      "eight"
    IL_014e:  ldstr      "nine"
    IL_0153:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0158:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_015d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0162:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0167:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_016c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0171:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0176:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_017b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0180:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0185:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_018a:  dup
    IL_018b:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::'digits@49-2'
    IL_0190:  stloc.s    digits
    .line 50,55 : 1,20
    IL_0192:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0197:  stloc.s    V_13
    IL_0199:  ldloc.s    V_13
    IL_019b:  ldloc.s    V_13
    IL_019d:  ldnull
    IL_019e:  ldnull
    IL_019f:  ldnull
    IL_01a0:  ldc.i4.0
    IL_01a1:  ldnull
    IL_01a2:  newobj     instance void Linq101Ordering01/sortedDigits@52::.ctor(string,
                                                                                string,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
    IL_01a7:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_01ac:  newobj     instance void Linq101Ordering01/'sortedDigits@53-1'::.ctor()
    IL_01b1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01b6:  newobj     instance void Linq101Ordering01/'sortedDigits@54-2'::.ctor()
    IL_01bb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::ThenBy<string,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01c0:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01c5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01ca:  dup
    IL_01cb:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedDigits@50
    IL_01d0:  stloc.s    sortedDigits
    .line 58,64 : 1,21
    IL_01d2:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01d7:  stloc.s    V_14
    IL_01d9:  ldloc.s    V_14
    IL_01db:  ldloc.s    V_14
    IL_01dd:  ldloc.s    V_14
    IL_01df:  ldloc.s    V_14
    IL_01e1:  ldloc.s    V_14
    IL_01e3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
    IL_01e8:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
    IL_01ed:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01f2:  ldloc.s    V_14
    IL_01f4:  newobj     instance void Linq101Ordering01/sortedProducts3@60::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01f9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01fe:  newobj     instance void Linq101Ordering01/'sortedProducts3@61-1'::.ctor()
    IL_0203:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0208:  newobj     instance void Linq101Ordering01/'sortedProducts3@62-2'::.ctor()
    IL_020d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::ThenByDescending<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,valuetype [mscorlib]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0212:  newobj     instance void Linq101Ordering01/'sortedProducts3@63-3'::.ctor()
    IL_0217:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_021c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0221:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0226:  dup
    IL_0227:  stsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts3@58
    IL_022c:  stloc.s    sortedProducts3
    IL_022e:  ret
  } // end of method $Linq101Ordering01::main@

} // end of class '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
