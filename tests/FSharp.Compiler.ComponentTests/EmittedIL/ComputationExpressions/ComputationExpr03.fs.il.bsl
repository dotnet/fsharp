
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern ComputationExprLibrary
{
  .ver 0:0:0:0
}
.assembly ComputationExpr03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ComputationExpr03
{
  // Offset: 0x00000000 Length: 0x0000028B
  // WARNING: managed resource file FSharpSignatureData.ComputationExpr03 created
}
.mresource public FSharpOptimizationData.ComputationExpr03
{
  // Offset: 0x00000290 Length: 0x00000082
  // WARNING: managed resource file FSharpOptimizationData.ComputationExpr03 created
}
.module ComputationExpr03.exe
// MVID: {6229A4F2-3649-E566-A745-0383F2A42962}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x007F0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Program
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit res2@9
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res2@9::builder@
      IL_000d:  ret
    } // end of method res2@9::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  nop
      IL_0001:  ldstr      "hello"
      IL_0006:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0010:  pop
      IL_0011:  ldstr      "hello"
      IL_0016:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_001b:  stloc.0
      IL_001c:  ldarg.0
      IL_001d:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res2@9::builder@
      IL_0022:  ldloc.0
      IL_0023:  ldloc.0
      IL_0024:  add
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_002c:  ret
    } // end of method res2@9::Invoke

  } // end of class res2@9

  .class auto ansi serializable sealed nested assembly beforefieldinit 'res3@18-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res3@18-2'::builder@
      IL_000d:  ret
    } // end of method 'res3@18-2'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       43 (0x2b)
      .maxstack  6
      .locals init (int32 V_0)
      IL_0000:  nop
      IL_0001:  ldstr      "hello"
      IL_0006:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0010:  pop
      IL_0011:  ldstr      "hello"
      IL_0016:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_001b:  stloc.0
      IL_001c:  ldarg.0
      IL_001d:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res3@18-2'::builder@
      IL_0022:  ldc.i4.1
      IL_0023:  tail.
      IL_0025:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_002a:  ret
    } // end of method 'res3@18-2'::Invoke

  } // end of class 'res3@18-2'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'res3@21-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res3@21-3'::builder@
      IL_000d:  ret
    } // end of method 'res3@21-3'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(int32 _arg2) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  6
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res3@21-3'::builder@
      IL_0008:  ldc.i4.1
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Return<int32>(!!0)
      IL_0010:  ret
    } // end of method 'res3@21-3'::Invoke

  } // end of class 'res3@21-3'

  .class auto ansi serializable sealed nested assembly beforefieldinit 'res3@16-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res3@16-1'::builder@
      IL_000d:  ret
    } // end of method 'res3@16-1'::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(int32 _arg1) cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  7
      .locals init (int32 V_0,
               class [ComputationExprLibrary]Library.EventuallyBuilder V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res3@16-1'::builder@
      IL_0008:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  ldloc.1
      IL_0010:  newobj     instance void Program/'res3@18-2'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0015:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
      IL_001a:  ldarg.0
      IL_001b:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/'res3@16-1'::builder@
      IL_0020:  newobj     instance void Program/'res3@21-3'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!1> [ComputationExprLibrary]Library.EventuallyBuilder::Bind<int32,int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>,
                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [ComputationExprLibrary]Library.Eventually`1<!!1>>)
      IL_002c:  ret
    } // end of method 'res3@16-1'::Invoke

  } // end of class 'res3@16-1'

  .class auto ansi serializable sealed nested assembly beforefieldinit res3@15
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>
  {
    .field public class [ComputationExprLibrary]Library.EventuallyBuilder builder@
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [ComputationExprLibrary]Library.EventuallyBuilder builder@) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res3@15::builder@
      IL_000d:  ret
    } // end of method res3@15::.ctor

    .method public strict virtual instance class [ComputationExprLibrary]Library.Eventually`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       30 (0x1e)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res3@15::builder@
      IL_0006:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> Program::get_res2()
      IL_000b:  ldarg.0
      IL_000c:  ldfld      class [ComputationExprLibrary]Library.EventuallyBuilder Program/res3@15::builder@
      IL_0011:  newobj     instance void Program/'res3@16-1'::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
      IL_0016:  tail.
      IL_0018:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!1> [ComputationExprLibrary]Library.EventuallyBuilder::Bind<int32,int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>,
                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [ComputationExprLibrary]Library.Eventually`1<!!1>>)
      IL_001d:  ret
    } // end of method res3@15::Invoke

  } // end of class res3@15

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res2() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr03>'.$Program::res2@7
    IL_0005:  ret
  } // end of method Program::get_res2

  .method public specialname static class [ComputationExprLibrary]Library.Eventually`1<int32> 
          get_res3() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr03>'.$Program::res3@13
    IL_0005:  ret
  } // end of method Program::get_res3

  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> Program::get_res2()
  } // end of property Program::res2
  .property class [ComputationExprLibrary]Library.Eventually`1<int32>
          res3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [ComputationExprLibrary]Library.Eventually`1<int32> Program::get_res3()
  } // end of property Program::res3
} // end of class Program

