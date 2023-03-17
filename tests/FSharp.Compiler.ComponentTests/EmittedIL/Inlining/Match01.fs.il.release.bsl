




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.assembly
{
  
  
}
.mresource public FSharpOptimizationData.assembly
{
  
  
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
  .class abstract auto autochar serializable nested public beforefieldinit Test1
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/Test1>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/Test1>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                         61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [runtime]System.Object
    {
      .field public static literal int32 X11 = int32(0x00000000)
      .field public static literal int32 X12 = int32(0x00000001)
      .field public static literal int32 X13 = int32(0x00000002)
      .field public static literal int32 X14 = int32(0x00000003)
    } 

    .class auto ansi serializable nested public beforefieldinit specialname X11
           extends assembly/Test1
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [runtime]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   
                                                                                                                                        2B 58 31 31 40 44 65 62 75 67 54 79 70 65 50 72   
                                                                                                                                        6F 78 79 00 00 )                                  
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   
      .field assembly initonly int32 item
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.0
        IL_0002:  call       instance void assembly/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 assembly/Test1/X11::item
        IL_000e:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/Test1/X11::item
        IL_0006:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/Test1/X11::get_Item()
      } 
    } 

    .class auto ansi serializable nested public beforefieldinit specialname X12
           extends assembly/Test1
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [runtime]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   
                                                                                                                                        2B 58 31 32 40 44 65 62 75 67 54 79 70 65 50 72   
                                                                                                                                        6F 78 79 00 00 )                                  
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   
      .field assembly initonly int32 item
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.1
        IL_0002:  call       instance void assembly/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 assembly/Test1/X12::item
        IL_000e:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/Test1/X12::item
        IL_0006:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/Test1/X12::get_Item()
      } 
    } 

    .class auto ansi serializable nested public beforefieldinit specialname X13
           extends assembly/Test1
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [runtime]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   
                                                                                                                                        2B 58 31 33 40 44 65 62 75 67 54 79 70 65 50 72   
                                                                                                                                        6F 78 79 00 00 )                                  
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   
      .field assembly initonly int32 item
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.2
        IL_0002:  call       instance void assembly/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 assembly/Test1/X13::item
        IL_000e:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/Test1/X13::item
        IL_0006:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/Test1/X13::get_Item()
      } 
    } 

    .class auto ansi serializable nested public beforefieldinit specialname X14
           extends assembly/Test1
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [runtime]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   
                                                                                                                                        2B 58 31 34 40 44 65 62 75 67 54 79 70 65 50 72   
                                                                                                                                        6F 78 79 00 00 )                                  
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   
      .field assembly initonly int32 item
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.3
        IL_0002:  call       instance void assembly/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 assembly/Test1/X14::item
        IL_000e:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/Test1/X14::item
        IL_0006:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/Test1/X14::get_Item()
      } 
    } 

    .class auto ansi nested assembly beforefieldinit specialname X11@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class assembly/Test1/X11 _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class assembly/Test1/X11 obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/Test1/X11 assembly/Test1/X11@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class assembly/Test1/X11 assembly/Test1/X11@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 assembly/Test1/X11::item
        IL_000b:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/Test1/X11@DebugTypeProxy::get_Item()
      } 
    } 

    .class auto ansi nested assembly beforefieldinit specialname X12@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class assembly/Test1/X12 _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class assembly/Test1/X12 obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/Test1/X12 assembly/Test1/X12@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class assembly/Test1/X12 assembly/Test1/X12@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 assembly/Test1/X12::item
        IL_000b:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/Test1/X12@DebugTypeProxy::get_Item()
      } 
    } 

    .class auto ansi nested assembly beforefieldinit specialname X13@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class assembly/Test1/X13 _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class assembly/Test1/X13 obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/Test1/X13 assembly/Test1/X13@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class assembly/Test1/X13 assembly/Test1/X13@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 assembly/Test1/X13::item
        IL_000b:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/Test1/X13@DebugTypeProxy::get_Item()
      } 
    } 

    .class auto ansi nested assembly beforefieldinit specialname X14@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class assembly/Test1/X14 _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class assembly/Test1/X14 obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/Test1/X14 assembly/Test1/X14@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class assembly/Test1/X14 assembly/Test1/X14@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 assembly/Test1/X14::item
        IL_000b:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/Test1/X14@DebugTypeProxy::get_Item()
      } 
    } 

    .field assembly initonly int32 _tag
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 _tag) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/Test1::_tag
      IL_000d:  ret
    } 

    .method public static class assembly/Test1 
            NewX11(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void assembly/Test1/X11::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool 
            get_IsX11() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 assembly/Test1::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public static class assembly/Test1 
            NewX12(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void assembly/Test1/X12::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool 
            get_IsX12() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 assembly/Test1::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public static class assembly/Test1 
            NewX13(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 02 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void assembly/Test1/X13::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool 
            get_IsX13() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 assembly/Test1::get_Tag()
      IL_0006:  ldc.i4.2
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public static class assembly/Test1 
            NewX14(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 03 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void assembly/Test1/X14::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool 
            get_IsX14() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 assembly/Test1::get_Tag()
      IL_0006:  ldc.i4.3
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/Test1::_tag
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Test1,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Test1,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Test1,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Test1,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/Test1>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Test1,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Test1,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(class assembly/Test1 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0011

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_000f

      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  ldnull
      IL_0009:  call       int32 assembly::CompareTo$cont@4(class assembly/Test1,
                                                           class assembly/Test1,
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
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/Test1
      IL_0007:  callvirt   instance int32 assembly/Test1::CompareTo(class assembly/Test1)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  6
      .locals init (class assembly/Test1 V_0)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/Test1
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0014

      IL_000a:  ldarg.0
      IL_000b:  ldarg.1
      IL_000c:  ldloc.0
      IL_000d:  ldnull
      IL_000e:  call       int32 assembly::'CompareTo$cont@4-1'(class assembly/Test1,
                                                               object,
                                                               class assembly/Test1,
                                                               class [FSharp.Core]Microsoft.FSharp.Core.Unit)
      IL_0013:  ret

      IL_0014:  ldarg.1
      IL_0015:  unbox.any  assembly/Test1
      IL_001a:  brfalse.s  IL_001e

      IL_001c:  ldc.i4.m1
      IL_001d:  ret

      IL_001e:  ldc.i4.0
      IL_001f:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               class assembly/Test1/X11 V_1,
               class assembly/Test1/X12 V_2,
               class assembly/Test1/X13 V_3,
               class assembly/Test1/X14 V_4)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_00a5

      IL_0006:  ldc.i4.0
      IL_0007:  stloc.0
      IL_0008:  ldarg.0
      IL_0009:  call       instance int32 assembly/Test1::get_Tag()
      IL_000e:  switch     ( 
                            IL_0023,
                            IL_0043,
                            IL_0063,
                            IL_0083)
      IL_0023:  ldarg.0
      IL_0024:  castclass  assembly/Test1/X11
      IL_0029:  stloc.1
      IL_002a:  ldc.i4.0
      IL_002b:  stloc.0
      IL_002c:  ldc.i4     0x9e3779b9
      IL_0031:  ldloc.1
      IL_0032:  ldfld      int32 assembly/Test1/X11::item
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
      IL_0044:  castclass  assembly/Test1/X12
      IL_0049:  stloc.2
      IL_004a:  ldc.i4.1
      IL_004b:  stloc.0
      IL_004c:  ldc.i4     0x9e3779b9
      IL_0051:  ldloc.2
      IL_0052:  ldfld      int32 assembly/Test1/X12::item
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
      IL_0064:  castclass  assembly/Test1/X13
      IL_0069:  stloc.3
      IL_006a:  ldc.i4.2
      IL_006b:  stloc.0
      IL_006c:  ldc.i4     0x9e3779b9
      IL_0071:  ldloc.3
      IL_0072:  ldfld      int32 assembly/Test1/X13::item
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
      IL_0084:  castclass  assembly/Test1/X14
      IL_0089:  stloc.s    V_4
      IL_008b:  ldc.i4.3
      IL_008c:  stloc.0
      IL_008d:  ldc.i4     0x9e3779b9
      IL_0092:  ldloc.s    V_4
      IL_0094:  ldfld      int32 assembly/Test1/X14::item
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
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 assembly/Test1::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/Test1 V_0,
               int32 V_1,
               int32 V_2,
               class assembly/Test1/X11 V_3,
               class assembly/Test1/X11 V_4,
               class assembly/Test1/X12 V_5,
               class assembly/Test1/X12 V_6,
               class assembly/Test1/X13 V_7,
               class assembly/Test1/X13 V_8,
               class assembly/Test1/X14 V_9,
               class assembly/Test1/X14 V_10)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_00c9

      IL_0006:  ldarg.1
      IL_0007:  isinst     assembly/Test1
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  brfalse    IL_00c7

      IL_0013:  ldarg.0
      IL_0014:  ldfld      int32 assembly/Test1::_tag
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldfld      int32 assembly/Test1::_tag
      IL_0020:  stloc.2
      IL_0021:  ldloc.1
      IL_0022:  ldloc.2
      IL_0023:  bne.un     IL_00c5

      IL_0028:  ldarg.0
      IL_0029:  call       instance int32 assembly/Test1::get_Tag()
      IL_002e:  switch     ( 
                            IL_0043,
                            IL_0062,
                            IL_0083,
                            IL_00a4)
      IL_0043:  ldarg.0
      IL_0044:  castclass  assembly/Test1/X11
      IL_0049:  stloc.3
      IL_004a:  ldloc.0
      IL_004b:  castclass  assembly/Test1/X11
      IL_0050:  stloc.s    V_4
      IL_0052:  ldloc.3
      IL_0053:  ldfld      int32 assembly/Test1/X11::item
      IL_0058:  ldloc.s    V_4
      IL_005a:  ldfld      int32 assembly/Test1/X11::item
      IL_005f:  ceq
      IL_0061:  ret

      IL_0062:  ldarg.0
      IL_0063:  castclass  assembly/Test1/X12
      IL_0068:  stloc.s    V_5
      IL_006a:  ldloc.0
      IL_006b:  castclass  assembly/Test1/X12
      IL_0070:  stloc.s    V_6
      IL_0072:  ldloc.s    V_5
      IL_0074:  ldfld      int32 assembly/Test1/X12::item
      IL_0079:  ldloc.s    V_6
      IL_007b:  ldfld      int32 assembly/Test1/X12::item
      IL_0080:  ceq
      IL_0082:  ret

      IL_0083:  ldarg.0
      IL_0084:  castclass  assembly/Test1/X13
      IL_0089:  stloc.s    V_7
      IL_008b:  ldloc.0
      IL_008c:  castclass  assembly/Test1/X13
      IL_0091:  stloc.s    V_8
      IL_0093:  ldloc.s    V_7
      IL_0095:  ldfld      int32 assembly/Test1/X13::item
      IL_009a:  ldloc.s    V_8
      IL_009c:  ldfld      int32 assembly/Test1/X13::item
      IL_00a1:  ceq
      IL_00a3:  ret

      IL_00a4:  ldarg.0
      IL_00a5:  castclass  assembly/Test1/X14
      IL_00aa:  stloc.s    V_9
      IL_00ac:  ldloc.0
      IL_00ad:  castclass  assembly/Test1/X14
      IL_00b2:  stloc.s    V_10
      IL_00b4:  ldloc.s    V_9
      IL_00b6:  ldfld      int32 assembly/Test1/X14::item
      IL_00bb:  ldloc.s    V_10
      IL_00bd:  ldfld      int32 assembly/Test1/X14::item
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
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(class assembly/Test1 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               class assembly/Test1/X11 V_2,
               class assembly/Test1/X11 V_3,
               class assembly/Test1/X12 V_4,
               class assembly/Test1/X12 V_5,
               class assembly/Test1/X13 V_6,
               class assembly/Test1/X13 V_7,
               class assembly/Test1/X14 V_8,
               class assembly/Test1/X14 V_9)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_00c0

      IL_0006:  ldarg.1
      IL_0007:  brfalse    IL_00be

      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 assembly/Test1::_tag
      IL_0012:  stloc.0
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 assembly/Test1::_tag
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldloc.1
      IL_001c:  bne.un     IL_00bc

      IL_0021:  ldarg.0
      IL_0022:  call       instance int32 assembly/Test1::get_Tag()
      IL_0027:  switch     ( 
                            IL_003c,
                            IL_0059,
                            IL_007a,
                            IL_009b)
      IL_003c:  ldarg.0
      IL_003d:  castclass  assembly/Test1/X11
      IL_0042:  stloc.2
      IL_0043:  ldarg.1
      IL_0044:  castclass  assembly/Test1/X11
      IL_0049:  stloc.3
      IL_004a:  ldloc.2
      IL_004b:  ldfld      int32 assembly/Test1/X11::item
      IL_0050:  ldloc.3
      IL_0051:  ldfld      int32 assembly/Test1/X11::item
      IL_0056:  ceq
      IL_0058:  ret

      IL_0059:  ldarg.0
      IL_005a:  castclass  assembly/Test1/X12
      IL_005f:  stloc.s    V_4
      IL_0061:  ldarg.1
      IL_0062:  castclass  assembly/Test1/X12
      IL_0067:  stloc.s    V_5
      IL_0069:  ldloc.s    V_4
      IL_006b:  ldfld      int32 assembly/Test1/X12::item
      IL_0070:  ldloc.s    V_5
      IL_0072:  ldfld      int32 assembly/Test1/X12::item
      IL_0077:  ceq
      IL_0079:  ret

      IL_007a:  ldarg.0
      IL_007b:  castclass  assembly/Test1/X13
      IL_0080:  stloc.s    V_6
      IL_0082:  ldarg.1
      IL_0083:  castclass  assembly/Test1/X13
      IL_0088:  stloc.s    V_7
      IL_008a:  ldloc.s    V_6
      IL_008c:  ldfld      int32 assembly/Test1/X13::item
      IL_0091:  ldloc.s    V_7
      IL_0093:  ldfld      int32 assembly/Test1/X13::item
      IL_0098:  ceq
      IL_009a:  ret

      IL_009b:  ldarg.0
      IL_009c:  castclass  assembly/Test1/X14
      IL_00a1:  stloc.s    V_8
      IL_00a3:  ldarg.1
      IL_00a4:  castclass  assembly/Test1/X14
      IL_00a9:  stloc.s    V_9
      IL_00ab:  ldloc.s    V_8
      IL_00ad:  ldfld      int32 assembly/Test1/X14::item
      IL_00b2:  ldloc.s    V_9
      IL_00b4:  ldfld      int32 assembly/Test1/X14::item
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
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/Test1 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/Test1
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/Test1::Equals(class assembly/Test1)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 Tag()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/Test1::get_Tag()
    } 
    .property instance bool IsX11()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool assembly/Test1::get_IsX11()
    } 
    .property instance bool IsX12()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool assembly/Test1::get_IsX12()
    } 
    .property instance bool IsX13()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool assembly/Test1::get_IsX13()
    } 
    .property instance bool IsX14()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool assembly/Test1::get_IsX14()
    } 
  } 

  .method assembly static int32  CompareTo$cont@4(class assembly/Test1 this,
                                                  class assembly/Test1 obj,
                                                  class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             class assembly/Test1/X11 V_2,
             class assembly/Test1/X11 V_3,
             class [runtime]System.Collections.IComparer V_4,
             int32 V_5,
             int32 V_6,
             class assembly/Test1/X12 V_7,
             class assembly/Test1/X12 V_8,
             class assembly/Test1/X13 V_9,
             class assembly/Test1/X13 V_10,
             class assembly/Test1/X14 V_11,
             class assembly/Test1/X14 V_12)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 assembly/Test1::_tag
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldfld      int32 assembly/Test1::_tag
    IL_000d:  stloc.1
    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  bne.un     IL_0108

    IL_0015:  ldarg.0
    IL_0016:  call       instance int32 assembly/Test1::get_Tag()
    IL_001b:  switch     ( 
                          IL_0030,
                          IL_0063,
                          IL_009a,
                          IL_00d1)
    IL_0030:  ldarg.0
    IL_0031:  castclass  assembly/Test1/X11
    IL_0036:  stloc.2
    IL_0037:  ldarg.1
    IL_0038:  castclass  assembly/Test1/X11
    IL_003d:  stloc.3
    IL_003e:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0043:  stloc.s    V_4
    IL_0045:  ldloc.2
    IL_0046:  ldfld      int32 assembly/Test1/X11::item
    IL_004b:  stloc.s    V_5
    IL_004d:  ldloc.3
    IL_004e:  ldfld      int32 assembly/Test1/X11::item
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
    IL_0064:  castclass  assembly/Test1/X12
    IL_0069:  stloc.s    V_7
    IL_006b:  ldarg.1
    IL_006c:  castclass  assembly/Test1/X12
    IL_0071:  stloc.s    V_8
    IL_0073:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0078:  stloc.s    V_4
    IL_007a:  ldloc.s    V_7
    IL_007c:  ldfld      int32 assembly/Test1/X12::item
    IL_0081:  stloc.s    V_5
    IL_0083:  ldloc.s    V_8
    IL_0085:  ldfld      int32 assembly/Test1/X12::item
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
    IL_009b:  castclass  assembly/Test1/X13
    IL_00a0:  stloc.s    V_9
    IL_00a2:  ldarg.1
    IL_00a3:  castclass  assembly/Test1/X13
    IL_00a8:  stloc.s    V_10
    IL_00aa:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_00af:  stloc.s    V_4
    IL_00b1:  ldloc.s    V_9
    IL_00b3:  ldfld      int32 assembly/Test1/X13::item
    IL_00b8:  stloc.s    V_5
    IL_00ba:  ldloc.s    V_10
    IL_00bc:  ldfld      int32 assembly/Test1/X13::item
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
    IL_00d2:  castclass  assembly/Test1/X14
    IL_00d7:  stloc.s    V_11
    IL_00d9:  ldarg.1
    IL_00da:  castclass  assembly/Test1/X14
    IL_00df:  stloc.s    V_12
    IL_00e1:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_00e6:  stloc.s    V_4
    IL_00e8:  ldloc.s    V_11
    IL_00ea:  ldfld      int32 assembly/Test1/X14::item
    IL_00ef:  stloc.s    V_5
    IL_00f1:  ldloc.s    V_12
    IL_00f3:  ldfld      int32 assembly/Test1/X14::item
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
  } 

  .method assembly static int32  'CompareTo$cont@4-1'(class assembly/Test1 this,
                                                      object obj,
                                                      class assembly/Test1 objTemp,
                                                      class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             class assembly/Test1/X11 V_2,
             class assembly/Test1/X11 V_3,
             int32 V_4,
             int32 V_5,
             class assembly/Test1/X12 V_6,
             class assembly/Test1/X12 V_7,
             class assembly/Test1/X13 V_8,
             class assembly/Test1/X13 V_9,
             class assembly/Test1/X14 V_10,
             class assembly/Test1/X14 V_11)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  assembly/Test1
    IL_0006:  brfalse    IL_00fb

    IL_000b:  ldarg.0
    IL_000c:  ldfld      int32 assembly/Test1::_tag
    IL_0011:  stloc.0
    IL_0012:  ldarg.2
    IL_0013:  ldfld      int32 assembly/Test1::_tag
    IL_0018:  stloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldloc.1
    IL_001b:  bne.un     IL_00f7

    IL_0020:  ldarg.0
    IL_0021:  call       instance int32 assembly/Test1::get_Tag()
    IL_0026:  switch     ( 
                          IL_003b,
                          IL_0067,
                          IL_0097,
                          IL_00c7)
    IL_003b:  ldarg.0
    IL_003c:  castclass  assembly/Test1/X11
    IL_0041:  stloc.2
    IL_0042:  ldarg.2
    IL_0043:  castclass  assembly/Test1/X11
    IL_0048:  stloc.3
    IL_0049:  ldloc.2
    IL_004a:  ldfld      int32 assembly/Test1/X11::item
    IL_004f:  stloc.s    V_4
    IL_0051:  ldloc.3
    IL_0052:  ldfld      int32 assembly/Test1/X11::item
    IL_0057:  stloc.s    V_5
    IL_0059:  ldloc.s    V_4
    IL_005b:  ldloc.s    V_5
    IL_005d:  cgt
    IL_005f:  ldloc.s    V_4
    IL_0061:  ldloc.s    V_5
    IL_0063:  clt
    IL_0065:  sub
    IL_0066:  ret

    IL_0067:  ldarg.0
    IL_0068:  castclass  assembly/Test1/X12
    IL_006d:  stloc.s    V_6
    IL_006f:  ldarg.2
    IL_0070:  castclass  assembly/Test1/X12
    IL_0075:  stloc.s    V_7
    IL_0077:  ldloc.s    V_6
    IL_0079:  ldfld      int32 assembly/Test1/X12::item
    IL_007e:  stloc.s    V_4
    IL_0080:  ldloc.s    V_7
    IL_0082:  ldfld      int32 assembly/Test1/X12::item
    IL_0087:  stloc.s    V_5
    IL_0089:  ldloc.s    V_4
    IL_008b:  ldloc.s    V_5
    IL_008d:  cgt
    IL_008f:  ldloc.s    V_4
    IL_0091:  ldloc.s    V_5
    IL_0093:  clt
    IL_0095:  sub
    IL_0096:  ret

    IL_0097:  ldarg.0
    IL_0098:  castclass  assembly/Test1/X13
    IL_009d:  stloc.s    V_8
    IL_009f:  ldarg.2
    IL_00a0:  castclass  assembly/Test1/X13
    IL_00a5:  stloc.s    V_9
    IL_00a7:  ldloc.s    V_8
    IL_00a9:  ldfld      int32 assembly/Test1/X13::item
    IL_00ae:  stloc.s    V_4
    IL_00b0:  ldloc.s    V_9
    IL_00b2:  ldfld      int32 assembly/Test1/X13::item
    IL_00b7:  stloc.s    V_5
    IL_00b9:  ldloc.s    V_4
    IL_00bb:  ldloc.s    V_5
    IL_00bd:  cgt
    IL_00bf:  ldloc.s    V_4
    IL_00c1:  ldloc.s    V_5
    IL_00c3:  clt
    IL_00c5:  sub
    IL_00c6:  ret

    IL_00c7:  ldarg.0
    IL_00c8:  castclass  assembly/Test1/X14
    IL_00cd:  stloc.s    V_10
    IL_00cf:  ldarg.2
    IL_00d0:  castclass  assembly/Test1/X14
    IL_00d5:  stloc.s    V_11
    IL_00d7:  ldloc.s    V_10
    IL_00d9:  ldfld      int32 assembly/Test1/X14::item
    IL_00de:  stloc.s    V_4
    IL_00e0:  ldloc.s    V_11
    IL_00e2:  ldfld      int32 assembly/Test1/X14::item
    IL_00e7:  stloc.s    V_5
    IL_00e9:  ldloc.s    V_4
    IL_00eb:  ldloc.s    V_5
    IL_00ed:  cgt
    IL_00ef:  ldloc.s    V_4
    IL_00f1:  ldloc.s    V_5
    IL_00f3:  clt
    IL_00f5:  sub
    IL_00f6:  ret

    IL_00f7:  ldloc.0
    IL_00f8:  ldloc.1
    IL_00f9:  sub
    IL_00fa:  ret

    IL_00fb:  ldc.i4.1
    IL_00fc:  ret
  } 

  .method public static int32  select1(class assembly/Test1 x) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 assembly/Test1::get_Tag()
    IL_0007:  switch     ( 
                          IL_001c,
                          IL_0028,
                          IL_002a,
                          IL_002c)
    IL_001c:  ldarg.0
    IL_001d:  castclass  assembly/Test1/X11
    IL_0022:  ldfld      int32 assembly/Test1/X11::item
    IL_0027:  ret

    IL_0028:  ldc.i4.2
    IL_0029:  ret

    IL_002a:  ldc.i4.3
    IL_002b:  ret

    IL_002c:  ldc.i4.4
    IL_002d:  ret
  } 

  .method public static int32  fm(class assembly/Test1 y) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       int32 assembly::select1(class assembly/Test1)
    IL_0006:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






