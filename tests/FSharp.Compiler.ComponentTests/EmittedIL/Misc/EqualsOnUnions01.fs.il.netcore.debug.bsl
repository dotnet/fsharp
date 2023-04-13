




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
  .class abstract auto autochar serializable nested public beforefieldinit U
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/U>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/U>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                         61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [runtime]System.Object
    {
      .field public static literal int32 A = int32(0x00000000)
      .field public static literal int32 B = int32(0x00000001)
    } 

    .class auto ansi serializable nested assembly beforefieldinit specialname _A
           extends assembly/U
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [runtime]System.Type) = ( 01 00 24 45 71 75 61 6C 73 4F 6E 55 6E 69 6F 6E   
                                                                                                                                        73 30 31 2B 55 2B 5F 41 40 44 65 62 75 67 54 79   
                                                                                                                                        70 65 50 72 6F 78 79 00 00 )                      
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   
      .method assembly specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 12 45 71 75 61 6C 73 4F 6E 55   
                                                                                                                                                       6E 69 6F 6E 73 30 31 2B 55 00 00 )                
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void assembly/U::.ctor()
        IL_0006:  ret
      } 

    } 

    .class auto ansi serializable nested public beforefieldinit specialname B
           extends assembly/U
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [runtime]System.Type) = ( 01 00 23 45 71 75 61 6C 73 4F 6E 55 6E 69 6F 6E   
                                                                                                                                        73 30 31 2B 55 2B 42 40 44 65 62 75 67 54 79 70   
                                                                                                                                        65 50 72 6F 78 79 00 00 )                         
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   
      .field assembly initonly int32 item
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 12 45 71 75 61 6C 73 4F 6E 55   
                                                                                                                                                       6E 69 6F 6E 73 30 31 2B 55 00 00 )                
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void assembly/U::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 assembly/U/B::item
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/U/B::item
        IL_0006:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/U/B::get_Item()
      } 
    } 

    .class auto ansi nested assembly beforefieldinit specialname _A@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class assembly/U/_A _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class assembly/U/_A obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/U/_A assembly/U/_A@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

    } 

    .class auto ansi nested assembly beforefieldinit specialname B@DebugTypeProxy
           extends [runtime]System.Object
    {
      .field assembly class assembly/U/B _obj
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class assembly/U/B obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/U/B assembly/U/B@DebugTypeProxy::_obj
        IL_000d:  ret
      } 

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class assembly/U/B assembly/U/B@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 assembly/U/B::item
        IL_000b:  ret
      } 

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/U/B@DebugTypeProxy::get_Item()
      } 
    } 

    .field static assembly initonly class assembly/U _unique_A
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  newobj     instance void assembly/U/_A::.ctor()
      IL_0005:  stsfld     class assembly/U assembly/U::_unique_A
      IL_000a:  ret
    } 

    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 E0 07 00 00 12 45 71 75 61 6C 73 4F 6E 55   
                                                                                                                                                     6E 69 6F 6E 73 30 31 2B 55 00 00 )                
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ret
    } 

    .method public static class assembly/U 
            get_A() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldsfld     class assembly/U assembly/U::_unique_A
      IL_0005:  ret
    } 

    .method public hidebysig instance bool 
            get_IsA() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  isinst     assembly/U/_A
      IL_0006:  ldnull
      IL_0007:  cgt.un
      IL_0009:  ret
    } 

    .method public static class assembly/U 
            NewB(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void assembly/U/B::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool 
            get_IsB() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  isinst     assembly/U/B
      IL_0006:  ldnull
      IL_0007:  cgt.un
      IL_0009:  ret
    } 

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  isinst     assembly/U/B
      IL_0006:  brfalse.s  IL_000b

      IL_0008:  ldc.i4.1
      IL_0009:  br.s       IL_000c

      IL_000b:  ldc.i4.0
      IL_000c:  ret
    } 

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/U>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/U,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(class assembly/U obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (int32 V_0,
               class assembly/U V_1,
               int32 V_2,
               class assembly/U V_3,
               class assembly/U/B V_4,
               class assembly/U/B V_5,
               class [runtime]System.Collections.IComparer V_6,
               int32 V_7,
               int32 V_8,
               class [runtime]System.Collections.IComparer V_9,
               int32 V_10,
               int32 V_11)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_0081

      IL_0006:  ldarg.1
      IL_0007:  brfalse    IL_007f

      IL_000c:  ldarg.0
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  isinst     assembly/U/B
      IL_0014:  brfalse.s  IL_0019

      IL_0016:  ldc.i4.1
      IL_0017:  br.s       IL_001a

      IL_0019:  ldc.i4.0
      IL_001a:  stloc.0
      IL_001b:  ldarg.1
      IL_001c:  stloc.3
      IL_001d:  ldloc.3
      IL_001e:  isinst     assembly/U/B
      IL_0023:  brfalse.s  IL_0028

      IL_0025:  ldc.i4.1
      IL_0026:  br.s       IL_0029

      IL_0028:  ldc.i4.0
      IL_0029:  stloc.2
      IL_002a:  ldloc.0
      IL_002b:  ldloc.2
      IL_002c:  bne.un.s   IL_007b

      IL_002e:  ldarg.0
      IL_002f:  isinst     assembly/U/B
      IL_0034:  brfalse.s  IL_0079

      IL_0036:  ldarg.0
      IL_0037:  castclass  assembly/U/B
      IL_003c:  stloc.s    V_4
      IL_003e:  ldarg.1
      IL_003f:  castclass  assembly/U/B
      IL_0044:  stloc.s    V_5
      IL_0046:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_004b:  stloc.s    V_6
      IL_004d:  ldloc.s    V_4
      IL_004f:  ldfld      int32 assembly/U/B::item
      IL_0054:  stloc.s    V_7
      IL_0056:  ldloc.s    V_5
      IL_0058:  ldfld      int32 assembly/U/B::item
      IL_005d:  stloc.s    V_8
      IL_005f:  ldloc.s    V_6
      IL_0061:  stloc.s    V_9
      IL_0063:  ldloc.s    V_7
      IL_0065:  stloc.s    V_10
      IL_0067:  ldloc.s    V_8
      IL_0069:  stloc.s    V_11
      IL_006b:  ldloc.s    V_10
      IL_006d:  ldloc.s    V_11
      IL_006f:  cgt
      IL_0071:  ldloc.s    V_10
      IL_0073:  ldloc.s    V_11
      IL_0075:  clt
      IL_0077:  sub
      IL_0078:  ret

      IL_0079:  ldc.i4.0
      IL_007a:  ret

      IL_007b:  ldloc.0
      IL_007c:  ldloc.2
      IL_007d:  sub
      IL_007e:  ret

      IL_007f:  ldc.i4.1
      IL_0080:  ret

      IL_0081:  ldarg.1
      IL_0082:  brfalse.s  IL_0086

      IL_0084:  ldc.i4.m1
      IL_0085:  ret

      IL_0086:  ldc.i4.0
      IL_0087:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/U
      IL_0007:  callvirt   instance int32 assembly/U::CompareTo(class assembly/U)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (class assembly/U V_0,
               int32 V_1,
               class assembly/U V_2,
               int32 V_3,
               class assembly/U V_4,
               class assembly/U/B V_5,
               class assembly/U/B V_6,
               class [runtime]System.Collections.IComparer V_7,
               int32 V_8,
               int32 V_9,
               class [runtime]System.Collections.IComparer V_10,
               int32 V_11,
               int32 V_12)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/U
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse    IL_008b

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  assembly/U
      IL_0013:  brfalse    IL_0089

      IL_0018:  ldarg.0
      IL_0019:  stloc.2
      IL_001a:  ldloc.2
      IL_001b:  isinst     assembly/U/B
      IL_0020:  brfalse.s  IL_0025

      IL_0022:  ldc.i4.1
      IL_0023:  br.s       IL_0026

      IL_0025:  ldc.i4.0
      IL_0026:  stloc.1
      IL_0027:  ldloc.0
      IL_0028:  stloc.s    V_4
      IL_002a:  ldloc.s    V_4
      IL_002c:  isinst     assembly/U/B
      IL_0031:  brfalse.s  IL_0036

      IL_0033:  ldc.i4.1
      IL_0034:  br.s       IL_0037

      IL_0036:  ldc.i4.0
      IL_0037:  stloc.3
      IL_0038:  ldloc.1
      IL_0039:  ldloc.3
      IL_003a:  bne.un.s   IL_0085

      IL_003c:  ldarg.0
      IL_003d:  isinst     assembly/U/B
      IL_0042:  brfalse.s  IL_0083

      IL_0044:  ldarg.0
      IL_0045:  castclass  assembly/U/B
      IL_004a:  stloc.s    V_5
      IL_004c:  ldloc.0
      IL_004d:  castclass  assembly/U/B
      IL_0052:  stloc.s    V_6
      IL_0054:  ldarg.2
      IL_0055:  stloc.s    V_7
      IL_0057:  ldloc.s    V_5
      IL_0059:  ldfld      int32 assembly/U/B::item
      IL_005e:  stloc.s    V_8
      IL_0060:  ldloc.s    V_6
      IL_0062:  ldfld      int32 assembly/U/B::item
      IL_0067:  stloc.s    V_9
      IL_0069:  ldloc.s    V_7
      IL_006b:  stloc.s    V_10
      IL_006d:  ldloc.s    V_8
      IL_006f:  stloc.s    V_11
      IL_0071:  ldloc.s    V_9
      IL_0073:  stloc.s    V_12
      IL_0075:  ldloc.s    V_11
      IL_0077:  ldloc.s    V_12
      IL_0079:  cgt
      IL_007b:  ldloc.s    V_11
      IL_007d:  ldloc.s    V_12
      IL_007f:  clt
      IL_0081:  sub
      IL_0082:  ret

      IL_0083:  ldc.i4.0
      IL_0084:  ret

      IL_0085:  ldloc.1
      IL_0086:  ldloc.3
      IL_0087:  sub
      IL_0088:  ret

      IL_0089:  ldc.i4.1
      IL_008a:  ret

      IL_008b:  ldarg.1
      IL_008c:  unbox.any  assembly/U
      IL_0091:  brfalse.s  IL_0095

      IL_0093:  ldc.i4.m1
      IL_0094:  ret

      IL_0095:  ldc.i4.0
      IL_0096:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0,
               class assembly/U/B V_1,
               class [runtime]System.Collections.IEqualityComparer V_2,
               int32 V_3,
               class [runtime]System.Collections.IEqualityComparer V_4,
               class assembly/U V_5)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0045

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldarg.0
      IL_0006:  isinst     assembly/U/B
      IL_000b:  brfalse.s  IL_0034

      IL_000d:  ldarg.0
      IL_000e:  castclass  assembly/U/B
      IL_0013:  stloc.1
      IL_0014:  ldc.i4.1
      IL_0015:  stloc.0
      IL_0016:  ldc.i4     0x9e3779b9
      IL_001b:  ldarg.1
      IL_001c:  stloc.2
      IL_001d:  ldloc.1
      IL_001e:  ldfld      int32 assembly/U/B::item
      IL_0023:  stloc.3
      IL_0024:  ldloc.2
      IL_0025:  stloc.s    V_4
      IL_0027:  ldloc.3
      IL_0028:  ldloc.0
      IL_0029:  ldc.i4.6
      IL_002a:  shl
      IL_002b:  ldloc.0
      IL_002c:  ldc.i4.2
      IL_002d:  shr
      IL_002e:  add
      IL_002f:  add
      IL_0030:  add
      IL_0031:  stloc.0
      IL_0032:  ldloc.0
      IL_0033:  ret

      IL_0034:  ldarg.0
      IL_0035:  stloc.s    V_5
      IL_0037:  ldloc.s    V_5
      IL_0039:  isinst     assembly/U/B
      IL_003e:  brfalse.s  IL_0043

      IL_0040:  ldc.i4.1
      IL_0041:  br.s       IL_0044

      IL_0043:  ldc.i4.0
      IL_0044:  ret

      IL_0045:  ldc.i4.0
      IL_0046:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 assembly/U::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/U V_0,
               class assembly/U V_1,
               int32 V_2,
               class assembly/U V_3,
               int32 V_4,
               class assembly/U V_5,
               class assembly/U/B V_6,
               class assembly/U/B V_7,
               class [runtime]System.Collections.IEqualityComparer V_8,
               int32 V_9,
               int32 V_10,
               class [runtime]System.Collections.IEqualityComparer V_11)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_0076

      IL_0006:  ldarg.1
      IL_0007:  isinst     assembly/U
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  brfalse.s  IL_0074

      IL_0010:  ldloc.0
      IL_0011:  stloc.1
      IL_0012:  ldarg.0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  isinst     assembly/U/B
      IL_001a:  brfalse.s  IL_001f

      IL_001c:  ldc.i4.1
      IL_001d:  br.s       IL_0020

      IL_001f:  ldc.i4.0
      IL_0020:  stloc.2
      IL_0021:  ldloc.1
      IL_0022:  stloc.s    V_5
      IL_0024:  ldloc.s    V_5
      IL_0026:  isinst     assembly/U/B
      IL_002b:  brfalse.s  IL_0030

      IL_002d:  ldc.i4.1
      IL_002e:  br.s       IL_0031

      IL_0030:  ldc.i4.0
      IL_0031:  stloc.s    V_4
      IL_0033:  ldloc.2
      IL_0034:  ldloc.s    V_4
      IL_0036:  bne.un.s   IL_0072

      IL_0038:  ldarg.0
      IL_0039:  isinst     assembly/U/B
      IL_003e:  brfalse.s  IL_0070

      IL_0040:  ldarg.0
      IL_0041:  castclass  assembly/U/B
      IL_0046:  stloc.s    V_6
      IL_0048:  ldloc.1
      IL_0049:  castclass  assembly/U/B
      IL_004e:  stloc.s    V_7
      IL_0050:  ldarg.2
      IL_0051:  stloc.s    V_8
      IL_0053:  ldloc.s    V_6
      IL_0055:  ldfld      int32 assembly/U/B::item
      IL_005a:  stloc.s    V_9
      IL_005c:  ldloc.s    V_7
      IL_005e:  ldfld      int32 assembly/U/B::item
      IL_0063:  stloc.s    V_10
      IL_0065:  ldloc.s    V_8
      IL_0067:  stloc.s    V_11
      IL_0069:  ldloc.s    V_9
      IL_006b:  ldloc.s    V_10
      IL_006d:  ceq
      IL_006f:  ret

      IL_0070:  ldc.i4.1
      IL_0071:  ret

      IL_0072:  ldc.i4.0
      IL_0073:  ret

      IL_0074:  ldc.i4.0
      IL_0075:  ret

      IL_0076:  ldarg.1
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  ldc.i4.0
      IL_007b:  ceq
      IL_007d:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(class assembly/U obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (int32 V_0,
               class assembly/U V_1,
               int32 V_2,
               class assembly/U V_3,
               class assembly/U/B V_4,
               class assembly/U/B V_5)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0057

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0055

      IL_0006:  ldarg.0
      IL_0007:  stloc.1
      IL_0008:  ldloc.1
      IL_0009:  isinst     assembly/U/B
      IL_000e:  brfalse.s  IL_0013

      IL_0010:  ldc.i4.1
      IL_0011:  br.s       IL_0014

      IL_0013:  ldc.i4.0
      IL_0014:  stloc.0
      IL_0015:  ldarg.1
      IL_0016:  stloc.3
      IL_0017:  ldloc.3
      IL_0018:  isinst     assembly/U/B
      IL_001d:  brfalse.s  IL_0022

      IL_001f:  ldc.i4.1
      IL_0020:  br.s       IL_0023

      IL_0022:  ldc.i4.0
      IL_0023:  stloc.2
      IL_0024:  ldloc.0
      IL_0025:  ldloc.2
      IL_0026:  bne.un.s   IL_0053

      IL_0028:  ldarg.0
      IL_0029:  isinst     assembly/U/B
      IL_002e:  brfalse.s  IL_0051

      IL_0030:  ldarg.0
      IL_0031:  castclass  assembly/U/B
      IL_0036:  stloc.s    V_4
      IL_0038:  ldarg.1
      IL_0039:  castclass  assembly/U/B
      IL_003e:  stloc.s    V_5
      IL_0040:  ldloc.s    V_4
      IL_0042:  ldfld      int32 assembly/U/B::item
      IL_0047:  ldloc.s    V_5
      IL_0049:  ldfld      int32 assembly/U/B::item
      IL_004e:  ceq
      IL_0050:  ret

      IL_0051:  ldc.i4.1
      IL_0052:  ret

      IL_0053:  ldc.i4.0
      IL_0054:  ret

      IL_0055:  ldc.i4.0
      IL_0056:  ret

      IL_0057:  ldarg.1
      IL_0058:  ldnull
      IL_0059:  cgt.un
      IL_005b:  ldc.i4.0
      IL_005c:  ceq
      IL_005e:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/U V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/U
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/U::Equals(class assembly/U)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 Tag()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/U::get_Tag()
    } 
    .property class assembly/U A()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class assembly/U assembly/U::get_A()
    } 
    .property instance bool IsA()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool assembly/U::get_IsA()
    } 
    .property instance bool IsB()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool assembly/U::get_IsB()
    } 
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






