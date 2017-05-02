
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
.assembly Linq101ElementOperators01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101ElementOperators01
{
  // Offset: 0x00000000 Length: 0x00000386
}
.mresource public FSharpOptimizationData.Linq101ElementOperators01
{
  // Offset: 0x00000390 Length: 0x00000127
}
.module Linq101ElementOperators01.exe
// MVID: {590846DB-19D7-C20D-A745-0383DB460859}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00CD0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101ElementOperators01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname products12@12
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
      IL_0002:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::p
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101ElementOperators01/products12@12::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101ElementOperators01/products12@12::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_002b:  ret
    } // end of method products12@12::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      // Code size       191 (0xbf)
      .maxstack  6
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\src\\manofstick\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101ElementOperators01.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101ElementOperators01/products12@12::pc
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
      .line 12,12 : 9,29 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101ElementOperators01::get_products()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101ElementOperators01/products12@12::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101ElementOperators01/products12@12::pc
      .line 12,12 : 9,29 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101ElementOperators01/products12@12::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101ElementOperators01/products12@12::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_005e:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::_arg1
      .line 12,12 : 9,29 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::_arg1
      IL_006a:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::p
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101ElementOperators01/products12@12::pc
      .line 13,13 : 9,33 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::p
      IL_007d:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldnull
      IL_0086:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::p
      .line 12,12 : 9,29 ''
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101ElementOperators01/products12@12::pc
      .line 12,12 : 9,29 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101ElementOperators01/products12@12::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101ElementOperators01/products12@12::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101ElementOperators01/products12@12::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldnull
      IL_00b8:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
    } // end of method products12@12::GenerateNext

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
      IL_0003:  ldfld      int32 Linq101ElementOperators01/products12@12::pc
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
        IL_001d:  ldfld      int32 Linq101ElementOperators01/products12@12::pc
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
        IL_0051:  stfld      int32 Linq101ElementOperators01/products12@12::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> Linq101ElementOperators01/products12@12::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101ElementOperators01/products12@12::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 12,12 : 9,29 ''
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
    } // end of method products12@12::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101ElementOperators01/products12@12::pc
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
    } // end of method products12@12::get_CheckClose

    .method public strict virtual instance class [Utils]Utils/Product 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [Utils]Utils/Product Linq101ElementOperators01/products12@12::current
      IL_0007:  ret
    } // end of method products12@12::get_LastGenerated

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
      IL_0006:  newobj     instance void Linq101ElementOperators01/products12@12::.ctor(class [Utils]Utils/Product,
                                                                                        class [Utils]Utils/Product,
                                                                                        class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                        int32,
                                                                                        class [Utils]Utils/Product)
      IL_000b:  ret
    } // end of method products12@12::GetFreshEnumerator

  } // end of class products12@12

  .class auto ansi serializable nested assembly beforefieldinit 'products12@13-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'products12@13-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      .line 13,13 : 16,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance int32 [Utils]Utils/Product::get_ProductID()
      IL_0007:  ldc.i4.s   12
      IL_0009:  ceq
      IL_000b:  ret
    } // end of method 'products12@13-1'::Invoke

  } // end of class 'products12@13-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname startsWithO@22
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public string _arg1
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string s
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
                                 string s,
                                 class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string Linq101ElementOperators01/startsWithO@22::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      string Linq101ElementOperators01/startsWithO@22::s
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101ElementOperators01/startsWithO@22::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101ElementOperators01/startsWithO@22::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      string Linq101ElementOperators01/startsWithO@22::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_002b:  ret
    } // end of method startsWithO@22::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       191 (0xbf)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101ElementOperators01/startsWithO@22::pc
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
      .line 22,22 : 9,28 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101ElementOperators01::get_strings()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101ElementOperators01/startsWithO@22::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101ElementOperators01/startsWithO@22::pc
      .line 22,22 : 9,28 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101ElementOperators01/startsWithO@22::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101ElementOperators01/startsWithO@22::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_005e:  stfld      string Linq101ElementOperators01/startsWithO@22::_arg1
      .line 22,22 : 9,28 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      string Linq101ElementOperators01/startsWithO@22::_arg1
      IL_006a:  stfld      string Linq101ElementOperators01/startsWithO@22::s
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101ElementOperators01/startsWithO@22::pc
      .line 23,23 : 9,28 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      string Linq101ElementOperators01/startsWithO@22::s
      IL_007d:  stfld      string Linq101ElementOperators01/startsWithO@22::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldnull
      IL_0086:  stfld      string Linq101ElementOperators01/startsWithO@22::s
      .line 22,22 : 9,28 ''
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string Linq101ElementOperators01/startsWithO@22::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101ElementOperators01/startsWithO@22::pc
      .line 22,22 : 9,28 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101ElementOperators01/startsWithO@22::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101ElementOperators01/startsWithO@22::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101ElementOperators01/startsWithO@22::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldnull
      IL_00b8:  stfld      string Linq101ElementOperators01/startsWithO@22::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
    } // end of method startsWithO@22::GenerateNext

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
      IL_0003:  ldfld      int32 Linq101ElementOperators01/startsWithO@22::pc
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
        IL_001d:  ldfld      int32 Linq101ElementOperators01/startsWithO@22::pc
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
        IL_0051:  stfld      int32 Linq101ElementOperators01/startsWithO@22::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101ElementOperators01/startsWithO@22::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101ElementOperators01/startsWithO@22::pc
        IL_006a:  ldarg.0
        IL_006b:  ldnull
        IL_006c:  stfld      string Linq101ElementOperators01/startsWithO@22::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 22,22 : 9,28 ''
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
    } // end of method startsWithO@22::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101ElementOperators01/startsWithO@22::pc
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
    } // end of method startsWithO@22::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string Linq101ElementOperators01/startsWithO@22::current
      IL_0007:  ret
    } // end of method startsWithO@22::get_LastGenerated

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
      IL_0006:  newobj     instance void Linq101ElementOperators01/startsWithO@22::.ctor(string,
                                                                                         string,
                                                                                         class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                         int32,
                                                                                         string)
      IL_000b:  ret
    } // end of method startsWithO@22::GetFreshEnumerator

  } // end of class startsWithO@22

  .class auto ansi serializable nested assembly beforefieldinit 'startsWithO@23-1'
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
    } // end of method 'startsWithO@23-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(string s) cil managed
    {
      // Code size       13 (0xd)
      .maxstack  8
      .line 23,23 : 16,27 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.0
      IL_0003:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
      IL_0008:  ldc.i4.s   111
      IL_000a:  ceq
      IL_000c:  ret
    } // end of method 'startsWithO@23-1'::Invoke

  } // end of class 'startsWithO@23-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname firstNumOrDefault@31
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
      IL_0002:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::n
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/firstNumOrDefault@31::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_002b:  ret
    } // end of method firstNumOrDefault@31::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       191 (0xbf)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
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
      .line 31,31 : 9,28 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101ElementOperators01::get_numbers()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/firstNumOrDefault@31::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
      .line 31,31 : 9,28 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/firstNumOrDefault@31::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/firstNumOrDefault@31::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::_arg1
      .line 31,31 : 9,28 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::_arg1
      IL_006a:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::n
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
      .line 32,32 : 9,22 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::n
      IL_007d:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.0
      IL_0086:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::n
      .line 31,31 : 9,28 ''
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
      .line 31,31 : 9,28 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/firstNumOrDefault@31::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/firstNumOrDefault@31::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4.0
      IL_00b8:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
    } // end of method firstNumOrDefault@31::GenerateNext

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
      IL_0003:  ldfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
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
        IL_001d:  ldfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
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
        IL_0051:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/firstNumOrDefault@31::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
        IL_006a:  ldarg.0
        IL_006b:  ldc.i4.0
        IL_006c:  stfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 31,31 : 9,28 ''
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
    } // end of method firstNumOrDefault@31::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::pc
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
    } // end of method firstNumOrDefault@31::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101ElementOperators01/firstNumOrDefault@31::current
      IL_0007:  ret
    } // end of method firstNumOrDefault@31::get_LastGenerated

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
      IL_0006:  newobj     instance void Linq101ElementOperators01/firstNumOrDefault@31::.ctor(int32,
                                                                                               int32,
                                                                                               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                               int32,
                                                                                               int32)
      IL_000b:  ret
    } // end of method firstNumOrDefault@31::GetFreshEnumerator

  } // end of class firstNumOrDefault@31

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname fourthLowNum@52
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
      IL_0002:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::_arg1
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::n
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/fourthLowNum@52::'enum'
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_002b:  ret
    } // end of method fourthLowNum@52::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       191 (0xbf)
      .maxstack  6
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
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
      .line 52,52 : 9,29 ''
      IL_002e:  ldarg.0
      IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101ElementOperators01::get_numbers2()
      IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/fourthLowNum@52::'enum'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
      .line 52,52 : 9,29 ''
      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/fourthLowNum@52::'enum'
      IL_004b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0050:  brfalse.s  IL_0095

      IL_0052:  ldarg.0
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/fourthLowNum@52::'enum'
      IL_0059:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005e:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::_arg1
      .line 52,52 : 9,29 ''
      IL_0063:  ldarg.0
      IL_0064:  ldarg.0
      IL_0065:  ldfld      int32 Linq101ElementOperators01/fourthLowNum@52::_arg1
      IL_006a:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::n
      IL_006f:  ldarg.0
      IL_0070:  ldc.i4.2
      IL_0071:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
      .line 53,53 : 9,22 ''
      IL_0076:  ldarg.0
      IL_0077:  ldarg.0
      IL_0078:  ldfld      int32 Linq101ElementOperators01/fourthLowNum@52::n
      IL_007d:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::current
      IL_0082:  ldc.i4.1
      IL_0083:  ret

      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.0
      IL_0086:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::n
      .line 52,52 : 9,29 ''
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::_arg1
      .line 100001,100001 : 0,0 ''
      IL_0092:  nop
      IL_0093:  br.s       IL_0045

      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
      .line 52,52 : 9,29 ''
      IL_009c:  ldarg.0
      IL_009d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/fourthLowNum@52::'enum'
      IL_00a2:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_00a7:  nop
      IL_00a8:  ldarg.0
      IL_00a9:  ldnull
      IL_00aa:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/fourthLowNum@52::'enum'
      IL_00af:  ldarg.0
      IL_00b0:  ldc.i4.3
      IL_00b1:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4.0
      IL_00b8:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::current
      IL_00bd:  ldc.i4.0
      IL_00be:  ret
    } // end of method fourthLowNum@52::GenerateNext

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
      IL_0003:  ldfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
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
        IL_001d:  ldfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
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
        IL_0051:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101ElementOperators01/fourthLowNum@52::'enum'
        IL_005c:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0061:  nop
        .line 100001,100001 : 0,0 ''
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldc.i4.3
        IL_0065:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
        IL_006a:  ldarg.0
        IL_006b:  ldc.i4.0
        IL_006c:  stfld      int32 Linq101ElementOperators01/fourthLowNum@52::current
        IL_0071:  ldnull
        IL_0072:  stloc.1
        IL_0073:  leave.s    IL_0081

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0075:  castclass  [mscorlib]System.Exception
        IL_007a:  stloc.2
        .line 52,52 : 9,29 ''
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
    } // end of method fourthLowNum@52::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       57 (0x39)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101ElementOperators01/fourthLowNum@52::pc
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
    } // end of method fourthLowNum@52::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32 Linq101ElementOperators01/fourthLowNum@52::current
      IL_0007:  ret
    } // end of method fourthLowNum@52::get_LastGenerated

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
      IL_0006:  newobj     instance void Linq101ElementOperators01/fourthLowNum@52::.ctor(int32,
                                                                                          int32,
                                                                                          class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                          int32,
                                                                                          int32)
      IL_000b:  ret
    } // end of method fourthLowNum@52::GetFreshEnumerator

  } // end of class fourthLowNum@52

  .class auto ansi serializable nested assembly beforefieldinit 'fourthLowNum@53-1'
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
    } // end of method 'fourthLowNum@53-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      .line 53,53 : 16,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.5
      IL_0003:  cgt
      IL_0005:  ret
    } // end of method 'fourthLowNum@53-1'::Invoke

  } // end of class 'fourthLowNum@53-1'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::'products@8-2'
    IL_0005:  ret
  } // end of method Linq101ElementOperators01::get_products

  .method public specialname static class [Utils]Utils/Product 
          get_products12() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Utils]Utils/Product '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::products12@10
    IL_0005:  ret
  } // end of method Linq101ElementOperators01::get_products12

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_strings() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::strings@18
    IL_0005:  ret
  } // end of method Linq101ElementOperators01::get_strings

  .method public specialname static string 
          get_startsWithO() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     string '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::startsWithO@20
    IL_0005:  ret
  } // end of method Linq101ElementOperators01::get_startsWithO

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0006:  ret
  } // end of method Linq101ElementOperators01::get_numbers

  .method public specialname static int32 
          get_firstNumOrDefault() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::firstNumOrDefault@29
    IL_0005:  ret
  } // end of method Linq101ElementOperators01::get_firstNumOrDefault

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::'numbers2@48-2'
    IL_0005:  ret
  } // end of method Linq101ElementOperators01::get_numbers2

  .method public specialname static int32 
          get_fourthLowNum() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::fourthLowNum@50
    IL_0005:  ret
  } // end of method Linq101ElementOperators01::get_fourthLowNum

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101ElementOperators01::get_products()
  } // end of property Linq101ElementOperators01::products
  .property class [Utils]Utils/Product products12()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [Utils]Utils/Product Linq101ElementOperators01::get_products12()
  } // end of property Linq101ElementOperators01::products12
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          strings()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101ElementOperators01::get_strings()
  } // end of property Linq101ElementOperators01::strings
  .property string startsWithO()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get string Linq101ElementOperators01::get_startsWithO()
  } // end of property Linq101ElementOperators01::startsWithO
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101ElementOperators01::get_numbers()
  } // end of property Linq101ElementOperators01::numbers
  .property int32 firstNumOrDefault()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101ElementOperators01::get_firstNumOrDefault()
  } // end of property Linq101ElementOperators01::firstNumOrDefault
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101ElementOperators01::get_numbers2()
  } // end of property Linq101ElementOperators01::numbers2
  .property int32 fourthLowNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 Linq101ElementOperators01::get_fourthLowNum()
  } // end of property Linq101ElementOperators01::fourthLowNum
} // end of class Linq101ElementOperators01

