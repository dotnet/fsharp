




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly extern System.Linq
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         
  .ver 9:0:0:0
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
  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@14'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 12@14' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(string c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 12@14'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 12@14' assembly/'Pipe #1 input at line 12@14'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@14-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 12@14-1' @_instance
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
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 12@14-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 12@14-1' assembly/'Pipe #1 input at line 12@14-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@14-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [Utils]Utils/Product,class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 12@14-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [Utils]Utils/Product,class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [Utils]Utils/Product> 
            Invoke(string c,
                   class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>::.ctor(!0,
                                                                                                                        !1)
      IL_0007:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 12@14-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 12@14-2' assembly/'Pipe #1 input at line 12@14-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@14-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 12@14-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,object> Invoke(class [runtime]System.Tuple`2<string,class [Utils]Utils/Product> _arg1) cil managed
    {
      
      .maxstack  7
      .locals init (class [runtime]System.Tuple`2<string,class [Utils]Utils/Product> V_0,
               class [Utils]Utils/Product V_1,
               string V_2)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance !1 class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>::get_Item2()
      IL_0008:  stloc.1
      IL_0009:  ldloc.0
      IL_000a:  call       instance !0 class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>::get_Item1()
      IL_000f:  stloc.2
      IL_0010:  ldarg.0
      IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 12@14-3'::builder@
      IL_0016:  ldloc.2
      IL_0017:  ldloc.1
      IL_0018:  newobj     instance void class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>::.ctor(!0,
                                                                                                                        !1)
      IL_001d:  tail.
      IL_001f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0024:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@15-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,string>>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 12@15-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,string>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,string> Invoke(class [runtime]System.Tuple`2<string,class [Utils]Utils/Product> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (string V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0015:  newobj     instance void class [runtime]System.Tuple`2<string,string>::.ctor(!0,
                                                                                                    !1)
      IL_001a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 12@15-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 12@15-4' assembly/'Pipe #1 input at line 12@15-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@22'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 20@22' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(string c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 20@22'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 20@22' assembly/'Pipe #2 input at line 20@22'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@22-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 20@22-1' @_instance
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
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 20@22-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 20@22-1' assembly/'Pipe #2 input at line 20@22-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@22-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 20@22-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(string c,
                   class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                        !1)
      IL_0007:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 20@22-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 20@22-2' assembly/'Pipe #2 input at line 20@22-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@22-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 20@22-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object> Invoke(class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> _arg1) cil managed
    {
      
      .maxstack  7
      .locals init (class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_1,
               string V_2)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance !1 class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_0008:  stloc.1
      IL_0009:  ldloc.0
      IL_000a:  call       instance !0 class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_000f:  stloc.2
      IL_0010:  ldarg.0
      IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 20@22-3'::builder@
      IL_0016:  ldloc.2
      IL_0017:  ldloc.1
      IL_0018:  newobj     instance void class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                        !1)
      IL_001d:  tail.
      IL_001f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_0024:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@23-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 20@23-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> Invoke(class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (string V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  newobj     instance void class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                        !1)
      IL_0015:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 20@23-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 20@23-4' assembly/'Pipe #2 input at line 20@23-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@30'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 28@30' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(string c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 28@30'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 28@30' assembly/'Pipe #3 input at line 28@30'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@30-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 28@30-1' @_instance
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
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 28@30-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 28@30-1' assembly/'Pipe #3 input at line 28@30-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@30-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 28@30-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(string c,
                   class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                        !1)
      IL_0007:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 28@30-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 28@30-2' assembly/'Pipe #3 input at line 28@30-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@31-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps
    .field public string c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps,
                                 string c) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 28@31-4'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> assembly/'Pipe #3 input at line 28@31-4'::ps
      IL_0014:  ldarg.0
      IL_0015:  ldarg.3
      IL_0016:  stfld      string assembly/'Pipe #3 input at line 28@31-4'::c
      IL_001b:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object> Invoke(class [Utils]Utils/Product _arg2) cil managed
    {
      
      .maxstack  8
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 28@31-4'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      string assembly/'Pipe #3 input at line 28@31-4'::c
      IL_000e:  ldarg.0
      IL_000f:  ldfld      class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> assembly/'Pipe #3 input at line 28@31-4'::ps
      IL_0014:  ldloc.0
      IL_0015:  newobj     instance void class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>::.ctor(!0,
                                                                                                                                                                                                                   !1,
                                                                                                                                                                                                                   !2)
      IL_001a:  tail.
      IL_001c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object>(!!0)
      IL_0021:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@30-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 28@30-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable> Invoke(class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> _arg1) cil managed
    {
      
      .maxstack  9
      .locals init (class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_1,
               string V_2)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance !1 class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_0008:  stloc.1
      IL_0009:  ldloc.0
      IL_000a:  call       instance !0 class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_000f:  stloc.2
      IL_0010:  ldarg.0
      IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 28@30-3'::builder@
      IL_0016:  ldarg.0
      IL_0017:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 28@30-3'::builder@
      IL_001c:  ldloc.1
      IL_001d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0022:  ldarg.0
      IL_0023:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 28@30-3'::builder@
      IL_0028:  ldloc.1
      IL_0029:  ldloc.2
      IL_002a:  newobj     instance void assembly/'Pipe #3 input at line 28@31-4'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,
                                                                                               string)
      IL_002f:  tail.
      IL_0031:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_0036:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@32-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,string>>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 28@32-5' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,string>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,string> Invoke(class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (string V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_1,
               class [Utils]Utils/Product V_2)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>::get_Item3()
      IL_0014:  stloc.2
      IL_0015:  ldloc.0
      IL_0016:  ldloc.2
      IL_0017:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_001c:  newobj     instance void class [runtime]System.Tuple`2<string,string>::.ctor(!0,
                                                                                                    !1)
      IL_0021:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 28@32-5'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 28@32-5' assembly/'Pipe #3 input at line 28@32-5'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@39'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class assembly/'Pipe #4 input at line 37@39' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(string c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #4 input at line 37@39'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #4 input at line 37@39' assembly/'Pipe #4 input at line 37@39'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@39-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #4 input at line 37@39-1' @_instance
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
      IL_0000:  newobj     instance void assembly/'Pipe #4 input at line 37@39-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #4 input at line 37@39-1' assembly/'Pipe #4 input at line 37@39-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@39-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class assembly/'Pipe #4 input at line 37@39-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(string c,
                   class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                        !1)
      IL_0007:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #4 input at line 37@39-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #4 input at line 37@39-2' assembly/'Pipe #4 input at line 37@39-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@40-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps
    .field public string c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps,
                                 string c) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 37@40-4'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> assembly/'Pipe #4 input at line 37@40-4'::ps
      IL_0014:  ldarg.0
      IL_0015:  ldarg.3
      IL_0016:  stfld      string assembly/'Pipe #4 input at line 37@40-4'::c
      IL_001b:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object> Invoke(class [Utils]Utils/Product _arg2) cil managed
    {
      
      .maxstack  9
      .locals init (class [Utils]Utils/Product V_0,
               string V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  ldloc.0
      IL_0004:  box        [Utils]Utils/Product
      IL_0009:  ldnull
      IL_000a:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<object>(!!0,
                                                                                                                                    !!0)
      IL_000f:  brfalse.s  IL_0019

      IL_0011:  ldstr      "(No products)"
      IL_0016:  nop
      IL_0017:  br.s       IL_0020

      IL_0019:  ldloc.0
      IL_001a:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_001f:  nop
      IL_0020:  stloc.1
      IL_0021:  ldarg.0
      IL_0022:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 37@40-4'::builder@
      IL_0027:  ldarg.0
      IL_0028:  ldfld      string assembly/'Pipe #4 input at line 37@40-4'::c
      IL_002d:  ldarg.0
      IL_002e:  ldfld      class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> assembly/'Pipe #4 input at line 37@40-4'::ps
      IL_0033:  ldloc.0
      IL_0034:  ldloc.1
      IL_0035:  newobj     instance void class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::.ctor(!0,
                                                                                                                                                                                                                          !1,
                                                                                                                                                                                                                          !2,
                                                                                                                                                                                                                          !3)
      IL_003a:  tail.
      IL_003c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object>(!!0)
      IL_0041:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@39-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [runtime]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 37@39-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [runtime]System.Collections.IEnumerable> Invoke(class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> _arg1) cil managed
    {
      
      .maxstack  9
      .locals init (class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_1,
               string V_2)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance !1 class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_0008:  stloc.1
      IL_0009:  ldloc.0
      IL_000a:  call       instance !0 class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_000f:  stloc.2
      IL_0010:  ldarg.0
      IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 37@39-3'::builder@
      IL_0016:  ldarg.0
      IL_0017:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 37@39-3'::builder@
      IL_001c:  ldloc.1
      IL_001d:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [System.Linq]System.Linq.Enumerable::DefaultIfEmpty<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0022:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 37@39-3'::builder@
      IL_002d:  ldloc.1
      IL_002e:  ldloc.2
      IL_002f:  newobj     instance void assembly/'Pipe #4 input at line 37@40-4'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,
                                                                                               string)
      IL_0034:  tail.
      IL_0036:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_003b:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@42-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [runtime]System.Tuple`2<string,string>>
  {
    .field static assembly initonly class assembly/'Pipe #4 input at line 37@42-5' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [runtime]System.Tuple`2<string,string>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,string> Invoke(class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (string V_0,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_1,
               class [Utils]Utils/Product V_2,
               string V_3)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::get_Item3()
      IL_0014:  stloc.2
      IL_0015:  ldarg.1
      IL_0016:  call       instance !3 class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::get_Item4()
      IL_001b:  stloc.3
      IL_001c:  ldloc.0
      IL_001d:  ldloc.3
      IL_001e:  newobj     instance void class [runtime]System.Tuple`2<string,string>::.ctor(!0,
                                                                                                    !1)
      IL_0023:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #4 input at line 37@42-5'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #4 input at line 37@42-5' assembly/'Pipe #4 input at line 37@42-5'::@_instance
      IL_000a:  ret
    } 

  } 

  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> categories@8
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@9
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,string>[] q@11
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] q2@19
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,string>[] q3@27
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,string>[] q4@36
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> get_categories() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::categories@8
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> get_products() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::products@9
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,string>[] get_q() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,string>[] assembly::q@11
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] get_q2() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] assembly::q2@19
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,string>[] get_q3() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,string>[] assembly::q3@27
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,string>[] get_q4() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,string>[] assembly::q4@36
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
    
    .maxstack  10
    .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,string>> V_0,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>> V_2,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,string>> V_4,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_5,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,string>> V_6,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_7)
    IL_0000:  ldstr      "Beverages"
    IL_0005:  ldstr      "Condiments"
    IL_000a:  ldstr      "Vegetables"
    IL_000f:  ldstr      "Dairy Products"
    IL_0014:  ldstr      "Seafood"
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0023:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0028:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_002d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0032:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0037:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::categories@8
    IL_003c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_0041:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::products@9
    IL_0046:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_004b:  stloc.1
    IL_004c:  ldloc.1
    IL_004d:  ldloc.1
    IL_004e:  ldloc.1
    IL_004f:  ldloc.1
    IL_0050:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_categories()
    IL_0055:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_005a:  ldloc.1
    IL_005b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_0060:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0065:  ldsfld     class assembly/'Pipe #1 input at line 12@14' assembly/'Pipe #1 input at line 12@14'::@_instance
    IL_006a:  ldsfld     class assembly/'Pipe #1 input at line 12@14-1' assembly/'Pipe #1 input at line 12@14-1'::@_instance
    IL_006f:  ldsfld     class assembly/'Pipe #1 input at line 12@14-2' assembly/'Pipe #1 input at line 12@14-2'::@_instance
    IL_0074:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!4,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Join<string,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,string,class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1>,
                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>,
                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!4>>)
    IL_0079:  ldloc.1
    IL_007a:  newobj     instance void assembly/'Pipe #1 input at line 12@14-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_007f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0084:  ldsfld     class assembly/'Pipe #1 input at line 12@15-4' assembly/'Pipe #1 input at line 12@15-4'::@_instance
    IL_0089:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,string>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_008e:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,string>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_0093:  stloc.0
    IL_0094:  ldloc.0
    IL_0095:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,string>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_009a:  stsfld     class [runtime]System.Tuple`2<string,string>[] assembly::q@11
    IL_009f:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a4:  stloc.3
    IL_00a5:  ldloc.3
    IL_00a6:  ldloc.3
    IL_00a7:  ldloc.3
    IL_00a8:  ldloc.3
    IL_00a9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_categories()
    IL_00ae:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00b3:  ldloc.3
    IL_00b4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_00b9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00be:  ldsfld     class assembly/'Pipe #2 input at line 20@22' assembly/'Pipe #2 input at line 20@22'::@_instance
    IL_00c3:  ldsfld     class assembly/'Pipe #2 input at line 20@22-1' assembly/'Pipe #2 input at line 20@22-1'::@_instance
    IL_00c8:  ldsfld     class assembly/'Pipe #2 input at line 20@22-2' assembly/'Pipe #2 input at line 20@22-2'::@_instance
    IL_00cd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!4,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupJoin<string,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,string,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Collections.Generic.IEnumerable`1<!!2>,!!4>>)
    IL_00d2:  ldloc.3
    IL_00d3:  newobj     instance void assembly/'Pipe #2 input at line 20@22-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00d8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00dd:  ldsfld     class assembly/'Pipe #2 input at line 20@23-4' assembly/'Pipe #2 input at line 20@23-4'::@_instance
    IL_00e2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00e7:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_00ec:  stloc.2
    IL_00ed:  ldloc.2
    IL_00ee:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00f3:  stsfld     class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] assembly::q2@19
    IL_00f8:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00fd:  stloc.s    V_5
    IL_00ff:  ldloc.s    V_5
    IL_0101:  ldloc.s    V_5
    IL_0103:  ldloc.s    V_5
    IL_0105:  ldloc.s    V_5
    IL_0107:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_categories()
    IL_010c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0111:  ldloc.s    V_5
    IL_0113:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_0118:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_011d:  ldsfld     class assembly/'Pipe #3 input at line 28@30' assembly/'Pipe #3 input at line 28@30'::@_instance
    IL_0122:  ldsfld     class assembly/'Pipe #3 input at line 28@30-1' assembly/'Pipe #3 input at line 28@30-1'::@_instance
    IL_0127:  ldsfld     class assembly/'Pipe #3 input at line 28@30-2' assembly/'Pipe #3 input at line 28@30-2'::@_instance
    IL_012c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!4,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupJoin<string,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,string,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Collections.Generic.IEnumerable`1<!!2>,!!4>>)
    IL_0131:  ldloc.s    V_5
    IL_0133:  newobj     instance void assembly/'Pipe #3 input at line 28@30-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0138:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_013d:  ldsfld     class assembly/'Pipe #3 input at line 28@32-5' assembly/'Pipe #3 input at line 28@32-5'::@_instance
    IL_0142:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`3<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,string>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0147:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,string>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_014c:  stloc.s    V_4
    IL_014e:  ldloc.s    V_4
    IL_0150:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,string>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0155:  stsfld     class [runtime]System.Tuple`2<string,string>[] assembly::q3@27
    IL_015a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_015f:  stloc.s    V_7
    IL_0161:  ldloc.s    V_7
    IL_0163:  ldloc.s    V_7
    IL_0165:  ldloc.s    V_7
    IL_0167:  ldloc.s    V_7
    IL_0169:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_categories()
    IL_016e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0173:  ldloc.s    V_7
    IL_0175:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_017a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_017f:  ldsfld     class assembly/'Pipe #4 input at line 37@39' assembly/'Pipe #4 input at line 37@39'::@_instance
    IL_0184:  ldsfld     class assembly/'Pipe #4 input at line 37@39-1' assembly/'Pipe #4 input at line 37@39-1'::@_instance
    IL_0189:  ldsfld     class assembly/'Pipe #4 input at line 37@39-2' assembly/'Pipe #4 input at line 37@39-2'::@_instance
    IL_018e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!4,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupJoin<string,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,string,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Collections.Generic.IEnumerable`1<!!2>,!!4>>)
    IL_0193:  ldloc.s    V_7
    IL_0195:  newobj     instance void assembly/'Pipe #4 input at line 37@39-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_019a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_019f:  ldsfld     class assembly/'Pipe #4 input at line 37@42-5' assembly/'Pipe #4 input at line 37@42-5'::@_instance
    IL_01a4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`4<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,string>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01a9:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,string>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_01ae:  stloc.s    V_6
    IL_01b0:  ldloc.s    V_6
    IL_01b2:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,string>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01b7:  stsfld     class [runtime]System.Tuple`2<string,string>[] assembly::q4@36
    IL_01bc:  ret
  } 

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          categories()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_categories()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
  } 
  .property class [runtime]System.Tuple`2<string,string>[]
          q()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,string>[] assembly::get_q()
  } 
  .property class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[]
          q2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] assembly::get_q2()
  } 
  .property class [runtime]System.Tuple`2<string,string>[]
          q3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,string>[] assembly::get_q3()
  } 
  .property class [runtime]System.Tuple`2<string,string>[]
          q4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,string>[] assembly::get_q4()
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





