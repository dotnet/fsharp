




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
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed Match01
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto autochar serializable nested public beforefieldinit Test1
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class Match01/Test1>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class Match01/Test1>,
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
           extends Match01/Test1
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
      .method assembly specialname rtspecialname instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 0D 4D 61 74 63 68 30 31 2B 54   
                                                                                                                                                       65 73 74 31 00 00 )                               
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.0
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X11::item
        IL_000e:  ret
      } 

      .method public hidebysig instance int32 get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X11::item
        IL_0006:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X11::get_Item()
      } 
    } 

    .class auto ansi serializable nested public beforefieldinit specialname X12
           extends Match01/Test1
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
      .method assembly specialname rtspecialname instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 0D 4D 61 74 63 68 30 31 2B 54   
                                                                                                                                                       65 73 74 31 00 00 )                               
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.1
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X12::item
        IL_000e:  ret
      } 

      .method public hidebysig instance int32 get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X12::item
        IL_0006:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X12::get_Item()
      } 
    } 

    .class auto ansi serializable nested public beforefieldinit specialname X13
           extends Match01/Test1
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
      .method assembly specialname rtspecialname instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 0D 4D 61 74 63 68 30 31 2B 54   
                                                                                                                                                       65 73 74 31 00 00 )                               
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.2
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X13::item
        IL_000e:  ret
      } 

      .method public hidebysig instance int32 get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X13::item
        IL_0006:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X13::get_Item()
      } 
    } 

    .class auto ansi serializable nested public beforefieldinit specialname X14
           extends Match01/Test1
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
      .method assembly specialname rtspecialname instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 0D 4D 61 74 63 68 30 31 2B 54   
                                                                                                                                                       65 73 74 31 00 00 )                               
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.3
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X14::item
        IL_000e:  ret
      } 

      .method public hidebysig instance int32 get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X14::item
        IL_0006:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X14::get_Item()
      } 
    } 

    .class auto ansi nested assembly beforefieldinit specialname X11@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class Match01/Test1/X11 _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname instance void  .ctor(class Match01/Test1/X11 obj) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 0D 4D 61 74 63 68 30 31 2B 54   
                                                                                                                                                       65 73 74 31 00 00 )                               
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X11 Match01/Test1/X11@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X11 Match01/Test1/X11@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X11::item
        IL_000b:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X11@DebugTypeProxy::get_Item()
      } 
    } 

    .class auto ansi nested assembly beforefieldinit specialname X12@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class Match01/Test1/X12 _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname instance void  .ctor(class Match01/Test1/X12 obj) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 0D 4D 61 74 63 68 30 31 2B 54   
                                                                                                                                                       65 73 74 31 00 00 )                               
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X12 Match01/Test1/X12@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X12 Match01/Test1/X12@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X12::item
        IL_000b:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X12@DebugTypeProxy::get_Item()
      } 
    } 

    .class auto ansi nested assembly beforefieldinit specialname X13@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class Match01/Test1/X13 _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname instance void  .ctor(class Match01/Test1/X13 obj) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 0D 4D 61 74 63 68 30 31 2B 54   
                                                                                                                                                       65 73 74 31 00 00 )                               
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X13 Match01/Test1/X13@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X13 Match01/Test1/X13@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X13::item
        IL_000b:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X13@DebugTypeProxy::get_Item()
      } 
    } 

    .class auto ansi nested assembly beforefieldinit specialname X14@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class Match01/Test1/X14 _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname instance void  .ctor(class Match01/Test1/X14 obj) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 0D 4D 61 74 63 68 30 31 2B 54   
                                                                                                                                                       65 73 74 31 00 00 )                               
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X14 Match01/Test1/X14@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X14 Match01/Test1/X14@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X14::item
        IL_000b:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X14@DebugTypeProxy::get_Item()
      } 
    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit clo@4
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
    {
      .field public class Match01/Test1 this
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class Match01/Test1 obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class Match01/Test1 this, class Match01/Test1 obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1 Match01/Test1/clo@4::this
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class Match01/Test1 Match01/Test1/clo@4::obj
        IL_0014:  ret
      } 

      .method public strict virtual instance int32 Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  7
        .locals init (int32 V_0,
                 int32 V_1,
                 class Match01/Test1/X11 V_2,
                 class Match01/Test1/X11 V_3,
                 class [runtime]System.Collections.IComparer V_4,
                 int32 V_5,
                 int32 V_6,
                 class Match01/Test1/X12 V_7,
                 class Match01/Test1/X12 V_8,
                 class Match01/Test1/X13 V_9,
                 class Match01/Test1/X13 V_10,
                 class Match01/Test1/X14 V_11,
                 class Match01/Test1/X14 V_12)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1 Match01/Test1/clo@4::this
        IL_0006:  ldfld      int32 Match01/Test1::_tag
        IL_000b:  stloc.0
        IL_000c:  ldarg.0
        IL_000d:  ldfld      class Match01/Test1 Match01/Test1/clo@4::obj
        IL_0012:  ldfld      int32 Match01/Test1::_tag
        IL_0017:  stloc.1
        IL_0018:  ldloc.0
        IL_0019:  ldloc.1
        IL_001a:  bne.un     IL_013f

        IL_001f:  ldarg.0
        IL_0020:  ldfld      class Match01/Test1 Match01/Test1/clo@4::this
        IL_0025:  call       instance int32 Match01/Test1::get_Tag()
        IL_002a:  switch     ( 
                              IL_003f,
                              IL_007c,
                              IL_00bd,
                              IL_00fe)
        IL_003f:  ldarg.0
        IL_0040:  ldfld      class Match01/Test1 Match01/Test1/clo@4::this
        IL_0045:  castclass  Match01/Test1/X11
        IL_004a:  stloc.2
        IL_004b:  ldarg.0
        IL_004c:  ldfld      class Match01/Test1 Match01/Test1/clo@4::obj
        IL_0051:  castclass  Match01/Test1/X11
        IL_0056:  stloc.3
        IL_0057:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_005c:  stloc.s    V_4
        IL_005e:  ldloc.2
        IL_005f:  ldfld      int32 Match01/Test1/X11::item
        IL_0064:  stloc.s    V_5
        IL_0066:  ldloc.3
        IL_0067:  ldfld      int32 Match01/Test1/X11::item
        IL_006c:  stloc.s    V_6
        IL_006e:  ldloc.s    V_5
        IL_0070:  ldloc.s    V_6
        IL_0072:  cgt
        IL_0074:  ldloc.s    V_5
        IL_0076:  ldloc.s    V_6
        IL_0078:  clt
        IL_007a:  sub
        IL_007b:  ret

        IL_007c:  ldarg.0
        IL_007d:  ldfld      class Match01/Test1 Match01/Test1/clo@4::this
        IL_0082:  castclass  Match01/Test1/X12
        IL_0087:  stloc.s    V_7
        IL_0089:  ldarg.0
        IL_008a:  ldfld      class Match01/Test1 Match01/Test1/clo@4::obj
        IL_008f:  castclass  Match01/Test1/X12
        IL_0094:  stloc.s    V_8
        IL_0096:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_009b:  stloc.s    V_4
        IL_009d:  ldloc.s    V_7
        IL_009f:  ldfld      int32 Match01/Test1/X12::item
        IL_00a4:  stloc.s    V_5
        IL_00a6:  ldloc.s    V_8
        IL_00a8:  ldfld      int32 Match01/Test1/X12::item
        IL_00ad:  stloc.s    V_6
        IL_00af:  ldloc.s    V_5
        IL_00b1:  ldloc.s    V_6
        IL_00b3:  cgt
        IL_00b5:  ldloc.s    V_5
        IL_00b7:  ldloc.s    V_6
        IL_00b9:  clt
        IL_00bb:  sub
        IL_00bc:  ret

        IL_00bd:  ldarg.0
        IL_00be:  ldfld      class Match01/Test1 Match01/Test1/clo@4::this
        IL_00c3:  castclass  Match01/Test1/X13
        IL_00c8:  stloc.s    V_9
        IL_00ca:  ldarg.0
        IL_00cb:  ldfld      class Match01/Test1 Match01/Test1/clo@4::obj
        IL_00d0:  castclass  Match01/Test1/X13
        IL_00d5:  stloc.s    V_10
        IL_00d7:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_00dc:  stloc.s    V_4
        IL_00de:  ldloc.s    V_9
        IL_00e0:  ldfld      int32 Match01/Test1/X13::item
        IL_00e5:  stloc.s    V_5
        IL_00e7:  ldloc.s    V_10
        IL_00e9:  ldfld      int32 Match01/Test1/X13::item
        IL_00ee:  stloc.s    V_6
        IL_00f0:  ldloc.s    V_5
        IL_00f2:  ldloc.s    V_6
        IL_00f4:  cgt
        IL_00f6:  ldloc.s    V_5
        IL_00f8:  ldloc.s    V_6
        IL_00fa:  clt
        IL_00fc:  sub
        IL_00fd:  ret

        IL_00fe:  ldarg.0
        IL_00ff:  ldfld      class Match01/Test1 Match01/Test1/clo@4::this
        IL_0104:  castclass  Match01/Test1/X14
        IL_0109:  stloc.s    V_11
        IL_010b:  ldarg.0
        IL_010c:  ldfld      class Match01/Test1 Match01/Test1/clo@4::obj
        IL_0111:  castclass  Match01/Test1/X14
        IL_0116:  stloc.s    V_12
        IL_0118:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_011d:  stloc.s    V_4
        IL_011f:  ldloc.s    V_11
        IL_0121:  ldfld      int32 Match01/Test1/X14::item
        IL_0126:  stloc.s    V_5
        IL_0128:  ldloc.s    V_12
        IL_012a:  ldfld      int32 Match01/Test1/X14::item
        IL_012f:  stloc.s    V_6
        IL_0131:  ldloc.s    V_5
        IL_0133:  ldloc.s    V_6
        IL_0135:  cgt
        IL_0137:  ldloc.s    V_5
        IL_0139:  ldloc.s    V_6
        IL_013b:  clt
        IL_013d:  sub
        IL_013e:  ret

        IL_013f:  ldloc.0
        IL_0140:  ldloc.1
        IL_0141:  sub
        IL_0142:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit 'clo@4-1'
           extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
    {
      .field public class Match01/Test1 this
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class Match01/Test1 objTemp
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class Match01/Test1 this, class Match01/Test1 objTemp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1 Match01/Test1/'clo@4-1'::this
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      class Match01/Test1 Match01/Test1/'clo@4-1'::objTemp
        IL_0014:  ret
      } 

      .method public strict virtual instance int32 Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
      {
        
        .maxstack  7
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
        IL_0001:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::this
        IL_0006:  ldfld      int32 Match01/Test1::_tag
        IL_000b:  stloc.0
        IL_000c:  ldarg.0
        IL_000d:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::objTemp
        IL_0012:  ldfld      int32 Match01/Test1::_tag
        IL_0017:  stloc.1
        IL_0018:  ldloc.0
        IL_0019:  ldloc.1
        IL_001a:  bne.un     IL_0123

        IL_001f:  ldarg.0
        IL_0020:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::this
        IL_0025:  call       instance int32 Match01/Test1::get_Tag()
        IL_002a:  switch     ( 
                              IL_003f,
                              IL_0075,
                              IL_00af,
                              IL_00e9)
        IL_003f:  ldarg.0
        IL_0040:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::this
        IL_0045:  castclass  Match01/Test1/X11
        IL_004a:  stloc.2
        IL_004b:  ldarg.0
        IL_004c:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::objTemp
        IL_0051:  castclass  Match01/Test1/X11
        IL_0056:  stloc.3
        IL_0057:  ldloc.2
        IL_0058:  ldfld      int32 Match01/Test1/X11::item
        IL_005d:  stloc.s    V_4
        IL_005f:  ldloc.3
        IL_0060:  ldfld      int32 Match01/Test1/X11::item
        IL_0065:  stloc.s    V_5
        IL_0067:  ldloc.s    V_4
        IL_0069:  ldloc.s    V_5
        IL_006b:  cgt
        IL_006d:  ldloc.s    V_4
        IL_006f:  ldloc.s    V_5
        IL_0071:  clt
        IL_0073:  sub
        IL_0074:  ret

        IL_0075:  ldarg.0
        IL_0076:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::this
        IL_007b:  castclass  Match01/Test1/X12
        IL_0080:  stloc.s    V_6
        IL_0082:  ldarg.0
        IL_0083:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::objTemp
        IL_0088:  castclass  Match01/Test1/X12
        IL_008d:  stloc.s    V_7
        IL_008f:  ldloc.s    V_6
        IL_0091:  ldfld      int32 Match01/Test1/X12::item
        IL_0096:  stloc.s    V_4
        IL_0098:  ldloc.s    V_7
        IL_009a:  ldfld      int32 Match01/Test1/X12::item
        IL_009f:  stloc.s    V_5
        IL_00a1:  ldloc.s    V_4
        IL_00a3:  ldloc.s    V_5
        IL_00a5:  cgt
        IL_00a7:  ldloc.s    V_4
        IL_00a9:  ldloc.s    V_5
        IL_00ab:  clt
        IL_00ad:  sub
        IL_00ae:  ret

        IL_00af:  ldarg.0
        IL_00b0:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::this
        IL_00b5:  castclass  Match01/Test1/X13
        IL_00ba:  stloc.s    V_8
        IL_00bc:  ldarg.0
        IL_00bd:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::objTemp
        IL_00c2:  castclass  Match01/Test1/X13
        IL_00c7:  stloc.s    V_9
        IL_00c9:  ldloc.s    V_8
        IL_00cb:  ldfld      int32 Match01/Test1/X13::item
        IL_00d0:  stloc.s    V_4
        IL_00d2:  ldloc.s    V_9
        IL_00d4:  ldfld      int32 Match01/Test1/X13::item
        IL_00d9:  stloc.s    V_5
        IL_00db:  ldloc.s    V_4
        IL_00dd:  ldloc.s    V_5
        IL_00df:  cgt
        IL_00e1:  ldloc.s    V_4
        IL_00e3:  ldloc.s    V_5
        IL_00e5:  clt
        IL_00e7:  sub
        IL_00e8:  ret

        IL_00e9:  ldarg.0
        IL_00ea:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::this
        IL_00ef:  castclass  Match01/Test1/X14
        IL_00f4:  stloc.s    V_10
        IL_00f6:  ldarg.0
        IL_00f7:  ldfld      class Match01/Test1 Match01/Test1/'clo@4-1'::objTemp
        IL_00fc:  castclass  Match01/Test1/X14
        IL_0101:  stloc.s    V_11
        IL_0103:  ldloc.s    V_10
        IL_0105:  ldfld      int32 Match01/Test1/X14::item
        IL_010a:  stloc.s    V_4
        IL_010c:  ldloc.s    V_11
        IL_010e:  ldfld      int32 Match01/Test1/X14::item
        IL_0113:  stloc.s    V_5
        IL_0115:  ldloc.s    V_4
        IL_0117:  ldloc.s    V_5
        IL_0119:  cgt
        IL_011b:  ldloc.s    V_4
        IL_011d:  ldloc.s    V_5
        IL_011f:  clt
        IL_0121:  sub
        IL_0122:  ret

        IL_0123:  ldloc.0
        IL_0124:  ldloc.1
        IL_0125:  sub
        IL_0126:  ret
      } 

    } 

    .field assembly initonly int32 _tag
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(int32 _tag) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 E0 07 00 00 0D 4D 61 74 63 68 30 31 2B 54   
                                                                                                                                                     65 73 74 31 00 00 )                               
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 Match01/Test1::_tag
      IL_000d:  ret
    } 

    .method public static class Match01/Test1 NewX11(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X11::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool get_IsX11() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public static class Match01/Test1 NewX12(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X12::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool get_IsX12() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public static class Match01/Test1 NewX13(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 02 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X13::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool get_IsX13() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.2
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public static class Match01/Test1 NewX14(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 03 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X14::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool get_IsX14() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.3
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public hidebysig instance int32 get_Tag() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Match01/Test1::_tag
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname instance object  __DebugDisplay() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Match01/Test1>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(class Match01/Test1 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0018

      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  newobj     instance void Match01/Test1/clo@4::.ctor(class Match01/Test1,
                                                                    class Match01/Test1)
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  ldnull
      IL_0010:  tail.
      IL_0012:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0017:  ret

      IL_0018:  ldc.i4.1
      IL_0019:  ret

      IL_001a:  ldarg.1
      IL_001b:  brfalse.s  IL_001f

      IL_001d:  ldc.i4.m1
      IL_001e:  ret

      IL_001f:  ldc.i4.0
      IL_0020:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  Match01/Test1
      IL_0007:  callvirt   instance int32 Match01/Test1::CompareTo(class Match01/Test1)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class Match01/Test1 V_0,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> V_1)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  Match01/Test1
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0026

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  Match01/Test1
      IL_0010:  brfalse.s  IL_0024

      IL_0012:  ldarg.0
      IL_0013:  ldloc.0
      IL_0014:  newobj     instance void Match01/Test1/'clo@4-1'::.ctor(class Match01/Test1,
                                                                        class Match01/Test1)
      IL_0019:  stloc.1
      IL_001a:  ldloc.1
      IL_001b:  ldnull
      IL_001c:  tail.
      IL_001e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0023:  ret

      IL_0024:  ldc.i4.1
      IL_0025:  ret

      IL_0026:  ldarg.1
      IL_0027:  unbox.any  Match01/Test1
      IL_002c:  brfalse.s  IL_0030

      IL_002e:  ldc.i4.m1
      IL_002f:  ret

      IL_0030:  ldc.i4.0
      IL_0031:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
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
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 Match01/Test1::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(class Match01/Test1 obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
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
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class Match01/Test1 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     Match01/Test1
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  ldarg.2
      IL_000d:  callvirt   instance bool Match01/Test1::Equals(class Match01/Test1,
                                                               class [runtime]System.Collections.IEqualityComparer)
      IL_0012:  ret

      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(class Match01/Test1 obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
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
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
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
    } 

    .property instance int32 Tag()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 Match01/Test1::get_Tag()
    } 
    .property instance bool IsX11()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX11()
    } 
    .property instance bool IsX12()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX12()
    } 
    .property instance bool IsX13()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX13()
    } 
    .property instance bool IsX14()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX14()
    } 
  } 

  .method public static int32  select1(class Match01/Test1 x) cil managed
  {
    
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
  } 

  .method public static int32  fm(class Match01/Test1 y) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       int32 Match01::select1(class Match01/Test1)
    IL_0006:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Match01
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






