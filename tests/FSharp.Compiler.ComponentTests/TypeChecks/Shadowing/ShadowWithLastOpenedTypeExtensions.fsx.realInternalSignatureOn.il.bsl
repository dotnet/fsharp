




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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
{
  
  
}
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public Foo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly int32 x
    .field static assembly int32 init@4
    .method private specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public specialname static int32 get_X() cil managed
    {
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 assembly/Foo::init@4
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 assembly/Foo::x
      IL_0016:  ret
    } 

    .method public specialname static void set_X(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 assembly/Foo::init@4
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 assembly/Foo::x
      IL_0017:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly specialname static void staticInitialization@() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 assembly/Foo::x
      IL_0006:  ldc.i4.1
      IL_0007:  volatile.
      IL_0009:  stsfld     int32 assembly/Foo::init@4
      IL_000e:  ret
    } 

    .property int32 X()
    {
      .set void assembly/Foo::set_X(int32)
      .get int32 assembly/Foo::get_X()
    } 
  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'todo1@20-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
  {
    .field public valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'value'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'value') cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly/'todo1@20-1'::'value'
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> ctxt) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  ldfld      valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly/'todo1@20-1'::'value'
      IL_0007:  tail.
      IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>::Success(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!0>,
                                                                                                                                                                                                                                                                                !0)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'todo1@22-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
  {
    .field public valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'value'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'value') cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly/'todo1@22-2'::'value'
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> ctxt) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  ldfld      valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly/'todo1@22-2'::'value'
      IL_0007:  tail.
      IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>::Success(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!0>,
                                                                                                                                                                                                                                                                                !0)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit todo1@18
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>>
  {
    .field static assembly initonly class assembly/todo1@18 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  6
      .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> V_0)
      IL_0000:  ldc.i4.1
      IL_0001:  call       void assembly/Foo::set_X(int32)
      IL_0006:  call       int32 assembly/Foo::get_X()
      IL_000b:  ldc.i4.1
      IL_000c:  beq.s      IL_0023

      IL_000e:  ldc.i4.1
      IL_000f:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::NewError(!1)
      IL_0014:  stloc.0
      IL_0015:  ldloc.0
      IL_0016:  newobj     instance void assembly/'todo1@20-1'::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>)
      IL_001b:  tail.
      IL_001d:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
      IL_0022:  ret

      IL_0023:  ldnull
      IL_0024:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::NewOk(!0)
      IL_0029:  stloc.0
      IL_002a:  ldloc.0
      IL_002b:  newobj     instance void assembly/'todo1@22-2'::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>)
      IL_0030:  tail.
      IL_0032:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
      IL_0037:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/todo1@18::.ctor()
      IL_0005:  stsfld     class assembly/todo1@18 assembly/todo1@18::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'todo2@39-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
  {
    .field public valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'value'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'value') cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly/'todo2@39-1'::'value'
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> ctxt) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  ldfld      valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly/'todo2@39-1'::'value'
      IL_0007:  tail.
      IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>::Success(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!0>,
                                                                                                                                                                                                                                                                                !0)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'todo2@41-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>
  {
    .field public valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'value'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'value') cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly/'todo2@41-2'::'value'
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn Invoke(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> ctxt) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  ldfld      valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly/'todo2@41-2'::'value'
      IL_0007:  tail.
      IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>::Success(valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!0>,
                                                                                                                                                                                                                                                                                !0)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit todo2@37
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>>
  {
    .field static assembly initonly class assembly/todo2@37 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  6
      .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> V_0)
      IL_0000:  ldc.i4.2
      IL_0001:  call       void assembly/Foo::set_X(int32)
      IL_0006:  call       int32 assembly/Foo::get_X()
      IL_000b:  ldc.i4.2
      IL_000c:  beq.s      IL_0023

      IL_000e:  ldc.i4.2
      IL_000f:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::NewError(!1)
      IL_0014:  stloc.0
      IL_0015:  ldloc.0
      IL_0016:  newobj     instance void assembly/'todo2@39-1'::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>)
      IL_001b:  tail.
      IL_001d:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
      IL_0022:  ret

      IL_0023:  ldnull
      IL_0024:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!0,!1> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::NewOk(!0)
      IL_0029:  stloc.0
      IL_002a:  ldloc.0
      IL_002b:  newobj     instance void assembly/'todo2@41-2'::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>)
      IL_0030:  tail.
      IL_0032:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.AsyncPrimitives::MakeAsync<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [FSharp.Core]Microsoft.FSharp.Control.AsyncActivation`1<!!0>,class [FSharp.Core]Microsoft.FSharp.Control.AsyncReturn>)
      IL_0037:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/todo2@37::.ctor()
      IL_0005:  stsfld     class assembly/todo2@37 assembly/todo2@37::@_instance
      IL_000a:  ret
    } 

  } 

  .class abstract auto ansi sealed nested public Exts2
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  Foo.X.Static(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.2
      IL_0002:  mul
      IL_0003:  call       void assembly/Foo::set_X(int32)
      IL_0008:  ret
    } 

  } 

  .class abstract auto ansi sealed nested public Exts
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  Foo.X.Static(int32 v) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       void assembly/Foo::set_X(int32)
      IL_0006:  ret
    } 

  } 

  .field static assembly class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> todo1@16
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> matchValue@25
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> computation@25
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> todo2@35
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'matchValue@44-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> 'computation@44-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> get_todo1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::todo1@16
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> get_matchValue@25() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::matchValue@25
    IL_0005:  ret
  } 

  .method assembly specialname static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> get_computation@25() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::computation@25
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> get_todo2() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::todo2@35
    IL_0005:  ret
  } 

  .method assembly specialname static valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> 'get_matchValue@44-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::'matchValue@44-1'
    IL_0005:  ret
  } 

  .method assembly specialname static class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> 'get_computation@44-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::'computation@44-1'
    IL_0005:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  5
    IL_0000:  call       void assembly/Foo::staticInitialization@()
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
    IL_000a:  ldsfld     class assembly/todo1@18 assembly/todo1@18::@_instance
    IL_000f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
    IL_0014:  stsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::todo1@16
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::get_todo1()
    IL_001e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::computation@25
    IL_0023:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::get_computation@25()
    IL_0028:  ldnull
    IL_0029:  ldnull
    IL_002a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync::RunSynchronously<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>,
                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<valuetype [runtime]System.Threading.CancellationToken>)
    IL_002f:  stsfld     valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::matchValue@25
    IL_0034:  ldsflda    valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::matchValue@25
    IL_0039:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::get_Tag()
    IL_003e:  ldc.i4.1
    IL_003f:  bne.un.s   IL_0043

    IL_0041:  br.s       IL_0045

    IL_0043:  br.s       IL_0054

    IL_0045:  ldsflda    valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::matchValue@25
    IL_004a:  call       instance !1 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::get_ErrorValue()
    IL_004f:  call       void [runtime]System.Environment::Exit(int32)
    IL_0054:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_DefaultAsyncBuilder()
    IL_0059:  ldsfld     class assembly/todo2@37 assembly/todo2@37::@_instance
    IL_005e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0> [FSharp.Core]Microsoft.FSharp.Control.FSharpAsyncBuilder::Delay<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>>)
    IL_0063:  stsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::todo2@35
    IL_0068:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::get_todo2()
    IL_006d:  stsfld     class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::'computation@44-1'
    IL_0072:  call       class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::'get_computation@44-1'()
    IL_0077:  ldnull
    IL_0078:  ldnull
    IL_0079:  call       !!0 [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync::RunSynchronously<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>(class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<!!0>,
                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>,
                                                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<valuetype [runtime]System.Threading.CancellationToken>)
    IL_007e:  stsfld     valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::'matchValue@44-1'
    IL_0083:  ldsflda    valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::'matchValue@44-1'
    IL_0088:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::get_Tag()
    IL_008d:  ldc.i4.1
    IL_008e:  bne.un.s   IL_0092

    IL_0090:  br.s       IL_0094

    IL_0092:  br.s       IL_00a3

    IL_0094:  ldsflda    valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::'matchValue@44-1'
    IL_0099:  call       instance !1 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::get_ErrorValue()
    IL_009e:  call       void [runtime]System.Environment::Exit(int32)
    IL_00a3:  ret
  } 

  .property class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>
          todo1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::get_todo1()
  } 
  .property valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
          matchValue@25()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::get_matchValue@25()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>
          computation@25()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::get_computation@25()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>
          todo2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::get_todo2()
  } 
  .property valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
          'matchValue@44-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> assembly::'get_matchValue@44-1'()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>>
          'computation@44-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>> assembly::'get_computation@44-1'()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






