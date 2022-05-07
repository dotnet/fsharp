
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
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
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:1:0:0
}
.assembly Linq101Where01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Linq101Where01
{
  // Offset: 0x00000000 Length: 0x00000409
  // WARNING: managed resource file FSharpSignatureData.Linq101Where01 created
}
.mresource public FSharpOptimizationData.Linq101Where01
{
  // Offset: 0x00000410 Length: 0x0000012E
  // WARNING: managed resource file FSharpOptimizationData.Linq101Where01 created
}
.module Linq101Where01.exe
// MVID: {624FDC53-FF2F-6FF9-A745-038353DC4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000001D384040000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Linq101Where01
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@14'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/'Pipe #1 input at line 13@14'::builder@
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
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/'Pipe #1 input at line 13@14'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<int32,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #1 input at line 13@14'::Invoke

  } // end of class 'Pipe #1 input at line 13@14'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@15-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Linq101Where01/'Pipe #1 input at line 13@15-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 13@15-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.5
      IL_0002:  clt
      IL_0004:  ret
    } // end of method 'Pipe #1 input at line 13@15-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'Pipe #1 input at line 13@15-1'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'Pipe #1 input at line 13@15-1' Linq101Where01/'Pipe #1 input at line 13@15-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 13@15-1'::.cctor

  } // end of class 'Pipe #1 input at line 13@15-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 13@16-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class Linq101Where01/'Pipe #1 input at line 13@16-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #1 input at line 13@16-2'::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 n) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #1 input at line 13@16-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'Pipe #1 input at line 13@16-2'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'Pipe #1 input at line 13@16-2' Linq101Where01/'Pipe #1 input at line 13@16-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #1 input at line 13@16-2'::.cctor

  } // end of class 'Pipe #1 input at line 13@16-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit soldOutProducts@24
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
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
    .field static assembly initonly class Linq101Where01/'soldOutProducts@25-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method 'soldOutProducts@25-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'soldOutProducts@25-1'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'soldOutProducts@25-1' Linq101Where01/'soldOutProducts@25-1'::@_instance
      IL_000a:  ret
    } // end of method 'soldOutProducts@25-1'::.cctor

  } // end of class 'soldOutProducts@25-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'soldOutProducts@26-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Where01/'soldOutProducts@26-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'soldOutProducts@26-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'soldOutProducts@26-2'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'soldOutProducts@26-2' Linq101Where01/'soldOutProducts@26-2'::@_instance
      IL_000a:  ret
    } // end of method 'soldOutProducts@26-2'::.cctor

  } // end of class 'soldOutProducts@26-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit expensiveInStockProducts@32
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
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
    .field static assembly initonly class Linq101Where01/'expensiveInStockProducts@33-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'expensiveInStockProducts@33-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      // Code size       37 (0x25)
      .maxstack  10
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0006:  ldc.i4.0
      IL_0007:  ble.s      IL_0023

      IL_0009:  ldarg.1
      IL_000a:  callvirt   instance valuetype [System.Runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_000f:  ldc.i4     0x12c
      IL_0014:  ldc.i4.0
      IL_0015:  ldc.i4.0
      IL_0016:  ldc.i4.0
      IL_0017:  ldc.i4.2
      IL_0018:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
      IL_001d:  call       bool [netstandard]System.Decimal::op_GreaterThan(valuetype [netstandard]System.Decimal,
                                                                            valuetype [netstandard]System.Decimal)
      IL_0022:  ret

      IL_0023:  ldc.i4.0
      IL_0024:  ret
    } // end of method 'expensiveInStockProducts@33-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'expensiveInStockProducts@33-1'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'expensiveInStockProducts@33-1' Linq101Where01/'expensiveInStockProducts@33-1'::@_instance
      IL_000a:  ret
    } // end of method 'expensiveInStockProducts@33-1'::.cctor

  } // end of class 'expensiveInStockProducts@33-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'expensiveInStockProducts@34-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class Linq101Where01/'expensiveInStockProducts@34-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'expensiveInStockProducts@34-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'expensiveInStockProducts@34-2'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'expensiveInStockProducts@34-2' Linq101Where01/'expensiveInStockProducts@34-2'::@_instance
      IL_000a:  ret
    } // end of method 'expensiveInStockProducts@34-2'::.cctor

  } // end of class 'expensiveInStockProducts@34-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 41@42'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/'Pipe #2 input at line 41@42'::builder@
      IL_000d:  ret
    } // end of method 'Pipe #2 input at line 41@42'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object> 
            Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder Linq101Where01/'Pipe #2 input at line 41@42'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Customer,object>(!!0)
      IL_0010:  ret
    } // end of method 'Pipe #2 input at line 41@42'::Invoke

  } // end of class 'Pipe #2 input at line 41@42'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 41@43-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>
  {
    .field static assembly initonly class Linq101Where01/'Pipe #2 input at line 41@43-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 41@43-1'::.ctor

    .method public strict virtual instance bool 
            Invoke(class [Utils]Utils/Customer c) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance string [Utils]Utils/Customer::get_Region()
      IL_0006:  ldstr      "WA"
      IL_000b:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0010:  ret
    } // end of method 'Pipe #2 input at line 41@43-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'Pipe #2 input at line 41@43-1'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'Pipe #2 input at line 41@43-1' Linq101Where01/'Pipe #2 input at line 41@43-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 41@43-1'::.cctor

  } // end of class 'Pipe #2 input at line 41@43-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 41@44-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [Utils]Utils/Customer>
  {
    .field static assembly initonly class Linq101Where01/'Pipe #2 input at line 41@44-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [Utils]Utils/Customer>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #2 input at line 41@44-2'::.ctor

    .method public strict virtual instance class [Utils]Utils/Customer 
            Invoke(class [Utils]Utils/Customer c) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method 'Pipe #2 input at line 41@44-2'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'Pipe #2 input at line 41@44-2'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'Pipe #2 input at line 41@44-2' Linq101Where01/'Pipe #2 input at line 41@44-2'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #2 input at line 41@44-2'::.cctor

  } // end of class 'Pipe #2 input at line 41@44-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 50@51-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>>
  {
    .field static assembly initonly class Linq101Where01/'Pipe #3 input at line 50@51-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 50@51-1'::.ctor

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> 
            Invoke(string _arg1) cil managed
    {
      // Code size       11 (0xb)
      .maxstack  5
      .locals init (string V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<string>(!!0)
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 50@51-1'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'Pipe #3 input at line 50@51-1'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'Pipe #3 input at line 50@51-1' Linq101Where01/'Pipe #3 input at line 50@51-1'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 input at line 50@51-1'::.cctor

  } // end of class 'Pipe #3 input at line 50@51-1'

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #3 input at line 50@52'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'Pipe #3 input at line 50@52'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string Linq101Where01/'Pipe #3 input at line 50@52'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } // end of method 'Pipe #3 input at line 50@52'::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      // Code size       155 (0x9b)
      .maxstack  7
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_0071

      IL_001e:  nop
      IL_001f:  br.s       IL_0064

      IL_0021:  nop
      IL_0022:  br.s       IL_0092

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldsfld     class Linq101Where01/'Pipe #3 input at line 50@51-1' Linq101Where01/'Pipe #3 input at line 50@51-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> Linq101Where01::get_digits()
      IL_0030:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<string,class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>,string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                  class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003a:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'Pipe #3 input at line 50@52'::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
      IL_0046:  br.s       IL_0064

      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'Pipe #3 input at line 50@52'::'enum'
      IL_004e:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0053:  stloc.0
      IL_0054:  ldarg.0
      IL_0055:  ldc.i4.2
      IL_0056:  stfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
      IL_005b:  ldarg.0
      IL_005c:  ldloc.0
      IL_005d:  stfld      string Linq101Where01/'Pipe #3 input at line 50@52'::current
      IL_0062:  ldc.i4.1
      IL_0063:  ret

      IL_0064:  ldarg.0
      IL_0065:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'Pipe #3 input at line 50@52'::'enum'
      IL_006a:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_006f:  brtrue.s   IL_0048

      IL_0071:  ldarg.0
      IL_0072:  ldc.i4.3
      IL_0073:  stfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
      IL_0078:  ldarg.0
      IL_0079:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'Pipe #3 input at line 50@52'::'enum'
      IL_007e:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_0083:  nop
      IL_0084:  ldarg.0
      IL_0085:  ldnull
      IL_0086:  stfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'Pipe #3 input at line 50@52'::'enum'
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.3
      IL_008d:  stfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
      IL_0092:  ldarg.0
      IL_0093:  ldnull
      IL_0094:  stfld      string Linq101Where01/'Pipe #3 input at line 50@52'::current
      IL_0099:  ldc.i4.0
      IL_009a:  ret
    } // end of method 'Pipe #3 input at line 50@52'::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       124 (0x7c)
      .maxstack  6
      .locals init (class [System.Runtime]System.Exception V_0,
               class [System.Runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> Linq101Where01/'Pipe #3 input at line 50@52'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string Linq101Where01/'Pipe #3 input at line 50@52'::current
        IL_0064:  leave.s    IL_0070

      }  // end .try
      catch [System.Runtime]System.Object 
      {
        IL_0066:  castclass  [System.Runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  // end handler
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } // end of method 'Pipe #3 input at line 50@52'::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       48 (0x30)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Linq101Where01/'Pipe #3 input at line 50@52'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method 'Pipe #3 input at line 50@52'::get_CheckClose

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string Linq101Where01/'Pipe #3 input at line 50@52'::current
      IL_0006:  ret
    } // end of method 'Pipe #3 input at line 50@52'::get_LastGenerated

    .method public strict virtual instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void Linq101Where01/'Pipe #3 input at line 50@52'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                             int32,
                                                                                             string)
      IL_0008:  ret
    } // end of method 'Pipe #3 input at line 50@52'::GetFreshEnumerator

  } // end of class 'Pipe #3 input at line 50@52'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 stage #1 at line 54@54'
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<int32,string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>
  {
    .field static assembly initonly class Linq101Where01/'Pipe #3 stage #1 at line 54@54' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<int32,string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>::.ctor()
      IL_0006:  ret
    } // end of method 'Pipe #3 stage #1 at line 54@54'::.ctor

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> 
            Invoke(int32 i,
                   string d) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.2
      IL_0002:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
      IL_0007:  ldarg.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  ldarg.2
      IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>::Some(!0)
      IL_0010:  ret

      IL_0011:  ldnull
      IL_0012:  ret
    } // end of method 'Pipe #3 stage #1 at line 54@54'::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/'Pipe #3 stage #1 at line 54@54'::.ctor()
      IL_0005:  stsfld     class Linq101Where01/'Pipe #3 stage #1 at line 54@54' Linq101Where01/'Pipe #3 stage #1 at line 54@54'::@_instance
      IL_000a:  ret
    } // end of method 'Pipe #3 stage #1 at line 54@54'::.cctor

  } // end of class 'Pipe #3 stage #1 at line 54@54'

  .class auto ansi serializable sealed nested assembly beforefieldinit shortDigits@55
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>
  {
    .field static assembly initonly class Linq101Where01/shortDigits@55 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>(!!0)
      IL_0008:  ret
    } // end of method shortDigits@55::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void Linq101Where01/shortDigits@55::.ctor()
      IL_0005:  stsfld     class Linq101Where01/shortDigits@55 Linq101Where01/shortDigits@55::@_instance
      IL_000a:  ret
    } // end of method shortDigits@55::.cctor

  } // end of class shortDigits@55

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Where01>'.$Linq101Where01::numbers@9
    IL_0005:  ret
  } // end of method Linq101Where01::get_numbers

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_lowNums() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Where01>'.$Linq101Where01::lowNums@12
    IL_0005:  ret
  } // end of method Linq101Where01::get_lowNums

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::products@20
    IL_0005:  ret
  } // end of method Linq101Where01::get_products

  .method public specialname static class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 
          get_soldOutProducts() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::soldOutProducts@22
    IL_0005:  ret
  } // end of method Linq101Where01::get_soldOutProducts

  .method public specialname static class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> 
          get_expensiveInStockProducts() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::expensiveInStockProducts@30
    IL_0005:  ret
  } // end of method Linq101Where01::get_expensiveInStockProducts

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> 
          get_customers() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Where01>'.$Linq101Where01::customers@38
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
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Where01>'.$Linq101Where01::digits@48
    IL_0005:  ret
  } // end of method Linq101Where01::get_digits

  .method public specialname static class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> 
          get_shortDigits() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> '<StartupCode$Linq101Where01>'.$Linq101Where01::shortDigits@49
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
  .property class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
          soldOutProducts()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Linq101Where01::get_soldOutProducts()
  } // end of property Linq101Where01::soldOutProducts
  .property class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
          expensiveInStockProducts()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Linq101Where01::get_expensiveInStockProducts()
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
  .property class [System.Runtime]System.Collections.Generic.IEnumerable`1<string>
          shortDigits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> Linq101Where01::get_shortDigits()
  } // end of property Linq101Where01::shortDigits
} // end of class Linq101Where01

.class private abstract auto ansi sealed '<StartupCode$Linq101Where01>'.$Linq101Where01
       extends [System.Runtime]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@9
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> lowNums@12
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@20
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> soldOutProducts@22
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> expensiveInStockProducts@30
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers@38
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [Utils]Utils/Customer[] waCustomers@40
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits@48
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> shortDigits@49
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       559 (0x22f)
    .maxstack  13
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> V_2,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_3,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_4,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> V_5,
             class [Utils]Utils/Customer[] V_6,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_7,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> V_8,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_9,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Customer> V_13,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_14,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> V_15,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_16,
             class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>> V_17)
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
    IL_0043:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Where01>'.$Linq101Where01::numbers@9
    IL_0048:  stloc.0
    IL_0049:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_004e:  stloc.s    V_10
    IL_0050:  ldloc.s    V_10
    IL_0052:  ldloc.s    V_10
    IL_0054:  ldloc.s    V_10
    IL_0056:  ldloc.s    V_10
    IL_0058:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Linq101Where01::get_numbers()
    IL_005d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0062:  ldloc.s    V_10
    IL_0064:  newobj     instance void Linq101Where01/'Pipe #1 input at line 13@14'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0069:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [System.Runtime]System.Collections.IEnumerable,int32,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_006e:  ldsfld     class Linq101Where01/'Pipe #1 input at line 13@15-1' Linq101Where01/'Pipe #1 input at line 13@15-1'::@_instance
    IL_0073:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<int32,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0078:  ldsfld     class Linq101Where01/'Pipe #1 input at line 13@16-2' Linq101Where01/'Pipe #1 input at line 13@16-2'::@_instance
    IL_007d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<int32,class [System.Runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0082:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0087:  stloc.s    V_9
    IL_0089:  ldloc.s    V_9
    IL_008b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::OfSeq<int32>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0090:  dup
    IL_0091:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$Linq101Where01>'.$Linq101Where01::lowNums@12
    IL_0096:  stloc.1
    IL_0097:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_009c:  dup
    IL_009d:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::products@20
    IL_00a2:  stloc.2
    IL_00a3:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00a8:  stloc.s    V_11
    IL_00aa:  ldloc.s    V_11
    IL_00ac:  ldloc.s    V_11
    IL_00ae:  ldloc.s    V_11
    IL_00b0:  ldloc.s    V_11
    IL_00b2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Where01::get_products()
    IL_00b7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00bc:  ldloc.s    V_11
    IL_00be:  newobj     instance void Linq101Where01/soldOutProducts@24::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00c3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00c8:  ldsfld     class Linq101Where01/'soldOutProducts@25-1' Linq101Where01/'soldOutProducts@25-1'::@_instance
    IL_00cd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_00d2:  ldsfld     class Linq101Where01/'soldOutProducts@26-2' Linq101Where01/'soldOutProducts@26-2'::@_instance
    IL_00d7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00dc:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_00e1:  dup
    IL_00e2:  stsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::soldOutProducts@22
    IL_00e7:  stloc.3
    IL_00e8:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00ed:  stloc.s    V_12
    IL_00ef:  ldloc.s    V_12
    IL_00f1:  ldloc.s    V_12
    IL_00f3:  ldloc.s    V_12
    IL_00f5:  ldloc.s    V_12
    IL_00f7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> Linq101Where01::get_products()
    IL_00fc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0101:  ldloc.s    V_12
    IL_0103:  newobj     instance void Linq101Where01/expensiveInStockProducts@32::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0108:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_010d:  ldsfld     class Linq101Where01/'expensiveInStockProducts@33-1' Linq101Where01/'expensiveInStockProducts@33-1'::@_instance
    IL_0112:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0117:  ldsfld     class Linq101Where01/'expensiveInStockProducts@34-2' Linq101Where01/'expensiveInStockProducts@34-2'::@_instance
    IL_011c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0121:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0126:  dup
    IL_0127:  stsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> '<StartupCode$Linq101Where01>'.$Linq101Where01::expensiveInStockProducts@30
    IL_012c:  stloc.s    V_4
    IL_012e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_0133:  dup
    IL_0134:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> '<StartupCode$Linq101Where01>'.$Linq101Where01::customers@38
    IL_0139:  stloc.s    V_5
    IL_013b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0140:  stloc.s    V_14
    IL_0142:  ldloc.s    V_14
    IL_0144:  ldloc.s    V_14
    IL_0146:  ldloc.s    V_14
    IL_0148:  ldloc.s    V_14
    IL_014a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> Linq101Where01::get_customers()
    IL_014f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [System.Runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0154:  ldloc.s    V_14
    IL_0156:  newobj     instance void Linq101Where01/'Pipe #2 input at line 41@42'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_015b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Customer,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0160:  ldsfld     class Linq101Where01/'Pipe #2 input at line 41@43-1' Linq101Where01/'Pipe #2 input at line 41@43-1'::@_instance
    IL_0165:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_016a:  ldsfld     class Linq101Where01/'Pipe #2 input at line 41@44-2' Linq101Where01/'Pipe #2 input at line 41@44-2'::@_instance
    IL_016f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable,class [Utils]Utils/Customer>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0174:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,class [System.Runtime]System.Collections.IEnumerable>::get_Source()
    IL_0179:  stloc.s    V_13
    IL_017b:  ldloc.s    V_13
    IL_017d:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [Utils]Utils/Customer>(class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0182:  dup
    IL_0183:  stsfld     class [Utils]Utils/Customer[] '<StartupCode$Linq101Where01>'.$Linq101Where01::waCustomers@40
    IL_0188:  stloc.s    V_6
    IL_018a:  ldstr      "zero"
    IL_018f:  ldstr      "one"
    IL_0194:  ldstr      "two"
    IL_0199:  ldstr      "three"
    IL_019e:  ldstr      "four"
    IL_01a3:  ldstr      "five"
    IL_01a8:  ldstr      "six"
    IL_01ad:  ldstr      "seven"
    IL_01b2:  ldstr      "eight"
    IL_01b7:  ldstr      "nine"
    IL_01bc:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_01c1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01c6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01cb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01d0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01d5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01da:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01df:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01e4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01e9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01ee:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_01f3:  dup
    IL_01f4:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$Linq101Where01>'.$Linq101Where01::digits@48
    IL_01f9:  stloc.s    V_7
    IL_01fb:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0200:  stloc.s    V_16
    IL_0202:  ldnull
    IL_0203:  ldc.i4.0
    IL_0204:  ldnull
    IL_0205:  newobj     instance void Linq101Where01/'Pipe #3 input at line 50@52'::.ctor(class [System.Runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                           int32,
                                                                                           string)
    IL_020a:  stloc.s    V_15
    IL_020c:  ldsfld     class Linq101Where01/'Pipe #3 stage #1 at line 54@54' Linq101Where01/'Pipe #3 stage #1 at line 54@54'::@_instance
    IL_0211:  ldloc.s    V_15
    IL_0213:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::MapIndexed<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>>,
                                                                                                                                                                                                                                     class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0218:  stloc.s    V_17
    IL_021a:  ldsfld     class Linq101Where01/shortDigits@55 Linq101Where01/shortDigits@55::@_instance
    IL_021f:  ldloc.s    V_17
    IL_0221:  call       class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Choose<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>,string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!!1>>,
                                                                                                                                                                                                                                 class [System.Runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0226:  dup
    IL_0227:  stsfld     class [System.Runtime]System.Collections.Generic.IEnumerable`1<string> '<StartupCode$Linq101Where01>'.$Linq101Where01::shortDigits@49
    IL_022c:  stloc.s    V_8
    IL_022e:  ret
  } // end of method $Linq101Where01::main@

} // end of class '<StartupCode$Linq101Where01>'.$Linq101Where01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\QueryExpressionStepping\Linq101Where01_fs\Linq101Where01.res