.class private abstract auto ansi sealed '<StartupCode$ComputationExpr03>'.$Program
       extends [mscorlib]System.Object
{
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res2@7
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [ComputationExprLibrary]Library.Eventually`1<int32> res3@13
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       82 (0x52)
    .maxstack  4
    .locals init (class [ComputationExprLibrary]Library.Eventually`1<int32> V_0,
             class [ComputationExprLibrary]Library.Eventually`1<int32> V_1,
             class [ComputationExprLibrary]Library.EventuallyBuilder V_2,
             class [ComputationExprLibrary]Library.Eventually`1<int32> V_3,
             class [ComputationExprLibrary]Library.EventuallyBuilder V_4,
             class [ComputationExprLibrary]Library.Eventually`1<int32> V_5)
    IL_0000:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_0005:  stloc.2
    IL_0006:  ldloc.2
    IL_0007:  ldloc.2
    IL_0008:  newobj     instance void Program/res2@9::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_000d:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_0012:  dup
    IL_0013:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr03>'.$Program::res2@7
    IL_0018:  stloc.0
    IL_0019:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> Program::get_res2()
    IL_001e:  stloc.3
    IL_001f:  ldloc.3
    IL_0020:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0025:  pop
    IL_0026:  call       class [ComputationExprLibrary]Library.EventuallyBuilder [ComputationExprLibrary]Library.TheEventuallyBuilder::get_eventually()
    IL_002b:  stloc.s    V_4
    IL_002d:  ldloc.s    V_4
    IL_002f:  ldloc.s    V_4
    IL_0031:  newobj     instance void Program/res3@15::.ctor(class [ComputationExprLibrary]Library.EventuallyBuilder)
    IL_0036:  callvirt   instance class [ComputationExprLibrary]Library.Eventually`1<!!0> [ComputationExprLibrary]Library.EventuallyBuilder::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [ComputationExprLibrary]Library.Eventually`1<!!0>>)
    IL_003b:  dup
    IL_003c:  stsfld     class [ComputationExprLibrary]Library.Eventually`1<int32> '<StartupCode$ComputationExpr03>'.$Program::res3@13
    IL_0041:  stloc.1
    IL_0042:  call       class [ComputationExprLibrary]Library.Eventually`1<int32> Program::get_res3()
    IL_0047:  stloc.s    V_5
    IL_0049:  ldloc.s    V_5
    IL_004b:  call       !!0 [ComputationExprLibrary]Library.EventuallyModule::force<int32>(class [ComputationExprLibrary]Library.Eventually`1<!!0>)
    IL_0050:  pop
    IL_0051:  ret
  } // end of method $Program::main@

} // end of class '<StartupCode$ComputationExpr03>'.$Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\ComputationExpressions\ComputationExpr03_fs\ComputationExpr03.res
