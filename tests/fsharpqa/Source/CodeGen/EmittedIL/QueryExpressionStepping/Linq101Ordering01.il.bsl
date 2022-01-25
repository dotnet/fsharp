
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
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
  .ver 6:0:0:0
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Ordering01
{
  // Offset: 0x00000000 Length: 0x000003AE
}
.mresource public FSharpOptimizationData.Linq101Ordering01
{
  // Offset: 0x000003B8 Length: 0x00000134
}
.module Linq101Ordering01.exe
// MVID: {61E07032-649A-6956-A745-03833270E061}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x069D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Ordering01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #1 input at line 10@11'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Ordering01/'Pipe #1 input at line 10@11'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #1 input at line 10@11'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init ([0] string V_0,
               [1] string w)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Ordering01.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 11,11 : 9,12 ''
      IL_0025:  nop
      .line 11,11 : 9,12 ''
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_words()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0043:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      .line 12,12 : 9,17 ''
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      string Linq101Ordering01/'Pipe #1 input at line 10@11'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      .line 100001,100001 : 0,0 ''
      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      string Linq101Ordering01/'Pipe #1 input at line 10@11'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method 'Pipe #1 input at line 10@11'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      .line 100001,100001 : 0,0 ''
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Ordering01/'Pipe #1 input at line 10@11'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0066:  castclass  [mscorlib]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      .line 100001,100001 : 0,0 ''
      IL_0076:  ldloc.0
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  brfalse.s  IL_007e

      .line 100001,100001 : 0,0 ''
      IL_007c:  ldloc.0
      IL_007d:  throw

      .line 100001,100001 : 0,0 ''
      IL_007e:  ret
    } // end of method 'Pipe #1 input at line 10@11'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method 'Pipe #1 input at line 10@11'::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Ordering01/'Pipe #1 input at line 10@11'::current
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 10@11'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Ordering01/'Pipe #1 input at line 10@11'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                                int32,
                                                                                                string)
      IL_0008:  ret
    } // end of method 'Pipe #1 input at line 10@11'::GetFreshEnumerator

  } // end of class 'Pipe #1 input at line 10@11'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 10@12-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #1 input at line 10@12-1' @_instance
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
    } // end of method 'Pipe #1 input at line 10@12-1'::.ctor

    .method public strict virtual instance string 
            Invoke(string w) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 12,12 : 16,17 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #1 input at line 10@12-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #1 input at line 10@12-1'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #1 input at line 10@12-1' Linq101Ordering01/'Pipe #1 input at line 10@12-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 10@12-1'::.cctor

  } // end of class 'Pipe #1 input at line 10@12-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #2 input at line 17@18'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Ordering01/'Pipe #2 input at line 17@18'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #2 input at line 17@18'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init ([0] string V_0,
               [1] string w)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 18,18 : 9,12 ''
      IL_0025:  nop
      .line 18,18 : 9,12 ''
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_words()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0043:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      .line 19,19 : 9,26 ''
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      string Linq101Ordering01/'Pipe #2 input at line 17@18'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      .line 100001,100001 : 0,0 ''
      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      string Linq101Ordering01/'Pipe #2 input at line 17@18'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method 'Pipe #2 input at line 17@18'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      .line 100001,100001 : 0,0 ''
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Ordering01/'Pipe #2 input at line 17@18'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0066:  castclass  [mscorlib]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      .line 100001,100001 : 0,0 ''
      IL_0076:  ldloc.0
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  brfalse.s  IL_007e

      .line 100001,100001 : 0,0 ''
      IL_007c:  ldloc.0
      IL_007d:  throw

      .line 100001,100001 : 0,0 ''
      IL_007e:  ret
    } // end of method 'Pipe #2 input at line 17@18'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method 'Pipe #2 input at line 17@18'::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Ordering01/'Pipe #2 input at line 17@18'::current
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 17@18'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Ordering01/'Pipe #2 input at line 17@18'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                                int32,
                                                                                                string)
      IL_0008:  ret
    } // end of method 'Pipe #2 input at line 17@18'::GetFreshEnumerator

  } // end of class 'Pipe #2 input at line 17@18'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 17@19-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #2 input at line 17@19-1' @_instance
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
    } // end of method 'Pipe #2 input at line 17@19-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string w) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      .line 19,19 : 17,25 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 17@19-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #2 input at line 17@19-1'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #2 input at line 17@19-1' Linq101Ordering01/'Pipe #2 input at line 17@19-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 17@19-1'::.cctor

  } // end of class 'Pipe #2 input at line 17@19-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 25@26'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Ordering01/'Pipe #3 input at line 25@26'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #3 input at line 25@26'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 27,27 : 9,29 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Ordering01/'Pipe #3 input at line 25@26'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #3 input at line 25@26'::Invoke

  } // end of class 'Pipe #3 input at line 25@26'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 25@27-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #3 input at line 25@27-1' @_instance
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
    } // end of method 'Pipe #3 input at line 25@27-1'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 27,27 : 16,29 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0008:  ret
    } // end of method 'Pipe #3 input at line 25@27-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #3 input at line 25@27-1'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #3 input at line 25@27-1' Linq101Ordering01/'Pipe #3 input at line 25@27-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 25@27-1'::.cctor

  } // end of class 'Pipe #3 input at line 25@27-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 25@28-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #3 input at line 25@28-2' @_instance
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
    } // end of method 'Pipe #3 input at line 25@28-2'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 28,28 : 16,17 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #3 input at line 25@28-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #3 input at line 25@28-2'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #3 input at line 25@28-2' Linq101Ordering01/'Pipe #3 input at line 25@28-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 25@28-2'::.cctor

  } // end of class 'Pipe #3 input at line 25@28-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #4 input at line 43@44'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [Utils]Utils/Product Linq101Ordering01/'Pipe #4 input at line 43@44'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #4 input at line 43@44'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product V_0,
               [1] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 44,44 : 9,12 ''
      IL_0025:  nop
      .line 44,44 : 9,12 ''
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0043:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      .line 45,45 : 9,40 ''
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      class [Utils]Utils/Product Linq101Ordering01/'Pipe #4 input at line 43@44'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      .line 100001,100001 : 0,0 ''
      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Ordering01/'Pipe #4 input at line 43@44'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method 'Pipe #4 input at line 43@44'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      .line 100001,100001 : 0,0 ''
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product Linq101Ordering01/'Pipe #4 input at line 43@44'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0066:  castclass  [mscorlib]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      .line 100001,100001 : 0,0 ''
      IL_0076:  ldloc.0
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  brfalse.s  IL_007e

      .line 100001,100001 : 0,0 ''
      IL_007c:  ldloc.0
      IL_007d:  throw

      .line 100001,100001 : 0,0 ''
      IL_007e:  ret
    } // end of method 'Pipe #4 input at line 43@44'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method 'Pipe #4 input at line 43@44'::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Ordering01/'Pipe #4 input at line 43@44'::current
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 43@44'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Ordering01/'Pipe #4 input at line 43@44'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                                int32,
                                                                                                class [Utils]Utils/Product)
      IL_0008:  ret
    } // end of method 'Pipe #4 input at line 43@44'::GetFreshEnumerator

  } // end of class 'Pipe #4 input at line 43@44'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 43@45-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #4 input at line 43@45-1' @_instance
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
    } // end of method 'Pipe #4 input at line 43@45-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 45,45 : 26,40 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0008:  ret
    } // end of method 'Pipe #4 input at line 43@45-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #4 input at line 43@45-1'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #4 input at line 43@45-1' Linq101Ordering01/'Pipe #4 input at line 43@45-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 43@45-1'::.cctor

  } // end of class 'Pipe #4 input at line 43@45-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #5 input at line 51@52'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Ordering01/'Pipe #5 input at line 51@52'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #5 input at line 51@52'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init ([0] string V_0,
               [1] string d)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      .line 100001,100001 : 0,0 ''
      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      .line 52,52 : 9,12 ''
      IL_0025:  nop
      .line 52,52 : 9,12 ''
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_digits()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0043:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0050:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      .line 53,53 : 9,24 ''
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      string Linq101Ordering01/'Pipe #5 input at line 51@52'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      .line 100001,100001 : 0,0 ''
      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      string Linq101Ordering01/'Pipe #5 input at line 51@52'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method 'Pipe #5 input at line 51@52'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       127 (0x7f)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      .line 100001,100001 : 0,0 ''
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        .line 100001,100001 : 0,0 ''
        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        .line 100001,100001 : 0,0 ''
        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        .line 100001,100001 : 0,0 ''
        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        .line 100001,100001 : 0,0 ''
        IL_0040:  nop
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        .line 100001,100001 : 0,0 ''
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Ordering01/'Pipe #5 input at line 51@52'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0066:  castclass  [mscorlib]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      .line 100001,100001 : 0,0 ''
      IL_0076:  ldloc.0
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  brfalse.s  IL_007e

      .line 100001,100001 : 0,0 ''
      IL_007c:  ldloc.0
      IL_007d:  throw

      .line 100001,100001 : 0,0 ''
      IL_007e:  ret
    } // end of method 'Pipe #5 input at line 51@52'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      .line 100001,100001 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      .line 100001,100001 : 0,0 ''
      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      .line 100001,100001 : 0,0 ''
      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method 'Pipe #5 input at line 51@52'::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Ordering01/'Pipe #5 input at line 51@52'::current
      IL_0006:  ret
    } // end of method 'Pipe #5 input at line 51@52'::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Ordering01/'Pipe #5 input at line 51@52'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                                int32,
                                                                                                string)
      IL_0008:  ret
    } // end of method 'Pipe #5 input at line 51@52'::GetFreshEnumerator

  } // end of class 'Pipe #5 input at line 51@52'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 51@53-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #5 input at line 51@53-1' @_instance
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
    } // end of method 'Pipe #5 input at line 51@53-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string d) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      .line 53,53 : 16,24 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0006:  ret
    } // end of method 'Pipe #5 input at line 51@53-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #5 input at line 51@53-1'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #5 input at line 51@53-1' Linq101Ordering01/'Pipe #5 input at line 51@53-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #5 input at line 51@53-1'::.cctor

  } // end of class 'Pipe #5 input at line 51@53-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 51@54-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #5 input at line 51@54-2' @_instance
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
    } // end of method 'Pipe #5 input at line 51@54-2'::.ctor

    .method public strict virtual instance string 
            Invoke(string d) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 54,54 : 16,17 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #5 input at line 51@54-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #5 input at line 51@54-2'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #5 input at line 51@54-2' Linq101Ordering01/'Pipe #5 input at line 51@54-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #5 input at line 51@54-2'::.cctor

  } // end of class 'Pipe #5 input at line 51@54-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@60'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Ordering01/'Pipe #6 input at line 59@60'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #6 input at line 59@60'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 61,61 : 9,26 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Ordering01/'Pipe #6 input at line 59@60'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #6 input at line 59@60'::Invoke

  } // end of class 'Pipe #6 input at line 59@60'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@61-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #6 input at line 59@61-1' @_instance
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
    } // end of method 'Pipe #6 input at line 59@61-1'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 61,61 : 16,26 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #6 input at line 59@61-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #6 input at line 59@61-1'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #6 input at line 59@61-1' Linq101Ordering01/'Pipe #6 input at line 59@61-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 59@61-1'::.cctor

  } // end of class 'Pipe #6 input at line 59@61-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@62-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [mscorlib]System.Decimal>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #6 input at line 59@62-2' @_instance
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
    } // end of method 'Pipe #6 input at line 59@62-2'::.ctor

    .method public strict virtual instance valuetype [mscorlib]System.Decimal 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 62,62 : 26,37 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } // end of method 'Pipe #6 input at line 59@62-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #6 input at line 59@62-2'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #6 input at line 59@62-2' Linq101Ordering01/'Pipe #6 input at line 59@62-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 59@62-2'::.cctor

  } // end of class 'Pipe #6 input at line 59@62-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@63-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Ordering01/'Pipe #6 input at line 59@63-3' @_instance
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
    } // end of method 'Pipe #6 input at line 59@63-3'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 63,63 : 16,17 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #6 input at line 59@63-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Ordering01/'Pipe #6 input at line 59@63-3'::.ctor()
      IL_0005:  stsfld     class Linq101Ordering01/'Pipe #6 input at line 59@63-3' Linq101Ordering01/'Pipe #6 input at line 59@63-3'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 59@63-3'::.cctor

  } // end of class 'Pipe #6 input at line 59@63-3'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_words() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::words@8
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
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::products@23
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
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::digits@49
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
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@8
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> sortedWords@9
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> sortedWords2@16
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@23
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [Utils]Utils/Product[] sortedProducts@24
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [Utils]Utils/Product[] sortedProducts2@42
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits@49
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
    // Code size       570 (0x23a)
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
             [9] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 'Pipe #1 input at line 10',
             [10] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             [11] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 'Pipe #2 input at line 17',
             [12] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             [13] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 'Pipe #3 input at line 25',
             [14] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_14,
             [15] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 'Pipe #4 input at line 43',
             [16] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_16,
             [17] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 'Pipe #5 input at line 51',
             [18] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_18,
             [19] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 'Pipe #6 input at line 59',
             [20] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_20)
    .line 8,8 : 1,45 ''
    IL_0000:  ldstr      "cherry"
    IL_0005:  ldstr      "apple"
    IL_000a:  ldstr      "blueberry"
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0023:  dup
    IL_0024:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::words@8
    IL_0029:  stloc.0
    .line 9,13 : 1,20 ''
    IL_002a:  nop
    .line 10,10 : 5,10 ''
    IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0030:  stloc.s    V_10
    .line 10,10 : 5,10 ''
    IL_0032:  ldloc.s    V_10
    IL_0034:  ldnull
    IL_0035:  ldc.i4.0
    IL_0036:  ldnull
    IL_0037:  newobj     instance void Linq101Ordering01/'Pipe #1 input at line 10@11'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                              int32,
                                                                                              string)
    IL_003c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0041:  ldsfld     class Linq101Ordering01/'Pipe #1 input at line 10@12-1' Linq101Ordering01/'Pipe #1 input at line 10@12-1'::@_instance
    IL_0046:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<string,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_004b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0050:  stloc.s    'Pipe #1 input at line 10'
    .line 13,13 : 10,20 ''
    IL_0052:  ldloc.s    'Pipe #1 input at line 10'
    IL_0054:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0059:  dup
    IL_005a:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedWords@9
    IL_005f:  stloc.1
    .line 16,20 : 1,20 ''
    IL_0060:  nop
    .line 17,17 : 5,10 ''
    IL_0061:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0066:  stloc.s    V_12
    .line 17,17 : 5,10 ''
    IL_0068:  ldloc.s    V_12
    IL_006a:  ldnull
    IL_006b:  ldc.i4.0
    IL_006c:  ldnull
    IL_006d:  newobj     instance void Linq101Ordering01/'Pipe #2 input at line 17@18'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                              int32,
                                                                                              string)
    IL_0072:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0077:  ldsfld     class Linq101Ordering01/'Pipe #2 input at line 17@19-1' Linq101Ordering01/'Pipe #2 input at line 17@19-1'::@_instance
    IL_007c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0081:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0086:  stloc.s    'Pipe #2 input at line 17'
    .line 20,20 : 10,20 ''
    IL_0088:  ldloc.s    'Pipe #2 input at line 17'
    IL_008a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_008f:  dup
    IL_0090:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedWords2@16
    IL_0095:  stloc.2
    .line 23,23 : 1,32 ''
    IL_0096:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_009b:  dup
    IL_009c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::products@23
    IL_00a1:  stloc.3
    .line 24,29 : 1,21 ''
    IL_00a2:  nop
    .line 25,25 : 5,10 ''
    IL_00a3:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a8:  stloc.s    V_14
    .line 25,25 : 5,10 ''
    IL_00aa:  ldloc.s    V_14
    IL_00ac:  ldloc.s    V_14
    .line 26,26 : 9,12 ''
    IL_00ae:  ldloc.s    V_14
    IL_00b0:  ldloc.s    V_14
    IL_00b2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
    IL_00b7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00bc:  ldloc.s    V_14
    IL_00be:  newobj     instance void Linq101Ordering01/'Pipe #3 input at line 25@26'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00c3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00c8:  ldsfld     class Linq101Ordering01/'Pipe #3 input at line 25@27-1' Linq101Ordering01/'Pipe #3 input at line 25@27-1'::@_instance
    IL_00cd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00d2:  ldsfld     class Linq101Ordering01/'Pipe #3 input at line 25@28-2' Linq101Ordering01/'Pipe #3 input at line 25@28-2'::@_instance
    IL_00d7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00dc:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00e1:  stloc.s    'Pipe #3 input at line 25'
    .line 29,29 : 10,21 ''
    IL_00e3:  ldloc.s    'Pipe #3 input at line 25'
    IL_00e5:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00ea:  dup
    IL_00eb:  stsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts@24
    IL_00f0:  stloc.s    sortedProducts
    .line 42,46 : 1,21 ''
    IL_00f2:  nop
    .line 43,43 : 5,10 ''
    IL_00f3:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00f8:  stloc.s    V_16
    .line 43,43 : 5,10 ''
    IL_00fa:  ldloc.s    V_16
    IL_00fc:  ldnull
    IL_00fd:  ldc.i4.0
    IL_00fe:  ldnull
    IL_00ff:  newobj     instance void Linq101Ordering01/'Pipe #4 input at line 43@44'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                              int32,
                                                                                              class [Utils]Utils/Product)
    IL_0104:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0109:  ldsfld     class Linq101Ordering01/'Pipe #4 input at line 43@45-1' Linq101Ordering01/'Pipe #4 input at line 43@45-1'::@_instance
    IL_010e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortByDescending<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0113:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0118:  stloc.s    'Pipe #4 input at line 43'
    .line 46,46 : 10,21 ''
    IL_011a:  ldloc.s    'Pipe #4 input at line 43'
    IL_011c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0121:  dup
    IL_0122:  stsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts2@42
    IL_0127:  stloc.s    sortedProducts2
    .line 49,49 : 1,96 ''
    IL_0129:  ldstr      "zero"
    IL_012e:  ldstr      "one"
    IL_0133:  ldstr      "two"
    IL_0138:  ldstr      "three"
    IL_013d:  ldstr      "four"
    IL_0142:  ldstr      "five"
    IL_0147:  ldstr      "six"
    IL_014c:  ldstr      "seven"
    IL_0151:  ldstr      "eight"
    IL_0156:  ldstr      "nine"
    IL_015b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0160:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0165:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_016a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_016f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0174:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0179:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_017e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0183:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0188:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_018d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0192:  dup
    IL_0193:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::digits@49
    IL_0198:  stloc.s    digits
    .line 50,55 : 1,20 ''
    IL_019a:  nop
    .line 51,51 : 5,10 ''
    IL_019b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01a0:  stloc.s    V_18
    .line 51,51 : 5,10 ''
    IL_01a2:  ldloc.s    V_18
    IL_01a4:  ldloc.s    V_18
    IL_01a6:  ldnull
    IL_01a7:  ldc.i4.0
    IL_01a8:  ldnull
    IL_01a9:  newobj     instance void Linq101Ordering01/'Pipe #5 input at line 51@52'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                              int32,
                                                                                              string)
    IL_01ae:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_01b3:  ldsfld     class Linq101Ordering01/'Pipe #5 input at line 51@53-1' Linq101Ordering01/'Pipe #5 input at line 51@53-1'::@_instance
    IL_01b8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01bd:  ldsfld     class Linq101Ordering01/'Pipe #5 input at line 51@54-2' Linq101Ordering01/'Pipe #5 input at line 51@54-2'::@_instance
    IL_01c2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::ThenBy<string,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01c7:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01cc:  stloc.s    'Pipe #5 input at line 51'
    .line 55,55 : 10,20 ''
    IL_01ce:  ldloc.s    'Pipe #5 input at line 51'
    IL_01d0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01d5:  dup
    IL_01d6:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedDigits@50
    IL_01db:  stloc.s    sortedDigits
    .line 58,64 : 1,21 ''
    IL_01dd:  nop
    .line 59,59 : 5,10 ''
    IL_01de:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01e3:  stloc.s    V_20
    .line 59,59 : 5,10 ''
    IL_01e5:  ldloc.s    V_20
    IL_01e7:  ldloc.s    V_20
    IL_01e9:  ldloc.s    V_20
    .line 60,60 : 9,12 ''
    IL_01eb:  ldloc.s    V_20
    IL_01ed:  ldloc.s    V_20
    IL_01ef:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
    IL_01f4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01f9:  ldloc.s    V_20
    IL_01fb:  newobj     instance void Linq101Ordering01/'Pipe #6 input at line 59@60'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0200:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0205:  ldsfld     class Linq101Ordering01/'Pipe #6 input at line 59@61-1' Linq101Ordering01/'Pipe #6 input at line 59@61-1'::@_instance
    IL_020a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_020f:  ldsfld     class Linq101Ordering01/'Pipe #6 input at line 59@62-2' Linq101Ordering01/'Pipe #6 input at line 59@62-2'::@_instance
    IL_0214:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::ThenByDescending<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,valuetype [mscorlib]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0219:  ldsfld     class Linq101Ordering01/'Pipe #6 input at line 59@63-3' Linq101Ordering01/'Pipe #6 input at line 59@63-3'::@_instance
    IL_021e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0223:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0228:  stloc.s    'Pipe #6 input at line 59'
    .line 64,64 : 10,21 ''
    IL_022a:  ldloc.s    'Pipe #6 input at line 59'
    IL_022c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0231:  dup
    IL_0232:  stsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts3@58
    IL_0237:  stloc.s    sortedProducts3
    IL_0239:  ret
  } // end of method $Linq101Ordering01::main@

} // end of class '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
