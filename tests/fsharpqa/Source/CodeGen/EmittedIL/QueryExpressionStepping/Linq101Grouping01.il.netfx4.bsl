
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
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly Linq101Grouping01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Grouping01
{
  // Offset: 0x00000000 Length: 0x0000040B
}
.mresource public FSharpOptimizationData.Linq101Grouping01
{
  // Offset: 0x00000410 Length: 0x00000129
}
.module Linq101Grouping01.exe
// MVID: {590846DB-FB79-E5BF-A745-0383DB460859}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01110000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Grouping01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit numberGroups@14
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/numberGroups@14::builder@
      IL_000d:  ret
    } // end of method numberGroups@14::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] int32 n)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 14,14 : 9,28 'C:\\src\\manofstick\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Grouping01.fs'
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 15,15 : 9,29 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/numberGroups@14::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<int32,object>(!!0)
      IL_0011:  ret
    } // end of method numberGroups@14::Invoke

  } // end of class numberGroups@14

  .class auto ansi serializable nested assembly beforefieldinit 'numberGroups@15-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'numberGroups@15-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 15,15 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'numberGroups@15-1'::Invoke

  } // end of class 'numberGroups@15-1'

  .class auto ansi serializable nested assembly beforefieldinit 'numberGroups@15-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'numberGroups@15-2'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      .line 15,15 : 23,28 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.5
      IL_0003:  rem
      IL_0004:  ret
    } // end of method 'numberGroups@15-2'::Invoke

  } // end of class 'numberGroups@15-2'

  .class auto ansi serializable nested assembly beforefieldinit 'numberGroups@15-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'numberGroups@15-3'::builder@
      IL_000d:  ret
    } // end of method 'numberGroups@15-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<int32,int32> _arg2) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<int32,int32> g)
      .line 15,15 : 35,36 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'numberGroups@15-3'::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<int32,int32>,object>(!!0)
      IL_0011:  ret
    } // end of method 'numberGroups@15-3'::Invoke

  } // end of class 'numberGroups@15-3'

  .class auto ansi serializable nested assembly beforefieldinit 'numberGroups@16-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [mscorlib]System.Tuple`2<int32,int32[]>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [mscorlib]System.Tuple`2<int32,int32[]>>::.ctor()
      IL_0006:  ret
    } // end of method 'numberGroups@16-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<int32,int32[]> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<int32,int32> g) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      .line 16,16 : 17,35 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<int32,int32>::get_Key()
      IL_0007:  ldarg.1
      IL_0008:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000d:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32[]>::.ctor(!0,
                                                                                              !1)
      IL_0012:  ret
    } // end of method 'numberGroups@16-4'::Invoke

  } // end of class 'numberGroups@16-4'

  .class auto ansi serializable nested assembly beforefieldinit wordGroups@24
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/wordGroups@24::builder@
      IL_000d:  ret
    } // end of method wordGroups@24::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,object> 
            Invoke(string _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] string w)
      .line 24,24 : 9,26 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 25,25 : 9,29 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/wordGroups@24::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<string,object>(!!0)
      IL_0011:  ret
    } // end of method wordGroups@24::Invoke

  } // end of class wordGroups@24

  .class auto ansi serializable nested assembly beforefieldinit 'wordGroups@25-1'
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
    } // end of method 'wordGroups@25-1'::.ctor

    .method public strict virtual instance string 
            Invoke(string w) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 25,25 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'wordGroups@25-1'::Invoke

  } // end of class 'wordGroups@25-1'

  .class auto ansi serializable nested assembly beforefieldinit 'wordGroups@25-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,char>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,char>::.ctor()
      IL_0006:  ret
    } // end of method 'wordGroups@25-2'::.ctor

    .method public strict virtual instance char 
            Invoke(string w) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 25,25 : 24,25 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.0
      IL_0003:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
      IL_0008:  ret
    } // end of method 'wordGroups@25-2'::Invoke

  } // end of class 'wordGroups@25-2'

  .class auto ansi serializable nested assembly beforefieldinit 'wordGroups@25-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<char,string>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<char,string>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<char,string>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<char,string>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'wordGroups@25-3'::builder@
      IL_000d:  ret
    } // end of method 'wordGroups@25-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<char,string>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<char,string> _arg2) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<char,string> g)
      .line 25,25 : 35,36 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'wordGroups@25-3'::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<char,string>,object>(!!0)
      IL_0011:  ret
    } // end of method 'wordGroups@25-3'::Invoke

  } // end of class 'wordGroups@25-3'

  .class auto ansi serializable nested assembly beforefieldinit 'wordGroups@26-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<char,string>,class [mscorlib]System.Tuple`2<char,string[]>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<char,string>,class [mscorlib]System.Tuple`2<char,string[]>>::.ctor()
      IL_0006:  ret
    } // end of method 'wordGroups@26-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<char,string[]> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<char,string> g) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      .line 26,26 : 17,35 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<char,string>::get_Key()
      IL_0007:  ldarg.1
      IL_0008:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000d:  newobj     instance void class [mscorlib]System.Tuple`2<char,string[]>::.ctor(!0,
                                                                                              !1)
      IL_0012:  ret
    } // end of method 'wordGroups@26-4'::Invoke

  } // end of class 'wordGroups@26-4'

  .class auto ansi serializable nested assembly beforefieldinit orderGroups@34
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/orderGroups@34::builder@
      IL_000d:  ret
    } // end of method orderGroups@34::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 34,34 : 9,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 35,35 : 9,32 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/orderGroups@34::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0011:  ret
    } // end of method orderGroups@34::Invoke

  } // end of class orderGroups@34

  .class auto ansi serializable nested assembly beforefieldinit 'orderGroups@35-1'
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
    } // end of method 'orderGroups@35-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 35,35 : 20,21 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'orderGroups@35-1'::Invoke

  } // end of class 'orderGroups@35-1'

  .class auto ansi serializable nested assembly beforefieldinit 'orderGroups@35-2'
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
    } // end of method 'orderGroups@35-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 35,35 : 22,32 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  tail.
      IL_0004:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0009:  ret
    } // end of method 'orderGroups@35-2'::Invoke

  } // end of class 'orderGroups@35-2'

  .class auto ansi serializable nested assembly beforefieldinit 'orderGroups@35-3'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'orderGroups@35-3'::builder@
      IL_000d:  ret
    } // end of method 'orderGroups@35-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g)
      .line 35,35 : 38,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'orderGroups@35-3'::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0011:  ret
    } // end of method 'orderGroups@35-3'::Invoke

  } // end of class 'orderGroups@35-3'

  .class auto ansi serializable nested assembly beforefieldinit 'orderGroups@36-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>>::.ctor()
      IL_0006:  ret
    } // end of method 'orderGroups@36-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      .line 36,36 : 17,35 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0007:  ldarg.1
      IL_0008:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000d:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>::.ctor(!0,
                                                                                                                    !1)
      IL_0012:  ret
    } // end of method 'orderGroups@36-4'::Invoke

  } // end of class 'orderGroups@36-4'

  .class auto ansi serializable nested assembly beforefieldinit yearGroups@47
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/yearGroups@47::builder@
      IL_000d:  ret
    } // end of method yearGroups@47::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Order o)
      .line 47,47 : 17,37 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 48,48 : 17,48 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/yearGroups@47::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Order,object>(!!0)
      IL_0011:  ret
    } // end of method yearGroups@47::Invoke

  } // end of class yearGroups@47

  .class auto ansi serializable nested assembly beforefieldinit 'yearGroups@48-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>::.ctor()
      IL_0006:  ret
    } // end of method 'yearGroups@48-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Order 
            Invoke(class [Utils]Utils/Order o) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 48,48 : 28,29 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'yearGroups@48-1'::Invoke

  } // end of class 'yearGroups@48-1'

  .class auto ansi serializable nested assembly beforefieldinit 'yearGroups@48-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'yearGroups@48-2'::.ctor

    .method public strict virtual instance int32 
            Invoke(class [Utils]Utils/Order o) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  5
      .locals init ([0] valuetype [mscorlib]System.DateTime V_0)
      .line 48,48 : 31,47 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance valuetype [mscorlib]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0007:  stloc.0
      IL_0008:  ldloca.s   V_0
      IL_000a:  call       instance int32 [mscorlib]System.DateTime::get_Year()
      IL_000f:  ret
    } // end of method 'yearGroups@48-2'::Invoke

  } // end of class 'yearGroups@48-2'

  .class auto ansi serializable nested assembly beforefieldinit monthGroups@51
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/monthGroups@51::builder@
      IL_000d:  ret
    } // end of method monthGroups@51::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object> 
            Invoke(class [Utils]Utils/Order _arg4) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Order o)
      .line 51,51 : 25,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 52,52 : 25,57 ''
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/monthGroups@51::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Order,object>(!!0)
      IL_0011:  ret
    } // end of method monthGroups@51::Invoke

  } // end of class monthGroups@51

  .class auto ansi serializable nested assembly beforefieldinit 'monthGroups@52-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>::.ctor()
      IL_0006:  ret
    } // end of method 'monthGroups@52-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Order 
            Invoke(class [Utils]Utils/Order o) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      .line 52,52 : 36,37 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ret
    } // end of method 'monthGroups@52-1'::Invoke

  } // end of class 'monthGroups@52-1'

  .class auto ansi serializable nested assembly beforefieldinit 'monthGroups@52-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'monthGroups@52-2'::.ctor

    .method public strict virtual instance int32 
            Invoke(class [Utils]Utils/Order o) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  5
      .locals init ([0] valuetype [mscorlib]System.DateTime V_0)
      .line 52,52 : 39,56 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance valuetype [mscorlib]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0007:  stloc.0
      IL_0008:  ldloca.s   V_0
      IL_000a:  call       instance int32 [mscorlib]System.DateTime::get_Month()
      IL_000f:  ret
    } // end of method 'monthGroups@52-2'::Invoke

  } // end of class 'monthGroups@52-2'

  .class auto ansi serializable nested assembly beforefieldinit 'monthGroups@52-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'monthGroups@52-3'::builder@
      IL_000d:  ret
    } // end of method 'monthGroups@52-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> _arg5) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> mg)
      .line 52,52 : 63,65 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'monthGroups@52-3'::builder@
      IL_0009:  ldloc.0
      IL_000a:  tail.
      IL_000c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>(!!0)
      IL_0011:  ret
    } // end of method 'monthGroups@52-3'::Invoke

  } // end of class 'monthGroups@52-3'

  .class auto ansi serializable nested assembly beforefieldinit 'monthGroups@53-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>::.ctor()
      IL_0006:  ret
    } // end of method 'monthGroups@53-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> mg) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      .line 53,53 : 33,53 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>::get_Key()
      IL_0007:  ldarg.1
      IL_0008:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000d:  newobj     instance void class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>::.ctor(!0,
                                                                                                                 !1)
      IL_0012:  ret
    } // end of method 'monthGroups@53-4'::Invoke

  } // end of class 'monthGroups@53-4'

  .class auto ansi serializable nested assembly beforefieldinit 'yearGroups@48-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'yearGroups@48-3'::builder@
      IL_000d:  ret
    } // end of method 'yearGroups@48-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> _arg3) cil managed
    {
      // Code size       94 (0x5e)
      .maxstack  10
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> yg,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>> monthGroups,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@)
      .line 48,48 : 54,56 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0008:  stloc.2
      IL_0009:  ldloc.2
      IL_000a:  ldloc.2
      IL_000b:  ldloc.2
      IL_000c:  ldloc.2
      IL_000d:  ldloc.2
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0014:  ldloc.2
      IL_0015:  newobj     instance void Linq101Grouping01/monthGroups@51::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_001a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Order,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_001f:  newobj     instance void Linq101Grouping01/'monthGroups@52-1'::.ctor()
      IL_0024:  newobj     instance void Linq101Grouping01/'monthGroups@52-2'::.ctor()
      IL_0029:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Order,int32,class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
      IL_002e:  ldloc.2
      IL_002f:  newobj     instance void Linq101Grouping01/'monthGroups@52-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_0034:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_0039:  newobj     instance void Linq101Grouping01/'monthGroups@53-4'::.ctor()
      IL_003e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0043:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_0048:  stloc.1
      .line 55,55 : 17,55 ''
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'yearGroups@48-3'::builder@
      IL_004f:  ldloc.0
      IL_0050:  ldloc.1
      IL_0051:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::.ctor(!0,
                                                                                                                                                                                                                                                                                      !1)
      IL_0056:  tail.
      IL_0058:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>(!!0)
      IL_005d:  ret
    } // end of method 'yearGroups@48-3'::Invoke

  } // end of class 'yearGroups@48-3'

  .class auto ansi serializable nested assembly beforefieldinit 'yearGroups@55-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>::.ctor()
      IL_0006:  ret
    } // end of method 'yearGroups@55-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]> 
            Invoke(class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>> tupledArg) cil managed
    {
      // Code size       33 (0x21)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> yg,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>> monthGroups)
      .line 55,55 : 25,54 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  nop
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>::get_Key()
      IL_0015:  ldloc.1
      IL_0016:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_001b:  newobj     instance void class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>::.ctor(!0,
                                                                                                                                                         !1)
      IL_0020:  ret
    } // end of method 'yearGroups@55-4'::Invoke

  } // end of class 'yearGroups@55-4'

  .class auto ansi serializable nested assembly beforefieldinit customerOrderGroups@44
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/customerOrderGroups@44::builder@
      IL_000d:  ret
    } // end of method customerOrderGroups@44::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       99 (0x63)
      .maxstack  10
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>> yearGroups,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@)
      .line 44,44 : 9,30 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  stloc.0
      .line 57,57 : 9,53 ''
      IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0008:  stloc.2
      IL_0009:  ldloc.2
      IL_000a:  ldloc.2
      IL_000b:  ldloc.2
      IL_000c:  ldloc.2
      IL_000d:  ldloc.2
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldloc.2
      IL_001a:  newobj     instance void Linq101Grouping01/yearGroups@47::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_001f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Order,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_0024:  newobj     instance void Linq101Grouping01/'yearGroups@48-1'::.ctor()
      IL_0029:  newobj     instance void Linq101Grouping01/'yearGroups@48-2'::.ctor()
      IL_002e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Order,int32,class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
      IL_0033:  ldloc.2
      IL_0034:  newobj     instance void Linq101Grouping01/'yearGroups@48-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_0039:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_003e:  newobj     instance void Linq101Grouping01/'yearGroups@55-4'::.ctor()
      IL_0043:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0048:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_004d:  stloc.1
      .line 57,57 : 9,53 ''
      IL_004e:  ldarg.0
      IL_004f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/customerOrderGroups@44::builder@
      IL_0054:  ldloc.0
      IL_0055:  ldloc.1
      IL_0056:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::.ctor(!0,
                                                                                                                                                                                                                                                                               !1)
      IL_005b:  tail.
      IL_005d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>(!!0)
      IL_0062:  ret
    } // end of method customerOrderGroups@44::Invoke

  } // end of class customerOrderGroups@44

  .class auto ansi serializable nested assembly beforefieldinit 'customerOrderGroups@57-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>::.ctor()
      IL_0006:  ret
    } // end of method 'customerOrderGroups@57-1'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]> 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>> tupledArg) cil managed
    {
      // Code size       33 (0x21)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>> yearGroups)
      .line 57,57 : 17,52 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  nop
      IL_000f:  ldloc.0
      IL_0010:  callvirt   instance string [Utils]Utils/Customer::get_CompanyName()
      IL_0015:  ldloc.1
      IL_0016:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_001b:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>::.ctor(!0,
                                                                                                                                                                                                  !1)
      IL_0020:  ret
    } // end of method 'customerOrderGroups@57-1'::Invoke

  } // end of class 'customerOrderGroups@57-1'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_digits() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::digits@7
    IL_0005:  ret
  } // end of method Linq101Grouping01::get_digits

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::'numbers@10-3'
    IL_0005:  ret
  } // end of method Linq101Grouping01::get_numbers

  .method public specialname static class [mscorlib]System.Tuple`2<int32,int32[]>[] 
          get_numberGroups() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<int32,int32[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::numberGroups@12
    IL_0005:  ret
  } // end of method Linq101Grouping01::get_numberGroups

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_words() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::'words@20-2'
    IL_0005:  ret
  } // end of method Linq101Grouping01::get_words

  .method public specialname static class [mscorlib]System.Tuple`2<char,string[]>[] 
          get_wordGroups() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<char,string[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::wordGroups@22
    IL_0005:  ret
  } // end of method Linq101Grouping01::get_wordGroups

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::'products@30-4'
    IL_0005:  ret
  } // end of method Linq101Grouping01::get_products

  .method public specialname static class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[] 
          get_orderGroups() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::orderGroups@32
    IL_0005:  ret
  } // end of method Linq101Grouping01::get_orderGroups

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> 
          get_customers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::customers@40
    IL_0005:  ret
  } // end of method Linq101Grouping01::get_customers

  .method public specialname static class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] 
          get_customerOrderGroups() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::customerOrderGroups@42
    IL_0005:  ret
  } // end of method Linq101Grouping01::get_customerOrderGroups

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          digits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Grouping01::get_digits()
  } // end of property Linq101Grouping01::digits
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Grouping01::get_numbers()
  } // end of property Linq101Grouping01::numbers
  .property class [mscorlib]System.Tuple`2<int32,int32[]>[]
          numberGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<int32,int32[]>[] Linq101Grouping01::get_numberGroups()
  } // end of property Linq101Grouping01::numberGroups
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Grouping01::get_words()
  } // end of property Linq101Grouping01::words
  .property class [mscorlib]System.Tuple`2<char,string[]>[]
          wordGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<char,string[]>[] Linq101Grouping01::get_wordGroups()
  } // end of property Linq101Grouping01::wordGroups
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Grouping01::get_products()
  } // end of property Linq101Grouping01::products
  .property class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[]
          orderGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[] Linq101Grouping01::get_orderGroups()
  } // end of property Linq101Grouping01::orderGroups
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer>
          customers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Grouping01::get_customers()
  } // end of property Linq101Grouping01::customers
  .property class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[]
          customerOrderGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] Linq101Grouping01::get_customerOrderGroups()
  } // end of property Linq101Grouping01::customerOrderGroups
} // end of class Linq101Grouping01

.class private abstract auto ansi sealed '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits@7
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'numbers@10-3'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<int32,int32[]>[] numberGroups@12
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 'words@20-2'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<char,string[]>[] wordGroups@22
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 'products@30-4'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[] orderGroups@32
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers@40
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] customerOrderGroups@42
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       629 (0x275)
    .maxstack  13
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers,
             [2] class [mscorlib]System.Tuple`2<int32,int32[]>[] numberGroups,
             [3] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words,
             [4] class [mscorlib]System.Tuple`2<char,string[]>[] wordGroups,
             [5] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products,
             [6] class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[] orderGroups,
             [7] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers,
             [8] class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] customerOrderGroups,
             [9] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
             [10] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             [11] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11,
             [12] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12)
    .line 7,7 : 1,96 ''
    IL_0000:  nop
    IL_0001:  ldstr      "zero"
    IL_0006:  ldstr      "one"
    IL_000b:  ldstr      "two"
    IL_0010:  ldstr      "three"
    IL_0015:  ldstr      "four"
    IL_001a:  ldstr      "five"
    IL_001f:  ldstr      "six"
    IL_0024:  ldstr      "seven"
    IL_0029:  ldstr      "eight"
    IL_002e:  ldstr      "nine"
    IL_0033:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0038:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_003d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0042:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0047:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_004c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0051:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0056:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_005b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0060:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0065:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_006a:  dup
    IL_006b:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::digits@7
    IL_0070:  stloc.0
    .line 10,10 : 1,47 ''
    IL_0071:  ldc.i4.5
    IL_0072:  ldc.i4.4
    IL_0073:  ldc.i4.1
    IL_0074:  ldc.i4.3
    IL_0075:  ldc.i4.s   9
    IL_0077:  ldc.i4.8
    IL_0078:  ldc.i4.6
    IL_0079:  ldc.i4.7
    IL_007a:  ldc.i4.2
    IL_007b:  ldc.i4.0
    IL_007c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0081:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0086:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_008b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0090:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0095:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_009a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_009f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00a4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00a9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ae:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00b3:  dup
    IL_00b4:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::'numbers@10-3'
    IL_00b9:  stloc.1
    .line 12,17 : 1,21 ''
    IL_00ba:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00bf:  stloc.s    builder@
    IL_00c1:  ldloc.s    builder@
    IL_00c3:  ldloc.s    builder@
    IL_00c5:  ldloc.s    builder@
    IL_00c7:  ldloc.s    builder@
    IL_00c9:  ldloc.s    builder@
    IL_00cb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Grouping01::get_numbers()
    IL_00d0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00d5:  ldloc.s    builder@
    IL_00d7:  newobj     instance void Linq101Grouping01/numberGroups@14::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00dc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [mscorlib]System.Collections.IEnumerable,int32,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00e1:  newobj     instance void Linq101Grouping01/'numberGroups@15-1'::.ctor()
    IL_00e6:  newobj     instance void Linq101Grouping01/'numberGroups@15-2'::.ctor()
    IL_00eb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<int32,int32,int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_00f0:  ldloc.s    builder@
    IL_00f2:  newobj     instance void Linq101Grouping01/'numberGroups@15-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00f7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<int32,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00fc:  newobj     instance void Linq101Grouping01/'numberGroups@16-4'::.ctor()
    IL_0101:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,int32[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0106:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_010b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<int32,int32[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0110:  dup
    IL_0111:  stsfld     class [mscorlib]System.Tuple`2<int32,int32[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::numberGroups@12
    IL_0116:  stloc.2
    .line 20,20 : 1,80 ''
    IL_0117:  ldstr      "blueberry"
    IL_011c:  ldstr      "chimpanzee"
    IL_0121:  ldstr      "abacus"
    IL_0126:  ldstr      "banana"
    IL_012b:  ldstr      "apple"
    IL_0130:  ldstr      "cheese"
    IL_0135:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_013a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0144:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0149:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_014e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0153:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0158:  dup
    IL_0159:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::'words@20-2'
    IL_015e:  stloc.3
    .line 22,27 : 1,21 ''
    IL_015f:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0164:  stloc.s    V_10
    IL_0166:  ldloc.s    V_10
    IL_0168:  ldloc.s    V_10
    IL_016a:  ldloc.s    V_10
    IL_016c:  ldloc.s    V_10
    IL_016e:  ldloc.s    V_10
    IL_0170:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Grouping01::get_words()
    IL_0175:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_017a:  ldloc.s    V_10
    IL_017c:  newobj     instance void Linq101Grouping01/wordGroups@24::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0181:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [mscorlib]System.Collections.IEnumerable,string,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0186:  newobj     instance void Linq101Grouping01/'wordGroups@25-1'::.ctor()
    IL_018b:  newobj     instance void Linq101Grouping01/'wordGroups@25-2'::.ctor()
    IL_0190:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<string,char,string,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0195:  ldloc.s    V_10
    IL_0197:  newobj     instance void Linq101Grouping01/'wordGroups@25-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_019c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<char,string>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<char,string>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01a1:  newobj     instance void Linq101Grouping01/'wordGroups@26-4'::.ctor()
    IL_01a6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<char,string>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<char,string[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01ab:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<char,string[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01b0:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<char,string[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01b5:  dup
    IL_01b6:  stsfld     class [mscorlib]System.Tuple`2<char,string[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::wordGroups@22
    IL_01bb:  stloc.s    wordGroups
    .line 30,30 : 1,32 ''
    IL_01bd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_01c2:  dup
    IL_01c3:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::'products@30-4'
    IL_01c8:  stloc.s    products
    .line 32,37 : 1,21 ''
    IL_01ca:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01cf:  stloc.s    V_11
    IL_01d1:  ldloc.s    V_11
    IL_01d3:  ldloc.s    V_11
    IL_01d5:  ldloc.s    V_11
    IL_01d7:  ldloc.s    V_11
    IL_01d9:  ldloc.s    V_11
    IL_01db:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Grouping01::get_products()
    IL_01e0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01e5:  ldloc.s    V_11
    IL_01e7:  newobj     instance void Linq101Grouping01/orderGroups@34::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01ec:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01f1:  newobj     instance void Linq101Grouping01/'orderGroups@35-1'::.ctor()
    IL_01f6:  newobj     instance void Linq101Grouping01/'orderGroups@35-2'::.ctor()
    IL_01fb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0200:  ldloc.s    V_11
    IL_0202:  newobj     instance void Linq101Grouping01/'orderGroups@35-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0207:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_020c:  newobj     instance void Linq101Grouping01/'orderGroups@36-4'::.ctor()
    IL_0211:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0216:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_021b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0220:  dup
    IL_0221:  stsfld     class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::orderGroups@32
    IL_0226:  stloc.s    orderGroups
    .line 40,40 : 1,34 ''
    IL_0228:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_022d:  dup
    IL_022e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::customers@40
    IL_0233:  stloc.s    customers
    .line 42,58 : 1,21 ''
    IL_0235:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_023a:  stloc.s    V_12
    IL_023c:  ldloc.s    V_12
    IL_023e:  ldloc.s    V_12
    IL_0240:  ldloc.s    V_12
    IL_0242:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Grouping01::get_customers()
    IL_0247:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_024c:  ldloc.s    V_12
    IL_024e:  newobj     instance void Linq101Grouping01/customerOrderGroups@44::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0253:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0258:  newobj     instance void Linq101Grouping01/'customerOrderGroups@57-1'::.ctor()
    IL_025d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0262:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0267:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_026c:  dup
    IL_026d:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::customerOrderGroups@42
    IL_0272:  stloc.s    customerOrderGroups
    IL_0274:  ret
  } // end of method $Linq101Grouping01::main@

} // end of class '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
