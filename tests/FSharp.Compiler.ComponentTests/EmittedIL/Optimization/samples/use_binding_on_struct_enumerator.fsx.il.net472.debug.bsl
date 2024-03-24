




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:0:0:0
}
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





.class public abstract auto ansi sealed Use_binding_on_struct_enumerator
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit testFoldOnListT@32
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,int32,string>
  {
    .field static assembly initonly class Use_binding_on_struct_enumerator/testFoldOnListT@32 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,int32,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string 
            Invoke(string state,
                   int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  box        [runtime]System.Int32
      IL_0007:  unbox.any  [runtime]System.IFormattable
      IL_000c:  ldnull
      IL_000d:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0012:  callvirt   instance string [netstandard]System.IFormattable::ToString(string,
                                                                                      class [netstandard]System.IFormatProvider)
      IL_0017:  call       string [runtime]System.String::Concat(string,
                                                                  string)
      IL_001c:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Use_binding_on_struct_enumerator/testFoldOnListT@32::.ctor()
      IL_0005:  stsfld     class Use_binding_on_struct_enumerator/testFoldOnListT@32 Use_binding_on_struct_enumerator/testFoldOnListT@32::@_instance
      IL_000a:  ret
    } 

  } 

  .field static assembly valuetype '<PrivateImplementationDetails$assembly>'/T71036_24Bytes@ field71037@ at I_00003917
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public static !!State  FoldStrong<State,T,(class [runtime]System.Collections.Generic.IEnumerator`1<!!T>) TEnumerator>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!State,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!T,!!State>> folder,
                                                                                                                                 !!State state,
                                                                                                                                 !!TEnumerator enumerator) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationSourceNameAttribute::.ctor(string) = ( 01 00 0A 66 6F 6C 64 53 74 72 6F 6E 67 00 00 )    
    
    .maxstack  5
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!!State,!!T,!!State> V_0,
             !!TEnumerator V_1,
             !!State V_2,
             !!TEnumerator V_3)
    IL_0000:  ldarg.0
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!0,!1,!2> class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!!State,!!T,!!State>::Adapt(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!2>>)
    IL_0006:  stloc.0
    IL_0007:  ldarg.2
    IL_0008:  stloc.1
    IL_0009:  ldarg.1
    IL_000a:  stloc.2
    IL_000b:  br.s       IL_0024

    IL_000d:  ldloc.0
    IL_000e:  ldloc.2
    IL_000f:  ldarg.2
    IL_0010:  stloc.3
    IL_0011:  ldloca.s   V_3
    IL_0013:  constrained. !!TEnumerator
    IL_0019:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<!!T>::get_Current()
    IL_001e:  callvirt   instance !2 class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<!!State,!!T,!!State>::Invoke(!0,
                                                                                                                                          !1)
    IL_0023:  stloc.2
    IL_0024:  ldloca.s   V_1
    IL_0026:  constrained. !!TEnumerator
    IL_002c:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
    IL_0031:  brtrue.s   IL_000d

    IL_0033:  ldloc.2
    IL_0034:  ret
  } 

  .method public static !!State  Fold<State,T,TSeq,(class [runtime]System.Collections.Generic.IEnumerator`1<!!T>) TEnumerator>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!State,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!T,!!State>> folder,
                                                                                                                                !!State state,
                                                                                                                                !!TSeq source) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationSourceNameAttribute::.ctor(string) = ( 01 00 04 66 6F 6C 64 00 00 )                      
    
    .maxstack  5
    .locals init (!!TEnumerator V_0,
             !!State V_1,
             class [runtime]System.IDisposable V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  brfalse.s  IL_000b

    IL_0003:  ldnull
    IL_0004:  unbox.any  !!TEnumerator
    IL_0009:  br.s       IL_0016

    IL_000b:  ldstr      "Dynamic invocation of GetEnumerator is not supported"
    IL_0010:  newobj     instance void [runtime]System.NotSupportedException::.ctor(string)
    IL_0015:  throw

    IL_0016:  stloc.0
    .try
    {
      IL_0017:  ldarg.0
      IL_0018:  ldarg.1
      IL_0019:  ldloc.0
      IL_001a:  call       !!0 Use_binding_on_struct_enumerator::FoldStrong<!!0,!!1,!!3>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,!!0>>,
                                                                                         !!0,
                                                                                         !!2)
      IL_001f:  stloc.1
      IL_0020:  leave.s    IL_0039

    }  
    finally
    {
      IL_0022:  ldloc.0
      IL_0023:  box        !!TEnumerator
      IL_0028:  isinst     [runtime]System.IDisposable
      IL_002d:  stloc.2
      IL_002e:  ldloc.2
      IL_002f:  brfalse.s  IL_0038

      IL_0031:  ldloc.2
      IL_0032:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0037:  endfinally
      IL_0038:  endfinally
    }  
    IL_0039:  ldloc.1
    IL_003a:  ret
  } 

  .method public static !!State  Fold$W<State,T,TSeq,(class [runtime]System.Collections.Generic.IEnumerator`1<!!T>) TEnumerator>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!TSeq,!!TEnumerator> getEnumerator,
                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!State,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!T,!!State>> folder,
                                                                                                                                  !!State state,
                                                                                                                                  !!TSeq source) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationSourceNameAttribute::.ctor(string) = ( 01 00 04 66 6F 6C 64 00 00 )                      
    
    .maxstack  5
    .locals init (!!TEnumerator V_0,
             !!State V_1,
             class [runtime]System.IDisposable V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.3
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!TSeq,!!TEnumerator>::Invoke(!0)
    IL_0007:  stloc.0
    .try
    {
      IL_0008:  ldarg.1
      IL_0009:  ldarg.2
      IL_000a:  ldloc.0
      IL_000b:  call       !!0 Use_binding_on_struct_enumerator::FoldStrong<!!0,!!1,!!3>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,!!0>>,
                                                                                         !!0,
                                                                                         !!2)
      IL_0010:  stloc.1
      IL_0011:  leave.s    IL_002a

    }  
    finally
    {
      IL_0013:  ldloc.0
      IL_0014:  box        !!TEnumerator
      IL_0019:  isinst     [runtime]System.IDisposable
      IL_001e:  stloc.2
      IL_001f:  ldloc.2
      IL_0020:  brfalse.s  IL_0029

      IL_0022:  ldloc.2
      IL_0023:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0028:  endfinally
      IL_0029:  endfinally
    }  
    IL_002a:  ldloc.1
    IL_002b:  ret
  } 

  .method public static string  testFoldOnListT() cil managed
  {
    
    .maxstack  5
    .locals init (class [runtime]System.Collections.Generic.List`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>> V_1,
             valuetype [runtime]System.Collections.Generic.List`1/Enumerator<int32> V_2,
             string V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  ldc.i4.6
    IL_0001:  newarr     [runtime]System.Int32
    IL_0006:  dup
    IL_0007:  ldtoken    field valuetype '<PrivateImplementationDetails$assembly>'/T71036_24Bytes@ Use_binding_on_struct_enumerator::field71037@
    IL_000c:  call       void [runtime]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [runtime]System.Array,
                                                                                                        valuetype [runtime]System.RuntimeFieldHandle)
    IL_0011:  newobj     instance void class [runtime]System.Collections.Generic.List`1<int32>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0016:  stloc.0
    IL_0017:  ldsfld     class Use_binding_on_struct_enumerator/testFoldOnListT@32 Use_binding_on_struct_enumerator/testFoldOnListT@32::@_instance
    IL_001c:  stloc.1
    IL_001d:  ldloc.0
    IL_001e:  callvirt   instance valuetype [runtime]System.Collections.Generic.List`1/Enumerator<!0> class [runtime]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_0023:  stloc.2
    .try
    {
      IL_0024:  ldloc.1
      IL_0025:  ldstr      ""
      IL_002a:  ldloc.2
      IL_002b:  call       !!0 Use_binding_on_struct_enumerator::FoldStrong<string,int32,valuetype [runtime]System.Collections.Generic.List`1/Enumerator<int32>>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,!!0>>,
                                                                                                                                                                  !!0,
                                                                                                                                                                  !!2)
      IL_0030:  stloc.3
      IL_0031:  leave.s    IL_004d

    }  
    finally
    {
      IL_0033:  ldloc.2
      IL_0034:  box        valuetype [runtime]System.Collections.Generic.List`1/Enumerator<int32>
      IL_0039:  isinst     [runtime]System.IDisposable
      IL_003e:  stloc.s    V_4
      IL_0040:  ldloc.s    V_4
      IL_0042:  brfalse.s  IL_004c

      IL_0044:  ldloc.s    V_4
      IL_0046:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_004b:  endfinally
      IL_004c:  endfinally
    }  
    IL_004d:  ldloc.3
    IL_004e:  ret
  } 

  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Use_binding_on_struct_enumerator$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Use_binding_on_struct_enumerator$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Use_binding_on_struct_enumerator$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       string Use_binding_on_struct_enumerator::testFoldOnListT()
    IL_0005:  call       void [runtime]System.Console::WriteLine(string)
    IL_000a:  ret
  } 

} 

.class private abstract auto ansi sealed beforefieldinit '<PrivateImplementationDetails$assembly>'
       extends [runtime]System.Object
{
  .class explicit ansi sealed nested assembly beforefieldinit T71036_24Bytes@
         extends [runtime]System.ValueType
  {
    .pack 0
    .size 24
  } 

} 




.data cil I_00003917 = bytearray (
                 01 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00
                 05 00 00 00 08 00 00 00) 


