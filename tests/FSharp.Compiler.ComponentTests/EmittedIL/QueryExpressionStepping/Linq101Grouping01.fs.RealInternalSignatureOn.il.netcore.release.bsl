




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern System.Linq
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         
  .ver 9:0:0:0
}
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:1:0:0
}
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@14'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 13@14'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object> Invoke(int32 _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 13@14'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<int32,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@15-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 13@15-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 13@15-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 13@15-1' assembly/'Pipe #1 input at line 13@15-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@15-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 13@15-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.5
      IL_0002:  rem
      IL_0003:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 13@15-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 13@15-2' assembly/'Pipe #1 input at line 13@15-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@15-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 13@15-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,object> Invoke(class [System.Linq]System.Linq.IGrouping`2<int32,int32> _arg2) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<int32,int32> V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 13@15-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@16-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,class [runtime]System.Tuple`2<int32,int32[]>>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 13@16-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,class [runtime]System.Tuple`2<int32,int32[]>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<int32,int32[]> Invoke(class [System.Linq]System.Linq.IGrouping`2<int32,int32> g) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<int32,int32>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  call       !!0[] [System.Linq]System.Linq.Enumerable::ToArray<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class [runtime]System.Tuple`2<int32,int32[]>::.ctor(!0,
                                                                                                    !1)
      IL_0011:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 13@16-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 13@16-4' assembly/'Pipe #1 input at line 13@16-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@24'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 23@24'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,object> Invoke(string _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (string V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 23@24'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<string,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@25-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 23@25-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(string w) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 23@25-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 23@25-1' assembly/'Pipe #2 input at line 23@25-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@25-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,char>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 23@25-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,char>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance char Invoke(string w) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.0
      IL_0002:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
      IL_0007:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 23@25-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 23@25-2' assembly/'Pipe #2 input at line 23@25-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@25-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<char,string>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<char,string>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<char,string>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<char,string>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 23@25-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<char,string>,object> Invoke(class [System.Linq]System.Linq.IGrouping`2<char,string> _arg2) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<char,string> V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 23@25-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Linq]System.Linq.IGrouping`2<char,string>,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 23@26-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<char,string>,class [runtime]System.Tuple`2<char,string[]>>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 23@26-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<char,string>,class [runtime]System.Tuple`2<char,string[]>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<char,string[]> Invoke(class [System.Linq]System.Linq.IGrouping`2<char,string> g) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<char,string>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  call       !!0[] [System.Linq]System.Linq.Enumerable::ToArray<string>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class [runtime]System.Tuple`2<char,string[]>::.ctor(!0,
                                                                                                    !1)
      IL_0011:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 23@26-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 23@26-4' assembly/'Pipe #2 input at line 23@26-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@34'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 33@34'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 33@34'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@35-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 33@35-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 33@35-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 33@35-1' assembly/'Pipe #3 input at line 33@35-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@35-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 33@35-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 33@35-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 33@35-2' assembly/'Pipe #3 input at line 33@35-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@35-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 33@35-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object> Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 33@35-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 33@36-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 33@36-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]> Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  call       !!0[] [System.Linq]System.Linq.Enumerable::ToArray<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>::.ctor(!0,
                                                                                                                          !1)
      IL_0011:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 33@36-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 33@36-4' assembly/'Pipe #3 input at line 33@36-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit yearGroups@47
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/yearGroups@47::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object> Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/yearGroups@47::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Order,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'yearGroups@48-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>
  {
    .field static assembly initonly class assembly/'yearGroups@48-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Order Invoke(class [Utils]Utils/Order o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'yearGroups@48-1'::.ctor()
      IL_0005:  stsfld     class assembly/'yearGroups@48-1' assembly/'yearGroups@48-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'yearGroups@48-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>
  {
    .field static assembly initonly class assembly/'yearGroups@48-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(class [Utils]Utils/Order o) cil managed
    {
      
      .maxstack  5
      .locals init (valuetype [runtime]System.DateTime V_0)
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [runtime]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  call       instance int32 [runtime]System.DateTime::get_Year()
      IL_000e:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'yearGroups@48-2'::.ctor()
      IL_0005:  stsfld     class assembly/'yearGroups@48-2' assembly/'yearGroups@48-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit monthGroups@51
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/monthGroups@51::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Order,object> Invoke(class [Utils]Utils/Order _arg4) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/monthGroups@51::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Order,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'monthGroups@52-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>
  {
    .field static assembly initonly class assembly/'monthGroups@52-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [Utils]Utils/Order>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Order Invoke(class [Utils]Utils/Order o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'monthGroups@52-1'::.ctor()
      IL_0005:  stsfld     class assembly/'monthGroups@52-1' assembly/'monthGroups@52-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'monthGroups@52-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>
  {
    .field static assembly initonly class assembly/'monthGroups@52-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(class [Utils]Utils/Order o) cil managed
    {
      
      .maxstack  5
      .locals init (valuetype [runtime]System.DateTime V_0)
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [runtime]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  call       instance int32 [runtime]System.DateTime::get_Month()
      IL_000e:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'monthGroups@52-2'::.ctor()
      IL_0005:  stsfld     class assembly/'monthGroups@52-2' assembly/'monthGroups@52-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'monthGroups@52-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'monthGroups@52-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object> Invoke(class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> _arg5) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'monthGroups@52-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'monthGroups@53-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>
  {
    .field static assembly initonly class assembly/'monthGroups@53-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]> Invoke(class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> mg) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  call       !!0[] [System.Linq]System.Linq.Enumerable::ToArray<class [Utils]Utils/Order>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>::.ctor(!0,
                                                                                                                       !1)
      IL_0011:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'monthGroups@53-4'::.ctor()
      IL_0005:  stsfld     class assembly/'monthGroups@53-4' assembly/'monthGroups@53-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'yearGroups@48-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'yearGroups@48-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object> Invoke(class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> _arg3) cil managed
    {
      
      .maxstack  10
      .locals init (class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>> V_1,
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
      IL_000e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0013:  ldloc.2
      IL_0014:  newobj     instance void assembly/monthGroups@51::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_0019:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Order,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_001e:  ldsfld     class assembly/'monthGroups@52-1' assembly/'monthGroups@52-1'::@_instance
      IL_0023:  ldsfld     class assembly/'monthGroups@52-2' assembly/'monthGroups@52-2'::@_instance
      IL_0028:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Order,int32,class [Utils]Utils/Order,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
      IL_002d:  ldloc.2
      IL_002e:  newobj     instance void assembly/'monthGroups@52-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_0033:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable,class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_0038:  ldsfld     class assembly/'monthGroups@53-4' assembly/'monthGroups@53-4'::@_instance
      IL_003d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0042:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>,class [runtime]System.Collections.IEnumerable>::get_Source()
      IL_0047:  stloc.1
      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'yearGroups@48-3'::builder@
      IL_004e:  ldloc.0
      IL_004f:  ldloc.1
      IL_0050:  newobj     instance void class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                        !1)
      IL_0055:  tail.
      IL_0057:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>(!!0)
      IL_005c:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'yearGroups@55-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>
  {
    .field static assembly initonly class assembly/'yearGroups@55-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]> Invoke(class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order> V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>> V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  call       !!0[] [System.Linq]System.Linq.Enumerable::ToArray<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_001a:  newobj     instance void class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>::.ctor(!0,
                                                                                                                                                                     !1)
      IL_001f:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'yearGroups@55-4'::.ctor()
      IL_0005:  stsfld     class assembly/'yearGroups@55-4' assembly/'yearGroups@55-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 43@44'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 43@44'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object> Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      
      .maxstack  10
      .locals init (class [Utils]Utils/Customer V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>> V_1,
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
      IL_0013:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0018:  ldloc.2
      IL_0019:  newobj     instance void assembly/yearGroups@47::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_001e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Order,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_0023:  ldsfld     class assembly/'yearGroups@48-1' assembly/'yearGroups@48-1'::@_instance
      IL_0028:  ldsfld     class assembly/'yearGroups@48-2' assembly/'yearGroups@48-2'::@_instance
      IL_002d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Order,int32,class [Utils]Utils/Order,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
      IL_0032:  ldloc.2
      IL_0033:  newobj     instance void assembly/'yearGroups@48-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
      IL_0038:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_003d:  ldsfld     class assembly/'yearGroups@55-4' assembly/'yearGroups@55-4'::@_instance
      IL_0042:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [System.Linq]System.Linq.IGrouping`2<int32,class [Utils]Utils/Order>,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>>>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_0047:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>,class [runtime]System.Collections.IEnumerable>::get_Source()
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 43@44'::builder@
      IL_0053:  ldloc.0
      IL_0054:  ldloc.1
      IL_0055:  newobj     instance void class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                       !1)
      IL_005a:  tail.
      IL_005c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>(!!0)
      IL_0061:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 43@57-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>
  {
    .field static assembly initonly class assembly/'Pipe #4 input at line 43@57-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]> Invoke(class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Customer V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>> V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CompanyName()
      IL_0014:  ldloc.1
      IL_0015:  call       !!0[] [System.Linq]System.Linq.Enumerable::ToArray<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_001a:  newobj     instance void class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>::.ctor(!0,
                                                                                                                                                                                                                    !1)
      IL_001f:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #4 input at line 43@57-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #4 input at line 43@57-1' assembly/'Pipe #4 input at line 43@57-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@10
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<int32,int32[]>[] numberGroups@12
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@20
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<char,string[]>[] wordGroups@22
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@30
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>[] orderGroups@32
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers@40
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] customerOrderGroups@42
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> get_digits() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::digits@7
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_numbers() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbers@10
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<int32,int32[]>[] get_numberGroups() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<int32,int32[]>[] assembly::numberGroups@12
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> get_words() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::words@20
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<char,string[]>[] get_wordGroups() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<char,string[]>[] assembly::wordGroups@22
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> get_products() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::products@30
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>[] get_orderGroups() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>[] assembly::orderGroups@32
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> get_customers() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::customers@40
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] get_customerOrderGroups() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] assembly::customerOrderGroups@42
    IL_0005:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  13
    .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32[]>> V_0,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<char,string[]>> V_2,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>> V_4,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_5,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>> V_6,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_7)
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
    IL_0069:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::digits@7
    IL_006e:  ldc.i4.5
    IL_006f:  ldc.i4.4
    IL_0070:  ldc.i4.1
    IL_0071:  ldc.i4.3
    IL_0072:  ldc.i4.s   9
    IL_0074:  ldc.i4.8
    IL_0075:  ldc.i4.6
    IL_0076:  ldc.i4.7
    IL_0077:  ldc.i4.2
    IL_0078:  ldc.i4.0
    IL_0079:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
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
    IL_009c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00a1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00a6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ab:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00b0:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbers@10
    IL_00b5:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00ba:  stloc.1
    IL_00bb:  ldloc.1
    IL_00bc:  ldloc.1
    IL_00bd:  ldloc.1
    IL_00be:  ldloc.1
    IL_00bf:  ldloc.1
    IL_00c0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
    IL_00c5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00ca:  ldloc.1
    IL_00cb:  newobj     instance void assembly/'Pipe #1 input at line 13@14'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00d0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [runtime]System.Collections.IEnumerable,int32,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00d5:  ldsfld     class assembly/'Pipe #1 input at line 13@15-1' assembly/'Pipe #1 input at line 13@15-1'::@_instance
    IL_00da:  ldsfld     class assembly/'Pipe #1 input at line 13@15-2' assembly/'Pipe #1 input at line 13@15-2'::@_instance
    IL_00df:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<int32,int32,int32,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_00e4:  ldloc.1
    IL_00e5:  newobj     instance void assembly/'Pipe #1 input at line 13@15-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00ea:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,class [runtime]System.Collections.IEnumerable,class [System.Linq]System.Linq.IGrouping`2<int32,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00ef:  ldsfld     class assembly/'Pipe #1 input at line 13@16-4' assembly/'Pipe #1 input at line 13@16-4'::@_instance
    IL_00f4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Linq]System.Linq.IGrouping`2<int32,int32>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<int32,int32[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00f9:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,int32[]>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_00fe:  stloc.0
    IL_00ff:  ldloc.0
    IL_0100:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<int32,int32[]>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0105:  stsfld     class [runtime]System.Tuple`2<int32,int32[]>[] assembly::numberGroups@12
    IL_010a:  ldstr      "blueberry"
    IL_010f:  ldstr      "chimpanzee"
    IL_0114:  ldstr      "abacus"
    IL_0119:  ldstr      "banana"
    IL_011e:  ldstr      "apple"
    IL_0123:  ldstr      "cheese"
    IL_0128:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_012d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0132:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0137:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_013c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0141:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0146:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_014b:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::words@20
    IL_0150:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0155:  stloc.3
    IL_0156:  ldloc.3
    IL_0157:  ldloc.3
    IL_0158:  ldloc.3
    IL_0159:  ldloc.3
    IL_015a:  ldloc.3
    IL_015b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
    IL_0160:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0165:  ldloc.3
    IL_0166:  newobj     instance void assembly/'Pipe #2 input at line 23@24'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_016b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [runtime]System.Collections.IEnumerable,string,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0170:  ldsfld     class assembly/'Pipe #2 input at line 23@25-1' assembly/'Pipe #2 input at line 23@25-1'::@_instance
    IL_0175:  ldsfld     class assembly/'Pipe #2 input at line 23@25-2' assembly/'Pipe #2 input at line 23@25-2'::@_instance
    IL_017a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<string,char,string,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_017f:  ldloc.3
    IL_0180:  newobj     instance void assembly/'Pipe #2 input at line 23@25-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0185:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<char,string>,class [runtime]System.Collections.IEnumerable,class [System.Linq]System.Linq.IGrouping`2<char,string>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_018a:  ldsfld     class assembly/'Pipe #2 input at line 23@26-4' assembly/'Pipe #2 input at line 23@26-4'::@_instance
    IL_018f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Linq]System.Linq.IGrouping`2<char,string>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<char,string[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0194:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<char,string[]>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_0199:  stloc.2
    IL_019a:  ldloc.2
    IL_019b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<char,string[]>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01a0:  stsfld     class [runtime]System.Tuple`2<char,string[]>[] assembly::wordGroups@22
    IL_01a5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_01aa:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::products@30
    IL_01af:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01b4:  stloc.s    V_5
    IL_01b6:  ldloc.s    V_5
    IL_01b8:  ldloc.s    V_5
    IL_01ba:  ldloc.s    V_5
    IL_01bc:  ldloc.s    V_5
    IL_01be:  ldloc.s    V_5
    IL_01c0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_01c5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01ca:  ldloc.s    V_5
    IL_01cc:  newobj     instance void assembly/'Pipe #3 input at line 33@34'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01d1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01d6:  ldsfld     class assembly/'Pipe #3 input at line 33@35-1' assembly/'Pipe #3 input at line 33@35-1'::@_instance
    IL_01db:  ldsfld     class assembly/'Pipe #3 input at line 33@35-2' assembly/'Pipe #3 input at line 33@35-2'::@_instance
    IL_01e0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_01e5:  ldloc.s    V_5
    IL_01e7:  newobj     instance void assembly/'Pipe #3 input at line 33@35-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01ec:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01f1:  ldsfld     class assembly/'Pipe #3 input at line 33@36-4' assembly/'Pipe #3 input at line 33@36-4'::@_instance
    IL_01f6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01fb:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_0200:  stloc.s    V_4
    IL_0202:  ldloc.s    V_4
    IL_0204:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0209:  stsfld     class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>[] assembly::orderGroups@32
    IL_020e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_0213:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::customers@40
    IL_0218:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_021d:  stloc.s    V_7
    IL_021f:  ldloc.s    V_7
    IL_0221:  ldloc.s    V_7
    IL_0223:  ldloc.s    V_7
    IL_0225:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::get_customers()
    IL_022a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_022f:  ldloc.s    V_7
    IL_0231:  newobj     instance void assembly/'Pipe #4 input at line 43@44'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0236:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_023b:  ldsfld     class assembly/'Pipe #4 input at line 43@57-1' assembly/'Pipe #4 input at line 43@57-1'::@_instance
    IL_0240:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>>>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0245:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_024a:  stloc.s    V_6
    IL_024c:  ldloc.s    V_6
    IL_024e:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0253:  stsfld     class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] assembly::customerOrderGroups@42
    IL_0258:  ret
  } 

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          digits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_digits()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
  } 
  .property class [runtime]System.Tuple`2<int32,int32[]>[]
          numberGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<int32,int32[]>[] assembly::get_numberGroups()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
  } 
  .property class [runtime]System.Tuple`2<char,string[]>[]
          wordGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<char,string[]>[] assembly::get_wordGroups()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
  } 
  .property class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>[]
          orderGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,class [Utils]Utils/Product[]>[] assembly::get_orderGroups()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer>
          customers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::get_customers()
  } 
  .property class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[]
          customerOrderGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,class [runtime]System.Tuple`2<int32,class [runtime]System.Tuple`2<int32,class [Utils]Utils/Order[]>[]>[]>[] assembly::get_customerOrderGroups()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 





