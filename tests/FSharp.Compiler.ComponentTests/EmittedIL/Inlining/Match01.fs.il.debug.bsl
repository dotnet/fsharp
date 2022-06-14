
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
.assembly Match01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Match01
{
  // Offset: 0x00000000 Length: 0x00000718
  // WARNING: managed resource file FSharpSignatureData.Match01 created
}
.mresource public FSharpOptimizationData.Match01
{
  // Offset: 0x00000720 Length: 0x000003C3
  // WARNING: managed resource file FSharpOptimizationData.Match01 created
}
.module Match01.exe
// MVID: {628F4C90-D9D6-6143-A745-0383904C8F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000019CC0BB0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Match01
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto autochar serializable nested public beforefieldinit Test1
         extends [System.Runtime]System.Object
         implements class [System.Runtime]System.IEquatable`1<class Match01/Test1>,
                    [System.Runtime]System.Collections.IStructuralEquatable,
                    class [System.Runtime]System.IComparable`1<class Match01/Test1>,
                    [System.Runtime]System.IComparable,
                    [System.Runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                         61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [System.Runtime]System.Object
    {
      .field public static literal int32 X11 = int32(0x00000000)
      .field public static literal int32 X12 = int32(0x00000001)
      .field public static literal int32 X13 = int32(0x00000002)
      .field public static literal int32 X14 = int32(0x00000003)
    } // end of class Tags

    .class auto ansi serializable nested public beforefieldinit specialname X11
           extends Match01/Test1
    {
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [System.Runtime]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   // .. Match01+Test1
                                                                                                                                        2B 58 31 31 40 44 65 62 75 67 54 79 70 65 50 72   // +X11@DebugTypePr
                                                                                                                                        6F 78 79 00 00 )                                  // oxy..
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .field assembly initonly int32 item
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.0
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X11::item
        IL_000e:  ret
      } // end of method X11::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X11::item
        IL_0006:  ret
      } // end of method X11::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X11::get_Item()
      } // end of property X11::Item
    } // end of class X11

    .class auto ansi serializable nested public beforefieldinit specialname X12
           extends Match01/Test1
    {
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [System.Runtime]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   // .. Match01+Test1
                                                                                                                                        2B 58 31 32 40 44 65 62 75 67 54 79 70 65 50 72   // +X12@DebugTypePr
                                                                                                                                        6F 78 79 00 00 )                                  // oxy..
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .field assembly initonly int32 item
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.1
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X12::item
        IL_000e:  ret
      } // end of method X12::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X12::item
        IL_0006:  ret
      } // end of method X12::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X12::get_Item()
      } // end of property X12::Item
    } // end of class X12

    .class auto ansi serializable nested public beforefieldinit specialname X13
           extends Match01/Test1
    {
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [System.Runtime]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   // .. Match01+Test1
                                                                                                                                        2B 58 31 33 40 44 65 62 75 67 54 79 70 65 50 72   // +X13@DebugTypePr
                                                                                                                                        6F 78 79 00 00 )                                  // oxy..
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .field assembly initonly int32 item
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.2
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X13::item
        IL_000e:  ret
      } // end of method X13::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X13::item
        IL_0006:  ret
      } // end of method X13::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X13::get_Item()
      } // end of property X13::Item
    } // end of class X13

    .class auto ansi serializable nested public beforefieldinit specialname X14
           extends Match01/Test1
    {
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [System.Runtime]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   // .. Match01+Test1
                                                                                                                                        2B 58 31 34 40 44 65 62 75 67 54 79 70 65 50 72   // +X14@DebugTypePr
                                                                                                                                        6F 78 79 00 00 )                                  // oxy..
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .field assembly initonly int32 item
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.3
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X14::item
        IL_000e:  ret
      } // end of method X14::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X14::item
        IL_0006:  ret
      } // end of method X14::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X14::get_Item()
      } // end of property X14::Item
    } // end of class X14

    .class auto ansi nested assembly beforefieldinit specialname X11@DebugTypeProxy
           extends [System.Runtime]System.Object
    {
      .field assembly class Match01/Test1/X11 _obj
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class Match01/Test1/X11 obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X11 Match01/Test1/X11@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method X11@DebugTypeProxy::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X11 Match01/Test1/X11@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X11::item
        IL_000b:  ret
      } // end of method X11@DebugTypeProxy::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X11@DebugTypeProxy::get_Item()
      } // end of property X11@DebugTypeProxy::Item
    } // end of class X11@DebugTypeProxy

    .class auto ansi nested assembly beforefieldinit specialname X12@DebugTypeProxy
           extends [System.Runtime]System.Object
    {
      .field assembly class Match01/Test1/X12 _obj
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class Match01/Test1/X12 obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X12 Match01/Test1/X12@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method X12@DebugTypeProxy::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X12 Match01/Test1/X12@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X12::item
        IL_000b:  ret
      } // end of method X12@DebugTypeProxy::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X12@DebugTypeProxy::get_Item()
      } // end of property X12@DebugTypeProxy::Item
    } // end of class X12@DebugTypeProxy

    .class auto ansi nested assembly beforefieldinit specialname X13@DebugTypeProxy
           extends [System.Runtime]System.Object
    {
      .field assembly class Match01/Test1/X13 _obj
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class Match01/Test1/X13 obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X13 Match01/Test1/X13@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method X13@DebugTypeProxy::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X13 Match01/Test1/X13@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X13::item
        IL_000b:  ret
      } // end of method X13@DebugTypeProxy::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X13@DebugTypeProxy::get_Item()
      } // end of property X13@DebugTypeProxy::Item
    } // end of class X13@DebugTypeProxy

    .class auto ansi nested assembly beforefieldinit specialname X14@DebugTypeProxy
           extends [System.Runtime]System.Object
    {
      .field assembly class Match01/Test1/X14 _obj
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class Match01/Test1/X14 obj) cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X14 Match01/Test1/X14@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method X14@DebugTypeProxy::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X14 Match01/Test1/X14@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X14::item
        IL_000b:  ret
      } // end of method X14@DebugTypeProxy::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X14@DebugTypeProxy::get_Item()
      } // end of property X14@DebugTypeProxy::Item
    } // end of class X14@DebugTypeProxy

    .field assembly initonly int32 _tag
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 _tag) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 Match01/Test1::_tag
      IL_000d:  ret
    } // end of method Test1::.ctor

    .method public static class Match01/Test1 
            NewX11(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X11::.ctor(int32)
      IL_0006:  ret
    } // end of method Test1::NewX11

    .method public hidebysig instance bool 
            get_IsX11() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Test1::get_IsX11

    .method public static class Match01/Test1 
            NewX12(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X12::.ctor(int32)
      IL_0006:  ret
    } // end of method Test1::NewX12

    .method public hidebysig instance bool 
            get_IsX12() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Test1::get_IsX12

    .method public static class Match01/Test1 
            NewX13(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 02 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X13::.ctor(int32)
      IL_0006:  ret
    } // end of method Test1::NewX13

    .method public hidebysig instance bool 
            get_IsX13() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.2
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Test1::get_IsX13

    .method public static class Match01/Test1 
            NewX14(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 03 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X14::.ctor(int32)
      IL_0006:  ret
    } // end of method Test1::NewX14

    .method public hidebysig instance bool 
            get_IsX14() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.3
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Test1::get_IsX14

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Match01/Test1::_tag
      IL_0006:  ret
    } // end of method Test1::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Test1::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Match01/Test1>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Test1::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class Match01/Test1 obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0011

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_000f

      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  ldnull
      IL_0009:  call       int32 Match01::CompareTo$cont@4(class Match01/Test1,
                                                           class Match01/Test1,
                                                           class [FSharp.Core]Microsoft.FSharp.Core.Unit)
      IL_000e:  ret

      IL_000f:  ldc.i4.1
      IL_0010:  ret

      IL_0011:  ldarg.1
      IL_0012:  brfalse.s  IL_0016

      IL_0014:  ldc.i4.m1
      IL_0015:  ret

      IL_0016:  ldc.i4.0
      IL_0017:  ret
    } // end of method Test1::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  Match01/Test1
      IL_0007:  callvirt   instance int32 Match01/Test1::CompareTo(class Match01/Test1)
      IL_000c:  ret
    } // end of method Test1::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [System.Runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       41 (0x29)
      .maxstack  5
      .locals init (class Match01/Test1 V_0)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  Match01/Test1
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_001d

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  Match01/Test1
      IL_0010:  brfalse.s  IL_001b

      IL_0012:  ldarg.0
      IL_0013:  ldloc.0
      IL_0014:  ldnull
      IL_0015:  call       int32 Match01::'CompareTo$cont@4-1'(class Match01/Test1,
                                                               class Match01/Test1,
                                                               class [FSharp.Core]Microsoft.FSharp.Core.Unit)
      IL_001a:  ret

      IL_001b:  ldc.i4.1
      IL_001c:  ret

      IL_001d:  ldarg.1
      IL_001e:  unbox.any  Match01/Test1
      IL_0023:  brfalse.s  IL_0027

      IL_0025:  ldc.i4.m1
      IL_0026:  ret

      IL_0027:  ldc.i4.0
      IL_0028:  ret
    } // end of method Test1::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       167 (0xa7)
      .maxstack  7
      .locals init (int32 V_0,
               class Match01/Test1/X11 V_1,
               class Match01/Test1/X12 V_2,
               class Match01/Test1/X13 V_3,
               class Match01/Test1/X14 V_4)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_00a5

      IL_0006:  ldc.i4.0
      IL_0007:  stloc.0
      IL_0008:  ldarg.0
      IL_0009:  call       instance int32 Match01/Test1::get_Tag()
      IL_000e:  switch     ( 
                            IL_0023,
                            IL_0043,
                            IL_0063,
                            IL_0083)
      IL_0023:  ldarg.0
      IL_0024:  castclass  Match01/Test1/X11
      IL_0029:  stloc.1
      IL_002a:  ldc.i4.0
      IL_002b:  stloc.0
      IL_002c:  ldc.i4     0x9e3779b9
      IL_0031:  ldloc.1
      IL_0032:  ldfld      int32 Match01/Test1/X11::item
      IL_0037:  ldloc.0
      IL_0038:  ldc.i4.6
      IL_0039:  shl
      IL_003a:  ldloc.0
      IL_003b:  ldc.i4.2
      IL_003c:  shr
      IL_003d:  add
      IL_003e:  add
      IL_003f:  add
      IL_0040:  stloc.0
      IL_0041:  ldloc.0
      IL_0042:  ret

      IL_0043:  ldarg.0
      IL_0044:  castclass  Match01/Test1/X12
      IL_0049:  stloc.2
      IL_004a:  ldc.i4.1
      IL_004b:  stloc.0
      IL_004c:  ldc.i4     0x9e3779b9
      IL_0051:  ldloc.2
      IL_0052:  ldfld      int32 Match01/Test1/X12::item
      IL_0057:  ldloc.0
      IL_0058:  ldc.i4.6
      IL_0059:  shl
      IL_005a:  ldloc.0
      IL_005b:  ldc.i4.2
      IL_005c:  shr
      IL_005d:  add
      IL_005e:  add
      IL_005f:  add
      IL_0060:  stloc.0
      IL_0061:  ldloc.0
      IL_0062:  ret

      IL_0063:  ldarg.0
      IL_0064:  castclass  Match01/Test1/X13
      IL_0069:  stloc.3
      IL_006a:  ldc.i4.2
      IL_006b:  stloc.0
      IL_006c:  ldc.i4     0x9e3779b9
      IL_0071:  ldloc.3
      IL_0072:  ldfld      int32 Match01/Test1/X13::item
      IL_0077:  ldloc.0
      IL_0078:  ldc.i4.6
      IL_0079:  shl
      IL_007a:  ldloc.0
      IL_007b:  ldc.i4.2
      IL_007c:  shr
      IL_007d:  add
      IL_007e:  add
      IL_007f:  add
      IL_0080:  stloc.0
      IL_0081:  ldloc.0
      IL_0082:  ret

      IL_0083:  ldarg.0
      IL_0084:  castclass  Match01/Test1/X14
      IL_0089:  stloc.s    V_4
      IL_008b:  ldc.i4.3
      IL_008c:  stloc.0
      IL_008d:  ldc.i4     0x9e3779b9
      IL_0092:  ldloc.s    V_4
      IL_0094:  ldfld      int32 Match01/Test1/X14::item
      IL_0099:  ldloc.0
      IL_009a:  ldc.i4.6
      IL_009b:  shl
      IL_009c:  ldloc.0
      IL_009d:  ldc.i4.2
      IL_009e:  shr
      IL_009f:  add
      IL_00a0:  add
      IL_00a1:  add
      IL_00a2:  stloc.0
      IL_00a3:  ldloc.0
      IL_00a4:  ret

      IL_00a5:  ldc.i4.0
      IL_00a6:  ret
    } // end of method Test1::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 Match01/Test1::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method Test1::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       209 (0xd1)
      .maxstack  4
      .locals init (class Match01/Test1 V_0,
               int32 V_1,
               int32 V_2,
               class Match01/Test1/X11 V_3,
               class Match01/Test1/X11 V_4,
               class Match01/Test1/X12 V_5,
               class Match01/Test1/X12 V_6,
               class Match01/Test1/X13 V_7,
               class Match01/Test1/X13 V_8,
               class Match01/Test1/X14 V_9,
               class Match01/Test1/X14 V_10)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_00c9

      IL_0006:  ldarg.1
      IL_0007:  isinst     Match01/Test1
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  brfalse    IL_00c7

      IL_0013:  ldarg.0
      IL_0014:  ldfld      int32 Match01/Test1::_tag
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldfld      int32 Match01/Test1::_tag
      IL_0020:  stloc.2
      IL_0021:  ldloc.1
      IL_0022:  ldloc.2
      IL_0023:  bne.un     IL_00c5

      IL_0028:  ldarg.0
      IL_0029:  call       instance int32 Match01/Test1::get_Tag()
      IL_002e:  switch     ( 
                            IL_0043,
                            IL_0062,
                            IL_0083,
                            IL_00a4)
      IL_0043:  ldarg.0
      IL_0044:  castclass  Match01/Test1/X11
      IL_0049:  stloc.3
      IL_004a:  ldloc.0
      IL_004b:  castclass  Match01/Test1/X11
      IL_0050:  stloc.s    V_4
      IL_0052:  ldloc.3
      IL_0053:  ldfld      int32 Match01/Test1/X11::item
      IL_0058:  ldloc.s    V_4
      IL_005a:  ldfld      int32 Match01/Test1/X11::item
      IL_005f:  ceq
      IL_0061:  ret

      IL_0062:  ldarg.0
      IL_0063:  castclass  Match01/Test1/X12
      IL_0068:  stloc.s    V_5
      IL_006a:  ldloc.0
      IL_006b:  castclass  Match01/Test1/X12
      IL_0070:  stloc.s    V_6
      IL_0072:  ldloc.s    V_5
      IL_0074:  ldfld      int32 Match01/Test1/X12::item
      IL_0079:  ldloc.s    V_6
      IL_007b:  ldfld      int32 Match01/Test1/X12::item
      IL_0080:  ceq
      IL_0082:  ret

      IL_0083:  ldarg.0
      IL_0084:  castclass  Match01/Test1/X13
      IL_0089:  stloc.s    V_7
      IL_008b:  ldloc.0
      IL_008c:  castclass  Match01/Test1/X13
      IL_0091:  stloc.s    V_8
      IL_0093:  ldloc.s    V_7
      IL_0095:  ldfld      int32 Match01/Test1/X13::item
      IL_009a:  ldloc.s    V_8
      IL_009c:  ldfld      int32 Match01/Test1/X13::item
      IL_00a1:  ceq
      IL_00a3:  ret

      IL_00a4:  ldarg.0
      IL_00a5:  castclass  Match01/Test1/X14
      IL_00aa:  stloc.s    V_9
      IL_00ac:  ldloc.0
      IL_00ad:  castclass  Match01/Test1/X14
      IL_00b2:  stloc.s    V_10
      IL_00b4:  ldloc.s    V_9
      IL_00b6:  ldfld      int32 Match01/Test1/X14::item
      IL_00bb:  ldloc.s    V_10
      IL_00bd:  ldfld      int32 Match01/Test1/X14::item
      IL_00c2:  ceq
      IL_00c4:  ret

      IL_00c5:  ldc.i4.0
      IL_00c6:  ret

      IL_00c7:  ldc.i4.0
      IL_00c8:  ret

      IL_00c9:  ldarg.1
      IL_00ca:  ldnull
      IL_00cb:  cgt.un
      IL_00cd:  ldc.i4.0
      IL_00ce:  ceq
      IL_00d0:  ret
    } // end of method Test1::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class Match01/Test1 obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       200 (0xc8)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               class Match01/Test1/X11 V_2,
               class Match01/Test1/X11 V_3,
               class Match01/Test1/X12 V_4,
               class Match01/Test1/X12 V_5,
               class Match01/Test1/X13 V_6,
               class Match01/Test1/X13 V_7,
               class Match01/Test1/X14 V_8,
               class Match01/Test1/X14 V_9)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_00c0

      IL_0006:  ldarg.1
      IL_0007:  brfalse    IL_00be

      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 Match01/Test1::_tag
      IL_0012:  stloc.0
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 Match01/Test1::_tag
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldloc.1
      IL_001c:  bne.un     IL_00bc

      IL_0021:  ldarg.0
      IL_0022:  call       instance int32 Match01/Test1::get_Tag()
      IL_0027:  switch     ( 
                            IL_003c,
                            IL_0059,
                            IL_007a,
                            IL_009b)
      IL_003c:  ldarg.0
      IL_003d:  castclass  Match01/Test1/X11
      IL_0042:  stloc.2
      IL_0043:  ldarg.1
      IL_0044:  castclass  Match01/Test1/X11
      IL_0049:  stloc.3
      IL_004a:  ldloc.2
      IL_004b:  ldfld      int32 Match01/Test1/X11::item
      IL_0050:  ldloc.3
      IL_0051:  ldfld      int32 Match01/Test1/X11::item
      IL_0056:  ceq
      IL_0058:  ret

      IL_0059:  ldarg.0
      IL_005a:  castclass  Match01/Test1/X12
      IL_005f:  stloc.s    V_4
      IL_0061:  ldarg.1
      IL_0062:  castclass  Match01/Test1/X12
      IL_0067:  stloc.s    V_5
      IL_0069:  ldloc.s    V_4
      IL_006b:  ldfld      int32 Match01/Test1/X12::item
      IL_0070:  ldloc.s    V_5
      IL_0072:  ldfld      int32 Match01/Test1/X12::item
      IL_0077:  ceq
      IL_0079:  ret

      IL_007a:  ldarg.0
      IL_007b:  castclass  Match01/Test1/X13
      IL_0080:  stloc.s    V_6
      IL_0082:  ldarg.1
      IL_0083:  castclass  Match01/Test1/X13
      IL_0088:  stloc.s    V_7
      IL_008a:  ldloc.s    V_6
      IL_008c:  ldfld      int32 Match01/Test1/X13::item
      IL_0091:  ldloc.s    V_7
      IL_0093:  ldfld      int32 Match01/Test1/X13::item
      IL_0098:  ceq
      IL_009a:  ret

      IL_009b:  ldarg.0
      IL_009c:  castclass  Match01/Test1/X14
      IL_00a1:  stloc.s    V_8
      IL_00a3:  ldarg.1
      IL_00a4:  castclass  Match01/Test1/X14
      IL_00a9:  stloc.s    V_9
      IL_00ab:  ldloc.s    V_8
      IL_00ad:  ldfld      int32 Match01/Test1/X14::item
      IL_00b2:  ldloc.s    V_9
      IL_00b4:  ldfld      int32 Match01/Test1/X14::item
      IL_00b9:  ceq
      IL_00bb:  ret

      IL_00bc:  ldc.i4.0
      IL_00bd:  ret

      IL_00be:  ldc.i4.0
      IL_00bf:  ret

      IL_00c0:  ldarg.1
      IL_00c1:  ldnull
      IL_00c2:  cgt.un
      IL_00c4:  ldc.i4.0
      IL_00c5:  ceq
      IL_00c7:  ret
    } // end of method Test1::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class Match01/Test1 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     Match01/Test1
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool Match01/Test1::Equals(class Match01/Test1)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method Test1::Equals

    .property instance int32 Tag()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 Match01/Test1::get_Tag()
    } // end of property Test1::Tag
    .property instance bool IsX11()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX11()
    } // end of property Test1::IsX11
    .property instance bool IsX12()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX12()
    } // end of property Test1::IsX12
    .property instance bool IsX13()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX13()
    } // end of property Test1::IsX13
    .property instance bool IsX14()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX14()
    } // end of property Test1::IsX14
  } // end of class Test1

  .method assembly static int32  CompareTo$cont@4(class Match01/Test1 this,
                                                  class Match01/Test1 obj,
                                                  class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       268 (0x10c)
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             class Match01/Test1/X11 V_2,
             class Match01/Test1/X11 V_3,
             class [System.Runtime]System.Collections.IComparer V_4,
             int32 V_5,
             int32 V_6,
             class Match01/Test1/X12 V_7,
             class Match01/Test1/X12 V_8,
             class Match01/Test1/X13 V_9,
             class Match01/Test1/X13 V_10,
             class Match01/Test1/X14 V_11,
             class Match01/Test1/X14 V_12)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Match01/Test1::_tag
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldfld      int32 Match01/Test1::_tag
    IL_000d:  stloc.1
    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  bne.un     IL_0108

    IL_0015:  ldarg.0
    IL_0016:  call       instance int32 Match01/Test1::get_Tag()
    IL_001b:  switch     ( 
                          IL_0030,
                          IL_0063,
                          IL_009a,
                          IL_00d1)
    IL_0030:  ldarg.0
    IL_0031:  castclass  Match01/Test1/X11
    IL_0036:  stloc.2
    IL_0037:  ldarg.1
    IL_0038:  castclass  Match01/Test1/X11
    IL_003d:  stloc.3
    IL_003e:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0043:  stloc.s    V_4
    IL_0045:  ldloc.2
    IL_0046:  ldfld      int32 Match01/Test1/X11::item
    IL_004b:  stloc.s    V_5
    IL_004d:  ldloc.3
    IL_004e:  ldfld      int32 Match01/Test1/X11::item
    IL_0053:  stloc.s    V_6
    IL_0055:  ldloc.s    V_5
    IL_0057:  ldloc.s    V_6
    IL_0059:  cgt
    IL_005b:  ldloc.s    V_5
    IL_005d:  ldloc.s    V_6
    IL_005f:  clt
    IL_0061:  sub
    IL_0062:  ret

    IL_0063:  ldarg.0
    IL_0064:  castclass  Match01/Test1/X12
    IL_0069:  stloc.s    V_7
    IL_006b:  ldarg.1
    IL_006c:  castclass  Match01/Test1/X12
    IL_0071:  stloc.s    V_8
    IL_0073:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0078:  stloc.s    V_4
    IL_007a:  ldloc.s    V_7
    IL_007c:  ldfld      int32 Match01/Test1/X12::item
    IL_0081:  stloc.s    V_5
    IL_0083:  ldloc.s    V_8
    IL_0085:  ldfld      int32 Match01/Test1/X12::item
    IL_008a:  stloc.s    V_6
    IL_008c:  ldloc.s    V_5
    IL_008e:  ldloc.s    V_6
    IL_0090:  cgt
    IL_0092:  ldloc.s    V_5
    IL_0094:  ldloc.s    V_6
    IL_0096:  clt
    IL_0098:  sub
    IL_0099:  ret

    IL_009a:  ldarg.0
    IL_009b:  castclass  Match01/Test1/X13
    IL_00a0:  stloc.s    V_9
    IL_00a2:  ldarg.1
    IL_00a3:  castclass  Match01/Test1/X13
    IL_00a8:  stloc.s    V_10
    IL_00aa:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_00af:  stloc.s    V_4
    IL_00b1:  ldloc.s    V_9
    IL_00b3:  ldfld      int32 Match01/Test1/X13::item
    IL_00b8:  stloc.s    V_5
    IL_00ba:  ldloc.s    V_10
    IL_00bc:  ldfld      int32 Match01/Test1/X13::item
    IL_00c1:  stloc.s    V_6
    IL_00c3:  ldloc.s    V_5
    IL_00c5:  ldloc.s    V_6
    IL_00c7:  cgt
    IL_00c9:  ldloc.s    V_5
    IL_00cb:  ldloc.s    V_6
    IL_00cd:  clt
    IL_00cf:  sub
    IL_00d0:  ret

    IL_00d1:  ldarg.0
    IL_00d2:  castclass  Match01/Test1/X14
    IL_00d7:  stloc.s    V_11
    IL_00d9:  ldarg.1
    IL_00da:  castclass  Match01/Test1/X14
    IL_00df:  stloc.s    V_12
    IL_00e1:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_00e6:  stloc.s    V_4
    IL_00e8:  ldloc.s    V_11
    IL_00ea:  ldfld      int32 Match01/Test1/X14::item
    IL_00ef:  stloc.s    V_5
    IL_00f1:  ldloc.s    V_12
    IL_00f3:  ldfld      int32 Match01/Test1/X14::item
    IL_00f8:  stloc.s    V_6
    IL_00fa:  ldloc.s    V_5
    IL_00fc:  ldloc.s    V_6
    IL_00fe:  cgt
    IL_0100:  ldloc.s    V_5
    IL_0102:  ldloc.s    V_6
    IL_0104:  clt
    IL_0106:  sub
    IL_0107:  ret

    IL_0108:  ldloc.0
    IL_0109:  ldloc.1
    IL_010a:  sub
    IL_010b:  ret
  } // end of method Match01::CompareTo$cont@4

  .method assembly static int32  'CompareTo$cont@4-1'(class Match01/Test1 this,
                                                      class Match01/Test1 objTemp,
                                                      class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       240 (0xf0)
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             class Match01/Test1/X11 V_2,
             class Match01/Test1/X11 V_3,
             int32 V_4,
             int32 V_5,
             class Match01/Test1/X12 V_6,
             class Match01/Test1/X12 V_7,
             class Match01/Test1/X13 V_8,
             class Match01/Test1/X13 V_9,
             class Match01/Test1/X14 V_10,
             class Match01/Test1/X14 V_11)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Match01/Test1::_tag
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldfld      int32 Match01/Test1::_tag
    IL_000d:  stloc.1
    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  bne.un     IL_00ec

    IL_0015:  ldarg.0
    IL_0016:  call       instance int32 Match01/Test1::get_Tag()
    IL_001b:  switch     ( 
                          IL_0030,
                          IL_005c,
                          IL_008c,
                          IL_00bc)
    IL_0030:  ldarg.0
    IL_0031:  castclass  Match01/Test1/X11
    IL_0036:  stloc.2
    IL_0037:  ldarg.1
    IL_0038:  castclass  Match01/Test1/X11
    IL_003d:  stloc.3
    IL_003e:  ldloc.2
    IL_003f:  ldfld      int32 Match01/Test1/X11::item
    IL_0044:  stloc.s    V_4
    IL_0046:  ldloc.3
    IL_0047:  ldfld      int32 Match01/Test1/X11::item
    IL_004c:  stloc.s    V_5
    IL_004e:  ldloc.s    V_4
    IL_0050:  ldloc.s    V_5
    IL_0052:  cgt
    IL_0054:  ldloc.s    V_4
    IL_0056:  ldloc.s    V_5
    IL_0058:  clt
    IL_005a:  sub
    IL_005b:  ret

    IL_005c:  ldarg.0
    IL_005d:  castclass  Match01/Test1/X12
    IL_0062:  stloc.s    V_6
    IL_0064:  ldarg.1
    IL_0065:  castclass  Match01/Test1/X12
    IL_006a:  stloc.s    V_7
    IL_006c:  ldloc.s    V_6
    IL_006e:  ldfld      int32 Match01/Test1/X12::item
    IL_0073:  stloc.s    V_4
    IL_0075:  ldloc.s    V_7
    IL_0077:  ldfld      int32 Match01/Test1/X12::item
    IL_007c:  stloc.s    V_5
    IL_007e:  ldloc.s    V_4
    IL_0080:  ldloc.s    V_5
    IL_0082:  cgt
    IL_0084:  ldloc.s    V_4
    IL_0086:  ldloc.s    V_5
    IL_0088:  clt
    IL_008a:  sub
    IL_008b:  ret

    IL_008c:  ldarg.0
    IL_008d:  castclass  Match01/Test1/X13
    IL_0092:  stloc.s    V_8
    IL_0094:  ldarg.1
    IL_0095:  castclass  Match01/Test1/X13
    IL_009a:  stloc.s    V_9
    IL_009c:  ldloc.s    V_8
    IL_009e:  ldfld      int32 Match01/Test1/X13::item
    IL_00a3:  stloc.s    V_4
    IL_00a5:  ldloc.s    V_9
    IL_00a7:  ldfld      int32 Match01/Test1/X13::item
    IL_00ac:  stloc.s    V_5
    IL_00ae:  ldloc.s    V_4
    IL_00b0:  ldloc.s    V_5
    IL_00b2:  cgt
    IL_00b4:  ldloc.s    V_4
    IL_00b6:  ldloc.s    V_5
    IL_00b8:  clt
    IL_00ba:  sub
    IL_00bb:  ret

    IL_00bc:  ldarg.0
    IL_00bd:  castclass  Match01/Test1/X14
    IL_00c2:  stloc.s    V_10
    IL_00c4:  ldarg.1
    IL_00c5:  castclass  Match01/Test1/X14
    IL_00ca:  stloc.s    V_11
    IL_00cc:  ldloc.s    V_10
    IL_00ce:  ldfld      int32 Match01/Test1/X14::item
    IL_00d3:  stloc.s    V_4
    IL_00d5:  ldloc.s    V_11
    IL_00d7:  ldfld      int32 Match01/Test1/X14::item
    IL_00dc:  stloc.s    V_5
    IL_00de:  ldloc.s    V_4
    IL_00e0:  ldloc.s    V_5
    IL_00e2:  cgt
    IL_00e4:  ldloc.s    V_4
    IL_00e6:  ldloc.s    V_5
    IL_00e8:  clt
    IL_00ea:  sub
    IL_00eb:  ret

    IL_00ec:  ldloc.0
    IL_00ed:  ldloc.1
    IL_00ee:  sub
    IL_00ef:  ret
  } // end of method Match01::'CompareTo$cont@4-1'

  .method public static int32  select1(class Match01/Test1 x) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 Match01/Test1::get_Tag()
    IL_0007:  switch     ( 
                          IL_001c,
                          IL_0028,
                          IL_002a,
                          IL_002c)
    IL_001c:  ldarg.0
    IL_001d:  castclass  Match01/Test1/X11
    IL_0022:  ldfld      int32 Match01/Test1/X11::item
    IL_0027:  ret

    IL_0028:  ldc.i4.2
    IL_0029:  ret

    IL_002a:  ldc.i4.3
    IL_002b:  ret

    IL_002c:  ldc.i4.4
    IL_002d:  ret
  } // end of method Match01::select1

  .method public static int32  fm(class Match01/Test1 y) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       int32 Match01::select1(class Match01/Test1)
    IL_0006:  ret
  } // end of method Match01::fm

} // end of class Match01

.class private abstract auto ansi sealed '<StartupCode$Match01>'.$Match01
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Match01::main@

} // end of class '<StartupCode$Match01>'.$Match01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\dev\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\Inlining\Match01_fs\Match01.res
