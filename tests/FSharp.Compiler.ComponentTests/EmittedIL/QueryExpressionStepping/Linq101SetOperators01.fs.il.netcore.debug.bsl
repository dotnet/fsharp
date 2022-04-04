
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
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:1:0:0
}
.assembly Linq101SetOperators01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101SetOperators01
{
  // Offset: 0x00000000 Length: 0x000003C0
  // WARNING: managed resource file FSharpSignatureData.Linq101SetOperators01 created
}
.mresource public FSharpOptimizationData.Linq101SetOperators01
{
  // Offset: 0x000003C8 Length: 0x0000011E
  // WARNING: managed resource file FSharpOptimizationData.Linq101SetOperators01 created
}
.module Linq101SetOperators01.exe
// MVID: {62466677-47B8-3AF1-A745-038377664662}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000002854FCD0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101SetOperators01
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #1 input at line 12@13'
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
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101SetOperators01/'Pipe #1 input at line 12@13'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #1 input at line 12@13'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       149 (0x95)
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
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
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101SetOperators01::get_factorsOf300()
      IL_002c:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101SetOperators01/'Pipe #1 input at line 12@13'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101SetOperators01/'Pipe #1 input at line 12@13'::'enum'
      IL_0043:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0048:  brfalse.s  IL_006b

      IL_004a:  ldarg.0
      IL_004b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101SetOperators01/'Pipe #1 input at line 12@13'::'enum'
      IL_0050:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0055:  stloc.0
      IL_0056:  ldloc.0
      IL_0057:  stloc.1
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
      IL_005f:  ldarg.0
      IL_0060:  ldloc.1
      IL_0061:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::current
      IL_0066:  ldc.i4.1
      IL_0067:  ret

      IL_0068:  nop
      IL_0069:  br.s       IL_003d

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101SetOperators01/'Pipe #1 input at line 12@13'::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101SetOperators01/'Pipe #1 input at line 12@13'::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
      IL_008c:  ldarg.0
      IL_008d:  ldc.i4.0
      IL_008e:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } // end of method 'Pipe #1 input at line 12@13'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
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
        IL_0018:  ldfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
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
        IL_0044:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101SetOperators01/'Pipe #1 input at line 12@13'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::current
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
    } // end of method 'Pipe #1 input at line 12@13'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::pc
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
    } // end of method 'Pipe #1 input at line 12@13'::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/'Pipe #1 input at line 12@13'::current
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 12@13'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101SetOperators01/'Pipe #1 input at line 12@13'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                    int32,
                                                                                                    int32)
      IL_0008:  ret
    } // end of method 'Pipe #1 input at line 12@13'::GetFreshEnumerator

  } // end of class 'Pipe #1 input at line 12@13'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 21@22-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>
  {
    .field static assembly initonly class Linq101SetOperators01/'Pipe #2 input at line 21@22-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 21@22-1'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<class [Utils]Utils/Product>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 21@22-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101SetOperators01/'Pipe #2 input at line 21@22-1'::.ctor()
      IL_0005:  stsfld     class Linq101SetOperators01/'Pipe #2 input at line 21@22-1' Linq101SetOperators01/'Pipe #2 input at line 21@22-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 21@22-1'::.cctor

  } // end of class 'Pipe #2 input at line 21@22-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #2 input at line 21@23'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
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
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/'Pipe #2 input at line 21@23'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101SetOperators01/'Pipe #2 input at line 21@23'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #2 input at line 21@23'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       161 (0xa1)
      .maxstack  7
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_0077

      IL_001e:  nop
      IL_001f:  br.s       IL_0074

      IL_0021:  nop
      IL_0022:  br.s       IL_0098

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldsfld     class Linq101SetOperators01/'Pipe #2 input at line 21@22-1' Linq101SetOperators01/'Pipe #2 input at line 21@22-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101SetOperators01::get_products()
      IL_0030:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                              class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/'Pipe #2 input at line 21@23'::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/'Pipe #2 input at line 21@23'::'enum'
      IL_004c:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brfalse.s  IL_0077

      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/'Pipe #2 input at line 21@23'::'enum'
      IL_0059:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005e:  stloc.0
      IL_005f:  ldarg.0
      IL_0060:  ldc.i4.2
      IL_0061:  stfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
      IL_0066:  ldarg.0
      IL_0067:  ldloc.0
      IL_0068:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_006d:  stfld      string Linq101SetOperators01/'Pipe #2 input at line 21@23'::current
      IL_0072:  ldc.i4.1
      IL_0073:  ret

      IL_0074:  nop
      IL_0075:  br.s       IL_0046

      IL_0077:  ldarg.0
      IL_0078:  ldc.i4.3
      IL_0079:  stfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
      IL_007e:  ldarg.0
      IL_007f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/'Pipe #2 input at line 21@23'::'enum'
      IL_0084:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_0089:  nop
      IL_008a:  ldarg.0
      IL_008b:  ldnull
      IL_008c:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/'Pipe #2 input at line 21@23'::'enum'
      IL_0091:  ldarg.0
      IL_0092:  ldc.i4.3
      IL_0093:  stfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
      IL_0098:  ldarg.0
      IL_0099:  ldnull
      IL_009a:  stfld      string Linq101SetOperators01/'Pipe #2 input at line 21@23'::current
      IL_009f:  ldc.i4.0
      IL_00a0:  ret
    } // end of method 'Pipe #2 input at line 21@23'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
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
        IL_0018:  ldfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
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
        IL_0044:  stfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/'Pipe #2 input at line 21@23'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101SetOperators01/'Pipe #2 input at line 21@23'::current
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
    } // end of method 'Pipe #2 input at line 21@23'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/'Pipe #2 input at line 21@23'::pc
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
    } // end of method 'Pipe #2 input at line 21@23'::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101SetOperators01/'Pipe #2 input at line 21@23'::current
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 21@23'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101SetOperators01/'Pipe #2 input at line 21@23'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                                    int32,
                                                                                                    string)
      IL_0008:  ret
    } // end of method 'Pipe #2 input at line 21@23'::GetFreshEnumerator

  } // end of class 'Pipe #2 input at line 21@23'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productFirstChars@32-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>
  {
    .field static assembly initonly class Linq101SetOperators01/'productFirstChars@32-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor()
      IL_0006:  ret
    } // end of method 'productFirstChars@32-1'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<class [Utils]Utils/Product>(!!0)
      IL_000a:  ret
    } // end of method 'productFirstChars@32-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101SetOperators01/'productFirstChars@32-1'::.ctor()
      IL_0005:  stsfld     class Linq101SetOperators01/'productFirstChars@32-1' Linq101SetOperators01/'productFirstChars@32-1'::@_instance
      IL_000a:  ret
    } // end of method 'productFirstChars@32-1'::.cctor

  } // end of class 'productFirstChars@32-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname productFirstChars@33
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<char>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public char current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 char current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/productFirstChars@33::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101SetOperators01/productFirstChars@33::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      char Linq101SetOperators01/productFirstChars@33::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<char>::.ctor()
      IL_001b:  ret
    } // end of method productFirstChars@33::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<char>& next) cil managed
    {
      // Code size       170 (0xaa)
      .maxstack  7
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/productFirstChars@33::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      IL_001b:  nop
      IL_001c:  br.s       IL_0080

      IL_001e:  nop
      IL_001f:  br.s       IL_007d

      IL_0021:  nop
      IL_0022:  br         IL_00a1

      IL_0027:  nop
      IL_0028:  ldarg.0
      IL_0029:  ldsfld     class Linq101SetOperators01/'productFirstChars@32-1' Linq101SetOperators01/'productFirstChars@32-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101SetOperators01::get_products()
      IL_0033:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                              class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003d:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/productFirstChars@33::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 Linq101SetOperators01/productFirstChars@33::pc
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/productFirstChars@33::'enum'
      IL_004f:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0054:  brfalse.s  IL_0080

      IL_0056:  ldarg.0
      IL_0057:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/productFirstChars@33::'enum'
      IL_005c:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0061:  stloc.0
      IL_0062:  ldarg.0
      IL_0063:  ldc.i4.2
      IL_0064:  stfld      int32 Linq101SetOperators01/productFirstChars@33::pc
      IL_0069:  ldarg.0
      IL_006a:  ldloc.0
      IL_006b:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0070:  ldc.i4.0
      IL_0071:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
      IL_0076:  stfld      char Linq101SetOperators01/productFirstChars@33::current
      IL_007b:  ldc.i4.1
      IL_007c:  ret

      IL_007d:  nop
      IL_007e:  br.s       IL_0049

      IL_0080:  ldarg.0
      IL_0081:  ldc.i4.3
      IL_0082:  stfld      int32 Linq101SetOperators01/productFirstChars@33::pc
      IL_0087:  ldarg.0
      IL_0088:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/productFirstChars@33::'enum'
      IL_008d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_0092:  nop
      IL_0093:  ldarg.0
      IL_0094:  ldnull
      IL_0095:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/productFirstChars@33::'enum'
      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101SetOperators01/productFirstChars@33::pc
      IL_00a1:  ldarg.0
      IL_00a2:  ldc.i4.0
      IL_00a3:  stfld      char Linq101SetOperators01/productFirstChars@33::current
      IL_00a8:  ldc.i4.0
      IL_00a9:  ret
    } // end of method productFirstChars@33::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/productFirstChars@33::pc
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
        IL_0018:  ldfld      int32 Linq101SetOperators01/productFirstChars@33::pc
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
        IL_0044:  stfld      int32 Linq101SetOperators01/productFirstChars@33::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101SetOperators01/productFirstChars@33::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101SetOperators01/productFirstChars@33::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      char Linq101SetOperators01/productFirstChars@33::current
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
    } // end of method productFirstChars@33::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/productFirstChars@33::pc
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
    } // end of method productFirstChars@33::get_CheckClose

    .method public strict virtual instance char 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char Linq101SetOperators01/productFirstChars@33::current
      IL_0006:  ret
    } // end of method productFirstChars@33::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<char> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101SetOperators01/productFirstChars@33::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                           int32,
                                                                                           char)
      IL_0008:  ret
    } // end of method productFirstChars@33::GetFreshEnumerator

  } // end of class productFirstChars@33

  .class auto ansi serializable sealed nested assembly beforefieldinit 'customerFirstChars@38-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Customer>>
  {
    .field static assembly initonly class Linq101SetOperators01/'customerFirstChars@38-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Customer>>::.ctor()
      IL_0006:  ret
    } // end of method 'customerFirstChars@38-1'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Customer> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<class [Utils]Utils/Customer>(!!0)
      IL_000a:  ret
    } // end of method 'customerFirstChars@38-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101SetOperators01/'customerFirstChars@38-1'::.ctor()
      IL_0005:  stsfld     class Linq101SetOperators01/'customerFirstChars@38-1' Linq101SetOperators01/'customerFirstChars@38-1'::@_instance
      IL_000a:  ret
    } // end of method 'customerFirstChars@38-1'::.cctor

  } // end of class 'customerFirstChars@38-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname customerFirstChars@39
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<char>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public char current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer> 'enum',
                                 int32 pc,
                                 char current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer> Linq101SetOperators01/customerFirstChars@39::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      char Linq101SetOperators01/customerFirstChars@39::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<char>::.ctor()
      IL_001b:  ret
    } // end of method customerFirstChars@39::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<char>& next) cil managed
    {
      // Code size       170 (0xaa)
      .maxstack  7
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      IL_001b:  nop
      IL_001c:  br.s       IL_0080

      IL_001e:  nop
      IL_001f:  br.s       IL_007d

      IL_0021:  nop
      IL_0022:  br         IL_00a1

      IL_0027:  nop
      IL_0028:  ldarg.0
      IL_0029:  ldsfld     class Linq101SetOperators01/'customerFirstChars@38-1' Linq101SetOperators01/'customerFirstChars@38-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101SetOperators01::get_customers()
      IL_0033:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Customer>,class [Utils]Utils/Customer>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                 class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Customer>::GetEnumerator()
      IL_003d:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer> Linq101SetOperators01/customerFirstChars@39::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer> Linq101SetOperators01/customerFirstChars@39::'enum'
      IL_004f:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0054:  brfalse.s  IL_0080

      IL_0056:  ldarg.0
      IL_0057:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer> Linq101SetOperators01/customerFirstChars@39::'enum'
      IL_005c:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer>::get_Current()
      IL_0061:  stloc.0
      IL_0062:  ldarg.0
      IL_0063:  ldc.i4.2
      IL_0064:  stfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
      IL_0069:  ldarg.0
      IL_006a:  ldloc.0
      IL_006b:  callvirt   instance string [Utils]Utils/Customer::get_CompanyName()
      IL_0070:  ldc.i4.0
      IL_0071:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
      IL_0076:  stfld      char Linq101SetOperators01/customerFirstChars@39::current
      IL_007b:  ldc.i4.1
      IL_007c:  ret

      IL_007d:  nop
      IL_007e:  br.s       IL_0049

      IL_0080:  ldarg.0
      IL_0081:  ldc.i4.3
      IL_0082:  stfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
      IL_0087:  ldarg.0
      IL_0088:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer> Linq101SetOperators01/customerFirstChars@39::'enum'
      IL_008d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer>>(!!0)
      IL_0092:  nop
      IL_0093:  ldarg.0
      IL_0094:  ldnull
      IL_0095:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer> Linq101SetOperators01/customerFirstChars@39::'enum'
      IL_009a:  ldarg.0
      IL_009b:  ldc.i4.3
      IL_009c:  stfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
      IL_00a1:  ldarg.0
      IL_00a2:  ldc.i4.0
      IL_00a3:  stfld      char Linq101SetOperators01/customerFirstChars@39::current
      IL_00a8:  ldc.i4.0
      IL_00a9:  ret
    } // end of method customerFirstChars@39::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
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
        IL_0018:  ldfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
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
        IL_0044:  stfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer> Linq101SetOperators01/customerFirstChars@39::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      char Linq101SetOperators01/customerFirstChars@39::current
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
    } // end of method customerFirstChars@39::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101SetOperators01/customerFirstChars@39::pc
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
    } // end of method customerFirstChars@39::get_CheckClose

    .method public strict virtual instance char 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char Linq101SetOperators01/customerFirstChars@39::current
      IL_0006:  ret
    } // end of method customerFirstChars@39::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<char> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101SetOperators01/customerFirstChars@39::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer>,
                                                                                            int32,
                                                                                            char)
      IL_0008:  ret
    } // end of method customerFirstChars@39::GetFreshEnumerator

  } // end of class customerFirstChars@39

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_factorsOf300() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::factorsOf300@9
    IL_0005:  ret
  } // end of method Linq101SetOperators01::get_factorsOf300

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_uniqueFactors() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::uniqueFactors@11
    IL_0005:  ret
  } // end of method Linq101SetOperators01::get_uniqueFactors

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::products@18
    IL_0005:  ret
  } // end of method Linq101SetOperators01::get_products

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_categoryNames() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::categoryNames@20
    IL_0005:  ret
  } // end of method Linq101SetOperators01::get_categoryNames

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> 
          get_customers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::customers@28
    IL_0005:  ret
  } // end of method Linq101SetOperators01::get_customers

  .method public specialname static class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> 
          get_productFirstChars() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::productFirstChars@30
    IL_0005:  ret
  } // end of method Linq101SetOperators01::get_productFirstChars

  .method public specialname static class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> 
          get_customerFirstChars() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::customerFirstChars@36
    IL_0005:  ret
  } // end of method Linq101SetOperators01::get_customerFirstChars

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          factorsOf300()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101SetOperators01::get_factorsOf300()
  } // end of property Linq101SetOperators01::factorsOf300
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          uniqueFactors()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101SetOperators01::get_uniqueFactors()
  } // end of property Linq101SetOperators01::uniqueFactors
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101SetOperators01::get_products()
  } // end of property Linq101SetOperators01::products
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          categoryNames()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101SetOperators01::get_categoryNames()
  } // end of property Linq101SetOperators01::categoryNames
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer>
          customers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101SetOperators01::get_customers()
  } // end of property Linq101SetOperators01::customers
  .property class [System.Runtime]System.Collections.Generic.IEnumerable`1<char>
          productFirstChars()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> Linq101SetOperators01::get_productFirstChars()
  } // end of property Linq101SetOperators01::productFirstChars
  .property class [System.Runtime]System.Collections.Generic.IEnumerable`1<char>
          customerFirstChars()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> Linq101SetOperators01::get_customerFirstChars()
  } // end of property Linq101SetOperators01::customerFirstChars
} // end of class Linq101SetOperators01

