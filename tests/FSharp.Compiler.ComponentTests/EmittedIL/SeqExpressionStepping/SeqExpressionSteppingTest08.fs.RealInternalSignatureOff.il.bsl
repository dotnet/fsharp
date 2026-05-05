




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





.class public abstract auto ansi sealed SeqExpressionSteppingTest8
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest8
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname directValues@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public int32 pc
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname instance void  .ctor(int32 pc, int32 current) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::pc
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::current
        IL_000e:  ldarg.0
        IL_000f:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0014:  ret
      } 

      .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        
        .maxstack  6
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001f,
                              IL_0022,
                              IL_0025,
                              IL_0028)
        IL_001d:  br.s       IL_002b

        IL_001f:  nop
        IL_0020:  br.s       IL_003e

        IL_0022:  nop
        IL_0023:  br.s       IL_004e

        IL_0025:  nop
        IL_0026:  br.s       IL_005e

        IL_0028:  nop
        IL_0029:  br.s       IL_0065

        IL_002b:  nop
        IL_002c:  br.s       IL_002e

        IL_002e:  ldarg.0
        IL_002f:  ldc.i4.1
        IL_0030:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::pc
        IL_0035:  ldarg.0
        IL_0036:  ldc.i4.1
        IL_0037:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::current
        IL_003c:  ldc.i4.1
        IL_003d:  ret

        IL_003e:  ldarg.0
        IL_003f:  ldc.i4.2
        IL_0040:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::pc
        IL_0045:  ldarg.0
        IL_0046:  ldc.i4.2
        IL_0047:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::current
        IL_004c:  ldc.i4.1
        IL_004d:  ret

        IL_004e:  ldarg.0
        IL_004f:  ldc.i4.3
        IL_0050:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::pc
        IL_0055:  ldarg.0
        IL_0056:  ldc.i4.3
        IL_0057:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::current
        IL_005c:  ldc.i4.1
        IL_005d:  ret

        IL_005e:  ldarg.0
        IL_005f:  ldc.i4.4
        IL_0060:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::pc
        IL_0065:  ldarg.0
        IL_0066:  ldc.i4.0
        IL_0067:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::current
        IL_006c:  ldc.i4.0
        IL_006d:  ret
      } 

      .method public strict virtual instance void Close() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.4
        IL_0002:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::pc
        IL_0007:  ret
      } 

      .method public strict virtual instance bool get_CheckClose() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::pc
        IL_0006:  switch     ( 
                              IL_0021,
                              IL_0024,
                              IL_0027,
                              IL_002a,
                              IL_002d)
        IL_001f:  br.s       IL_0030

        IL_0021:  nop
        IL_0022:  br.s       IL_0039

        IL_0024:  nop
        IL_0025:  br.s       IL_0037

        IL_0027:  nop
        IL_0028:  br.s       IL_0035

        IL_002a:  nop
        IL_002b:  br.s       IL_0033

        IL_002d:  nop
        IL_002e:  br.s       IL_0039

        IL_0030:  nop
        IL_0031:  br.s       IL_0033

        IL_0033:  ldc.i4.0
        IL_0034:  ret

        IL_0035:  ldc.i4.0
        IL_0036:  ret

        IL_0037:  ldc.i4.0
        IL_0038:  ret

        IL_0039:  ldc.i4.0
        IL_003a:  ret
      } 

      .method public strict virtual instance int32 get_LastGenerated() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::current
        IL_0006:  ret
      } 

      .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldc.i4.0
        IL_0001:  ldc.i4.0
        IL_0002:  newobj     instance void SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::.ctor(int32,
                                                                                                                       int32)
        IL_0007:  ret
      } 

    } 

    .class auto autochar serializable sealed nested assembly beforefieldinit specialname doubledWithForArrow@9
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public class [runtime]System.Collections.Generic.IEnumerable`1<int32> xs
      .field public class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 pc
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerable`1<int32> xs,
                                   class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                   int32 pc,
                                   int32 current) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::xs
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::'enum'
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    current
        IL_0018:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::current
        IL_001d:  ldarg.0
        IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0023:  ret
      } 

      .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        
        .maxstack  7
        .locals init (int32 V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001b,
                              IL_001e,
                              IL_0021)
        IL_0019:  br.s       IL_0024

        IL_001b:  nop
        IL_001c:  br.s       IL_006c

        IL_001e:  nop
        IL_001f:  br.s       IL_005f

        IL_0021:  nop
        IL_0022:  br.s       IL_008d

        IL_0024:  nop
        IL_0025:  br.s       IL_0027

        IL_0027:  ldarg.0
        IL_0028:  ldarg.0
        IL_0029:  ldfld      class [runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::xs
        IL_002e:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
        IL_0033:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::'enum'
        IL_0038:  ldarg.0
        IL_0039:  ldc.i4.1
        IL_003a:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
        IL_003f:  br.s       IL_005f

        IL_0041:  ldarg.0
        IL_0042:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::'enum'
        IL_0047:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_004c:  stloc.0
        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.2
        IL_004f:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
        IL_0054:  ldarg.0
        IL_0055:  ldloc.0
        IL_0056:  ldc.i4.2
        IL_0057:  mul
        IL_0058:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::current
        IL_005d:  ldc.i4.1
        IL_005e:  ret

        IL_005f:  ldarg.0
        IL_0060:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::'enum'
        IL_0065:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
        IL_006a:  brtrue.s   IL_0041

        IL_006c:  ldarg.0
        IL_006d:  ldc.i4.3
        IL_006e:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
        IL_0073:  ldarg.0
        IL_0074:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::'enum'
        IL_0079:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_007e:  nop
        IL_007f:  ldarg.0
        IL_0080:  ldnull
        IL_0081:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::'enum'
        IL_0086:  ldarg.0
        IL_0087:  ldc.i4.3
        IL_0088:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
        IL_008d:  ldarg.0
        IL_008e:  ldc.i4.0
        IL_008f:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::current
        IL_0094:  ldc.i4.0
        IL_0095:  ret
      } 

      .method public strict virtual instance void Close() cil managed
      {
        
        .maxstack  6
        .locals init (class [runtime]System.Exception V_0,
                 class [runtime]System.Exception V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
        IL_0006:  ldc.i4.3
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0013)
        IL_0011:  br.s       IL_0016

        IL_0013:  nop
        IL_0014:  br.s       IL_0078

        IL_0016:  nop
        .try
        {
          IL_0017:  ldarg.0
          IL_0018:  ldfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
          IL_001d:  switch     ( 
                                IL_0034,
                                IL_0037,
                                IL_003a,
                                IL_003d)
          IL_0032:  br.s       IL_0040

          IL_0034:  nop
          IL_0035:  br.s       IL_0058

          IL_0037:  nop
          IL_0038:  br.s       IL_0044

          IL_003a:  nop
          IL_003b:  br.s       IL_0043

          IL_003d:  nop
          IL_003e:  br.s       IL_0058

          IL_0040:  nop
          IL_0041:  br.s       IL_0043

          IL_0043:  nop
          IL_0044:  ldarg.0
          IL_0045:  ldc.i4.3
          IL_0046:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
          IL_004b:  ldarg.0
          IL_004c:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::'enum'
          IL_0051:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
          IL_0056:  nop
          IL_0057:  nop
          IL_0058:  ldarg.0
          IL_0059:  ldc.i4.3
          IL_005a:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
          IL_005f:  ldarg.0
          IL_0060:  ldc.i4.0
          IL_0061:  stfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::current
          IL_0066:  leave.s    IL_0072

        }  
        catch [runtime]System.Object 
        {
          IL_0068:  castclass  [runtime]System.Exception
          IL_006d:  stloc.1
          IL_006e:  ldloc.1
          IL_006f:  stloc.0
          IL_0070:  leave.s    IL_0072

        }  
        IL_0072:  nop
        IL_0073:  br         IL_0000

        IL_0078:  ldloc.0
        IL_0079:  brfalse.s  IL_007d

        IL_007b:  ldloc.0
        IL_007c:  throw

        IL_007d:  ret
      } 

      .method public strict virtual instance bool get_CheckClose() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::pc
        IL_0006:  switch     ( 
                              IL_001d,
                              IL_0020,
                              IL_0023,
                              IL_0026)
        IL_001b:  br.s       IL_0029

        IL_001d:  nop
        IL_001e:  br.s       IL_0030

        IL_0020:  nop
        IL_0021:  br.s       IL_002e

        IL_0023:  nop
        IL_0024:  br.s       IL_002c

        IL_0026:  nop
        IL_0027:  br.s       IL_0030

        IL_0029:  nop
        IL_002a:  br.s       IL_002c

        IL_002c:  ldc.i4.1
        IL_002d:  ret

        IL_002e:  ldc.i4.1
        IL_002f:  ret

        IL_0030:  ldc.i4.0
        IL_0031:  ret
      } 

      .method public strict virtual instance int32 get_LastGenerated() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::current
        IL_0006:  ret
      } 

      .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class [runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::xs
        IL_0006:  ldnull
        IL_0007:  ldc.i4.0
        IL_0008:  ldc.i4.0
        IL_0009:  newobj     instance void SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<int32>,
                                                                                                                              class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                                              int32,
                                                                                                                              int32)
        IL_000e:  ret
      } 

    } 

    .method public static class [runtime]System.Collections.Generic.IEnumerable`1<int32> directValues() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ldc.i4.0
      IL_0002:  newobj     instance void SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/directValues@6::.ctor(int32,
                                                                                                                     int32)
      IL_0007:  ret
    } 

    .method public static class [runtime]System.Collections.Generic.IEnumerable`1<int32> doubledWithForArrow(class [runtime]System.Collections.Generic.IEnumerable`1<int32> xs) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  newobj     instance void SeqExpressionSteppingTest8/SeqExpressionSteppingTest8/doubledWithForArrow@9::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<int32>,
                                                                                                                            class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                                            int32,
                                                                                                                            int32)
      IL_0009:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$SeqExpressionSteppingTest8
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  6
    .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8::directValues()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  pop
    IL_000d:  ldc.i4.1
    IL_000e:  ldc.i4.2
    IL_000f:  ldc.i4.3
    IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0024:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest8/SeqExpressionSteppingTest8::doubledWithForArrow(class [runtime]System.Collections.Generic.IEnumerable`1<int32>)
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0030:  pop
    IL_0031:  ret
  } 

} 





