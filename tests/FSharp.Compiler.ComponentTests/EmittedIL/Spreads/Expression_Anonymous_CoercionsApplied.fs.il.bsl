




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





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit T
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.DefaultAugmentationAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                         61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .field assembly initonly int32 item
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(int32 item) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 27 45 78 70 72 65 73 73 69 6F   
                                                                                                                                                     6E 5F 41 6E 6F 6E 79 6D 6F 75 73 5F 43 6F 65 72   
                                                                                                                                                     63 69 6F 6E 73 41 70 70 6C 69 65 64 2B 54 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/T::item
      IL_000d:  ret
    } 

    .method public hidebysig instance int32 get_Item() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/T::item
      IL_0006:  ret
    } 

    .method public hidebysig instance int32 get_Tag() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldc.i4.0
      IL_0003:  ret
    } 

    .method assembly hidebysig specialname instance object  __DebugDisplay() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/T,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/T,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/T,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/T,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/T>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/T,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/T,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public specialname static class assembly/U op_Implicit(class assembly/T _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/T::item
      IL_0006:  newobj     instance void assembly/U::.ctor(int32)
      IL_000b:  ret
    } 

    .property instance int32 Tag()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/T::get_Tag()
    } 
    .property instance int32 Item()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 assembly/T::get_Item()
    } 
  } 

  .class auto autochar serializable sealed nested public beforefieldinit U
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.DefaultAugmentationAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                         61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .field assembly initonly int32 item
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor(int32 item) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 27 45 78 70 72 65 73 73 69 6F   
                                                                                                                                                     6E 5F 41 6E 6F 6E 79 6D 6F 75 73 5F 43 6F 65 72   
                                                                                                                                                     63 69 6F 6E 73 41 70 70 6C 69 65 64 2B 55 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/U::item
      IL_000d:  ret
    } 

    .method public hidebysig instance int32 get_Item() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/U::item
      IL_0006:  ret
    } 

    .method public hidebysig instance int32 get_Tag() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldc.i4.0
      IL_0003:  ret
    } 

    .method assembly hidebysig specialname instance object  __DebugDisplay() cil managed
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

    .method public strict virtual instance string ToString() cil managed
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

    .property instance int32 Tag()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/U::get_Tag()
    } 
    .property instance int32 Item()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance int32 assembly/U::get_Item()
    } 
  } 

  .field static assembly class '<>f__AnonymousType2396826819`1'<class assembly/T> r6@11
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class '<>f__AnonymousType2396826819`1'<class assembly/U> r7@12
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class '<>f__AnonymousType2396826819`1'<class assembly/U> r8@13
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class assembly/T _arg1@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class '<>f__AnonymousType2396826819`1'<class assembly/T> get_r6() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class '<>f__AnonymousType2396826819`1'<class assembly/T> assembly::r6@11
    IL_0005:  ret
  } 

  .method public specialname static class '<>f__AnonymousType2396826819`1'<class assembly/U> get_r7() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class '<>f__AnonymousType2396826819`1'<class assembly/U> assembly::r7@12
    IL_0005:  ret
  } 

  .method public specialname static class '<>f__AnonymousType2396826819`1'<class assembly/U> get_r8() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class '<>f__AnonymousType2396826819`1'<class assembly/U> assembly::r8@13
    IL_0005:  ret
  } 

  .method assembly specialname static class assembly/T get__arg1@4() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/T assembly::_arg1@4
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

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  3
    IL_0000:  ldc.i4.3
    IL_0001:  newobj     instance void assembly/T::.ctor(int32)
    IL_0006:  newobj     instance void class '<>f__AnonymousType2396826819`1'<class assembly/T>::.ctor(!0)
    IL_000b:  stsfld     class '<>f__AnonymousType2396826819`1'<class assembly/T> assembly::r6@11
    IL_0010:  ldc.i4.3
    IL_0011:  newobj     instance void assembly/U::.ctor(int32)
    IL_0016:  newobj     instance void class '<>f__AnonymousType2396826819`1'<class assembly/U>::.ctor(!0)
    IL_001b:  stsfld     class '<>f__AnonymousType2396826819`1'<class assembly/U> assembly::r7@12
    IL_0020:  call       class '<>f__AnonymousType2396826819`1'<class assembly/T> assembly::get_r6()
    IL_0025:  call       instance !0 class '<>f__AnonymousType2396826819`1'<class assembly/T>::get_A()
    IL_002a:  stsfld     class assembly/T assembly::_arg1@4
    IL_002f:  call       class assembly/T assembly::get__arg1@4()
    IL_0034:  ldfld      int32 assembly/T::item
    IL_0039:  newobj     instance void assembly/U::.ctor(int32)
    IL_003e:  newobj     instance void class '<>f__AnonymousType2396826819`1'<class assembly/U>::.ctor(!0)
    IL_0043:  stsfld     class '<>f__AnonymousType2396826819`1'<class assembly/U> assembly::r8@13
    IL_0048:  ret
  } 

  .property class '<>f__AnonymousType2396826819`1'<class assembly/T>
          r6()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class '<>f__AnonymousType2396826819`1'<class assembly/T> assembly::get_r6()
  } 
  .property class '<>f__AnonymousType2396826819`1'<class assembly/U>
          r7()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class '<>f__AnonymousType2396826819`1'<class assembly/U> assembly::get_r7()
  } 
  .property class '<>f__AnonymousType2396826819`1'<class assembly/U>
          r8()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class '<>f__AnonymousType2396826819`1'<class assembly/U> assembly::get_r8()
  } 
  .property class assembly/T
          _arg1@4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class assembly/T assembly::get__arg1@4()
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

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType2396826819`1'<'<A>j__TPar'>
       extends [runtime]System.Object
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>>
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
  .field private !'<A>j__TPar' A@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(!'<A>j__TPar' A) cil managed
  {
    .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 1E 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 32 33 39 36 38 32 36   
                                                                                                                                                   38 31 39 60 31 00 00 )                            
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_000d:  ret
  } 

  .method public hidebysig specialname instance !'<A>j__TPar' get_A() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>,string>::Invoke(!0)
    IL_0015:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0021

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001f

    IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_0011:  ldarg.1
    IL_0012:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_0017:  tail.
    IL_0019:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_001e:  ret

    IL_001f:  ldc.i4.1
    IL_0020:  ret

    IL_0021:  ldarg.1
    IL_0022:  brfalse.s  IL_0026

    IL_0024:  ldc.i4.m1
    IL_0025:  ret

    IL_0026:  ldc.i4.0
    IL_0027:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>
    IL_0007:  tail.
    IL_0009:  callvirt   instance int32 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::CompareTo(class '<>f__AnonymousType2396826819`1'<!0>)
    IL_000e:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'> V_0,
             class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'> V_1)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_002b

    IL_000c:  ldarg.1
    IL_000d:  unbox.any  class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>
    IL_0012:  brfalse.s  IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_001b:  ldloc.1
    IL_001c:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_0021:  tail.
    IL_0023:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0028:  ret

    IL_0029:  ldc.i4.1
    IL_002a:  ret

    IL_002b:  ldarg.1
    IL_002c:  unbox.any  class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>
    IL_0031:  brfalse.s  IL_0035

    IL_0033:  ldc.i4.m1
    IL_0034:  ret

    IL_0035:  ldc.i4.0
    IL_0036:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  7
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0022

    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4     0x9e3779b9
    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_0011:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.6
    IL_0018:  shl
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.2
    IL_001b:  shr
    IL_001c:  add
    IL_001d:  add
    IL_001e:  add
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ret

    IL_0022:  ldc.i4.0
    IL_0023:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  tail.
    IL_0008:  callvirt   instance int32 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000d:  ret
  } 

  .method public hidebysig instance bool Equals(class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_001f

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001d

    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  ldarg.0
    IL_000a:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_000f:  ldloc.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_0015:  tail.
    IL_0017:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_001c:  ret

    IL_001d:  ldc.i4.0
    IL_001e:  ret

    IL_001f:  ldarg.1
    IL_0020:  ldnull
    IL_0021:  cgt.un
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0015

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  tail.
    IL_000f:  callvirt   instance bool class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::Equals(class '<>f__AnonymousType2396826819`1'<!0>,
                                                                                                     class [runtime]System.Collections.IEqualityComparer)
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_001c

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001a

    IL_0006:  ldarg.0
    IL_0007:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_000c:  ldarg.1
    IL_000d:  ldfld      !0 class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::A@
    IL_0012:  tail.
    IL_0014:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0019:  ret

    IL_001a:  ldc.i4.0
    IL_001b:  ret

    IL_001c:  ldarg.1
    IL_001d:  ldnull
    IL_001e:  cgt.un
    IL_0020:  ldc.i4.0
    IL_0021:  ceq
    IL_0023:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool class '<>f__AnonymousType2396826819`1'<!'<A>j__TPar'>::Equals(class '<>f__AnonymousType2396826819`1'<!0>)
    IL_0013:  ret

    IL_0014:  ldc.i4.0
    IL_0015:  ret
  } 

  .property instance !'<A>j__TPar' A()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<A>j__TPar' '<>f__AnonymousType2396826819`1'::get_A()
  } 
} 






