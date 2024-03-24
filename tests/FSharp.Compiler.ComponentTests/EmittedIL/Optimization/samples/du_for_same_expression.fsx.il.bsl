




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
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed Du_for_same_expression
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!b,!!c> 
          map<a,b,c>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!b> mapping,
                     valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!c> result) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (!!c V_0,
             !!a V_1)
    IL_0000:  ldarga.s   result
    IL_0002:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!c>::get_Tag()
    IL_0007:  ldc.i4.0
    IL_0008:  bne.un.s   IL_000c

    IL_000a:  br.s       IL_001b

    IL_000c:  ldarga.s   result
    IL_000e:  call       instance !1 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!c>::get_ErrorValue()
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!b,!!c>::NewError(!1)
    IL_001a:  ret

    IL_001b:  ldarga.s   result
    IL_001d:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!c>::get_ResultValue()
    IL_0022:  stloc.1
    IL_0023:  ldarg.0
    IL_0024:  ldloc.1
    IL_0025:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!b>::Invoke(!0)
    IL_002a:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!b,!!c>::NewOk(!0)
    IL_002f:  ret
  } 

  .method public static valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,!!b> 
          ff<a,b>(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b> x) cil managed
  {
    
    .maxstack  3
    .locals init (!!a V_0,
             !!a V_1)
    IL_0000:  ldarga.s   x
    IL_0002:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b>::get_Tag()
    IL_0007:  ldc.i4.0
    IL_0008:  bne.un.s   IL_000c

    IL_000a:  br.s       IL_0019

    IL_000c:  ldarga.s   x
    IL_000e:  call       instance !1 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b>::get_ErrorValue()
    IL_0013:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,!!b>::NewError(!1)
    IL_0018:  ret

    IL_0019:  ldarga.s   x
    IL_001b:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b>::get_ResultValue()
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  stloc.1
    IL_0023:  ldloca.s   V_1
    IL_0025:  constrained. !!a
    IL_002b:  callvirt   instance string [runtime]System.Object::ToString()
    IL_0030:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,!!b>::NewOk(!0)
    IL_0035:  ret
  } 

  .method public static valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b> 
          ffs<a,b>(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b> x) cil managed
  {
    
    .maxstack  3
    .locals init (!!a V_0)
    IL_0000:  ldarga.s   x
    IL_0002:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b>::get_Tag()
    IL_0007:  ldc.i4.0
    IL_0008:  bne.un.s   IL_000c

    IL_000a:  br.s       IL_0019

    IL_000c:  ldarga.s   x
    IL_000e:  call       instance !1 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b>::get_ErrorValue()
    IL_0013:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b>::NewError(!1)
    IL_0018:  ret

    IL_0019:  ldarga.s   x
    IL_001b:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b>::get_ResultValue()
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<!!0>(!!0)
    IL_0027:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a,!!b>::NewOk(!0)
    IL_002c:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Du_for_same_expression$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  3
    IL_0000:  ldstr      ""
    IL_0005:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,object>::NewOk(!0)
    IL_000a:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,!!1> Du_for_same_expression::ff<string,object>(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!0,!!1>)
    IL_000f:  box        valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,object>
    IL_0014:  call       void [runtime]System.Console::WriteLine(object)
    IL_0019:  ldstr      ""
    IL_001e:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,object>::NewOk(!0)
    IL_0023:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!0,!!1> Du_for_same_expression::ffs<string,object>(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!0,!!1>)
    IL_0028:  box        valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,object>
    IL_002d:  call       void [runtime]System.Console::WriteLine(object)
    IL_0032:  ldstr      ""
    IL_0037:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<object,string>::NewError(!1)
    IL_003c:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,!!1> Du_for_same_expression::ff<object,string>(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!0,!!1>)
    IL_0041:  box        valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<string,string>
    IL_0046:  call       void [runtime]System.Console::WriteLine(object)
    IL_004b:  ldstr      ""
    IL_0050:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<object,string>::NewError(!1)
    IL_0055:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!0,!!1> Du_for_same_expression::ffs<object,string>(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!0,!!1>)
    IL_005a:  box        valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<object,string>
    IL_005f:  call       void [runtime]System.Console::WriteLine(object)
    IL_0064:  ret
  } 

} 






