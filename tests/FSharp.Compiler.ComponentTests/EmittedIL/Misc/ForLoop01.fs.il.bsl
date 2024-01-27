




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
  .class auto ansi serializable sealed nested assembly beforefieldinit clo@1
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class assembly/clo@1 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 
            Invoke(int32 i) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  ldarg.1
      IL_0004:  add
      IL_0005:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/clo@1::.ctor()
      IL_0005:  stsfld     class assembly/clo@1 assembly/clo@1::@_instance
      IL_000a:  ret
    } 

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
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_5,
             int32 V_6)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  ldc.i4.1
    IL_0005:  stloc.2
    IL_0006:  ldloc.2
    IL_0007:  bge.s      IL_0011

    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000e:  nop
    IL_000f:  br.s       IL_0027

    IL_0011:  ldc.i4.3
    IL_0012:  stloc.3
    IL_0013:  ldloc.3
    IL_0014:  ldc.i4.1
    IL_0015:  stloc.s    V_4
    IL_0017:  ldloc.s    V_4
    IL_0019:  sub
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  ldsfld     class assembly/clo@1 assembly/clo@1::@_instance
    IL_0021:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Initialize<int32>(int32,
                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
    IL_0026:  nop
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_002e:  stloc.s    V_5
    IL_0030:  br.s       IL_005c

    IL_0032:  ldloc.0
    IL_0033:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0038:  stloc.s    V_6
    IL_003a:  ldstr      "%A"
    IL_003f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0044:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0049:  ldloc.s    V_6
    IL_004b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0050:  pop
    IL_0051:  ldloc.s    V_5
    IL_0053:  stloc.0
    IL_0054:  ldloc.0
    IL_0055:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_005a:  stloc.s    V_5
    IL_005c:  ldloc.s    V_5
    IL_005e:  brtrue.s   IL_0032

    IL_0060:  ret
  } 

} 






