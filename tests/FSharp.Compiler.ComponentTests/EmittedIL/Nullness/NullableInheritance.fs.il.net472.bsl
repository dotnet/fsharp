




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
  .class auto ansi serializable nested public DerivedWhichAllowsNull
         extends class [runtime]System.Collections.Generic.List`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 02 00 00 00 01 02 00 00 ) 
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<string>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public hidebysig specialname 
            instance string  get_FirstItem() cil managed
    {
      .param [0]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (class MyTestModule/DerivedWhichAllowsNull V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.0
      IL_0004:  tail.
      IL_0006:  callvirt   instance !0 class [runtime]System.Collections.Generic.List`1<string>::get_Item(int32)
      IL_000b:  ret
    } 

    .property instance string FirstItem()
    {
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
      .get instance string MyTestModule/DerivedWhichAllowsNull::get_FirstItem()
    } 
  } 

  .class auto ansi serializable nested public DerivedWithoutNull
         extends class [runtime]System.Collections.Generic.List`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<string>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public hidebysig specialname 
            instance string  get_FirstItem() cil managed
    {
      .param [0]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (class MyTestModule/DerivedWithoutNull V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.0
      IL_0004:  tail.
      IL_0006:  callvirt   instance !0 class [runtime]System.Collections.Generic.List`1<string>::get_Item(int32)
      IL_000b:  ret
    } 

    .property instance string FirstItem()
    {
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
      .get instance string MyTestModule/DerivedWithoutNull::get_FirstItem()
    } 
  } 

  .class interface abstract auto ansi serializable nested public beforefieldinit ICanGetAnything`1<T>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param type T 
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .method public hidebysig abstract virtual 
            instance !T  Get() cil managed
    {
    } 

  } 

  .class auto ansi serializable nested public MyClassImplementingTheSameInterface
         extends [runtime]System.Object
         implements class MyTestModule/ICanGetAnything`1<class [runtime]System.Collections.Generic.List`1<string>>,
                    class MyTestModule/ICanGetAnything`1<class [runtime]System.Collections.Generic.List`1<class [runtime]System.Collections.Generic.List`1<string>>>,
                    class MyTestModule/ICanGetAnything`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .interfaceimpl type class MyTestModule/ICanGetAnything`1<class [runtime]System.Collections.Generic.List`1<string>>
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 03 00 00 00 01 02 02 00 00 ) 
    .interfaceimpl type class MyTestModule/ICanGetAnything`1<string>
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 02 00 00 00 01 02 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method private hidebysig newslot virtual 
            instance string  'MyTestModule.ICanGetAnything<System.String>.Get'() cil managed
    {
      .param [0]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      .override  method instance !0 class MyTestModule/ICanGetAnything`1<string>::Get()
      
      .maxstack  3
      .locals init (class MyTestModule/MyClassImplementingTheSameInterface V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldnull
      IL_0003:  ret
    } 

    .method private hidebysig newslot virtual 
            instance class [runtime]System.Collections.Generic.List`1<class [runtime]System.Collections.Generic.List`1<string>> 
            'MyTestModule.ICanGetAnything<System.Collections.Generic.List<System.Collections.Generic.List<System.String>>>.Get'() cil managed
    {
      .override  method instance !0 class MyTestModule/ICanGetAnything`1<class [runtime]System.Collections.Generic.List`1<class [runtime]System.Collections.Generic.List`1<string>>>::Get()
      
      .maxstack  3
      .locals init (class MyTestModule/MyClassImplementingTheSameInterface V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class [runtime]System.Collections.Generic.List`1<string>>::.ctor()
      IL_0007:  ret
    } 

    .method private hidebysig newslot virtual 
            instance class [runtime]System.Collections.Generic.List`1<string> 
            'MyTestModule.ICanGetAnything<System.Collections.Generic.List<System.String>>.Get'() cil managed
    {
      .param [0]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      .override  method instance !0 class MyTestModule/ICanGetAnything`1<class [runtime]System.Collections.Generic.List`1<string>>::Get()
      
      .maxstack  3
      .locals init (class MyTestModule/MyClassImplementingTheSameInterface V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldnull
      IL_0003:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyTestModule
       extends [runtime]System.Object
{
} 

.class private auto ansi beforefieldinit System.Runtime.CompilerServices.NullableAttribute
       extends [runtime]System.Attribute
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public uint8[] NullableFlags
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname 
          instance void  .ctor(uint8 scalarByteValue) cil managed
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

  .method public specialname rtspecialname 
          instance void  .ctor(uint8[] NullableFlags) cil managed
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
  .method public specialname rtspecialname 
          instance void  .ctor(uint8 Flag) cil managed
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






