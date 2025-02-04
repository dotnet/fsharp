




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
  .class auto ansi serializable nested public beforefieldinit JustStringE
         extends [runtime]System.Exception
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 05 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .field assembly string Data0@
    .method public specialname rtspecialname instance void  .ctor(string data0) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      string MyLibrary/JustStringE::Data0@
      IL_000d:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ret
    } 

    .method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  call       instance void [runtime]System.Exception::.ctor(class [runtime]System.Runtime.Serialization.SerializationInfo,
                                                                                 valuetype [runtime]System.Runtime.Serialization.StreamingContext)
      IL_0008:  ret
    } 

    .method public hidebysig specialname instance string  get_Data0() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyLibrary/JustStringE::Data0@
      IL_0006:  ret
    } 

    .method public strict virtual instance string get_Message() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyLibrary/JustStringE,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class MyLibrary/JustStringE>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyLibrary/JustStringE,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyLibrary/JustStringE,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .property instance string Data0()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance string MyLibrary/JustStringE::get_Data0()
    } 
  } 

  .class auto ansi serializable nested public beforefieldinit TwoStrings
         extends [runtime]System.Exception
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 05 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .field assembly string M1@
    .field assembly string M2@
    .method public specialname rtspecialname instance void  .ctor(string m1, string m2) cil managed
    {
      .param [2]
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      string MyLibrary/TwoStrings::M1@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      string MyLibrary/TwoStrings::M2@
      IL_0014:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ret
    } 

    .method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  call       instance void [runtime]System.Exception::.ctor(class [runtime]System.Runtime.Serialization.SerializationInfo,
                                                                                 valuetype [runtime]System.Runtime.Serialization.StreamingContext)
      IL_0008:  ret
    } 

    .method public hidebysig specialname instance string  get_M1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyLibrary/TwoStrings::M1@
      IL_0006:  ret
    } 

    .method public hidebysig specialname instance string  get_M2() cil managed
    {
      .param [0]
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyLibrary/TwoStrings::M2@
      IL_0006:  ret
    } 

    .method public strict virtual instance string get_Message() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyLibrary/TwoStrings,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class MyLibrary/TwoStrings>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyLibrary/TwoStrings,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyLibrary/TwoStrings,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .property instance string M1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance string MyLibrary/TwoStrings::get_M1()
    } 
    .property instance string M2()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
      .get instance string MyLibrary/TwoStrings::get_M2()
    } 
  } 

  .class auto ansi serializable nested public beforefieldinit NullableStringE
         extends [runtime]System.Exception
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 05 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .field assembly string Data0@
    .method public specialname rtspecialname instance void  .ctor(string data0) cil managed
    {
      .param [1]
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      string MyLibrary/NullableStringE::Data0@
      IL_000d:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ret
    } 

    .method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  call       instance void [runtime]System.Exception::.ctor(class [runtime]System.Runtime.Serialization.SerializationInfo,
                                                                                 valuetype [runtime]System.Runtime.Serialization.StreamingContext)
      IL_0008:  ret
    } 

    .method public hidebysig specialname instance string  get_Data0() cil managed
    {
      .param [0]
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyLibrary/NullableStringE::Data0@
      IL_0006:  ret
    } 

    .method public strict virtual instance string get_Message() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyLibrary/NullableStringE,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class MyLibrary/NullableStringE>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyLibrary/NullableStringE,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class MyLibrary/NullableStringE,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .property instance string Data0()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance string MyLibrary/NullableStringE::get_Data0()
    } 
  } 

  .class auto ansi serializable nested public beforefieldinit NullableMessage
         extends [runtime]System.Exception
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 05 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .field assembly string Message@
    .method public specialname rtspecialname instance void  .ctor(string message) cil managed
    {
      .param [1]
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      string MyLibrary/NullableMessage::Message@
      IL_000d:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Exception::.ctor()
      IL_0006:  ret
    } 

    .method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  ldarg.2
      IL_0003:  call       instance void [runtime]System.Exception::.ctor(class [runtime]System.Runtime.Serialization.SerializationInfo,
                                                                                 valuetype [runtime]System.Runtime.Serialization.StreamingContext)
      IL_0008:  ret
    } 

    .method public hidebysig specialname virtual instance string  get_Message() cil managed
    {
      .param [0]
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyLibrary/NullableMessage::Message@
      IL_0006:  ret
    } 

    .property instance string Message()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
      .get instance string MyLibrary/NullableMessage::get_Message()
    } 
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyLibrary
       extends [runtime]System.Object
{
} 