.class private abstract auto ansi sealed '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01
       extends [System.Runtime]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> factorsOf300@9
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> uniqueFactors@11
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@18
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> categoryNames@20
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers@28
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> productFirstChars@30
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> customerFirstChars@36
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       210 (0xd2)
    .maxstack  8
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> V_2,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_3,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> V_4,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> V_5,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> V_6,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_7,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_8,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> V_9,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12)
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
    IL_0024:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::factorsOf300@9
    IL_0029:  stloc.0
    IL_002a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_002f:  stloc.s    V_8
    IL_0031:  ldloc.s    V_8
    IL_0033:  ldnull
    IL_0034:  ldc.i4.0
    IL_0035:  ldc.i4.0
    IL_0036:  newobj     instance void Linq101SetOperators01/'Pipe #1 input at line 12@13'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                  int32,
                                                                                                  int32)
    IL_003b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0040:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Distinct<int32,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>)
    IL_0045:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_004a:  stloc.s    V_7
    IL_004c:  ldloc.s    V_7
    IL_004e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0053:  dup
    IL_0054:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::uniqueFactors@11
    IL_0059:  stloc.1
    IL_005a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_005f:  dup
    IL_0060:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::products@18
    IL_0065:  stloc.2
    IL_0066:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_006b:  stloc.s    V_10
    IL_006d:  ldloc.s    V_10
    IL_006f:  ldnull
    IL_0070:  ldc.i4.0
    IL_0071:  ldnull
    IL_0072:  newobj     instance void Linq101SetOperators01/'Pipe #2 input at line 21@23'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                                  int32,
                                                                                                  string)
    IL_0077:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_007c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Distinct<string,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>)
    IL_0081:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0086:  stloc.s    V_9
    IL_0088:  ldloc.s    V_9
    IL_008a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_008f:  dup
    IL_0090:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::categoryNames@20
    IL_0095:  stloc.3
    IL_0096:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_009b:  dup
    IL_009c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::customers@28
    IL_00a1:  stloc.s    V_4
    IL_00a3:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a8:  stloc.s    V_11
    IL_00aa:  ldnull
    IL_00ab:  ldc.i4.0
    IL_00ac:  ldc.i4.0
    IL_00ad:  newobj     instance void Linq101SetOperators01/productFirstChars@33::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                         int32,
                                                                                         char)
    IL_00b2:  dup
    IL_00b3:  stsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::productFirstChars@30
    IL_00b8:  stloc.s    V_5
    IL_00ba:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00bf:  stloc.s    V_12
    IL_00c1:  ldnull
    IL_00c2:  ldc.i4.0
    IL_00c3:  ldc.i4.0
    IL_00c4:  newobj     instance void Linq101SetOperators01/customerFirstChars@39::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Customer>,
                                                                                          int32,
                                                                                          char)
    IL_00c9:  dup
    IL_00ca:  stsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<char> '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01::customerFirstChars@36
    IL_00cf:  stloc.s    V_6
    IL_00d1:  ret
  } // end of method $Linq101SetOperators01::main@

} // end of class '<StartupCode$Linq101SetOperators01>'.$Linq101SetOperators01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\QueryExpressionStepping\Linq101SetOperators01_fs\Linq101SetOperators01.res
