




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
  .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .class sequential autochar serializable sealed nested public beforefieldinit Myassembly
         extends [runtime]System.ValueType
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [runtime]System.Object
    {
      .field public static literal int32 A = int32(0x00000000)
      .field public static literal int32 B = int32(0x00000001)
      .field public static literal int32 C = int32(0x00000002)
    } 

    .field assembly string _nonNullableString
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field assembly string _nullableString
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field assembly int32 _tag
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public static valuetype MyTestModule/Myassembly get_A() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  newobj     instance void MyTestModule/Myassembly::.ctor(int32)
      IL_0006:  ret
    } 

    .method public hidebysig instance bool get_IsA() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 MyTestModule/Myassembly::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public static valuetype MyTestModule/Myassembly NewB(string _nonNullableString) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  2
      .locals init (valuetype MyTestModule/Myassembly V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    MyTestModule/Myassembly
      IL_0008:  ldloca.s   V_0
      IL_000a:  ldc.i4.1
      IL_000b:  stfld      int32 MyTestModule/Myassembly::_tag
      IL_0010:  ldloca.s   V_0
      IL_0012:  ldarg.0
      IL_0013:  stfld      string MyTestModule/Myassembly::_nonNullableString
      IL_0018:  ldloc.0
      IL_0019:  ret
    } 

    .method public hidebysig instance bool get_IsB() cil managed
    {
      .custom instance void System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute::.ctor(bool,
                                                                                              string[]) = ( 01 00 01 02 00 00 00 11 6E 6F 6E 4E 75 6C 6C 61   
                                                                                                            62 6C 65 53 74 72 69 6E 67 12 5F 6E 6F 6E 4E 75   
                                                                                                            6C 6C 61 62 6C 65 53 74 72 69 6E 67 00 00 )       
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 MyTestModule/Myassembly::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public static valuetype MyTestModule/Myassembly NewC(string _nullableString) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 02 00 00 00 00 00 ) 
      .param [1]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  2
      .locals init (valuetype MyTestModule/Myassembly V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    MyTestModule/Myassembly
      IL_0008:  ldloca.s   V_0
      IL_000a:  ldc.i4.2
      IL_000b:  stfld      int32 MyTestModule/Myassembly::_tag
      IL_0010:  ldloca.s   V_0
      IL_0012:  ldarg.0
      IL_0013:  stfld      string MyTestModule/Myassembly::_nullableString
      IL_0018:  ldloc.0
      IL_0019:  ret
    } 

    .method public hidebysig instance bool get_IsC() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 MyTestModule/Myassembly::get_Tag()
      IL_0006:  ldc.i4.2
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method assembly specialname rtspecialname instance void  .ctor(int32 _tag) cil managed
    {
      .custom instance void System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 17 4D 79 54 65 73 74 4D 6F 64   
                                                                                                                               75 6C 65 2B 4D 79 53 74 72 75 63 74 44 55 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 MyTestModule/Myassembly::_tag
      IL_0007:  ret
    } 

    .method public hidebysig instance string get_nonNullableString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .param [0]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyTestModule/Myassembly::_nonNullableString
      IL_0006:  ret
    } 

    .method public hidebysig instance string get_nullableString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .param [0]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyTestModule/Myassembly::_nullableString
      IL_0006:  ret
    } 

    .method public hidebysig instance int32 get_Tag() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 MyTestModule/Myassembly::_tag
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname instance object  __DebugDisplay() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype MyTestModule/Myassembly,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype MyTestModule/Myassembly,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  ldobj      MyTestModule/Myassembly
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype MyTestModule/Myassembly,string>::Invoke(!0)
      IL_001a:  ret
    } 

    .method public hidebysig virtual instance string ToString() cil managed
    {
      
      .maxstack  3
      .locals init (valuetype MyTestModule/Myassembly V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldobj      MyTestModule/Myassembly
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  call       instance int32 MyTestModule/Myassembly::get_Tag()
      IL_000d:  switch     ( 
                            IL_001e,
                            IL_0024,
                            IL_002a)
      IL_001e:  ldstr      "A"
      IL_0023:  ret

      IL_0024:  ldstr      "B"
      IL_0029:  ret

      IL_002a:  ldstr      "C"
      IL_002f:  ret
    } 

    .property instance int32 Tag()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 MyTestModule/Myassembly::get_Tag()
    } 
    .property valuetype MyTestModule/Myassembly
            A()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get valuetype MyTestModule/Myassembly MyTestModule/Myassembly::get_A()
    } 
    .property instance bool IsA()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool MyTestModule/Myassembly::get_IsA()
    } 
    .property instance bool IsB()
    {
      .custom instance void System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute::.ctor(bool,
                                                                                              string[]) = ( 01 00 01 02 00 00 00 11 6E 6F 6E 4E 75 6C 6C 61   
                                                                                                            62 6C 65 53 74 72 69 6E 67 12 5F 6E 6F 6E 4E 75   
                                                                                                            6C 6C 61 62 6C 65 53 74 72 69 6E 67 00 00 )       
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool MyTestModule/Myassembly::get_IsB()
    } 
    .property instance bool IsC()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool MyTestModule/Myassembly::get_IsC()
    } 
    .property instance string nonNullableString()
    {
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance string MyTestModule/Myassembly::get_nonNullableString()
    } 
    .property instance string nullableString()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32,
                                                                                                  int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 00 00 00 00 ) 
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .get instance string MyTestModule/Myassembly::get_nullableString()
    } 
  } 

  .method public static string  printMyDu(valuetype MyTestModule/Myassembly x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarga.s   x
    IL_0002:  callvirt   instance string MyTestModule/Myassembly::ToString()
    IL_0007:  ret
  } 

  .method public static string  getVal(valuetype MyTestModule/Myassembly x) cil managed
  {
    .param [0]
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  3
    .locals init (valuetype MyTestModule/Myassembly V_0,
             string V_1,
             string V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance int32 MyTestModule/Myassembly::get_Tag()
    IL_0009:  ldc.i4.2
    IL_000a:  bne.un.s   IL_0016

    IL_000c:  ldloca.s   V_0
    IL_000e:  ldfld      string MyTestModule/Myassembly::_nullableString
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  ret

    IL_0016:  ldstr      "fail"
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  call       class [runtime]System.Exception [FSharp.Core]Microsoft.FSharp.Core.Operators::Failure(string)
    IL_0022:  throw
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyTestModule
       extends [runtime]System.Object
{
} 

.class private auto ansi serializable sealed System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes
       extends [runtime]System.Enum
{
  .custom instance void [runtime]System.FlagsAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes All = int32(0xFFFFFFFF)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes None = int32(0x00000000)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicParameterlessConstructor = int32(0x00000001)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicConstructors = int32(0x00000003)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicConstructors = int32(0x00000004)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicMethods = int32(0x00000008)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicMethods = int32(0x00000010)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicFields = int32(0x00000020)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicFields = int32(0x00000040)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicNestedTypes = int32(0x00000080)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicNestedTypes = int32(0x00000100)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicProperties = int32(0x00000200)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicProperties = int32(0x00000400)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes PublicEvents = int32(0x00000800)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes NonPublicEvents = int32(0x00001000)
  .field public static literal valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes Interfaces = int32(0x00002000)
} 

.class private auto ansi beforefieldinit System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute
       extends [runtime]System.Attribute
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes MemberType@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class [runtime]System.Type Type@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes MemberType, class [runtime]System.Type Type) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::MemberType@
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      class [runtime]System.Type System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::Type@
    IL_0014:  ret
  } 

  .method public hidebysig specialname instance class [runtime]System.Type get_Type() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [runtime]System.Type System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::Type@
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes get_MemberType() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::MemberType@
    IL_0006:  ret
  } 

  .property instance valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes
          MemberType()
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .get instance valuetype System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::get_MemberType()
  } 
  .property instance class [runtime]System.Type
          Type()
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .get instance class [runtime]System.Type System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::get_Type()
  } 
} 

