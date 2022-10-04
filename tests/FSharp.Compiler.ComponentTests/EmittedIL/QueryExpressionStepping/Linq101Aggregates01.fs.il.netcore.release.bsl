
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 7:0:0:0
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
.assembly extern System.Linq
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 7:0:0:0
}
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:1:0:0
}
.assembly Linq101Aggregates01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Aggregates01
{
  // Offset: 0x00000000 Length: 0x0000061B
  // WARNING: managed resource file FSharpSignatureData.Linq101Aggregates01 created
}
.mresource public FSharpOptimizationData.Linq101Aggregates01
{
  // Offset: 0x00000620 Length: 0x00000211
  // WARNING: managed resource file FSharpOptimizationData.Linq101Aggregates01 created
}
.module Linq101Aggregates01.exe
// MVID: {630166A9-C2E4-58B4-A745-0383A9660163}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000001EC1FEC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Aggregates01
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #1 input at line 11@12'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #1 input at line 11@12'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
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
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_factorsOf300()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0045:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0063:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method 'Pipe #1 input at line 11@12'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method 'Pipe #1 input at line 11@12'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
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
    } // end of method 'Pipe #1 input at line 11@12'::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::current
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 11@12'::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101Aggregates01/'Pipe #1 input at line 11@12'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                  int32,
                                                                                                  int32)
      IL_0008:  ret
    } // end of method 'Pipe #1 input at line 11@12'::GetFreshEnumerator

  } // end of class 'Pipe #1 input at line 11@12'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname numSum@21
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method numSum@21::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/numSum@21::pc
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
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0045:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0063:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method numSum@21::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/numSum@21::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/numSum@21::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/numSum@21::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/numSum@21::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Aggregates01/numSum@21::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method numSum@21::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/numSum@21::pc
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
    } // end of method numSum@21::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0006:  ret
    } // end of method numSum@21::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101Aggregates01/numSum@21::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_0008:  ret
    } // end of method numSum@21::GetFreshEnumerator

  } // end of class numSum@21

  .class auto ansi serializable sealed nested assembly beforefieldinit 'numSum@22-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class Linq101Aggregates01/'numSum@22-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'numSum@22-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'numSum@22-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'numSum@22-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'numSum@22-1' Linq101Aggregates01/'numSum@22-1'::@_instance
      IL_000a:  ret
    } // end of method 'numSum@22-1'::.cctor

  } // end of class 'numSum@22-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname totalChars@30
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method totalChars@30::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/totalChars@30::pc
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
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0045:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0063:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method totalChars@30::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/totalChars@30::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/totalChars@30::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Aggregates01/totalChars@30::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method totalChars@30::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/totalChars@30::pc
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
    } // end of method totalChars@30::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Aggregates01/totalChars@30::current
      IL_0006:  ret
    } // end of method totalChars@30::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Aggregates01/totalChars@30::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
      IL_0008:  ret
    } // end of method totalChars@30::GetFreshEnumerator

  } // end of class totalChars@30

  .class auto ansi serializable sealed nested assembly beforefieldinit 'totalChars@31-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .field static assembly initonly class Linq101Aggregates01/'totalChars@31-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'totalChars@31-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string w) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
      IL_0006:  ret
    } // end of method 'totalChars@31-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'totalChars@31-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'totalChars@31-1' Linq101Aggregates01/'totalChars@31-1'::@_instance
      IL_000a:  ret
    } // end of method 'totalChars@31-1'::.cctor

  } // end of class 'totalChars@31-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@39'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #2 input at line 38@39'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #2 input at line 38@39'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #2 input at line 38@39'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #2 input at line 38@39'::Invoke

  } // end of class 'Pipe #2 input at line 38@39'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@40-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #2 input at line 38@40-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 38@40-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #2 input at line 38@40-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #2 input at line 38@40-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #2 input at line 38@40-1' Linq101Aggregates01/'Pipe #2 input at line 38@40-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 38@40-1'::.cctor

  } // end of class 'Pipe #2 input at line 38@40-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@40-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #2 input at line 38@40-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 38@40-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #2 input at line 38@40-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #2 input at line 38@40-2'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #2 input at line 38@40-2' Linq101Aggregates01/'Pipe #2 input at line 38@40-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 38@40-2'::.cctor

  } // end of class 'Pipe #2 input at line 38@40-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname sum@42
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/sum@42::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } // end of method sum@42::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/sum@42::g
      IL_002d:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0046:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0064:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method sum@42::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/sum@42::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/sum@42::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/sum@42::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/sum@42::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method sum@42::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/sum@42::pc
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
    } // end of method sum@42::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_0006:  ret
    } // end of method sum@42::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/sum@42::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void Linq101Aggregates01/sum@42::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_000e:  ret
    } // end of method sum@42::GetFreshEnumerator

  } // end of class sum@42

  .class auto ansi serializable sealed nested assembly beforefieldinit 'sum@43-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>
  {
    .field static assembly initonly class Linq101Aggregates01/'sum@43-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'sum@43-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0008:  ret
    } // end of method 'sum@43-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'sum@43-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'sum@43-1' Linq101Aggregates01/'sum@43-1'::@_instance
      IL_000a:  ret
    } // end of method 'sum@43-1'::.cctor

  } // end of class 'sum@43-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@40-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #2 input at line 38@40-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #2 input at line 38@40-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       136 (0x88)
      .maxstack  8
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               int32 V_1,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable> V_4,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> V_5,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_6,
               class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_7,
               int32 V_8,
               int32 V_9,
               class [System.Runtime]System.IDisposable V_10)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  stloc.3
      IL_000a:  ldloc.0
      IL_000b:  ldnull
      IL_000c:  ldc.i4.0
      IL_000d:  ldnull
      IL_000e:  newobj     instance void Linq101Aggregates01/sum@42::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0013:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0018:  stloc.s    V_4
      IL_001a:  ldsfld     class Linq101Aggregates01/'sum@43-1' Linq101Aggregates01/'sum@43-1'::@_instance
      IL_001f:  stloc.s    V_5
      IL_0021:  ldloc.s    V_4
      IL_0023:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
      IL_0028:  stloc.s    V_6
      IL_002a:  ldloc.s    V_6
      IL_002c:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0031:  stloc.s    V_7
      .try
      {
        IL_0033:  ldc.i4.0
        IL_0034:  stloc.s    V_9
        IL_0036:  br.s       IL_004b

        IL_0038:  ldloc.s    V_9
        IL_003a:  ldloc.s    V_5
        IL_003c:  ldloc.s    V_7
        IL_003e:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_0043:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::Invoke(!0)
        IL_0048:  add.ovf
        IL_0049:  stloc.s    V_9
        IL_004b:  ldloc.s    V_7
        IL_004d:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
        IL_0052:  brtrue.s   IL_0038

        IL_0054:  ldloc.s    V_9
        IL_0056:  stloc.s    V_8
        IL_0058:  leave.s    IL_0070

      }  // end .try
      finally
      {
        IL_005a:  ldloc.s    V_7
        IL_005c:  isinst     [System.Runtime]System.IDisposable
        IL_0061:  stloc.s    V_10
        IL_0063:  ldloc.s    V_10
        IL_0065:  brfalse.s  IL_006f

        IL_0067:  ldloc.s    V_10
        IL_0069:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
        IL_006e:  endfinally
        IL_006f:  endfinally
      }  // end handler
      IL_0070:  ldloc.s    V_8
      IL_0072:  stloc.1
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #2 input at line 38@40-3'::builder@
      IL_0079:  ldloc.0
      IL_007a:  ldloc.1
      IL_007b:  newobj     instance void class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::.ctor(!0,
                                                                                                                                                                          !1)
      IL_0080:  tail.
      IL_0082:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(!!0)
      IL_0087:  ret
    } // end of method 'Pipe #2 input at line 38@40-3'::Invoke

  } // end of class 'Pipe #2 input at line 38@40-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@45-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [System.Runtime]System.Tuple`2<string,int32>>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #2 input at line 38@45-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [System.Runtime]System.Tuple`2<string,int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 38@45-4'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<string,int32> 
            Invoke(class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32> tupledArg) cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               int32 V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [System.Runtime]System.Tuple`2<string,int32>::.ctor(!0,
                                                                                                   !1)
      IL_001a:  ret
    } // end of method 'Pipe #2 input at line 38@45-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #2 input at line 38@45-4'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #2 input at line 38@45-4' Linq101Aggregates01/'Pipe #2 input at line 38@45-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 38@45-4'::.cctor

  } // end of class 'Pipe #2 input at line 38@45-4'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname minNum@49
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method minNum@49::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/minNum@49::pc
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
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0045:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0063:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method minNum@49::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/minNum@49::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/minNum@49::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/minNum@49::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/minNum@49::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Aggregates01/minNum@49::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method minNum@49::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/minNum@49::pc
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
    } // end of method minNum@49::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0006:  ret
    } // end of method minNum@49::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101Aggregates01/minNum@49::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_0008:  ret
    } // end of method minNum@49::GetFreshEnumerator

  } // end of class minNum@49

  .class auto ansi serializable sealed nested assembly beforefieldinit 'minNum@49-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class Linq101Aggregates01/'minNum@49-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'minNum@49-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'minNum@49-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'minNum@49-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'minNum@49-1' Linq101Aggregates01/'minNum@49-1'::@_instance
      IL_000a:  ret
    } // end of method 'minNum@49-1'::.cctor

  } // end of class 'minNum@49-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname shortestWord@52
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method shortestWord@52::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/shortestWord@52::pc
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
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0045:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0063:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method shortestWord@52::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/shortestWord@52::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/shortestWord@52::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Aggregates01/shortestWord@52::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method shortestWord@52::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/shortestWord@52::pc
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
    } // end of method shortestWord@52::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0006:  ret
    } // end of method shortestWord@52::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Aggregates01/shortestWord@52::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                    int32,
                                                                                    string)
      IL_0008:  ret
    } // end of method shortestWord@52::GetFreshEnumerator

  } // end of class shortestWord@52

  .class auto ansi serializable sealed nested assembly beforefieldinit 'shortestWord@52-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .field static assembly initonly class Linq101Aggregates01/'shortestWord@52-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'shortestWord@52-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string w) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
      IL_0006:  ret
    } // end of method 'shortestWord@52-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'shortestWord@52-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'shortestWord@52-1' Linq101Aggregates01/'shortestWord@52-1'::@_instance
      IL_000a:  ret
    } // end of method 'shortestWord@52-1'::.cctor

  } // end of class 'shortestWord@52-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@57'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #3 input at line 56@57'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #3 input at line 56@57'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #3 input at line 56@57'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #3 input at line 56@57'::Invoke

  } // end of class 'Pipe #3 input at line 56@57'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@58-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #3 input at line 56@58-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 56@58-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #3 input at line 56@58-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #3 input at line 56@58-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #3 input at line 56@58-1' Linq101Aggregates01/'Pipe #3 input at line 56@58-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 56@58-1'::.cctor

  } // end of class 'Pipe #3 input at line 56@58-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@58-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #3 input at line 56@58-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 56@58-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #3 input at line 56@58-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #3 input at line 56@58-2'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #3 input at line 56@58-2' Linq101Aggregates01/'Pipe #3 input at line 56@58-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 56@58-2'::.cctor

  } // end of class 'Pipe #3 input at line 56@58-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname min@59
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/min@59::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } // end of method min@59::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/min@59::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/min@59::g
      IL_002d:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0046:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0064:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method min@59::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/min@59::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/min@59::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/min@59::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/min@59::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method min@59::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/min@59::pc
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
    } // end of method min@59::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_0006:  ret
    } // end of method min@59::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/min@59::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void Linq101Aggregates01/min@59::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_000e:  ret
    } // end of method min@59::GetFreshEnumerator

  } // end of class min@59

  .class auto ansi serializable sealed nested assembly beforefieldinit 'min@59-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>
  {
    .field static assembly initonly class Linq101Aggregates01/'min@59-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>::.ctor()
      IL_0006:  ret
    } // end of method 'min@59-1'::.ctor

    .method public strict virtual instance valuetype [System.Runtime]System.Decimal 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } // end of method 'min@59-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'min@59-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'min@59-1' Linq101Aggregates01/'min@59-1'::@_instance
      IL_000a:  ret
    } // end of method 'min@59-1'::.cctor

  } // end of class 'min@59-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@58-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #3 input at line 56@58-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #3 input at line 56@58-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       53 (0x35)
      .maxstack  9
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  ldloc.0
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/min@59::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0010:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0015:  ldsfld     class Linq101Aggregates01/'min@59-1' Linq101Aggregates01/'min@59-1'::@_instance
      IL_001a:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,valuetype [System.Runtime]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_001f:  stloc.1
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #3 input at line 56@58-3'::builder@
      IL_0026:  ldloc.0
      IL_0027:  ldloc.1
      IL_0028:  newobj     instance void class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                             !1)
      IL_002d:  tail.
      IL_002f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(!!0)
      IL_0034:  ret
    } // end of method 'Pipe #3 input at line 56@58-3'::Invoke

  } // end of class 'Pipe #3 input at line 56@58-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@60-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #3 input at line 56@60-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 56@60-4'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal> 
            Invoke(class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal> tupledArg) cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                      !1)
      IL_001a:  ret
    } // end of method 'Pipe #3 input at line 56@60-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #3 input at line 56@60-4'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #3 input at line 56@60-4' Linq101Aggregates01/'Pipe #3 input at line 56@60-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 56@60-4'::.cctor

  } // end of class 'Pipe #3 input at line 56@60-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@66'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #4 input at line 65@66'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #4 input at line 65@66'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #4 input at line 65@66'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #4 input at line 65@66'::Invoke

  } // end of class 'Pipe #4 input at line 65@66'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@67-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #4 input at line 65@67-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 65@67-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #4 input at line 65@67-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #4 input at line 65@67-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #4 input at line 65@67-1' Linq101Aggregates01/'Pipe #4 input at line 65@67-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 65@67-1'::.cctor

  } // end of class 'Pipe #4 input at line 65@67-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@67-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #4 input at line 65@67-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 65@67-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #4 input at line 65@67-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #4 input at line 65@67-2'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #4 input at line 65@67-2' Linq101Aggregates01/'Pipe #4 input at line 65@67-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 65@67-2'::.cctor

  } // end of class 'Pipe #4 input at line 65@67-2'

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname 'min@68-2'
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static valuetype [System.Runtime]System.Decimal 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } // end of method 'min@68-2'::Invoke

  } // end of class 'min@68-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname cheapestProducts@69
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } // end of method cheapestProducts@69::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::g
      IL_002d:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0046:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0064:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method cheapestProducts@69::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method cheapestProducts@69::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
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
    } // end of method cheapestProducts@69::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_0006:  ret
    } // end of method cheapestProducts@69::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void Linq101Aggregates01/cheapestProducts@69::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                        class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                        int32,
                                                                                        class [Utils]Utils/Product)
      IL_000e:  ret
    } // end of method cheapestProducts@69::GetFreshEnumerator

  } // end of class cheapestProducts@69

  .class auto ansi serializable sealed nested assembly beforefieldinit 'cheapestProducts@69-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>
  {
    .field public valuetype [System.Runtime]System.Decimal min
    .method assembly specialname rtspecialname 
            instance void  .ctor(valuetype [System.Runtime]System.Decimal min) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [System.Runtime]System.Decimal Linq101Aggregates01/'cheapestProducts@69-1'::min
      IL_000d:  ret
    } // end of method 'cheapestProducts@69-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0006:  ldarg.0
      IL_0007:  ldfld      valuetype [System.Runtime]System.Decimal Linq101Aggregates01/'cheapestProducts@69-1'::min
      IL_000c:  call       bool [netstandard]System.Decimal::op_Equality(valuetype [netstandard]System.Decimal,
                                                                         valuetype [netstandard]System.Decimal)
      IL_0011:  ret
    } // end of method 'cheapestProducts@69-1'::Invoke

  } // end of class 'cheapestProducts@69-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@67-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #4 input at line 65@67-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #4 input at line 65@67-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       81 (0x51)
      .maxstack  9
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  ldnull
      IL_0004:  ldftn      valuetype [System.Runtime]System.Decimal Linq101Aggregates01/'min@68-2'::Invoke(class [Utils]Utils/Product)
      IL_000a:  newobj     instance void class [System.Runtime]System.Func`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>::.ctor(object,
                                                                                                                                                         native int)
      IL_000f:  call       valuetype [System.Runtime]System.Decimal [System.Linq]System.Linq.Enumerable::Min<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                         class [System.Runtime]System.Func`2<!!0,valuetype [System.Runtime]System.Decimal>)
      IL_0014:  stloc.1
      IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_001a:  stloc.3
      IL_001b:  ldloc.3
      IL_001c:  ldloc.0
      IL_001d:  ldnull
      IL_001e:  ldc.i4.0
      IL_001f:  ldnull
      IL_0020:  newobj     instance void Linq101Aggregates01/cheapestProducts@69::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                        class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                        int32,
                                                                                        class [Utils]Utils/Product)
      IL_0025:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_002a:  ldloc.1
      IL_002b:  newobj     instance void Linq101Aggregates01/'cheapestProducts@69-1'::.ctor(valuetype [System.Runtime]System.Decimal)
      IL_0030:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
      IL_0035:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
      IL_003a:  stloc.2
      IL_003b:  ldarg.0
      IL_003c:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #4 input at line 65@67-3'::builder@
      IL_0041:  ldloc.0
      IL_0042:  ldloc.1
      IL_0043:  ldloc.2
      IL_0044:  newobj     instance void class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                                                                                        !2)
      IL_0049:  tail.
      IL_004b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_0050:  ret
    } // end of method 'Pipe #4 input at line 65@67-3'::Invoke

  } // end of class 'Pipe #4 input at line 65@67-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@70-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #4 input at line 65@70-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 65@70-4'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> tupledArg) cil managed
    {
      // Code size       34 (0x22)
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_2)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item3()
      IL_0014:  stloc.2
      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_001b:  ldloc.2
      IL_001c:  newobj     instance void class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                        !1)
      IL_0021:  ret
    } // end of method 'Pipe #4 input at line 65@70-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #4 input at line 65@70-4'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #4 input at line 65@70-4' Linq101Aggregates01/'Pipe #4 input at line 65@70-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 65@70-4'::.cctor

  } // end of class 'Pipe #4 input at line 65@70-4'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname maxNum@74
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method maxNum@74::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxNum@74::pc
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
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0045:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0063:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method maxNum@74::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxNum@74::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/maxNum@74::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Aggregates01/maxNum@74::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method maxNum@74::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxNum@74::pc
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
    } // end of method maxNum@74::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0006:  ret
    } // end of method maxNum@74::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101Aggregates01/maxNum@74::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_0008:  ret
    } // end of method maxNum@74::GetFreshEnumerator

  } // end of class maxNum@74

  .class auto ansi serializable sealed nested assembly beforefieldinit 'maxNum@74-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class Linq101Aggregates01/'maxNum@74-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'maxNum@74-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'maxNum@74-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'maxNum@74-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'maxNum@74-1' Linq101Aggregates01/'maxNum@74-1'::@_instance
      IL_000a:  ret
    } // end of method 'maxNum@74-1'::.cctor

  } // end of class 'maxNum@74-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname longestLength@77
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method longestLength@77::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/longestLength@77::pc
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
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0045:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0063:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } // end of method longestLength@77::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/longestLength@77::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/longestLength@77::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Aggregates01/longestLength@77::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method longestLength@77::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/longestLength@77::pc
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
    } // end of method longestLength@77::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Aggregates01/longestLength@77::current
      IL_0006:  ret
    } // end of method longestLength@77::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Aggregates01/longestLength@77::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                     int32,
                                                                                     string)
      IL_0008:  ret
    } // end of method longestLength@77::GetFreshEnumerator

  } // end of class longestLength@77

  .class auto ansi serializable sealed nested assembly beforefieldinit 'longestLength@77-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .field static assembly initonly class Linq101Aggregates01/'longestLength@77-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'longestLength@77-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(string w) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
      IL_0006:  ret
    } // end of method 'longestLength@77-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'longestLength@77-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'longestLength@77-1' Linq101Aggregates01/'longestLength@77-1'::@_instance
      IL_000a:  ret
    } // end of method 'longestLength@77-1'::.cctor

  } // end of class 'longestLength@77-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@82'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #5 input at line 81@82'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #5 input at line 81@82'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #5 input at line 81@82'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #5 input at line 81@82'::Invoke

  } // end of class 'Pipe #5 input at line 81@82'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@83-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #5 input at line 81@83-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #5 input at line 81@83-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #5 input at line 81@83-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #5 input at line 81@83-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #5 input at line 81@83-1' Linq101Aggregates01/'Pipe #5 input at line 81@83-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #5 input at line 81@83-1'::.cctor

  } // end of class 'Pipe #5 input at line 81@83-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@83-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #5 input at line 81@83-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #5 input at line 81@83-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #5 input at line 81@83-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #5 input at line 81@83-2'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #5 input at line 81@83-2' Linq101Aggregates01/'Pipe #5 input at line 81@83-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #5 input at line 81@83-2'::.cctor

  } // end of class 'Pipe #5 input at line 81@83-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname mostExpensivePrice@84
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } // end of method mostExpensivePrice@84::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::g
      IL_002d:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0046:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0064:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method mostExpensivePrice@84::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method mostExpensivePrice@84::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
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
    } // end of method mostExpensivePrice@84::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_0006:  ret
    } // end of method mostExpensivePrice@84::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void Linq101Aggregates01/mostExpensivePrice@84::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                          class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                          int32,
                                                                                          class [Utils]Utils/Product)
      IL_000e:  ret
    } // end of method mostExpensivePrice@84::GetFreshEnumerator

  } // end of class mostExpensivePrice@84

  .class auto ansi serializable sealed nested assembly beforefieldinit 'mostExpensivePrice@84-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>
  {
    .field static assembly initonly class Linq101Aggregates01/'mostExpensivePrice@84-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>::.ctor()
      IL_0006:  ret
    } // end of method 'mostExpensivePrice@84-1'::.ctor

    .method public strict virtual instance valuetype [System.Runtime]System.Decimal 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } // end of method 'mostExpensivePrice@84-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'mostExpensivePrice@84-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'mostExpensivePrice@84-1' Linq101Aggregates01/'mostExpensivePrice@84-1'::@_instance
      IL_000a:  ret
    } // end of method 'mostExpensivePrice@84-1'::.cctor

  } // end of class 'mostExpensivePrice@84-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@83-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #5 input at line 81@83-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #5 input at line 81@83-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       53 (0x35)
      .maxstack  9
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  ldloc.0
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/mostExpensivePrice@84::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                          class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                          int32,
                                                                                          class [Utils]Utils/Product)
      IL_0010:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0015:  ldsfld     class Linq101Aggregates01/'mostExpensivePrice@84-1' Linq101Aggregates01/'mostExpensivePrice@84-1'::@_instance
      IL_001a:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,valuetype [System.Runtime]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_001f:  stloc.1
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #5 input at line 81@83-3'::builder@
      IL_0026:  ldloc.0
      IL_0027:  ldloc.1
      IL_0028:  newobj     instance void class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                             !1)
      IL_002d:  tail.
      IL_002f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(!!0)
      IL_0034:  ret
    } // end of method 'Pipe #5 input at line 81@83-3'::Invoke

  } // end of class 'Pipe #5 input at line 81@83-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@85-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #5 input at line 81@85-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #5 input at line 81@85-4'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal> 
            Invoke(class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal> tupledArg) cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                      !1)
      IL_001a:  ret
    } // end of method 'Pipe #5 input at line 81@85-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #5 input at line 81@85-4'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #5 input at line 81@85-4' Linq101Aggregates01/'Pipe #5 input at line 81@85-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #5 input at line 81@85-4'::.cctor

  } // end of class 'Pipe #5 input at line 81@85-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@91'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #6 input at line 90@91'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #6 input at line 90@91'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #6 input at line 90@91'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #6 input at line 90@91'::Invoke

  } // end of class 'Pipe #6 input at line 90@91'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@92-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #6 input at line 90@92-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #6 input at line 90@92-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #6 input at line 90@92-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #6 input at line 90@92-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #6 input at line 90@92-1' Linq101Aggregates01/'Pipe #6 input at line 90@92-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 90@92-1'::.cctor

  } // end of class 'Pipe #6 input at line 90@92-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@92-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #6 input at line 90@92-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #6 input at line 90@92-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #6 input at line 90@92-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #6 input at line 90@92-2'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #6 input at line 90@92-2' Linq101Aggregates01/'Pipe #6 input at line 90@92-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 90@92-2'::.cctor

  } // end of class 'Pipe #6 input at line 90@92-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname maxPrice@93
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } // end of method maxPrice@93::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::g
      IL_002d:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0046:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0064:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method maxPrice@93::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxPrice@93::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/maxPrice@93::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method maxPrice@93::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/maxPrice@93::pc
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
    } // end of method maxPrice@93::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_0006:  ret
    } // end of method maxPrice@93::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void Linq101Aggregates01/maxPrice@93::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                class [Utils]Utils/Product)
      IL_000e:  ret
    } // end of method maxPrice@93::GetFreshEnumerator

  } // end of class maxPrice@93

  .class auto ansi serializable sealed nested assembly beforefieldinit 'maxPrice@93-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>
  {
    .field static assembly initonly class Linq101Aggregates01/'maxPrice@93-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>::.ctor()
      IL_0006:  ret
    } // end of method 'maxPrice@93-1'::.ctor

    .method public strict virtual instance valuetype [System.Runtime]System.Decimal 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } // end of method 'maxPrice@93-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'maxPrice@93-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'maxPrice@93-1' Linq101Aggregates01/'maxPrice@93-1'::@_instance
      IL_000a:  ret
    } // end of method 'maxPrice@93-1'::.cctor

  } // end of class 'maxPrice@93-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname mostExpensiveProducts@94
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } // end of method mostExpensiveProducts@94::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::g
      IL_002d:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0046:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0064:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method mostExpensiveProducts@94::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method mostExpensiveProducts@94::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
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
    } // end of method mostExpensiveProducts@94::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_0006:  ret
    } // end of method mostExpensiveProducts@94::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void Linq101Aggregates01/mostExpensiveProducts@94::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                             class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                             int32,
                                                                                             class [Utils]Utils/Product)
      IL_000e:  ret
    } // end of method mostExpensiveProducts@94::GetFreshEnumerator

  } // end of class mostExpensiveProducts@94

  .class auto ansi serializable sealed nested assembly beforefieldinit 'mostExpensiveProducts@94-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>
  {
    .field public valuetype [System.Runtime]System.Decimal maxPrice
    .method assembly specialname rtspecialname 
            instance void  .ctor(valuetype [System.Runtime]System.Decimal maxPrice) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [System.Runtime]System.Decimal Linq101Aggregates01/'mostExpensiveProducts@94-1'::maxPrice
      IL_000d:  ret
    } // end of method 'mostExpensiveProducts@94-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0006:  ldarg.0
      IL_0007:  ldfld      valuetype [System.Runtime]System.Decimal Linq101Aggregates01/'mostExpensiveProducts@94-1'::maxPrice
      IL_000c:  call       bool [netstandard]System.Decimal::op_Equality(valuetype [netstandard]System.Decimal,
                                                                         valuetype [netstandard]System.Decimal)
      IL_0011:  ret
    } // end of method 'mostExpensiveProducts@94-1'::Invoke

  } // end of class 'mostExpensiveProducts@94-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@92-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #6 input at line 90@92-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #6 input at line 90@92-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       92 (0x5c)
      .maxstack  9
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  ldloc.0
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void Linq101Aggregates01/maxPrice@93::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                class [Utils]Utils/Product)
      IL_0010:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0015:  ldsfld     class Linq101Aggregates01/'maxPrice@93-1' Linq101Aggregates01/'maxPrice@93-1'::@_instance
      IL_001a:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,valuetype [System.Runtime]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_001f:  stloc.1
      IL_0020:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0025:  stloc.3
      IL_0026:  ldloc.3
      IL_0027:  ldloc.0
      IL_0028:  ldnull
      IL_0029:  ldc.i4.0
      IL_002a:  ldnull
      IL_002b:  newobj     instance void Linq101Aggregates01/mostExpensiveProducts@94::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                             class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                             int32,
                                                                                             class [Utils]Utils/Product)
      IL_0030:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0035:  ldloc.1
      IL_0036:  newobj     instance void Linq101Aggregates01/'mostExpensiveProducts@94-1'::.ctor(valuetype [System.Runtime]System.Decimal)
      IL_003b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
      IL_0040:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
      IL_0045:  stloc.2
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #6 input at line 90@92-3'::builder@
      IL_004c:  ldloc.0
      IL_004d:  ldloc.1
      IL_004e:  ldloc.2
      IL_004f:  newobj     instance void class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                                                                                        !2)
      IL_0054:  tail.
      IL_0056:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_005b:  ret
    } // end of method 'Pipe #6 input at line 90@92-3'::Invoke

  } // end of class 'Pipe #6 input at line 90@92-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@95-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #6 input at line 90@95-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #6 input at line 90@95-4'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> tupledArg) cil managed
    {
      // Code size       34 (0x22)
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_2)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item3()
      IL_0014:  stloc.2
      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_001b:  ldloc.2
      IL_001c:  newobj     instance void class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                        !1)
      IL_0021:  ret
    } // end of method 'Pipe #6 input at line 90@95-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #6 input at line 90@95-4'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #6 input at line 90@95-4' Linq101Aggregates01/'Pipe #6 input at line 90@95-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 90@95-4'::.cctor

  } // end of class 'Pipe #6 input at line 90@95-4'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname averageNum@100
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<float64>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public float64 current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> 'enum',
                                 int32 pc,
                                 float64 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<float64>::.ctor()
      IL_001b:  ret
    } // end of method averageNum@100::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<float64>& next) cil managed
    {
      // Code size       156 (0x9c)
      .maxstack  6
      .locals init (float64 V_0,
               float64 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averageNum@100::pc
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
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> Linq101Aggregates01::get_numbers2()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0045:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0063:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.r8     0.0
      IL_0095:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_009a:  ldc.i4.0
      IL_009b:  ret
    } // end of method averageNum@100::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       132 (0x84)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_007e

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Aggregates01/averageNum@100::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.r8     0.0
        IL_0067:  stfld      float64 Linq101Aggregates01/averageNum@100::current
        IL_006c:  leave.s    IL_0078

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_006e:  castclass  [System.Runtime]System.Exception
        IL_0073:  stloc.1
        IL_0074:  ldloc.1
        IL_0075:  stloc.0
        IL_0076:  leave.s    IL_0078

      }  // end handler
      IL_0078:  nop
      IL_0079:  br         IL_0000

      IL_007e:  ldloc.0
      IL_007f:  brfalse.s  IL_0083

      IL_0081:  ldloc.0
      IL_0082:  throw

      IL_0083:  ret
    } // end of method averageNum@100::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averageNum@100::pc
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
    } // end of method averageNum@100::get_CheckClose

    .method public strict virtual instance float64 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_0006:  ret
    } // end of method averageNum@100::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.r8     0.0
      IL_000b:  newobj     instance void Linq101Aggregates01/averageNum@100::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                   int32,
                                                                                   float64)
      IL_0010:  ret
    } // end of method averageNum@100::GetFreshEnumerator

  } // end of class averageNum@100

  .class auto ansi serializable sealed nested assembly beforefieldinit 'averageNum@100-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>
  {
    .field static assembly initonly class Linq101Aggregates01/'averageNum@100-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::.ctor()
      IL_0006:  ret
    } // end of method 'averageNum@100-1'::.ctor

    .method public strict virtual instance float64 
            Invoke(float64 n) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'averageNum@100-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'averageNum@100-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'averageNum@100-1' Linq101Aggregates01/'averageNum@100-1'::@_instance
      IL_000a:  ret
    } // end of method 'averageNum@100-1'::.cctor

  } // end of class 'averageNum@100-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit averageLength@105
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,float64>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,float64>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/averageLength@105::builder@
      IL_000d:  ret
    } // end of method averageLength@105::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,float64>,object> 
            Invoke(string _arg1) cil managed
    {
      // Code size       33 (0x21)
      .maxstack  7
      .locals init (string V_0,
               float64 V_1,
               int32 V_2)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
      IL_0008:  stloc.2
      IL_0009:  ldloc.2
      IL_000a:  conv.r8
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/averageLength@105::builder@
      IL_0012:  ldloc.0
      IL_0013:  ldloc.1
      IL_0014:  newobj     instance void class [System.Runtime]System.Tuple`2<string,float64>::.ctor(!0,
                                                                                                     !1)
      IL_0019:  tail.
      IL_001b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<string,float64>,object>(!!0)
      IL_0020:  ret
    } // end of method averageLength@105::Invoke

  } // end of class averageLength@105

  .class auto ansi serializable sealed nested assembly beforefieldinit 'averageLength@107-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<string,float64>,float64>
  {
    .field static assembly initonly class Linq101Aggregates01/'averageLength@107-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<string,float64>,float64>::.ctor()
      IL_0006:  ret
    } // end of method 'averageLength@107-1'::.ctor

    .method public strict virtual instance float64 
            Invoke(class [System.Runtime]System.Tuple`2<string,float64> tupledArg) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  5
      .locals init (string V_0,
               float64 V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<string,float64>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<string,float64>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  ret
    } // end of method 'averageLength@107-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'averageLength@107-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'averageLength@107-1' Linq101Aggregates01/'averageLength@107-1'::@_instance
      IL_000a:  ret
    } // end of method 'averageLength@107-1'::.cctor

  } // end of class 'averageLength@107-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@113'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #7 input at line 112@113'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #7 input at line 112@113'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #7 input at line 112@113'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #7 input at line 112@113'::Invoke

  } // end of class 'Pipe #7 input at line 112@113'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@114-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #7 input at line 112@114-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #7 input at line 112@114-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #7 input at line 112@114-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #7 input at line 112@114-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #7 input at line 112@114-1' Linq101Aggregates01/'Pipe #7 input at line 112@114-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #7 input at line 112@114-1'::.cctor

  } // end of class 'Pipe #7 input at line 112@114-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@114-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #7 input at line 112@114-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #7 input at line 112@114-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #7 input at line 112@114-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #7 input at line 112@114-2'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #7 input at line 112@114-2' Linq101Aggregates01/'Pipe #7 input at line 112@114-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #7 input at line 112@114-2'::.cctor

  } // end of class 'Pipe #7 input at line 112@114-2'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname averagePrice@115
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } // end of method averagePrice@115::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::g
      IL_002d:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0046:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0064:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method averagePrice@115::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averagePrice@115::pc
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
        IL_0018:  ldfld      int32 Linq101Aggregates01/averagePrice@115::pc
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
        IL_0044:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
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
    } // end of method averagePrice@115::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Aggregates01/averagePrice@115::pc
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
    } // end of method averagePrice@115::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_0006:  ret
    } // end of method averagePrice@115::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void Linq101Aggregates01/averagePrice@115::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                     class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                     int32,
                                                                                     class [Utils]Utils/Product)
      IL_000e:  ret
    } // end of method averagePrice@115::GetFreshEnumerator

  } // end of class averagePrice@115

  .class auto ansi serializable sealed nested assembly beforefieldinit 'averagePrice@115-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>
  {
    .field static assembly initonly class Linq101Aggregates01/'averagePrice@115-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>::.ctor()
      IL_0006:  ret
    } // end of method 'averagePrice@115-1'::.ctor

    .method public strict virtual instance valuetype [System.Runtime]System.Decimal 
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } // end of method 'averagePrice@115-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'averagePrice@115-1'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'averagePrice@115-1' Linq101Aggregates01/'averagePrice@115-1'::@_instance
      IL_000a:  ret
    } // end of method 'averagePrice@115-1'::.cctor

  } // end of class 'averagePrice@115-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@114-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #7 input at line 112@114-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #7 input at line 112@114-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       215 (0xd7)
      .maxstack  9
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable> V_4,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal> V_5,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_6,
               class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_7,
               valuetype [System.Runtime]System.Decimal V_8,
               valuetype [System.Runtime]System.Decimal V_9,
               int32 V_10,
               valuetype [System.Runtime]System.Decimal V_11,
               int32 V_12,
               class [System.Runtime]System.IDisposable V_13)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  stloc.3
      IL_000a:  ldloc.0
      IL_000b:  ldnull
      IL_000c:  ldc.i4.0
      IL_000d:  ldnull
      IL_000e:  newobj     instance void Linq101Aggregates01/averagePrice@115::.ctor(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                     class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                     int32,
                                                                                     class [Utils]Utils/Product)
      IL_0013:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0018:  stloc.s    V_4
      IL_001a:  ldsfld     class Linq101Aggregates01/'averagePrice@115-1' Linq101Aggregates01/'averagePrice@115-1'::@_instance
      IL_001f:  stloc.s    V_5
      IL_0021:  ldloc.s    V_4
      IL_0023:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
      IL_0028:  stloc.s    V_6
      IL_002a:  ldloc.s    V_6
      IL_002c:  box        class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_0031:  brtrue.s   IL_003e

      IL_0033:  ldstr      "source"
      IL_0038:  newobj     instance void [netstandard]System.ArgumentNullException::.ctor(string)
      IL_003d:  throw

      IL_003e:  nop
      IL_003f:  ldloc.s    V_6
      IL_0041:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0046:  stloc.s    V_7
      .try
      {
        IL_0048:  ldc.i4.0
        IL_0049:  ldc.i4.0
        IL_004a:  ldc.i4.0
        IL_004b:  ldc.i4.0
        IL_004c:  ldc.i4.0
        IL_004d:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                              int32,
                                                                              int32,
                                                                              bool,
                                                                              uint8)
        IL_0052:  stloc.s    V_9
        IL_0054:  ldc.i4.0
        IL_0055:  stloc.s    V_10
        IL_0057:  br.s       IL_0076

        IL_0059:  ldloc.s    V_9
        IL_005b:  ldloc.s    V_5
        IL_005d:  ldloc.s    V_7
        IL_005f:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_0064:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>::Invoke(!0)
        IL_0069:  call       valuetype [netstandard]System.Decimal [netstandard]System.Decimal::op_Addition(valuetype [netstandard]System.Decimal,
                                                                                                            valuetype [netstandard]System.Decimal)
        IL_006e:  stloc.s    V_9
        IL_0070:  ldloc.s    V_10
        IL_0072:  ldc.i4.1
        IL_0073:  add
        IL_0074:  stloc.s    V_10
        IL_0076:  ldloc.s    V_7
        IL_0078:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
        IL_007d:  brtrue.s   IL_0059

        IL_007f:  ldloc.s    V_10
        IL_0081:  brtrue.s   IL_008e

        IL_0083:  ldstr      "source"
        IL_0088:  newobj     instance void [netstandard]System.InvalidOperationException::.ctor(string)
        IL_008d:  throw

        IL_008e:  nop
        IL_008f:  ldloc.s    V_9
        IL_0091:  stloc.s    V_11
        IL_0093:  ldloc.s    V_10
        IL_0095:  stloc.s    V_12
        IL_0097:  ldloc.s    V_11
        IL_0099:  ldloc.s    V_12
        IL_009b:  call       valuetype [netstandard]System.Decimal [netstandard]System.Convert::ToDecimal(int32)
        IL_00a0:  call       valuetype [netstandard]System.Decimal [netstandard]System.Decimal::Divide(valuetype [netstandard]System.Decimal,
                                                                                                       valuetype [netstandard]System.Decimal)
        IL_00a5:  stloc.s    V_8
        IL_00a7:  leave.s    IL_00bf

      }  // end .try
      finally
      {
        IL_00a9:  ldloc.s    V_7
        IL_00ab:  isinst     [System.Runtime]System.IDisposable
        IL_00b0:  stloc.s    V_13
        IL_00b2:  ldloc.s    V_13
        IL_00b4:  brfalse.s  IL_00be

        IL_00b6:  ldloc.s    V_13
        IL_00b8:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
        IL_00bd:  endfinally
        IL_00be:  endfinally
      }  // end handler
      IL_00bf:  ldloc.s    V_8
      IL_00c1:  stloc.1
      IL_00c2:  ldarg.0
      IL_00c3:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #7 input at line 112@114-3'::builder@
      IL_00c8:  ldloc.0
      IL_00c9:  ldloc.1
      IL_00ca:  newobj     instance void class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                             !1)
      IL_00cf:  tail.
      IL_00d1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(!!0)
      IL_00d6:  ret
    } // end of method 'Pipe #7 input at line 112@114-3'::Invoke

  } // end of class 'Pipe #7 input at line 112@114-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@116-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>
  {
    .field static assembly initonly class Linq101Aggregates01/'Pipe #7 input at line 112@116-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #7 input at line 112@116-4'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal> 
            Invoke(class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal> tupledArg) cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                      !1)
      IL_001a:  ret
    } // end of method 'Pipe #7 input at line 112@116-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Aggregates01/'Pipe #7 input at line 112@116-4'::.ctor()
      IL_0005:  stsfld     class Linq101Aggregates01/'Pipe #7 input at line 112@116-4' Linq101Aggregates01/'Pipe #7 input at line 112@116-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #7 input at line 112@116-4'::.cctor

  } // end of class 'Pipe #7 input at line 112@116-4'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_factorsOf300() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::factorsOf300@8
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_factorsOf300

  .method public specialname static int32 
          get_uniqueFactors() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::uniqueFactors@10
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_uniqueFactors

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers@17
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_numbers

  .method public specialname static int32 
          get_numSum() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numSum@19
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_numSum

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_words() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::words@26
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_words

  .method public specialname static int32 
          get_totalChars() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::totalChars@28
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_totalChars

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::products@35
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_products

  .method public specialname static class [System.Runtime]System.Tuple`2<string,int32>[] 
          get_categories() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`2<string,int32>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories@37
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories

  .method public specialname static int32 
          get_minNum() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::minNum@49
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_minNum

  .method public specialname static int32 
          get_shortestWord() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::shortestWord@52
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_shortestWord

  .method public specialname static class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] 
          get_categories2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories2@55
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories2

  .method public specialname static class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] 
          get_categories3() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories3@64
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories3

  .method public specialname static int32 
          get_maxNum() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::maxNum@74
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_maxNum

  .method public specialname static int32 
          get_longestLength() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::longestLength@77
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_longestLength

  .method public specialname static class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] 
          get_categories4() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories4@80
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories4

  .method public specialname static class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] 
          get_categories5() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories5@89
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories5

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> 
          get_numbers2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers2@99
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_numbers2

  .method public specialname static float64 
          get_averageNum() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageNum@100
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_averageNum

  .method public specialname static float64 
          get_averageLength() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageLength@103
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_averageLength

  .method public specialname static class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] 
          get_categories6() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories6@111
    IL_0005:  ret
  } // end of method Linq101Aggregates01::get_categories6

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          factorsOf300()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_factorsOf300()
  } // end of property Linq101Aggregates01::factorsOf300
  .property int32 uniqueFactors()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_uniqueFactors()
  } // end of property Linq101Aggregates01::uniqueFactors
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
  } // end of property Linq101Aggregates01::numbers
  .property int32 numSum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_numSum()
  } // end of property Linq101Aggregates01::numSum
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
  } // end of property Linq101Aggregates01::words
  .property int32 totalChars()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_totalChars()
  } // end of property Linq101Aggregates01::totalChars
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
  } // end of property Linq101Aggregates01::products
  .property class [System.Runtime]System.Tuple`2<string,int32>[]
          categories()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`2<string,int32>[] Linq101Aggregates01::get_categories()
  } // end of property Linq101Aggregates01::categories
  .property int32 minNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_minNum()
  } // end of property Linq101Aggregates01::minNum
  .property int32 shortestWord()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_shortestWord()
  } // end of property Linq101Aggregates01::shortestWord
  .property class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[]
          categories2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] Linq101Aggregates01::get_categories2()
  } // end of property Linq101Aggregates01::categories2
  .property class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[]
          categories3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] Linq101Aggregates01::get_categories3()
  } // end of property Linq101Aggregates01::categories3
  .property int32 maxNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_maxNum()
  } // end of property Linq101Aggregates01::maxNum
  .property int32 longestLength()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101Aggregates01::get_longestLength()
  } // end of property Linq101Aggregates01::longestLength
  .property class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[]
          categories4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] Linq101Aggregates01::get_categories4()
  } // end of property Linq101Aggregates01::categories4
  .property class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[]
          categories5()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] Linq101Aggregates01::get_categories5()
  } // end of property Linq101Aggregates01::categories5
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>
          numbers2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> Linq101Aggregates01::get_numbers2()
  } // end of property Linq101Aggregates01::numbers2
  .property float64 averageNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get float64 Linq101Aggregates01::get_averageNum()
  } // end of property Linq101Aggregates01::averageNum
  .property float64 averageLength()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get float64 Linq101Aggregates01::get_averageLength()
  } // end of property Linq101Aggregates01::averageLength
  .property class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[]
          categories6()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] Linq101Aggregates01::get_categories6()
  } // end of property Linq101Aggregates01::categories6
} // end of class Linq101Aggregates01

.class private abstract auto ansi sealed '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01
       extends [System.Runtime]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> factorsOf300@8
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 uniqueFactors@10
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@17
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 numSum@19
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@26
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 totalChars@28
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@35
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`2<string,int32>[] categories@37
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 minNum@49
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 shortestWord@52
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] categories2@55
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] categories3@64
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 maxNum@74
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 longestLength@77
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] categories4@80
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] categories5@89
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> numbers2@99
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly float64 averageNum@100
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly float64 averageLength@103
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] categories6@111
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1745 (0x6d1)
    .maxstack  13
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             int32 V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             int32 V_3,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_4,
             int32 V_5,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> V_6,
             class [System.Runtime]System.Tuple`2<string,int32>[] V_7,
             int32 V_8,
             int32 V_9,
             class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] V_10,
             class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] V_11,
             int32 V_12,
             int32 V_13,
             class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] V_14,
             class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] V_15,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> V_16,
             float64 V_17,
             float64 V_18,
             class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] V_19,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_20,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_21,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_22,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_23,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable> V_24,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_25,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_26,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> V_27,
             int32 V_28,
             int32 V_29,
             class [System.Runtime]System.IDisposable V_30,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_31,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_32,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable> V_33,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> V_34,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> V_35,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> V_36,
             int32 V_37,
             int32 V_38,
             class [System.Runtime]System.IDisposable V_39,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,int32>> V_40,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_41,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>> V_42,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_43,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>> V_44,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_45,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>> V_46,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_47,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>> V_48,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_49,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_50,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_51,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [System.Runtime]System.Collections.IEnumerable> V_52,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64> V_53,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<float64> V_54,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> V_55,
             float64 V_56,
             float64 V_57,
             int32 V_58,
             float64 V_59,
             int32 V_60,
             class [System.Runtime]System.IDisposable V_61,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_62,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_63,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,float64>,class [System.Runtime]System.Collections.IEnumerable> V_64,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<string,float64>,float64> V_65,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,float64>> V_66,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [System.Runtime]System.Tuple`2<string,float64>> V_67,
             float64 V_68,
             float64 V_69,
             int32 V_70,
             float64 V_71,
             int32 V_72,
             class [System.Runtime]System.IDisposable V_73,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>> V_74,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_75)
    IL_0000:  ldc.i4.2
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  ldc.i4.5
    IL_0004:  ldc.i4.5
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0023:  dup
    IL_0024:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::factorsOf300@8
    IL_0029:  stloc.0
    IL_002a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_002f:  stloc.s    V_21
    IL_0031:  ldloc.s    V_21
    IL_0033:  ldnull
    IL_0034:  ldc.i4.0
    IL_0035:  ldc.i4.0
    IL_0036:  newobj     instance void Linq101Aggregates01/'Pipe #1 input at line 11@12'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                int32,
                                                                                                int32)
    IL_003b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0040:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Distinct<int32,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>)
    IL_0045:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_004a:  stloc.s    V_20
    IL_004c:  ldloc.s    V_20
    IL_004e:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0053:  dup
    IL_0054:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::uniqueFactors@10
    IL_0059:  stloc.1
    IL_005a:  ldc.i4.5
    IL_005b:  ldc.i4.4
    IL_005c:  ldc.i4.1
    IL_005d:  ldc.i4.3
    IL_005e:  ldc.i4.s   9
    IL_0060:  ldc.i4.8
    IL_0061:  ldc.i4.6
    IL_0062:  ldc.i4.7
    IL_0063:  ldc.i4.2
    IL_0064:  ldc.i4.0
    IL_0065:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_006a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_006f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0074:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0079:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_007e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0083:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0088:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_008d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0092:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0097:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_009c:  dup
    IL_009d:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers@17
    IL_00a2:  stloc.2
    IL_00a3:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a8:  stloc.s    V_22
    IL_00aa:  ldloc.s    V_22
    IL_00ac:  stloc.s    V_23
    IL_00ae:  ldnull
    IL_00af:  ldc.i4.0
    IL_00b0:  ldc.i4.0
    IL_00b1:  newobj     instance void Linq101Aggregates01/numSum@21::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_00b6:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00bb:  stloc.s    V_24
    IL_00bd:  ldsfld     class Linq101Aggregates01/'numSum@22-1' Linq101Aggregates01/'numSum@22-1'::@_instance
    IL_00c2:  stloc.s    V_25
    IL_00c4:  ldloc.s    V_24
    IL_00c6:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_00cb:  stloc.s    V_26
    IL_00cd:  ldloc.s    V_26
    IL_00cf:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_00d4:  stloc.s    V_27
    .try
    {
      IL_00d6:  ldc.i4.0
      IL_00d7:  stloc.s    V_29
      IL_00d9:  br.s       IL_00ee

      IL_00db:  ldloc.s    V_29
      IL_00dd:  ldloc.s    V_25
      IL_00df:  ldloc.s    V_27
      IL_00e1:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_00e6:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_00eb:  add.ovf
      IL_00ec:  stloc.s    V_29
      IL_00ee:  ldloc.s    V_27
      IL_00f0:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_00f5:  brtrue.s   IL_00db

      IL_00f7:  ldloc.s    V_29
      IL_00f9:  stloc.s    V_28
      IL_00fb:  leave.s    IL_0113

    }  // end .try
    finally
    {
      IL_00fd:  ldloc.s    V_27
      IL_00ff:  isinst     [System.Runtime]System.IDisposable
      IL_0104:  stloc.s    V_30
      IL_0106:  ldloc.s    V_30
      IL_0108:  brfalse.s  IL_0112

      IL_010a:  ldloc.s    V_30
      IL_010c:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_0111:  endfinally
      IL_0112:  endfinally
    }  // end handler
    IL_0113:  ldloc.s    V_28
    IL_0115:  dup
    IL_0116:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numSum@19
    IL_011b:  stloc.3
    IL_011c:  ldstr      "cherry"
    IL_0121:  ldstr      "apple"
    IL_0126:  ldstr      "blueberry"
    IL_012b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0130:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0135:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013f:  dup
    IL_0140:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::words@26
    IL_0145:  stloc.s    V_4
    IL_0147:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_014c:  stloc.s    V_31
    IL_014e:  ldloc.s    V_31
    IL_0150:  stloc.s    V_32
    IL_0152:  ldnull
    IL_0153:  ldc.i4.0
    IL_0154:  ldnull
    IL_0155:  newobj     instance void Linq101Aggregates01/totalChars@30::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
    IL_015a:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_015f:  stloc.s    V_33
    IL_0161:  ldsfld     class Linq101Aggregates01/'totalChars@31-1' Linq101Aggregates01/'totalChars@31-1'::@_instance
    IL_0166:  stloc.s    V_34
    IL_0168:  ldloc.s    V_33
    IL_016a:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_016f:  stloc.s    V_35
    IL_0171:  ldloc.s    V_35
    IL_0173:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_0178:  stloc.s    V_36
    .try
    {
      IL_017a:  ldc.i4.0
      IL_017b:  stloc.s    V_38
      IL_017d:  br.s       IL_0192

      IL_017f:  ldloc.s    V_38
      IL_0181:  ldloc.s    V_34
      IL_0183:  ldloc.s    V_36
      IL_0185:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_018a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::Invoke(!0)
      IL_018f:  add.ovf
      IL_0190:  stloc.s    V_38
      IL_0192:  ldloc.s    V_36
      IL_0194:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_0199:  brtrue.s   IL_017f

      IL_019b:  ldloc.s    V_38
      IL_019d:  stloc.s    V_37
      IL_019f:  leave.s    IL_01b7

    }  // end .try
    finally
    {
      IL_01a1:  ldloc.s    V_36
      IL_01a3:  isinst     [System.Runtime]System.IDisposable
      IL_01a8:  stloc.s    V_39
      IL_01aa:  ldloc.s    V_39
      IL_01ac:  brfalse.s  IL_01b6

      IL_01ae:  ldloc.s    V_39
      IL_01b0:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_01b5:  endfinally
      IL_01b6:  endfinally
    }  // end handler
    IL_01b7:  ldloc.s    V_37
    IL_01b9:  dup
    IL_01ba:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::totalChars@28
    IL_01bf:  stloc.s    V_5
    IL_01c1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_01c6:  dup
    IL_01c7:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::products@35
    IL_01cc:  stloc.s    V_6
    IL_01ce:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01d3:  stloc.s    V_41
    IL_01d5:  ldloc.s    V_41
    IL_01d7:  ldloc.s    V_41
    IL_01d9:  ldloc.s    V_41
    IL_01db:  ldloc.s    V_41
    IL_01dd:  ldloc.s    V_41
    IL_01df:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_01e4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01e9:  ldloc.s    V_41
    IL_01eb:  newobj     instance void Linq101Aggregates01/'Pipe #2 input at line 38@39'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01f0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01f5:  ldsfld     class Linq101Aggregates01/'Pipe #2 input at line 38@40-1' Linq101Aggregates01/'Pipe #2 input at line 38@40-1'::@_instance
    IL_01fa:  ldsfld     class Linq101Aggregates01/'Pipe #2 input at line 38@40-2' Linq101Aggregates01/'Pipe #2 input at line 38@40-2'::@_instance
    IL_01ff:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0204:  ldloc.s    V_41
    IL_0206:  newobj     instance void Linq101Aggregates01/'Pipe #2 input at line 38@40-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_020b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0210:  ldsfld     class Linq101Aggregates01/'Pipe #2 input at line 38@45-4' Linq101Aggregates01/'Pipe #2 input at line 38@45-4'::@_instance
    IL_0215:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_021a:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,int32>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_021f:  stloc.s    V_40
    IL_0221:  ldloc.s    V_40
    IL_0223:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,int32>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0228:  dup
    IL_0229:  stsfld     class [System.Runtime]System.Tuple`2<string,int32>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories@37
    IL_022e:  stloc.s    V_7
    IL_0230:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0235:  ldnull
    IL_0236:  ldc.i4.0
    IL_0237:  ldc.i4.0
    IL_0238:  newobj     instance void Linq101Aggregates01/minNum@49::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_023d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0242:  ldsfld     class Linq101Aggregates01/'minNum@49-1' Linq101Aggregates01/'minNum@49-1'::@_instance
    IL_0247:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<int32,class [System.Runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_024c:  dup
    IL_024d:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::minNum@49
    IL_0252:  stloc.s    V_8
    IL_0254:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0259:  ldnull
    IL_025a:  ldc.i4.0
    IL_025b:  ldnull
    IL_025c:  newobj     instance void Linq101Aggregates01/shortestWord@52::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
    IL_0261:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0266:  ldsfld     class Linq101Aggregates01/'shortestWord@52-1' Linq101Aggregates01/'shortestWord@52-1'::@_instance
    IL_026b:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<string,class [System.Runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0270:  dup
    IL_0271:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::shortestWord@52
    IL_0276:  stloc.s    V_9
    IL_0278:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_027d:  stloc.s    V_43
    IL_027f:  ldloc.s    V_43
    IL_0281:  ldloc.s    V_43
    IL_0283:  ldloc.s    V_43
    IL_0285:  ldloc.s    V_43
    IL_0287:  ldloc.s    V_43
    IL_0289:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_028e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0293:  ldloc.s    V_43
    IL_0295:  newobj     instance void Linq101Aggregates01/'Pipe #3 input at line 56@57'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_029a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_029f:  ldsfld     class Linq101Aggregates01/'Pipe #3 input at line 56@58-1' Linq101Aggregates01/'Pipe #3 input at line 56@58-1'::@_instance
    IL_02a4:  ldsfld     class Linq101Aggregates01/'Pipe #3 input at line 56@58-2' Linq101Aggregates01/'Pipe #3 input at line 56@58-2'::@_instance
    IL_02a9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_02ae:  ldloc.s    V_43
    IL_02b0:  newobj     instance void Linq101Aggregates01/'Pipe #3 input at line 56@58-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02b5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02ba:  ldsfld     class Linq101Aggregates01/'Pipe #3 input at line 56@60-4' Linq101Aggregates01/'Pipe #3 input at line 56@60-4'::@_instance
    IL_02bf:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_02c4:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_02c9:  stloc.s    V_42
    IL_02cb:  ldloc.s    V_42
    IL_02cd:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02d2:  dup
    IL_02d3:  stsfld     class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories2@55
    IL_02d8:  stloc.s    V_10
    IL_02da:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_02df:  stloc.s    V_45
    IL_02e1:  ldloc.s    V_45
    IL_02e3:  ldloc.s    V_45
    IL_02e5:  ldloc.s    V_45
    IL_02e7:  ldloc.s    V_45
    IL_02e9:  ldloc.s    V_45
    IL_02eb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_02f0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02f5:  ldloc.s    V_45
    IL_02f7:  newobj     instance void Linq101Aggregates01/'Pipe #4 input at line 65@66'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02fc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0301:  ldsfld     class Linq101Aggregates01/'Pipe #4 input at line 65@67-1' Linq101Aggregates01/'Pipe #4 input at line 65@67-1'::@_instance
    IL_0306:  ldsfld     class Linq101Aggregates01/'Pipe #4 input at line 65@67-2' Linq101Aggregates01/'Pipe #4 input at line 65@67-2'::@_instance
    IL_030b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0310:  ldloc.s    V_45
    IL_0312:  newobj     instance void Linq101Aggregates01/'Pipe #4 input at line 65@67-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0317:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_031c:  ldsfld     class Linq101Aggregates01/'Pipe #4 input at line 65@70-4' Linq101Aggregates01/'Pipe #4 input at line 65@70-4'::@_instance
    IL_0321:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0326:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_032b:  stloc.s    V_44
    IL_032d:  ldloc.s    V_44
    IL_032f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0334:  dup
    IL_0335:  stsfld     class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories3@64
    IL_033a:  stloc.s    V_11
    IL_033c:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0341:  ldnull
    IL_0342:  ldc.i4.0
    IL_0343:  ldc.i4.0
    IL_0344:  newobj     instance void Linq101Aggregates01/maxNum@74::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0349:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_034e:  ldsfld     class Linq101Aggregates01/'maxNum@74-1' Linq101Aggregates01/'maxNum@74-1'::@_instance
    IL_0353:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<int32,class [System.Runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0358:  dup
    IL_0359:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::maxNum@74
    IL_035e:  stloc.s    V_12
    IL_0360:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0365:  ldnull
    IL_0366:  ldc.i4.0
    IL_0367:  ldnull
    IL_0368:  newobj     instance void Linq101Aggregates01/longestLength@77::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                   int32,
                                                                                   string)
    IL_036d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0372:  ldsfld     class Linq101Aggregates01/'longestLength@77-1' Linq101Aggregates01/'longestLength@77-1'::@_instance
    IL_0377:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<string,class [System.Runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_037c:  dup
    IL_037d:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::longestLength@77
    IL_0382:  stloc.s    V_13
    IL_0384:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0389:  stloc.s    V_47
    IL_038b:  ldloc.s    V_47
    IL_038d:  ldloc.s    V_47
    IL_038f:  ldloc.s    V_47
    IL_0391:  ldloc.s    V_47
    IL_0393:  ldloc.s    V_47
    IL_0395:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_039a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_039f:  ldloc.s    V_47
    IL_03a1:  newobj     instance void Linq101Aggregates01/'Pipe #5 input at line 81@82'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03a6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03ab:  ldsfld     class Linq101Aggregates01/'Pipe #5 input at line 81@83-1' Linq101Aggregates01/'Pipe #5 input at line 81@83-1'::@_instance
    IL_03b0:  ldsfld     class Linq101Aggregates01/'Pipe #5 input at line 81@83-2' Linq101Aggregates01/'Pipe #5 input at line 81@83-2'::@_instance
    IL_03b5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_03ba:  ldloc.s    V_47
    IL_03bc:  newobj     instance void Linq101Aggregates01/'Pipe #5 input at line 81@83-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03c1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03c6:  ldsfld     class Linq101Aggregates01/'Pipe #5 input at line 81@85-4' Linq101Aggregates01/'Pipe #5 input at line 81@85-4'::@_instance
    IL_03cb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03d0:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_03d5:  stloc.s    V_46
    IL_03d7:  ldloc.s    V_46
    IL_03d9:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03de:  dup
    IL_03df:  stsfld     class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories4@80
    IL_03e4:  stloc.s    V_14
    IL_03e6:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03eb:  stloc.s    V_49
    IL_03ed:  ldloc.s    V_49
    IL_03ef:  ldloc.s    V_49
    IL_03f1:  ldloc.s    V_49
    IL_03f3:  ldloc.s    V_49
    IL_03f5:  ldloc.s    V_49
    IL_03f7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_03fc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0401:  ldloc.s    V_49
    IL_0403:  newobj     instance void Linq101Aggregates01/'Pipe #6 input at line 90@91'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0408:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_040d:  ldsfld     class Linq101Aggregates01/'Pipe #6 input at line 90@92-1' Linq101Aggregates01/'Pipe #6 input at line 90@92-1'::@_instance
    IL_0412:  ldsfld     class Linq101Aggregates01/'Pipe #6 input at line 90@92-2' Linq101Aggregates01/'Pipe #6 input at line 90@92-2'::@_instance
    IL_0417:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_041c:  ldloc.s    V_49
    IL_041e:  newobj     instance void Linq101Aggregates01/'Pipe #6 input at line 90@92-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0423:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0428:  ldsfld     class Linq101Aggregates01/'Pipe #6 input at line 90@95-4' Linq101Aggregates01/'Pipe #6 input at line 90@95-4'::@_instance
    IL_042d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0432:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0437:  stloc.s    V_48
    IL_0439:  ldloc.s    V_48
    IL_043b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0440:  dup
    IL_0441:  stsfld     class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories5@89
    IL_0446:  stloc.s    V_15
    IL_0448:  ldc.r8     5.0999999999999996
    IL_0451:  ldc.r8     4.0999999999999996
    IL_045a:  ldc.r8     1.1000000000000001
    IL_0463:  ldc.r8     3.1000000000000001
    IL_046c:  ldc.r8     9.0999999999999996
    IL_0475:  ldc.r8     8.0999999999999996
    IL_047e:  ldc.r8     6.0999999999999996
    IL_0487:  ldc.r8     7.0999999999999996
    IL_0490:  ldc.r8     2.1000000000000001
    IL_0499:  ldc.r8     0.10000000000000001
    IL_04a2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
    IL_04a7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04ac:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04b1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04b6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04bb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04c0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04c5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04ca:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04cf:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04d4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04d9:  dup
    IL_04da:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers2@99
    IL_04df:  stloc.s    V_16
    IL_04e1:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_04e6:  stloc.s    V_50
    IL_04e8:  ldloc.s    V_50
    IL_04ea:  stloc.s    V_51
    IL_04ec:  ldnull
    IL_04ed:  ldc.i4.0
    IL_04ee:  ldc.r8     0.0
    IL_04f7:  newobj     instance void Linq101Aggregates01/averageNum@100::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                 int32,
                                                                                 float64)
    IL_04fc:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0501:  stloc.s    V_52
    IL_0503:  ldsfld     class Linq101Aggregates01/'averageNum@100-1' Linq101Aggregates01/'averageNum@100-1'::@_instance
    IL_0508:  stloc.s    V_53
    IL_050a:  ldloc.s    V_52
    IL_050c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0511:  stloc.s    V_54
    IL_0513:  ldloc.s    V_54
    IL_0515:  box        class [System.Runtime]System.Collections.Generic.IEnumerable`1<float64>
    IL_051a:  brtrue.s   IL_0527

    IL_051c:  ldstr      "source"
    IL_0521:  newobj     instance void [netstandard]System.ArgumentNullException::.ctor(string)
    IL_0526:  throw

    IL_0527:  nop
    IL_0528:  ldloc.s    V_54
    IL_052a:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_052f:  stloc.s    V_55
    .try
    {
      IL_0531:  ldc.r8     0.0
      IL_053a:  stloc.s    V_57
      IL_053c:  ldc.i4.0
      IL_053d:  stloc.s    V_58
      IL_053f:  br.s       IL_055a

      IL_0541:  ldloc.s    V_57
      IL_0543:  ldloc.s    V_53
      IL_0545:  ldloc.s    V_55
      IL_0547:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_054c:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::Invoke(!0)
      IL_0551:  add
      IL_0552:  stloc.s    V_57
      IL_0554:  ldloc.s    V_58
      IL_0556:  ldc.i4.1
      IL_0557:  add
      IL_0558:  stloc.s    V_58
      IL_055a:  ldloc.s    V_55
      IL_055c:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_0561:  brtrue.s   IL_0541

      IL_0563:  ldloc.s    V_58
      IL_0565:  brtrue.s   IL_0572

      IL_0567:  ldstr      "source"
      IL_056c:  newobj     instance void [netstandard]System.InvalidOperationException::.ctor(string)
      IL_0571:  throw

      IL_0572:  nop
      IL_0573:  ldloc.s    V_57
      IL_0575:  stloc.s    V_59
      IL_0577:  ldloc.s    V_58
      IL_0579:  stloc.s    V_60
      IL_057b:  ldloc.s    V_59
      IL_057d:  ldloc.s    V_60
      IL_057f:  conv.r8
      IL_0580:  div
      IL_0581:  stloc.s    V_56
      IL_0583:  leave.s    IL_059b

    }  // end .try
    finally
    {
      IL_0585:  ldloc.s    V_55
      IL_0587:  isinst     [System.Runtime]System.IDisposable
      IL_058c:  stloc.s    V_61
      IL_058e:  ldloc.s    V_61
      IL_0590:  brfalse.s  IL_059a

      IL_0592:  ldloc.s    V_61
      IL_0594:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_0599:  endfinally
      IL_059a:  endfinally
    }  // end handler
    IL_059b:  ldloc.s    V_56
    IL_059d:  dup
    IL_059e:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageNum@100
    IL_05a3:  stloc.s    V_17
    IL_05a5:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_05aa:  stloc.s    V_62
    IL_05ac:  ldloc.s    V_62
    IL_05ae:  stloc.s    V_63
    IL_05b0:  ldloc.s    V_62
    IL_05b2:  ldloc.s    V_62
    IL_05b4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
    IL_05b9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_05be:  ldloc.s    V_62
    IL_05c0:  newobj     instance void Linq101Aggregates01/averageLength@105::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_05c5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,float64>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_05ca:  stloc.s    V_64
    IL_05cc:  ldsfld     class Linq101Aggregates01/'averageLength@107-1' Linq101Aggregates01/'averageLength@107-1'::@_instance
    IL_05d1:  stloc.s    V_65
    IL_05d3:  ldloc.s    V_64
    IL_05d5:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,float64>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_05da:  stloc.s    V_66
    IL_05dc:  ldloc.s    V_66
    IL_05de:  box        class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,float64>>
    IL_05e3:  brtrue.s   IL_05f0

    IL_05e5:  ldstr      "source"
    IL_05ea:  newobj     instance void [netstandard]System.ArgumentNullException::.ctor(string)
    IL_05ef:  throw

    IL_05f0:  nop
    IL_05f1:  ldloc.s    V_66
    IL_05f3:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,float64>>::GetEnumerator()
    IL_05f8:  stloc.s    V_67
    .try
    {
      IL_05fa:  ldc.r8     0.0
      IL_0603:  stloc.s    V_69
      IL_0605:  ldc.i4.0
      IL_0606:  stloc.s    V_70
      IL_0608:  br.s       IL_0623

      IL_060a:  ldloc.s    V_69
      IL_060c:  ldloc.s    V_65
      IL_060e:  ldloc.s    V_67
      IL_0610:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<class [System.Runtime]System.Tuple`2<string,float64>>::get_Current()
      IL_0615:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<string,float64>,float64>::Invoke(!0)
      IL_061a:  add
      IL_061b:  stloc.s    V_69
      IL_061d:  ldloc.s    V_70
      IL_061f:  ldc.i4.1
      IL_0620:  add
      IL_0621:  stloc.s    V_70
      IL_0623:  ldloc.s    V_67
      IL_0625:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_062a:  brtrue.s   IL_060a

      IL_062c:  ldloc.s    V_70
      IL_062e:  brtrue.s   IL_063b

      IL_0630:  ldstr      "source"
      IL_0635:  newobj     instance void [netstandard]System.InvalidOperationException::.ctor(string)
      IL_063a:  throw

      IL_063b:  nop
      IL_063c:  ldloc.s    V_69
      IL_063e:  stloc.s    V_71
      IL_0640:  ldloc.s    V_70
      IL_0642:  stloc.s    V_72
      IL_0644:  ldloc.s    V_71
      IL_0646:  ldloc.s    V_72
      IL_0648:  conv.r8
      IL_0649:  div
      IL_064a:  stloc.s    V_68
      IL_064c:  leave.s    IL_0664

    }  // end .try
    finally
    {
      IL_064e:  ldloc.s    V_67
      IL_0650:  isinst     [System.Runtime]System.IDisposable
      IL_0655:  stloc.s    V_73
      IL_0657:  ldloc.s    V_73
      IL_0659:  brfalse.s  IL_0663

      IL_065b:  ldloc.s    V_73
      IL_065d:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_0662:  endfinally
      IL_0663:  endfinally
    }  // end handler
    IL_0664:  ldloc.s    V_68
    IL_0666:  dup
    IL_0667:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageLength@103
    IL_066c:  stloc.s    V_18
    IL_066e:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0673:  stloc.s    V_75
    IL_0675:  ldloc.s    V_75
    IL_0677:  ldloc.s    V_75
    IL_0679:  ldloc.s    V_75
    IL_067b:  ldloc.s    V_75
    IL_067d:  ldloc.s    V_75
    IL_067f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_0684:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0689:  ldloc.s    V_75
    IL_068b:  newobj     instance void Linq101Aggregates01/'Pipe #7 input at line 112@113'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0690:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0695:  ldsfld     class Linq101Aggregates01/'Pipe #7 input at line 112@114-1' Linq101Aggregates01/'Pipe #7 input at line 112@114-1'::@_instance
    IL_069a:  ldsfld     class Linq101Aggregates01/'Pipe #7 input at line 112@114-2' Linq101Aggregates01/'Pipe #7 input at line 112@114-2'::@_instance
    IL_069f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_06a4:  ldloc.s    V_75
    IL_06a6:  newobj     instance void Linq101Aggregates01/'Pipe #7 input at line 112@114-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_06ab:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_06b0:  ldsfld     class Linq101Aggregates01/'Pipe #7 input at line 112@116-4' Linq101Aggregates01/'Pipe #7 input at line 112@116-4'::@_instance
    IL_06b5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_06ba:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_06bf:  stloc.s    V_74
    IL_06c1:  ldloc.s    V_74
    IL_06c3:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_06c8:  dup
    IL_06c9:  stsfld     class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories6@111
    IL_06ce:  stloc.s    V_19
    IL_06d0:  ret
  } // end of method $Linq101Aggregates01::main@

} // end of class '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net7.0\tests\EmittedIL\QueryExpressionStepping\Linq101Aggregates01_fs\Linq101Aggregates01.res
