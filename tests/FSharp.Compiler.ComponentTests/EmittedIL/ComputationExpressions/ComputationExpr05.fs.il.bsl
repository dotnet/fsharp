




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern ComputationExprLibrary
{
  .ver 0:0:0:0
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
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed Program
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'res5@10-1'
         extends [runtime]System.Object
         implements [runtime]System.IDisposable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [runtime]System.IDisposable::Dispose
      
      .maxstack  4
      .locals init (class [runtime]System.IDisposable V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'res5@11-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.IDisposable,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.IDisposable,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res5@11-2'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [runtime]System.IDisposable _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.IDisposable V_0,
               int32 V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  ldstr      "hello"
      IL_0008:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0012:  pop
      IL_0013:  ldstr      "hello"
      IL_0018:  callvirt   instance int32 [runtime]System.String::get_Length()
      IL_001d:  stloc.1
      IL_001e:  ldarg.0
      IL_001f:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res5@11-2'::builder@
      IL_0024:  ldc.i4.1
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_002c:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit res5@9
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res5@9::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  nop
      IL_0001:  nop
      IL_0002:  ldstr      "hello"
      IL_0007:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0011:  pop
      IL_0012:  ldstr      "hello"
      IL_0017:  callvirt   instance int32 [runtime]System.String::get_Length()
      IL_001c:  stloc.0
      IL_001d:  ldarg.0
      IL_001e:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res5@9::builder@
      IL_0023:  newobj     instance void Program/'res5@10-1'::.ctor()
      IL_0028:  ldarg.0
      IL_0029:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res5@9::builder@
      IL_002e:  newobj     instance void Program/'res5@11-2'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0033:  tail.
      IL_0035:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Using<int32>(class [runtime]System.IDisposable,
                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.IDisposable,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_003a:  ret
    } 

  } 

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res5() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$assembly>'.$Program::res5@7
    IL_0005:  ret
  } 

  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res5()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> Program::get_res5()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Program
       extends [runtime]System.Object
{
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res5@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  4
    .locals init (class [ComputationExprLibrary]Library.Eventually`1<int32> V_0,
             class [ComputationExprLibrary]Library.EventuallyBuilder V_1,
             class [ComputationExprLibrary]Library.Eventually`1<int32> V_2)
    IL_0000:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldloc.1
    IL_0008:  newobj     instance void Program/res5@9::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_000d:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_0012:  dup
    IL_0013:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$assembly>'.$Program::res5@7
    IL_0018:  stloc.0
    IL_0019:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> Program::get_res5()
    IL_001e:  stloc.2
    IL_001f:  ldloc.2
    IL_0020:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0025:  pop
    IL_0026:  ret
  } 

} 






