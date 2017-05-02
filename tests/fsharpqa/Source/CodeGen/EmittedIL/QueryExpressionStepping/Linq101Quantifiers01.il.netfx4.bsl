
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
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Linq101Quantifiers01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Quantifiers01
{
  // Offset: 0x00000000 Length: 0x0000039B
}
.mresource public FSharpOptimizationData.Linq101Quantifiers01
{
  // Offset: 0x000003A0 Length: 0x000000FF
}
.module Linq101Quantifiers01.exe
// MVID: {590846DB-76DD-E373-A745-0383DB460859}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01290000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Quantifiers01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname iAfterE@12
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
      IL_0002:  stfld      string Linq101Quantifiers01/iAfterE@12::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      string Linq101Quantifiers01/iAfterE@12::w
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      string Linq101Quantifiers01/iAfterE@12::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_002b:  ret
    } // end of method iAfterE@12::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       191 (0xbf)
      .maxstack  6
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\src\\manofstick\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Quantifiers01.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Quantifiers01/iAfterE@12::pc
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

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0084

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b6

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 12,12 : 9,26 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Quantifiers01::get_words()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      .line 12,12 : 9,26 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_005e:  stfld      string Linq101Quantifiers01/iAfterE@12::_arg1
      .line 12,12 : 9,26 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      string Linq101Quantifiers01/iAfterE@12::_arg1
      IL_006a:  stfld      string Linq101Quantifiers01/iAfterE@12::w
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      .line 13,13 : 9,34 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      string Linq101Quantifiers01/iAfterE@12::w
      IL_007d:  stfld      string Linq101Quantifiers01/iAfterE@12::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldnull
      IL_0086:  stfld      string Linq101Quantifiers01/iAfterE@12::w
      .line 12,12 : 9,26 ''
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101Quantifiers01/iAfterE@12::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      .line 12,12 : 9,26 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldnull
      IL_00b8:  stfld      string Linq101Quantifiers01/iAfterE@12::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
    } // end of method iAfterE@12::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Quantifiers01/iAfterE@12::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      string Linq101Quantifiers01/iAfterE@12::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 12,12 : 9,26 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method iAfterE@12::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Quantifiers01/iAfterE@12::pc
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
    } // end of method iAfterE@12::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string Linq101Quantifiers01/iAfterE@12::current
      IL_0007:  ret
    } // end of method iAfterE@12::get_LastGenerated

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
      IL_0006:  newobj     instance void Linq101Quantifiers01/iAfterE@12::.ctor(string,
                                                                                string,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
      IL_000b:  ret
    } // end of method iAfterE@12::GetFreshEnumerator

  } // end of class iAfterE@12

  .class auto ansi serializable nested assembly beforefieldinit 'iAfterE@13-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'iAfterE@13-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(string w) cil managed
    {
      // Code size       13 (0xd)
      .maxstack  8
      .line 13,13 : 17,33 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldstr      "ei"
      IL_0007:  callvirt   instance bool [mscorlib]System.String::Contains(string)
      IL_000c:  ret
    } // end of method 'iAfterE@13-1'::Invoke

  } // end of class 'iAfterE@13-1'

  .class auto ansi serializable nested assembly beforefieldinit productGroups@21
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/productGroups@21::builder@
      IL_000d:  ret
    } // end of method productGroups@21::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 21,21 : 9,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 22,22 : 9,32 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/productGroups@21::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method productGroups@21::Invoke

  } // end of class productGroups@21

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups@22-1'
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
    } // end of method 'productGroups@22-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 22,22 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'productGroups@22-1'::Invoke

  } // end of class 'productGroups@22-1'

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups@22-2'
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
    } // end of method 'productGroups@22-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 22,22 : 22,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'productGroups@22-2'::Invoke

  } // end of class 'productGroups@22-2'

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups@22-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/'productGroups@22-3'::builder@
      IL_000d:  ret
    } // end of method 'productGroups@22-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g)
      .line 22,22 : 38,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/'productGroups@22-3'::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0011:  ret
    } // end of method 'productGroups@22-3'::Invoke

  } // end of class 'productGroups@22-3'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'productGroups@23-5'
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method 'productGroups@23-5'::.ctor

    .method assembly hidebysig instance bool 
            Invoke(class [Utils]Utils/Product arg) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      .line 23,23 : 31,49 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0007:  ldc.i4.0
      IL_0008:  ceq
      IL_000a:  ret
    } // end of method 'productGroups@23-5'::Invoke

  } // end of class 'productGroups@23-5'

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups@23-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'productGroups@23-4'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      .line 23,23 : 16,50 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  newobj     instance void Linq101Quantifiers01/'productGroups@23-5'::.ctor()
      IL_0007:  ldftn      instance bool Linq101Quantifiers01/'productGroups@23-5'::Invoke(class [Utils]Utils/Product)
      IL_000d:  newobj     instance void class [mscorlib]System.Func`2<class [Utils]Utils/Product,bool>::.ctor(object,
                                                                                                               native int)
      IL_0012:  call       bool [System.Core]System.Linq.Enumerable::Any<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
      IL_0017:  ret
    } // end of method 'productGroups@23-4'::Invoke

  } // end of class 'productGroups@23-4'

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups@24-6'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'productGroups@24-6'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      .line 24,24 : 17,25 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0007:  ldarg.1
      IL_0008:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                     !1)
      IL_000d:  ret
    } // end of method 'productGroups@24-6'::Invoke

  } // end of class 'productGroups@24-6'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname onlyOdd@32
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 n
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(int32 _arg1,
                                 int32 n,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::n
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_002b:  ret
    } // end of method onlyOdd@32::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       191 (0xbf)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
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

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0021:  nop
      IL_0022:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0084

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br         IL_00b6

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_002d:  nop
      .line 32,32 : 9,28 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Quantifiers01::get_numbers()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      .line 32,32 : 9,28 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::_arg1
      .line 32,32 : 9,28 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::_arg1
      IL_006a:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::n
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      .line 33,33 : 9,24 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::n
      IL_007d:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.0
      IL_0086:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::n
      .line 32,32 : 9,28 ''
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      .line 32,32 : 9,28 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4.0
      IL_00b8:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
    } // end of method onlyOdd@32::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       150 (0x96)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      IL_0008:  ldc.i4.3
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0015)
      IL_0013:  br.s       IL_001b

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_0015:  nop
      IL_0016:  br         IL_0089

      .line 100001,100001 : 0,0 ''
      .line 100001,100001 : 0,0 ''
      IL_001b:  nop
      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
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

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0041:  nop
        IL_0042:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0044:  nop
        IL_0045:  br.s       IL_004f

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_0047:  nop
        IL_0048:  br.s       IL_004e

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004a:  nop
        IL_004b:  br.s       IL_0063

        .line 100001,100001 : 0,0 ''
        .line 100001,100001 : 0,0 ''
        IL_004d:  nop
        .line 100001,100001 : 0,0 ''
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.3
        IL_0051:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
        IL_006a:  ldarg.0
        IL_006b:  ldc.i4.0
        IL_006c:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 32,32 : 9,28 ''
        IL_007b:  ldloc.2
        IL_007c:  stloc.0
        IL_007d:  ldnull
        IL_007e:  stloc.1
        IL_007f:  leave.s    IL_0081

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_0081:  ldloc.1
      IL_0082:  pop
      .line 100001,100001 : 0,0 ''
      IL_0083:  nop
      IL_0084:  br         IL_0002

      IL_0089:  ldloc.0
      IL_008a:  ldnull
      IL_008b:  cgt.un
      IL_008d:  brfalse.s  IL_0091

      IL_008f:  br.s       IL_0093

      IL_0091:  br.s       IL_0095

      .line 100001,100001 : 0,0 ''
      IL_0093:  ldloc.0
      IL_0094:  throw

      .line 100001,100001 : 0,0 ''
      IL_0095:  ret
    } // end of method onlyOdd@32::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
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
    } // end of method onlyOdd@32::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::current
      IL_0007:  ret
    } // end of method onlyOdd@32::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  9
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldnull
      IL_0004:  ldc.i4.0
      IL_0005:  ldc.i4.0
      IL_0006:  newobj     instance void Linq101Quantifiers01/onlyOdd@32::.ctor(int32,
                                                                                int32,
                                                                                class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                int32,
                                                                                int32)
      IL_000b:  ret
    } // end of method onlyOdd@32::GetFreshEnumerator

  } // end of class onlyOdd@32

  .class auto ansi serializable nested assembly beforefieldinit 'onlyOdd@33-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'onlyOdd@33-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 33,33 : 14,23 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.2
      IL_0003:  rem
      IL_0004:  ldc.i4.1
      IL_0005:  ceq
      IL_0007:  ret
    } // end of method 'onlyOdd@33-1'::Invoke

  } // end of class 'onlyOdd@33-1'

  .class auto ansi serializable nested assembly beforefieldinit productGroups2@39
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/productGroups2@39::builder@
      IL_000d:  ret
    } // end of method productGroups2@39::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 39,39 : 9,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 40,40 : 9,32 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/productGroups2@39::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method productGroups2@39::Invoke

  } // end of class productGroups2@39

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups2@40-1'
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
    } // end of method 'productGroups2@40-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 40,40 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'productGroups2@40-1'::Invoke

  } // end of class 'productGroups2@40-1'

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups2@40-2'
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
    } // end of method 'productGroups2@40-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 40,40 : 22,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'productGroups2@40-2'::Invoke

  } // end of class 'productGroups2@40-2'

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups2@40-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/'productGroups2@40-3'::builder@
      IL_000d:  ret
    } // end of method 'productGroups2@40-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g)
      .line 40,40 : 38,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/'productGroups2@40-3'::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0011:  ret
    } // end of method 'productGroups2@40-3'::Invoke

  } // end of class 'productGroups2@40-3'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'productGroups2@41-5'
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method 'productGroups2@41-5'::.ctor

    .method assembly hidebysig instance bool 
            Invoke(class [Utils]Utils/Product arg) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      .line 41,41 : 31,49 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0007:  ldc.i4.0
      IL_0008:  cgt
      IL_000a:  ret
    } // end of method 'productGroups2@41-5'::Invoke

  } // end of class 'productGroups2@41-5'

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups2@41-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'productGroups2@41-4'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      .line 41,41 : 16,50 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  newobj     instance void Linq101Quantifiers01/'productGroups2@41-5'::.ctor()
      IL_0007:  ldftn      instance bool Linq101Quantifiers01/'productGroups2@41-5'::Invoke(class [Utils]Utils/Product)
      IL_000d:  newobj     instance void class [mscorlib]System.Func`2<class [Utils]Utils/Product,bool>::.ctor(object,
                                                                                                               native int)
      IL_0012:  call       bool [System.Core]System.Linq.Enumerable::All<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
      IL_0017:  ret
    } // end of method 'productGroups2@41-4'::Invoke

  } // end of class 'productGroups2@41-4'

  .class auto ansi serializable nested assembly beforefieldinit 'productGroups2@42-6'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'productGroups2@42-6'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      .line 42,42 : 17,25 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0007:  ldarg.1
      IL_0008:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                     !1)
      IL_000d:  ret
    } // end of method 'productGroups2@42-6'::Invoke

  } // end of class 'productGroups2@42-6'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_words() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::'words@8-6'
    IL_0005:  ret
  } // end of method Linq101Quantifiers01::get_words

  .method public specialname static bool 
          get_iAfterE() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     bool '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::iAfterE@10
    IL_0005:  ret
  } // end of method Linq101Quantifiers01::get_iAfterE

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::'products@17-10'
    IL_0005:  ret
  } // end of method Linq101Quantifiers01::get_products

  .method public specialname static class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] 
          get_productGroups() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::productGroups@19
    IL_0005:  ret
  } // end of method Linq101Quantifiers01::get_productGroups

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::'numbers@28-7'
    IL_0005:  ret
  } // end of method Linq101Quantifiers01::get_numbers

  .method public specialname static bool 
          get_onlyOdd() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     bool '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::onlyOdd@30
    IL_0005:  ret
  } // end of method Linq101Quantifiers01::get_onlyOdd

  .method public specialname static class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] 
          get_productGroups2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::productGroups2@37
    IL_0005:  ret
  } // end of method Linq101Quantifiers01::get_productGroups2

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Quantifiers01::get_words()
  } // end of property Linq101Quantifiers01::words
  .property bool iAfterE()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get bool Linq101Quantifiers01::get_iAfterE()
  } // end of property Linq101Quantifiers01::iAfterE
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Quantifiers01::get_products()
  } // end of property Linq101Quantifiers01::products
  .property class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[]
          productGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] Linq101Quantifiers01::get_productGroups()
  } // end of property Linq101Quantifiers01::productGroups
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Quantifiers01::get_numbers()
  } // end of property Linq101Quantifiers01::numbers
  .property bool onlyOdd()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get bool Linq101Quantifiers01::get_onlyOdd()
  } // end of property Linq101Quantifiers01::onlyOdd
  .property class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[]
          productGroups2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] Linq101Quantifiers01::get_productGroups2()
  } // end of property Linq101Quantifiers01::productGroups2
} // end of class Linq101Quantifiers01

