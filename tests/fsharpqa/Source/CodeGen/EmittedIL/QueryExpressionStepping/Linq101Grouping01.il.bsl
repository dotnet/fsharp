
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
  .ver 5:0:0:0
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
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:0:0:0
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
  // Offset: 0x00000000 Length: 0x00000403
}
.mresource public FSharpOptimizationData.Linq101Grouping01
{
  // Offset: 0x00000408 Length: 0x00000129
}
.module Linq101Grouping01.exe
// MVID: {611B0EC5-FB79-E5BF-A745-0383C50E1B61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x053C0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Grouping01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@14'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #1 input at line 13@14'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #1 input at line 13@14'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] int32 n)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 14,14 : 9,28 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Grouping01.fs'
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 15,15 : 9,29 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #1 input at line 13@14'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<int32,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #1 input at line 13@14'::Invoke

  } // end of class 'Pipe #1 input at line 13@14'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@15-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #1 input at line 13@15-1' @_instance
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
    } // end of method 'Pipe #1 input at line 13@15-1'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 15,15 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #1 input at line 13@15-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #1 input at line 13@15-1'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #1 input at line 13@15-1' Linq101Grouping01/'Pipe #1 input at line 13@15-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 13@15-1'::.cctor

  } // end of class 'Pipe #1 input at line 13@15-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@15-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #1 input at line 13@15-2' @_instance
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
    } // end of method 'Pipe #1 input at line 13@15-2'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      .line 15,15 : 23,28 ''
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.5
      IL_0002:  rem
      IL_0003:  ret
    } // end of method 'Pipe #1 input at line 13@15-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #1 input at line 13@15-2'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #1 input at line 13@15-2' Linq101Grouping01/'Pipe #1 input at line 13@15-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 13@15-2'::.cctor

  } // end of class 'Pipe #1 input at line 13@15-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@15-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #1 input at line 13@15-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #1 input at line 13@15-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<int32,int32> _arg2) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<int32,int32> g)
      .line 15,15 : 35,36 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #1 input at line 13@15-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<int32,int32>,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #1 input at line 13@15-3'::Invoke

  } // end of class 'Pipe #1 input at line 13@15-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@16-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [mscorlib]System.Tuple`2<int32,int32[]>>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #1 input at line 13@16-4' @_instance
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
    } // end of method 'Pipe #1 input at line 13@16-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<int32,int32[]> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<int32,int32> g) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      .line 16,16 : 17,35 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<int32,int32>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32[]>::.ctor(!0,
                                                                                              !1)
      IL_0011:  ret
    } // end of method 'Pipe #1 input at line 13@16-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #1 input at line 13@16-4'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #1 input at line 13@16-4' Linq101Grouping01/'Pipe #1 input at line 13@16-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 13@16-4'::.cctor

  } // end of class 'Pipe #1 input at line 13@16-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@24'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #2 input at line 23@24'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #2 input at line 23@24'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,object> 
            Invoke(string _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] string w)
      .line 24,24 : 9,26 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 25,25 : 9,29 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #2 input at line 23@24'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<string,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #2 input at line 23@24'::Invoke

  } // end of class 'Pipe #2 input at line 23@24'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@25-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #2 input at line 23@25-1' @_instance
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
    } // end of method 'Pipe #2 input at line 23@25-1'::.ctor

    .method public strict virtual instance string 
            Invoke(string w) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 25,25 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #2 input at line 23@25-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #2 input at line 23@25-1'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #2 input at line 23@25-1' Linq101Grouping01/'Pipe #2 input at line 23@25-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 23@25-1'::.cctor

  } // end of class 'Pipe #2 input at line 23@25-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@25-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,char>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #2 input at line 23@25-2' @_instance
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
    } // end of method 'Pipe #2 input at line 23@25-2'::.ctor

    .method public strict virtual instance char 
            Invoke(string w) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 25,25 : 23,28 ''
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.0
      IL_0002:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
      IL_0007:  ret
    } // end of method 'Pipe #2 input at line 23@25-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #2 input at line 23@25-2'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #2 input at line 23@25-2' Linq101Grouping01/'Pipe #2 input at line 23@25-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 23@25-2'::.cctor

  } // end of class 'Pipe #2 input at line 23@25-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@25-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<char,string>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<char,string>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<char,string>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<char,string>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #2 input at line 23@25-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #2 input at line 23@25-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<char,string>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<char,string> _arg2) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<char,string> g)
      .line 25,25 : 35,36 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #2 input at line 23@25-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<char,string>,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #2 input at line 23@25-3'::Invoke

  } // end of class 'Pipe #2 input at line 23@25-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@26-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<char,string>,class [mscorlib]System.Tuple`2<char,string[]>>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #2 input at line 23@26-4' @_instance
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
    } // end of method 'Pipe #2 input at line 23@26-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<char,string[]> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<char,string> g) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      .line 26,26 : 17,35 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<char,string>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class [mscorlib]System.Tuple`2<char,string[]>::.ctor(!0,
                                                                                              !1)
      IL_0011:  ret
    } // end of method 'Pipe #2 input at line 23@26-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #2 input at line 23@26-4'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #2 input at line 23@26-4' Linq101Grouping01/'Pipe #2 input at line 23@26-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 23@26-4'::.cctor

  } // end of class 'Pipe #2 input at line 23@26-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@34'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #3 input at line 33@34'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #3 input at line 33@34'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 34,34 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 35,35 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #3 input at line 33@34'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #3 input at line 33@34'::Invoke

  } // end of class 'Pipe #3 input at line 33@34'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@35-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #3 input at line 33@35-1' @_instance
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
    } // end of method 'Pipe #3 input at line 33@35-1'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 35,35 : 20,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #3 input at line 33@35-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #3 input at line 33@35-1'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #3 input at line 33@35-1' Linq101Grouping01/'Pipe #3 input at line 33@35-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 33@35-1'::.cctor

  } // end of class 'Pipe #3 input at line 33@35-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@35-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #3 input at line 33@35-2' @_instance
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
    } // end of method 'Pipe #3 input at line 33@35-2'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 35,35 : 22,32 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #3 input at line 33@35-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #3 input at line 33@35-2'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #3 input at line 33@35-2' Linq101Grouping01/'Pipe #3 input at line 33@35-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 33@35-2'::.cctor

  } // end of class 'Pipe #3 input at line 33@35-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@35-3'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #3 input at line 33@35-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #3 input at line 33@35-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g)
      .line 35,35 : 38,39 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #3 input at line 33@35-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #3 input at line 33@35-3'::Invoke

  } // end of class 'Pipe #3 input at line 33@35-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@36-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #3 input at line 33@36-4' @_instance
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
    } // end of method 'Pipe #3 input at line 33@36-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      .line 36,36 : 17,35 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>::.ctor(!0,
                                                                                                                    !1)
      IL_0011:  ret
    } // end of method 'Pipe #3 input at line 33@36-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #3 input at line 33@36-4'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #3 input at line 33@36-4' Linq101Grouping01/'Pipe #3 input at line 33@36-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 33@36-4'::.cctor

  } // end of class 'Pipe #3 input at line 33@36-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit yearGroups@47
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/yearGroups@47::builder@
      IL_000d:  ret
    } // end of method yearGroups@47::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object> 
            Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Order o)
      .line 47,47 : 17,37 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 48,48 : 17,48 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/yearGroups@47::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Order,object>(!!0)
      IL_0010:  ret
    } // end of method yearGroups@47::Invoke

  } // end of class yearGroups@47

  .class auto ansi serializable sealed nested assembly beforefieldinit 'yearGroups@48-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>
  {
    .field static assembly initonly class Linq101Grouping01/'yearGroups@48-1' @_instance
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 48,48 : 28,29 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'yearGroups@48-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'yearGroups@48-1'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'yearGroups@48-1' Linq101Grouping01/'yearGroups@48-1'::@_instance
      IL_000a:  ret
    } // end of method 'yearGroups@48-1'::.cctor

  } // end of class 'yearGroups@48-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'yearGroups@48-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>
  {
    .field static assembly initonly class Linq101Grouping01/'yearGroups@48-2' @_instance
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
      // Code size       15 (0xf)
      .maxstack  5
      .locals init ([0] valuetype [mscorlib]System.DateTime V_0)
      .line 48,48 : 31,47 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [mscorlib]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  call       instance int32 [mscorlib]System.DateTime::get_Year()
      IL_000e:  ret
    } // end of method 'yearGroups@48-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'yearGroups@48-2'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'yearGroups@48-2' Linq101Grouping01/'yearGroups@48-2'::@_instance
      IL_000a:  ret
    } // end of method 'yearGroups@48-2'::.cctor

  } // end of class 'yearGroups@48-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit monthGroups@51
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/monthGroups@51::builder@
      IL_000d:  ret
    } // end of method monthGroups@51::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object> 
            Invoke(class [Utils]Utils/Order _arg4) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Order o)
      .line 51,51 : 25,39 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 52,52 : 25,57 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/monthGroups@51::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Order,object>(!!0)
      IL_0010:  ret
    } // end of method monthGroups@51::Invoke

  } // end of class monthGroups@51

  .class auto ansi serializable sealed nested assembly beforefieldinit 'monthGroups@52-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>
  {
    .field static assembly initonly class Linq101Grouping01/'monthGroups@52-1' @_instance
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
      // Code size       2 (0x2)
      .maxstack  8
      .line 52,52 : 36,37 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'monthGroups@52-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'monthGroups@52-1'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'monthGroups@52-1' Linq101Grouping01/'monthGroups@52-1'::@_instance
      IL_000a:  ret
    } // end of method 'monthGroups@52-1'::.cctor

  } // end of class 'monthGroups@52-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'monthGroups@52-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>
  {
    .field static assembly initonly class Linq101Grouping01/'monthGroups@52-2' @_instance
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
      // Code size       15 (0xf)
      .maxstack  5
      .locals init ([0] valuetype [mscorlib]System.DateTime V_0)
      .line 52,52 : 39,56 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [mscorlib]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  call       instance int32 [mscorlib]System.DateTime::get_Month()
      IL_000e:  ret
    } // end of method 'monthGroups@52-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'monthGroups@52-2'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'monthGroups@52-2' Linq101Grouping01/'monthGroups@52-2'::@_instance
      IL_000a:  ret
    } // end of method 'monthGroups@52-2'::.cctor

  } // end of class 'monthGroups@52-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'monthGroups@52-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'monthGroups@52-3'::builder@
      IL_000d:  ret
    } // end of method 'monthGroups@52-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> _arg5) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> mg)
      .line 52,52 : 63,65 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'monthGroups@52-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>(!!0)
      IL_0010:  ret
    } // end of method 'monthGroups@52-3'::Invoke

  } // end of class 'monthGroups@52-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'monthGroups@53-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>
  {
    .field static assembly initonly class Linq101Grouping01/'monthGroups@53-4' @_instance
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
      // Code size       18 (0x12)
      .maxstack  8
      .line 53,53 : 33,53 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>::.ctor(!0,
                                                                                                                 !1)
      IL_0011:  ret
    } // end of method 'monthGroups@53-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'monthGroups@53-4'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'monthGroups@53-4' Linq101Grouping01/'monthGroups@53-4'::@_instance
      IL_000a:  ret
    } // end of method 'monthGroups@53-4'::.cctor

  } // end of class 'monthGroups@53-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'yearGroups@48-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'yearGroups@48-3'::builder@
      IL_000d:  ret
    } // end of method 'yearGroups@48-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object> 
            Invoke(class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> _arg3) cil managed
    {
      // Code size       93 (0x5d)
      .maxstack  10
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> yg,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>> monthGroups,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2)
      .line 48,48 : 54,56 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  ldloc.2
      IL_000a:  ldloc.2
      IL_000b:  ldloc.2
      IL_000c:  ldloc.2
      IL_000d:  ldloc.0
      IL_000e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0013:  ldloc.2
      IL_0014:  newobj     instance void Linq101Grouping01/monthGroups@51::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_0019:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Order,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_001e:  ldsfld     class Linq101Grouping01/'monthGroups@52-1' Linq101Grouping01/'monthGroups@52-1'::@_instance
      IL_0023:  ldsfld     class Linq101Grouping01/'monthGroups@52-2' Linq101Grouping01/'monthGroups@52-2'::@_instance
      IL_0028:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Order,int32,class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
      IL_002d:  ldloc.2
      IL_002e:  newobj     instance void Linq101Grouping01/'monthGroups@52-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_0033:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_0038:  ldsfld     class Linq101Grouping01/'monthGroups@53-4' Linq101Grouping01/'monthGroups@53-4'::@_instance
      IL_003d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0042:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_0047:  stloc.1
      .line 55,55 : 17,55 ''
      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'yearGroups@48-3'::builder@
      IL_004e:  ldloc.0
      IL_004f:  ldloc.1
      IL_0050:  newobj     instance void class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::.ctor(!0,
                                                                                                                                                                                                                                                                                      !1)
      IL_0055:  tail.
      IL_0057:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>(!!0)
      IL_005c:  ret
    } // end of method 'yearGroups@48-3'::Invoke

  } // end of class 'yearGroups@48-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'yearGroups@55-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>
  {
    .field static assembly initonly class Linq101Grouping01/'yearGroups@55-4' @_instance
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
      // Code size       32 (0x20)
      .maxstack  6
      .locals init ([0] class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> yg,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>> monthGroups)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::get_Item2()
      IL_000d:  stloc.1
      .line 55,55 : 25,54 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_001a:  newobj     instance void class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>::.ctor(!0,
                                                                                                                                                         !1)
      IL_001f:  ret
    } // end of method 'yearGroups@55-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'yearGroups@55-4'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'yearGroups@55-4' Linq101Grouping01/'yearGroups@55-4'::@_instance
      IL_000a:  ret
    } // end of method 'yearGroups@55-4'::.cctor

  } // end of class 'yearGroups@55-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 43@44'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #4 input at line 43@44'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #4 input at line 43@44'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       98 (0x62)
      .maxstack  10
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>> yearGroups,
               [2] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2)
      .line 44,44 : 9,30 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 57,57 : 9,53 ''
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  ldloc.2
      IL_000a:  ldloc.2
      IL_000b:  ldloc.2
      IL_000c:  ldloc.2
      IL_000d:  ldloc.0
      IL_000e:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0013:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0018:  ldloc.2
      IL_0019:  newobj     instance void Linq101Grouping01/yearGroups@47::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_001e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Order,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_0023:  ldsfld     class Linq101Grouping01/'yearGroups@48-1' Linq101Grouping01/'yearGroups@48-1'::@_instance
      IL_0028:  ldsfld     class Linq101Grouping01/'yearGroups@48-2' Linq101Grouping01/'yearGroups@48-2'::@_instance
      IL_002d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Order,int32,class [Utils]Utils/Order,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
      IL_0032:  ldloc.2
      IL_0033:  newobj     instance void Linq101Grouping01/'yearGroups@48-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_0038:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_003d:  ldsfld     class Linq101Grouping01/'yearGroups@55-4' Linq101Grouping01/'yearGroups@55-4'::@_instance
      IL_0042:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0047:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
      IL_004c:  stloc.1
      .line 57,57 : 9,53 ''
      IL_004d:  ldarg.0
      IL_004e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Grouping01/'Pipe #4 input at line 43@44'::builder@
      IL_0053:  ldloc.0
      IL_0054:  ldloc.1
      IL_0055:  newobj     instance void class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::.ctor(!0,
                                                                                                                                                                                                                                                                               !1)
      IL_005a:  tail.
      IL_005c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>(!!0)
      IL_0061:  ret
    } // end of method 'Pipe #4 input at line 43@44'::Invoke

  } // end of class 'Pipe #4 input at line 43@44'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 43@57-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>
  {
    .field static assembly initonly class Linq101Grouping01/'Pipe #4 input at line 43@57-1' @_instance
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
    } // end of method 'Pipe #4 input at line 43@57-1'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]> 
            Invoke(class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>> tupledArg) cil managed
    {
      // Code size       32 (0x20)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Customer c,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>> yearGroups)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::get_Item2()
      IL_000d:  stloc.1
      .line 57,57 : 17,52 ''
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CompanyName()
      IL_0014:  ldloc.1
      IL_0015:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_001a:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>::.ctor(!0,
                                                                                                                                                                                                  !1)
      IL_001f:  ret
    } // end of method 'Pipe #4 input at line 43@57-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Grouping01/'Pipe #4 input at line 43@57-1'::.ctor()
      IL_0005:  stsfld     class Linq101Grouping01/'Pipe #4 input at line 43@57-1' Linq101Grouping01/'Pipe #4 input at line 43@57-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 43@57-1'::.cctor

  } // end of class 'Pipe #4 input at line 43@57-1'

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
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::numbers@10
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
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::words@20
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
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::products@30
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
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@10
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<int32,int32[]>[] numberGroups@12
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@20
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<char,string[]>[] wordGroups@22
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@30
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
    // Code size       648 (0x288)
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
             [9] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32[]>> 'Pipe #1 input at line 13',
             [10] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             [11] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<char,string[]>> 'Pipe #2 input at line 23',
             [12] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             [13] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>> 'Pipe #3 input at line 33',
             [14] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_14,
             [15] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>> 'Pipe #4 input at line 43',
             [16] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_16)
    .line 7,7 : 1,96 ''
    IL_0000:  ldstr      "zero"
    IL_0005:  ldstr      "one"
    IL_000a:  ldstr      "two"
    IL_000f:  ldstr      "three"
    IL_0014:  ldstr      "four"
    IL_0019:  ldstr      "five"
    IL_001e:  ldstr      "six"
    IL_0023:  ldstr      "seven"
    IL_0028:  ldstr      "eight"
    IL_002d:  ldstr      "nine"
    IL_0032:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0037:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_003c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0041:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0046:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_004b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0050:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0055:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_005a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_005f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0064:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0069:  dup
    IL_006a:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::digits@7
    IL_006f:  stloc.0
    .line 10,10 : 1,47 ''
    IL_0070:  ldc.i4.5
    IL_0071:  ldc.i4.4
    IL_0072:  ldc.i4.1
    IL_0073:  ldc.i4.3
    IL_0074:  ldc.i4.s   9
    IL_0076:  ldc.i4.8
    IL_0077:  ldc.i4.6
    IL_0078:  ldc.i4.7
    IL_0079:  ldc.i4.2
    IL_007a:  ldc.i4.0
    IL_007b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0080:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0085:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_008a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_008f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0094:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0099:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_009e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00a3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00a8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ad:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00b2:  dup
    IL_00b3:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::numbers@10
    IL_00b8:  stloc.1
    .line 12,17 : 1,21 ''
    IL_00b9:  nop
    .line 13,13 : 5,10 ''
    IL_00ba:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00bf:  stloc.s    V_10
    IL_00c1:  ldloc.s    V_10
    IL_00c3:  ldloc.s    V_10
    IL_00c5:  ldloc.s    V_10
    IL_00c7:  ldloc.s    V_10
    IL_00c9:  ldloc.s    V_10
    IL_00cb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Grouping01::get_numbers()
    IL_00d0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00d5:  ldloc.s    V_10
    IL_00d7:  newobj     instance void Linq101Grouping01/'Pipe #1 input at line 13@14'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00dc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [mscorlib]System.Collections.IEnumerable,int32,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00e1:  ldsfld     class Linq101Grouping01/'Pipe #1 input at line 13@15-1' Linq101Grouping01/'Pipe #1 input at line 13@15-1'::@_instance
    IL_00e6:  ldsfld     class Linq101Grouping01/'Pipe #1 input at line 13@15-2' Linq101Grouping01/'Pipe #1 input at line 13@15-2'::@_instance
    IL_00eb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<int32,int32,int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_00f0:  ldloc.s    V_10
    IL_00f2:  newobj     instance void Linq101Grouping01/'Pipe #1 input at line 13@15-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00f7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<int32,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00fc:  ldsfld     class Linq101Grouping01/'Pipe #1 input at line 13@16-4' Linq101Grouping01/'Pipe #1 input at line 13@16-4'::@_instance
    IL_0101:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,int32[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0106:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_010b:  stloc.s    'Pipe #1 input at line 13'
    .line 17,17 : 10,21 ''
    IL_010d:  ldloc.s    'Pipe #1 input at line 13'
    IL_010f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<int32,int32[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0114:  dup
    IL_0115:  stsfld     class [mscorlib]System.Tuple`2<int32,int32[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::numberGroups@12
    IL_011a:  stloc.2
    .line 20,20 : 1,80 ''
    IL_011b:  ldstr      "blueberry"
    IL_0120:  ldstr      "chimpanzee"
    IL_0125:  ldstr      "abacus"
    IL_012a:  ldstr      "banana"
    IL_012f:  ldstr      "apple"
    IL_0134:  ldstr      "cheese"
    IL_0139:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_013e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0143:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0148:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_014d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0152:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0157:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_015c:  dup
    IL_015d:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::words@20
    IL_0162:  stloc.3
    .line 22,27 : 1,21 ''
    IL_0163:  nop
    .line 23,23 : 5,10 ''
    IL_0164:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0169:  stloc.s    V_12
    IL_016b:  ldloc.s    V_12
    IL_016d:  ldloc.s    V_12
    IL_016f:  ldloc.s    V_12
    IL_0171:  ldloc.s    V_12
    IL_0173:  ldloc.s    V_12
    IL_0175:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Grouping01::get_words()
    IL_017a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_017f:  ldloc.s    V_12
    IL_0181:  newobj     instance void Linq101Grouping01/'Pipe #2 input at line 23@24'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0186:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [mscorlib]System.Collections.IEnumerable,string,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_018b:  ldsfld     class Linq101Grouping01/'Pipe #2 input at line 23@25-1' Linq101Grouping01/'Pipe #2 input at line 23@25-1'::@_instance
    IL_0190:  ldsfld     class Linq101Grouping01/'Pipe #2 input at line 23@25-2' Linq101Grouping01/'Pipe #2 input at line 23@25-2'::@_instance
    IL_0195:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<string,char,string,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_019a:  ldloc.s    V_12
    IL_019c:  newobj     instance void Linq101Grouping01/'Pipe #2 input at line 23@25-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01a1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<char,string>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<char,string>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01a6:  ldsfld     class Linq101Grouping01/'Pipe #2 input at line 23@26-4' Linq101Grouping01/'Pipe #2 input at line 23@26-4'::@_instance
    IL_01ab:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<char,string>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<char,string[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01b0:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<char,string[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01b5:  stloc.s    'Pipe #2 input at line 23'
    .line 27,27 : 10,21 ''
    IL_01b7:  ldloc.s    'Pipe #2 input at line 23'
    IL_01b9:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<char,string[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01be:  dup
    IL_01bf:  stsfld     class [mscorlib]System.Tuple`2<char,string[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::wordGroups@22
    IL_01c4:  stloc.s    wordGroups
    .line 30,30 : 1,32 ''
    IL_01c6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_01cb:  dup
    IL_01cc:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::products@30
    IL_01d1:  stloc.s    products
    .line 32,37 : 1,21 ''
    IL_01d3:  nop
    .line 33,33 : 5,10 ''
    IL_01d4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01d9:  stloc.s    V_14
    IL_01db:  ldloc.s    V_14
    IL_01dd:  ldloc.s    V_14
    IL_01df:  ldloc.s    V_14
    IL_01e1:  ldloc.s    V_14
    IL_01e3:  ldloc.s    V_14
    IL_01e5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Grouping01::get_products()
    IL_01ea:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01ef:  ldloc.s    V_14
    IL_01f1:  newobj     instance void Linq101Grouping01/'Pipe #3 input at line 33@34'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01f6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01fb:  ldsfld     class Linq101Grouping01/'Pipe #3 input at line 33@35-1' Linq101Grouping01/'Pipe #3 input at line 33@35-1'::@_instance
    IL_0200:  ldsfld     class Linq101Grouping01/'Pipe #3 input at line 33@35-2' Linq101Grouping01/'Pipe #3 input at line 33@35-2'::@_instance
    IL_0205:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_020a:  ldloc.s    V_14
    IL_020c:  newobj     instance void Linq101Grouping01/'Pipe #3 input at line 33@35-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0211:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0216:  ldsfld     class Linq101Grouping01/'Pipe #3 input at line 33@36-4' Linq101Grouping01/'Pipe #3 input at line 33@36-4'::@_instance
    IL_021b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0220:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0225:  stloc.s    'Pipe #3 input at line 33'
    .line 37,37 : 10,21 ''
    IL_0227:  ldloc.s    'Pipe #3 input at line 33'
    IL_0229:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_022e:  dup
    IL_022f:  stsfld     class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::orderGroups@32
    IL_0234:  stloc.s    orderGroups
    .line 40,40 : 1,34 ''
    IL_0236:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_023b:  dup
    IL_023c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::customers@40
    IL_0241:  stloc.s    customers
    .line 42,58 : 1,21 ''
    IL_0243:  nop
    .line 43,43 : 5,10 ''
    IL_0244:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0249:  stloc.s    V_16
    IL_024b:  ldloc.s    V_16
    IL_024d:  ldloc.s    V_16
    IL_024f:  ldloc.s    V_16
    IL_0251:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Grouping01::get_customers()
    IL_0256:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_025b:  ldloc.s    V_16
    IL_025d:  newobj     instance void Linq101Grouping01/'Pipe #4 input at line 43@44'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0262:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0267:  ldsfld     class Linq101Grouping01/'Pipe #4 input at line 43@57-1' Linq101Grouping01/'Pipe #4 input at line 43@57-1'::@_instance
    IL_026c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0271:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0276:  stloc.s    'Pipe #4 input at line 43'
    .line 58,58 : 10,21 ''
    IL_0278:  ldloc.s    'Pipe #4 input at line 43'
    IL_027a:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_027f:  dup
    IL_0280:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::customerOrderGroups@42
    IL_0285:  stloc.s    customerOrderGroups
    IL_0287:  ret
  } // end of method $Linq101Grouping01::main@

} // end of class '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
