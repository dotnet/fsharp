
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
  .ver 6:0:0:0
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
.assembly Linq101Joins01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Joins01
{
  // Offset: 0x00000000 Length: 0x0000030A
}
.mresource public FSharpOptimizationData.Linq101Joins01
{
  // Offset: 0x00000310 Length: 0x000000C3
}
.module Linq101Joins01.exe
// MVID: {61E07032-151B-685E-A745-03833270E061}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06FE0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Joins01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@14'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #1 input at line 12@14' @_instance
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
    } // end of method 'Pipe #1 input at line 12@14'::.ctor

    .method public strict virtual instance string 
            Invoke(string c) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 14,14 : 32,33 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Joins01.fs'
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #1 input at line 12@14'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #1 input at line 12@14'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #1 input at line 12@14' Linq101Joins01/'Pipe #1 input at line 12@14'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 12@14'::.cctor

  } // end of class 'Pipe #1 input at line 12@14'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@14-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #1 input at line 12@14-1' @_instance
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
    } // end of method 'Pipe #1 input at line 12@14-1'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 14,14 : 36,46 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #1 input at line 12@14-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #1 input at line 12@14-1'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #1 input at line 12@14-1' Linq101Joins01/'Pipe #1 input at line 12@14-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 12@14-1'::.cctor

  } // end of class 'Pipe #1 input at line 12@14-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@14-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [Utils]Utils/Product,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #1 input at line 12@14-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [Utils]Utils/Product,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 12@14-2'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product> 
            Invoke(string c,
                   class [Utils]Utils/Product p) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 14,14 : 9,47 ''
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>::.ctor(!0,
                                                                                                                  !1)
      IL_0007:  ret
    } // end of method 'Pipe #1 input at line 12@14-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #1 input at line 12@14-2'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #1 input at line 12@14-2' Linq101Joins01/'Pipe #1 input at line 12@14-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 12@14-2'::.cctor

  } // end of class 'Pipe #1 input at line 12@14-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@14-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #1 input at line 12@14-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #1 input at line 12@14-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,object> 
            Invoke(class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product> _arg1) cil managed
    {
      // Code size       37 (0x25)
      .maxstack  7
      .locals init ([0] class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product> V_0,
               [1] class [Utils]Utils/Product p,
               [2] string c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance !1 class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>::get_Item2()
      IL_0008:  stloc.1
      IL_0009:  ldloc.0
      IL_000a:  call       instance !0 class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>::get_Item1()
      IL_000f:  stloc.2
      .line 15,15 : 9,34 ''
      IL_0010:  ldarg.0
      IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #1 input at line 12@14-3'::builder@
      IL_0016:  ldloc.2
      IL_0017:  ldloc.1
      IL_0018:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>::.ctor(!0,
                                                                                                                  !1)
      IL_001d:  tail.
      IL_001f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0024:  ret
    } // end of method 'Pipe #1 input at line 12@14-3'::Invoke

  } // end of class 'Pipe #1 input at line 12@14-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 12@15-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,string>>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #1 input at line 12@15-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,string>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 12@15-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,string> 
            Invoke(class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product> tupledArg) cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  6
      .locals init ([0] string c,
               [1] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>::get_Item2()
      IL_000d:  stloc.1
      .line 15,15 : 17,33 ''
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0015:  newobj     instance void class [mscorlib]System.Tuple`2<string,string>::.ctor(!0,
                                                                                              !1)
      IL_001a:  ret
    } // end of method 'Pipe #1 input at line 12@15-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #1 input at line 12@15-4'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #1 input at line 12@15-4' Linq101Joins01/'Pipe #1 input at line 12@15-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 12@15-4'::.cctor

  } // end of class 'Pipe #1 input at line 12@15-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@22'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #2 input at line 20@22' @_instance
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
    } // end of method 'Pipe #2 input at line 20@22'::.ctor

    .method public strict virtual instance string 
            Invoke(string c) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 22,22 : 37,38 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #2 input at line 20@22'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #2 input at line 20@22'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #2 input at line 20@22' Linq101Joins01/'Pipe #2 input at line 20@22'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 20@22'::.cctor

  } // end of class 'Pipe #2 input at line 20@22'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@22-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #2 input at line 20@22-1' @_instance
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
    } // end of method 'Pipe #2 input at line 20@22-1'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 22,22 : 41,51 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #2 input at line 20@22-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #2 input at line 20@22-1'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #2 input at line 20@22-1' Linq101Joins01/'Pipe #2 input at line 20@22-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 20@22-1'::.cctor

  } // end of class 'Pipe #2 input at line 20@22-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@22-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #2 input at line 20@22-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 20@22-2'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(string c,
                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 22,22 : 9,60 ''
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0007:  ret
    } // end of method 'Pipe #2 input at line 20@22-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #2 input at line 20@22-2'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #2 input at line 20@22-2' Linq101Joins01/'Pipe #2 input at line 20@22-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 20@22-2'::.cctor

  } // end of class 'Pipe #2 input at line 20@22-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@22-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #2 input at line 20@22-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #2 input at line 20@22-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object> 
            Invoke(class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> _arg1) cil managed
    {
      // Code size       37 (0x25)
      .maxstack  7
      .locals init ([0] class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> V_0,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps,
               [2] string c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance !1 class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_0008:  stloc.1
      IL_0009:  ldloc.0
      IL_000a:  call       instance !0 class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_000f:  stloc.2
      .line 23,23 : 9,23 ''
      IL_0010:  ldarg.0
      IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #2 input at line 20@22-3'::builder@
      IL_0016:  ldloc.2
      IL_0017:  ldloc.1
      IL_0018:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_001d:  tail.
      IL_001f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_0024:  ret
    } // end of method 'Pipe #2 input at line 20@22-3'::Invoke

  } // end of class 'Pipe #2 input at line 20@22-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 20@23-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #2 input at line 20@23-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 20@23-4'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> tupledArg) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  6
      .locals init ([0] string c,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      .line 23,23 : 17,22 ''
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0015:  ret
    } // end of method 'Pipe #2 input at line 20@23-4'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #2 input at line 20@23-4'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #2 input at line 20@23-4' Linq101Joins01/'Pipe #2 input at line 20@23-4'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 20@23-4'::.cctor

  } // end of class 'Pipe #2 input at line 20@23-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@30'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #3 input at line 28@30' @_instance
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
    } // end of method 'Pipe #3 input at line 28@30'::.ctor

    .method public strict virtual instance string 
            Invoke(string c) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 30,30 : 37,38 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #3 input at line 28@30'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #3 input at line 28@30'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #3 input at line 28@30' Linq101Joins01/'Pipe #3 input at line 28@30'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 28@30'::.cctor

  } // end of class 'Pipe #3 input at line 28@30'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@30-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #3 input at line 28@30-1' @_instance
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
    } // end of method 'Pipe #3 input at line 28@30-1'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 30,30 : 41,51 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #3 input at line 28@30-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #3 input at line 28@30-1'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #3 input at line 28@30-1' Linq101Joins01/'Pipe #3 input at line 28@30-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 28@30-1'::.cctor

  } // end of class 'Pipe #3 input at line 28@30-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@30-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #3 input at line 28@30-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 28@30-2'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(string c,
                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 30,30 : 9,60 ''
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0007:  ret
    } // end of method 'Pipe #3 input at line 28@30-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #3 input at line 28@30-2'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #3 input at line 28@30-2' Linq101Joins01/'Pipe #3 input at line 28@30-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 28@30-2'::.cctor

  } // end of class 'Pipe #3 input at line 28@30-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@31-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps
    .field public string c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps,
                                 string c) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #3 input at line 28@31-4'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Linq101Joins01/'Pipe #3 input at line 28@31-4'::ps
      IL_0014:  ldarg.0
      IL_0015:  ldarg.3
      IL_0016:  stfld      string Linq101Joins01/'Pipe #3 input at line 28@31-4'::c
      IL_001b:  ret
    } // end of method 'Pipe #3 input at line 28@31-4'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object> 
            Invoke(class [Utils]Utils/Product _arg2) cil managed
    {
      // Code size       34 (0x22)
      .maxstack  8
      .locals init ([0] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 32,32 : 9,34 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #3 input at line 28@31-4'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      string Linq101Joins01/'Pipe #3 input at line 28@31-4'::c
      IL_000e:  ldarg.0
      IL_000f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Linq101Joins01/'Pipe #3 input at line 28@31-4'::ps
      IL_0014:  ldloc.0
      IL_0015:  newobj     instance void class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>::.ctor(!0,
                                                                                                                                                                                                       !1,
                                                                                                                                                                                                       !2)
      IL_001a:  tail.
      IL_001c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object>(!!0)
      IL_0021:  ret
    } // end of method 'Pipe #3 input at line 28@31-4'::Invoke

  } // end of class 'Pipe #3 input at line 28@31-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@30-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #3 input at line 28@30-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #3 input at line 28@30-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> _arg1) cil managed
    {
      // Code size       55 (0x37)
      .maxstack  9
      .locals init ([0] class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> V_0,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps,
               [2] string c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance !1 class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_0008:  stloc.1
      IL_0009:  ldloc.0
      IL_000a:  call       instance !0 class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_000f:  stloc.2
      .line 31,31 : 9,12 ''
      IL_0010:  ldarg.0
      IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #3 input at line 28@30-3'::builder@
      IL_0016:  ldarg.0
      IL_0017:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #3 input at line 28@30-3'::builder@
      IL_001c:  ldloc.1
      IL_001d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0022:  ldarg.0
      IL_0023:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #3 input at line 28@30-3'::builder@
      IL_0028:  ldloc.1
      IL_0029:  ldloc.2
      IL_002a:  newobj     instance void Linq101Joins01/'Pipe #3 input at line 28@31-4'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                               class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,
                                                                                               string)
      IL_002f:  tail.
      IL_0031:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_0036:  ret
    } // end of method 'Pipe #3 input at line 28@30-3'::Invoke

  } // end of class 'Pipe #3 input at line 28@30-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 28@32-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,string>>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #3 input at line 28@32-5' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,string>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 28@32-5'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,string> 
            Invoke(class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product> tupledArg) cil managed
    {
      // Code size       34 (0x22)
      .maxstack  6
      .locals init ([0] string c,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps,
               [2] class [Utils]Utils/Product p)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>::get_Item3()
      IL_0014:  stloc.2
      .line 32,32 : 17,33 ''
      IL_0015:  ldloc.0
      IL_0016:  ldloc.2
      IL_0017:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_001c:  newobj     instance void class [mscorlib]System.Tuple`2<string,string>::.ctor(!0,
                                                                                              !1)
      IL_0021:  ret
    } // end of method 'Pipe #3 input at line 28@32-5'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #3 input at line 28@32-5'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #3 input at line 28@32-5' Linq101Joins01/'Pipe #3 input at line 28@32-5'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 28@32-5'::.cctor

  } // end of class 'Pipe #3 input at line 28@32-5'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@39'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,string>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #4 input at line 37@39' @_instance
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
    } // end of method 'Pipe #4 input at line 37@39'::.ctor

    .method public strict virtual instance string 
            Invoke(string c) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 39,39 : 37,38 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #4 input at line 37@39'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #4 input at line 37@39'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #4 input at line 37@39' Linq101Joins01/'Pipe #4 input at line 37@39'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 37@39'::.cctor

  } // end of class 'Pipe #4 input at line 37@39'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@39-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #4 input at line 37@39-1' @_instance
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
    } // end of method 'Pipe #4 input at line 37@39-1'::.ctor

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 39,39 : 41,51 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } // end of method 'Pipe #4 input at line 37@39-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #4 input at line 37@39-1'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #4 input at line 37@39-1' Linq101Joins01/'Pipe #4 input at line 37@39-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 37@39-1'::.cctor

  } // end of class 'Pipe #4 input at line 37@39-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@39-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #4 input at line 37@39-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 37@39-2'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> 
            Invoke(string c,
                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      .line 39,39 : 9,60 ''
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0007:  ret
    } // end of method 'Pipe #4 input at line 37@39-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #4 input at line 37@39-2'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #4 input at line 37@39-2' Linq101Joins01/'Pipe #4 input at line 37@39-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 37@39-2'::.cctor

  } // end of class 'Pipe #4 input at line 37@39-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@40-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps
    .field public string c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps,
                                 string c) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #4 input at line 37@40-4'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Linq101Joins01/'Pipe #4 input at line 37@40-4'::ps
      IL_0014:  ldarg.0
      IL_0015:  ldarg.3
      IL_0016:  stfld      string Linq101Joins01/'Pipe #4 input at line 37@40-4'::c
      IL_001b:  ret
    } // end of method 'Pipe #4 input at line 37@40-4'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object> 
            Invoke(class [Utils]Utils/Product _arg2) cil managed
    {
      // Code size       66 (0x42)
      .maxstack  9
      .locals init ([0] class [Utils]Utils/Product p,
               [1] string t)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 41,41 : 17,39 ''
      IL_0002:  nop
      .line 100001,100001 : 0,0 ''
      IL_0003:  ldloc.0
      IL_0004:  box        [Utils]Utils/Product
      IL_0009:  ldnull
      IL_000a:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<object>(!!0,
                                                                                                                                    !!0)
      IL_000f:  brfalse.s  IL_0019

      .line 41,41 : 40,55 ''
      IL_0011:  ldstr      "(No products)"
      .line 100001,100001 : 0,0 ''
      IL_0016:  nop
      IL_0017:  br.s       IL_0020

      .line 41,41 : 61,74 ''
      IL_0019:  ldloc.0
      IL_001a:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      .line 100001,100001 : 0,0 ''
      IL_001f:  nop
      .line 100001,100001 : 0,0 ''
      IL_0020:  stloc.1
      .line 42,42 : 9,22 ''
      IL_0021:  ldarg.0
      IL_0022:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #4 input at line 37@40-4'::builder@
      IL_0027:  ldarg.0
      IL_0028:  ldfld      string Linq101Joins01/'Pipe #4 input at line 37@40-4'::c
      IL_002d:  ldarg.0
      IL_002e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Linq101Joins01/'Pipe #4 input at line 37@40-4'::ps
      IL_0033:  ldloc.0
      IL_0034:  ldloc.1
      IL_0035:  newobj     instance void class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::.ctor(!0,
                                                                                                                                                                                                              !1,
                                                                                                                                                                                                              !2,
                                                                                                                                                                                                              !3)
      IL_003a:  tail.
      IL_003c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object>(!!0)
      IL_0041:  ret
    } // end of method 'Pipe #4 input at line 37@40-4'::Invoke

  } // end of class 'Pipe #4 input at line 37@40-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@39-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [mscorlib]System.Collections.IEnumerable>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [mscorlib]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #4 input at line 37@39-3'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #4 input at line 37@39-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [mscorlib]System.Collections.IEnumerable> 
            Invoke(class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> _arg1) cil managed
    {
      // Code size       60 (0x3c)
      .maxstack  9
      .locals init ([0] class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> V_0,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps,
               [2] string c)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance !1 class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_0008:  stloc.1
      IL_0009:  ldloc.0
      IL_000a:  call       instance !0 class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_000f:  stloc.2
      .line 40,40 : 9,12 ''
      IL_0010:  ldarg.0
      IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #4 input at line 37@39-3'::builder@
      IL_0016:  ldarg.0
      IL_0017:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #4 input at line 37@39-3'::builder@
      IL_001c:  ldloc.1
      IL_001d:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::DefaultIfEmpty<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0022:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Joins01/'Pipe #4 input at line 37@39-3'::builder@
      IL_002d:  ldloc.1
      IL_002e:  ldloc.2
      IL_002f:  newobj     instance void Linq101Joins01/'Pipe #4 input at line 37@40-4'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                               class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,
                                                                                               string)
      IL_0034:  tail.
      IL_0036:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_003b:  ret
    } // end of method 'Pipe #4 input at line 37@39-3'::Invoke

  } // end of class 'Pipe #4 input at line 37@39-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 37@42-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [mscorlib]System.Tuple`2<string,string>>
  {
    .field static assembly initonly class Linq101Joins01/'Pipe #4 input at line 37@42-5' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [mscorlib]System.Tuple`2<string,string>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #4 input at line 37@42-5'::.ctor

    .method public strict virtual instance class [mscorlib]System.Tuple`2<string,string> 
            Invoke(class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string> tupledArg) cil managed
    {
      // Code size       36 (0x24)
      .maxstack  6
      .locals init ([0] string c,
               [1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> ps,
               [2] class [Utils]Utils/Product p,
               [3] string t)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::get_Item3()
      IL_0014:  stloc.2
      IL_0015:  ldarg.1
      IL_0016:  call       instance !3 class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>::get_Item4()
      IL_001b:  stloc.3
      .line 42,42 : 17,21 ''
      IL_001c:  ldloc.0
      IL_001d:  ldloc.3
      IL_001e:  newobj     instance void class [mscorlib]System.Tuple`2<string,string>::.ctor(!0,
                                                                                              !1)
      IL_0023:  ret
    } // end of method 'Pipe #4 input at line 37@42-5'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Joins01/'Pipe #4 input at line 37@42-5'::.ctor()
      IL_0005:  stsfld     class Linq101Joins01/'Pipe #4 input at line 37@42-5' Linq101Joins01/'Pipe #4 input at line 37@42-5'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #4 input at line 37@42-5'::.cctor

  } // end of class 'Pipe #4 input at line 37@42-5'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_categories() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Joins01>'.$Linq101Joins01::categories@8
    IL_0005:  ret
  } // end of method Linq101Joins01::get_categories

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Joins01>'.$Linq101Joins01::products@9
    IL_0005:  ret
  } // end of method Linq101Joins01::get_products

  .method public specialname static class [mscorlib]System.Tuple`2<string,string>[] 
          get_q() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,string>[] '<StartupCode$Linq101Joins01>'.$Linq101Joins01::q@11
    IL_0005:  ret
  } // end of method Linq101Joins01::get_q

  .method public specialname static class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] 
          get_q2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Joins01>'.$Linq101Joins01::q2@19
    IL_0005:  ret
  } // end of method Linq101Joins01::get_q2

  .method public specialname static class [mscorlib]System.Tuple`2<string,string>[] 
          get_q3() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,string>[] '<StartupCode$Linq101Joins01>'.$Linq101Joins01::q3@27
    IL_0005:  ret
  } // end of method Linq101Joins01::get_q3

  .method public specialname static class [mscorlib]System.Tuple`2<string,string>[] 
          get_q4() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Tuple`2<string,string>[] '<StartupCode$Linq101Joins01>'.$Linq101Joins01::q4@36
    IL_0005:  ret
  } // end of method Linq101Joins01::get_q4

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          categories()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Joins01::get_categories()
  } // end of property Linq101Joins01::categories
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Joins01::get_products()
  } // end of property Linq101Joins01::products
  .property class [mscorlib]System.Tuple`2<string,string>[]
          q()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,string>[] Linq101Joins01::get_q()
  } // end of property Linq101Joins01::q
  .property class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[]
          q2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] Linq101Joins01::get_q2()
  } // end of property Linq101Joins01::q2
  .property class [mscorlib]System.Tuple`2<string,string>[]
          q3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,string>[] Linq101Joins01::get_q3()
  } // end of property Linq101Joins01::q3
  .property class [mscorlib]System.Tuple`2<string,string>[]
          q4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Tuple`2<string,string>[] Linq101Joins01::get_q4()
  } // end of property Linq101Joins01::q4
} // end of class Linq101Joins01

.class private abstract auto ansi sealed '<StartupCode$Linq101Joins01>'.$Linq101Joins01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> categories@8
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@9
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,string>[] q@11
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] q2@19
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,string>[] q3@27
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Tuple`2<string,string>[] q4@36
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       481 (0x1e1)
    .maxstack  10
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> categories,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products,
             [2] class [mscorlib]System.Tuple`2<string,string>[] q,
             [3] class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] q2,
             [4] class [mscorlib]System.Tuple`2<string,string>[] q3,
             [5] class [mscorlib]System.Tuple`2<string,string>[] q4,
             [6] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,string>> 'Pipe #1 input at line 12',
             [7] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_7,
             [8] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>> 'Pipe #2 input at line 20',
             [9] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_9,
             [10] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,string>> 'Pipe #3 input at line 28',
             [11] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11,
             [12] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Tuple`2<string,string>> 'Pipe #4 input at line 37',
             [13] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_13)
    .line 8,8 : 1,88 ''
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
    IL_0037:  dup
    IL_0038:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Joins01>'.$Linq101Joins01::categories@8
    IL_003d:  stloc.0
    .line 9,9 : 1,32 ''
    IL_003e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_0043:  dup
    IL_0044:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Joins01>'.$Linq101Joins01::products@9
    IL_0049:  stloc.1
    .line 11,16 : 1,21 ''
    IL_004a:  nop
    .line 12,12 : 5,10 ''
    IL_004b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0050:  stloc.s    V_7
    .line 12,12 : 5,10 ''
    IL_0052:  ldloc.s    V_7
    IL_0054:  ldloc.s    V_7
    IL_0056:  ldloc.s    V_7
    IL_0058:  ldloc.s    V_7
    IL_005a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Joins01::get_categories()
    IL_005f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0064:  ldloc.s    V_7
    IL_0066:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Joins01::get_products()
    IL_006b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0070:  ldsfld     class Linq101Joins01/'Pipe #1 input at line 12@14' Linq101Joins01/'Pipe #1 input at line 12@14'::@_instance
    IL_0075:  ldsfld     class Linq101Joins01/'Pipe #1 input at line 12@14-1' Linq101Joins01/'Pipe #1 input at line 12@14-1'::@_instance
    IL_007a:  ldsfld     class Linq101Joins01/'Pipe #1 input at line 12@14-2' Linq101Joins01/'Pipe #1 input at line 12@14-2'::@_instance
    IL_007f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!4,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Join<string,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,string,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1>,
                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>,
                                                                                                                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!4>>)
    IL_0084:  ldloc.s    V_7
    IL_0086:  newobj     instance void Linq101Joins01/'Pipe #1 input at line 12@14-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_008b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0090:  ldsfld     class Linq101Joins01/'Pipe #1 input at line 12@15-4' Linq101Joins01/'Pipe #1 input at line 12@15-4'::@_instance
    IL_0095:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<string,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,string>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_009a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,string>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_009f:  stloc.s    'Pipe #1 input at line 12'
    .line 16,16 : 10,21 ''
    IL_00a1:  ldloc.s    'Pipe #1 input at line 12'
    IL_00a3:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,string>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00a8:  dup
    IL_00a9:  stsfld     class [mscorlib]System.Tuple`2<string,string>[] '<StartupCode$Linq101Joins01>'.$Linq101Joins01::q@11
    IL_00ae:  stloc.2
    .line 19,24 : 1,21 ''
    IL_00af:  nop
    .line 20,20 : 5,10 ''
    IL_00b0:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00b5:  stloc.s    V_9
    .line 20,20 : 5,10 ''
    IL_00b7:  ldloc.s    V_9
    IL_00b9:  ldloc.s    V_9
    IL_00bb:  ldloc.s    V_9
    IL_00bd:  ldloc.s    V_9
    IL_00bf:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Joins01::get_categories()
    IL_00c4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00c9:  ldloc.s    V_9
    IL_00cb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Joins01::get_products()
    IL_00d0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00d5:  ldsfld     class Linq101Joins01/'Pipe #2 input at line 20@22' Linq101Joins01/'Pipe #2 input at line 20@22'::@_instance
    IL_00da:  ldsfld     class Linq101Joins01/'Pipe #2 input at line 20@22-1' Linq101Joins01/'Pipe #2 input at line 20@22-1'::@_instance
    IL_00df:  ldsfld     class Linq101Joins01/'Pipe #2 input at line 20@22-2' Linq101Joins01/'Pipe #2 input at line 20@22-2'::@_instance
    IL_00e4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!4,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupJoin<string,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,string,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2>,!!4>>)
    IL_00e9:  ldloc.s    V_9
    IL_00eb:  newobj     instance void Linq101Joins01/'Pipe #2 input at line 20@22-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00f0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00f5:  ldsfld     class Linq101Joins01/'Pipe #2 input at line 20@23-4' Linq101Joins01/'Pipe #2 input at line 20@23-4'::@_instance
    IL_00fa:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00ff:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0104:  stloc.s    'Pipe #2 input at line 20'
    .line 24,24 : 10,21 ''
    IL_0106:  ldloc.s    'Pipe #2 input at line 20'
    IL_0108:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_010d:  dup
    IL_010e:  stsfld     class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] '<StartupCode$Linq101Joins01>'.$Linq101Joins01::q2@19
    IL_0113:  stloc.3
    .line 27,33 : 1,21 ''
    IL_0114:  nop
    .line 28,28 : 5,10 ''
    IL_0115:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_011a:  stloc.s    V_11
    .line 28,28 : 5,10 ''
    IL_011c:  ldloc.s    V_11
    IL_011e:  ldloc.s    V_11
    IL_0120:  ldloc.s    V_11
    IL_0122:  ldloc.s    V_11
    IL_0124:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Joins01::get_categories()
    IL_0129:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_012e:  ldloc.s    V_11
    IL_0130:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Joins01::get_products()
    IL_0135:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_013a:  ldsfld     class Linq101Joins01/'Pipe #3 input at line 28@30' Linq101Joins01/'Pipe #3 input at line 28@30'::@_instance
    IL_013f:  ldsfld     class Linq101Joins01/'Pipe #3 input at line 28@30-1' Linq101Joins01/'Pipe #3 input at line 28@30-1'::@_instance
    IL_0144:  ldsfld     class Linq101Joins01/'Pipe #3 input at line 28@30-2' Linq101Joins01/'Pipe #3 input at line 28@30-2'::@_instance
    IL_0149:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!4,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupJoin<string,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,string,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2>,!!4>>)
    IL_014e:  ldloc.s    V_11
    IL_0150:  newobj     instance void Linq101Joins01/'Pipe #3 input at line 28@30-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0155:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_015a:  ldsfld     class Linq101Joins01/'Pipe #3 input at line 28@32-5' Linq101Joins01/'Pipe #3 input at line 28@32-5'::@_instance
    IL_015f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`3<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,string>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0164:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,string>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0169:  stloc.s    'Pipe #3 input at line 28'
    .line 33,33 : 10,21 ''
    IL_016b:  ldloc.s    'Pipe #3 input at line 28'
    IL_016d:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,string>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0172:  dup
    IL_0173:  stsfld     class [mscorlib]System.Tuple`2<string,string>[] '<StartupCode$Linq101Joins01>'.$Linq101Joins01::q3@27
    IL_0178:  stloc.s    q3
    .line 36,43 : 1,21 ''
    IL_017a:  nop
    .line 37,37 : 5,10 ''
    IL_017b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0180:  stloc.s    V_13
    .line 37,37 : 5,10 ''
    IL_0182:  ldloc.s    V_13
    IL_0184:  ldloc.s    V_13
    IL_0186:  ldloc.s    V_13
    IL_0188:  ldloc.s    V_13
    IL_018a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Joins01::get_categories()
    IL_018f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0194:  ldloc.s    V_13
    IL_0196:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Joins01::get_products()
    IL_019b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01a0:  ldsfld     class Linq101Joins01/'Pipe #4 input at line 37@39' Linq101Joins01/'Pipe #4 input at line 37@39'::@_instance
    IL_01a5:  ldsfld     class Linq101Joins01/'Pipe #4 input at line 37@39-1' Linq101Joins01/'Pipe #4 input at line 37@39-1'::@_instance
    IL_01aa:  ldsfld     class Linq101Joins01/'Pipe #4 input at line 37@39-2' Linq101Joins01/'Pipe #4 input at line 37@39-2'::@_instance
    IL_01af:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!4,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupJoin<string,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,string,class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!2,!!3>,
                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2>,!!4>>)
    IL_01b4:  ldloc.s    V_13
    IL_01b6:  newobj     instance void Linq101Joins01/'Pipe #4 input at line 37@39-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01bb:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [mscorlib]System.Tuple`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01c0:  ldsfld     class Linq101Joins01/'Pipe #4 input at line 37@42-5' Linq101Joins01/'Pipe #4 input at line 37@42-5'::@_instance
    IL_01c5:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [mscorlib]System.Tuple`4<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product,string>,class [mscorlib]System.Collections.IEnumerable,class [mscorlib]System.Tuple`2<string,string>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01ca:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [mscorlib]System.Tuple`2<string,string>,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_01cf:  stloc.s    'Pipe #4 input at line 37'
    .line 43,43 : 10,21 ''
    IL_01d1:  ldloc.s    'Pipe #4 input at line 37'
    IL_01d3:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [mscorlib]System.Tuple`2<string,string>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01d8:  dup
    IL_01d9:  stsfld     class [mscorlib]System.Tuple`2<string,string>[] '<StartupCode$Linq101Joins01>'.$Linq101Joins01::q4@36
    IL_01de:  stloc.s    q4
    IL_01e0:  ret
  } // end of method $Linq101Joins01::main@

} // end of class '<StartupCode$Linq101Joins01>'.$Linq101Joins01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
