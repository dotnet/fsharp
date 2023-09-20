




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
  .class auto ansi serializable nested public Myassembly
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .field static assembly string uglyGlobalMutableString
    .field static assembly string uglyGlobalMutableNullableString
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .field assembly string Nullable@
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .field assembly string NonNullable@
    .field assembly int32 JustSomeInt@
    .field static assembly int32 init@6
    .method public specialname rtspecialname 
            instance void  .ctor(string x,
                                 string y) cil managed
    {
      .param [1]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      string MyTestModule/Myassembly::Nullable@
      IL_000f:  ldarg.0
      IL_0010:  ldarg.2
      IL_0011:  stfld      string MyTestModule/Myassembly::NonNullable@
      IL_0016:  ldarg.0
      IL_0017:  ldc.i4.s   42
      IL_0019:  stfld      int32 MyTestModule/Myassembly::JustSomeInt@
      IL_001e:  ret
    } 

    .method public hidebysig specialname 
            instance string  get_Nullable() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .param [0]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyTestModule/Myassembly::Nullable@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance string  get_NonNullable() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyTestModule/Myassembly::NonNullable@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_JustSomeInt() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 MyTestModule/Myassembly::JustSomeInt@
      IL_0006:  ret
    } 

    .method public static string  GiveMeNull() cil managed
    {
      .param [0]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ret
    } 

    .method public static string  GiveMeString() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldstr      ""
      IL_0005:  ret
    } 

    .method public hidebysig instance void 
            UnitFunc() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance class MyTestModule/Myassembly 
            GetThis() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ret
    } 

    .method public hidebysig instance class MyTestModule/Myassembly 
            GetThisOrNull() cil managed
    {
      .param [0]
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$MyTestModule::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$MyTestModule::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .property instance string Nullable()
    {
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      .get instance string MyTestModule/Myassembly::get_Nullable()
    } 
    .property instance string NonNullable()
    {
      .get instance string MyTestModule/Myassembly::get_NonNullable()
    } 
    .property instance int32 JustSomeInt()
    {
      .get instance int32 MyTestModule/Myassembly::get_JustSomeInt()
    } 
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyTestModule
       extends [runtime]System.Object
{
  .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      ""
    IL_0005:  stsfld     string MyTestModule/Myassembly::uglyGlobalMutableString
    IL_000a:  ldc.i4.2
    IL_000b:  volatile.
    IL_000d:  stsfld     int32 MyTestModule/Myassembly::init@6
    IL_0012:  ldnull
    IL_0013:  stsfld     string MyTestModule/Myassembly::uglyGlobalMutableNullableString
    IL_0018:  ldc.i4.3
    IL_0019:  volatile.
    IL_001b:  stsfld     int32 MyTestModule/Myassembly::init@6
    IL_0020:  ret
  } 

} 

.class private auto ansi beforefieldinit System.Runtime.CompilerServices.NullableAttribute
       extends [runtime]System.Attribute
{
  .field public class [runtime]System.Array<uint8> NullableFlags
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname 
          instance void  .ctor(class [runtime]System.Array<uint8> NullableFlags) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      class [runtime]System.Array<uint8> System.Runtime.CompilerServices.NullableAttribute::NullableFlags
    IL_000d:  ret
  } 

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
    IL_0011:  stfld      class [runtime]System.Array<uint8> System.Runtime.CompilerServices.NullableAttribute::NullableFlags
    IL_0016:  ret
  } 

} 

.class private auto ansi beforefieldinit System.Runtime.CompilerServices.NullableContextAttribute
       extends [runtime]System.Attribute
{
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






