
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
  .ver 4:5:0:0
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
  // Offset: 0x00000000 Length: 0x0000039F
}
.mresource public FSharpOptimizationData.Linq101Quantifiers01
{
  // Offset: 0x000003A8 Length: 0x000000FF
}
.module Linq101Quantifiers01.exe
// MVID: {5B9A632A-76DD-E373-A745-03832A639A5B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x025D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Quantifiers01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname iAfterE@12
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
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Quantifiers01/iAfterE@12::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method iAfterE@12::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       154 (0x9a)
      .maxstack  6
      .locals init ([0] string V_0,
               [1] string w)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 100001,100001 : 0,0 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Quantifiers01.fs'
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Quantifiers01/iAfterE@12::pc
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
      IL_0022:  br.s       IL_0070

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_006d

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_0091

      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      .line 12,12 : 9,26 ''
      IL_002b:  ldarg.0
      IL_002c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Quantifiers01::get_words()
      IL_0031:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0036:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.1
      IL_003d:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      .line 12,12 : 9,26 ''
      IL_0042:  ldarg.0
      IL_0043:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_0048:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_004d:  brfalse.s  IL_0070

      IL_004f:  ldarg.0
      IL_0050:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_0055:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_005a:  stloc.0
      .line 12,12 : 9,26 ''
      IL_005b:  ldloc.0
      IL_005c:  stloc.1
      IL_005d:  ldarg.0
      IL_005e:  ldc.i4.2
      IL_005f:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      .line 13,13 : 9,34 ''
      IL_0064:  ldarg.0
      IL_0065:  ldloc.1
      IL_0066:  stfld      string Linq101Quantifiers01/iAfterE@12::current
      IL_006b:  ldc.i4.1
      IL_006c:  ret

      .line 100001,100001 : 0,0 ''
      IL_006d:  nop
      IL_006e:  br.s       IL_0042

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.3
      IL_0072:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      .line 12,12 : 9,26 ''
      IL_0077:  ldarg.0
      IL_0078:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_007d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_0082:  nop
      IL_0083:  ldarg.0
      IL_0084:  ldnull
      IL_0085:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.3
      IL_008c:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      IL_0091:  ldarg.0
      IL_0092:  ldnull
      IL_0093:  stfld      string Linq101Quantifiers01/iAfterE@12::current
      IL_0098:  ldc.i4.0
      IL_0099:  ret
    } // end of method iAfterE@12::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Quantifiers01/iAfterE@12::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0019

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br         IL_0087

      .line 100001,100001 : 0,0 ''
      IL_0019:  nop
      .try
      {
        IL_001a:  ldarg.0
        IL_001b:  ldfld      int32 Linq101Quantifiers01/iAfterE@12::pc
        IL_0020:  switch     ( 
                              IL_0037,
                              IL_0039,
                              IL_003b,
                              IL_003d)
        IL_0035:  br.s       IL_004b

        IL_0037:  br.s       IL_003f

        IL_0039:  br.s       IL_0042

        IL_003b:  br.s       IL_0045

        IL_003d:  br.s       IL_0048

        .line 100001,100001 : 0,0 ''
        IL_003f:  nop
        IL_0040:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_0042:  nop
        IL_0043:  br.s       IL_004d

        .line 100001,100001 : 0,0 ''
        IL_0045:  nop
        IL_0046:  br.s       IL_004c

        .line 100001,100001 : 0,0 ''
        IL_0048:  nop
        IL_0049:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_004b:  nop
        .line 100001,100001 : 0,0 ''
        IL_004c:  nop
        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.3
        IL_004f:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Quantifiers01/iAfterE@12::'enum'
        IL_005a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_005f:  nop
        .line 100001,100001 : 0,0 ''
        IL_0060:  nop
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.3
        IL_0063:  stfld      int32 Linq101Quantifiers01/iAfterE@12::pc
        IL_0068:  ldarg.0
        IL_0069:  ldnull
        IL_006a:  stfld      string Linq101Quantifiers01/iAfterE@12::current
        IL_006f:  ldnull
        IL_0070:  stloc.1
        IL_0071:  leave.s    IL_007f

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0073:  castclass  [mscorlib]System.Exception
        IL_0078:  stloc.2
        .line 12,12 : 9,26 ''
        IL_0079:  ldloc.2
        IL_007a:  stloc.0
        IL_007b:  ldnull
        IL_007c:  stloc.1
        IL_007d:  leave.s    IL_007f

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_007f:  ldloc.1
      IL_0080:  pop
      .line 100001,100001 : 0,0 ''
      IL_0081:  nop
      IL_0082:  br         IL_0000

      IL_0087:  ldloc.0
      IL_0088:  ldnull
      IL_0089:  cgt.un
      IL_008b:  brfalse.s  IL_008f

      IL_008d:  br.s       IL_0091

      IL_008f:  br.s       IL_0093

      .line 100001,100001 : 0,0 ''
      IL_0091:  ldloc.0
      IL_0092:  throw

      .line 100001,100001 : 0,0 ''
      IL_0093:  ret
    } // end of method iAfterE@12::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Quantifiers01/iAfterE@12::pc
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
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method iAfterE@12::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Quantifiers01/iAfterE@12::current
      IL_0006:  ret
    } // end of method iAfterE@12::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Quantifiers01/iAfterE@12::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
      IL_0008:  ret
    } // end of method iAfterE@12::GetFreshEnumerator

  } // end of class iAfterE@12

  .class auto ansi serializable sealed nested assembly beforefieldinit 'iAfterE@13-1'
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
      // Code size       12 (0xc)
      .maxstack  8
      .line 13,13 : 17,33 ''
      IL_0000:  ldarg.1
      IL_0001:  ldstr      "ei"
      IL_0006:  callvirt   instance bool [mscorlib]System.String::Contains(string)
      IL_000b:  ret
    } // end of method 'iAfterE@13-1'::Invoke

  } // end of class 'iAfterE@13-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit productGroups@21
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/productGroups@21::builder@
      IL_000d:  ret
    } // end of method productGroups@21::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 21,21 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 22,22 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/productGroups@21::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method productGroups@21::Invoke

  } // end of class productGroups@21

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups@22-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 22,22 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'productGroups@22-1'::Invoke

  } // end of class 'productGroups@22-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups@22-2'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 22,22 : 22,32 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'productGroups@22-2'::Invoke

  } // end of class 'productGroups@22-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups@22-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/'productGroups@22-3'::builder@
      IL_000d:  ret
    } // end of method 'productGroups@22-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g)
      .line 22,22 : 38,39 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/'productGroups@22-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0010:  ret
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
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 23,23 : 31,49 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method 'productGroups@23-5'::Invoke

  } // end of class 'productGroups@23-5'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups@23-4'
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
      // Code size       23 (0x17)
      .maxstack  8
      .line 23,23 : 16,50 ''
      IL_0000:  ldarg.1
      IL_0001:  newobj     instance void Linq101Quantifiers01/'productGroups@23-5'::.ctor()
      IL_0006:  ldftn      instance bool Linq101Quantifiers01/'productGroups@23-5'::Invoke(class [Utils]Utils/Product)
      IL_000c:  newobj     instance void class [mscorlib]System.Func`2<class [Utils]Utils/Product,bool>::.ctor(object,
                                                                                                               native int)
      IL_0011:  call       bool [System.Core]System.Linq.Enumerable::Any<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
      IL_0016:  ret
    } // end of method 'productGroups@23-4'::Invoke

  } // end of class 'productGroups@23-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups@24-6'
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
      // Code size       13 (0xd)
      .maxstack  8
      .line 24,24 : 17,25 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                     !1)
      IL_000c:  ret
    } // end of method 'productGroups@24-6'::Invoke

  } // end of class 'productGroups@24-6'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname onlyOdd@32
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } // end of method onlyOdd@32::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       154 (0x9a)
      .maxstack  6
      .locals init ([0] int32 V_0,
               [1] int32 n)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
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
      IL_0022:  br.s       IL_0070

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_006d

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_0091

      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      .line 32,32 : 9,28 ''
      IL_002b:  ldarg.0
      IL_002c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Quantifiers01::get_numbers()
      IL_0031:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0036:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.1
      IL_003d:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      .line 32,32 : 9,28 ''
      IL_0042:  ldarg.0
      IL_0043:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_0048:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_004d:  brfalse.s  IL_0070

      IL_004f:  ldarg.0
      IL_0050:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_0055:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_005a:  stloc.0
      .line 32,32 : 9,28 ''
      IL_005b:  ldloc.0
      IL_005c:  stloc.1
      IL_005d:  ldarg.0
      IL_005e:  ldc.i4.2
      IL_005f:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      .line 33,33 : 9,24 ''
      IL_0064:  ldarg.0
      IL_0065:  ldloc.1
      IL_0066:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::current
      IL_006b:  ldc.i4.1
      IL_006c:  ret

      .line 100001,100001 : 0,0 ''
      IL_006d:  nop
      IL_006e:  br.s       IL_0042

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.3
      IL_0072:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      .line 32,32 : 9,28 ''
      IL_0077:  ldarg.0
      IL_0078:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_007d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0082:  nop
      IL_0083:  ldarg.0
      IL_0084:  ldnull
      IL_0085:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.3
      IL_008c:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      IL_0091:  ldarg.0
      IL_0092:  ldc.i4.0
      IL_0093:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::current
      IL_0098:  ldc.i4.0
      IL_0099:  ret
    } // end of method onlyOdd@32::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       148 (0x94)
      .maxstack  6
      .locals init ([0] class [mscorlib]System.Exception V_0,
               [1] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_1,
               [2] class [mscorlib]System.Exception e)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0019

      .line 100001,100001 : 0,0 ''
      IL_0013:  nop
      IL_0014:  br         IL_0087

      .line 100001,100001 : 0,0 ''
      IL_0019:  nop
      .try
      {
        IL_001a:  ldarg.0
        IL_001b:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
        IL_0020:  switch     ( 
                              IL_0037,
                              IL_0039,
                              IL_003b,
                              IL_003d)
        IL_0035:  br.s       IL_004b

        IL_0037:  br.s       IL_003f

        IL_0039:  br.s       IL_0042

        IL_003b:  br.s       IL_0045

        IL_003d:  br.s       IL_0048

        .line 100001,100001 : 0,0 ''
        IL_003f:  nop
        IL_0040:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_0042:  nop
        IL_0043:  br.s       IL_004d

        .line 100001,100001 : 0,0 ''
        IL_0045:  nop
        IL_0046:  br.s       IL_004c

        .line 100001,100001 : 0,0 ''
        IL_0048:  nop
        IL_0049:  br.s       IL_0061

        .line 100001,100001 : 0,0 ''
        IL_004b:  nop
        .line 100001,100001 : 0,0 ''
        IL_004c:  nop
        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.3
        IL_004f:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> Linq101Quantifiers01/onlyOdd@32::'enum'
        IL_005a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_005f:  nop
        .line 100001,100001 : 0,0 ''
        IL_0060:  nop
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.3
        IL_0063:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.0
        IL_006a:  stfld      int32 Linq101Quantifiers01/onlyOdd@32::current
        IL_006f:  ldnull
        IL_0070:  stloc.1
        IL_0071:  leave.s    IL_007f

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0073:  castclass  [mscorlib]System.Exception
        IL_0078:  stloc.2
        .line 32,32 : 9,28 ''
        IL_0079:  ldloc.2
        IL_007a:  stloc.0
        IL_007b:  ldnull
        IL_007c:  stloc.1
        IL_007d:  leave.s    IL_007f

        .line 100001,100001 : 0,0 ''
      }  // end handler
      IL_007f:  ldloc.1
      IL_0080:  pop
      .line 100001,100001 : 0,0 ''
      IL_0081:  nop
      IL_0082:  br         IL_0000

      IL_0087:  ldloc.0
      IL_0088:  ldnull
      IL_0089:  cgt.un
      IL_008b:  brfalse.s  IL_008f

      IL_008d:  br.s       IL_0091

      IL_008f:  br.s       IL_0093

      .line 100001,100001 : 0,0 ''
      IL_0091:  ldloc.0
      IL_0092:  throw

      .line 100001,100001 : 0,0 ''
      IL_0093:  ret
    } // end of method onlyOdd@32::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::pc
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
      IL_0032:  ldc.i4.1
      IL_0033:  ret

      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method onlyOdd@32::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Quantifiers01/onlyOdd@32::current
      IL_0006:  ret
    } // end of method onlyOdd@32::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void Linq101Quantifiers01/onlyOdd@32::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                int32,
                                                                                int32)
      IL_0008:  ret
    } // end of method onlyOdd@32::GetFreshEnumerator

  } // end of class onlyOdd@32

  .class auto ansi serializable sealed nested assembly beforefieldinit 'onlyOdd@33-1'
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
      // Code size       7 (0x7)
      .maxstack  8
      .line 33,33 : 14,23 ''
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.2
      IL_0002:  rem
      IL_0003:  ldc.i4.1
      IL_0004:  ceq
      IL_0006:  ret
    } // end of method 'onlyOdd@33-1'::Invoke

  } // end of class 'onlyOdd@33-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit productGroups2@39
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/productGroups2@39::builder@
      IL_000d:  ret
    } // end of method productGroups2@39::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 39,39 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 40,40 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/productGroups2@39::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method productGroups2@39::Invoke

  } // end of class productGroups2@39

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups2@40-1'
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 40,40 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'productGroups2@40-1'::Invoke

  } // end of class 'productGroups2@40-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups2@40-2'
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
      // Code size       9 (0x9)
      .maxstack  8
      .line 40,40 : 22,32 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'productGroups2@40-2'::Invoke

  } // end of class 'productGroups2@40-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups2@40-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/'productGroups2@40-3'::builder@
      IL_000d:  ret
    } // end of method 'productGroups2@40-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g)
      .line 40,40 : 38,39 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Quantifiers01/'productGroups2@40-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0010:  ret
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
            Invoke(class [Utils]Utils/Product x) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 41,41 : 31,49 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0006:  ldc.i4.0
      IL_0007:  cgt
      IL_0009:  ret
    } // end of method 'productGroups2@41-5'::Invoke

  } // end of class 'productGroups2@41-5'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups2@41-4'
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
      // Code size       23 (0x17)
      .maxstack  8
      .line 41,41 : 16,50 ''
      IL_0000:  ldarg.1
      IL_0001:  newobj     instance void Linq101Quantifiers01/'productGroups2@41-5'::.ctor()
      IL_0006:  ldftn      instance bool Linq101Quantifiers01/'productGroups2@41-5'::Invoke(class [Utils]Utils/Product)
      IL_000c:  newobj     instance void class [mscorlib]System.Func`2<class [Utils]Utils/Product,bool>::.ctor(object,
                                                                                                               native int)
      IL_0011:  call       bool [System.Core]System.Linq.Enumerable::All<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
      IL_0016:  ret
    } // end of method 'productGroups2@41-4'::Invoke

  } // end of class 'productGroups2@41-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productGroups2@42-6'
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
      // Code size       13 (0xd)
      .maxstack  8
      .line 42,42 : 17,25 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                     !1)
      IL_000c:  ret
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
    // Code size       407 (0x197)
    .maxstack  10
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words,
             [1] bool iAfterE,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products,
             [3] class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] productGroups,
             [4] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers,
             [5] bool onlyOdd,
             [6] class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] productGroups2,
             [7] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_7,
             [8] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_8)
    .line 8,8 : 1,54 ''
    IL_0000:  ldstr      "believe"
    IL_0005:  ldstr      "relief"
    IL_000a:  ldstr      "receipt"
    IL_000f:  ldstr      "field"
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0023:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0028:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_002d:  dup
    IL_002e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::'words@8-6'
    IL_0033:  stloc.0
    IL_0034:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0039:  ldnull
    IL_003a:  ldc.i4.0
    IL_003b:  ldnull
    IL_003c:  newobj     instance void Linq101Quantifiers01/iAfterE@12::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                              int32,
                                                                              string)
    IL_0041:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0046:  newobj     instance void Linq101Quantifiers01/'iAfterE@13-1'::.ctor()
    IL_004b:  callvirt   instance bool [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Exists<string,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0050:  dup
    IL_0051:  stsfld     bool '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::iAfterE@10
    IL_0056:  stloc.1
    .line 17,17 : 1,32 ''
    IL_0057:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_005c:  dup
    IL_005d:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::'products@17-10'
    IL_0062:  stloc.2
    .line 19,25 : 1,21 ''
    IL_0063:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0068:  stloc.s    V_7
    IL_006a:  ldloc.s    V_7
    IL_006c:  ldloc.s    V_7
    IL_006e:  ldloc.s    V_7
    IL_0070:  ldloc.s    V_7
    IL_0072:  ldloc.s    V_7
    IL_0074:  ldloc.s    V_7
    IL_0076:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Quantifiers01::get_products()
    IL_007b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0080:  ldloc.s    V_7
    IL_0082:  newobj     instance void Linq101Quantifiers01/productGroups@21::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0087:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_008c:  newobj     instance void Linq101Quantifiers01/'productGroups@22-1'::.ctor()
    IL_0091:  newobj     instance void Linq101Quantifiers01/'productGroups@22-2'::.ctor()
    IL_0096:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_009b:  ldloc.s    V_7
    IL_009d:  newobj     instance void Linq101Quantifiers01/'productGroups@22-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00a2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00a7:  newobj     instance void Linq101Quantifiers01/'productGroups@23-4'::.ctor()
    IL_00ac:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_00b1:  newobj     instance void Linq101Quantifiers01/'productGroups@24-6'::.ctor()
    IL_00b6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00bb:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00c0:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00c5:  dup
    IL_00c6:  stsfld     class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::productGroups@19
    IL_00cb:  stloc.3
    .line 28,28 : 1,35 ''
    IL_00cc:  ldc.i4.1
    IL_00cd:  ldc.i4.s   11
    IL_00cf:  ldc.i4.3
    IL_00d0:  ldc.i4.s   19
    IL_00d2:  ldc.i4.s   41
    IL_00d4:  ldc.i4.s   65
    IL_00d6:  ldc.i4.s   19
    IL_00d8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_00dd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ec:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00f1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00f6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00fb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0100:  dup
    IL_0101:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::'numbers@28-7'
    IL_0106:  stloc.s    numbers
    IL_0108:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_010d:  ldnull
    IL_010e:  ldc.i4.0
    IL_010f:  ldc.i4.0
    IL_0110:  newobj     instance void Linq101Quantifiers01/onlyOdd@32::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
    IL_0115:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_011a:  newobj     instance void Linq101Quantifiers01/'onlyOdd@33-1'::.ctor()
    IL_011f:  callvirt   instance bool [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::All<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0124:  dup
    IL_0125:  stsfld     bool '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::onlyOdd@30
    IL_012a:  stloc.s    onlyOdd
    .line 37,43 : 1,21 ''
    IL_012c:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0131:  stloc.s    V_8
    IL_0133:  ldloc.s    V_8
    IL_0135:  ldloc.s    V_8
    IL_0137:  ldloc.s    V_8
    IL_0139:  ldloc.s    V_8
    IL_013b:  ldloc.s    V_8
    IL_013d:  ldloc.s    V_8
    IL_013f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Quantifiers01::get_products()
    IL_0144:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0149:  ldloc.s    V_8
    IL_014b:  newobj     instance void Linq101Quantifiers01/productGroups2@39::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0150:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0155:  newobj     instance void Linq101Quantifiers01/'productGroups2@40-1'::.ctor()
    IL_015a:  newobj     instance void Linq101Quantifiers01/'productGroups2@40-2'::.ctor()
    IL_015f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0164:  ldloc.s    V_8
    IL_0166:  newobj     instance void Linq101Quantifiers01/'productGroups2@40-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_016b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0170:  newobj     instance void Linq101Quantifiers01/'productGroups2@41-4'::.ctor()
    IL_0175:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_017a:  newobj     instance void Linq101Quantifiers01/'productGroups2@42-6'::.ctor()
    IL_017f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0184:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0189:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_018e:  dup
    IL_018f:  stsfld     class [mscorlib]System.Tuple`2<string,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01::productGroups2@37
    IL_0194:  stloc.s    productGroups2
    IL_0196:  ret
  } // end of method $Linq101Quantifiers01::main@

} // end of class '<StartupCode$Linq101Quantifiers01>'.$Linq101Quantifiers01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
