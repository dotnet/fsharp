
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
.assembly Linq101Where01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Where01
{
  // Offset: 0x00000000 Length: 0x000003D6
}
.mresource public FSharpOptimizationData.Linq101Where01
{
  // Offset: 0x000003E0 Length: 0x0000012E
}
.module Linq101Where01.exe
// MVID: {5B9A632A-FF23-CD21-A745-03832A639A5B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x009E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Where01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'lowNums@14-3'
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/'lowNums@14-3'::builder@
      IL_000d:  ret
    } // end of method 'lowNums@14-3'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] int32 n)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 14,14 : 9,28 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\QueryExpressionStepping\\Linq101Where01.fs'
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 15,15 : 9,22 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/'lowNums@14-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<int32,object>(!!0)
      IL_0010:  ret
    } // end of method 'lowNums@14-3'::Invoke

  } // end of class 'lowNums@14-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'lowNums@15-4'
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
    } // end of method 'lowNums@15-4'::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      .line 15,15 : 16,21 ''
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.5
      IL_0002:  clt
      IL_0004:  ret
    } // end of method 'lowNums@15-4'::Invoke

  } // end of class 'lowNums@15-4'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'lowNums@16-5'
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
    } // end of method 'lowNums@16-5'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 16,16 : 16,17 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'lowNums@16-5'::Invoke

  } // end of class 'lowNums@16-5'

  .class auto ansi serializable sealed nested assembly beforefieldinit soldOutProducts@24
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/soldOutProducts@24::builder@
      IL_000d:  ret
    } // end of method soldOutProducts@24::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 24,24 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 25,25 : 9,35 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/soldOutProducts@24::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method soldOutProducts@24::Invoke

  } // end of class soldOutProducts@24

  .class auto ansi serializable sealed nested assembly beforefieldinit 'soldOutProducts@25-1'
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
    } // end of method 'soldOutProducts@25-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      .line 25,25 : 16,34 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method 'soldOutProducts@25-1'::Invoke

  } // end of class 'soldOutProducts@25-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'soldOutProducts@26-2'
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
    } // end of method 'soldOutProducts@26-2'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 26,26 : 16,17 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'soldOutProducts@26-2'::Invoke

  } // end of class 'soldOutProducts@26-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit expensiveInStockProducts@32
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
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/expensiveInStockProducts@32::builder@
      IL_000d:  ret
    } // end of method expensiveInStockProducts@32::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Product p)
      .line 32,32 : 9,29 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 33,33 : 9,58 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/expensiveInStockProducts@32::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } // end of method expensiveInStockProducts@32::Invoke

  } // end of class expensiveInStockProducts@32

  .class auto ansi serializable sealed nested assembly beforefieldinit 'expensiveInStockProducts@33-1'
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
    } // end of method 'expensiveInStockProducts@33-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       41 (0x29)
      .maxstack  10
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0006:  ldc.i4.0
      IL_0007:  ble.s      IL_000b

      IL_0009:  br.s       IL_000d

      IL_000b:  br.s       IL_0027

      .line 100001,100001 : 0,0 ''
      IL_000d:  ldarg.1
      IL_000e:  callvirt   instance valuetype [mscorlib]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0013:  ldc.i4     0x12c
      IL_0018:  ldc.i4.0
      IL_0019:  ldc.i4.0
      IL_001a:  ldc.i4.0
      IL_001b:  ldc.i4.2
      IL_001c:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                         int32,
                                                                         int32,
                                                                         bool,
                                                                         uint8)
      IL_0021:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                         valuetype [mscorlib]System.Decimal)
      IL_0026:  ret

      .line 100001,100001 : 0,0 ''
      IL_0027:  ldc.i4.0
      IL_0028:  ret
    } // end of method 'expensiveInStockProducts@33-1'::Invoke

  } // end of class 'expensiveInStockProducts@33-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'expensiveInStockProducts@34-2'
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
    } // end of method 'expensiveInStockProducts@34-2'::.ctor

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 34,34 : 16,17 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'expensiveInStockProducts@34-2'::Invoke

  } // end of class 'expensiveInStockProducts@34-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit waCustomers@42
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>
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
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/waCustomers@42::builder@
      IL_000d:  ret
    } // end of method waCustomers@42::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init ([0] class [Utils]Utils/Customer c)
      .line 42,42 : 9,30 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 43,43 : 9,32 ''
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/waCustomers@42::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Customer,object>(!!0)
      IL_0010:  ret
    } // end of method waCustomers@42::Invoke

  } // end of class waCustomers@42

  .class auto ansi serializable sealed nested assembly beforefieldinit 'waCustomers@43-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'waCustomers@43-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Customer c) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      .line 43,43 : 16,31 ''
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance string [Utils]Utils/Customer::get_Region()
      IL_0006:  ldstr      "WA"
      IL_000b:  call       bool [mscorlib]System.String::Equals(string,
                                                                string)
      IL_0010:  ret
    } // end of method 'waCustomers@43-1'::Invoke

  } // end of class 'waCustomers@43-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'waCustomers@44-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [Utils]Utils/Customer>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [Utils]Utils/Customer>::.ctor()
      IL_0006:  ret
    } // end of method 'waCustomers@44-2'::.ctor

    .method public strict virtual instance class [Utils]Utils/Customer 
            Invoke(class [Utils]Utils/Customer c) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      .line 44,44 : 16,17 ''
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'waCustomers@44-2'::Invoke

  } // end of class 'waCustomers@44-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit shortDigits@55
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>::.ctor()
      IL_0006:  ret
    } // end of method shortDigits@55::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> x) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      .line 55,55 : 19,21 ''
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>(!!0)
      IL_0008:  ret
    } // end of method shortDigits@55::Invoke

  } // end of class shortDigits@55

  .class auto ansi serializable sealed nested assembly beforefieldinit 'shortDigits@54-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<int32,string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<int32,string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>::.ctor()
      IL_0006:  ret
    } // end of method 'shortDigits@54-1'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> 
            Invoke(int32 i,
                   string d) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      .line 54,54 : 29,49 ''
      IL_0000:  ldarg.2
      IL_0001:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0006:  ldarg.1
      IL_0007:  bge.s      IL_000b

      IL_0009:  br.s       IL_000d

      IL_000b:  br.s       IL_0014

      .line 54,54 : 50,57 ''
      IL_000d:  ldarg.2
      IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>::Some(!0)
      IL_0013:  ret

      .line 54,54 : 63,67 ''
      IL_0014:  ldnull
      IL_0015:  ret
    } // end of method 'shortDigits@54-1'::Invoke

  } // end of class 'shortDigits@54-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'shortDigits@51-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<string>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<string>>::.ctor()
      IL_0006:  ret
    } // end of method 'shortDigits@51-3'::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
            Invoke(string _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init ([0] string d)
      .line 51,51 : 9,27 ''
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      .line 52,52 : 9,17 ''
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<string>(!!0)
      IL_000a:  ret
    } // end of method 'shortDigits@51-3'::Invoke

  } // end of class 'shortDigits@51-3'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'shortDigits@52-2'
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
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'shortDigits@52-2'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Where01/'shortDigits@52-2'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Where01/'shortDigits@52-2'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method 'shortDigits@52-2'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       162 (0xa2)
      .maxstack  7
      .locals init ([0] string d)
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Where01/'shortDigits@52-2'::pc
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
      IL_0022:  br.s       IL_0078

      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  br.s       IL_0075

      .line 100001,100001 : 0,0 ''
      IL_0027:  nop
      IL_0028:  br.s       IL_0099

      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      .line 52,52 : 9,17 ''
      IL_002b:  ldarg.0
      IL_002c:  newobj     instance void Linq101Where01/'shortDigits@51-3'::.ctor()
      IL_0031:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Where01::get_digits()
      IL_0036:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<string,class [mscorlib]System.Collections.Generic.IEnumerable`1<string>,string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                      class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_003b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0040:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'shortDigits@52-2'::'enum'
      IL_0045:  ldarg.0
      IL_0046:  ldc.i4.1
      IL_0047:  stfld      int32 Linq101Where01/'shortDigits@52-2'::pc
      .line 52,52 : 9,17 ''
      IL_004c:  ldarg.0
      IL_004d:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'shortDigits@52-2'::'enum'
      IL_0052:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0057:  brfalse.s  IL_0078

      IL_0059:  ldarg.0
      IL_005a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'shortDigits@52-2'::'enum'
      IL_005f:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0064:  stloc.0
      IL_0065:  ldarg.0
      IL_0066:  ldc.i4.2
      IL_0067:  stfld      int32 Linq101Where01/'shortDigits@52-2'::pc
      .line 52,52 : 16,17 ''
      IL_006c:  ldarg.0
      IL_006d:  ldloc.0
      IL_006e:  stfld      string Linq101Where01/'shortDigits@52-2'::current
      IL_0073:  ldc.i4.1
      IL_0074:  ret

      .line 100001,100001 : 0,0 ''
      IL_0075:  nop
      IL_0076:  br.s       IL_004c

      IL_0078:  ldarg.0
      IL_0079:  ldc.i4.3
      IL_007a:  stfld      int32 Linq101Where01/'shortDigits@52-2'::pc
      .line 52,52 : 9,17 ''
      IL_007f:  ldarg.0
      IL_0080:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'shortDigits@52-2'::'enum'
      IL_0085:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_008a:  nop
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'shortDigits@52-2'::'enum'
      IL_0092:  ldarg.0
      IL_0093:  ldc.i4.3
      IL_0094:  stfld      int32 Linq101Where01/'shortDigits@52-2'::pc
      IL_0099:  ldarg.0
      IL_009a:  ldnull
      IL_009b:  stfld      string Linq101Where01/'shortDigits@52-2'::current
      IL_00a0:  ldc.i4.0
      IL_00a1:  ret
    } // end of method 'shortDigits@52-2'::GenerateNext

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
      IL_0001:  ldfld      int32 Linq101Where01/'shortDigits@52-2'::pc
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
        IL_001b:  ldfld      int32 Linq101Where01/'shortDigits@52-2'::pc
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
        IL_004f:  stfld      int32 Linq101Where01/'shortDigits@52-2'::pc
        IL_0054:  ldarg.0
        IL_0055:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'shortDigits@52-2'::'enum'
        IL_005a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [mscorlib]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_005f:  nop
        .line 100001,100001 : 0,0 ''
        IL_0060:  nop
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.3
        IL_0063:  stfld      int32 Linq101Where01/'shortDigits@52-2'::pc
        IL_0068:  ldarg.0
        IL_0069:  ldnull
        IL_006a:  stfld      string Linq101Where01/'shortDigits@52-2'::current
        IL_006f:  ldnull
        IL_0070:  stloc.1
        IL_0071:  leave.s    IL_007f

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0073:  castclass  [mscorlib]System.Exception
        IL_0078:  stloc.2
        .line 52,52 : 9,17 ''
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
    } // end of method 'shortDigits@52-2'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       56 (0x38)
      .maxstack  8
      .line 100001,100001 : 0,0 ''
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Where01/'shortDigits@52-2'::pc
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
    } // end of method 'shortDigits@52-2'::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Where01/'shortDigits@52-2'::current
      IL_0006:  ret
    } // end of method 'shortDigits@52-2'::get_LastGenerated

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
      IL_0003:  newobj     instance void Linq101Where01/'shortDigits@52-2'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
      IL_0008:  ret
    } // end of method 'shortDigits@52-2'::GetFreshEnumerator

  } // end of class 'shortDigits@52-2'

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Where01>'.$Linq101Where01::'numbers@9-11'
    IL_0005:  ret
  } // end of method Linq101Where01::get_numbers

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_lowNums() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Where01>'.$Linq101Where01::'lowNums@12-2'
    IL_0005:  ret
  } // end of method Linq101Where01::get_lowNums

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::'products@20-16'
    IL_0005:  ret
  } // end of method Linq101Where01::get_products

  .method public specialname static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 
          get_soldOutProducts() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::soldOutProducts@22
    IL_0005:  ret
  } // end of method Linq101Where01::get_soldOutProducts

  .method public specialname static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 
          get_expensiveInStockProducts() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::expensiveInStockProducts@30
    IL_0005:  ret
  } // end of method Linq101Where01::get_expensiveInStockProducts

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> 
          get_customers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Where01>'.$Linq101Where01::'customers@38-8'
    IL_0005:  ret
  } // end of method Linq101Where01::get_customers

  .method public specialname static class [Utils]Utils/Customer[] 
          get_waCustomers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [Utils]Utils/Customer[] '<StartupCode$Linq101Where01>'.$Linq101Where01::waCustomers@40
    IL_0005:  ret
  } // end of method Linq101Where01::get_waCustomers

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_digits() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Where01>'.$Linq101Where01::'digits@48-6'
    IL_0005:  ret
  } // end of method Linq101Where01::get_digits

  .method public specialname static class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          get_shortDigits() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<StartupCode$Linq101Where01>'.$Linq101Where01::shortDigits@49
    IL_0005:  ret
  } // end of method Linq101Where01::get_shortDigits

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Where01::get_numbers()
  } // end of property Linq101Where01::numbers
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          lowNums()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Where01::get_lowNums()
  } // end of property Linq101Where01::lowNums
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Where01::get_products()
  } // end of property Linq101Where01::products
  .property class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
          soldOutProducts()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Linq101Where01::get_soldOutProducts()
  } // end of property Linq101Where01::soldOutProducts
  .property class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
          expensiveInStockProducts()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Linq101Where01::get_expensiveInStockProducts()
  } // end of property Linq101Where01::expensiveInStockProducts
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer>
          customers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Where01::get_customers()
  } // end of property Linq101Where01::customers
  .property class [Utils]Utils/Customer[]
          waCustomers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [Utils]Utils/Customer[] Linq101Where01::get_waCustomers()
  } // end of property Linq101Where01::waCustomers
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          digits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Where01::get_digits()
  } // end of property Linq101Where01::digits
  .property class [mscorlib]System.Collections.Generic.IEnumerable`1<string>
          shortDigits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Collections.Generic.IEnumerable`1<string> Linq101Where01::get_shortDigits()
  } // end of property Linq101Where01::shortDigits
} // end of class Linq101Where01

.class private abstract auto ansi sealed '<StartupCode$Linq101Where01>'.$Linq101Where01
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'numbers@9-11'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 'lowNums@12-2'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 'products@20-16'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> soldOutProducts@22
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> expensiveInStockProducts@30
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> 'customers@38-8'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [Utils]Utils/Customer[] waCustomers@40
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 'digits@48-6'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.IEnumerable`1<string> shortDigits@49
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       543 (0x21f)
    .maxstack  13
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> lowNums,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products,
             [3] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> soldOutProducts,
             [4] class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> expensiveInStockProducts,
             [5] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers,
             [6] class [Utils]Utils/Customer[] waCustomers,
             [7] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits,
             [8] class [mscorlib]System.Collections.Generic.IEnumerable`1<string> shortDigits,
             [9] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_9,
             [10] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             [11] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11,
             [12] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             [13] class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_13)
    .line 9,9 : 1,47 ''
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
    IL_0043:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Where01>'.$Linq101Where01::'numbers@9-11'
    IL_0048:  stloc.0
    .line 12,17 : 1,20 ''
    IL_0049:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_004e:  stloc.s    V_9
    IL_0050:  ldloc.s    V_9
    IL_0052:  ldloc.s    V_9
    IL_0054:  ldloc.s    V_9
    IL_0056:  ldloc.s    V_9
    IL_0058:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Where01::get_numbers()
    IL_005d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0062:  ldloc.s    V_9
    IL_0064:  newobj     instance void Linq101Where01/'lowNums@14-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0069:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [mscorlib]System.Collections.IEnumerable,int32,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_006e:  newobj     instance void Linq101Where01/'lowNums@15-4'::.ctor()
    IL_0073:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<int32,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0078:  newobj     instance void Linq101Where01/'lowNums@16-5'::.ctor()
    IL_007d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<int32,class [mscorlib]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0082:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0087:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfSeq<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_008c:  dup
    IL_008d:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Where01>'.$Linq101Where01::'lowNums@12-2'
    IL_0092:  stloc.1
    .line 20,20 : 1,32 ''
    IL_0093:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_0098:  dup
    IL_0099:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::'products@20-16'
    IL_009e:  stloc.2
    IL_009f:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a4:  stloc.s    V_10
    IL_00a6:  ldloc.s    V_10
    IL_00a8:  ldloc.s    V_10
    IL_00aa:  ldloc.s    V_10
    IL_00ac:  ldloc.s    V_10
    IL_00ae:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Where01::get_products()
    IL_00b3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00b8:  ldloc.s    V_10
    IL_00ba:  newobj     instance void Linq101Where01/soldOutProducts@24::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00bf:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00c4:  newobj     instance void Linq101Where01/'soldOutProducts@25-1'::.ctor()
    IL_00c9:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_00ce:  newobj     instance void Linq101Where01/'soldOutProducts@26-2'::.ctor()
    IL_00d3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00d8:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_00dd:  dup
    IL_00de:  stsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::soldOutProducts@22
    IL_00e3:  stloc.3
    IL_00e4:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00e9:  stloc.s    V_11
    IL_00eb:  ldloc.s    V_11
    IL_00ed:  ldloc.s    V_11
    IL_00ef:  ldloc.s    V_11
    IL_00f1:  ldloc.s    V_11
    IL_00f3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Where01::get_products()
    IL_00f8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00fd:  ldloc.s    V_11
    IL_00ff:  newobj     instance void Linq101Where01/expensiveInStockProducts@32::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0104:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0109:  newobj     instance void Linq101Where01/'expensiveInStockProducts@33-1'::.ctor()
    IL_010e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0113:  newobj     instance void Linq101Where01/'expensiveInStockProducts@34-2'::.ctor()
    IL_0118:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_011d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0122:  dup
    IL_0123:  stsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::expensiveInStockProducts@30
    IL_0128:  stloc.s    expensiveInStockProducts
    .line 38,38 : 1,34 ''
    IL_012a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_012f:  dup
    IL_0130:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Where01>'.$Linq101Where01::'customers@38-8'
    IL_0135:  stloc.s    customers
    .line 40,45 : 1,21 ''
    IL_0137:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_013c:  stloc.s    V_12
    IL_013e:  ldloc.s    V_12
    IL_0140:  ldloc.s    V_12
    IL_0142:  ldloc.s    V_12
    IL_0144:  ldloc.s    V_12
    IL_0146:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Where01::get_customers()
    IL_014b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [mscorlib]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0150:  ldloc.s    V_12
    IL_0152:  newobj     instance void Linq101Where01/waCustomers@42::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0157:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Customer,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_015c:  newobj     instance void Linq101Where01/'waCustomers@43-1'::.ctor()
    IL_0161:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0166:  newobj     instance void Linq101Where01/'waCustomers@44-2'::.ctor()
    IL_016b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable,class [Utils]Utils/Customer>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0170:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,class [mscorlib]System.Collections.IEnumerable>::get_Source()
    IL_0175:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_017a:  dup
    IL_017b:  stsfld     class [Utils]Utils/Customer[] '<StartupCode$Linq101Where01>'.$Linq101Where01::waCustomers@40
    IL_0180:  stloc.s    waCustomers
    .line 48,48 : 1,96 ''
    IL_0182:  ldstr      "zero"
    IL_0187:  ldstr      "one"
    IL_018c:  ldstr      "two"
    IL_0191:  ldstr      "three"
    IL_0196:  ldstr      "four"
    IL_019b:  ldstr      "five"
    IL_01a0:  ldstr      "six"
    IL_01a5:  ldstr      "seven"
    IL_01aa:  ldstr      "eight"
    IL_01af:  ldstr      "nine"
    IL_01b4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_01b9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01be:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01c3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01c8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01cd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01d2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01d7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01dc:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01e1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01e6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01eb:  dup
    IL_01ec:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Where01>'.$Linq101Where01::'digits@48-6'
    IL_01f1:  stloc.s    digits
    .line 49,55 : 1,21 ''
    IL_01f3:  newobj     instance void Linq101Where01/shortDigits@55::.ctor()
    IL_01f8:  newobj     instance void Linq101Where01/'shortDigits@54-1'::.ctor()
    IL_01fd:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0202:  stloc.s    V_13
    IL_0204:  ldnull
    IL_0205:  ldc.i4.0
    IL_0206:  ldnull
    IL_0207:  newobj     instance void Linq101Where01/'shortDigits@52-2'::.ctor(class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
    IL_020c:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::MapIndexed<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>>,
                                                                                                                                                                                                                               class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0211:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Choose<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>,string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!!1>>,
                                                                                                                                                                                                                           class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0216:  dup
    IL_0217:  stsfld     class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<StartupCode$Linq101Where01>'.$Linq101Where01::shortDigits@49
    IL_021c:  stloc.s    shortDigits
    IL_021e:  ret
  } // end of method $Linq101Where01::main@

} // end of class '<StartupCode$Linq101Where01>'.$Linq101Where01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