.class private auto ansi beforefieldinit System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute
       extends [runtime]System.Attribute
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private bool ReturnValue@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private string[] Members@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(bool ReturnValue, string[] Members) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      bool System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute::ReturnValue@
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      string[] System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute::Members@
    IL_0014:  ret
  } 

  .method public hidebysig specialname instance string[] get_Members() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string[] System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute::Members@
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance bool get_ReturnValue() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute::ReturnValue@
    IL_0006:  ret
  } 

  .property instance bool ReturnValue()
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .get instance bool System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute::get_ReturnValue()
  } 
  .property instance string[] Members()
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .get instance string[] System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute::get_Members()
  } 
} 

.class private auto ansi beforefieldinit System.Runtime.CompilerServices.NullableAttribute
       extends [runtime]System.Attribute
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint8[] NullableFlags
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(uint8 scalarByteValue) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.1
    IL_0008:  newarr     [runtime]System.Byte
    IL_000d:  dup
    IL_000e:  ldc.i4.0
    IL_000f:  ldarg.1
    IL_0010:  stelem.i1
    IL_0011:  stfld      uint8[] System.Runtime.CompilerServices.NullableAttribute::NullableFlags
    IL_0016:  ret
  } 

  .method public specialname rtspecialname instance void  .ctor(uint8[] NullableFlags) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      uint8[] System.Runtime.CompilerServices.NullableAttribute::NullableFlags
    IL_000d:  ret
  } 

} 

.class private auto ansi beforefieldinit System.Runtime.CompilerServices.NullableContextAttribute
       extends [runtime]System.Attribute
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint8 Flag
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(uint8 Flag) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      uint8 System.Runtime.CompilerServices.NullableContextAttribute::Flag
    IL_000d:  ret
  } 

} 






