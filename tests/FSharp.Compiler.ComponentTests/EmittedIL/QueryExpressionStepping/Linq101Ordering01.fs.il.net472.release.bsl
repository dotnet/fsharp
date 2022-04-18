
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Ordering01
{
  // Offset: 0x00000000 Length: 0x000003F0
  // WARNING: managed resource file FSharpSignatureData.Linq101Ordering01 created
}
.mresource public FSharpOptimizationData.Linq101Ordering01
{
  // Offset: 0x000003F8 Length: 0x00000134
  // WARNING: managed resource file FSharpOptimizationData.Linq101Ordering01 created
}
.module Linq101Ordering01.exe
// MVID: {624FE95B-649A-6956-A745-03835BE94F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00670000


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
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_words()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0045:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string Linq101Ordering01/'Pipe #1 input at line 10@11'::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0063:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Ordering01/'Pipe #1 input at line 10@11'::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method 'Pipe #1 input at line 10@11'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [mscorlib]System.Exception V_0,
               class [mscorlib]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
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

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #1 input at line 10@11'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
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

      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } // end of method 'Pipe #1 input at line 10@11'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #1 input at line 10@11'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

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
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_words()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0045:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string Linq101Ordering01/'Pipe #2 input at line 17@18'::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0063:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Ordering01/'Pipe #2 input at line 17@18'::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method 'Pipe #2 input at line 17@18'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [mscorlib]System.Exception V_0,
               class [mscorlib]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
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

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #2 input at line 17@18'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
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

      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } // end of method 'Pipe #2 input at line 17@18'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #2 input at line 17@18'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

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
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
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
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0045:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      class [Utils]Utils/Product Linq101Ordering01/'Pipe #4 input at line 43@44'::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0063:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      class [Utils]Utils/Product Linq101Ordering01/'Pipe #4 input at line 43@44'::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method 'Pipe #4 input at line 43@44'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [mscorlib]System.Exception V_0,
               class [mscorlib]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
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

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Ordering01/'Pipe #4 input at line 43@44'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
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

      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } // end of method 'Pipe #4 input at line 43@44'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #4 input at line 43@44'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

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
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Ordering01::get_digits()
      IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0045:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string Linq101Ordering01/'Pipe #5 input at line 51@52'::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0063:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Ordering01/'Pipe #5 input at line 51@52'::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method 'Pipe #5 input at line 51@52'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [mscorlib]System.Exception V_0,
               class [mscorlib]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
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

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Ordering01/'Pipe #5 input at line 51@52'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
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

      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } // end of method 'Pipe #5 input at line 51@52'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Ordering01/'Pipe #5 input at line 51@52'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

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
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
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
    // Code size       564 (0x234)
    .maxstack  13
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_2,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> V_3,
             class [Utils]Utils/Product[] V_4,
             class [Utils]Utils/Product[] V_5,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_6,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_7,
             class [Utils]Utils/Product[] V_8,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_9,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_11,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_13,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_14,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_15,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_16,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_17,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_18,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_19,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_20)
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
    IL_002a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_002f:  stloc.s    V_10
    IL_0031:  ldloc.s    V_10
    IL_0033:  ldnull
    IL_0034:  ldc.i4.0
    IL_0035:  ldnull
    IL_0036:  newobj     instance void Linq101Ordering01/'Pipe #1 input at line 10@11'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                              int32,
                                                                                              string)
    IL_003b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0040:  ldsfld     class Linq101Ordering01/'Pipe #1 input at line 10@12-1' Linq101Ordering01/'Pipe #1 input at line 10@12-1'::@_instance
    IL_0045:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<string,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_004a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_004f:  stloc.s    V_9
    IL_0051:  ldloc.s    V_9
    IL_0053:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0058:  dup
    IL_0059:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedWords@9
    IL_005e:  stloc.1
    IL_005f:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0064:  stloc.s    V_12
    IL_0066:  ldloc.s    V_12
    IL_0068:  ldnull
    IL_0069:  ldc.i4.0
    IL_006a:  ldnull
    IL_006b:  newobj     instance void Linq101Ordering01/'Pipe #2 input at line 17@18'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                              int32,
                                                                                              string)
    IL_0070:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0075:  ldsfld     class Linq101Ordering01/'Pipe #2 input at line 17@19-1' Linq101Ordering01/'Pipe #2 input at line 17@19-1'::@_instance
    IL_007a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_007f:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0084:  stloc.s    V_11
    IL_0086:  ldloc.s    V_11
    IL_0088:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_008d:  dup
    IL_008e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedWords2@16
    IL_0093:  stloc.2
    IL_0094:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_0099:  dup
    IL_009a:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::products@23
    IL_009f:  stloc.3
    IL_00a0:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a5:  stloc.s    V_14
    IL_00a7:  ldloc.s    V_14
    IL_00a9:  ldloc.s    V_14
    IL_00ab:  ldloc.s    V_14
    IL_00ad:  ldloc.s    V_14
    IL_00af:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
    IL_00b4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00b9:  ldloc.s    V_14
    IL_00bb:  newobj     instance void Linq101Ordering01/'Pipe #3 input at line 25@26'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00c0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00c5:  ldsfld     class Linq101Ordering01/'Pipe #3 input at line 25@27-1' Linq101Ordering01/'Pipe #3 input at line 25@27-1'::@_instance
    IL_00ca:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00cf:  ldsfld     class Linq101Ordering01/'Pipe #3 input at line 25@28-2' Linq101Ordering01/'Pipe #3 input at line 25@28-2'::@_instance
    IL_00d4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00d9:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00de:  stloc.s    V_13
    IL_00e0:  ldloc.s    V_13
    IL_00e2:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00e7:  dup
    IL_00e8:  stsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts@24
    IL_00ed:  stloc.s    V_4
    IL_00ef:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00f4:  stloc.s    V_16
    IL_00f6:  ldloc.s    V_16
    IL_00f8:  ldnull
    IL_00f9:  ldc.i4.0
    IL_00fa:  ldnull
    IL_00fb:  newobj     instance void Linq101Ordering01/'Pipe #4 input at line 43@44'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                              int32,
                                                                                              class [Utils]Utils/Product)
    IL_0100:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0105:  ldsfld     class Linq101Ordering01/'Pipe #4 input at line 43@45-1' Linq101Ordering01/'Pipe #4 input at line 43@45-1'::@_instance
    IL_010a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortByDescending<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_010f:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0114:  stloc.s    V_15
    IL_0116:  ldloc.s    V_15
    IL_0118:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_011d:  dup
    IL_011e:  stsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts2@42
    IL_0123:  stloc.s    V_5
    IL_0125:  ldstr      "zero"
    IL_012a:  ldstr      "one"
    IL_012f:  ldstr      "two"
    IL_0134:  ldstr      "three"
    IL_0139:  ldstr      "four"
    IL_013e:  ldstr      "five"
    IL_0143:  ldstr      "six"
    IL_0148:  ldstr      "seven"
    IL_014d:  ldstr      "eight"
    IL_0152:  ldstr      "nine"
    IL_0157:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_015c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0161:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0166:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_016b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0170:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0175:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_017a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_017f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0184:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0189:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_018e:  dup
    IL_018f:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::digits@49
    IL_0194:  stloc.s    V_6
    IL_0196:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_019b:  stloc.s    V_18
    IL_019d:  ldloc.s    V_18
    IL_019f:  ldloc.s    V_18
    IL_01a1:  ldnull
    IL_01a2:  ldc.i4.0
    IL_01a3:  ldnull
    IL_01a4:  newobj     instance void Linq101Ordering01/'Pipe #5 input at line 51@52'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                              int32,
                                                                                              string)
    IL_01a9:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_01ae:  ldsfld     class Linq101Ordering01/'Pipe #5 input at line 51@53-1' Linq101Ordering01/'Pipe #5 input at line 51@53-1'::@_instance
    IL_01b3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<string,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01b8:  ldsfld     class Linq101Ordering01/'Pipe #5 input at line 51@54-2' Linq101Ordering01/'Pipe #5 input at line 51@54-2'::@_instance
    IL_01bd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::ThenBy<string,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01c2:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01c7:  stloc.s    V_17
    IL_01c9:  ldloc.s    V_17
    IL_01cb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01d0:  dup
    IL_01d1:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedDigits@50
    IL_01d6:  stloc.s    V_7
    IL_01d8:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01dd:  stloc.s    V_20
    IL_01df:  ldloc.s    V_20
    IL_01e1:  ldloc.s    V_20
    IL_01e3:  ldloc.s    V_20
    IL_01e5:  ldloc.s    V_20
    IL_01e7:  ldloc.s    V_20
    IL_01e9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Ordering01::get_products()
    IL_01ee:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01f3:  ldloc.s    V_20
    IL_01f5:  newobj     instance void Linq101Ordering01/'Pipe #6 input at line 59@60'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01fa:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01ff:  ldsfld     class Linq101Ordering01/'Pipe #6 input at line 59@61-1' Linq101Ordering01/'Pipe #6 input at line 59@61-1'::@_instance
    IL_0204:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::SortBy<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0209:  ldsfld     class Linq101Ordering01/'Pipe #6 input at line 59@62-2' Linq101Ordering01/'Pipe #6 input at line 59@62-2'::@_instance
    IL_020e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::ThenByDescending<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,valuetype [mscorlib]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0213:  ldsfld     class Linq101Ordering01/'Pipe #6 input at line 59@63-3' Linq101Ordering01/'Pipe #6 input at line 59@63-3'::@_instance
    IL_0218:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_021d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0222:  stloc.s    V_19
    IL_0224:  ldloc.s    V_19
    IL_0226:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_022b:  dup
    IL_022c:  stsfld     class [Utils]Utils/Product[] '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01::sortedProducts3@58
    IL_0231:  stloc.s    V_8
    IL_0233:  ret
  } // end of method $Linq101Ordering01::main@

} // end of class '<StartupCode$Linq101Ordering01>'.$Linq101Ordering01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\QueryExpressionStepping\Linq101Ordering01_fs\Linq101Ordering01.res
