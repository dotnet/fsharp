
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Grouping01
{
  // Offset: 0x00000000 Length: 0x00000433
  // WARNING: managed resource file FSharpSignatureData.Linq101Grouping01 created
}
.mresource public FSharpOptimizationData.Linq101Grouping01
{
  // Offset: 0x00000438 Length: 0x00000129
  // WARNING: managed resource file FSharpOptimizationData.Linq101Grouping01 created
}
.module Linq101Grouping01.exe
// MVID: {62466AC2-FB79-E5BF-A745-0383C26A4662}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x034C0000


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
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
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
      .locals init (class [System.Core]System.Linq.IGrouping`2<int32,int32> V_0)
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
      .locals init (string V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
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
      .locals init (class [System.Core]System.Linq.IGrouping`2<char,string> V_0)
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
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
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
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0)
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
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
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
      .locals init (valuetype [mscorlib]System.DateTime V_0)
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
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
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
      .locals init (valuetype [mscorlib]System.DateTime V_0)
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
      .locals init (class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> V_0)
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
      .locals init (class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> V_0,
               class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>> V_1,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2)
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
      .locals init (class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> V_0,
               class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>> V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::get_Item2()
      IL_000d:  stloc.1
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
      .locals init (class [Utils]Utils/Customer V_0,
               class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>> V_1,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2)
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
      .locals init (class [Utils]Utils/Customer V_0,
               class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>> V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::get_Item2()
      IL_000d:  stloc.1
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
    // Code size       644 (0x284)
    .maxstack  13
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [mscorlib]System.Tuple`2<int32,int32[]>[] V_2,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_3,
             class [mscorlib]System.Tuple`2<char,string[]>[] V_4,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> V_5,
             class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[] V_6,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> V_7,
             class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] V_8,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,int32[]>> V_9,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<char,string[]>> V_11,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>> V_13,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_14,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>> V_15,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_16)
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
    IL_00b9:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00be:  stloc.s    V_10
    IL_00c0:  ldloc.s    V_10
    IL_00c2:  ldloc.s    V_10
    IL_00c4:  ldloc.s    V_10
    IL_00c6:  ldloc.s    V_10
    IL_00c8:  ldloc.s    V_10
    IL_00ca:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Grouping01::get_numbers()
    IL_00cf:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00d4:  ldloc.s    V_10
    IL_00d6:  newobj     instance void Linq101Grouping01/'Pipe #1 input at line 13@14'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00db:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [mscorlib]System.Collections.IEnumerable,int32,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00e0:  ldsfld     class Linq101Grouping01/'Pipe #1 input at line 13@15-1' Linq101Grouping01/'Pipe #1 input at line 13@15-1'::@_instance
    IL_00e5:  ldsfld     class Linq101Grouping01/'Pipe #1 input at line 13@15-2' Linq101Grouping01/'Pipe #1 input at line 13@15-2'::@_instance
    IL_00ea:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<int32,int32,int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_00ef:  ldloc.s    V_10
    IL_00f1:  newobj     instance void Linq101Grouping01/'Pipe #1 input at line 13@15-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00f6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<int32,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00fb:  ldsfld     class Linq101Grouping01/'Pipe #1 input at line 13@16-4' Linq101Grouping01/'Pipe #1 input at line 13@16-4'::@_instance
    IL_0100:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<int32,int32>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<int32,int32[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0105:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<int32,int32[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_010a:  stloc.s    V_9
    IL_010c:  ldloc.s    V_9
    IL_010e:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<int32,int32[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0113:  dup
    IL_0114:  stsfld     class [mscorlib]System.Tuple`2<int32,int32[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::numberGroups@12
    IL_0119:  stloc.2
    IL_011a:  ldstr      "blueberry"
    IL_011f:  ldstr      "chimpanzee"
    IL_0124:  ldstr      "abacus"
    IL_0129:  ldstr      "banana"
    IL_012e:  ldstr      "apple"
    IL_0133:  ldstr      "cheese"
    IL_0138:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_013d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0142:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0147:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_014c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0151:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0156:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_015b:  dup
    IL_015c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::words@20
    IL_0161:  stloc.3
    IL_0162:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0167:  stloc.s    V_12
    IL_0169:  ldloc.s    V_12
    IL_016b:  ldloc.s    V_12
    IL_016d:  ldloc.s    V_12
    IL_016f:  ldloc.s    V_12
    IL_0171:  ldloc.s    V_12
    IL_0173:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Grouping01::get_words()
    IL_0178:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_017d:  ldloc.s    V_12
    IL_017f:  newobj     instance void Linq101Grouping01/'Pipe #2 input at line 23@24'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0184:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [mscorlib]System.Collections.IEnumerable,string,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0189:  ldsfld     class Linq101Grouping01/'Pipe #2 input at line 23@25-1' Linq101Grouping01/'Pipe #2 input at line 23@25-1'::@_instance
    IL_018e:  ldsfld     class Linq101Grouping01/'Pipe #2 input at line 23@25-2' Linq101Grouping01/'Pipe #2 input at line 23@25-2'::@_instance
    IL_0193:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<string,char,string,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0198:  ldloc.s    V_12
    IL_019a:  newobj     instance void Linq101Grouping01/'Pipe #2 input at line 23@25-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_019f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<char,string>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<char,string>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01a4:  ldsfld     class Linq101Grouping01/'Pipe #2 input at line 23@26-4' Linq101Grouping01/'Pipe #2 input at line 23@26-4'::@_instance
    IL_01a9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<char,string>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<char,string[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01ae:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<char,string[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01b3:  stloc.s    V_11
    IL_01b5:  ldloc.s    V_11
    IL_01b7:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<char,string[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01bc:  dup
    IL_01bd:  stsfld     class [mscorlib]System.Tuple`2<char,string[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::wordGroups@22
    IL_01c2:  stloc.s    V_4
    IL_01c4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_01c9:  dup
    IL_01ca:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::products@30
    IL_01cf:  stloc.s    V_5
    IL_01d1:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01d6:  stloc.s    V_14
    IL_01d8:  ldloc.s    V_14
    IL_01da:  ldloc.s    V_14
    IL_01dc:  ldloc.s    V_14
    IL_01de:  ldloc.s    V_14
    IL_01e0:  ldloc.s    V_14
    IL_01e2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Grouping01::get_products()
    IL_01e7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01ec:  ldloc.s    V_14
    IL_01ee:  newobj     instance void Linq101Grouping01/'Pipe #3 input at line 33@34'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01f3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01f8:  ldsfld     class Linq101Grouping01/'Pipe #3 input at line 33@35-1' Linq101Grouping01/'Pipe #3 input at line 33@35-1'::@_instance
    IL_01fd:  ldsfld     class Linq101Grouping01/'Pipe #3 input at line 33@35-2' Linq101Grouping01/'Pipe #3 input at line 33@35-2'::@_instance
    IL_0202:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0207:  ldloc.s    V_14
    IL_0209:  newobj     instance void Linq101Grouping01/'Pipe #3 input at line 33@35-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_020e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0213:  ldsfld     class Linq101Grouping01/'Pipe #3 input at line 33@36-4' Linq101Grouping01/'Pipe #3 input at line 33@36-4'::@_instance
    IL_0218:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_021d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0222:  stloc.s    V_13
    IL_0224:  ldloc.s    V_13
    IL_0226:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_022b:  dup
    IL_022c:  stsfld     class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::orderGroups@32
    IL_0231:  stloc.s    V_6
    IL_0233:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_0238:  dup
    IL_0239:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::customers@40
    IL_023e:  stloc.s    V_7
    IL_0240:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0245:  stloc.s    V_16
    IL_0247:  ldloc.s    V_16
    IL_0249:  ldloc.s    V_16
    IL_024b:  ldloc.s    V_16
    IL_024d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Grouping01::get_customers()
    IL_0252:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0257:  ldloc.s    V_16
    IL_0259:  newobj     instance void Linq101Grouping01/'Pipe #4 input at line 43@44'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_025e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0263:  ldsfld     class Linq101Grouping01/'Pipe #4 input at line 43@57-1' Linq101Grouping01/'Pipe #4 input at line 43@57-1'::@_instance
    IL_0268:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_026d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0272:  stloc.s    V_15
    IL_0274:  ldloc.s    V_15
    IL_0276:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_027b:  dup
    IL_027c:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Tuple`2<int32,class [mscorlib]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01::customerOrderGroups@42
    IL_0281:  stloc.s    V_8
    IL_0283:  ret
  } // end of method $Linq101Grouping01::main@

} // end of class '<StartupCode$Linq101Grouping01>'.$Linq101Grouping01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\QueryExpressionStepping\Linq101Grouping01_fs\Linq101Grouping01.res
