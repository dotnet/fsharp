
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly ListExpressionStepping06
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ListExpressionStepping06
{
  // Offset: 0x00000000 Length: 0x000002F0
  // WARNING: managed resource file FSharpSignatureData.ListExpressionStepping06 created
}
.mresource public FSharpOptimizationData.ListExpressionStepping06
{
  // Offset: 0x000002F8 Length: 0x000000B9
  // WARNING: managed resource file FSharpOptimizationData.ListExpressionStepping06 created
}
.module ListExpressionStepping06.exe
// MVID: {624FDC53-C77E-5A81-A745-038353DC4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000229CF350000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ListExpressionSteppingTest6
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public ListExpressionSteppingTest6
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            get_es() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$ListExpressionStepping06>'.$ListExpressionSteppingTest6::es@5
      IL_0005:  ret
    } // end of method ListExpressionSteppingTest6::get_es

    .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
            f7() cil managed
    {
      // Code size       179 (0xb3)
      .maxstack  4
      .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
               class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
               int32 V_3,
               class [System.Runtime]System.IDisposable V_4,
               class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> V_5,
               class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32> V_6,
               int32 V_7,
               class [System.Runtime]System.IDisposable V_8)
      IL_0000:  nop
      IL_0001:  nop
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::get_es()
      IL_0007:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_000c:  stloc.1
      .try
      {
        IL_000d:  br.s       IL_002f

        IL_000f:  ldloc.1
        IL_0010:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0015:  stloc.3
        IL_0016:  ldstr      "hello"
        IL_001b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0020:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0025:  pop
        IL_0026:  ldloca.s   V_0
        IL_0028:  ldloc.3
        IL_0029:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
        IL_002e:  nop
        IL_002f:  ldloc.1
        IL_0030:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
        IL_0035:  brtrue.s   IL_000f

        IL_0037:  ldnull
        IL_0038:  stloc.2
        IL_0039:  leave.s    IL_0050

      }  // end .try
      finally
      {
        IL_003b:  ldloc.1
        IL_003c:  isinst     [System.Runtime]System.IDisposable
        IL_0041:  stloc.s    V_4
        IL_0043:  ldloc.s    V_4
        IL_0045:  brfalse.s  IL_004f

        IL_0047:  ldloc.s    V_4
        IL_0049:  callvirt   instance void [System.Runtime]System.IDisposable::Dispose()
        IL_004e:  endfinally
        IL_004f:  endfinally
      }  // end handler
      IL_0050:  ldloc.2
      IL_0051:  pop
      IL_0052:  nop
      IL_0053:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::get_es()
      IL_0058:  callvirt   instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<!0> class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_005d:  stloc.s    V_5
      .try
      {
        IL_005f:  br.s       IL_0084

        IL_0061:  ldloc.s    V_5
        IL_0063:  callvirt   instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0068:  stloc.s    V_7
        IL_006a:  ldstr      "goodbye"
        IL_006f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0074:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0079:  pop
        IL_007a:  ldloca.s   V_0
        IL_007c:  ldloc.s    V_7
        IL_007e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
        IL_0083:  nop
        IL_0084:  ldloc.s    V_5
        IL_0086:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
        IL_008b:  brtrue.s   IL_0061

        IL_008d:  ldnull
        IL_008e:  stloc.s    V_6
        IL_0090:  leave.s    IL_00a8

      }  // end .try
      finally
      {
        IL_0092:  ldloc.s    V_5
        IL_0094:  isinst     [System.Runtime]System.IDisposable
        IL_0099:  stloc.s    V_8
        IL_009b:  ldloc.s    V_8
        IL_009d:  brfalse.s  IL_00a7

        IL_009f:  ldloc.s    V_8
        IL_00a1:  callvirt   instance void [System.Runtime]System.IDisposable::Dispose()
        IL_00a6:  endfinally
        IL_00a7:  endfinally
      }  // end handler
      IL_00a8:  ldloc.s    V_6
      IL_00aa:  pop
      IL_00ab:  ldloca.s   V_0
      IL_00ad:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
      IL_00b2:  ret
    } // end of method ListExpressionSteppingTest6::f7

    .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
            es()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::get_es()
    } // end of property ListExpressionSteppingTest6::es
  } // end of class ListExpressionSteppingTest6

} // end of class ListExpressionSteppingTest6

.class private abstract auto ansi sealed '<StartupCode$ListExpressionStepping06>'.$ListExpressionSteppingTest6
       extends [System.Runtime]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> es@5
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       37 (0x25)
    .maxstack  6
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  dup
    IL_0018:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$ListExpressionStepping06>'.$ListExpressionSteppingTest6::es@5
    IL_001d:  stloc.0
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest6/ListExpressionSteppingTest6::f7()
    IL_0023:  pop
    IL_0024:  ret
  } // end of method $ListExpressionSteppingTest6::main@

} // end of class '<StartupCode$ListExpressionStepping06>'.$ListExpressionSteppingTest6


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\ListExpressionStepping\ListExpressionStepping06_fs\ListExpressionStepping06.res
