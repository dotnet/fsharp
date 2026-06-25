




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
  .class auto ansi serializable sealed nested assembly beforefieldinit f33@47
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/f33@47 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/f33@47::.ctor()
      IL_0005:  stsfld     class assembly/f33@47 assembly/f33@47::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f33@47-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f33@47-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f33@47-1'::.ctor()
      IL_0005:  stsfld     class assembly/'f33@47-1' assembly/'f33@47-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f34@48
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/f34@48 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/f34@48::.ctor()
      IL_0005:  stsfld     class assembly/f34@48 assembly/f34@48::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f34@48-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/'f34@48-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f34@48-2'::.ctor()
      IL_0005:  stsfld     class assembly/'f34@48-2' assembly/'f34@48-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f34@48-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f34@48-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f34@48-1'::.ctor()
      IL_0005:  stsfld     class assembly/'f34@48-1' assembly/'f34@48-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f34@48-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f34@48-3' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f34@48-3'::.ctor()
      IL_0005:  stsfld     class assembly/'f34@48-3' assembly/'f34@48-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f35@49
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/f35@49 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/f35@49::.ctor()
      IL_0005:  stsfld     class assembly/f35@49 assembly/f35@49::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f35@49-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/'f35@49-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f35@49-2'::.ctor()
      IL_0005:  stsfld     class assembly/'f35@49-2' assembly/'f35@49-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f35@49-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f35@49-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f35@49-1'::.ctor()
      IL_0005:  stsfld     class assembly/'f35@49-1' assembly/'f35@49-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f35@49-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f35@49-3' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f35@49-3'::.ctor()
      IL_0005:  stsfld     class assembly/'f35@49-3' assembly/'f35@49-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .method public static int32[]  f0(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f) cil managed
  {
    
    .maxstack  6
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0039

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  ldarg.0
    IL_001f:  ldnull
    IL_0020:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0025:  pop
    IL_0026:  stloc.s    V_6
    IL_0028:  stloc.s    V_7
    IL_002a:  ldloc.s    V_7
    IL_002c:  ldloc.s    V_6
    IL_002e:  ldloc.3
    IL_002f:  stelem.i4
    IL_0030:  ldloc.2
    IL_0031:  ldc.i4.1
    IL_0032:  add
    IL_0033:  stloc.2
    IL_0034:  ldloc.1
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.1
    IL_0039:  ldloc.1
    IL_003a:  ldc.i4.s   10
    IL_003c:  conv.i8
    IL_003d:  blt.un.s   IL_0011

    IL_003f:  ldloc.0
    IL_0040:  ret
  } 

  .method public static int32[]  f00(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5,
             native int V_6,
             int32[] V_7,
             native int V_8,
             int32[] V_9)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0049

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  ldarg.0
    IL_001f:  ldnull
    IL_0020:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0025:  pop
    IL_0026:  stloc.s    V_6
    IL_0028:  stloc.s    V_7
    IL_002a:  ldloc.s    V_7
    IL_002c:  ldloc.s    V_6
    IL_002e:  ldarg.1
    IL_002f:  ldnull
    IL_0030:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0035:  pop
    IL_0036:  stloc.s    V_8
    IL_0038:  stloc.s    V_9
    IL_003a:  ldloc.s    V_9
    IL_003c:  ldloc.s    V_8
    IL_003e:  ldloc.3
    IL_003f:  stelem.i4
    IL_0040:  ldloc.2
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.2
    IL_0044:  ldloc.1
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.s   10
    IL_004c:  conv.i8
    IL_004d:  blt.un.s   IL_0011

    IL_004f:  ldloc.0
    IL_0050:  ret
  } 

  .method public static int32[]  f000(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f) cil managed
  {
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000a:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  br.s       IL_0035

      IL_0012:  ldloc.1
      IL_0013:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0018:  stloc.3
      IL_0019:  ldarg.0
      IL_001a:  ldnull
      IL_001b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0020:  pop
      IL_0021:  ldloca.s   V_0
      IL_0023:  ldloc.3
      IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>::Add(!0)
      IL_0029:  nop
      IL_002a:  ldloca.s   V_0
      IL_002c:  ldloc.3
      IL_002d:  ldc.i4.1
      IL_002e:  add
      IL_002f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>::Add(!0)
      IL_0034:  nop
      IL_0035:  ldloc.1
      IL_0036:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003b:  brtrue.s   IL_0012

      IL_003d:  ldnull
      IL_003e:  stloc.2
      IL_003f:  leave.s    IL_0056

    }  
    finally
    {
      IL_0041:  ldloc.1
      IL_0042:  isinst     [runtime]System.IDisposable
      IL_0047:  stloc.s    V_4
      IL_0049:  ldloc.s    V_4
      IL_004b:  brfalse.s  IL_0055

      IL_004d:  ldloc.s    V_4
      IL_004f:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0054:  endfinally
      IL_0055:  endfinally
    }  
    IL_0056:  ldloc.2
    IL_0057:  pop
    IL_0058:  ldloca.s   V_0
    IL_005a:  call       instance !0[] valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>::Close()
    IL_005f:  ret
  } 

  .method public static int32[]  f0000() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0029

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  ldloc.3
    IL_001f:  stelem.i4
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.s   10
    IL_002c:  conv.i8
    IL_002d:  blt.un.s   IL_0011

    IL_002f:  ldloc.0
    IL_0030:  ret
  } 

  .method public static int32[]  f00000() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0029

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  ldloc.3
    IL_001f:  stelem.i4
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.s   10
    IL_002c:  conv.i8
    IL_002d:  blt.un.s   IL_0011

    IL_002f:  ldloc.0
    IL_0030:  ret
  } 

  .method public static int32[]  f000000() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0031

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  stloc.s    V_6
    IL_0020:  stloc.s    V_7
    IL_0022:  ldloc.s    V_7
    IL_0024:  ldloc.s    V_6
    IL_0026:  ldloc.3
    IL_0027:  stelem.i4
    IL_0028:  ldloc.2
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  conv.i8
    IL_002f:  add
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  ldc.i4.s   10
    IL_0034:  conv.i8
    IL_0035:  blt.un.s   IL_0011

    IL_0037:  ldloc.0
    IL_0038:  ret
  } 

  .method public static int32[]  f0000000() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0031

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  stloc.s    V_6
    IL_0020:  stloc.s    V_7
    IL_0022:  ldloc.s    V_7
    IL_0024:  ldloc.s    V_6
    IL_0026:  ldloc.3
    IL_0027:  stelem.i4
    IL_0028:  ldloc.2
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  conv.i8
    IL_002f:  add
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  ldc.i4.s   10
    IL_0034:  conv.i8
    IL_0035:  blt.un.s   IL_0011

    IL_0037:  ldloc.0
    IL_0038:  ret
  } 

  .method public static int32[]  f00000000() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5,
             native int V_6,
             int32[] V_7,
             native int V_8,
             int32[] V_9)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0039

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  stloc.s    V_6
    IL_0020:  stloc.s    V_7
    IL_0022:  ldloc.s    V_7
    IL_0024:  ldloc.s    V_6
    IL_0026:  stloc.s    V_8
    IL_0028:  stloc.s    V_9
    IL_002a:  ldloc.s    V_9
    IL_002c:  ldloc.s    V_8
    IL_002e:  ldloc.3
    IL_002f:  stelem.i4
    IL_0030:  ldloc.2
    IL_0031:  ldc.i4.1
    IL_0032:  add
    IL_0033:  stloc.2
    IL_0034:  ldloc.1
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.1
    IL_0039:  ldloc.1
    IL_003a:  ldc.i4.s   10
    IL_003c:  conv.i8
    IL_003d:  blt.un.s   IL_0011

    IL_003f:  ldloc.0
    IL_0040:  ret
  } 

  .method public static int32[]  f000000000(int32 x,
                                            int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             native int V_5,
             int32[] V_6,
             int32 V_7,
             native int V_8,
             int32[] V_9,
             native int V_10,
             int32[] V_11)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0049

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_5
    IL_0018:  stloc.s    V_6
    IL_001a:  ldloc.s    V_6
    IL_001c:  ldloc.s    V_5
    IL_001e:  ldloc.3
    IL_001f:  ldarg.0
    IL_0020:  add
    IL_0021:  stloc.s    V_4
    IL_0023:  stloc.s    V_8
    IL_0025:  stloc.s    V_9
    IL_0027:  ldloc.s    V_9
    IL_0029:  ldloc.s    V_8
    IL_002b:  ldloc.3
    IL_002c:  ldarg.1
    IL_002d:  add
    IL_002e:  stloc.s    V_7
    IL_0030:  stloc.s    V_10
    IL_0032:  stloc.s    V_11
    IL_0034:  ldloc.s    V_11
    IL_0036:  ldloc.s    V_10
    IL_0038:  ldloc.3
    IL_0039:  ldloc.s    V_4
    IL_003b:  add
    IL_003c:  ldloc.s    V_7
    IL_003e:  add
    IL_003f:  stelem.i4
    IL_0040:  ldloc.2
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.2
    IL_0044:  ldloc.1
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.s   10
    IL_004c:  conv.i8
    IL_004d:  blt.un.s   IL_0011

    IL_004f:  ldloc.0
    IL_0050:  ret
  } 

  .method public static int32[]  f0000000000(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5,
             native int V_6,
             int32[] V_7,
             native int V_8,
             int32[] V_9)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0049

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  ldarg.0
    IL_001f:  ldnull
    IL_0020:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0025:  pop
    IL_0026:  stloc.s    V_6
    IL_0028:  stloc.s    V_7
    IL_002a:  ldloc.s    V_7
    IL_002c:  ldloc.s    V_6
    IL_002e:  ldarg.1
    IL_002f:  ldnull
    IL_0030:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0035:  pop
    IL_0036:  stloc.s    V_8
    IL_0038:  stloc.s    V_9
    IL_003a:  ldloc.s    V_9
    IL_003c:  ldloc.s    V_8
    IL_003e:  ldloc.3
    IL_003f:  stelem.i4
    IL_0040:  ldloc.2
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.2
    IL_0044:  ldloc.1
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.s   10
    IL_004c:  conv.i8
    IL_004d:  blt.un.s   IL_0011

    IL_004f:  ldloc.0
    IL_0050:  ret
  } 

  .method public static int32[]  f00000000000(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>& V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>& V_6,
             class [runtime]System.IDisposable V_7)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000a:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  br.s       IL_004c

      IL_0012:  ldloc.1
      IL_0013:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0018:  stloc.3
      IL_0019:  ldloca.s   V_0
      IL_001b:  stloc.s    V_4
      IL_001d:  ldloc.s    V_4
      IL_001f:  ldarg.0
      IL_0020:  ldnull
      IL_0021:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0026:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>::Add(!0)
      IL_002b:  nop
      IL_002c:  ldloca.s   V_0
      IL_002e:  stloc.s    V_5
      IL_0030:  ldloc.s    V_5
      IL_0032:  ldarg.1
      IL_0033:  ldnull
      IL_0034:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0039:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>::Add(!0)
      IL_003e:  nop
      IL_003f:  ldloca.s   V_0
      IL_0041:  stloc.s    V_6
      IL_0043:  ldloc.s    V_6
      IL_0045:  ldloc.3
      IL_0046:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>::Add(!0)
      IL_004b:  nop
      IL_004c:  ldloc.1
      IL_004d:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0052:  brtrue.s   IL_0012

      IL_0054:  ldnull
      IL_0055:  stloc.2
      IL_0056:  leave.s    IL_006d

    }  
    finally
    {
      IL_0058:  ldloc.1
      IL_0059:  isinst     [runtime]System.IDisposable
      IL_005e:  stloc.s    V_7
      IL_0060:  ldloc.s    V_7
      IL_0062:  brfalse.s  IL_006c

      IL_0064:  ldloc.s    V_7
      IL_0066:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_006b:  endfinally
      IL_006c:  endfinally
    }  
    IL_006d:  ldloc.2
    IL_006e:  pop
    IL_006f:  ldloca.s   V_0
    IL_0071:  call       instance !0[] valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ArrayCollector`1<int32>::Close()
    IL_0076:  ret
  } 

  .method public static int32[]  f1() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0029

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  ldloc.3
    IL_001f:  stelem.i4
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.s   10
    IL_002c:  conv.i8
    IL_002d:  blt.un.s   IL_0011

    IL_002f:  ldloc.0
    IL_0030:  ret
  } 

  .method public static int32[]  f2() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0005:  ret
  } 

  .method public static int32[]  f3() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0029

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  ldloc.3
    IL_001f:  stelem.i4
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.s   10
    IL_002c:  conv.i8
    IL_002d:  blt.un.s   IL_0011

    IL_002f:  ldloc.0
    IL_0030:  ret
  } 

  .method public static int32[]  f4() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5)
    IL_0000:  ldc.i4.5
    IL_0001:  conv.i8
    IL_0002:  conv.ovf.i.un
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  conv.i8
    IL_000b:  stloc.1
    IL_000c:  ldc.i4.1
    IL_000d:  stloc.2
    IL_000e:  br.s       IL_0028

    IL_0010:  ldloc.2
    IL_0011:  stloc.3
    IL_0012:  ldloc.0
    IL_0013:  ldloc.1
    IL_0014:  conv.i
    IL_0015:  stloc.s    V_4
    IL_0017:  stloc.s    V_5
    IL_0019:  ldloc.s    V_5
    IL_001b:  ldloc.s    V_4
    IL_001d:  ldloc.3
    IL_001e:  stelem.i4
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.2
    IL_0021:  add
    IL_0022:  stloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add
    IL_0027:  stloc.1
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.5
    IL_002a:  conv.i8
    IL_002b:  blt.un.s   IL_0010

    IL_002d:  ldloc.0
    IL_002e:  ret
  } 

  .method public static int32[]  f5() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0005:  ret
  } 

  .method public static int32[]  f6() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0005:  ret
  } 

  .method public static int32[]  f7() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.s   10
    IL_000f:  stloc.2
    IL_0010:  br.s       IL_002a

    IL_0012:  ldloc.2
    IL_0013:  stloc.3
    IL_0014:  ldloc.0
    IL_0015:  ldloc.1
    IL_0016:  conv.i
    IL_0017:  stloc.s    V_4
    IL_0019:  stloc.s    V_5
    IL_001b:  ldloc.s    V_5
    IL_001d:  ldloc.s    V_4
    IL_001f:  ldloc.3
    IL_0020:  stelem.i4
    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.m1
    IL_0023:  add
    IL_0024:  stloc.2
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldc.i4.s   10
    IL_002d:  conv.i8
    IL_002e:  blt.un.s   IL_0012

    IL_0030:  ldloc.0
    IL_0031:  ret
  } 

  .method public static int32[]  f8() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5)
    IL_0000:  ldc.i4.5
    IL_0001:  conv.i8
    IL_0002:  conv.ovf.i.un
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  conv.i8
    IL_000b:  stloc.1
    IL_000c:  ldc.i4.s   10
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_002a

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  ldloc.3
    IL_001f:  stelem.i4
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.s   -2
    IL_0023:  add
    IL_0024:  stloc.2
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldc.i4.5
    IL_002c:  conv.i8
    IL_002d:  blt.un.s   IL_0011

    IL_002f:  ldloc.0
    IL_0030:  ret
  } 

  .method public static int32[]  f9(int32 start) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0013

    IL_000a:  ldc.i4.s   10
    IL_000c:  ldarg.0
    IL_000d:  sub
    IL_000e:  conv.i8
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  brtrue.s   IL_001f

    IL_0019:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001e:  ret

    IL_001f:  ldloc.1
    IL_0020:  conv.ovf.i.un
    IL_0021:  newarr     [runtime]System.Int32
    IL_0026:  stloc.2
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.3
    IL_002a:  ldarg.0
    IL_002b:  stloc.s    V_4
    IL_002d:  br.s       IL_004c

    IL_002f:  ldloc.s    V_4
    IL_0031:  stloc.s    V_5
    IL_0033:  ldloc.2
    IL_0034:  ldloc.3
    IL_0035:  conv.i
    IL_0036:  stloc.s    V_6
    IL_0038:  stloc.s    V_7
    IL_003a:  ldloc.s    V_7
    IL_003c:  ldloc.s    V_6
    IL_003e:  ldloc.s    V_5
    IL_0040:  stelem.i4
    IL_0041:  ldloc.s    V_4
    IL_0043:  ldc.i4.1
    IL_0044:  add
    IL_0045:  stloc.s    V_4
    IL_0047:  ldloc.3
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  stloc.3
    IL_004c:  ldloc.3
    IL_004d:  ldloc.0
    IL_004e:  blt.un.s   IL_002f

    IL_0050:  ldloc.2
    IL_0051:  ret
  } 

  .method public static int32[]  f10(int32 finish) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001c:  ret

    IL_001d:  ldloc.1
    IL_001e:  conv.ovf.i.un
    IL_001f:  newarr     [runtime]System.Int32
    IL_0024:  stloc.2
    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.3
    IL_0028:  ldc.i4.1
    IL_0029:  stloc.s    V_4
    IL_002b:  br.s       IL_004a

    IL_002d:  ldloc.s    V_4
    IL_002f:  stloc.s    V_5
    IL_0031:  ldloc.2
    IL_0032:  ldloc.3
    IL_0033:  conv.i
    IL_0034:  stloc.s    V_6
    IL_0036:  stloc.s    V_7
    IL_0038:  ldloc.s    V_7
    IL_003a:  ldloc.s    V_6
    IL_003c:  ldloc.s    V_5
    IL_003e:  stelem.i4
    IL_003f:  ldloc.s    V_4
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.s    V_4
    IL_0045:  ldloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.3
    IL_004a:  ldloc.3
    IL_004b:  ldloc.0
    IL_004c:  blt.un.s   IL_002d

    IL_004e:  ldloc.2
    IL_004f:  ret
  } 

  .method public static int32[]  f11(int32 start,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001c:  ret

    IL_001d:  ldloc.1
    IL_001e:  conv.ovf.i.un
    IL_001f:  newarr     [runtime]System.Int32
    IL_0024:  stloc.2
    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.3
    IL_0028:  ldarg.0
    IL_0029:  stloc.s    V_4
    IL_002b:  br.s       IL_004a

    IL_002d:  ldloc.s    V_4
    IL_002f:  stloc.s    V_5
    IL_0031:  ldloc.2
    IL_0032:  ldloc.3
    IL_0033:  conv.i
    IL_0034:  stloc.s    V_6
    IL_0036:  stloc.s    V_7
    IL_0038:  ldloc.s    V_7
    IL_003a:  ldloc.s    V_6
    IL_003c:  ldloc.s    V_5
    IL_003e:  stelem.i4
    IL_003f:  ldloc.s    V_4
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.s    V_4
    IL_0045:  ldloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.3
    IL_004a:  ldloc.3
    IL_004b:  ldloc.0
    IL_004c:  blt.un.s   IL_002d

    IL_004e:  ldloc.2
    IL_004f:  ret
  } 

  .method public static int32[]  f12(int32 start) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0013

    IL_000a:  ldc.i4.s   10
    IL_000c:  ldarg.0
    IL_000d:  sub
    IL_000e:  conv.i8
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  brtrue.s   IL_001f

    IL_0019:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001e:  ret

    IL_001f:  ldloc.1
    IL_0020:  conv.ovf.i.un
    IL_0021:  newarr     [runtime]System.Int32
    IL_0026:  stloc.2
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.3
    IL_002a:  ldarg.0
    IL_002b:  stloc.s    V_4
    IL_002d:  br.s       IL_004c

    IL_002f:  ldloc.s    V_4
    IL_0031:  stloc.s    V_5
    IL_0033:  ldloc.2
    IL_0034:  ldloc.3
    IL_0035:  conv.i
    IL_0036:  stloc.s    V_6
    IL_0038:  stloc.s    V_7
    IL_003a:  ldloc.s    V_7
    IL_003c:  ldloc.s    V_6
    IL_003e:  ldloc.s    V_5
    IL_0040:  stelem.i4
    IL_0041:  ldloc.s    V_4
    IL_0043:  ldc.i4.1
    IL_0044:  add
    IL_0045:  stloc.s    V_4
    IL_0047:  ldloc.3
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  stloc.3
    IL_004c:  ldloc.3
    IL_004d:  ldloc.0
    IL_004e:  blt.un.s   IL_002f

    IL_0050:  ldloc.2
    IL_0051:  ret
  } 

  .method public static int32[]  f13(int32 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldc.i4.1
    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.s   10
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.0
    IL_0013:  bge.s      IL_002c

    IL_0015:  ldc.i4.s   10
    IL_0017:  ldc.i4.1
    IL_0018:  bge.s      IL_001f

    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  nop
    IL_001d:  br.s       IL_0044

    IL_001f:  ldc.i4.s   10
    IL_0021:  ldc.i4.1
    IL_0022:  sub
    IL_0023:  ldarg.0
    IL_0024:  div.un
    IL_0025:  conv.i8
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  nop
    IL_002a:  br.s       IL_0044

    IL_002c:  ldc.i4.1
    IL_002d:  ldc.i4.s   10
    IL_002f:  bge.s      IL_0036

    IL_0031:  ldc.i4.0
    IL_0032:  conv.i8
    IL_0033:  nop
    IL_0034:  br.s       IL_0044

    IL_0036:  ldc.i4.1
    IL_0037:  ldc.i4.s   10
    IL_0039:  sub
    IL_003a:  ldarg.0
    IL_003b:  not
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  div.un
    IL_003f:  conv.i8
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  nop
    IL_0044:  stloc.0
    IL_0045:  ldloc.0
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  brtrue.s   IL_0050

    IL_004a:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_004f:  ret

    IL_0050:  ldloc.1
    IL_0051:  conv.ovf.i.un
    IL_0052:  newarr     [runtime]System.Int32
    IL_0057:  stloc.2
    IL_0058:  ldc.i4.0
    IL_0059:  conv.i8
    IL_005a:  stloc.3
    IL_005b:  ldc.i4.1
    IL_005c:  stloc.s    V_4
    IL_005e:  br.s       IL_007d

    IL_0060:  ldloc.s    V_4
    IL_0062:  stloc.s    V_5
    IL_0064:  ldloc.2
    IL_0065:  ldloc.3
    IL_0066:  conv.i
    IL_0067:  stloc.s    V_6
    IL_0069:  stloc.s    V_7
    IL_006b:  ldloc.s    V_7
    IL_006d:  ldloc.s    V_6
    IL_006f:  ldloc.s    V_5
    IL_0071:  stelem.i4
    IL_0072:  ldloc.s    V_4
    IL_0074:  ldarg.0
    IL_0075:  add
    IL_0076:  stloc.s    V_4
    IL_0078:  ldloc.3
    IL_0079:  ldc.i4.1
    IL_007a:  conv.i8
    IL_007b:  add
    IL_007c:  stloc.3
    IL_007d:  ldloc.3
    IL_007e:  ldloc.0
    IL_007f:  blt.un.s   IL_0060

    IL_0081:  ldloc.2
    IL_0082:  ret
  } 

  .method public static int32[]  f14(int32 finish) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001c:  ret

    IL_001d:  ldloc.1
    IL_001e:  conv.ovf.i.un
    IL_001f:  newarr     [runtime]System.Int32
    IL_0024:  stloc.2
    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.3
    IL_0028:  ldc.i4.1
    IL_0029:  stloc.s    V_4
    IL_002b:  br.s       IL_004a

    IL_002d:  ldloc.s    V_4
    IL_002f:  stloc.s    V_5
    IL_0031:  ldloc.2
    IL_0032:  ldloc.3
    IL_0033:  conv.i
    IL_0034:  stloc.s    V_6
    IL_0036:  stloc.s    V_7
    IL_0038:  ldloc.s    V_7
    IL_003a:  ldloc.s    V_6
    IL_003c:  ldloc.s    V_5
    IL_003e:  stelem.i4
    IL_003f:  ldloc.s    V_4
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.s    V_4
    IL_0045:  ldloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.3
    IL_004a:  ldloc.3
    IL_004b:  ldloc.0
    IL_004c:  blt.un.s   IL_002d

    IL_004e:  ldloc.2
    IL_004f:  ret
  } 

  .method public static int32[]  f15(int32 start,
                                     int32 step) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldc.i4.s   10
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.1
    IL_0013:  bge.s      IL_002c

    IL_0015:  ldc.i4.s   10
    IL_0017:  ldarg.0
    IL_0018:  bge.s      IL_001f

    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  nop
    IL_001d:  br.s       IL_0044

    IL_001f:  ldc.i4.s   10
    IL_0021:  ldarg.0
    IL_0022:  sub
    IL_0023:  ldarg.1
    IL_0024:  div.un
    IL_0025:  conv.i8
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  nop
    IL_002a:  br.s       IL_0044

    IL_002c:  ldarg.0
    IL_002d:  ldc.i4.s   10
    IL_002f:  bge.s      IL_0036

    IL_0031:  ldc.i4.0
    IL_0032:  conv.i8
    IL_0033:  nop
    IL_0034:  br.s       IL_0044

    IL_0036:  ldarg.0
    IL_0037:  ldc.i4.s   10
    IL_0039:  sub
    IL_003a:  ldarg.1
    IL_003b:  not
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  div.un
    IL_003f:  conv.i8
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  nop
    IL_0044:  stloc.0
    IL_0045:  ldloc.0
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  brtrue.s   IL_0050

    IL_004a:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_004f:  ret

    IL_0050:  ldloc.1
    IL_0051:  conv.ovf.i.un
    IL_0052:  newarr     [runtime]System.Int32
    IL_0057:  stloc.2
    IL_0058:  ldc.i4.0
    IL_0059:  conv.i8
    IL_005a:  stloc.3
    IL_005b:  ldarg.0
    IL_005c:  stloc.s    V_4
    IL_005e:  br.s       IL_007d

    IL_0060:  ldloc.s    V_4
    IL_0062:  stloc.s    V_5
    IL_0064:  ldloc.2
    IL_0065:  ldloc.3
    IL_0066:  conv.i
    IL_0067:  stloc.s    V_6
    IL_0069:  stloc.s    V_7
    IL_006b:  ldloc.s    V_7
    IL_006d:  ldloc.s    V_6
    IL_006f:  ldloc.s    V_5
    IL_0071:  stelem.i4
    IL_0072:  ldloc.s    V_4
    IL_0074:  ldarg.1
    IL_0075:  add
    IL_0076:  stloc.s    V_4
    IL_0078:  ldloc.3
    IL_0079:  ldc.i4.1
    IL_007a:  conv.i8
    IL_007b:  add
    IL_007c:  stloc.3
    IL_007d:  ldloc.3
    IL_007e:  ldloc.0
    IL_007f:  blt.un.s   IL_0060

    IL_0081:  ldloc.2
    IL_0082:  ret
  } 

  .method public static int32[]  f16(int32 start,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001c:  ret

    IL_001d:  ldloc.1
    IL_001e:  conv.ovf.i.un
    IL_001f:  newarr     [runtime]System.Int32
    IL_0024:  stloc.2
    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.3
    IL_0028:  ldarg.0
    IL_0029:  stloc.s    V_4
    IL_002b:  br.s       IL_004a

    IL_002d:  ldloc.s    V_4
    IL_002f:  stloc.s    V_5
    IL_0031:  ldloc.2
    IL_0032:  ldloc.3
    IL_0033:  conv.i
    IL_0034:  stloc.s    V_6
    IL_0036:  stloc.s    V_7
    IL_0038:  ldloc.s    V_7
    IL_003a:  ldloc.s    V_6
    IL_003c:  ldloc.s    V_5
    IL_003e:  stelem.i4
    IL_003f:  ldloc.s    V_4
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.s    V_4
    IL_0045:  ldloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.3
    IL_004a:  ldloc.3
    IL_004b:  ldloc.0
    IL_004c:  blt.un.s   IL_002d

    IL_004e:  ldloc.2
    IL_004f:  ret
  } 

  .method public static int32[]  f17(int32 step,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldc.i4.1
    IL_0004:  ldarg.0
    IL_0005:  ldarg.1
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.0
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.1
    IL_0015:  ldc.i4.1
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.1
    IL_001e:  ldc.i4.1
    IL_001f:  sub
    IL_0020:  ldarg.0
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldc.i4.1
    IL_002a:  ldarg.1
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldc.i4.1
    IL_0033:  ldarg.1
    IL_0034:  sub
    IL_0035:  ldarg.0
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldloc.0
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  brtrue.s   IL_004b

    IL_0045:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  conv.ovf.i.un
    IL_004d:  newarr     [runtime]System.Int32
    IL_0052:  stloc.2
    IL_0053:  ldc.i4.0
    IL_0054:  conv.i8
    IL_0055:  stloc.3
    IL_0056:  ldc.i4.1
    IL_0057:  stloc.s    V_4
    IL_0059:  br.s       IL_0078

    IL_005b:  ldloc.s    V_4
    IL_005d:  stloc.s    V_5
    IL_005f:  ldloc.2
    IL_0060:  ldloc.3
    IL_0061:  conv.i
    IL_0062:  stloc.s    V_6
    IL_0064:  stloc.s    V_7
    IL_0066:  ldloc.s    V_7
    IL_0068:  ldloc.s    V_6
    IL_006a:  ldloc.s    V_5
    IL_006c:  stelem.i4
    IL_006d:  ldloc.s    V_4
    IL_006f:  ldarg.0
    IL_0070:  add
    IL_0071:  stloc.s    V_4
    IL_0073:  ldloc.3
    IL_0074:  ldc.i4.1
    IL_0075:  conv.i8
    IL_0076:  add
    IL_0077:  stloc.3
    IL_0078:  ldloc.3
    IL_0079:  ldloc.0
    IL_007a:  blt.un.s   IL_005b

    IL_007c:  ldloc.2
    IL_007d:  ret
  } 

  .method public static int32[]  f18(int32 start,
                                     int32 step,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.2
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldarg.1
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldarg.0
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldarg.0
    IL_0033:  ldarg.2
    IL_0034:  sub
    IL_0035:  ldarg.1
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldloc.0
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  brtrue.s   IL_004b

    IL_0045:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  conv.ovf.i.un
    IL_004d:  newarr     [runtime]System.Int32
    IL_0052:  stloc.2
    IL_0053:  ldc.i4.0
    IL_0054:  conv.i8
    IL_0055:  stloc.3
    IL_0056:  ldarg.0
    IL_0057:  stloc.s    V_4
    IL_0059:  br.s       IL_0078

    IL_005b:  ldloc.s    V_4
    IL_005d:  stloc.s    V_5
    IL_005f:  ldloc.2
    IL_0060:  ldloc.3
    IL_0061:  conv.i
    IL_0062:  stloc.s    V_6
    IL_0064:  stloc.s    V_7
    IL_0066:  ldloc.s    V_7
    IL_0068:  ldloc.s    V_6
    IL_006a:  ldloc.s    V_5
    IL_006c:  stelem.i4
    IL_006d:  ldloc.s    V_4
    IL_006f:  ldarg.1
    IL_0070:  add
    IL_0071:  stloc.s    V_4
    IL_0073:  ldloc.3
    IL_0074:  ldc.i4.1
    IL_0075:  conv.i8
    IL_0076:  add
    IL_0077:  stloc.3
    IL_0078:  ldloc.3
    IL_0079:  ldloc.0
    IL_007a:  blt.un.s   IL_005b

    IL_007c:  ldloc.2
    IL_007d:  ret
  } 

  .method public static int32[]  f19(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5,
             int32 V_6,
             native int V_7,
             int32[] V_8)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.s   10
    IL_000a:  ldloc.0
    IL_000b:  bge.s      IL_0012

    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  nop
    IL_0010:  br.s       IL_001b

    IL_0012:  ldc.i4.s   10
    IL_0014:  ldloc.0
    IL_0015:  sub
    IL_0016:  conv.i8
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  nop
    IL_001b:  stloc.1
    IL_001c:  ldloc.1
    IL_001d:  stloc.2
    IL_001e:  ldloc.2
    IL_001f:  brtrue.s   IL_0027

    IL_0021:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0026:  ret

    IL_0027:  ldloc.2
    IL_0028:  conv.ovf.i.un
    IL_0029:  newarr     [runtime]System.Int32
    IL_002e:  stloc.3
    IL_002f:  ldc.i4.0
    IL_0030:  conv.i8
    IL_0031:  stloc.s    V_4
    IL_0033:  ldloc.0
    IL_0034:  stloc.s    V_5
    IL_0036:  br.s       IL_0058

    IL_0038:  ldloc.s    V_5
    IL_003a:  stloc.s    V_6
    IL_003c:  ldloc.3
    IL_003d:  ldloc.s    V_4
    IL_003f:  conv.i
    IL_0040:  stloc.s    V_7
    IL_0042:  stloc.s    V_8
    IL_0044:  ldloc.s    V_8
    IL_0046:  ldloc.s    V_7
    IL_0048:  ldloc.s    V_6
    IL_004a:  stelem.i4
    IL_004b:  ldloc.s    V_5
    IL_004d:  ldc.i4.1
    IL_004e:  add
    IL_004f:  stloc.s    V_5
    IL_0051:  ldloc.s    V_4
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add
    IL_0056:  stloc.s    V_4
    IL_0058:  ldloc.s    V_4
    IL_005a:  ldloc.1
    IL_005b:  blt.un.s   IL_0038

    IL_005d:  ldloc.3
    IL_005e:  ret
  } 

  .method public static int32[]  f20(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5,
             int32 V_6,
             native int V_7,
             int32[] V_8)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  bge.s      IL_0011

    IL_000c:  ldc.i4.0
    IL_000d:  conv.i8
    IL_000e:  nop
    IL_000f:  br.s       IL_0019

    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  sub
    IL_0014:  conv.i8
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  nop
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  brtrue.s   IL_0025

    IL_001f:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0024:  ret

    IL_0025:  ldloc.2
    IL_0026:  conv.ovf.i.un
    IL_0027:  newarr     [runtime]System.Int32
    IL_002c:  stloc.3
    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  stloc.s    V_4
    IL_0031:  ldc.i4.1
    IL_0032:  stloc.s    V_5
    IL_0034:  br.s       IL_0056

    IL_0036:  ldloc.s    V_5
    IL_0038:  stloc.s    V_6
    IL_003a:  ldloc.3
    IL_003b:  ldloc.s    V_4
    IL_003d:  conv.i
    IL_003e:  stloc.s    V_7
    IL_0040:  stloc.s    V_8
    IL_0042:  ldloc.s    V_8
    IL_0044:  ldloc.s    V_7
    IL_0046:  ldloc.s    V_6
    IL_0048:  stelem.i4
    IL_0049:  ldloc.s    V_5
    IL_004b:  ldc.i4.1
    IL_004c:  add
    IL_004d:  stloc.s    V_5
    IL_004f:  ldloc.s    V_4
    IL_0051:  ldc.i4.1
    IL_0052:  conv.i8
    IL_0053:  add
    IL_0054:  stloc.s    V_4
    IL_0056:  ldloc.s    V_4
    IL_0058:  ldloc.1
    IL_0059:  blt.un.s   IL_0036

    IL_005b:  ldloc.3
    IL_005c:  ret
  } 

  .method public static int32[]  f21(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             uint64 V_2,
             uint64 V_3,
             int32[] V_4,
             uint64 V_5,
             int32 V_6,
             int32 V_7,
             native int V_8,
             int32[] V_9)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  ldloc.0
    IL_0012:  bge.s      IL_0019

    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  nop
    IL_0017:  br.s       IL_0021

    IL_0019:  ldloc.1
    IL_001a:  ldloc.0
    IL_001b:  sub
    IL_001c:  conv.i8
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  add
    IL_0020:  nop
    IL_0021:  stloc.2
    IL_0022:  ldloc.2
    IL_0023:  stloc.3
    IL_0024:  ldloc.3
    IL_0025:  brtrue.s   IL_002d

    IL_0027:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_002c:  ret

    IL_002d:  ldloc.3
    IL_002e:  conv.ovf.i.un
    IL_002f:  newarr     [runtime]System.Int32
    IL_0034:  stloc.s    V_4
    IL_0036:  ldc.i4.0
    IL_0037:  conv.i8
    IL_0038:  stloc.s    V_5
    IL_003a:  ldloc.0
    IL_003b:  stloc.s    V_6
    IL_003d:  br.s       IL_0060

    IL_003f:  ldloc.s    V_6
    IL_0041:  stloc.s    V_7
    IL_0043:  ldloc.s    V_4
    IL_0045:  ldloc.s    V_5
    IL_0047:  conv.i
    IL_0048:  stloc.s    V_8
    IL_004a:  stloc.s    V_9
    IL_004c:  ldloc.s    V_9
    IL_004e:  ldloc.s    V_8
    IL_0050:  ldloc.s    V_7
    IL_0052:  stelem.i4
    IL_0053:  ldloc.s    V_6
    IL_0055:  ldc.i4.1
    IL_0056:  add
    IL_0057:  stloc.s    V_6
    IL_0059:  ldloc.s    V_5
    IL_005b:  ldc.i4.1
    IL_005c:  conv.i8
    IL_005d:  add
    IL_005e:  stloc.s    V_5
    IL_0060:  ldloc.s    V_5
    IL_0062:  ldloc.2
    IL_0063:  blt.un.s   IL_003f

    IL_0065:  ldloc.s    V_4
    IL_0067:  ret
  } 

  .method public static int32[]  f22(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5,
             int32 V_6,
             native int V_7,
             int32[] V_8)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.s   10
    IL_000a:  ldloc.0
    IL_000b:  bge.s      IL_0012

    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  nop
    IL_0010:  br.s       IL_001b

    IL_0012:  ldc.i4.s   10
    IL_0014:  ldloc.0
    IL_0015:  sub
    IL_0016:  conv.i8
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  nop
    IL_001b:  stloc.1
    IL_001c:  ldloc.1
    IL_001d:  stloc.2
    IL_001e:  ldloc.2
    IL_001f:  brtrue.s   IL_0027

    IL_0021:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0026:  ret

    IL_0027:  ldloc.2
    IL_0028:  conv.ovf.i.un
    IL_0029:  newarr     [runtime]System.Int32
    IL_002e:  stloc.3
    IL_002f:  ldc.i4.0
    IL_0030:  conv.i8
    IL_0031:  stloc.s    V_4
    IL_0033:  ldloc.0
    IL_0034:  stloc.s    V_5
    IL_0036:  br.s       IL_0058

    IL_0038:  ldloc.s    V_5
    IL_003a:  stloc.s    V_6
    IL_003c:  ldloc.3
    IL_003d:  ldloc.s    V_4
    IL_003f:  conv.i
    IL_0040:  stloc.s    V_7
    IL_0042:  stloc.s    V_8
    IL_0044:  ldloc.s    V_8
    IL_0046:  ldloc.s    V_7
    IL_0048:  ldloc.s    V_6
    IL_004a:  stelem.i4
    IL_004b:  ldloc.s    V_5
    IL_004d:  ldc.i4.1
    IL_004e:  add
    IL_004f:  stloc.s    V_5
    IL_0051:  ldloc.s    V_4
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add
    IL_0056:  stloc.s    V_4
    IL_0058:  ldloc.s    V_4
    IL_005a:  ldloc.1
    IL_005b:  blt.un.s   IL_0038

    IL_005d:  ldloc.3
    IL_005e:  ret
  } 

  .method public static int32[]  f23(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5,
             int32 V_6,
             native int V_7,
             int32[] V_8)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  brtrue.s   IL_0018

    IL_000b:  ldc.i4.1
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.s   10
    IL_000f:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0014:  pop
    IL_0015:  nop
    IL_0016:  br.s       IL_0019

    IL_0018:  nop
    IL_0019:  ldc.i4.0
    IL_001a:  ldloc.0
    IL_001b:  bge.s      IL_0034

    IL_001d:  ldc.i4.s   10
    IL_001f:  ldc.i4.1
    IL_0020:  bge.s      IL_0027

    IL_0022:  ldc.i4.0
    IL_0023:  conv.i8
    IL_0024:  nop
    IL_0025:  br.s       IL_004c

    IL_0027:  ldc.i4.s   10
    IL_0029:  ldc.i4.1
    IL_002a:  sub
    IL_002b:  ldloc.0
    IL_002c:  div.un
    IL_002d:  conv.i8
    IL_002e:  ldc.i4.1
    IL_002f:  conv.i8
    IL_0030:  add
    IL_0031:  nop
    IL_0032:  br.s       IL_004c

    IL_0034:  ldc.i4.1
    IL_0035:  ldc.i4.s   10
    IL_0037:  bge.s      IL_003e

    IL_0039:  ldc.i4.0
    IL_003a:  conv.i8
    IL_003b:  nop
    IL_003c:  br.s       IL_004c

    IL_003e:  ldc.i4.1
    IL_003f:  ldc.i4.s   10
    IL_0041:  sub
    IL_0042:  ldloc.0
    IL_0043:  not
    IL_0044:  ldc.i4.1
    IL_0045:  add
    IL_0046:  div.un
    IL_0047:  conv.i8
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  nop
    IL_004c:  stloc.1
    IL_004d:  ldloc.1
    IL_004e:  stloc.2
    IL_004f:  ldloc.2
    IL_0050:  brtrue.s   IL_0058

    IL_0052:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0057:  ret

    IL_0058:  ldloc.2
    IL_0059:  conv.ovf.i.un
    IL_005a:  newarr     [runtime]System.Int32
    IL_005f:  stloc.3
    IL_0060:  ldc.i4.0
    IL_0061:  conv.i8
    IL_0062:  stloc.s    V_4
    IL_0064:  ldc.i4.1
    IL_0065:  stloc.s    V_5
    IL_0067:  br.s       IL_0089

    IL_0069:  ldloc.s    V_5
    IL_006b:  stloc.s    V_6
    IL_006d:  ldloc.3
    IL_006e:  ldloc.s    V_4
    IL_0070:  conv.i
    IL_0071:  stloc.s    V_7
    IL_0073:  stloc.s    V_8
    IL_0075:  ldloc.s    V_8
    IL_0077:  ldloc.s    V_7
    IL_0079:  ldloc.s    V_6
    IL_007b:  stelem.i4
    IL_007c:  ldloc.s    V_5
    IL_007e:  ldloc.0
    IL_007f:  add
    IL_0080:  stloc.s    V_5
    IL_0082:  ldloc.s    V_4
    IL_0084:  ldc.i4.1
    IL_0085:  conv.i8
    IL_0086:  add
    IL_0087:  stloc.s    V_4
    IL_0089:  ldloc.s    V_4
    IL_008b:  ldloc.1
    IL_008c:  blt.un.s   IL_0069

    IL_008e:  ldloc.3
    IL_008f:  ret
  } 

  .method public static int32[]  f24(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5,
             int32 V_6,
             native int V_7,
             int32[] V_8)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  bge.s      IL_0011

    IL_000c:  ldc.i4.0
    IL_000d:  conv.i8
    IL_000e:  nop
    IL_000f:  br.s       IL_0019

    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  sub
    IL_0014:  conv.i8
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  nop
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  brtrue.s   IL_0025

    IL_001f:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0024:  ret

    IL_0025:  ldloc.2
    IL_0026:  conv.ovf.i.un
    IL_0027:  newarr     [runtime]System.Int32
    IL_002c:  stloc.3
    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  stloc.s    V_4
    IL_0031:  ldc.i4.1
    IL_0032:  stloc.s    V_5
    IL_0034:  br.s       IL_0056

    IL_0036:  ldloc.s    V_5
    IL_0038:  stloc.s    V_6
    IL_003a:  ldloc.3
    IL_003b:  ldloc.s    V_4
    IL_003d:  conv.i
    IL_003e:  stloc.s    V_7
    IL_0040:  stloc.s    V_8
    IL_0042:  ldloc.s    V_8
    IL_0044:  ldloc.s    V_7
    IL_0046:  ldloc.s    V_6
    IL_0048:  stelem.i4
    IL_0049:  ldloc.s    V_5
    IL_004b:  ldc.i4.1
    IL_004c:  add
    IL_004d:  stloc.s    V_5
    IL_004f:  ldloc.s    V_4
    IL_0051:  ldc.i4.1
    IL_0052:  conv.i8
    IL_0053:  add
    IL_0054:  stloc.s    V_4
    IL_0056:  ldloc.s    V_4
    IL_0058:  ldloc.1
    IL_0059:  blt.un.s   IL_0036

    IL_005b:  ldloc.3
    IL_005c:  ret
  } 

  .method public static int32[]  f25(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> h) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             uint64 V_3,
             uint64 V_4,
             int32[] V_5,
             uint64 V_6,
             int32 V_7,
             int32 V_8,
             native int V_9,
             int32[] V_10)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_000f:  stloc.1
    IL_0010:  ldarg.2
    IL_0011:  ldnull
    IL_0012:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0017:  stloc.2
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0027

    IL_001b:  ldloc.0
    IL_001c:  ldloc.1
    IL_001d:  ldloc.2
    IL_001e:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0023:  pop
    IL_0024:  nop
    IL_0025:  br.s       IL_0028

    IL_0027:  nop
    IL_0028:  ldc.i4.0
    IL_0029:  ldloc.1
    IL_002a:  bge.s      IL_0041

    IL_002c:  ldloc.2
    IL_002d:  ldloc.0
    IL_002e:  bge.s      IL_0035

    IL_0030:  ldc.i4.0
    IL_0031:  conv.i8
    IL_0032:  nop
    IL_0033:  br.s       IL_0057

    IL_0035:  ldloc.2
    IL_0036:  ldloc.0
    IL_0037:  sub
    IL_0038:  ldloc.1
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  br.s       IL_0057

    IL_0041:  ldloc.0
    IL_0042:  ldloc.2
    IL_0043:  bge.s      IL_004a

    IL_0045:  ldc.i4.0
    IL_0046:  conv.i8
    IL_0047:  nop
    IL_0048:  br.s       IL_0057

    IL_004a:  ldloc.0
    IL_004b:  ldloc.2
    IL_004c:  sub
    IL_004d:  ldloc.1
    IL_004e:  not
    IL_004f:  ldc.i4.1
    IL_0050:  add
    IL_0051:  div.un
    IL_0052:  conv.i8
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add
    IL_0056:  nop
    IL_0057:  stloc.3
    IL_0058:  ldloc.3
    IL_0059:  stloc.s    V_4
    IL_005b:  ldloc.s    V_4
    IL_005d:  brtrue.s   IL_0065

    IL_005f:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0064:  ret

    IL_0065:  ldloc.s    V_4
    IL_0067:  conv.ovf.i.un
    IL_0068:  newarr     [runtime]System.Int32
    IL_006d:  stloc.s    V_5
    IL_006f:  ldc.i4.0
    IL_0070:  conv.i8
    IL_0071:  stloc.s    V_6
    IL_0073:  ldloc.0
    IL_0074:  stloc.s    V_7
    IL_0076:  br.s       IL_0099

    IL_0078:  ldloc.s    V_7
    IL_007a:  stloc.s    V_8
    IL_007c:  ldloc.s    V_5
    IL_007e:  ldloc.s    V_6
    IL_0080:  conv.i
    IL_0081:  stloc.s    V_9
    IL_0083:  stloc.s    V_10
    IL_0085:  ldloc.s    V_10
    IL_0087:  ldloc.s    V_9
    IL_0089:  ldloc.s    V_8
    IL_008b:  stelem.i4
    IL_008c:  ldloc.s    V_7
    IL_008e:  ldloc.1
    IL_008f:  add
    IL_0090:  stloc.s    V_7
    IL_0092:  ldloc.s    V_6
    IL_0094:  ldc.i4.1
    IL_0095:  conv.i8
    IL_0096:  add
    IL_0097:  stloc.s    V_6
    IL_0099:  ldloc.s    V_6
    IL_009b:  ldloc.3
    IL_009c:  blt.un.s   IL_0078

    IL_009e:  ldloc.s    V_5
    IL_00a0:  ret
  } 

  .method public static class [runtime]System.Tuple`2<int32,float64>[] 
          f26(int32 start,
              int32 step,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (uint64 V_0,
             uint64 V_1,
             class [runtime]System.Tuple`2<int32,float64>[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             class [runtime]System.Tuple`2<int32,float64>[] V_7)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.2
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldarg.1
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldarg.0
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldarg.0
    IL_0033:  ldarg.2
    IL_0034:  sub
    IL_0035:  ldarg.1
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldloc.0
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  brtrue.s   IL_004b

    IL_0045:  call       !!0[] [runtime]System.Array::Empty<class [runtime]System.Tuple`2<int32,float64>>()
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  conv.ovf.i.un
    IL_004d:  newarr     class [runtime]System.Tuple`2<int32,float64>
    IL_0052:  stloc.2
    IL_0053:  ldc.i4.0
    IL_0054:  conv.i8
    IL_0055:  stloc.3
    IL_0056:  ldarg.0
    IL_0057:  stloc.s    V_4
    IL_0059:  br.s       IL_0084

    IL_005b:  ldloc.s    V_4
    IL_005d:  stloc.s    V_5
    IL_005f:  ldloc.2
    IL_0060:  ldloc.3
    IL_0061:  conv.i
    IL_0062:  stloc.s    V_6
    IL_0064:  stloc.s    V_7
    IL_0066:  ldloc.s    V_7
    IL_0068:  ldloc.s    V_6
    IL_006a:  ldloc.s    V_5
    IL_006c:  ldloc.s    V_5
    IL_006e:  conv.r8
    IL_006f:  newobj     instance void class [runtime]System.Tuple`2<int32,float64>::.ctor(!0,
                                                                                                  !1)
    IL_0074:  stelem     class [runtime]System.Tuple`2<int32,float64>
    IL_0079:  ldloc.s    V_4
    IL_007b:  ldarg.1
    IL_007c:  add
    IL_007d:  stloc.s    V_4
    IL_007f:  ldloc.3
    IL_0080:  ldc.i4.1
    IL_0081:  conv.i8
    IL_0082:  add
    IL_0083:  stloc.3
    IL_0084:  ldloc.3
    IL_0085:  ldloc.0
    IL_0086:  blt.un.s   IL_005b

    IL_0088:  ldloc.2
    IL_0089:  ret
  } 

  .method public static valuetype [runtime]System.ValueTuple`2<int32,float64>[] 
          f27(int32 start,
              int32 step,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (uint64 V_0,
             uint64 V_1,
             valuetype [runtime]System.ValueTuple`2<int32,float64>[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             valuetype [runtime]System.ValueTuple`2<int32,float64>[] V_7)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.2
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldarg.1
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldarg.0
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldarg.0
    IL_0033:  ldarg.2
    IL_0034:  sub
    IL_0035:  ldarg.1
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldloc.0
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  brtrue.s   IL_004b

    IL_0045:  call       !!0[] [runtime]System.Array::Empty<valuetype [runtime]System.ValueTuple`2<int32,float64>>()
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  conv.ovf.i.un
    IL_004d:  newarr     valuetype [runtime]System.ValueTuple`2<int32,float64>
    IL_0052:  stloc.2
    IL_0053:  ldc.i4.0
    IL_0054:  conv.i8
    IL_0055:  stloc.3
    IL_0056:  ldarg.0
    IL_0057:  stloc.s    V_4
    IL_0059:  br.s       IL_0084

    IL_005b:  ldloc.s    V_4
    IL_005d:  stloc.s    V_5
    IL_005f:  ldloc.2
    IL_0060:  ldloc.3
    IL_0061:  conv.i
    IL_0062:  stloc.s    V_6
    IL_0064:  stloc.s    V_7
    IL_0066:  ldloc.s    V_7
    IL_0068:  ldloc.s    V_6
    IL_006a:  ldloc.s    V_5
    IL_006c:  ldloc.s    V_5
    IL_006e:  conv.r8
    IL_006f:  newobj     instance void valuetype [runtime]System.ValueTuple`2<int32,float64>::.ctor(!0,
                                                                                                           !1)
    IL_0074:  stelem     valuetype [runtime]System.ValueTuple`2<int32,float64>
    IL_0079:  ldloc.s    V_4
    IL_007b:  ldarg.1
    IL_007c:  add
    IL_007d:  stloc.s    V_4
    IL_007f:  ldloc.3
    IL_0080:  ldc.i4.1
    IL_0081:  conv.i8
    IL_0082:  add
    IL_0083:  stloc.3
    IL_0084:  ldloc.3
    IL_0085:  ldloc.0
    IL_0086:  blt.un.s   IL_005b

    IL_0088:  ldloc.2
    IL_0089:  ret
  } 

  .method public static int32[]  f28(int32 start,
                                     int32 step,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7,
             native int V_8,
             int32[] V_9)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.2
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldarg.1
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldarg.0
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldarg.0
    IL_0033:  ldarg.2
    IL_0034:  sub
    IL_0035:  ldarg.1
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldloc.0
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  brtrue.s   IL_004b

    IL_0045:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  conv.ovf.i.un
    IL_004d:  newarr     [runtime]System.Int32
    IL_0052:  stloc.2
    IL_0053:  ldc.i4.0
    IL_0054:  conv.i8
    IL_0055:  stloc.3
    IL_0056:  ldarg.0
    IL_0057:  stloc.s    V_4
    IL_0059:  br.s       IL_0083

    IL_005b:  ldloc.s    V_4
    IL_005d:  stloc.s    V_5
    IL_005f:  ldloc.2
    IL_0060:  ldloc.3
    IL_0061:  conv.i
    IL_0062:  stloc.s    V_6
    IL_0064:  stloc.s    V_7
    IL_0066:  ldloc.s    V_7
    IL_0068:  ldloc.s    V_6
    IL_006a:  stloc.s    V_8
    IL_006c:  stloc.s    V_9
    IL_006e:  ldloc.s    V_9
    IL_0070:  ldloc.s    V_8
    IL_0072:  ldloc.s    V_5
    IL_0074:  ldloc.s    V_5
    IL_0076:  mul
    IL_0077:  stelem.i4
    IL_0078:  ldloc.s    V_4
    IL_007a:  ldarg.1
    IL_007b:  add
    IL_007c:  stloc.s    V_4
    IL_007e:  ldloc.3
    IL_007f:  ldc.i4.1
    IL_0080:  conv.i8
    IL_0081:  add
    IL_0082:  stloc.3
    IL_0083:  ldloc.3
    IL_0084:  ldloc.0
    IL_0085:  blt.un.s   IL_005b

    IL_0087:  ldloc.2
    IL_0088:  ret
  } 

  .method public static int32[]  f29(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             int32 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             int32[] V_7)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0008:  stloc.0
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0010:  stloc.1
    IL_0011:  ldc.i4.5
    IL_0012:  conv.i8
    IL_0013:  conv.ovf.i.un
    IL_0014:  newarr     [runtime]System.Int32
    IL_0019:  stloc.2
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.3
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.s    V_4
    IL_0020:  br.s       IL_0043

    IL_0022:  ldloc.s    V_4
    IL_0024:  stloc.s    V_5
    IL_0026:  ldloc.2
    IL_0027:  ldloc.3
    IL_0028:  conv.i
    IL_0029:  stloc.s    V_6
    IL_002b:  stloc.s    V_7
    IL_002d:  ldloc.s    V_7
    IL_002f:  ldloc.s    V_6
    IL_0031:  ldloc.s    V_5
    IL_0033:  ldloc.0
    IL_0034:  add
    IL_0035:  ldloc.1
    IL_0036:  add
    IL_0037:  stelem.i4
    IL_0038:  ldloc.s    V_4
    IL_003a:  ldc.i4.2
    IL_003b:  add
    IL_003c:  stloc.s    V_4
    IL_003e:  ldloc.3
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  add
    IL_0042:  stloc.3
    IL_0043:  ldloc.3
    IL_0044:  ldc.i4.5
    IL_0045:  conv.i8
    IL_0046:  blt.un.s   IL_0022

    IL_0048:  ldloc.2
    IL_0049:  ret
  } 

  .method public static int32[]  f30(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             int32[] V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             native int V_5,
             int32[] V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0008:  stloc.0
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0010:  pop
    IL_0011:  ldc.i4.5
    IL_0012:  conv.i8
    IL_0013:  conv.ovf.i.un
    IL_0014:  newarr     [runtime]System.Int32
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.2
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.3
    IL_001f:  br.s       IL_003d

    IL_0021:  ldloc.3
    IL_0022:  stloc.s    V_4
    IL_0024:  ldloc.1
    IL_0025:  ldloc.2
    IL_0026:  conv.i
    IL_0027:  stloc.s    V_5
    IL_0029:  stloc.s    V_6
    IL_002b:  ldloc.s    V_6
    IL_002d:  ldloc.s    V_5
    IL_002f:  ldloc.s    V_4
    IL_0031:  ldloc.0
    IL_0032:  add
    IL_0033:  stelem.i4
    IL_0034:  ldloc.3
    IL_0035:  ldc.i4.2
    IL_0036:  add
    IL_0037:  stloc.3
    IL_0038:  ldloc.2
    IL_0039:  ldc.i4.1
    IL_003a:  conv.i8
    IL_003b:  add
    IL_003c:  stloc.2
    IL_003d:  ldloc.2
    IL_003e:  ldc.i4.5
    IL_003f:  conv.i8
    IL_0040:  blt.un.s   IL_0021

    IL_0042:  ldloc.1
    IL_0043:  ret
  } 

  .method public static int32[]  f31(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0008:  pop
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0010:  pop
    IL_0011:  ldc.i4.5
    IL_0012:  conv.i8
    IL_0013:  conv.ovf.i.un
    IL_0014:  newarr     [runtime]System.Int32
    IL_0019:  stloc.0
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.1
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.2
    IL_001f:  br.s       IL_0039

    IL_0021:  ldloc.2
    IL_0022:  stloc.3
    IL_0023:  ldloc.0
    IL_0024:  ldloc.1
    IL_0025:  conv.i
    IL_0026:  stloc.s    V_4
    IL_0028:  stloc.s    V_5
    IL_002a:  ldloc.s    V_5
    IL_002c:  ldloc.s    V_4
    IL_002e:  ldloc.3
    IL_002f:  stelem.i4
    IL_0030:  ldloc.2
    IL_0031:  ldc.i4.2
    IL_0032:  add
    IL_0033:  stloc.2
    IL_0034:  ldloc.1
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.1
    IL_0039:  ldloc.1
    IL_003a:  ldc.i4.5
    IL_003b:  conv.i8
    IL_003c:  blt.un.s   IL_0021

    IL_003e:  ldloc.0
    IL_003f:  ret
  } 

  .method public static int32[]  f32(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             int32[] V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             native int V_5,
             int32[] V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0008:  pop
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0010:  stloc.0
    IL_0011:  ldc.i4.5
    IL_0012:  conv.i8
    IL_0013:  conv.ovf.i.un
    IL_0014:  newarr     [runtime]System.Int32
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.2
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.3
    IL_001f:  br.s       IL_003d

    IL_0021:  ldloc.3
    IL_0022:  stloc.s    V_4
    IL_0024:  ldloc.1
    IL_0025:  ldloc.2
    IL_0026:  conv.i
    IL_0027:  stloc.s    V_5
    IL_0029:  stloc.s    V_6
    IL_002b:  ldloc.s    V_6
    IL_002d:  ldloc.s    V_5
    IL_002f:  ldloc.s    V_4
    IL_0031:  ldloc.0
    IL_0032:  add
    IL_0033:  stelem.i4
    IL_0034:  ldloc.3
    IL_0035:  ldc.i4.2
    IL_0036:  add
    IL_0037:  stloc.3
    IL_0038:  ldloc.2
    IL_0039:  ldc.i4.1
    IL_003a:  conv.i8
    IL_003b:  add
    IL_003c:  stloc.2
    IL_003d:  ldloc.2
    IL_003e:  ldc.i4.5
    IL_003f:  conv.i8
    IL_0040:  blt.un.s   IL_0021

    IL_0042:  ldloc.1
    IL_0043:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Core.Unit[] 
          f33<a>(int32 start,
                 int32 finish,
                 !!a f) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  7
    .locals init (uint64 V_0,
             uint64 V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit[] V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             native int V_6,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit[] V_7)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  call       !!0[] [runtime]System.Array::Empty<class [FSharp.Core]Microsoft.FSharp.Core.Unit>()
    IL_001c:  ret

    IL_001d:  ldloc.1
    IL_001e:  conv.ovf.i.un
    IL_001f:  newarr     [FSharp.Core]Microsoft.FSharp.Core.Unit
    IL_0024:  stloc.2
    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.3
    IL_0028:  ldarg.0
    IL_0029:  stloc.s    V_4
    IL_002b:  br.s       IL_005c

    IL_002d:  ldloc.s    V_4
    IL_002f:  stloc.s    V_5
    IL_0031:  ldloc.2
    IL_0032:  ldloc.3
    IL_0033:  conv.i
    IL_0034:  stloc.s    V_6
    IL_0036:  stloc.s    V_7
    IL_0038:  ldloc.s    V_7
    IL_003a:  ldloc.s    V_6
    IL_003c:  ldsfld     class assembly/f33@47 assembly/f33@47::@_instance
    IL_0041:  ldsfld     class assembly/'f33@47-1' assembly/'f33@47-1'::@_instance
    IL_0046:  ldnull
    IL_0047:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_004c:  stelem     [FSharp.Core]Microsoft.FSharp.Core.Unit
    IL_0051:  ldloc.s    V_4
    IL_0053:  ldc.i4.1
    IL_0054:  add
    IL_0055:  stloc.s    V_4
    IL_0057:  ldloc.3
    IL_0058:  ldc.i4.1
    IL_0059:  conv.i8
    IL_005a:  add
    IL_005b:  stloc.3
    IL_005c:  ldloc.3
    IL_005d:  ldloc.0
    IL_005e:  blt.un.s   IL_002d

    IL_0060:  ldloc.2
    IL_0061:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Core.Unit[] 
          f34<a>(int64 start,
                 int64 finish,
                 !!a f) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  7
    .locals init (uint64 V_0,
             bool V_1,
             uint64 V_2,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit[] V_3,
             bool V_4,
             uint64 V_5,
             int64 V_6,
             int64 V_7,
             native int V_8,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit[] V_9,
             uint64 V_10,
             native int V_11,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit[] V_12)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_000d

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  nop
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.m1
    IL_0010:  conv.i8
    IL_0011:  ceq
    IL_0013:  stloc.1
    IL_0014:  ldarg.1
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_0024

    IL_001d:  ldarg.1
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldc.i4.1
    IL_0021:  conv.i8
    IL_0022:  add.ovf.un
    IL_0023:  nop
    IL_0024:  stloc.2
    IL_0025:  ldloc.2
    IL_0026:  brtrue.s   IL_002e

    IL_0028:  call       !!0[] [runtime]System.Array::Empty<class [FSharp.Core]Microsoft.FSharp.Core.Unit>()
    IL_002d:  ret

    IL_002e:  ldloc.2
    IL_002f:  conv.ovf.i.un
    IL_0030:  newarr     [FSharp.Core]Microsoft.FSharp.Core.Unit
    IL_0035:  stloc.3
    IL_0036:  ldloc.1
    IL_0037:  brfalse.s  IL_0087

    IL_0039:  ldc.i4.1
    IL_003a:  stloc.s    V_4
    IL_003c:  ldc.i4.0
    IL_003d:  conv.i8
    IL_003e:  stloc.s    V_5
    IL_0040:  ldarg.0
    IL_0041:  stloc.s    V_6
    IL_0043:  br.s       IL_0080

    IL_0045:  ldloc.s    V_6
    IL_0047:  stloc.s    V_7
    IL_0049:  ldloc.3
    IL_004a:  ldloc.s    V_5
    IL_004c:  conv.i
    IL_004d:  stloc.s    V_8
    IL_004f:  stloc.s    V_9
    IL_0051:  ldloc.s    V_9
    IL_0053:  ldloc.s    V_8
    IL_0055:  ldsfld     class assembly/f34@48 assembly/f34@48::@_instance
    IL_005a:  ldsfld     class assembly/'f34@48-1' assembly/'f34@48-1'::@_instance
    IL_005f:  ldnull
    IL_0060:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_0065:  stelem     [FSharp.Core]Microsoft.FSharp.Core.Unit
    IL_006a:  ldloc.s    V_6
    IL_006c:  ldc.i4.1
    IL_006d:  conv.i8
    IL_006e:  add
    IL_006f:  stloc.s    V_6
    IL_0071:  ldloc.s    V_5
    IL_0073:  ldc.i4.1
    IL_0074:  conv.i8
    IL_0075:  add
    IL_0076:  stloc.s    V_5
    IL_0078:  ldloc.s    V_5
    IL_007a:  ldc.i4.0
    IL_007b:  conv.i8
    IL_007c:  cgt.un
    IL_007e:  stloc.s    V_4
    IL_0080:  ldloc.s    V_4
    IL_0082:  brtrue.s   IL_0045

    IL_0084:  nop
    IL_0085:  br.s       IL_00dc

    IL_0087:  ldarg.1
    IL_0088:  ldarg.0
    IL_0089:  bge.s      IL_0090

    IL_008b:  ldc.i4.0
    IL_008c:  conv.i8
    IL_008d:  nop
    IL_008e:  br.s       IL_0097

    IL_0090:  ldarg.1
    IL_0091:  ldarg.0
    IL_0092:  sub
    IL_0093:  ldc.i4.1
    IL_0094:  conv.i8
    IL_0095:  add.ovf.un
    IL_0096:  nop
    IL_0097:  stloc.s    V_5
    IL_0099:  ldc.i4.0
    IL_009a:  conv.i8
    IL_009b:  stloc.s    V_10
    IL_009d:  ldarg.0
    IL_009e:  stloc.s    V_6
    IL_00a0:  br.s       IL_00d5

    IL_00a2:  ldloc.s    V_6
    IL_00a4:  stloc.s    V_7
    IL_00a6:  ldloc.3
    IL_00a7:  ldloc.s    V_10
    IL_00a9:  conv.i
    IL_00aa:  stloc.s    V_11
    IL_00ac:  stloc.s    V_12
    IL_00ae:  ldloc.s    V_12
    IL_00b0:  ldloc.s    V_11
    IL_00b2:  ldsfld     class assembly/'f34@48-2' assembly/'f34@48-2'::@_instance
    IL_00b7:  ldsfld     class assembly/'f34@48-3' assembly/'f34@48-3'::@_instance
    IL_00bc:  ldnull
    IL_00bd:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_00c2:  stelem     [FSharp.Core]Microsoft.FSharp.Core.Unit
    IL_00c7:  ldloc.s    V_6
    IL_00c9:  ldc.i4.1
    IL_00ca:  conv.i8
    IL_00cb:  add
    IL_00cc:  stloc.s    V_6
    IL_00ce:  ldloc.s    V_10
    IL_00d0:  ldc.i4.1
    IL_00d1:  conv.i8
    IL_00d2:  add
    IL_00d3:  stloc.s    V_10
    IL_00d5:  ldloc.s    V_10
    IL_00d7:  ldloc.s    V_5
    IL_00d9:  blt.un.s   IL_00a2

    IL_00db:  nop
    IL_00dc:  ldloc.3
    IL_00dd:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Core.Unit[] 
          f35<a>(uint64 start,
                 uint64 finish,
                 !!a f) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  7
    .locals init (uint64 V_0,
             bool V_1,
             uint64 V_2,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit[] V_3,
             bool V_4,
             uint64 V_5,
             uint64 V_6,
             uint64 V_7,
             native int V_8,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit[] V_9,
             uint64 V_10,
             native int V_11,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit[] V_12)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.un.s   IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_000d

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  nop
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.m1
    IL_0010:  conv.i8
    IL_0011:  ceq
    IL_0013:  stloc.1
    IL_0014:  ldarg.1
    IL_0015:  ldarg.0
    IL_0016:  bge.un.s   IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_0024

    IL_001d:  ldarg.1
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldc.i4.1
    IL_0021:  conv.i8
    IL_0022:  add.ovf.un
    IL_0023:  nop
    IL_0024:  stloc.2
    IL_0025:  ldloc.2
    IL_0026:  brtrue.s   IL_002e

    IL_0028:  call       !!0[] [runtime]System.Array::Empty<class [FSharp.Core]Microsoft.FSharp.Core.Unit>()
    IL_002d:  ret

    IL_002e:  ldloc.2
    IL_002f:  conv.ovf.i.un
    IL_0030:  newarr     [FSharp.Core]Microsoft.FSharp.Core.Unit
    IL_0035:  stloc.3
    IL_0036:  ldloc.1
    IL_0037:  brfalse.s  IL_0087

    IL_0039:  ldc.i4.1
    IL_003a:  stloc.s    V_4
    IL_003c:  ldc.i4.0
    IL_003d:  conv.i8
    IL_003e:  stloc.s    V_5
    IL_0040:  ldarg.0
    IL_0041:  stloc.s    V_6
    IL_0043:  br.s       IL_0080

    IL_0045:  ldloc.s    V_6
    IL_0047:  stloc.s    V_7
    IL_0049:  ldloc.3
    IL_004a:  ldloc.s    V_5
    IL_004c:  conv.i
    IL_004d:  stloc.s    V_8
    IL_004f:  stloc.s    V_9
    IL_0051:  ldloc.s    V_9
    IL_0053:  ldloc.s    V_8
    IL_0055:  ldsfld     class assembly/f35@49 assembly/f35@49::@_instance
    IL_005a:  ldsfld     class assembly/'f35@49-1' assembly/'f35@49-1'::@_instance
    IL_005f:  ldnull
    IL_0060:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_0065:  stelem     [FSharp.Core]Microsoft.FSharp.Core.Unit
    IL_006a:  ldloc.s    V_6
    IL_006c:  ldc.i4.1
    IL_006d:  conv.i8
    IL_006e:  add
    IL_006f:  stloc.s    V_6
    IL_0071:  ldloc.s    V_5
    IL_0073:  ldc.i4.1
    IL_0074:  conv.i8
    IL_0075:  add
    IL_0076:  stloc.s    V_5
    IL_0078:  ldloc.s    V_5
    IL_007a:  ldc.i4.0
    IL_007b:  conv.i8
    IL_007c:  cgt.un
    IL_007e:  stloc.s    V_4
    IL_0080:  ldloc.s    V_4
    IL_0082:  brtrue.s   IL_0045

    IL_0084:  nop
    IL_0085:  br.s       IL_00dc

    IL_0087:  ldarg.1
    IL_0088:  ldarg.0
    IL_0089:  bge.un.s   IL_0090

    IL_008b:  ldc.i4.0
    IL_008c:  conv.i8
    IL_008d:  nop
    IL_008e:  br.s       IL_0097

    IL_0090:  ldarg.1
    IL_0091:  ldarg.0
    IL_0092:  sub
    IL_0093:  ldc.i4.1
    IL_0094:  conv.i8
    IL_0095:  add.ovf.un
    IL_0096:  nop
    IL_0097:  stloc.s    V_5
    IL_0099:  ldc.i4.0
    IL_009a:  conv.i8
    IL_009b:  stloc.s    V_6
    IL_009d:  ldarg.0
    IL_009e:  stloc.s    V_7
    IL_00a0:  br.s       IL_00d5

    IL_00a2:  ldloc.s    V_7
    IL_00a4:  stloc.s    V_10
    IL_00a6:  ldloc.3
    IL_00a7:  ldloc.s    V_6
    IL_00a9:  conv.i
    IL_00aa:  stloc.s    V_11
    IL_00ac:  stloc.s    V_12
    IL_00ae:  ldloc.s    V_12
    IL_00b0:  ldloc.s    V_11
    IL_00b2:  ldsfld     class assembly/'f35@49-2' assembly/'f35@49-2'::@_instance
    IL_00b7:  ldsfld     class assembly/'f35@49-3' assembly/'f35@49-3'::@_instance
    IL_00bc:  ldnull
    IL_00bd:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_00c2:  stelem     [FSharp.Core]Microsoft.FSharp.Core.Unit
    IL_00c7:  ldloc.s    V_7
    IL_00c9:  ldc.i4.1
    IL_00ca:  conv.i8
    IL_00cb:  add
    IL_00cc:  stloc.s    V_7
    IL_00ce:  ldloc.s    V_6
    IL_00d0:  ldc.i4.1
    IL_00d1:  conv.i8
    IL_00d2:  add
    IL_00d3:  stloc.s    V_6
    IL_00d5:  ldloc.s    V_6
    IL_00d7:  ldloc.s    V_5
    IL_00d9:  blt.un.s   IL_00a2

    IL_00db:  nop
    IL_00dc:  ldloc.3
    IL_00dd:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






