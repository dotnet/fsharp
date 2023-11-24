




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





.class public abstract auto ansi sealed Lambda_inlining
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit fold@14<a,b>
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!b,bool>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!b,!a>> f
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!a> s
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!b,!a>> f,
                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!a> s) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!b,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!0>> class Lambda_inlining/fold@14<!a,!b>::f
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!0> class Lambda_inlining/fold@14<!a,!b>::s
      IL_0014:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(!b v) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!0> class Lambda_inlining/fold@14<!a,!b>::s
      IL_0006:  ldarg.0
      IL_0007:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!0>> class Lambda_inlining/fold@14<!a,!b>::f
      IL_000c:  ldarg.0
      IL_000d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!0> class Lambda_inlining/fold@14<!a,!b>::s
      IL_0012:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!a>::get_contents()
      IL_0017:  ldarg.1
      IL_0018:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!a,!b>::InvokeFast<!0>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                            !0,
                                                                                                            !1)
      IL_001d:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!a>::set_contents(!0)
      IL_0022:  ldc.i4.1
      IL_0023:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit thisIsNotInlined1@24
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool>
  {
    .field public int32[] vs
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(int32[] vs) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32[] Lambda_inlining/thisIsNotInlined1@24::vs
      IL_000d:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> r) cil managed
    {
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_0008

      IL_0004:  ldloc.0
      IL_0005:  ldc.i4.1
      IL_0006:  add
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32[] Lambda_inlining/thisIsNotInlined1@24::vs
      IL_000f:  ldlen
      IL_0010:  conv.i4
      IL_0011:  bge.s      IL_0027

      IL_0013:  ldarg.1
      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32[] Lambda_inlining/thisIsNotInlined1@24::vs
      IL_001a:  ldloc.0
      IL_001b:  ldelem     [runtime]System.Int32
      IL_0020:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::Invoke(!0)
      IL_0025:  br.s       IL_0028

      IL_0027:  ldc.i4.0
      IL_0028:  brtrue.s   IL_0004

      IL_002a:  ldloc.0
      IL_002b:  ldarg.0
      IL_002c:  ldfld      int32[] Lambda_inlining/thisIsNotInlined1@24::vs
      IL_0031:  ldlen
      IL_0032:  conv.i4
      IL_0033:  ceq
      IL_0035:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'thisIsNotInlined1@24-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> s
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> s) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Lambda_inlining/'thisIsNotInlined1@24-1'::s
      IL_000d:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Lambda_inlining/'thisIsNotInlined1@24-1'::s
      IL_0006:  ldarg.0
      IL_0007:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Lambda_inlining/'thisIsNotInlined1@24-1'::s
      IL_000c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0011:  ldarg.1
      IL_0012:  add
      IL_0013:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
      IL_0018:  ldc.i4.1
      IL_0019:  ret
    } 

  } 

  .class auto ansi serializable nested public Test
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32[] _values
    .field static assembly int32[] _svalues
    .field static assembly int32 init@26
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldc.i4.0
      IL_000a:  ldc.i4.1
      IL_000b:  ldc.i4     0x2710
      IL_0010:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                             int32,
                                                                                                                                                                             int32)
      IL_0015:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_001a:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_001f:  stfld      int32[] Lambda_inlining/Test::_values
      IL_0024:  ret
    } 

    .method public hidebysig instance int32 
            thisIsInlined2() cil managed
    {
      
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.0
      IL_0003:  stloc.1
      IL_0004:  br.s       IL_000a

      IL_0006:  ldloc.1
      IL_0007:  ldc.i4.1
      IL_0008:  add
      IL_0009:  stloc.1
      IL_000a:  ldloc.1
      IL_000b:  call       int32[] Lambda_inlining::get_values()
      IL_0010:  ldlen
      IL_0011:  conv.i4
      IL_0012:  bge.s      IL_0027

      IL_0014:  call       int32[] Lambda_inlining::get_values()
      IL_0019:  ldloc.1
      IL_001a:  ldelem     [runtime]System.Int32
      IL_001f:  stloc.2
      IL_0020:  ldloc.0
      IL_0021:  ldloc.2
      IL_0022:  add
      IL_0023:  stloc.0
      IL_0024:  ldc.i4.1
      IL_0025:  br.s       IL_0028

      IL_0027:  ldc.i4.0
      IL_0028:  brtrue.s   IL_0006

      IL_002a:  ldloc.1
      IL_002b:  call       int32[] Lambda_inlining::get_values()
      IL_0030:  ldlen
      IL_0031:  conv.i4
      IL_0032:  ceq
      IL_0034:  pop
      IL_0035:  ldloc.0
      IL_0036:  ret
    } 

    .method public hidebysig instance int32 
            thisIsInlined3() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32[] Lambda_inlining/Test::_values
      IL_0007:  callvirt   instance int32 Lambda_inlining/Test::'array'(int32[])
      IL_000c:  ret
    } 

    .method public hidebysig instance int32 
            thisIsNotInlined2() cil managed
    {
      
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool> V_0,
               int32[] V_1,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32[] Lambda_inlining/Test::_values
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  newobj     instance void Lambda_inlining/thisIsNotInlined2@35::.ctor(int32[])
      IL_000d:  stloc.0
      IL_000e:  ldc.i4.0
      IL_000f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
      IL_0014:  stloc.2
      IL_0015:  ldloc.0
      IL_0016:  ldloc.2
      IL_0017:  newobj     instance void Lambda_inlining/'thisIsNotInlined2@35-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_001c:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool>::Invoke(!0)
      IL_0021:  pop
      IL_0022:  ldloc.2
      IL_0023:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0028:  ret
    } 

    .method public hidebysig instance int32 
            thisIsNotInlined3() cil managed
    {
      
      .maxstack  4
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool> V_0,
               int32[] V_1,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_2)
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 Lambda_inlining/Test::init@26
      IL_0007:  ldc.i4.2
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32[] Lambda_inlining/Test::_svalues
      IL_0016:  stloc.1
      IL_0017:  ldloc.1
      IL_0018:  newobj     instance void Lambda_inlining/thisIsNotInlined3@36::.ctor(int32[])
      IL_001d:  stloc.0
      IL_001e:  ldc.i4.0
      IL_001f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
      IL_0024:  stloc.2
      IL_0025:  ldloc.0
      IL_0026:  ldloc.2
      IL_0027:  newobj     instance void Lambda_inlining/'thisIsNotInlined3@36-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
      IL_002c:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool>::Invoke(!0)
      IL_0031:  pop
      IL_0032:  ldloc.2
      IL_0033:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0038:  ret
    } 

    .method assembly hidebysig instance int32 
            'array'(int32[] vs) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.0
      IL_0003:  stloc.1
      IL_0004:  br.s       IL_000a

      IL_0006:  ldloc.1
      IL_0007:  ldc.i4.1
      IL_0008:  add
      IL_0009:  stloc.1
      IL_000a:  ldloc.1
      IL_000b:  ldarg.1
      IL_000c:  ldlen
      IL_000d:  conv.i4
      IL_000e:  bge.s      IL_001f

      IL_0010:  ldarg.1
      IL_0011:  ldloc.1
      IL_0012:  ldelem     [runtime]System.Int32
      IL_0017:  stloc.2
      IL_0018:  ldloc.0
      IL_0019:  ldloc.2
      IL_001a:  add
      IL_001b:  stloc.0
      IL_001c:  ldc.i4.1
      IL_001d:  br.s       IL_0020

      IL_001f:  ldc.i4.0
      IL_0020:  brtrue.s   IL_0006

      IL_0022:  ldloc.1
      IL_0023:  ldarg.1
      IL_0024:  ldlen
      IL_0025:  conv.i4
      IL_0026:  ceq
      IL_0028:  pop
      IL_0029:  ldloc.0
      IL_002a:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Lambda_inlining$fsx::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Lambda_inlining$fsx::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit thisIsNotInlined2@35
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool>
  {
    .field public int32[] vs
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(int32[] vs) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32[] Lambda_inlining/thisIsNotInlined2@35::vs
      IL_000d:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> r) cil managed
    {
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_0008

      IL_0004:  ldloc.0
      IL_0005:  ldc.i4.1
      IL_0006:  add
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32[] Lambda_inlining/thisIsNotInlined2@35::vs
      IL_000f:  ldlen
      IL_0010:  conv.i4
      IL_0011:  bge.s      IL_0027

      IL_0013:  ldarg.1
      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32[] Lambda_inlining/thisIsNotInlined2@35::vs
      IL_001a:  ldloc.0
      IL_001b:  ldelem     [runtime]System.Int32
      IL_0020:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::Invoke(!0)
      IL_0025:  br.s       IL_0028

      IL_0027:  ldc.i4.0
      IL_0028:  brtrue.s   IL_0004

      IL_002a:  ldloc.0
      IL_002b:  ldarg.0
      IL_002c:  ldfld      int32[] Lambda_inlining/thisIsNotInlined2@35::vs
      IL_0031:  ldlen
      IL_0032:  conv.i4
      IL_0033:  ceq
      IL_0035:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'thisIsNotInlined2@35-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> s
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> s) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Lambda_inlining/'thisIsNotInlined2@35-1'::s
      IL_000d:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Lambda_inlining/'thisIsNotInlined2@35-1'::s
      IL_0006:  ldarg.0
      IL_0007:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Lambda_inlining/'thisIsNotInlined2@35-1'::s
      IL_000c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0011:  ldarg.1
      IL_0012:  add
      IL_0013:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
      IL_0018:  ldc.i4.1
      IL_0019:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit thisIsNotInlined3@36
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool>
  {
    .field public int32[] vs
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(int32[] vs) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32[] Lambda_inlining/thisIsNotInlined3@36::vs
      IL_000d:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> r) cil managed
    {
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_0008

      IL_0004:  ldloc.0
      IL_0005:  ldc.i4.1
      IL_0006:  add
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32[] Lambda_inlining/thisIsNotInlined3@36::vs
      IL_000f:  ldlen
      IL_0010:  conv.i4
      IL_0011:  bge.s      IL_0027

      IL_0013:  ldarg.1
      IL_0014:  ldarg.0
      IL_0015:  ldfld      int32[] Lambda_inlining/thisIsNotInlined3@36::vs
      IL_001a:  ldloc.0
      IL_001b:  ldelem     [runtime]System.Int32
      IL_0020:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::Invoke(!0)
      IL_0025:  br.s       IL_0028

      IL_0027:  ldc.i4.0
      IL_0028:  brtrue.s   IL_0004

      IL_002a:  ldloc.0
      IL_002b:  ldarg.0
      IL_002c:  ldfld      int32[] Lambda_inlining/thisIsNotInlined3@36::vs
      IL_0031:  ldlen
      IL_0032:  conv.i4
      IL_0033:  ceq
      IL_0035:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'thisIsNotInlined3@36-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> s
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> s) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Lambda_inlining/'thisIsNotInlined3@36-1'::s
      IL_000d:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Lambda_inlining/'thisIsNotInlined3@36-1'::s
      IL_0006:  ldarg.0
      IL_0007:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> Lambda_inlining/'thisIsNotInlined3@36-1'::s
      IL_000c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
      IL_0011:  ldarg.1
      IL_0012:  add
      IL_0013:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
      IL_0018:  ldc.i4.1
      IL_0019:  ret
    } 

  } 

  .method public static bool  ofArray<a>(!!a[] vs,
                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,bool> r) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    .param [2]
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.InlineIfLambdaAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0008

    IL_0004:  ldloc.0
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  ldlen
    IL_000b:  conv.i4
    IL_000c:  bge.s      IL_001d

    IL_000e:  ldarg.1
    IL_000f:  ldarg.0
    IL_0010:  ldloc.0
    IL_0011:  ldelem     !!a
    IL_0016:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,bool>::Invoke(!0)
    IL_001b:  br.s       IL_001e

    IL_001d:  ldc.i4.0
    IL_001e:  brtrue.s   IL_0004

    IL_0020:  ldloc.0
    IL_0021:  ldarg.0
    IL_0022:  ldlen
    IL_0023:  conv.i4
    IL_0024:  ceq
    IL_0026:  ret
  } 

  .method public static !!a  fold<a,b>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!b,!!a>> f,
                                       !!a z,
                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!b,bool>,bool> ps) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    .param [1]
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.InlineIfLambdaAttribute::.ctor() = ( 01 00 00 00 ) 
    .param [3]
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.InlineIfLambdaAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!a> V_0)
    IL_0000:  ldarg.1
    IL_0001:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!a>::.ctor(!0)
    IL_0006:  stloc.0
    IL_0007:  ldarg.2
    IL_0008:  ldarg.0
    IL_0009:  ldloc.0
    IL_000a:  newobj     instance void class Lambda_inlining/fold@14<!!a,!!b>::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!0>>,
                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!0>)
    IL_000f:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!b,bool>,bool>::Invoke(!0)
    IL_0014:  pop
    IL_0015:  ldloc.0
    IL_0016:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!a>::get_contents()
    IL_001b:  ret
  } 

  .method public specialname static int32[] 
          get_values() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] '<StartupCode$assembly>'.$Lambda_inlining$fsx::values@17
    IL_0005:  ret
  } 

  .method public static int32  thisIsInlined1() cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_000a

    IL_0006:  ldloc.1
    IL_0007:  ldc.i4.1
    IL_0008:  add
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  call       int32[] Lambda_inlining::get_values()
    IL_0010:  ldlen
    IL_0011:  conv.i4
    IL_0012:  bge.s      IL_0027

    IL_0014:  call       int32[] Lambda_inlining::get_values()
    IL_0019:  ldloc.1
    IL_001a:  ldelem     [runtime]System.Int32
    IL_001f:  stloc.2
    IL_0020:  ldloc.0
    IL_0021:  ldloc.2
    IL_0022:  add
    IL_0023:  stloc.0
    IL_0024:  ldc.i4.1
    IL_0025:  br.s       IL_0028

    IL_0027:  ldc.i4.0
    IL_0028:  brtrue.s   IL_0006

    IL_002a:  ldloc.1
    IL_002b:  call       int32[] Lambda_inlining::get_values()
    IL_0030:  ldlen
    IL_0031:  conv.i4
    IL_0032:  ceq
    IL_0034:  pop
    IL_0035:  ldloc.0
    IL_0036:  ret
  } 

  .method public static int32  thisIsInlined2() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4     0x2710
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0011:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0016:  stloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  stloc.1
    IL_0019:  ldc.i4.0
    IL_001a:  stloc.2
    IL_001b:  br.s       IL_0021

    IL_001d:  ldloc.2
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.2
    IL_0021:  ldloc.2
    IL_0022:  ldloc.0
    IL_0023:  ldlen
    IL_0024:  conv.i4
    IL_0025:  bge.s      IL_0036

    IL_0027:  ldloc.0
    IL_0028:  ldloc.2
    IL_0029:  ldelem     [runtime]System.Int32
    IL_002e:  stloc.3
    IL_002f:  ldloc.1
    IL_0030:  ldloc.3
    IL_0031:  add
    IL_0032:  stloc.1
    IL_0033:  ldc.i4.1
    IL_0034:  br.s       IL_0037

    IL_0036:  ldc.i4.0
    IL_0037:  brtrue.s   IL_001d

    IL_0039:  ldloc.2
    IL_003a:  ldloc.0
    IL_003b:  ldlen
    IL_003c:  conv.i4
    IL_003d:  ceq
    IL_003f:  pop
    IL_0040:  ldloc.1
    IL_0041:  ret
  } 

  .method public static int32  thisIsNotInlined1() cil managed
  {
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool> V_0,
             int32[] V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4     0x2710
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0011:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0016:  stloc.1
    IL_0017:  ldloc.1
    IL_0018:  newobj     instance void Lambda_inlining/thisIsNotInlined1@24::.ctor(int32[])
    IL_001d:  stloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  ldloc.0
    IL_0026:  ldloc.2
    IL_0027:  newobj     instance void Lambda_inlining/'thisIsNotInlined1@24-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
    IL_002c:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,bool>::Invoke(!0)
    IL_0031:  pop
    IL_0032:  ldloc.2
    IL_0033:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
    IL_0038:  ret
  } 

  .property int32[] values()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] Lambda_inlining::get_values()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Lambda_inlining$fsx
       extends [runtime]System.Object
{
  .field static assembly initonly int32[] values@17
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4     0x2710
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0011:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0016:  stsfld     int32[] '<StartupCode$assembly>'.$Lambda_inlining$fsx::values@17
    IL_001b:  ldc.i4.0
    IL_001c:  ldc.i4.1
    IL_001d:  ldc.i4     0x2710
    IL_0022:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0027:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_002c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0031:  stsfld     int32[] Lambda_inlining/Test::_svalues
    IL_0036:  ldc.i4.2
    IL_0037:  volatile.
    IL_0039:  stsfld     int32 Lambda_inlining/Test::init@26
    IL_003e:  ret
  } 

} 





