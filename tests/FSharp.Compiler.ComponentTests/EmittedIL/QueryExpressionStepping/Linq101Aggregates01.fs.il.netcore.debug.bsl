
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
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
  .ver 6:0:0:0
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
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Aggregates01
{
  // Offset: 0x00000000 Length: 0x00000617
  // WARNING: managed resource file FSharpSignatureData.Linq101Aggregates01 created
}
.mresource public FSharpOptimizationData.Linq101Aggregates01
{
  // Offset: 0x00000620 Length: 0x00000211
  // WARNING: managed resource file FSharpOptimizationData.Linq101Aggregates01 created
}
.module Linq101Aggregates01.exe
// MVID: {624A4F0D-9A84-58A6-A745-03830D4F4A62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000013EDDE20000


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
      // Code size       149 (0x95)
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
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_factorsOf300()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0043:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0050:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/'Pipe #1 input at line 11@12'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.0
      IL_008e:  stfld      int32 Linq101Aggregates01/'Pipe #1 input at line 11@12'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
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
      // Code size       149 (0x95)
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
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0043:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0050:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/numSum@21::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/numSum@21::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.0
      IL_008e:  stfld      int32 Linq101Aggregates01/numSum@21::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
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
      // Code size       149 (0x95)
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
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0043:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0050:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/totalChars@30::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/totalChars@30::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      string Linq101Aggregates01/totalChars@30::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
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
      // Code size       150 (0x96)
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
      IL_001c:  br.s       IL_006c

      IL_001e:  nop
      IL_001f:  br.s       IL_0069

      IL_0021:  nop
      IL_0022:  br.s       IL_008d

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
      IL_003e:  ldarg.0
      IL_003f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0044:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0049:  brfalse.s  IL_006c

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldloc.0
      IL_0058:  stloc.1
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0060:  ldarg.0
      IL_0061:  ldloc.1
      IL_0062:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_0067:  ldc.i4.1
      IL_0068:  ret

      IL_0069:  nop
      IL_006a:  br.s       IL_003e

      IL_006c:  ldarg.0
      IL_006d:  ldc.i4.3
      IL_006e:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0079:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007e:  nop
      IL_007f:  ldarg.0
      IL_0080:  ldnull
      IL_0081:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/sum@42::'enum'
      IL_0086:  ldarg.0
      IL_0087:  ldc.i4.3
      IL_0088:  stfld      int32 Linq101Aggregates01/sum@42::pc
      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/sum@42::current
      IL_0094:  ldc.i4.0
      IL_0095:  ret
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
      // Code size       141 (0x8d)
      .maxstack  8
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               int32 V_1,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable> V_4,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> V_5,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> V_6,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_7,
               class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_8,
               int32 V_9,
               int32 V_10,
               class [System.Runtime]System.IDisposable V_11)
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
      IL_0021:  ldloc.s    V_5
      IL_0023:  stloc.s    V_6
      IL_0025:  ldloc.s    V_4
      IL_0027:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
      IL_002c:  stloc.s    V_7
      IL_002e:  ldloc.s    V_7
      IL_0030:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0035:  stloc.s    V_8
      .try
      {
        IL_0037:  ldc.i4.0
        IL_0038:  stloc.s    V_10
        IL_003a:  ldloc.s    V_8
        IL_003c:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
        IL_0041:  brfalse.s  IL_0059

        IL_0043:  ldloc.s    V_10
        IL_0045:  ldloc.s    V_6
        IL_0047:  ldloc.s    V_8
        IL_0049:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_004e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::Invoke(!0)
        IL_0053:  add.ovf
        IL_0054:  stloc.s    V_10
        IL_0056:  nop
        IL_0057:  br.s       IL_003a

        IL_0059:  ldloc.s    V_10
        IL_005b:  stloc.s    V_9
        IL_005d:  leave.s    IL_0075

      }  // end .try
      finally
      {
        IL_005f:  ldloc.s    V_8
        IL_0061:  isinst     [System.Runtime]System.IDisposable
        IL_0066:  stloc.s    V_11
        IL_0068:  ldloc.s    V_11
        IL_006a:  brfalse.s  IL_0074

        IL_006c:  ldloc.s    V_11
        IL_006e:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
        IL_0073:  endfinally
        IL_0074:  endfinally
      }  // end handler
      IL_0075:  ldloc.s    V_9
      IL_0077:  stloc.1
      IL_0078:  ldarg.0
      IL_0079:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #2 input at line 38@40-3'::builder@
      IL_007e:  ldloc.0
      IL_007f:  ldloc.1
      IL_0080:  newobj     instance void class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::.ctor(!0,
                                                                                                                                                                          !1)
      IL_0085:  tail.
      IL_0087:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(!!0)
      IL_008c:  ret
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
      // Code size       149 (0x95)
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
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0043:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0050:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/minNum@49::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/minNum@49::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.0
      IL_008e:  stfld      int32 Linq101Aggregates01/minNum@49::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
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
      // Code size       149 (0x95)
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
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0043:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0050:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/shortestWord@52::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/shortestWord@52::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      string Linq101Aggregates01/shortestWord@52::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
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
      // Code size       150 (0x96)
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
      IL_001c:  br.s       IL_006c

      IL_001e:  nop
      IL_001f:  br.s       IL_0069

      IL_0021:  nop
      IL_0022:  br.s       IL_008d

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
      IL_003e:  ldarg.0
      IL_003f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0044:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0049:  brfalse.s  IL_006c

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldloc.0
      IL_0058:  stloc.1
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_0060:  ldarg.0
      IL_0061:  ldloc.1
      IL_0062:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_0067:  ldc.i4.1
      IL_0068:  ret

      IL_0069:  nop
      IL_006a:  br.s       IL_003e

      IL_006c:  ldarg.0
      IL_006d:  ldc.i4.3
      IL_006e:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0079:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007e:  nop
      IL_007f:  ldarg.0
      IL_0080:  ldnull
      IL_0081:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/min@59::'enum'
      IL_0086:  ldarg.0
      IL_0087:  ldc.i4.3
      IL_0088:  stfld      int32 Linq101Aggregates01/min@59::pc
      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/min@59::current
      IL_0094:  ldc.i4.0
      IL_0095:  ret
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
      // Code size       150 (0x96)
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
      IL_001c:  br.s       IL_006c

      IL_001e:  nop
      IL_001f:  br.s       IL_0069

      IL_0021:  nop
      IL_0022:  br.s       IL_008d

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
      IL_003e:  ldarg.0
      IL_003f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0044:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0049:  brfalse.s  IL_006c

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldloc.0
      IL_0058:  stloc.1
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0060:  ldarg.0
      IL_0061:  ldloc.1
      IL_0062:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_0067:  ldc.i4.1
      IL_0068:  ret

      IL_0069:  nop
      IL_006a:  br.s       IL_003e

      IL_006c:  ldarg.0
      IL_006d:  ldc.i4.3
      IL_006e:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0079:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007e:  nop
      IL_007f:  ldarg.0
      IL_0080:  ldnull
      IL_0081:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/cheapestProducts@69::'enum'
      IL_0086:  ldarg.0
      IL_0087:  ldc.i4.3
      IL_0088:  stfld      int32 Linq101Aggregates01/cheapestProducts@69::pc
      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/cheapestProducts@69::current
      IL_0094:  ldc.i4.0
      IL_0095:  ret
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
      // Code size       149 (0x95)
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
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Aggregates01::get_numbers()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0043:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0050:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Aggregates01/maxNum@74::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/maxNum@74::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.0
      IL_008e:  stfld      int32 Linq101Aggregates01/maxNum@74::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
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
      // Code size       149 (0x95)
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
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0043:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0050:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Aggregates01/longestLength@77::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/longestLength@77::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      string Linq101Aggregates01/longestLength@77::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
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
      // Code size       150 (0x96)
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
      IL_001c:  br.s       IL_006c

      IL_001e:  nop
      IL_001f:  br.s       IL_0069

      IL_0021:  nop
      IL_0022:  br.s       IL_008d

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
      IL_003e:  ldarg.0
      IL_003f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0044:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0049:  brfalse.s  IL_006c

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldloc.0
      IL_0058:  stloc.1
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0060:  ldarg.0
      IL_0061:  ldloc.1
      IL_0062:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_0067:  ldc.i4.1
      IL_0068:  ret

      IL_0069:  nop
      IL_006a:  br.s       IL_003e

      IL_006c:  ldarg.0
      IL_006d:  ldc.i4.3
      IL_006e:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0079:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007e:  nop
      IL_007f:  ldarg.0
      IL_0080:  ldnull
      IL_0081:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensivePrice@84::'enum'
      IL_0086:  ldarg.0
      IL_0087:  ldc.i4.3
      IL_0088:  stfld      int32 Linq101Aggregates01/mostExpensivePrice@84::pc
      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensivePrice@84::current
      IL_0094:  ldc.i4.0
      IL_0095:  ret
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
      // Code size       150 (0x96)
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
      IL_001c:  br.s       IL_006c

      IL_001e:  nop
      IL_001f:  br.s       IL_0069

      IL_0021:  nop
      IL_0022:  br.s       IL_008d

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
      IL_003e:  ldarg.0
      IL_003f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0044:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0049:  brfalse.s  IL_006c

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldloc.0
      IL_0058:  stloc.1
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0060:  ldarg.0
      IL_0061:  ldloc.1
      IL_0062:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_0067:  ldc.i4.1
      IL_0068:  ret

      IL_0069:  nop
      IL_006a:  br.s       IL_003e

      IL_006c:  ldarg.0
      IL_006d:  ldc.i4.3
      IL_006e:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0079:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007e:  nop
      IL_007f:  ldarg.0
      IL_0080:  ldnull
      IL_0081:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/maxPrice@93::'enum'
      IL_0086:  ldarg.0
      IL_0087:  ldc.i4.3
      IL_0088:  stfld      int32 Linq101Aggregates01/maxPrice@93::pc
      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/maxPrice@93::current
      IL_0094:  ldc.i4.0
      IL_0095:  ret
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
      // Code size       150 (0x96)
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
      IL_001c:  br.s       IL_006c

      IL_001e:  nop
      IL_001f:  br.s       IL_0069

      IL_0021:  nop
      IL_0022:  br.s       IL_008d

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
      IL_003e:  ldarg.0
      IL_003f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0044:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0049:  brfalse.s  IL_006c

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldloc.0
      IL_0058:  stloc.1
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0060:  ldarg.0
      IL_0061:  ldloc.1
      IL_0062:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_0067:  ldc.i4.1
      IL_0068:  ret

      IL_0069:  nop
      IL_006a:  br.s       IL_003e

      IL_006c:  ldarg.0
      IL_006d:  ldc.i4.3
      IL_006e:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0079:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007e:  nop
      IL_007f:  ldarg.0
      IL_0080:  ldnull
      IL_0081:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/mostExpensiveProducts@94::'enum'
      IL_0086:  ldarg.0
      IL_0087:  ldc.i4.3
      IL_0088:  stfld      int32 Linq101Aggregates01/mostExpensiveProducts@94::pc
      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/mostExpensiveProducts@94::current
      IL_0094:  ldc.i4.0
      IL_0095:  ret
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
      // Code size       157 (0x9d)
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
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_0068

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> Linq101Aggregates01::get_numbers2()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0043:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0050:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> Linq101Aggregates01/averageNum@100::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101Aggregates01/averageNum@100::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.r8     0.0
      IL_0096:  stfld      float64 Linq101Aggregates01/averageNum@100::current
      IL_009b:  ldc.i4.0
      IL_009c:  ret
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
      // Code size       150 (0x96)
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
      IL_001c:  br.s       IL_006c

      IL_001e:  nop
      IL_001f:  br.s       IL_0069

      IL_0021:  nop
      IL_0022:  br.s       IL_008d

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
      IL_003e:  ldarg.0
      IL_003f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0044:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0049:  brfalse.s  IL_006c

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldloc.0
      IL_0058:  stloc.1
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0060:  ldarg.0
      IL_0061:  ldloc.1
      IL_0062:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_0067:  ldc.i4.1
      IL_0068:  ret

      IL_0069:  nop
      IL_006a:  br.s       IL_003e

      IL_006c:  ldarg.0
      IL_006d:  ldc.i4.3
      IL_006e:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0079:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007e:  nop
      IL_007f:  ldarg.0
      IL_0080:  ldnull
      IL_0081:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Aggregates01/averagePrice@115::'enum'
      IL_0086:  ldarg.0
      IL_0087:  ldc.i4.3
      IL_0088:  stfld      int32 Linq101Aggregates01/averagePrice@115::pc
      IL_008d:  ldarg.0
      IL_008e:  ldnull
      IL_008f:  stfld      class [Utils]Utils/Product Linq101Aggregates01/averagePrice@115::current
      IL_0094:  ldc.i4.0
      IL_0095:  ret
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
      // Code size       232 (0xe8)
      .maxstack  9
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [System.Runtime]System.Decimal V_1,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable> V_4,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal> V_5,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_6,
               string V_7,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_8,
               string V_9,
               class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_10,
               valuetype [System.Runtime]System.Decimal V_11,
               valuetype [System.Runtime]System.Decimal V_12,
               int32 V_13,
               string V_14,
               valuetype [System.Runtime]System.Decimal V_15,
               int32 V_16,
               class [System.Runtime]System.IDisposable V_17)
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
      IL_002a:  ldstr      "source"
      IL_002f:  stloc.s    V_7
      IL_0031:  ldloc.s    V_6
      IL_0033:  stloc.s    V_8
      IL_0035:  ldloc.s    V_8
      IL_0037:  box        class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_003c:  brtrue.s   IL_004a

      IL_003e:  ldloc.s    V_7
      IL_0040:  stloc.s    V_9
      IL_0042:  ldloc.s    V_9
      IL_0044:  newobj     instance void [netstandard]System.ArgumentNullException::.ctor(string)
      IL_0049:  throw

      IL_004a:  nop
      IL_004b:  ldloc.s    V_6
      IL_004d:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0052:  stloc.s    V_10
      .try
      {
        IL_0054:  ldc.i4.0
        IL_0055:  ldc.i4.0
        IL_0056:  ldc.i4.0
        IL_0057:  ldc.i4.0
        IL_0058:  ldc.i4.0
        IL_0059:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                              int32,
                                                                              int32,
                                                                              bool,
                                                                              uint8)
        IL_005e:  stloc.s    V_12
        IL_0060:  ldc.i4.0
        IL_0061:  stloc.s    V_13
        IL_0063:  ldloc.s    V_10
        IL_0065:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
        IL_006a:  brfalse.s  IL_008c

        IL_006c:  ldloc.s    V_12
        IL_006e:  ldloc.s    V_5
        IL_0070:  ldloc.s    V_10
        IL_0072:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_0077:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [System.Runtime]System.Decimal>::Invoke(!0)
        IL_007c:  call       valuetype [netstandard]System.Decimal [netstandard]System.Decimal::op_Addition(valuetype [netstandard]System.Decimal,
                                                                                                            valuetype [netstandard]System.Decimal)
        IL_0081:  stloc.s    V_12
        IL_0083:  ldloc.s    V_13
        IL_0085:  ldc.i4.1
        IL_0086:  add
        IL_0087:  stloc.s    V_13
        IL_0089:  nop
        IL_008a:  br.s       IL_0063

        IL_008c:  ldloc.s    V_13
        IL_008e:  brtrue.s   IL_009f

        IL_0090:  ldstr      "source"
        IL_0095:  stloc.s    V_14
        IL_0097:  ldloc.s    V_14
        IL_0099:  newobj     instance void [netstandard]System.InvalidOperationException::.ctor(string)
        IL_009e:  throw

        IL_009f:  nop
        IL_00a0:  ldloc.s    V_12
        IL_00a2:  stloc.s    V_15
        IL_00a4:  ldloc.s    V_13
        IL_00a6:  stloc.s    V_16
        IL_00a8:  ldloc.s    V_15
        IL_00aa:  ldloc.s    V_16
        IL_00ac:  call       valuetype [netstandard]System.Decimal [netstandard]System.Convert::ToDecimal(int32)
        IL_00b1:  call       valuetype [netstandard]System.Decimal [netstandard]System.Decimal::Divide(valuetype [netstandard]System.Decimal,
                                                                                                       valuetype [netstandard]System.Decimal)
        IL_00b6:  stloc.s    V_11
        IL_00b8:  leave.s    IL_00d0

      }  // end .try
      finally
      {
        IL_00ba:  ldloc.s    V_10
        IL_00bc:  isinst     [System.Runtime]System.IDisposable
        IL_00c1:  stloc.s    V_17
        IL_00c3:  ldloc.s    V_17
        IL_00c5:  brfalse.s  IL_00cf

        IL_00c7:  ldloc.s    V_17
        IL_00c9:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
        IL_00ce:  endfinally
        IL_00cf:  endfinally
      }  // end handler
      IL_00d0:  ldloc.s    V_11
      IL_00d2:  stloc.1
      IL_00d3:  ldarg.0
      IL_00d4:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Aggregates01/'Pipe #7 input at line 112@114-3'::builder@
      IL_00d9:  ldloc.0
      IL_00da:  ldloc.1
      IL_00db:  newobj     instance void class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                             !1)
      IL_00e0:  tail.
      IL_00e2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(!!0)
      IL_00e7:  ret
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
    // Code size       1789 (0x6fd)
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
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_26,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_27,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> V_28,
             int32 V_29,
             int32 V_30,
             class [System.Runtime]System.IDisposable V_31,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_32,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_33,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable> V_34,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> V_35,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> V_36,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> V_37,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> V_38,
             int32 V_39,
             int32 V_40,
             class [System.Runtime]System.IDisposable V_41,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,int32>> V_42,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_43,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>> V_44,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_45,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>> V_46,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_47,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>> V_48,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_49,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>> V_50,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_51,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_52,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_53,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [System.Runtime]System.Collections.IEnumerable> V_54,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64> V_55,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<float64> V_56,
             string V_57,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<float64> V_58,
             string V_59,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64> V_60,
             float64 V_61,
             float64 V_62,
             int32 V_63,
             string V_64,
             float64 V_65,
             int32 V_66,
             class [System.Runtime]System.IDisposable V_67,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_68,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_69,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,float64>,class [System.Runtime]System.Collections.IEnumerable> V_70,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<string,float64>,float64> V_71,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,float64>> V_72,
             string V_73,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,float64>> V_74,
             string V_75,
             class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [System.Runtime]System.Tuple`2<string,float64>> V_76,
             float64 V_77,
             float64 V_78,
             int32 V_79,
             string V_80,
             float64 V_81,
             int32 V_82,
             class [System.Runtime]System.IDisposable V_83,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>> V_84,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_85)
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
    IL_00c4:  ldloc.s    V_25
    IL_00c6:  stloc.s    V_26
    IL_00c8:  ldloc.s    V_24
    IL_00ca:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_00cf:  stloc.s    V_27
    IL_00d1:  ldloc.s    V_27
    IL_00d3:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_00d8:  stloc.s    V_28
    .try
    {
      IL_00da:  ldc.i4.0
      IL_00db:  stloc.s    V_30
      IL_00dd:  ldloc.s    V_28
      IL_00df:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_00e4:  brfalse.s  IL_00fc

      IL_00e6:  ldloc.s    V_30
      IL_00e8:  ldloc.s    V_26
      IL_00ea:  ldloc.s    V_28
      IL_00ec:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_00f1:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_00f6:  add.ovf
      IL_00f7:  stloc.s    V_30
      IL_00f9:  nop
      IL_00fa:  br.s       IL_00dd

      IL_00fc:  ldloc.s    V_30
      IL_00fe:  stloc.s    V_29
      IL_0100:  leave.s    IL_0118

    }  // end .try
    finally
    {
      IL_0102:  ldloc.s    V_28
      IL_0104:  isinst     [System.Runtime]System.IDisposable
      IL_0109:  stloc.s    V_31
      IL_010b:  ldloc.s    V_31
      IL_010d:  brfalse.s  IL_0117

      IL_010f:  ldloc.s    V_31
      IL_0111:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_0116:  endfinally
      IL_0117:  endfinally
    }  // end handler
    IL_0118:  ldloc.s    V_29
    IL_011a:  dup
    IL_011b:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numSum@19
    IL_0120:  stloc.3
    IL_0121:  ldstr      "cherry"
    IL_0126:  ldstr      "apple"
    IL_012b:  ldstr      "blueberry"
    IL_0130:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0135:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0144:  dup
    IL_0145:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::words@26
    IL_014a:  stloc.s    V_4
    IL_014c:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0151:  stloc.s    V_32
    IL_0153:  ldloc.s    V_32
    IL_0155:  stloc.s    V_33
    IL_0157:  ldnull
    IL_0158:  ldc.i4.0
    IL_0159:  ldnull
    IL_015a:  newobj     instance void Linq101Aggregates01/totalChars@30::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
    IL_015f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0164:  stloc.s    V_34
    IL_0166:  ldsfld     class Linq101Aggregates01/'totalChars@31-1' Linq101Aggregates01/'totalChars@31-1'::@_instance
    IL_016b:  stloc.s    V_35
    IL_016d:  ldloc.s    V_35
    IL_016f:  stloc.s    V_36
    IL_0171:  ldloc.s    V_34
    IL_0173:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0178:  stloc.s    V_37
    IL_017a:  ldloc.s    V_37
    IL_017c:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_0181:  stloc.s    V_38
    .try
    {
      IL_0183:  ldc.i4.0
      IL_0184:  stloc.s    V_40
      IL_0186:  ldloc.s    V_38
      IL_0188:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_018d:  brfalse.s  IL_01a5

      IL_018f:  ldloc.s    V_40
      IL_0191:  ldloc.s    V_36
      IL_0193:  ldloc.s    V_38
      IL_0195:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_019a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::Invoke(!0)
      IL_019f:  add.ovf
      IL_01a0:  stloc.s    V_40
      IL_01a2:  nop
      IL_01a3:  br.s       IL_0186

      IL_01a5:  ldloc.s    V_40
      IL_01a7:  stloc.s    V_39
      IL_01a9:  leave.s    IL_01c1

    }  // end .try
    finally
    {
      IL_01ab:  ldloc.s    V_38
      IL_01ad:  isinst     [System.Runtime]System.IDisposable
      IL_01b2:  stloc.s    V_41
      IL_01b4:  ldloc.s    V_41
      IL_01b6:  brfalse.s  IL_01c0

      IL_01b8:  ldloc.s    V_41
      IL_01ba:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_01bf:  endfinally
      IL_01c0:  endfinally
    }  // end handler
    IL_01c1:  ldloc.s    V_39
    IL_01c3:  dup
    IL_01c4:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::totalChars@28
    IL_01c9:  stloc.s    V_5
    IL_01cb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_01d0:  dup
    IL_01d1:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::products@35
    IL_01d6:  stloc.s    V_6
    IL_01d8:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01dd:  stloc.s    V_43
    IL_01df:  ldloc.s    V_43
    IL_01e1:  ldloc.s    V_43
    IL_01e3:  ldloc.s    V_43
    IL_01e5:  ldloc.s    V_43
    IL_01e7:  ldloc.s    V_43
    IL_01e9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_01ee:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01f3:  ldloc.s    V_43
    IL_01f5:  newobj     instance void Linq101Aggregates01/'Pipe #2 input at line 38@39'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01fa:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01ff:  ldsfld     class Linq101Aggregates01/'Pipe #2 input at line 38@40-1' Linq101Aggregates01/'Pipe #2 input at line 38@40-1'::@_instance
    IL_0204:  ldsfld     class Linq101Aggregates01/'Pipe #2 input at line 38@40-2' Linq101Aggregates01/'Pipe #2 input at line 38@40-2'::@_instance
    IL_0209:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_020e:  ldloc.s    V_43
    IL_0210:  newobj     instance void Linq101Aggregates01/'Pipe #2 input at line 38@40-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0215:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_021a:  ldsfld     class Linq101Aggregates01/'Pipe #2 input at line 38@45-4' Linq101Aggregates01/'Pipe #2 input at line 38@45-4'::@_instance
    IL_021f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0224:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,int32>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0229:  stloc.s    V_42
    IL_022b:  ldloc.s    V_42
    IL_022d:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,int32>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0232:  dup
    IL_0233:  stsfld     class [System.Runtime]System.Tuple`2<string,int32>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories@37
    IL_0238:  stloc.s    V_7
    IL_023a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_023f:  ldnull
    IL_0240:  ldc.i4.0
    IL_0241:  ldc.i4.0
    IL_0242:  newobj     instance void Linq101Aggregates01/minNum@49::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0247:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_024c:  ldsfld     class Linq101Aggregates01/'minNum@49-1' Linq101Aggregates01/'minNum@49-1'::@_instance
    IL_0251:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<int32,class [System.Runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0256:  dup
    IL_0257:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::minNum@49
    IL_025c:  stloc.s    V_8
    IL_025e:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0263:  ldnull
    IL_0264:  ldc.i4.0
    IL_0265:  ldnull
    IL_0266:  newobj     instance void Linq101Aggregates01/shortestWord@52::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
    IL_026b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0270:  ldsfld     class Linq101Aggregates01/'shortestWord@52-1' Linq101Aggregates01/'shortestWord@52-1'::@_instance
    IL_0275:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<string,class [System.Runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_027a:  dup
    IL_027b:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::shortestWord@52
    IL_0280:  stloc.s    V_9
    IL_0282:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0287:  stloc.s    V_45
    IL_0289:  ldloc.s    V_45
    IL_028b:  ldloc.s    V_45
    IL_028d:  ldloc.s    V_45
    IL_028f:  ldloc.s    V_45
    IL_0291:  ldloc.s    V_45
    IL_0293:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_0298:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_029d:  ldloc.s    V_45
    IL_029f:  newobj     instance void Linq101Aggregates01/'Pipe #3 input at line 56@57'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02a4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02a9:  ldsfld     class Linq101Aggregates01/'Pipe #3 input at line 56@58-1' Linq101Aggregates01/'Pipe #3 input at line 56@58-1'::@_instance
    IL_02ae:  ldsfld     class Linq101Aggregates01/'Pipe #3 input at line 56@58-2' Linq101Aggregates01/'Pipe #3 input at line 56@58-2'::@_instance
    IL_02b3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_02b8:  ldloc.s    V_45
    IL_02ba:  newobj     instance void Linq101Aggregates01/'Pipe #3 input at line 56@58-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02bf:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02c4:  ldsfld     class Linq101Aggregates01/'Pipe #3 input at line 56@60-4' Linq101Aggregates01/'Pipe #3 input at line 56@60-4'::@_instance
    IL_02c9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_02ce:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_02d3:  stloc.s    V_44
    IL_02d5:  ldloc.s    V_44
    IL_02d7:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02dc:  dup
    IL_02dd:  stsfld     class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories2@55
    IL_02e2:  stloc.s    V_10
    IL_02e4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_02e9:  stloc.s    V_47
    IL_02eb:  ldloc.s    V_47
    IL_02ed:  ldloc.s    V_47
    IL_02ef:  ldloc.s    V_47
    IL_02f1:  ldloc.s    V_47
    IL_02f3:  ldloc.s    V_47
    IL_02f5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_02fa:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02ff:  ldloc.s    V_47
    IL_0301:  newobj     instance void Linq101Aggregates01/'Pipe #4 input at line 65@66'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0306:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_030b:  ldsfld     class Linq101Aggregates01/'Pipe #4 input at line 65@67-1' Linq101Aggregates01/'Pipe #4 input at line 65@67-1'::@_instance
    IL_0310:  ldsfld     class Linq101Aggregates01/'Pipe #4 input at line 65@67-2' Linq101Aggregates01/'Pipe #4 input at line 65@67-2'::@_instance
    IL_0315:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_031a:  ldloc.s    V_47
    IL_031c:  newobj     instance void Linq101Aggregates01/'Pipe #4 input at line 65@67-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0321:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0326:  ldsfld     class Linq101Aggregates01/'Pipe #4 input at line 65@70-4' Linq101Aggregates01/'Pipe #4 input at line 65@70-4'::@_instance
    IL_032b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0330:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0335:  stloc.s    V_46
    IL_0337:  ldloc.s    V_46
    IL_0339:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_033e:  dup
    IL_033f:  stsfld     class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories3@64
    IL_0344:  stloc.s    V_11
    IL_0346:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_034b:  ldnull
    IL_034c:  ldc.i4.0
    IL_034d:  ldc.i4.0
    IL_034e:  newobj     instance void Linq101Aggregates01/maxNum@74::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0353:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0358:  ldsfld     class Linq101Aggregates01/'maxNum@74-1' Linq101Aggregates01/'maxNum@74-1'::@_instance
    IL_035d:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<int32,class [System.Runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0362:  dup
    IL_0363:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::maxNum@74
    IL_0368:  stloc.s    V_12
    IL_036a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_036f:  ldnull
    IL_0370:  ldc.i4.0
    IL_0371:  ldnull
    IL_0372:  newobj     instance void Linq101Aggregates01/longestLength@77::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                   int32,
                                                                                   string)
    IL_0377:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_037c:  ldsfld     class Linq101Aggregates01/'longestLength@77-1' Linq101Aggregates01/'longestLength@77-1'::@_instance
    IL_0381:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<string,class [System.Runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0386:  dup
    IL_0387:  stsfld     int32 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::longestLength@77
    IL_038c:  stloc.s    V_13
    IL_038e:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0393:  stloc.s    V_49
    IL_0395:  ldloc.s    V_49
    IL_0397:  ldloc.s    V_49
    IL_0399:  ldloc.s    V_49
    IL_039b:  ldloc.s    V_49
    IL_039d:  ldloc.s    V_49
    IL_039f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_03a4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03a9:  ldloc.s    V_49
    IL_03ab:  newobj     instance void Linq101Aggregates01/'Pipe #5 input at line 81@82'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03b0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03b5:  ldsfld     class Linq101Aggregates01/'Pipe #5 input at line 81@83-1' Linq101Aggregates01/'Pipe #5 input at line 81@83-1'::@_instance
    IL_03ba:  ldsfld     class Linq101Aggregates01/'Pipe #5 input at line 81@83-2' Linq101Aggregates01/'Pipe #5 input at line 81@83-2'::@_instance
    IL_03bf:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_03c4:  ldloc.s    V_49
    IL_03c6:  newobj     instance void Linq101Aggregates01/'Pipe #5 input at line 81@83-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03cb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03d0:  ldsfld     class Linq101Aggregates01/'Pipe #5 input at line 81@85-4' Linq101Aggregates01/'Pipe #5 input at line 81@85-4'::@_instance
    IL_03d5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03da:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_03df:  stloc.s    V_48
    IL_03e1:  ldloc.s    V_48
    IL_03e3:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03e8:  dup
    IL_03e9:  stsfld     class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories4@80
    IL_03ee:  stloc.s    V_14
    IL_03f0:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03f5:  stloc.s    V_51
    IL_03f7:  ldloc.s    V_51
    IL_03f9:  ldloc.s    V_51
    IL_03fb:  ldloc.s    V_51
    IL_03fd:  ldloc.s    V_51
    IL_03ff:  ldloc.s    V_51
    IL_0401:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_0406:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_040b:  ldloc.s    V_51
    IL_040d:  newobj     instance void Linq101Aggregates01/'Pipe #6 input at line 90@91'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0412:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0417:  ldsfld     class Linq101Aggregates01/'Pipe #6 input at line 90@92-1' Linq101Aggregates01/'Pipe #6 input at line 90@92-1'::@_instance
    IL_041c:  ldsfld     class Linq101Aggregates01/'Pipe #6 input at line 90@92-2' Linq101Aggregates01/'Pipe #6 input at line 90@92-2'::@_instance
    IL_0421:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0426:  ldloc.s    V_51
    IL_0428:  newobj     instance void Linq101Aggregates01/'Pipe #6 input at line 90@92-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_042d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0432:  ldsfld     class Linq101Aggregates01/'Pipe #6 input at line 90@95-4' Linq101Aggregates01/'Pipe #6 input at line 90@95-4'::@_instance
    IL_0437:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`3<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_043c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0441:  stloc.s    V_50
    IL_0443:  ldloc.s    V_50
    IL_0445:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_044a:  dup
    IL_044b:  stsfld     class [System.Runtime]System.Tuple`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories5@89
    IL_0450:  stloc.s    V_15
    IL_0452:  ldc.r8     5.0999999999999996
    IL_045b:  ldc.r8     4.0999999999999996
    IL_0464:  ldc.r8     1.1000000000000001
    IL_046d:  ldc.r8     3.1000000000000001
    IL_0476:  ldc.r8     9.0999999999999996
    IL_047f:  ldc.r8     8.0999999999999996
    IL_0488:  ldc.r8     6.0999999999999996
    IL_0491:  ldc.r8     7.0999999999999996
    IL_049a:  ldc.r8     2.1000000000000001
    IL_04a3:  ldc.r8     0.10000000000000001
    IL_04ac:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
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
    IL_04d9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04de:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04e3:  dup
    IL_04e4:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::numbers2@99
    IL_04e9:  stloc.s    V_16
    IL_04eb:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_04f0:  stloc.s    V_52
    IL_04f2:  ldloc.s    V_52
    IL_04f4:  stloc.s    V_53
    IL_04f6:  ldnull
    IL_04f7:  ldc.i4.0
    IL_04f8:  ldc.r8     0.0
    IL_0501:  newobj     instance void Linq101Aggregates01/averageNum@100::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                 int32,
                                                                                 float64)
    IL_0506:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_050b:  stloc.s    V_54
    IL_050d:  ldsfld     class Linq101Aggregates01/'averageNum@100-1' Linq101Aggregates01/'averageNum@100-1'::@_instance
    IL_0512:  stloc.s    V_55
    IL_0514:  ldloc.s    V_54
    IL_0516:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_051b:  stloc.s    V_56
    IL_051d:  ldstr      "source"
    IL_0522:  stloc.s    V_57
    IL_0524:  ldloc.s    V_56
    IL_0526:  stloc.s    V_58
    IL_0528:  ldloc.s    V_58
    IL_052a:  box        class [System.Runtime]System.Collections.Generic.IEnumerable`1<float64>
    IL_052f:  brtrue.s   IL_053d

    IL_0531:  ldloc.s    V_57
    IL_0533:  stloc.s    V_59
    IL_0535:  ldloc.s    V_59
    IL_0537:  newobj     instance void [netstandard]System.ArgumentNullException::.ctor(string)
    IL_053c:  throw

    IL_053d:  nop
    IL_053e:  ldloc.s    V_56
    IL_0540:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_0545:  stloc.s    V_60
    .try
    {
      IL_0547:  ldc.r8     0.0
      IL_0550:  stloc.s    V_62
      IL_0552:  ldc.i4.0
      IL_0553:  stloc.s    V_63
      IL_0555:  ldloc.s    V_60
      IL_0557:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_055c:  brfalse.s  IL_057a

      IL_055e:  ldloc.s    V_62
      IL_0560:  ldloc.s    V_55
      IL_0562:  ldloc.s    V_60
      IL_0564:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_0569:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::Invoke(!0)
      IL_056e:  add
      IL_056f:  stloc.s    V_62
      IL_0571:  ldloc.s    V_63
      IL_0573:  ldc.i4.1
      IL_0574:  add
      IL_0575:  stloc.s    V_63
      IL_0577:  nop
      IL_0578:  br.s       IL_0555

      IL_057a:  ldloc.s    V_63
      IL_057c:  brtrue.s   IL_058d

      IL_057e:  ldstr      "source"
      IL_0583:  stloc.s    V_64
      IL_0585:  ldloc.s    V_64
      IL_0587:  newobj     instance void [netstandard]System.InvalidOperationException::.ctor(string)
      IL_058c:  throw

      IL_058d:  nop
      IL_058e:  ldloc.s    V_62
      IL_0590:  stloc.s    V_65
      IL_0592:  ldloc.s    V_63
      IL_0594:  stloc.s    V_66
      IL_0596:  ldloc.s    V_65
      IL_0598:  ldloc.s    V_66
      IL_059a:  conv.r8
      IL_059b:  div
      IL_059c:  stloc.s    V_61
      IL_059e:  leave.s    IL_05b6

    }  // end .try
    finally
    {
      IL_05a0:  ldloc.s    V_60
      IL_05a2:  isinst     [System.Runtime]System.IDisposable
      IL_05a7:  stloc.s    V_67
      IL_05a9:  ldloc.s    V_67
      IL_05ab:  brfalse.s  IL_05b5

      IL_05ad:  ldloc.s    V_67
      IL_05af:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_05b4:  endfinally
      IL_05b5:  endfinally
    }  // end handler
    IL_05b6:  ldloc.s    V_61
    IL_05b8:  dup
    IL_05b9:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageNum@100
    IL_05be:  stloc.s    V_17
    IL_05c0:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_05c5:  stloc.s    V_68
    IL_05c7:  ldloc.s    V_68
    IL_05c9:  stloc.s    V_69
    IL_05cb:  ldloc.s    V_68
    IL_05cd:  ldloc.s    V_68
    IL_05cf:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Aggregates01::get_words()
    IL_05d4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_05d9:  ldloc.s    V_68
    IL_05db:  newobj     instance void Linq101Aggregates01/averageLength@105::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_05e0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,float64>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_05e5:  stloc.s    V_70
    IL_05e7:  ldsfld     class Linq101Aggregates01/'averageLength@107-1' Linq101Aggregates01/'averageLength@107-1'::@_instance
    IL_05ec:  stloc.s    V_71
    IL_05ee:  ldloc.s    V_70
    IL_05f0:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,float64>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_05f5:  stloc.s    V_72
    IL_05f7:  ldstr      "source"
    IL_05fc:  stloc.s    V_73
    IL_05fe:  ldloc.s    V_72
    IL_0600:  stloc.s    V_74
    IL_0602:  ldloc.s    V_74
    IL_0604:  box        class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,float64>>
    IL_0609:  brtrue.s   IL_0617

    IL_060b:  ldloc.s    V_73
    IL_060d:  stloc.s    V_75
    IL_060f:  ldloc.s    V_75
    IL_0611:  newobj     instance void [netstandard]System.ArgumentNullException::.ctor(string)
    IL_0616:  throw

    IL_0617:  nop
    IL_0618:  ldloc.s    V_72
    IL_061a:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,float64>>::GetEnumerator()
    IL_061f:  stloc.s    V_76
    .try
    {
      IL_0621:  ldc.r8     0.0
      IL_062a:  stloc.s    V_78
      IL_062c:  ldc.i4.0
      IL_062d:  stloc.s    V_79
      IL_062f:  ldloc.s    V_76
      IL_0631:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_0636:  brfalse.s  IL_0654

      IL_0638:  ldloc.s    V_78
      IL_063a:  ldloc.s    V_71
      IL_063c:  ldloc.s    V_76
      IL_063e:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<class [System.Runtime]System.Tuple`2<string,float64>>::get_Current()
      IL_0643:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<string,float64>,float64>::Invoke(!0)
      IL_0648:  add
      IL_0649:  stloc.s    V_78
      IL_064b:  ldloc.s    V_79
      IL_064d:  ldc.i4.1
      IL_064e:  add
      IL_064f:  stloc.s    V_79
      IL_0651:  nop
      IL_0652:  br.s       IL_062f

      IL_0654:  ldloc.s    V_79
      IL_0656:  brtrue.s   IL_0667

      IL_0658:  ldstr      "source"
      IL_065d:  stloc.s    V_80
      IL_065f:  ldloc.s    V_80
      IL_0661:  newobj     instance void [netstandard]System.InvalidOperationException::.ctor(string)
      IL_0666:  throw

      IL_0667:  nop
      IL_0668:  ldloc.s    V_78
      IL_066a:  stloc.s    V_81
      IL_066c:  ldloc.s    V_79
      IL_066e:  stloc.s    V_82
      IL_0670:  ldloc.s    V_81
      IL_0672:  ldloc.s    V_82
      IL_0674:  conv.r8
      IL_0675:  div
      IL_0676:  stloc.s    V_77
      IL_0678:  leave.s    IL_0690

    }  // end .try
    finally
    {
      IL_067a:  ldloc.s    V_76
      IL_067c:  isinst     [System.Runtime]System.IDisposable
      IL_0681:  stloc.s    V_83
      IL_0683:  ldloc.s    V_83
      IL_0685:  brfalse.s  IL_068f

      IL_0687:  ldloc.s    V_83
      IL_0689:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_068e:  endfinally
      IL_068f:  endfinally
    }  // end handler
    IL_0690:  ldloc.s    V_77
    IL_0692:  dup
    IL_0693:  stsfld     float64 '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::averageLength@103
    IL_0698:  stloc.s    V_18
    IL_069a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_069f:  stloc.s    V_85
    IL_06a1:  ldloc.s    V_85
    IL_06a3:  ldloc.s    V_85
    IL_06a5:  ldloc.s    V_85
    IL_06a7:  ldloc.s    V_85
    IL_06a9:  ldloc.s    V_85
    IL_06ab:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Aggregates01::get_products()
    IL_06b0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_06b5:  ldloc.s    V_85
    IL_06b7:  newobj     instance void Linq101Aggregates01/'Pipe #7 input at line 112@113'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_06bc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_06c1:  ldsfld     class Linq101Aggregates01/'Pipe #7 input at line 112@114-1' Linq101Aggregates01/'Pipe #7 input at line 112@114-1'::@_instance
    IL_06c6:  ldsfld     class Linq101Aggregates01/'Pipe #7 input at line 112@114-2' Linq101Aggregates01/'Pipe #7 input at line 112@114-2'::@_instance
    IL_06cb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_06d0:  ldloc.s    V_85
    IL_06d2:  newobj     instance void Linq101Aggregates01/'Pipe #7 input at line 112@114-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_06d7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_06dc:  ldsfld     class Linq101Aggregates01/'Pipe #7 input at line 112@116-4' Linq101Aggregates01/'Pipe #7 input at line 112@116-4'::@_instance
    IL_06e1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_06e6:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_06eb:  stloc.s    V_84
    IL_06ed:  ldloc.s    V_84
    IL_06ef:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_06f4:  dup
    IL_06f5:  stsfld     class [System.Runtime]System.Tuple`2<string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01::categories6@111
    IL_06fa:  stloc.s    V_19
    IL_06fc:  ret
  } // end of method $Linq101Aggregates01::main@

} // end of class '<StartupCode$Linq101Aggregates01>'.$Linq101Aggregates01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\debug\net6.0\tests\EmittedIL\QueryExpressionStepping\Linq101Aggregates01_fs\Linq101Aggregates01.res
