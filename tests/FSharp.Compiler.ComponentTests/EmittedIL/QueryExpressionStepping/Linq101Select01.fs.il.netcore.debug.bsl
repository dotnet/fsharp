
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
.assembly Linq101Select01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Select01
{
  // Offset: 0x00000000 Length: 0x00000685
  // WARNING: managed resource file FSharpSignatureData.Linq101Select01 created
}
.mresource public FSharpOptimizationData.Linq101Select01
{
  // Offset: 0x00000690 Length: 0x00000204
  // WARNING: managed resource file FSharpOptimizationData.Linq101Select01 created
}
.module Linq101Select01.exe
// MVID: {62501638-5042-78D5-A745-038338165062}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000001404EF30000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Select01
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 11@12-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #1 input at line 11@12-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 11@12-1'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 11@12-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #1 input at line 11@12-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #1 input at line 11@12-1' Linq101Select01/'Pipe #1 input at line 11@12-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 11@12-1'::.cctor

  } // end of class 'Pipe #1 input at line 11@12-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #1 input at line 11@13'
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
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #1 input at line 11@13'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       157 (0x9d)
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_0073

      IL_001e:  nop
      IL_001f:  br.s       IL_0066

      IL_0021:  nop
      IL_0022:  br.s       IL_0094

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldsfld     class Linq101Select01/'Pipe #1 input at line 11@12-1' Linq101Select01/'Pipe #1 input at line 11@12-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
      IL_0030:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                               class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003a:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_0046:  br.s       IL_0066

      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_004e:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0053:  stloc.0
      IL_0054:  ldarg.0
      IL_0055:  ldc.i4.2
      IL_0056:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_005b:  ldarg.0
      IL_005c:  ldloc.0
      IL_005d:  ldc.i4.1
      IL_005e:  add
      IL_005f:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
      IL_0064:  ldc.i4.1
      IL_0065:  ret

      IL_0066:  ldarg.0
      IL_0067:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_006c:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0071:  brtrue.s   IL_0048

      IL_0073:  ldarg.0
      IL_0074:  ldc.i4.3
      IL_0075:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_007a:  ldarg.0
      IL_007b:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_0080:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0085:  nop
      IL_0086:  ldarg.0
      IL_0087:  ldnull
      IL_0088:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
      IL_008d:  ldarg.0
      IL_008e:  ldc.i4.3
      IL_008f:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
      IL_0094:  ldarg.0
      IL_0095:  ldc.i4.0
      IL_0096:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
      IL_009b:  ldc.i4.0
      IL_009c:  ret
    } // end of method 'Pipe #1 input at line 11@13'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #1 input at line 11@13'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
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
    } // end of method 'Pipe #1 input at line 11@13'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::pc
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
    } // end of method 'Pipe #1 input at line 11@13'::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #1 input at line 11@13'::current
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 11@13'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #1 input at line 11@13'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                              int32,
                                                                                              int32)
      IL_0008:  ret
    } // end of method 'Pipe #1 input at line 11@13'::GetFreshEnumerator

  } // end of class 'Pipe #1 input at line 11@13'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productNames@21-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>
  {
    .field static assembly initonly class Linq101Select01/'productNames@21-1' @_instance
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
    } // end of method 'productNames@21-1'::.ctor

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
    } // end of method 'productNames@21-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'productNames@21-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'productNames@21-1' Linq101Select01/'productNames@21-1'::@_instance
      IL_000a:  ret
    } // end of method 'productNames@21-1'::.cctor

  } // end of class 'productNames@21-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname productNames@22
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
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Select01/productNames@22::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method productNames@22::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       160 (0xa0)
      .maxstack  7
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/productNames@22::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_0076

      IL_001e:  nop
      IL_001f:  br.s       IL_0069

      IL_0021:  nop
      IL_0022:  br.s       IL_0097

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldsfld     class Linq101Select01/'productNames@21-1' Linq101Select01/'productNames@21-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Select01::get_products()
      IL_0030:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                              class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_0046:  br.s       IL_0069

      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_004e:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0053:  stloc.0
      IL_0054:  ldarg.0
      IL_0055:  ldc.i4.2
      IL_0056:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_005b:  ldarg.0
      IL_005c:  ldloc.0
      IL_005d:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0062:  stfld      string Linq101Select01/productNames@22::current
      IL_0067:  ldc.i4.1
      IL_0068:  ret

      IL_0069:  ldarg.0
      IL_006a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_006f:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0074:  brtrue.s   IL_0048

      IL_0076:  ldarg.0
      IL_0077:  ldc.i4.3
      IL_0078:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_007d:  ldarg.0
      IL_007e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_0083:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_0088:  nop
      IL_0089:  ldarg.0
      IL_008a:  ldnull
      IL_008b:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
      IL_0090:  ldarg.0
      IL_0091:  ldc.i4.3
      IL_0092:  stfld      int32 Linq101Select01/productNames@22::pc
      IL_0097:  ldarg.0
      IL_0098:  ldnull
      IL_0099:  stfld      string Linq101Select01/productNames@22::current
      IL_009e:  ldc.i4.0
      IL_009f:  ret
    } // end of method productNames@22::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/productNames@22::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/productNames@22::pc
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
        IL_0044:  stfld      int32 Linq101Select01/productNames@22::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/productNames@22::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/productNames@22::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Select01/productNames@22::current
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
    } // end of method productNames@22::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/productNames@22::pc
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
    } // end of method productNames@22::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Select01/productNames@22::current
      IL_0006:  ret
    } // end of method productNames@22::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Select01/productNames@22::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                string)
      IL_0008:  ret
    } // end of method productNames@22::GetFreshEnumerator

  } // end of class productNames@22

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 28@29-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #2 input at line 28@29-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 28@29-1'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 28@29-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #2 input at line 28@29-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #2 input at line 28@29-1' Linq101Select01/'Pipe #2 input at line 28@29-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 28@29-1'::.cctor

  } // end of class 'Pipe #2 input at line 28@29-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #2 input at line 28@30'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
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
    .field public string current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #2 input at line 28@30'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       165 (0xa5)
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_007b

      IL_001e:  nop
      IL_001f:  br.s       IL_006e

      IL_0021:  nop
      IL_0022:  br.s       IL_009c

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldsfld     class Linq101Select01/'Pipe #2 input at line 28@29-1' Linq101Select01/'Pipe #2 input at line 28@29-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
      IL_0030:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                               class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003a:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_0046:  br.s       IL_006e

      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_004e:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0053:  stloc.0
      IL_0054:  ldarg.0
      IL_0055:  ldc.i4.2
      IL_0056:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_005b:  ldarg.0
      IL_005c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_strings()
      IL_0061:  ldloc.0
      IL_0062:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Item(int32)
      IL_0067:  stfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
      IL_006c:  ldc.i4.1
      IL_006d:  ret

      IL_006e:  ldarg.0
      IL_006f:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_0074:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0079:  brtrue.s   IL_0048

      IL_007b:  ldarg.0
      IL_007c:  ldc.i4.3
      IL_007d:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_0082:  ldarg.0
      IL_0083:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_0088:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_008d:  nop
      IL_008e:  ldarg.0
      IL_008f:  ldnull
      IL_0090:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
      IL_009c:  ldarg.0
      IL_009d:  ldnull
      IL_009e:  stfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
      IL_00a3:  ldc.i4.0
      IL_00a4:  ret
    } // end of method 'Pipe #2 input at line 28@30'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #2 input at line 28@30'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
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
    } // end of method 'Pipe #2 input at line 28@30'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #2 input at line 28@30'::pc
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
    } // end of method 'Pipe #2 input at line 28@30'::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Select01/'Pipe #2 input at line 28@30'::current
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 28@30'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #2 input at line 28@30'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                              int32,
                                                                                              string)
      IL_0008:  ret
    } // end of method 'Pipe #2 input at line 28@30'::GetFreshEnumerator

  } // end of class 'Pipe #2 input at line 28@30'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 37@38-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #3 input at line 37@38-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 37@38-1'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> 
            Invoke(string _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init (string V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<string>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 37@38-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #3 input at line 37@38-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #3 input at line 37@38-1' Linq101Select01/'Pipe #3 input at line 37@38-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 37@38-1'::.cctor

  } // end of class 'Pipe #3 input at line 37@38-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #3 input at line 37@39'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [System.Runtime]System.Tuple`2<string,string>>
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
    .field public class [System.Runtime]System.Tuple`2<string,string> current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 class [System.Runtime]System.Tuple`2<string,string> current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [System.Runtime]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [System.Runtime]System.Tuple`2<string,string>>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #3 input at line 37@39'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,string>>& next) cil managed
    {
      // Code size       174 (0xae)
      .maxstack  7
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      IL_001b:  nop
      IL_001c:  br.s       IL_0084

      IL_001e:  nop
      IL_001f:  br.s       IL_0077

      IL_0021:  nop
      IL_0022:  br         IL_00a5

      IL_0027:  nop
      IL_0028:  ldarg.0
      IL_0029:  ldsfld     class Linq101Select01/'Pipe #3 input at line 37@38-1' Linq101Select01/'Pipe #3 input at line 37@38-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_words()
      IL_0033:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>,string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                  class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003d:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_0049:  br.s       IL_0077

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.2
      IL_0059:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_005e:  ldarg.0
      IL_005f:  ldloc.0
      IL_0060:  callvirt   instance string [System.Runtime]System.String::ToUpper()
      IL_0065:  ldloc.0
      IL_0066:  callvirt   instance string [System.Runtime]System.String::ToLower()
      IL_006b:  newobj     instance void class [System.Runtime]System.Tuple`2<string,string>::.ctor(!0,
                                                                                                    !1)
      IL_0070:  stfld      class [System.Runtime]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
      IL_0075:  ldc.i4.1
      IL_0076:  ret

      IL_0077:  ldarg.0
      IL_0078:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_007d:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0082:  brtrue.s   IL_004b

      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_008b:  ldarg.0
      IL_008c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_0091:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_0096:  nop
      IL_0097:  ldarg.0
      IL_0098:  ldnull
      IL_0099:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.3
      IL_00a0:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
      IL_00a5:  ldarg.0
      IL_00a6:  ldnull
      IL_00a7:  stfld      class [System.Runtime]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
      IL_00ac:  ldc.i4.0
      IL_00ad:  ret
    } // end of method 'Pipe #3 input at line 37@39'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Select01/'Pipe #3 input at line 37@39'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [System.Runtime]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
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
    } // end of method 'Pipe #3 input at line 37@39'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #3 input at line 37@39'::pc
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
    } // end of method 'Pipe #3 input at line 37@39'::get_CheckClose

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<string,string> 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Runtime]System.Tuple`2<string,string> Linq101Select01/'Pipe #3 input at line 37@39'::current
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 37@39'::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [System.Runtime]System.Tuple`2<string,string>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #3 input at line 37@39'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                              int32,
                                                                                              class [System.Runtime]System.Tuple`2<string,string>)
      IL_0008:  ret
    } // end of method 'Pipe #3 input at line 37@39'::GetFreshEnumerator

  } // end of class 'Pipe #3 input at line 37@39'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 44@45-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #4 input at line 44@45-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 44@45-1'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 44@45-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #4 input at line 44@45-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #4 input at line 44@45-1' Linq101Select01/'Pipe #4 input at line 44@45-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 44@45-1'::.cctor

  } // end of class 'Pipe #4 input at line 44@45-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #4 input at line 44@46'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [System.Runtime]System.Tuple`2<string,bool>>
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
    .field public class [System.Runtime]System.Tuple`2<string,bool> current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 class [System.Runtime]System.Tuple`2<string,bool> current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [System.Runtime]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [System.Runtime]System.Tuple`2<string,bool>>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #4 input at line 44@46'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,bool>>& next) cil managed
    {
      // Code size       179 (0xb3)
      .maxstack  8
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      IL_001b:  nop
      IL_001c:  br.s       IL_0089

      IL_001e:  nop
      IL_001f:  br.s       IL_007c

      IL_0021:  nop
      IL_0022:  br         IL_00aa

      IL_0027:  nop
      IL_0028:  ldarg.0
      IL_0029:  ldsfld     class Linq101Select01/'Pipe #4 input at line 44@45-1' Linq101Select01/'Pipe #4 input at line 44@45-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
      IL_0033:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                               class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003d:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_0049:  br.s       IL_007c

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.2
      IL_0059:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_005e:  ldarg.0
      IL_005f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_strings()
      IL_0064:  ldloc.0
      IL_0065:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Item(int32)
      IL_006a:  ldloc.0
      IL_006b:  ldc.i4.2
      IL_006c:  rem
      IL_006d:  ldc.i4.0
      IL_006e:  ceq
      IL_0070:  newobj     instance void class [System.Runtime]System.Tuple`2<string,bool>::.ctor(!0,
                                                                                                  !1)
      IL_0075:  stfld      class [System.Runtime]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
      IL_007a:  ldc.i4.1
      IL_007b:  ret

      IL_007c:  ldarg.0
      IL_007d:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_0082:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0087:  brtrue.s   IL_004b

      IL_0089:  ldarg.0
      IL_008a:  ldc.i4.3
      IL_008b:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_0090:  ldarg.0
      IL_0091:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_0096:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_009b:  nop
      IL_009c:  ldarg.0
      IL_009d:  ldnull
      IL_009e:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
      IL_00a3:  ldarg.0
      IL_00a4:  ldc.i4.3
      IL_00a5:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
      IL_00aa:  ldarg.0
      IL_00ab:  ldnull
      IL_00ac:  stfld      class [System.Runtime]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
      IL_00b1:  ldc.i4.0
      IL_00b2:  ret
    } // end of method 'Pipe #4 input at line 44@46'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> Linq101Select01/'Pipe #4 input at line 44@46'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [System.Runtime]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
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
    } // end of method 'Pipe #4 input at line 44@46'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #4 input at line 44@46'::pc
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
    } // end of method 'Pipe #4 input at line 44@46'::get_CheckClose

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<string,bool> 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Runtime]System.Tuple`2<string,bool> Linq101Select01/'Pipe #4 input at line 44@46'::current
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 44@46'::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [System.Runtime]System.Tuple`2<string,bool>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #4 input at line 44@46'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                              int32,
                                                                                              class [System.Runtime]System.Tuple`2<string,bool>)
      IL_0008:  ret
    } // end of method 'Pipe #4 input at line 44@46'::GetFreshEnumerator

  } // end of class 'Pipe #4 input at line 44@46'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 51@52-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #5 input at line 51@52-1' @_instance
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
    } // end of method 'Pipe #5 input at line 51@52-1'::.ctor

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
    } // end of method 'Pipe #5 input at line 51@52-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #5 input at line 51@52-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #5 input at line 51@52-1' Linq101Select01/'Pipe #5 input at line 51@52-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #5 input at line 51@52-1'::.cctor

  } // end of class 'Pipe #5 input at line 51@52-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #5 input at line 51@53'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>>
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
    .field public class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal> current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal> current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #5 input at line 51@53'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>>& next) cil managed
    {
      // Code size       180 (0xb4)
      .maxstack  8
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      IL_001b:  nop
      IL_001c:  br.s       IL_008a

      IL_001e:  nop
      IL_001f:  br.s       IL_007d

      IL_0021:  nop
      IL_0022:  br         IL_00ab

      IL_0027:  nop
      IL_0028:  ldarg.0
      IL_0029:  ldsfld     class Linq101Select01/'Pipe #5 input at line 51@52-1' Linq101Select01/'Pipe #5 input at line 51@52-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Select01::get_products()
      IL_0033:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<class [Utils]Utils/Product,class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                              class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003d:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_0049:  br.s       IL_007d

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_0051:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.2
      IL_0059:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_005e:  ldarg.0
      IL_005f:  ldloc.0
      IL_0060:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0065:  ldloc.0
      IL_0066:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_006b:  ldloc.0
      IL_006c:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0071:  newobj     instance void class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                             !1,
                                                                                                                                             !2)
      IL_0076:  stfld      class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
      IL_007b:  ldc.i4.1
      IL_007c:  ret

      IL_007d:  ldarg.0
      IL_007e:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_0083:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0088:  brtrue.s   IL_004b

      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.3
      IL_008c:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_0091:  ldarg.0
      IL_0092:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_0097:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_009c:  nop
      IL_009d:  ldarg.0
      IL_009e:  ldnull
      IL_009f:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
      IL_00a4:  ldarg.0
      IL_00a5:  ldc.i4.3
      IL_00a6:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
      IL_00ab:  ldarg.0
      IL_00ac:  ldnull
      IL_00ad:  stfld      class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
      IL_00b2:  ldc.i4.0
      IL_00b3:  ret
    } // end of method 'Pipe #5 input at line 51@53'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
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
        IL_0018:  ldfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
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
        IL_0044:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101Select01/'Pipe #5 input at line 51@53'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
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
    } // end of method 'Pipe #5 input at line 51@53'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Select01/'Pipe #5 input at line 51@53'::pc
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
    } // end of method 'Pipe #5 input at line 51@53'::get_CheckClose

    .method public strict virtual instance class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal> 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal> Linq101Select01/'Pipe #5 input at line 51@53'::current
      IL_0006:  ret
    } // end of method 'Pipe #5 input at line 51@53'::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Select01/'Pipe #5 input at line 51@53'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                              int32,
                                                                                              class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>)
      IL_0008:  ret
    } // end of method 'Pipe #5 input at line 51@53'::GetFreshEnumerator

  } // end of class 'Pipe #5 input at line 51@53'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@60'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #6 input at line 59@60'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #6 input at line 59@60'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #6 input at line 59@60'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<int32,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #6 input at line 59@60'::Invoke

  } // end of class 'Pipe #6 input at line 59@60'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@61-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #6 input at line 59@61-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #6 input at line 59@61-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.5
      IL_0002:  clt
      IL_0004:  ret
    } // end of method 'Pipe #6 input at line 59@61-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #6 input at line 59@61-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #6 input at line 59@61-1' Linq101Select01/'Pipe #6 input at line 59@61-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 59@61-1'::.cctor

  } // end of class 'Pipe #6 input at line 59@61-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@62-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #6 input at line 59@62-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #6 input at line 59@62-2'::.ctor

    .method public strict virtual instance string 
            Invoke(int32 n) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_digits()
      IL_0005:  ldarg.1
      IL_0006:  tail.
      IL_0008:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Item(int32)
      IL_000d:  ret
    } // end of method 'Pipe #6 input at line 59@62-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #6 input at line 59@62-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #6 input at line 59@62-2' Linq101Select01/'Pipe #6 input at line 59@62-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #6 input at line 59@62-2'::.cctor

  } // end of class 'Pipe #6 input at line 59@62-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@73-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<int32,int32>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 a
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 int32 a) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<int32,int32>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@73-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 Linq101Select01/'Pipe #7 input at line 71@73-1'::a
      IL_0014:  ret
    } // end of method 'Pipe #7 input at line 71@73-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<int32,int32>,object> 
            Invoke(int32 _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@73-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      int32 Linq101Select01/'Pipe #7 input at line 71@73-1'::a
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [System.Runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                  !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<int32,int32>,object>(!!0)
      IL_001b:  ret
    } // end of method 'Pipe #7 input at line 71@73-1'::Invoke

  } // end of class 'Pipe #7 input at line 71@73-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@72'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<int32,int32>,class [System.Runtime]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<int32,int32>,class [System.Runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@72'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #7 input at line 71@72'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<int32,int32>,class [System.Runtime]System.Collections.IEnumerable> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@72'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@72'::builder@
      IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbersB()
      IL_0013:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0018:  ldarg.0
      IL_0019:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #7 input at line 71@72'::builder@
      IL_001e:  ldloc.0
      IL_001f:  newobj     instance void Linq101Select01/'Pipe #7 input at line 71@73-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                int32)
      IL_0024:  tail.
      IL_0026:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<int32,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002b:  ret
    } // end of method 'Pipe #7 input at line 71@72'::Invoke

  } // end of class 'Pipe #7 input at line 71@72'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@74-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<int32,int32>,bool>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #7 input at line 71@74-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<int32,int32>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #7 input at line 71@74-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [System.Runtime]System.Tuple`2<int32,int32> tupledArg) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<int32,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<int32,int32>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  clt
      IL_0012:  ret
    } // end of method 'Pipe #7 input at line 71@74-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #7 input at line 71@74-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #7 input at line 71@74-2' Linq101Select01/'Pipe #7 input at line 71@74-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #7 input at line 71@74-2'::.cctor

  } // end of class 'Pipe #7 input at line 71@74-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@75-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<int32,int32>,class [System.Runtime]System.Tuple`2<int32,int32>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #7 input at line 71@75-3' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<int32,int32>,class [System.Runtime]System.Tuple`2<int32,int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #7 input at line 71@75-3'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<int32,int32> 
            Invoke(class [System.Runtime]System.Tuple`2<int32,int32> tupledArg) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<int32,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<int32,int32>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  newobj     instance void class [System.Runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                                  !1)
      IL_0015:  ret
    } // end of method 'Pipe #7 input at line 71@75-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #7 input at line 71@75-3'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #7 input at line 71@75-3' Linq101Select01/'Pipe #7 input at line 71@75-3'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #7 input at line 71@75-3'::.cctor

  } // end of class 'Pipe #7 input at line 71@75-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@83-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@83-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Select01/'Pipe #8 input at line 81@83-1'::c
      IL_0014:  ret
    } // end of method 'Pipe #8 input at line 81@83-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@83-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Select01/'Pipe #8 input at line 81@83-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                           !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'Pipe #8 input at line 81@83-1'::Invoke

  } // end of class 'Pipe #8 input at line 81@83-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@82'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@82'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #8 input at line 81@82'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@82'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@82'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #8 input at line 81@82'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Select01/'Pipe #8 input at line 81@83-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method 'Pipe #8 input at line 81@82'::Invoke

  } // end of class 'Pipe #8 input at line 81@82'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@84-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #8 input at line 81@84-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #8 input at line 81@84-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       40 (0x28)
      .maxstack  10
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0014:  ldc.i4     0xc350
      IL_0019:  ldc.i4.0
      IL_001a:  ldc.i4.0
      IL_001b:  ldc.i4.0
      IL_001c:  ldc.i4.2
      IL_001d:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
      IL_0022:  call       bool [netstandard]System.Decimal::op_LessThan(valuetype [netstandard]System.Decimal,
                                                                         valuetype [netstandard]System.Decimal)
      IL_0027:  ret
    } // end of method 'Pipe #8 input at line 81@84-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #8 input at line 81@84-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #8 input at line 81@84-2' Linq101Select01/'Pipe #8 input at line 81@84-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #8 input at line 81@84-2'::.cctor

  } // end of class 'Pipe #8 input at line 81@84-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@85-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #8 input at line 81@85-3' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #8 input at line 81@85-3'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal> 
            Invoke(class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       38 (0x26)
      .maxstack  7
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0020:  newobj     instance void class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                            !1,
                                                                                                                                            !2)
      IL_0025:  ret
    } // end of method 'Pipe #8 input at line 81@85-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #8 input at line 81@85-3'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #8 input at line 81@85-3' Linq101Select01/'Pipe #8 input at line 81@85-3'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #8 input at line 81@85-3'::.cctor

  } // end of class 'Pipe #8 input at line 81@85-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@92-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@92-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Select01/'Pipe #9 input at line 90@92-1'::c
      IL_0014:  ret
    } // end of method 'Pipe #9 input at line 90@92-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@92-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Select01/'Pipe #9 input at line 90@92-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                           !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'Pipe #9 input at line 90@92-1'::Invoke

  } // end of class 'Pipe #9 input at line 90@92-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@91'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@91'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #9 input at line 90@91'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@91'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@91'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'Pipe #9 input at line 90@91'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Select01/'Pipe #9 input at line 90@92-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method 'Pipe #9 input at line 90@91'::Invoke

  } // end of class 'Pipe #9 input at line 90@91'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@93-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #9 input at line 90@93-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #9 input at line 90@93-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       64 (0x40)
      .maxstack  7
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1,
               valuetype [System.Runtime]System.DateTime V_2,
               valuetype [System.Runtime]System.DateTime V_3,
               valuetype [System.Runtime]System.DateTime V_4,
               valuetype [System.Runtime]System.DateTime V_5,
               valuetype [System.Runtime]System.DateTime V_6,
               valuetype [System.Runtime]System.DateTime V_7)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [System.Runtime]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0014:  stloc.2
      IL_0015:  ldc.i4     0x7ce
      IL_001a:  ldc.i4.1
      IL_001b:  ldc.i4.1
      IL_001c:  newobj     instance void [System.Runtime]System.DateTime::.ctor(int32,
                                                                                int32,
                                                                                int32)
      IL_0021:  stloc.3
      IL_0022:  ldloc.2
      IL_0023:  stloc.s    V_4
      IL_0025:  ldloc.3
      IL_0026:  stloc.s    V_5
      IL_0028:  ldloc.s    V_4
      IL_002a:  stloc.s    V_6
      IL_002c:  ldloc.s    V_5
      IL_002e:  stloc.s    V_7
      IL_0030:  ldloc.s    V_6
      IL_0032:  ldloc.s    V_7
      IL_0034:  call       int32 [netstandard]System.DateTime::Compare(valuetype [netstandard]System.DateTime,
                                                                       valuetype [netstandard]System.DateTime)
      IL_0039:  ldc.i4.0
      IL_003a:  clt
      IL_003c:  ldc.i4.0
      IL_003d:  ceq
      IL_003f:  ret
    } // end of method 'Pipe #9 input at line 90@93-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #9 input at line 90@93-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #9 input at line 90@93-2' Linq101Select01/'Pipe #9 input at line 90@93-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #9 input at line 90@93-2'::.cctor

  } // end of class 'Pipe #9 input at line 90@93-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@94-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>>
  {
    .field static assembly initonly class Linq101Select01/'Pipe #9 input at line 90@94-3' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #9 input at line 90@94-3'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime> 
            Invoke(class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       38 (0x26)
      .maxstack  7
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [System.Runtime]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0020:  newobj     instance void class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>::.ctor(!0,
                                                                                                                                             !1,
                                                                                                                                             !2)
      IL_0025:  ret
    } // end of method 'Pipe #9 input at line 90@94-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'Pipe #9 input at line 90@94-3'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'Pipe #9 input at line 90@94-3' Linq101Select01/'Pipe #9 input at line 90@94-3'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #9 input at line 90@94-3'::.cctor

  } // end of class 'Pipe #9 input at line 90@94-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders3@101-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders3@101-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Select01/'orders3@101-1'::c
      IL_0014:  ret
    } // end of method 'orders3@101-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders3@101-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Select01/'orders3@101-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                           !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'orders3@101-1'::Invoke

  } // end of class 'orders3@101-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit orders3@100
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders3@100::builder@
      IL_000d:  ret
    } // end of method orders3@100::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders3@100::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders3@100::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders3@100::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Select01/'orders3@101-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method orders3@100::Invoke

  } // end of class orders3@100

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders3@102-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Select01/'orders3@102-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'orders3@102-2'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       40 (0x28)
      .maxstack  10
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0014:  ldc.i4     0x4e20
      IL_0019:  ldc.i4.0
      IL_001a:  ldc.i4.0
      IL_001b:  ldc.i4.0
      IL_001c:  ldc.i4.1
      IL_001d:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
      IL_0022:  call       bool [netstandard]System.Decimal::op_GreaterThanOrEqual(valuetype [netstandard]System.Decimal,
                                                                                   valuetype [netstandard]System.Decimal)
      IL_0027:  ret
    } // end of method 'orders3@102-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders3@102-2'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders3@102-2' Linq101Select01/'orders3@102-2'::@_instance
      IL_000a:  ret
    } // end of method 'orders3@102-2'::.cctor

  } // end of class 'orders3@102-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders3@103-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>>
  {
    .field static assembly initonly class Linq101Select01/'orders3@103-3' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } // end of method 'orders3@103-3'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal> 
            Invoke(class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       38 (0x26)
      .maxstack  7
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0020:  newobj     instance void class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>::.ctor(!0,
                                                                                                                                            !1,
                                                                                                                                            !2)
      IL_0025:  ret
    } // end of method 'orders3@103-3'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders3@103-3'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders3@103-3' Linq101Select01/'orders3@103-3'::@_instance
      IL_000a:  ret
    } // end of method 'orders3@103-3'::.cctor

  } // end of class 'orders3@103-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit orders4@111
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders4@111::builder@
      IL_000d:  ret
    } // end of method orders4@111::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/orders4@111::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Customer,object>(!!0)
      IL_0010:  ret
    } // end of method orders4@111::Invoke

  } // end of class orders4@111

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@112-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>
  {
    .field static assembly initonly class Linq101Select01/'orders4@112-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'orders4@112-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Customer c) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance string [Utils]Utils/Customer::get_Region()
      IL_0006:  ldstr      "WA"
      IL_000b:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0010:  ret
    } // end of method 'orders4@112-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders4@112-1'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders4@112-1' Linq101Select01/'orders4@112-1'::@_instance
      IL_000a:  ret
    } // end of method 'orders4@112-1'::.cctor

  } // end of class 'orders4@112-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@113-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@113-3'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer Linq101Select01/'orders4@113-3'::c
      IL_0014:  ret
    } // end of method 'orders4@113-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> 
            Invoke(class [Utils]Utils/Order _arg3) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  7
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@113-3'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer Linq101Select01/'orders4@113-3'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                           !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } // end of method 'orders4@113-3'::Invoke

  } // end of class 'orders4@113-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@111-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@111-2'::builder@
      IL_000d:  ret
    } // end of method 'orders4@111-2'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable> 
            Invoke(class [Utils]Utils/Customer _arg2) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  8
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@111-2'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@111-2'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Select01/'orders4@111-2'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void Linq101Select01/'orders4@113-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } // end of method 'orders4@111-2'::Invoke

  } // end of class 'orders4@111-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@114-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class Linq101Select01/'orders4@114-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'orders4@114-4'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       57 (0x39)
      .maxstack  6
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1,
               valuetype [System.Runtime]System.DateTime V_2,
               valuetype [System.Runtime]System.DateTime V_3,
               valuetype [System.Runtime]System.DateTime V_4,
               valuetype [System.Runtime]System.DateTime V_5,
               valuetype [System.Runtime]System.DateTime V_6,
               valuetype [System.Runtime]System.DateTime V_7)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [System.Runtime]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0014:  stloc.2
      IL_0015:  call       valuetype [System.Runtime]System.DateTime Linq101Select01::get_cutOffDate()
      IL_001a:  stloc.3
      IL_001b:  ldloc.2
      IL_001c:  stloc.s    V_4
      IL_001e:  ldloc.3
      IL_001f:  stloc.s    V_5
      IL_0021:  ldloc.s    V_4
      IL_0023:  stloc.s    V_6
      IL_0025:  ldloc.s    V_5
      IL_0027:  stloc.s    V_7
      IL_0029:  ldloc.s    V_6
      IL_002b:  ldloc.s    V_7
      IL_002d:  call       int32 [netstandard]System.DateTime::Compare(valuetype [netstandard]System.DateTime,
                                                                       valuetype [netstandard]System.DateTime)
      IL_0032:  ldc.i4.0
      IL_0033:  clt
      IL_0035:  ldc.i4.0
      IL_0036:  ceq
      IL_0038:  ret
    } // end of method 'orders4@114-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders4@114-4'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders4@114-4' Linq101Select01/'orders4@114-4'::@_instance
      IL_000a:  ret
    } // end of method 'orders4@114-4'::.cctor

  } // end of class 'orders4@114-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@115-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Tuple`2<string,int32>>
  {
    .field static assembly initonly class Linq101Select01/'orders4@115-5' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Tuple`2<string,int32>>::.ctor()
      IL_0006:  ret
    } // end of method 'orders4@115-5'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Tuple`2<string,int32> 
            Invoke(class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      // Code size       32 (0x20)
      .maxstack  6
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  newobj     instance void class [System.Runtime]System.Tuple`2<string,int32>::.ctor(!0,
                                                                                                   !1)
      IL_001f:  ret
    } // end of method 'orders4@115-5'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Select01/'orders4@115-5'::.ctor()
      IL_0005:  stsfld     class Linq101Select01/'orders4@115-5' Linq101Select01/'orders4@115-5'::@_instance
      IL_000a:  ret
    } // end of method 'orders4@115-5'::.cctor

  } // end of class 'orders4@115-5'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbers@7
    IL_0005:  ret
  } // end of method Linq101Select01::get_numbers

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numsPlusOne() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numsPlusOne@10
    IL_0005:  ret
  } // end of method Linq101Select01::get_numsPlusOne

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Select01>'.$Linq101Select01::products@17
    IL_0005:  ret
  } // end of method Linq101Select01::get_products

  .method public specialname static class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> 
          get_productNames() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::productNames@19
    IL_0005:  ret
  } // end of method Linq101Select01::get_productNames

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_strings() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::strings@26
    IL_0005:  ret
  } // end of method Linq101Select01::get_strings

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_textNums() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::textNums@27
    IL_0005:  ret
  } // end of method Linq101Select01::get_textNums

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_words() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::words@34
    IL_0005:  ret
  } // end of method Linq101Select01::get_words

  .method public specialname static class [System.Runtime]System.Tuple`2<string,string>[] 
          get_upperLowerWords() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`2<string,string>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::upperLowerWords@36
    IL_0005:  ret
  } // end of method Linq101Select01::get_upperLowerWords

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [System.Runtime]System.Tuple`2<string,bool>> 
          get_digitOddEvens() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [System.Runtime]System.Tuple`2<string,bool>> '<StartupCode$Linq101Select01>'.$Linq101Select01::digitOddEvens@43
    IL_0005:  ret
  } // end of method Linq101Select01::get_digitOddEvens

  .method public specialname static class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>[] 
          get_productInfos() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::productInfos@50
    IL_0005:  ret
  } // end of method Linq101Select01::get_productInfos

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_digits() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::digits@57
    IL_0005:  ret
  } // end of method Linq101Select01::get_digits

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_lowNums() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::lowNums@58
    IL_0005:  ret
  } // end of method Linq101Select01::get_lowNums

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbersA() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbersA@67
    IL_0005:  ret
  } // end of method Linq101Select01::get_numbersA

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbersB() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbersB@68
    IL_0005:  ret
  } // end of method Linq101Select01::get_numbersB

  .method public specialname static class [System.Runtime]System.Tuple`2<int32,int32>[] 
          get_pairs() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`2<int32,int32>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::pairs@70
    IL_0005:  ret
  } // end of method Linq101Select01::get_pairs

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> 
          get_customers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Select01>'.$Linq101Select01::customers@79
    IL_0005:  ret
  } // end of method Linq101Select01::get_customers

  .method public specialname static class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>[] 
          get_orders() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::orders@80
    IL_0005:  ret
  } // end of method Linq101Select01::get_orders

  .method public specialname static class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>[] 
          get_orders2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::orders2@89
    IL_0005:  ret
  } // end of method Linq101Select01::get_orders2

  .method public specialname static class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>> 
          get_orders3() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>> '<StartupCode$Linq101Select01>'.$Linq101Select01::orders3@98
    IL_0005:  ret
  } // end of method Linq101Select01::get_orders3

  .method public specialname static valuetype [System.Runtime]System.DateTime 
          get_cutOffDate() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     valuetype [System.Runtime]System.DateTime '<StartupCode$Linq101Select01>'.$Linq101Select01::cutOffDate@107
    IL_0005:  ret
  } // end of method Linq101Select01::get_cutOffDate

  .method public specialname static class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,int32>> 
          get_orders4() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,int32>> '<StartupCode$Linq101Select01>'.$Linq101Select01::orders4@109
    IL_0005:  ret
  } // end of method Linq101Select01::get_orders4

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
  } // end of property Linq101Select01::numbers
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numsPlusOne()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numsPlusOne()
  } // end of property Linq101Select01::numsPlusOne
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Select01::get_products()
  } // end of property Linq101Select01::products
  .property class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>
          productNames()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> Linq101Select01::get_productNames()
  } // end of property Linq101Select01::productNames
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          strings()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_strings()
  } // end of property Linq101Select01::strings
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          textNums()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_textNums()
  } // end of property Linq101Select01::textNums
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_words()
  } // end of property Linq101Select01::words
  .property class [System.Runtime]System.Tuple`2<string,string>[]
          upperLowerWords()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`2<string,string>[] Linq101Select01::get_upperLowerWords()
  } // end of property Linq101Select01::upperLowerWords
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [System.Runtime]System.Tuple`2<string,bool>>
          digitOddEvens()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [System.Runtime]System.Tuple`2<string,bool>> Linq101Select01::get_digitOddEvens()
  } // end of property Linq101Select01::digitOddEvens
  .property class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>[]
          productInfos()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>[] Linq101Select01::get_productInfos()
  } // end of property Linq101Select01::productInfos
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          digits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_digits()
  } // end of property Linq101Select01::digits
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          lowNums()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_lowNums()
  } // end of property Linq101Select01::lowNums
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbersA()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbersA()
  } // end of property Linq101Select01::numbersA
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbersB()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbersB()
  } // end of property Linq101Select01::numbersB
  .property class [System.Runtime]System.Tuple`2<int32,int32>[]
          pairs()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`2<int32,int32>[] Linq101Select01::get_pairs()
  } // end of property Linq101Select01::pairs
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer>
          customers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
  } // end of property Linq101Select01::customers
  .property class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>[]
          orders()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>[] Linq101Select01::get_orders()
  } // end of property Linq101Select01::orders
  .property class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>[]
          orders2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>[] Linq101Select01::get_orders2()
  } // end of property Linq101Select01::orders2
  .property class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>>
          orders3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>> Linq101Select01::get_orders3()
  } // end of property Linq101Select01::orders3
  .property valuetype [System.Runtime]System.DateTime
          cutOffDate()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype [System.Runtime]System.DateTime Linq101Select01::get_cutOffDate()
  } // end of property Linq101Select01::cutOffDate
  .property class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,int32>>
          orders4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,int32>> Linq101Select01::get_orders4()
  } // end of property Linq101Select01::orders4
} // end of class Linq101Select01

.class private abstract auto ansi sealed '<StartupCode$Linq101Select01>'.$Linq101Select01
       extends [System.Runtime]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@7
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numsPlusOne@10
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@17
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> productNames@19
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> strings@26
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> textNums@27
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@34
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`2<string,string>[] upperLowerWords@36
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [System.Runtime]System.Tuple`2<string,bool>> digitOddEvens@43
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>[] productInfos@50
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits@57
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> lowNums@58
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbersA@67
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbersB@68
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`2<int32,int32>[] pairs@70
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers@79
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>[] orders@80
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>[] orders2@89
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>> orders3@98
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype [System.Runtime]System.DateTime cutOffDate@107
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,int32>> orders4@109
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1177 (0x499)
    .maxstack  13
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> V_2,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> V_3,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_4,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_5,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_6,
             class [System.Runtime]System.Tuple`2<string,string>[] V_7,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [System.Runtime]System.Tuple`2<string,bool>> V_8,
             class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>[] V_9,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_10,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_11,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_12,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_13,
             class [System.Runtime]System.Tuple`2<int32,int32>[] V_14,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> V_15,
             class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>[] V_16,
             class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>[] V_17,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>> V_18,
             valuetype [System.Runtime]System.DateTime V_19,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,int32>> V_20,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_21,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_22,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_23,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> V_24,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_25,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,string>> V_26,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_27,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,bool>> V_28,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_29,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>> V_30,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_31,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> V_32,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_33,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_34,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_35,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_36,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_37,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_38,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_39,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<int32,int32>> V_40,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_41,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>> V_42,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_43,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>> V_44,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_45,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_46,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_47)
    IL_0000:  ldc.i4.5
    IL_0001:  ldc.i4.4
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.3
    IL_0004:  ldc.i4.s   9
    IL_0006:  ldc.i4.8
    IL_0007:  ldc.i4.6
    IL_0008:  ldc.i4.7
    IL_0009:  ldc.i4.2
    IL_000a:  ldc.i4.0
    IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0024:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0029:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0033:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0038:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_003d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0042:  dup
    IL_0043:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbers@7
    IL_0048:  stloc.0
    IL_0049:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_004e:  stloc.s    V_22
    IL_0050:  ldnull
    IL_0051:  ldc.i4.0
    IL_0052:  ldc.i4.0
    IL_0053:  newobj     instance void Linq101Select01/'Pipe #1 input at line 11@13'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                            int32,
                                                                                            int32)
    IL_0058:  stloc.s    V_21
    IL_005a:  ldloc.s    V_21
    IL_005c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0061:  dup
    IL_0062:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numsPlusOne@10
    IL_0067:  stloc.1
    IL_0068:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_006d:  dup
    IL_006e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Select01>'.$Linq101Select01::products@17
    IL_0073:  stloc.2
    IL_0074:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0079:  stloc.s    V_23
    IL_007b:  ldnull
    IL_007c:  ldc.i4.0
    IL_007d:  ldnull
    IL_007e:  newobj     instance void Linq101Select01/productNames@22::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                              int32,
                                                                              string)
    IL_0083:  dup
    IL_0084:  stsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::productNames@19
    IL_0089:  stloc.3
    IL_008a:  ldstr      "zero"
    IL_008f:  ldstr      "one"
    IL_0094:  ldstr      "two"
    IL_0099:  ldstr      "three"
    IL_009e:  ldstr      "four"
    IL_00a3:  ldstr      "five"
    IL_00a8:  ldstr      "six"
    IL_00ad:  ldstr      "seven"
    IL_00b2:  ldstr      "eight"
    IL_00b7:  ldstr      "nine"
    IL_00bc:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_00c1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00c6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00cb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00d0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00d5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00da:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00df:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ee:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00f3:  dup
    IL_00f4:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::strings@26
    IL_00f9:  stloc.s    V_4
    IL_00fb:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0100:  stloc.s    V_25
    IL_0102:  ldnull
    IL_0103:  ldc.i4.0
    IL_0104:  ldnull
    IL_0105:  newobj     instance void Linq101Select01/'Pipe #2 input at line 28@30'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                            int32,
                                                                                            string)
    IL_010a:  stloc.s    V_24
    IL_010c:  ldloc.s    V_24
    IL_010e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0113:  dup
    IL_0114:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::textNums@27
    IL_0119:  stloc.s    V_5
    IL_011b:  ldstr      "aPPLE"
    IL_0120:  ldstr      "BlUeBeRrY"
    IL_0125:  ldstr      "cHeRry"
    IL_012a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_012f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0134:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0139:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013e:  dup
    IL_013f:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::words@34
    IL_0144:  stloc.s    V_6
    IL_0146:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_014b:  stloc.s    V_27
    IL_014d:  ldnull
    IL_014e:  ldc.i4.0
    IL_014f:  ldnull
    IL_0150:  newobj     instance void Linq101Select01/'Pipe #3 input at line 37@39'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                            int32,
                                                                                            class [System.Runtime]System.Tuple`2<string,string>)
    IL_0155:  stloc.s    V_26
    IL_0157:  ldloc.s    V_26
    IL_0159:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<string,string>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_015e:  dup
    IL_015f:  stsfld     class [System.Runtime]System.Tuple`2<string,string>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::upperLowerWords@36
    IL_0164:  stloc.s    V_7
    IL_0166:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_016b:  stloc.s    V_29
    IL_016d:  ldnull
    IL_016e:  ldc.i4.0
    IL_016f:  ldnull
    IL_0170:  newobj     instance void Linq101Select01/'Pipe #4 input at line 44@46'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                            int32,
                                                                                            class [System.Runtime]System.Tuple`2<string,bool>)
    IL_0175:  stloc.s    V_28
    IL_0177:  ldloc.s    V_28
    IL_0179:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<class [System.Runtime]System.Tuple`2<string,bool>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_017e:  dup
    IL_017f:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [System.Runtime]System.Tuple`2<string,bool>> '<StartupCode$Linq101Select01>'.$Linq101Select01::digitOddEvens@43
    IL_0184:  stloc.s    V_8
    IL_0186:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_018b:  stloc.s    V_31
    IL_018d:  ldnull
    IL_018e:  ldc.i4.0
    IL_018f:  ldnull
    IL_0190:  newobj     instance void Linq101Select01/'Pipe #5 input at line 51@53'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                            int32,
                                                                                            class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>)
    IL_0195:  stloc.s    V_30
    IL_0197:  ldloc.s    V_30
    IL_0199:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_019e:  dup
    IL_019f:  stsfld     class [System.Runtime]System.Tuple`3<string,string,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::productInfos@50
    IL_01a4:  stloc.s    V_9
    IL_01a6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_strings()
    IL_01ab:  dup
    IL_01ac:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::digits@57
    IL_01b1:  stloc.s    V_10
    IL_01b3:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01b8:  stloc.s    V_33
    IL_01ba:  ldloc.s    V_33
    IL_01bc:  ldloc.s    V_33
    IL_01be:  ldloc.s    V_33
    IL_01c0:  ldloc.s    V_33
    IL_01c2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbers()
    IL_01c7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01cc:  ldloc.s    V_33
    IL_01ce:  newobj     instance void Linq101Select01/'Pipe #6 input at line 59@60'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01d3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [System.Runtime]System.Collections.IEnumerable,int32,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01d8:  ldsfld     class Linq101Select01/'Pipe #6 input at line 59@61-1' Linq101Select01/'Pipe #6 input at line 59@61-1'::@_instance
    IL_01dd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<int32,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_01e2:  ldsfld     class Linq101Select01/'Pipe #6 input at line 59@62-2' Linq101Select01/'Pipe #6 input at line 59@62-2'::@_instance
    IL_01e7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<int32,class [System.Runtime]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01ec:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_01f1:  stloc.s    V_32
    IL_01f3:  ldloc.s    V_32
    IL_01f5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01fa:  dup
    IL_01fb:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Select01>'.$Linq101Select01::lowNums@58
    IL_0200:  stloc.s    V_11
    IL_0202:  nop
    IL_0203:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Select01::get_lowNums()
    IL_0208:  stloc.s    V_34
    IL_020a:  ldstr      "four"
    IL_020f:  ldstr      "one"
    IL_0214:  ldstr      "three"
    IL_0219:  ldstr      "two"
    IL_021e:  ldstr      "zero"
    IL_0223:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0228:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_022d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0232:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0237:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_023c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0241:  stloc.s    V_35
    IL_0243:  ldloc.s    V_34
    IL_0245:  stloc.s    V_36
    IL_0247:  ldloc.s    V_35
    IL_0249:  stloc.s    V_37
    IL_024b:  ldloc.s    V_36
    IL_024d:  stloc.s    V_38
    IL_024f:  ldloc.s    V_37
    IL_0251:  stloc.s    V_39
    IL_0253:  ldloc.s    V_38
    IL_0255:  ldloc.s    V_39
    IL_0257:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_025c:  callvirt   instance bool class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Equals(object,
                                                                                                                    class [System.Runtime]System.Collections.IEqualityComparer)
    IL_0261:  ldc.i4.0
    IL_0262:  ceq
    IL_0264:  brfalse.s  IL_0280

    IL_0266:  ldstr      "lowNums failed"
    IL_026b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0270:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0275:  pop
    IL_0276:  ldc.i4.1
    IL_0277:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_027c:  pop
    IL_027d:  nop
    IL_027e:  br.s       IL_0281

    IL_0280:  nop
    IL_0281:  ldc.i4.0
    IL_0282:  ldc.i4.2
    IL_0283:  ldc.i4.4
    IL_0284:  ldc.i4.5
    IL_0285:  ldc.i4.6
    IL_0286:  ldc.i4.8
    IL_0287:  ldc.i4.s   9
    IL_0289:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_028e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0293:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0298:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_029d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02a2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02a7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02ac:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02b1:  dup
    IL_02b2:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbersA@67
    IL_02b7:  stloc.s    V_12
    IL_02b9:  ldc.i4.1
    IL_02ba:  ldc.i4.3
    IL_02bb:  ldc.i4.5
    IL_02bc:  ldc.i4.7
    IL_02bd:  ldc.i4.8
    IL_02be:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_02c3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02c8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02cd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02d2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02d7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02dc:  dup
    IL_02dd:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Select01>'.$Linq101Select01::numbersB@68
    IL_02e2:  stloc.s    V_13
    IL_02e4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_02e9:  stloc.s    V_41
    IL_02eb:  ldloc.s    V_41
    IL_02ed:  ldloc.s    V_41
    IL_02ef:  ldloc.s    V_41
    IL_02f1:  ldloc.s    V_41
    IL_02f3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Select01::get_numbersA()
    IL_02f8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02fd:  ldloc.s    V_41
    IL_02ff:  newobj     instance void Linq101Select01/'Pipe #7 input at line 71@72'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0304:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<int32,int32>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0309:  ldsfld     class Linq101Select01/'Pipe #7 input at line 71@74-2' Linq101Select01/'Pipe #7 input at line 71@74-2'::@_instance
    IL_030e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Runtime]System.Tuple`2<int32,int32>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0313:  ldsfld     class Linq101Select01/'Pipe #7 input at line 71@75-3' Linq101Select01/'Pipe #7 input at line 71@75-3'::@_instance
    IL_0318:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<int32,int32>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<int32,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_031d:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<int32,int32>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0322:  stloc.s    V_40
    IL_0324:  ldloc.s    V_40
    IL_0326:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`2<int32,int32>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_032b:  dup
    IL_032c:  stsfld     class [System.Runtime]System.Tuple`2<int32,int32>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::pairs@70
    IL_0331:  stloc.s    V_14
    IL_0333:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_0338:  dup
    IL_0339:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Select01>'.$Linq101Select01::customers@79
    IL_033e:  stloc.s    V_15
    IL_0340:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0345:  stloc.s    V_43
    IL_0347:  ldloc.s    V_43
    IL_0349:  ldloc.s    V_43
    IL_034b:  ldloc.s    V_43
    IL_034d:  ldloc.s    V_43
    IL_034f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
    IL_0354:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0359:  ldloc.s    V_43
    IL_035b:  newobj     instance void Linq101Select01/'Pipe #8 input at line 81@82'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0360:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0365:  ldsfld     class Linq101Select01/'Pipe #8 input at line 81@84-2' Linq101Select01/'Pipe #8 input at line 81@84-2'::@_instance
    IL_036a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_036f:  ldsfld     class Linq101Select01/'Pipe #8 input at line 81@85-3' Linq101Select01/'Pipe #8 input at line 81@85-3'::@_instance
    IL_0374:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0379:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_037e:  stloc.s    V_42
    IL_0380:  ldloc.s    V_42
    IL_0382:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0387:  dup
    IL_0388:  stsfld     class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::orders@80
    IL_038d:  stloc.s    V_16
    IL_038f:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0394:  stloc.s    V_45
    IL_0396:  ldloc.s    V_45
    IL_0398:  ldloc.s    V_45
    IL_039a:  ldloc.s    V_45
    IL_039c:  ldloc.s    V_45
    IL_039e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
    IL_03a3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03a8:  ldloc.s    V_45
    IL_03aa:  newobj     instance void Linq101Select01/'Pipe #9 input at line 90@91'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03af:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03b4:  ldsfld     class Linq101Select01/'Pipe #9 input at line 90@93-2' Linq101Select01/'Pipe #9 input at line 90@93-2'::@_instance
    IL_03b9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_03be:  ldsfld     class Linq101Select01/'Pipe #9 input at line 90@94-3' Linq101Select01/'Pipe #9 input at line 90@94-3'::@_instance
    IL_03c3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03c8:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_03cd:  stloc.s    V_44
    IL_03cf:  ldloc.s    V_44
    IL_03d1:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03d6:  dup
    IL_03d7:  stsfld     class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.DateTime>[] '<StartupCode$Linq101Select01>'.$Linq101Select01::orders2@89
    IL_03dc:  stloc.s    V_17
    IL_03de:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03e3:  stloc.s    V_46
    IL_03e5:  ldloc.s    V_46
    IL_03e7:  ldloc.s    V_46
    IL_03e9:  ldloc.s    V_46
    IL_03eb:  ldloc.s    V_46
    IL_03ed:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
    IL_03f2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03f7:  ldloc.s    V_46
    IL_03f9:  newobj     instance void Linq101Select01/orders3@100::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03fe:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0403:  ldsfld     class Linq101Select01/'orders3@102-2' Linq101Select01/'orders3@102-2'::@_instance
    IL_0408:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_040d:  ldsfld     class Linq101Select01/'orders3@103-3' Linq101Select01/'orders3@103-3'::@_instance
    IL_0412:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0417:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_041c:  dup
    IL_041d:  stsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`3<string,int32,valuetype [System.Runtime]System.Decimal>> '<StartupCode$Linq101Select01>'.$Linq101Select01::orders3@98
    IL_0422:  stloc.s    V_18
    IL_0424:  ldc.i4     0x7cd
    IL_0429:  ldc.i4.1
    IL_042a:  ldc.i4.1
    IL_042b:  newobj     instance void [System.Runtime]System.DateTime::.ctor(int32,
                                                                              int32,
                                                                              int32)
    IL_0430:  dup
    IL_0431:  stsfld     valuetype [System.Runtime]System.DateTime '<StartupCode$Linq101Select01>'.$Linq101Select01::cutOffDate@107
    IL_0436:  stloc.s    V_19
    IL_0438:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_043d:  stloc.s    V_47
    IL_043f:  ldloc.s    V_47
    IL_0441:  ldloc.s    V_47
    IL_0443:  ldloc.s    V_47
    IL_0445:  ldloc.s    V_47
    IL_0447:  ldloc.s    V_47
    IL_0449:  ldloc.s    V_47
    IL_044b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Select01::get_customers()
    IL_0450:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0455:  ldloc.s    V_47
    IL_0457:  newobj     instance void Linq101Select01/orders4@111::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_045c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Customer,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0461:  ldsfld     class Linq101Select01/'orders4@112-1' Linq101Select01/'orders4@112-1'::@_instance
    IL_0466:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_046b:  ldloc.s    V_47
    IL_046d:  newobj     instance void Linq101Select01/'orders4@111-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0472:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0477:  ldsfld     class Linq101Select01/'orders4@114-4' Linq101Select01/'orders4@114-4'::@_instance
    IL_047c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0481:  ldsfld     class Linq101Select01/'orders4@115-5' Linq101Select01/'orders4@115-5'::@_instance
    IL_0486:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [System.Runtime]System.Collections.IEnumerable,class [System.Runtime]System.Tuple`2<string,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_048b:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Runtime]System.Tuple`2<string,int32>,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0490:  dup
    IL_0491:  stsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [System.Runtime]System.Tuple`2<string,int32>> '<StartupCode$Linq101Select01>'.$Linq101Select01::orders4@109
    IL_0496:  stloc.s    V_20
    IL_0498:  ret
  } // end of method $Linq101Select01::main@

} // end of class '<StartupCode$Linq101Select01>'.$Linq101Select01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\QueryExpressionStepping\Linq101Select01_fs\Linq101Select01.res