.class private abstract auto ansi sealed '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 'words@8-6'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly bool iAfterE@10
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 'products@17-10'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] productGroups@19
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'numbers@28-7'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly bool onlyOdd@30
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] productGroups2@37
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       420 (0x1a4)
    .maxstack  10
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words,
             [1] bool iAfterE,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products,
             [3] class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] productGroups,
             [4] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers,
             [5] bool onlyOdd,
             [6] class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] productGroups2,
             [7] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
             [8] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_8,
             [9] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_9,
             [10] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10)
    .line 8,8 : 1,54 ''
    IL_0000:  nop
    IL_0001:  ldstr      "believe"
    IL_0006:  ldstr      "relief"
    IL_000b:  ldstr      "receipt"
    IL_0010:  ldstr      "field"
    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_001a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0024:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0029:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_002e:  dup
    IL_002f:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::'words@8-6'
    IL_0034:  stloc.0
    IL_0035:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_003a:  stloc.s    builder@
    IL_003c:  ldloc.s    builder@
    IL_003e:  ldnull
    IL_003f:  ldnull
    IL_0040:  ldnull
    IL_0041:  ldc.i4.0
    IL_0042:  ldnull
    IL_0043:  newobj     instance void Linq101Quantifiers01/iAfterE@12::.ctor(string,
                                                                              string,
                                                                              class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                              int32,
                                                                              string)
    IL_0048:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_004d:  newobj     instance void Linq101Quantifiers01/'iAfterE@13-1'::.ctor()
    IL_0052:  callvirt   instance bool [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Exists<string,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0057:  dup
    IL_0058:  stsfld     bool '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::iAfterE@10
    IL_005d:  stloc.1
    .line 17,17 : 1,32 ''
    IL_005e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_0063:  dup
    IL_0064:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::'products@17-10'
    IL_0069:  stloc.2
    .line 19,25 : 1,21 ''
    IL_006a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_006f:  stloc.s    V_8
    IL_0071:  ldloc.s    V_8
    IL_0073:  ldloc.s    V_8
    IL_0075:  ldloc.s    V_8
    IL_0077:  ldloc.s    V_8
    IL_0079:  ldloc.s    V_8
    IL_007b:  ldloc.s    V_8
    IL_007d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Quantifiers01::get_products()
    IL_0082:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0087:  ldloc.s    V_8
    IL_0089:  newobj     instance void Linq101Quantifiers01/productGroups@21::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_008e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0093:  newobj     instance void Linq101Quantifiers01/'productGroups@22-1'::.ctor()
    IL_0098:  newobj     instance void Linq101Quantifiers01/'productGroups@22-2'::.ctor()
    IL_009d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_00a2:  ldloc.s    V_8
    IL_00a4:  newobj     instance void Linq101Quantifiers01/'productGroups@22-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00a9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00ae:  newobj     instance void Linq101Quantifiers01/'productGroups@23-4'::.ctor()
    IL_00b3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_00b8:  newobj     instance void Linq101Quantifiers01/'productGroups@24-6'::.ctor()
    IL_00bd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00c2:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00c7:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00cc:  dup
    IL_00cd:  stsfld     class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::productGroups@19
    IL_00d2:  stloc.3
    .line 28,28 : 1,35 ''
    IL_00d3:  ldc.i4.1
    IL_00d4:  ldc.i4.s   11
    IL_00d6:  ldc.i4.3
    IL_00d7:  ldc.i4.s   19
    IL_00d9:  ldc.i4.s   41
    IL_00db:  ldc.i4.s   65
    IL_00dd:  ldc.i4.s   19
    IL_00df:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_00e4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ee:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00f3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00f8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00fd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0102:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0107:  dup
    IL_0108:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::'numbers@28-7'
    IL_010d:  stloc.s    numbers
    IL_010f:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0114:  stloc.s    V_9
    IL_0116:  ldloc.s    V_9
    IL_0118:  ldc.i4.0
    IL_0119:  ldc.i4.0
    IL_011a:  ldnull
    IL_011b:  ldc.i4.0
    IL_011c:  ldc.i4.0
    IL_011d:  newobj     instance void Linq101Quantifiers01/onlyOdd@32::.ctor(int32,
                                                                              int32,
                                                                              class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
    IL_0122:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0127:  newobj     instance void Linq101Quantifiers01/'onlyOdd@33-1'::.ctor()
    IL_012c:  callvirt   instance bool [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::All<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0131:  dup
    IL_0132:  stsfld     bool '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::onlyOdd@30
    IL_0137:  stloc.s    onlyOdd
    .line 37,43 : 1,21 ''
    IL_0139:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_013e:  stloc.s    V_10
    IL_0140:  ldloc.s    V_10
    IL_0142:  ldloc.s    V_10
    IL_0144:  ldloc.s    V_10
    IL_0146:  ldloc.s    V_10
    IL_0148:  ldloc.s    V_10
    IL_014a:  ldloc.s    V_10
    IL_014c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Quantifiers01::get_products()
    IL_0151:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0156:  ldloc.s    V_10
    IL_0158:  newobj     instance void Linq101Quantifiers01/productGroups2@39::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_015d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0162:  newobj     instance void Linq101Quantifiers01/'productGroups2@40-1'::.ctor()
    IL_0167:  newobj     instance void Linq101Quantifiers01/'productGroups2@40-2'::.ctor()
    IL_016c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0171:  ldloc.s    V_10
    IL_0173:  newobj     instance void Linq101Quantifiers01/'productGroups2@40-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0178:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_017d:  newobj     instance void Linq101Quantifiers01/'productGroups2@41-4'::.ctor()
    IL_0182:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0187:  newobj     instance void Linq101Quantifiers01/'productGroups2@42-6'::.ctor()
    IL_018c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0191:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0196:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_019b:  dup
    IL_019c:  stsfld     class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::productGroups2@37
    IL_01a1:  stloc.s    productGroups2
    IL_01a3:  ret
  } // end of method $Linq101Quantifiers01::main@

} // end of class '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