.class private abstract auto ansi sealed '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 'products@8-2'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [Utils]Utils/Product products12@10
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> strings@18
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly string startsWithO@20
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 firstNumOrDefault@29
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'numbers2@48-2'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 fourthLowNum@50
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       390 (0x186)
    .maxstack  13
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products,
             [1] class [Utils]Utils/Product products12,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> strings,
             [3] string startsWithO,
             [4] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers,
             [5] int32 firstNumOrDefault,
             [6] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers2,
             [7] int32 fourthLowNum,
             [8] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
             [9] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_9,
             [10] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             [11] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11)
    .line 8,8 : 1,32 ''
    IL_0000:  nop
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_0006:  dup
    IL_0007:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::'products@8-2'
    IL_000c:  stloc.0
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0012:  stloc.s    builder@
    IL_0014:  ldloc.s    builder@
    IL_0016:  ldloc.s    builder@
    IL_0018:  ldnull
    IL_0019:  ldnull
    IL_001a:  ldnull
    IL_001b:  ldc.i4.0
    IL_001c:  ldnull
    IL_001d:  newobj     instance void Linq101ElementOperators01/products12@12::.ctor(class [Utils]Utils/Product,
                                                                                      class [Utils]Utils/Product,
                                                                                      class [mscorlib]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                      int32,
                                                                                      class [Utils]Utils/Product)
    IL_0022:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0027:  newobj     instance void Linq101ElementOperators01/'products12@13-1'::.ctor()
    IL_002c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0031:  callvirt   instance !!0 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Head<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>)
    IL_0036:  dup
    IL_0037:  stsfld     class [Utils]Utils/Product '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::products12@10
    IL_003c:  stloc.1
    .line 18,18 : 1,97 ''
    IL_003d:  ldstr      "zero"
    IL_0042:  ldstr      "one"
    IL_0047:  ldstr      "two"
    IL_004c:  ldstr      "three"
    IL_0051:  ldstr      "four"
    IL_0056:  ldstr      "five"
    IL_005b:  ldstr      "six"
    IL_0060:  ldstr      "seven"
    IL_0065:  ldstr      "eight"
    IL_006a:  ldstr      "nine"
    IL_006f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0074:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0079:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_007e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0083:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0088:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_008d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0092:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0097:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_009c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00a1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00a6:  dup
    IL_00a7:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::strings@18
    IL_00ac:  stloc.2
    IL_00ad:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00b2:  stloc.s    V_9
    IL_00b4:  ldloc.s    V_9
    IL_00b6:  ldloc.s    V_9
    IL_00b8:  ldnull
    IL_00b9:  ldnull
    IL_00ba:  ldnull
    IL_00bb:  ldc.i4.0
    IL_00bc:  ldnull
    IL_00bd:  newobj     instance void Linq101ElementOperators01/startsWithO@22::.ctor(string,
                                                                                       string,
                                                                                       class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                       int32,
                                                                                       string)
    IL_00c2:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00c7:  newobj     instance void Linq101ElementOperators01/'startsWithO@23-1'::.ctor()
    IL_00cc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<string,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_00d1:  callvirt   instance !!0 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Head<string,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>)
    IL_00d6:  dup
    IL_00d7:  stsfld     string '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::startsWithO@20
    IL_00dc:  stloc.3
    .line 28,28 : 1,28 ''
    IL_00dd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101ElementOperators01::get_numbers()
    IL_00e2:  stloc.s    numbers
    IL_00e4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00e9:  stloc.s    V_10
    IL_00eb:  ldloc.s    V_10
    IL_00ed:  ldc.i4.0
    IL_00ee:  ldc.i4.0
    IL_00ef:  ldnull
    IL_00f0:  ldc.i4.0
    IL_00f1:  ldc.i4.0
    IL_00f2:  newobj     instance void Linq101ElementOperators01/firstNumOrDefault@31::.ctor(int32,
                                                                                             int32,
                                                                                             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                             int32,
                                                                                             int32)
    IL_00f7:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00fc:  callvirt   instance !!0 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::HeadOrDefault<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>)
    IL_0101:  dup
    IL_0102:  stsfld     int32 '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::firstNumOrDefault@29
    IL_0107:  stloc.s    firstNumOrDefault
    .line 48,48 : 1,48 ''
    IL_0109:  ldc.i4.5
    IL_010a:  ldc.i4.4
    IL_010b:  ldc.i4.1
    IL_010c:  ldc.i4.3
    IL_010d:  ldc.i4.s   9
    IL_010f:  ldc.i4.8
    IL_0110:  ldc.i4.6
    IL_0111:  ldc.i4.7
    IL_0112:  ldc.i4.2
    IL_0113:  ldc.i4.0
    IL_0114:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0119:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_011e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0123:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0128:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_012d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0132:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0137:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0141:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0146:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_014b:  dup
    IL_014c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::'numbers2@48-2'
    IL_0151:  stloc.s    numbers2
    IL_0153:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0158:  stloc.s    V_11
    IL_015a:  ldloc.s    V_11
    IL_015c:  ldloc.s    V_11
    IL_015e:  ldc.i4.0
    IL_015f:  ldc.i4.0
    IL_0160:  ldnull
    IL_0161:  ldc.i4.0
    IL_0162:  ldc.i4.0
    IL_0163:  newobj     instance void Linq101ElementOperators01/fourthLowNum@52::.ctor(int32,
                                                                                        int32,
                                                                                        class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                        int32,
                                                                                        int32)
    IL_0168:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_016d:  newobj     instance void Linq101ElementOperators01/'fourthLowNum@53-1'::.ctor()
    IL_0172:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0177:  ldc.i4.1
    IL_0178:  callvirt   instance !!0 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Nth<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                 int32)
    IL_017d:  dup
    IL_017e:  stsfld     int32 '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01::fourthLowNum@50
    IL_0183:  stloc.s    fourthLowNum
    IL_0185:  ret
  } // end of method $Linq101ElementOperators01::main@

} // end of class '<StartupCode$Linq101ElementOperators01>'.$Linq101ElementOperators01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
