




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





.class public abstract auto ansi sealed MyLibrary
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .method public static void  strictlyNotNull(object x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ret
  } 

  .method public static void  myGenericFunction1<class a>(!!a p) cil managed
  {
    .param type a 
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param [1]
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  3
    .locals init (!!a V_0,
             !!a V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  box        !!a
    IL_0008:  brfalse.s  IL_000c

    IL_000a:  br.s       IL_000d

    IL_000c:  ret

    IL_000d:  ldloc.0
    IL_000e:  stloc.1
    IL_000f:  ldloc.1
    IL_0010:  box        !!a
    IL_0015:  call       void MyLibrary::strictlyNotNull(object)
    IL_001a:  ret
  } 

  .method public static void  myGenericFunction2<class a>(!!a p) cil managed
  {
    .param type a 
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param [1]
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  3
    .locals init (!!a V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!a> V_1,
             !!a V_2,
             !!a V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  stloc.2
    IL_0004:  ldloc.2
    IL_0005:  box        !!a
    IL_000a:  brfalse.s  IL_000e

    IL_000c:  br.s       IL_0016

    IL_000e:  ldnull
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!0,!1> class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!a>::NewChoice1Of2(!0)
    IL_0014:  br.s       IL_001c

    IL_0016:  ldloc.2
    IL_0017:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!0,!1> class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!a>::NewChoice2Of2(!1)
    IL_001c:  stloc.1
    IL_001d:  ldloc.1
    IL_001e:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!a>
    IL_0023:  brfalse.s  IL_0027

    IL_0025:  br.s       IL_0028

    IL_0027:  ret

    IL_0028:  ldloc.1
    IL_0029:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!a>
    IL_002e:  call       instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!a>::get_Item()
    IL_0033:  stloc.3
    IL_0034:  ldloc.3
    IL_0035:  box        !!a
    IL_003a:  call       void MyLibrary::strictlyNotNull(object)
    IL_003f:  ret
  } 

  .method public static void  myGenericFunction3<class a>(!!a p) cil managed
  {
    .param type a 
      .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param [1]
    .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  3
    .locals init (!!a V_0,
             !!a V_1,
             !!a V_2,
             !!a V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  box        !!a
    IL_0008:  brfalse.s  IL_000c

    IL_000a:  br.s       IL_000d

    IL_000c:  ret

    IL_000d:  ldloc.0
    IL_000e:  stloc.1
    IL_000f:  ldloc.1
    IL_0010:  stloc.2
    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.3
    IL_0014:  box        !!a
    IL_0019:  call       void MyLibrary::strictlyNotNull(object)
    IL_001e:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyLibrary
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






