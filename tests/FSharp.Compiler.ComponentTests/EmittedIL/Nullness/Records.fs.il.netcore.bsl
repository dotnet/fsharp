




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly assembly
{
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed MyTestModule
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .class auto ansi serializable sealed nested public beforefieldinit MyRecord`3<X,class Y,Z>
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param type X 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param type Y 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .param type Z 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .field assembly int32 JustInt@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly valuetype [runtime]System.Nullable`1<int32> NullInt@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly string JustString@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly string NullableString@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .field assembly !X GenericNormalField@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly !Y GenericNullableField@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field assembly !Z GenericNotNullField@
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname instance int32  get_JustInt() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class MyTestModule/MyRecord`3<!X,!Y,!Z>::JustInt@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance valuetype [runtime]System.Nullable`1<int32> 
            get_NullInt() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [runtime]System.Nullable`1<int32> class MyTestModule/MyRecord`3<!X,!Y,!Z>::NullInt@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance string  get_JustString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string class MyTestModule/MyRecord`3<!X,!Y,!Z>::JustString@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance string  get_NullableString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string class MyTestModule/MyRecord`3<!X,!Y,!Z>::NullableString@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance !X  get_GenericNormalField() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class MyTestModule/MyRecord`3<!X,!Y,!Z>::GenericNormalField@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance !Y  get_GenericNullableField() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1 class MyTestModule/MyRecord`3<!X,!Y,!Z>::GenericNullableField@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance !Z  get_GenericNotNullField() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !2 class MyTestModule/MyRecord`3<!X,!Y,!Z>::GenericNotNullField@
      IL_0006:  ret
    } 

    .method public specialname rtspecialname 
            instance void  .ctor(int32 justInt,
                                 valuetype [runtime]System.Nullable`1<int32> nullInt,
                                 string justString,
                                 string nullableString,
                                 !X genericNormalField,
                                 !Y genericNullableField,
                                 !Z genericNotNullField) cil managed
    {
      .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 17 4D 79 54 65 73 74 4D 6F 64   
                                                                                                                                                     75 6C 65 2B 4D 79 52 65 63 6F 72 64 60 33 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 class MyTestModule/MyRecord`3<!X,!Y,!Z>::JustInt@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      valuetype [runtime]System.Nullable`1<int32> class MyTestModule/MyRecord`3<!X,!Y,!Z>::NullInt@
      IL_0014:  ldarg.0
      IL_0015:  ldarg.3
      IL_0016:  stfld      string class MyTestModule/MyRecord`3<!X,!Y,!Z>::JustString@
      IL_001b:  ldarg.0
      IL_001c:  ldarg.s    nullableString
      IL_001e:  stfld      string class MyTestModule/MyRecord`3<!X,!Y,!Z>::NullableString@
      IL_0023:  ldarg.0
      IL_0024:  ldarg.s    genericNormalField
      IL_0026:  stfld      !0 class MyTestModule/MyRecord`3<!X,!Y,!Z>::GenericNormalField@
      IL_002b:  ldarg.0
      IL_002c:  ldarg.s    genericNullableField
      IL_002e:  stfld      !1 class MyTestModule/MyRecord`3<!X,!Y,!Z>::GenericNullableField@
      IL_0033:  ldarg.0
      IL_0034:  ldarg.s    genericNotNullField
      IL_0036:  stfld      !2 class MyTestModule/MyRecord`3<!X,!Y,!Z>::GenericNotNullField@
      IL_003b:  ret
    } 

    .method public strict virtual instance string ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyTestModule/MyRecord`3<!X,!Y,!Z>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class MyTestModule/MyRecord`3<!X,!Y,!Z>>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyTestModule/MyRecord`3<!X,!Y,!Z>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyTestModule/MyRecord`3<!X,!Y,!Z>,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .property instance int32 JustInt()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance int32 MyTestModule/MyRecord`3::get_JustInt()
    } 
    .property instance valuetype [runtime]System.Nullable`1<int32>
            NullInt()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance valuetype [runtime]System.Nullable`1<int32> MyTestModule/MyRecord`3::get_NullInt()
    } 
    .property instance string JustString()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 ) 
      .get instance string MyTestModule/MyRecord`3::get_JustString()
    } 
    .property instance string NullableString()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 ) 
      .get instance string MyTestModule/MyRecord`3::get_NullableString()
    } 
    .property instance !X GenericNormalField()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 04 00 00 00 00 00 ) 
      .get instance !X MyTestModule/MyRecord`3::get_GenericNormalField()
    } 
    .property instance !Y GenericNullableField()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 05 00 00 00 00 00 ) 
      .get instance !Y MyTestModule/MyRecord`3::get_GenericNullableField()
    } 
    .property instance !Z GenericNotNullField()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 06 00 00 00 00 00 ) 
      .get instance !Z MyTestModule/MyRecord`3::get_GenericNotNullField()
    } 
  } 

  .method public specialname static string get_maybeString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } 

  .method public static class MyTestModule/MyRecord`3<int32,string,string> createAnInstance() cil managed
  {
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 03 00 00 00 01 02 01 00 00 ) 
    
    .maxstack  9
    IL_0000:  ldc.i4.s   42
    IL_0002:  ldc.i4.s   42
    IL_0004:  call       valuetype [runtime]System.Nullable`1<!0> valuetype [runtime]System.Nullable`1<int32>::op_Implicit(!0)
    IL_0009:  ldstr      ""
    IL_000e:  ldnull
    IL_000f:  ldc.i4.s   42
    IL_0011:  call       string MyTestModule::get_maybeString()
    IL_0016:  ldstr      ""
    IL_001b:  newobj     instance void class MyTestModule/MyRecord`3<int32,string,string>::.ctor(int32,
                                                                                                 valuetype [runtime]System.Nullable`1<int32>,
                                                                                                 string,
                                                                                                 string,
                                                                                                 !0,
                                                                                                 !1,
                                                                                                 !2)
    IL_0020:  ret
  } 

  .property string maybeString()
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .get string MyTestModule::get_maybeString()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyTestModule
       extends [runtime]System.Object
{
} 






