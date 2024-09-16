




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         
  .ver 4:0:0:0
}
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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
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
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #1 input at line 11@12'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@12'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_factorsOf300()
      IL_002c:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@12'::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@12'::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@12'::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@12'::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@12'::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@12'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 assembly/'Pipe #1 input at line 11@12'::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #1 input at line 11@12'::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance int32 get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #1 input at line 11@12'::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void assembly/'Pipe #1 input at line 11@12'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                  int32,
                                                                                                  int32)
      IL_0008:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname numSum@21
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/numSum@21::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/numSum@21::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/numSum@21::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/numSum@21::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
      IL_002c:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/numSum@21::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/numSum@21::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/numSum@21::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/numSum@21::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      int32 assembly/numSum@21::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/numSum@21::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/numSum@21::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/numSum@21::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/numSum@21::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/numSum@21::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 assembly/numSum@21::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/numSum@21::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/numSum@21::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/numSum@21::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/numSum@21::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/numSum@21::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 assembly/numSum@21::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/numSum@21::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance int32 get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/numSum@21::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void assembly/numSum@21::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'numSum@22-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class assembly/'numSum@22-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'numSum@22-1'::.ctor()
      IL_0005:  stsfld     class assembly/'numSum@22-1' assembly/'numSum@22-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname totalChars@30
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/totalChars@30::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/totalChars@30::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string assembly/totalChars@30::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/totalChars@30::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
      IL_002c:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/totalChars@30::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/totalChars@30::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/totalChars@30::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/totalChars@30::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string assembly/totalChars@30::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/totalChars@30::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/totalChars@30::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/totalChars@30::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/totalChars@30::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/totalChars@30::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string assembly/totalChars@30::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/totalChars@30::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/totalChars@30::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/totalChars@30::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/totalChars@30::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/totalChars@30::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string assembly/totalChars@30::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/totalChars@30::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance string get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/totalChars@30::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<string> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void assembly/totalChars@30::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'totalChars@31-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .field static assembly initonly class assembly/'totalChars@31-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(string w) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [runtime]System.String::get_Length()
      IL_0006:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'totalChars@31-1'::.ctor()
      IL_0005:  stsfld     class assembly/'totalChars@31-1' assembly/'totalChars@31-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@39'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 38@39'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 38@39'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@40-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 38@40-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 38@40-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 38@40-1' assembly/'Pipe #2 input at line 38@40-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@40-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 38@40-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 38@40-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 38@40-2' assembly/'Pipe #2 input at line 38@40-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname sum@42
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/sum@42::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/sum@42::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/sum@42::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product assembly/sum@42::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/sum@42::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/sum@42::g
      IL_002d:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/sum@42::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 assembly/sum@42::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/sum@42::'enum'
      IL_0046:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 assembly/sum@42::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product assembly/sum@42::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/sum@42::'enum'
      IL_0064:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 assembly/sum@42::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/sum@42::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/sum@42::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 assembly/sum@42::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product assembly/sum@42::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/sum@42::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/sum@42::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/sum@42::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/sum@42::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/sum@42::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product assembly/sum@42::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/sum@42::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product assembly/sum@42::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/sum@42::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void assembly/sum@42::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'sum@43-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>
  {
    .field static assembly initonly class assembly/'sum@43-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(class [Utils]Utils/Product x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'sum@43-1'::.ctor()
      IL_0005:  stsfld     class assembly/'sum@43-1' assembly/'sum@43-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@40-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 38@40-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object> Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      
      .maxstack  8
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               int32 V_1,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable> V_4,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32> V_5,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_6,
               class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_7,
               int32 V_8,
               int32 V_9,
               class [runtime]System.IDisposable V_10)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  stloc.3
      IL_000a:  ldloc.0
      IL_000b:  ldnull
      IL_000c:  ldc.i4.0
      IL_000d:  ldnull
      IL_000e:  newobj     instance void assembly/sum@42::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0013:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0018:  stloc.s    V_4
      IL_001a:  ldsfld     class assembly/'sum@43-1' assembly/'sum@43-1'::@_instance
      IL_001f:  stloc.s    V_5
      IL_0021:  ldloc.s    V_4
      IL_0023:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::get_Source()
      IL_0028:  stloc.s    V_6
      IL_002a:  ldloc.s    V_6
      IL_002c:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0031:  stloc.s    V_7
      .try
      {
        IL_0033:  ldc.i4.0
        IL_0034:  stloc.s    V_9
        IL_0036:  br.s       IL_004b

        IL_0038:  ldloc.s    V_9
        IL_003a:  ldloc.s    V_5
        IL_003c:  ldloc.s    V_7
        IL_003e:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_0043:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,int32>::Invoke(!0)
        IL_0048:  add.ovf
        IL_0049:  stloc.s    V_9
        IL_004b:  ldloc.s    V_7
        IL_004d:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
        IL_0052:  brtrue.s   IL_0038

        IL_0054:  ldloc.s    V_9
        IL_0056:  stloc.s    V_8
        IL_0058:  leave.s    IL_0070

      }  
      finally
      {
        IL_005a:  ldloc.s    V_7
        IL_005c:  isinst     [runtime]System.IDisposable
        IL_0061:  stloc.s    V_10
        IL_0063:  ldloc.s    V_10
        IL_0065:  brfalse.s  IL_006f

        IL_0067:  ldloc.s    V_10
        IL_0069:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
        IL_006e:  endfinally
        IL_006f:  endfinally
      }  
      IL_0070:  ldloc.s    V_8
      IL_0072:  stloc.1
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 38@40-3'::builder@
      IL_0079:  ldloc.0
      IL_007a:  ldloc.1
      IL_007b:  newobj     instance void class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::.ctor(!0,
                                                                                                                                                                    !1)
      IL_0080:  tail.
      IL_0082:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(!!0)
      IL_0087:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@45-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [runtime]System.Tuple`2<string,int32>>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 38@45-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [runtime]System.Tuple`2<string,int32>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,int32> Invoke(class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               int32 V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [runtime]System.Tuple`2<string,int32>::.ctor(!0,
                                                                                             !1)
      IL_001a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 38@45-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 38@45-4' assembly/'Pipe #2 input at line 38@45-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname minNum@49
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/minNum@49::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/minNum@49::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/minNum@49::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/minNum@49::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
      IL_002c:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/minNum@49::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/minNum@49::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/minNum@49::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/minNum@49::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      int32 assembly/minNum@49::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/minNum@49::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/minNum@49::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/minNum@49::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/minNum@49::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/minNum@49::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 assembly/minNum@49::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/minNum@49::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/minNum@49::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/minNum@49::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/minNum@49::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/minNum@49::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 assembly/minNum@49::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/minNum@49::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance int32 get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/minNum@49::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void assembly/minNum@49::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'minNum@49-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class assembly/'minNum@49-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'minNum@49-1'::.ctor()
      IL_0005:  stsfld     class assembly/'minNum@49-1' assembly/'minNum@49-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname shortestWord@52
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/shortestWord@52::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/shortestWord@52::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string assembly/shortestWord@52::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/shortestWord@52::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
      IL_002c:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/shortestWord@52::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/shortestWord@52::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/shortestWord@52::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/shortestWord@52::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string assembly/shortestWord@52::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/shortestWord@52::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/shortestWord@52::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/shortestWord@52::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/shortestWord@52::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/shortestWord@52::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string assembly/shortestWord@52::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/shortestWord@52::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/shortestWord@52::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/shortestWord@52::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/shortestWord@52::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/shortestWord@52::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string assembly/shortestWord@52::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/shortestWord@52::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance string get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/shortestWord@52::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<string> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void assembly/shortestWord@52::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                    int32,
                                                                                    string)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'shortestWord@52-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .field static assembly initonly class assembly/'shortestWord@52-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(string w) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [runtime]System.String::get_Length()
      IL_0006:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'shortestWord@52-1'::.ctor()
      IL_0005:  stsfld     class assembly/'shortestWord@52-1' assembly/'shortestWord@52-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@57'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 56@57'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 56@57'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@58-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 56@58-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 56@58-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 56@58-1' assembly/'Pipe #3 input at line 56@58-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@58-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 56@58-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 56@58-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 56@58-2' assembly/'Pipe #3 input at line 56@58-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname min@59
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/min@59::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/min@59::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/min@59::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product assembly/min@59::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/min@59::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/min@59::g
      IL_002d:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/min@59::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 assembly/min@59::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/min@59::'enum'
      IL_0046:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 assembly/min@59::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product assembly/min@59::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/min@59::'enum'
      IL_0064:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 assembly/min@59::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/min@59::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/min@59::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 assembly/min@59::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product assembly/min@59::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/min@59::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/min@59::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/min@59::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/min@59::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/min@59::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product assembly/min@59::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/min@59::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product assembly/min@59::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/min@59::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void assembly/min@59::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'min@59-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>
  {
    .field static assembly initonly class assembly/'min@59-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance valuetype [runtime]System.Decimal Invoke(class [Utils]Utils/Product x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'min@59-1'::.ctor()
      IL_0005:  stsfld     class assembly/'min@59-1' assembly/'min@59-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@58-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 56@58-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object> Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      
      .maxstack  9
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  ldloc.0
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void assembly/min@59::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                           class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                           int32,
                                                                           class [Utils]Utils/Product)
      IL_0010:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0015:  ldsfld     class assembly/'min@59-1' assembly/'min@59-1'::@_instance
      IL_001a:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,valuetype [runtime]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_001f:  stloc.1
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #3 input at line 56@58-3'::builder@
      IL_0026:  ldloc.0
      IL_0027:  ldloc.1
      IL_0028:  newobj     instance void class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_002d:  tail.
      IL_002f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>(!!0)
      IL_0034:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 56@60-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 56@60-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal> Invoke(class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>::.ctor(!0,
                                                                                                                          !1)
      IL_001a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 56@60-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 56@60-4' assembly/'Pipe #3 input at line 56@60-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@66'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 65@66'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 65@66'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@67-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class assembly/'Pipe #4 input at line 65@67-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #4 input at line 65@67-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #4 input at line 65@67-1' assembly/'Pipe #4 input at line 65@67-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@67-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #4 input at line 65@67-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #4 input at line 65@67-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #4 input at line 65@67-2' assembly/'Pipe #4 input at line 65@67-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname 'min@68-2'
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static valuetype [runtime]System.Decimal Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname cheapestProducts@69
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/cheapestProducts@69::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/cheapestProducts@69::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/cheapestProducts@69::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product assembly/cheapestProducts@69::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/cheapestProducts@69::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/cheapestProducts@69::g
      IL_002d:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/cheapestProducts@69::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 assembly/cheapestProducts@69::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/cheapestProducts@69::'enum'
      IL_0046:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 assembly/cheapestProducts@69::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product assembly/cheapestProducts@69::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/cheapestProducts@69::'enum'
      IL_0064:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 assembly/cheapestProducts@69::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/cheapestProducts@69::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/cheapestProducts@69::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 assembly/cheapestProducts@69::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product assembly/cheapestProducts@69::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/cheapestProducts@69::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/cheapestProducts@69::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/cheapestProducts@69::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/cheapestProducts@69::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/cheapestProducts@69::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product assembly/cheapestProducts@69::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/cheapestProducts@69::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product assembly/cheapestProducts@69::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/cheapestProducts@69::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void assembly/cheapestProducts@69::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                        class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                        int32,
                                                                                        class [Utils]Utils/Product)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'cheapestProducts@69-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>
  {
    .field public valuetype [runtime]System.Decimal min
    .method assembly specialname rtspecialname instance void  .ctor(valuetype [runtime]System.Decimal min) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [runtime]System.Decimal assembly/'cheapestProducts@69-1'::min
      IL_000d:  ret
    } 

    .method public strict virtual instance bool Invoke(class [Utils]Utils/Product x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0006:  ldarg.0
      IL_0007:  ldfld      valuetype [runtime]System.Decimal assembly/'cheapestProducts@69-1'::min
      IL_000c:  call       bool [netstandard]System.Decimal::op_Equality(valuetype [netstandard]System.Decimal,
                                                                         valuetype [netstandard]System.Decimal)
      IL_0011:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@67-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 65@67-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object> Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      
      .maxstack  9
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  ldnull
      IL_0004:  ldftn      valuetype [runtime]System.Decimal assembly/'min@68-2'::Invoke(class [Utils]Utils/Product)
      IL_000a:  newobj     instance void class [runtime]System.Func`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>::.ctor(object,
                                                                                                                                             native int)
      IL_000f:  call       valuetype [runtime]System.Decimal [System.Core]System.Linq.Enumerable::Min<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                   class [runtime]System.Func`2<!!0,valuetype [runtime]System.Decimal>)
      IL_0014:  stloc.1
      IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_001a:  stloc.3
      IL_001b:  ldloc.3
      IL_001c:  ldloc.0
      IL_001d:  ldnull
      IL_001e:  ldc.i4.0
      IL_001f:  ldnull
      IL_0020:  newobj     instance void assembly/cheapestProducts@69::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                        class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                        int32,
                                                                                        class [Utils]Utils/Product)
      IL_0025:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_002a:  ldloc.1
      IL_002b:  newobj     instance void assembly/'cheapestProducts@69-1'::.ctor(valuetype [runtime]System.Decimal)
      IL_0030:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
      IL_0035:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::get_Source()
      IL_003a:  stloc.2
      IL_003b:  ldarg.0
      IL_003c:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #4 input at line 65@67-3'::builder@
      IL_0041:  ldloc.0
      IL_0042:  ldloc.1
      IL_0043:  ldloc.2
      IL_0044:  newobj     instance void class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                                                                                                                      !1,
                                                                                                                                                                                                                                                                                      !2)
      IL_0049:  tail.
      IL_004b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_0050:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 65@70-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class assembly/'Pipe #4 input at line 65@70-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> Invoke(class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_2)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item3()
      IL_0014:  stloc.2
      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_001b:  ldloc.2
      IL_001c:  newobj     instance void class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0021:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #4 input at line 65@70-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #4 input at line 65@70-4' assembly/'Pipe #4 input at line 65@70-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname maxNum@74
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
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
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 int32 current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/maxNum@74::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/maxNum@74::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/maxNum@74::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/maxNum@74::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
      IL_002c:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/maxNum@74::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/maxNum@74::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/maxNum@74::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/maxNum@74::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      int32 assembly/maxNum@74::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/maxNum@74::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/maxNum@74::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/maxNum@74::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/maxNum@74::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/maxNum@74::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 assembly/maxNum@74::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/maxNum@74::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/maxNum@74::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/maxNum@74::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/maxNum@74::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/maxNum@74::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 assembly/maxNum@74::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/maxNum@74::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance int32 get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/maxNum@74::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void assembly/maxNum@74::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'maxNum@74-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class assembly/'maxNum@74-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'maxNum@74-1'::.ctor()
      IL_0005:  stsfld     class assembly/'maxNum@74-1' assembly/'maxNum@74-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname longestLength@77
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/longestLength@77::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/longestLength@77::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string assembly/longestLength@77::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/longestLength@77::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
      IL_002c:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/longestLength@77::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/longestLength@77::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/longestLength@77::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/longestLength@77::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string assembly/longestLength@77::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/longestLength@77::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/longestLength@77::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/longestLength@77::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/longestLength@77::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/longestLength@77::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string assembly/longestLength@77::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/longestLength@77::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/longestLength@77::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/longestLength@77::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/longestLength@77::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/longestLength@77::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string assembly/longestLength@77::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/longestLength@77::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance string get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/longestLength@77::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<string> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void assembly/longestLength@77::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                     int32,
                                                                                     string)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'longestLength@77-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>
  {
    .field static assembly initonly class assembly/'longestLength@77-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(string w) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance int32 [runtime]System.String::get_Length()
      IL_0006:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'longestLength@77-1'::.ctor()
      IL_0005:  stsfld     class assembly/'longestLength@77-1' assembly/'longestLength@77-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@82'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #5 input at line 81@82'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #5 input at line 81@82'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@83-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class assembly/'Pipe #5 input at line 81@83-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #5 input at line 81@83-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #5 input at line 81@83-1' assembly/'Pipe #5 input at line 81@83-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@83-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #5 input at line 81@83-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #5 input at line 81@83-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #5 input at line 81@83-2' assembly/'Pipe #5 input at line 81@83-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname mostExpensivePrice@84
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/mostExpensivePrice@84::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensivePrice@84::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/mostExpensivePrice@84::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product assembly/mostExpensivePrice@84::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/mostExpensivePrice@84::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/mostExpensivePrice@84::g
      IL_002d:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensivePrice@84::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 assembly/mostExpensivePrice@84::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensivePrice@84::'enum'
      IL_0046:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 assembly/mostExpensivePrice@84::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product assembly/mostExpensivePrice@84::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensivePrice@84::'enum'
      IL_0064:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 assembly/mostExpensivePrice@84::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensivePrice@84::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensivePrice@84::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 assembly/mostExpensivePrice@84::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product assembly/mostExpensivePrice@84::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/mostExpensivePrice@84::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/mostExpensivePrice@84::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/mostExpensivePrice@84::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensivePrice@84::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/mostExpensivePrice@84::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product assembly/mostExpensivePrice@84::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/mostExpensivePrice@84::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product assembly/mostExpensivePrice@84::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/mostExpensivePrice@84::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void assembly/mostExpensivePrice@84::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                          class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                          int32,
                                                                                          class [Utils]Utils/Product)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'mostExpensivePrice@84-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>
  {
    .field static assembly initonly class assembly/'mostExpensivePrice@84-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance valuetype [runtime]System.Decimal Invoke(class [Utils]Utils/Product x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'mostExpensivePrice@84-1'::.ctor()
      IL_0005:  stsfld     class assembly/'mostExpensivePrice@84-1' assembly/'mostExpensivePrice@84-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@83-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #5 input at line 81@83-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object> Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      
      .maxstack  9
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  ldloc.0
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void assembly/mostExpensivePrice@84::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                          class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                          int32,
                                                                                          class [Utils]Utils/Product)
      IL_0010:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0015:  ldsfld     class assembly/'mostExpensivePrice@84-1' assembly/'mostExpensivePrice@84-1'::@_instance
      IL_001a:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,valuetype [runtime]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_001f:  stloc.1
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #5 input at line 81@83-3'::builder@
      IL_0026:  ldloc.0
      IL_0027:  ldloc.1
      IL_0028:  newobj     instance void class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_002d:  tail.
      IL_002f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>(!!0)
      IL_0034:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 81@85-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>
  {
    .field static assembly initonly class assembly/'Pipe #5 input at line 81@85-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal> Invoke(class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>::.ctor(!0,
                                                                                                                          !1)
      IL_001a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #5 input at line 81@85-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #5 input at line 81@85-4' assembly/'Pipe #5 input at line 81@85-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@91'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #6 input at line 90@91'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #6 input at line 90@91'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@92-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class assembly/'Pipe #6 input at line 90@92-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #6 input at line 90@92-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #6 input at line 90@92-1' assembly/'Pipe #6 input at line 90@92-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@92-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #6 input at line 90@92-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #6 input at line 90@92-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #6 input at line 90@92-2' assembly/'Pipe #6 input at line 90@92-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname maxPrice@93
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/maxPrice@93::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/maxPrice@93::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/maxPrice@93::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product assembly/maxPrice@93::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/maxPrice@93::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/maxPrice@93::g
      IL_002d:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/maxPrice@93::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 assembly/maxPrice@93::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/maxPrice@93::'enum'
      IL_0046:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 assembly/maxPrice@93::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product assembly/maxPrice@93::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/maxPrice@93::'enum'
      IL_0064:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 assembly/maxPrice@93::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/maxPrice@93::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/maxPrice@93::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 assembly/maxPrice@93::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product assembly/maxPrice@93::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/maxPrice@93::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/maxPrice@93::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/maxPrice@93::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/maxPrice@93::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/maxPrice@93::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product assembly/maxPrice@93::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/maxPrice@93::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product assembly/maxPrice@93::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/maxPrice@93::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void assembly/maxPrice@93::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                class [Utils]Utils/Product)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'maxPrice@93-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>
  {
    .field static assembly initonly class assembly/'maxPrice@93-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance valuetype [runtime]System.Decimal Invoke(class [Utils]Utils/Product x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'maxPrice@93-1'::.ctor()
      IL_0005:  stsfld     class assembly/'maxPrice@93-1' assembly/'maxPrice@93-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname mostExpensiveProducts@94
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/mostExpensiveProducts@94::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product assembly/mostExpensiveProducts@94::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/mostExpensiveProducts@94::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::g
      IL_002d:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 assembly/mostExpensiveProducts@94::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::'enum'
      IL_0046:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 assembly/mostExpensiveProducts@94::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product assembly/mostExpensiveProducts@94::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::'enum'
      IL_0064:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 assembly/mostExpensiveProducts@94::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 assembly/mostExpensiveProducts@94::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product assembly/mostExpensiveProducts@94::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/mostExpensiveProducts@94::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/mostExpensiveProducts@94::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/mostExpensiveProducts@94::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/mostExpensiveProducts@94::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product assembly/mostExpensiveProducts@94::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/mostExpensiveProducts@94::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product assembly/mostExpensiveProducts@94::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/mostExpensiveProducts@94::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void assembly/mostExpensiveProducts@94::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                             class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                             int32,
                                                                                             class [Utils]Utils/Product)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'mostExpensiveProducts@94-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>
  {
    .field public valuetype [runtime]System.Decimal maxPrice
    .method assembly specialname rtspecialname instance void  .ctor(valuetype [runtime]System.Decimal maxPrice) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,bool>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype [runtime]System.Decimal assembly/'mostExpensiveProducts@94-1'::maxPrice
      IL_000d:  ret
    } 

    .method public strict virtual instance bool Invoke(class [Utils]Utils/Product x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0006:  ldarg.0
      IL_0007:  ldfld      valuetype [runtime]System.Decimal assembly/'mostExpensiveProducts@94-1'::maxPrice
      IL_000c:  call       bool [netstandard]System.Decimal::op_Equality(valuetype [netstandard]System.Decimal,
                                                                         valuetype [netstandard]System.Decimal)
      IL_0011:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@92-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #6 input at line 90@92-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object> Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      
      .maxstack  9
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  ldloc.0
      IL_0008:  ldnull
      IL_0009:  ldc.i4.0
      IL_000a:  ldnull
      IL_000b:  newobj     instance void assembly/maxPrice@93::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                class [Utils]Utils/Product)
      IL_0010:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0015:  ldsfld     class assembly/'maxPrice@93-1' assembly/'maxPrice@93-1'::@_instance
      IL_001a:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,valuetype [runtime]System.Decimal>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
      IL_001f:  stloc.1
      IL_0020:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0025:  stloc.3
      IL_0026:  ldloc.3
      IL_0027:  ldloc.0
      IL_0028:  ldnull
      IL_0029:  ldc.i4.0
      IL_002a:  ldnull
      IL_002b:  newobj     instance void assembly/mostExpensiveProducts@94::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                             class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                             int32,
                                                                                             class [Utils]Utils/Product)
      IL_0030:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0035:  ldloc.1
      IL_0036:  newobj     instance void assembly/'mostExpensiveProducts@94-1'::.ctor(valuetype [runtime]System.Decimal)
      IL_003b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
      IL_0040:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::get_Source()
      IL_0045:  stloc.2
      IL_0046:  ldarg.0
      IL_0047:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #6 input at line 90@92-3'::builder@
      IL_004c:  ldloc.0
      IL_004d:  ldloc.1
      IL_004e:  ldloc.2
      IL_004f:  newobj     instance void class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                                                                                                                                      !1,
                                                                                                                                                                                                                                                                                      !2)
      IL_0054:  tail.
      IL_0056:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(!!0)
      IL_005b:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 90@95-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class assembly/'Pipe #6 input at line 90@95-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> Invoke(class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_2)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldarg.1
      IL_000f:  call       instance !2 class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::get_Item3()
      IL_0014:  stloc.2
      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_001b:  ldloc.2
      IL_001c:  newobj     instance void class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                            !1)
      IL_0021:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #6 input at line 90@95-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #6 input at line 90@95-4' assembly/'Pipe #6 input at line 90@95-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname averageNum@100
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<float64>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<float64> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public float64 current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<float64> 'enum',
                                 int32 pc,
                                 float64 current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<float64> assembly/averageNum@100::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/averageNum@100::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      float64 assembly/averageNum@100::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<float64>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<float64>& next) cil managed
    {
      
      .maxstack  6
      .locals init (float64 V_0,
               float64 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/averageNum@100::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006a

      IL_001e:  nop
      IL_001f:  br.s       IL_005d

      IL_0021:  nop
      IL_0022:  br.s       IL_008b

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> assembly::get_numbers2()
      IL_002c:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<float64> assembly/averageNum@100::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/averageNum@100::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<float64> assembly/averageNum@100::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/averageNum@100::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      float64 assembly/averageNum@100::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<float64> assembly/averageNum@100::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/averageNum@100::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<float64> assembly/averageNum@100::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<float64>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<float64> assembly/averageNum@100::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/averageNum@100::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.r8     0.0
      IL_0095:  stfld      float64 assembly/averageNum@100::current
      IL_009a:  ldc.i4.0
      IL_009b:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/averageNum@100::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_007e

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/averageNum@100::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/averageNum@100::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<float64> assembly/averageNum@100::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<float64>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/averageNum@100::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.r8     0.0
        IL_0067:  stfld      float64 assembly/averageNum@100::current
        IL_006c:  leave.s    IL_0078

      }  
      catch [runtime]System.Object 
      {
        IL_006e:  castclass  [runtime]System.Exception
        IL_0073:  stloc.1
        IL_0074:  ldloc.1
        IL_0075:  stloc.0
        IL_0076:  leave.s    IL_0078

      }  
      IL_0078:  nop
      IL_0079:  br         IL_0000

      IL_007e:  ldloc.0
      IL_007f:  brfalse.s  IL_0083

      IL_0081:  ldloc.0
      IL_0082:  throw

      IL_0083:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/averageNum@100::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance float64 get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      float64 assembly/averageNum@100::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<float64> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.r8     0.0
      IL_000b:  newobj     instance void assembly/averageNum@100::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                   int32,
                                                                                   float64)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'averageNum@100-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>
  {
    .field static assembly initonly class assembly/'averageNum@100-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance float64 Invoke(float64 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'averageNum@100-1'::.ctor()
      IL_0005:  stsfld     class assembly/'averageNum@100-1' assembly/'averageNum@100-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit averageLength@105
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,float64>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,float64>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/averageLength@105::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,float64>,object> Invoke(string _arg1) cil managed
    {
      
      .maxstack  7
      .locals init (string V_0,
               float64 V_1,
               int32 V_2)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  callvirt   instance int32 [runtime]System.String::get_Length()
      IL_0008:  stloc.2
      IL_0009:  ldloc.2
      IL_000a:  conv.r8
      IL_000b:  stloc.1
      IL_000c:  ldarg.0
      IL_000d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/averageLength@105::builder@
      IL_0012:  ldloc.0
      IL_0013:  ldloc.1
      IL_0014:  newobj     instance void class [runtime]System.Tuple`2<string,float64>::.ctor(!0,
                                                                                               !1)
      IL_0019:  tail.
      IL_001b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<string,float64>,object>(!!0)
      IL_0020:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'averageLength@107-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,float64>,float64>
  {
    .field static assembly initonly class assembly/'averageLength@107-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,float64>,float64>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance float64 Invoke(class [runtime]System.Tuple`2<string,float64> tupledArg) cil managed
    {
      
      .maxstack  5
      .locals init (string V_0,
               float64 V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<string,float64>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<string,float64>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'averageLength@107-1'::.ctor()
      IL_0005:  stsfld     class assembly/'averageLength@107-1' assembly/'averageLength@107-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@113'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 112@113'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 112@113'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@114-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class assembly/'Pipe #7 input at line 112@114-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #7 input at line 112@114-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #7 input at line 112@114-1' assembly/'Pipe #7 input at line 112@114-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@114-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #7 input at line 112@114-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #7 input at line 112@114-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #7 input at line 112@114-2' assembly/'Pipe #7 input at line 112@114-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname averagePrice@115
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Product current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g,
                                 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [Utils]Utils/Product current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/averagePrice@115::g
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/averagePrice@115::'enum'
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/averagePrice@115::pc
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    current
      IL_0018:  stfld      class [Utils]Utils/Product assembly/averagePrice@115::current
      IL_001d:  ldarg.0
      IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [Utils]Utils/Product>::.ctor()
      IL_0023:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>& next) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0,
               class [Utils]Utils/Product V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/averagePrice@115::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_006b

      IL_001e:  nop
      IL_001f:  br.s       IL_005e

      IL_0021:  nop
      IL_0022:  br.s       IL_008c

      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/averagePrice@115::g
      IL_002d:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0032:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/averagePrice@115::'enum'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 assembly/averagePrice@115::pc
      IL_003e:  br.s       IL_005e

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/averagePrice@115::'enum'
      IL_0046:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_004b:  stloc.0
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 assembly/averagePrice@115::pc
      IL_0055:  ldarg.0
      IL_0056:  ldloc.1
      IL_0057:  stfld      class [Utils]Utils/Product assembly/averagePrice@115::current
      IL_005c:  ldc.i4.1
      IL_005d:  ret

      IL_005e:  ldarg.0
      IL_005f:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/averagePrice@115::'enum'
      IL_0064:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0069:  brtrue.s   IL_0040

      IL_006b:  ldarg.0
      IL_006c:  ldc.i4.3
      IL_006d:  stfld      int32 assembly/averagePrice@115::pc
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/averagePrice@115::'enum'
      IL_0078:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_007d:  nop
      IL_007e:  ldarg.0
      IL_007f:  ldnull
      IL_0080:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/averagePrice@115::'enum'
      IL_0085:  ldarg.0
      IL_0086:  ldc.i4.3
      IL_0087:  stfld      int32 assembly/averagePrice@115::pc
      IL_008c:  ldarg.0
      IL_008d:  ldnull
      IL_008e:  stfld      class [Utils]Utils/Product assembly/averagePrice@115::current
      IL_0093:  ldc.i4.0
      IL_0094:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/averagePrice@115::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/averagePrice@115::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/averagePrice@115::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/averagePrice@115::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/averagePrice@115::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [Utils]Utils/Product assembly/averagePrice@115::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/averagePrice@115::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [Utils]Utils/Product assembly/averagePrice@115::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> assembly/averagePrice@115::g
      IL_0006:  ldnull
      IL_0007:  ldc.i4.0
      IL_0008:  ldnull
      IL_0009:  newobj     instance void assembly/averagePrice@115::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                     class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                     int32,
                                                                                     class [Utils]Utils/Product)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'averagePrice@115-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>
  {
    .field static assembly initonly class assembly/'averagePrice@115-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance valuetype [runtime]System.Decimal Invoke(class [Utils]Utils/Product x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'averagePrice@115-1'::.ctor()
      IL_0005:  stsfld     class assembly/'averagePrice@115-1' assembly/'averagePrice@115-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@114-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 112@114-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object> Invoke(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      
      .maxstack  9
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2,
               class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
               class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable> V_4,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal> V_5,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_6,
               string V_7,
               class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> V_8,
               string V_9,
               class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> V_10,
               valuetype [runtime]System.Decimal V_11,
               valuetype [runtime]System.Decimal V_12,
               int32 V_13,
               string V_14,
               valuetype [runtime]System.Decimal V_15,
               int32 V_16,
               class [runtime]System.IDisposable V_17)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
      IL_0007:  stloc.2
      IL_0008:  ldloc.2
      IL_0009:  stloc.3
      IL_000a:  ldloc.0
      IL_000b:  ldnull
      IL_000c:  ldc.i4.0
      IL_000d:  ldnull
      IL_000e:  newobj     instance void assembly/averagePrice@115::.ctor(class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,
                                                                                     class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                     int32,
                                                                                     class [Utils]Utils/Product)
      IL_0013:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0018:  stloc.s    V_4
      IL_001a:  ldsfld     class assembly/'averagePrice@115-1' assembly/'averagePrice@115-1'::@_instance
      IL_001f:  stloc.s    V_5
      IL_0021:  ldloc.s    V_4
      IL_0023:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>::get_Source()
      IL_0028:  stloc.s    V_6
      IL_002a:  ldstr      "source"
      IL_002f:  stloc.s    V_7
      IL_0031:  ldloc.s    V_6
      IL_0033:  stloc.s    V_8
      IL_0035:  ldloc.s    V_8
      IL_0037:  box        class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>
      IL_003c:  brtrue.s   IL_004a

      IL_003e:  ldloc.s    V_7
      IL_0040:  stloc.s    V_9
      IL_0042:  ldloc.s    V_9
      IL_0044:  newobj     instance void [netstandard]System.ArgumentNullException::.ctor(string)
      IL_0049:  throw

      IL_004a:  nop
      IL_004b:  ldloc.s    V_6
      IL_004d:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_0052:  stloc.s    V_10
      .try
      {
        IL_0054:  ldc.i4.0
        IL_0055:  ldc.i4.0
        IL_0056:  ldc.i4.0
        IL_0057:  ldc.i4.0
        IL_0058:  ldc.i4.0
        IL_0059:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                              int32,
                                                                              int32,
                                                                              bool,
                                                                              uint8)
        IL_005e:  stloc.s    V_12
        IL_0060:  ldc.i4.0
        IL_0061:  stloc.s    V_13
        IL_0063:  br.s       IL_0082

        IL_0065:  ldloc.s    V_12
        IL_0067:  ldloc.s    V_5
        IL_0069:  ldloc.s    V_10
        IL_006b:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
        IL_0070:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,valuetype [runtime]System.Decimal>::Invoke(!0)
        IL_0075:  call       valuetype [netstandard]System.Decimal [netstandard]System.Decimal::op_Addition(valuetype [netstandard]System.Decimal,
                                                                                                            valuetype [netstandard]System.Decimal)
        IL_007a:  stloc.s    V_12
        IL_007c:  ldloc.s    V_13
        IL_007e:  ldc.i4.1
        IL_007f:  add
        IL_0080:  stloc.s    V_13
        IL_0082:  ldloc.s    V_10
        IL_0084:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
        IL_0089:  brtrue.s   IL_0065

        IL_008b:  ldloc.s    V_13
        IL_008d:  brtrue.s   IL_009e

        IL_008f:  ldstr      "source"
        IL_0094:  stloc.s    V_14
        IL_0096:  ldloc.s    V_14
        IL_0098:  newobj     instance void [netstandard]System.InvalidOperationException::.ctor(string)
        IL_009d:  throw

        IL_009e:  nop
        IL_009f:  ldloc.s    V_12
        IL_00a1:  stloc.s    V_15
        IL_00a3:  ldloc.s    V_13
        IL_00a5:  stloc.s    V_16
        IL_00a7:  ldloc.s    V_15
        IL_00a9:  ldloc.s    V_16
        IL_00ab:  call       valuetype [netstandard]System.Decimal [netstandard]System.Convert::ToDecimal(int32)
        IL_00b0:  call       valuetype [netstandard]System.Decimal [netstandard]System.Decimal::Divide(valuetype [netstandard]System.Decimal,
                                                                                                       valuetype [netstandard]System.Decimal)
        IL_00b5:  stloc.s    V_11
        IL_00b7:  leave.s    IL_00cf

      }  
      finally
      {
        IL_00b9:  ldloc.s    V_10
        IL_00bb:  isinst     [runtime]System.IDisposable
        IL_00c0:  stloc.s    V_17
        IL_00c2:  ldloc.s    V_17
        IL_00c4:  brfalse.s  IL_00ce

        IL_00c6:  ldloc.s    V_17
        IL_00c8:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
        IL_00cd:  endfinally
        IL_00ce:  endfinally
      }  
      IL_00cf:  ldloc.s    V_11
      IL_00d1:  stloc.1
      IL_00d2:  ldarg.0
      IL_00d3:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 112@114-3'::builder@
      IL_00d8:  ldloc.0
      IL_00d9:  ldloc.1
      IL_00da:  newobj     instance void class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                 !1)
      IL_00df:  tail.
      IL_00e1:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>(!!0)
      IL_00e6:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 112@116-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>
  {
    .field static assembly initonly class assembly/'Pipe #7 input at line 112@116-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal> Invoke(class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0,
               valuetype [runtime]System.Decimal V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0014:  ldloc.1
      IL_0015:  newobj     instance void class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>::.ctor(!0,
                                                                                                                          !1)
      IL_001a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #7 input at line 112@116-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #7 input at line 112@116-4' assembly/'Pipe #7 input at line 112@116-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> factorsOf300@8
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 uniqueFactors@10
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@17
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 numSum@19
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@26
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 totalChars@28
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@35
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,int32>[] categories@37
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 minNum@49
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 shortestWord@52
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] categories2@55
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] categories3@64
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 maxNum@74
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 longestLength@77
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] categories4@80
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] categories5@89
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> numbers2@99
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly float64 averageNum@100
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly float64 averageLength@103
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] categories6@111
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_factorsOf300() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::factorsOf300@8
    IL_0005:  ret
  } 

  .method public specialname static int32 get_uniqueFactors() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::uniqueFactors@10
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_numbers() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbers@17
    IL_0005:  ret
  } 

  .method public specialname static int32 get_numSum() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::numSum@19
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> get_words() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::words@26
    IL_0005:  ret
  } 

  .method public specialname static int32 get_totalChars() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::totalChars@28
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> get_products() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::products@35
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,int32>[] get_categories() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,int32>[] assembly::categories@37
    IL_0005:  ret
  } 

  .method public specialname static int32 get_minNum() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::minNum@49
    IL_0005:  ret
  } 

  .method public specialname static int32 get_shortestWord() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::shortestWord@52
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] get_categories2() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] assembly::categories2@55
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] get_categories3() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] assembly::categories3@64
    IL_0005:  ret
  } 

  .method public specialname static int32 get_maxNum() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::maxNum@74
    IL_0005:  ret
  } 

  .method public specialname static int32 get_longestLength() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::longestLength@77
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] get_categories4() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] assembly::categories4@80
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] get_categories5() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] assembly::categories5@89
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> get_numbers2() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> assembly::numbers2@99
    IL_0005:  ret
  } 

  .method public specialname static float64 get_averageNum() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     float64 assembly::averageNum@100
    IL_0005:  ret
  } 

  .method public specialname static float64 get_averageLength() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     float64 assembly::averageLength@103
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] get_categories6() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] assembly::categories6@111
    IL_0005:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  13
    .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_1,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_3,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [runtime]System.Collections.IEnumerable> V_4,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> V_5,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_6,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_7,
             int32 V_8,
             int32 V_9,
             class [runtime]System.IDisposable V_10,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_11,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [runtime]System.Collections.IEnumerable> V_13,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32> V_14,
             class [runtime]System.Collections.Generic.IEnumerable`1<string> V_15,
             class [runtime]System.Collections.Generic.IEnumerator`1<string> V_16,
             int32 V_17,
             int32 V_18,
             class [runtime]System.IDisposable V_19,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,int32>> V_20,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_21,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>> V_22,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_23,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>> V_24,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_25,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>> V_26,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_27,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>> V_28,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_29,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_30,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_31,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [runtime]System.Collections.IEnumerable> V_32,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64> V_33,
             class [runtime]System.Collections.Generic.IEnumerable`1<float64> V_34,
             string V_35,
             class [runtime]System.Collections.Generic.IEnumerable`1<float64> V_36,
             string V_37,
             class [runtime]System.Collections.Generic.IEnumerator`1<float64> V_38,
             float64 V_39,
             float64 V_40,
             int32 V_41,
             string V_42,
             float64 V_43,
             int32 V_44,
             class [runtime]System.IDisposable V_45,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_46,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_47,
             class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,float64>,class [runtime]System.Collections.IEnumerable> V_48,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,float64>,float64> V_49,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,float64>> V_50,
             string V_51,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,float64>> V_52,
             string V_53,
             class [runtime]System.Collections.Generic.IEnumerator`1<class [runtime]System.Tuple`2<string,float64>> V_54,
             float64 V_55,
             float64 V_56,
             int32 V_57,
             string V_58,
             float64 V_59,
             int32 V_60,
             class [runtime]System.IDisposable V_61,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>> V_62,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_63)
    IL_0000:  ldc.i4.2
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  ldc.i4.5
    IL_0004:  ldc.i4.5
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0023:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::factorsOf300@8
    IL_0028:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_002d:  stloc.1
    IL_002e:  ldloc.1
    IL_002f:  ldnull
    IL_0030:  ldc.i4.0
    IL_0031:  ldc.i4.0
    IL_0032:  newobj     instance void assembly/'Pipe #1 input at line 11@12'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                int32,
                                                                                                int32)
    IL_0037:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_003c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Distinct<int32,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>)
    IL_0041:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_0046:  stloc.0
    IL_0047:  ldloc.0
    IL_0048:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_004d:  stsfld     int32 assembly::uniqueFactors@10
    IL_0052:  ldc.i4.5
    IL_0053:  ldc.i4.4
    IL_0054:  ldc.i4.1
    IL_0055:  ldc.i4.3
    IL_0056:  ldc.i4.s   9
    IL_0058:  ldc.i4.8
    IL_0059:  ldc.i4.6
    IL_005a:  ldc.i4.7
    IL_005b:  ldc.i4.2
    IL_005c:  ldc.i4.0
    IL_005d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0062:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0067:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_006c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0071:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0076:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_007b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0080:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0085:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_008a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_008f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0094:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbers@17
    IL_0099:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_009e:  stloc.2
    IL_009f:  ldloc.2
    IL_00a0:  stloc.3
    IL_00a1:  ldnull
    IL_00a2:  ldc.i4.0
    IL_00a3:  ldc.i4.0
    IL_00a4:  newobj     instance void assembly/numSum@21::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_00a9:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_00ae:  stloc.s    V_4
    IL_00b0:  ldsfld     class assembly/'numSum@22-1' assembly/'numSum@22-1'::@_instance
    IL_00b5:  stloc.s    V_5
    IL_00b7:  ldloc.s    V_4
    IL_00b9:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_00be:  stloc.s    V_6
    IL_00c0:  ldloc.s    V_6
    IL_00c2:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_00c7:  stloc.s    V_7
    .try
    {
      IL_00c9:  ldc.i4.0
      IL_00ca:  stloc.s    V_9
      IL_00cc:  br.s       IL_00e1

      IL_00ce:  ldloc.s    V_9
      IL_00d0:  ldloc.s    V_5
      IL_00d2:  ldloc.s    V_7
      IL_00d4:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_00d9:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
      IL_00de:  add.ovf
      IL_00df:  stloc.s    V_9
      IL_00e1:  ldloc.s    V_7
      IL_00e3:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_00e8:  brtrue.s   IL_00ce

      IL_00ea:  ldloc.s    V_9
      IL_00ec:  stloc.s    V_8
      IL_00ee:  leave.s    IL_0106

    }  
    finally
    {
      IL_00f0:  ldloc.s    V_7
      IL_00f2:  isinst     [runtime]System.IDisposable
      IL_00f7:  stloc.s    V_10
      IL_00f9:  ldloc.s    V_10
      IL_00fb:  brfalse.s  IL_0105

      IL_00fd:  ldloc.s    V_10
      IL_00ff:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_0104:  endfinally
      IL_0105:  endfinally
    }  
    IL_0106:  ldloc.s    V_8
    IL_0108:  stsfld     int32 assembly::numSum@19
    IL_010d:  ldstr      "cherry"
    IL_0112:  ldstr      "apple"
    IL_0117:  ldstr      "blueberry"
    IL_011c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0121:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0126:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_012b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0130:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::words@26
    IL_0135:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_013a:  stloc.s    V_11
    IL_013c:  ldloc.s    V_11
    IL_013e:  stloc.s    V_12
    IL_0140:  ldnull
    IL_0141:  ldc.i4.0
    IL_0142:  ldnull
    IL_0143:  newobj     instance void assembly/totalChars@30::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
    IL_0148:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_014d:  stloc.s    V_13
    IL_014f:  ldsfld     class assembly/'totalChars@31-1' assembly/'totalChars@31-1'::@_instance
    IL_0154:  stloc.s    V_14
    IL_0156:  ldloc.s    V_13
    IL_0158:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_015d:  stloc.s    V_15
    IL_015f:  ldloc.s    V_15
    IL_0161:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_0166:  stloc.s    V_16
    .try
    {
      IL_0168:  ldc.i4.0
      IL_0169:  stloc.s    V_18
      IL_016b:  br.s       IL_0180

      IL_016d:  ldloc.s    V_18
      IL_016f:  ldloc.s    V_14
      IL_0171:  ldloc.s    V_16
      IL_0173:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0178:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,int32>::Invoke(!0)
      IL_017d:  add.ovf
      IL_017e:  stloc.s    V_18
      IL_0180:  ldloc.s    V_16
      IL_0182:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_0187:  brtrue.s   IL_016d

      IL_0189:  ldloc.s    V_18
      IL_018b:  stloc.s    V_17
      IL_018d:  leave.s    IL_01a5

    }  
    finally
    {
      IL_018f:  ldloc.s    V_16
      IL_0191:  isinst     [runtime]System.IDisposable
      IL_0196:  stloc.s    V_19
      IL_0198:  ldloc.s    V_19
      IL_019a:  brfalse.s  IL_01a4

      IL_019c:  ldloc.s    V_19
      IL_019e:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_01a3:  endfinally
      IL_01a4:  endfinally
    }  
    IL_01a5:  ldloc.s    V_17
    IL_01a7:  stsfld     int32 assembly::totalChars@28
    IL_01ac:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_01b1:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::products@35
    IL_01b6:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_01bb:  stloc.s    V_21
    IL_01bd:  ldloc.s    V_21
    IL_01bf:  ldloc.s    V_21
    IL_01c1:  ldloc.s    V_21
    IL_01c3:  ldloc.s    V_21
    IL_01c5:  ldloc.s    V_21
    IL_01c7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_01cc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01d1:  ldloc.s    V_21
    IL_01d3:  newobj     instance void assembly/'Pipe #2 input at line 38@39'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01d8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01dd:  ldsfld     class assembly/'Pipe #2 input at line 38@40-1' assembly/'Pipe #2 input at line 38@40-1'::@_instance
    IL_01e2:  ldsfld     class assembly/'Pipe #2 input at line 38@40-2' assembly/'Pipe #2 input at line 38@40-2'::@_instance
    IL_01e7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_01ec:  ldloc.s    V_21
    IL_01ee:  newobj     instance void assembly/'Pipe #2 input at line 38@40-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01f3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01f8:  ldsfld     class assembly/'Pipe #2 input at line 38@45-4' assembly/'Pipe #2 input at line 38@45-4'::@_instance
    IL_01fd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,int32>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0202:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,int32>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_0207:  stloc.s    V_20
    IL_0209:  ldloc.s    V_20
    IL_020b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,int32>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0210:  stsfld     class [runtime]System.Tuple`2<string,int32>[] assembly::categories@37
    IL_0215:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_021a:  ldnull
    IL_021b:  ldc.i4.0
    IL_021c:  ldc.i4.0
    IL_021d:  newobj     instance void assembly/minNum@49::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0222:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0227:  ldsfld     class assembly/'minNum@49-1' assembly/'minNum@49-1'::@_instance
    IL_022c:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<int32,class [runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0231:  stsfld     int32 assembly::minNum@49
    IL_0236:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_023b:  ldnull
    IL_023c:  ldc.i4.0
    IL_023d:  ldnull
    IL_023e:  newobj     instance void assembly/shortestWord@52::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                  int32,
                                                                                  string)
    IL_0243:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0248:  ldsfld     class assembly/'shortestWord@52-1' assembly/'shortestWord@52-1'::@_instance
    IL_024d:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MinBy<string,class [runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0252:  stsfld     int32 assembly::shortestWord@52
    IL_0257:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_025c:  stloc.s    V_23
    IL_025e:  ldloc.s    V_23
    IL_0260:  ldloc.s    V_23
    IL_0262:  ldloc.s    V_23
    IL_0264:  ldloc.s    V_23
    IL_0266:  ldloc.s    V_23
    IL_0268:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_026d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0272:  ldloc.s    V_23
    IL_0274:  newobj     instance void assembly/'Pipe #3 input at line 56@57'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0279:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_027e:  ldsfld     class assembly/'Pipe #3 input at line 56@58-1' assembly/'Pipe #3 input at line 56@58-1'::@_instance
    IL_0283:  ldsfld     class assembly/'Pipe #3 input at line 56@58-2' assembly/'Pipe #3 input at line 56@58-2'::@_instance
    IL_0288:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_028d:  ldloc.s    V_23
    IL_028f:  newobj     instance void assembly/'Pipe #3 input at line 56@58-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0294:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0299:  ldsfld     class assembly/'Pipe #3 input at line 56@60-4' assembly/'Pipe #3 input at line 56@60-4'::@_instance
    IL_029e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_02a3:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_02a8:  stloc.s    V_22
    IL_02aa:  ldloc.s    V_22
    IL_02ac:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02b1:  stsfld     class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] assembly::categories2@55
    IL_02b6:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_02bb:  stloc.s    V_25
    IL_02bd:  ldloc.s    V_25
    IL_02bf:  ldloc.s    V_25
    IL_02c1:  ldloc.s    V_25
    IL_02c3:  ldloc.s    V_25
    IL_02c5:  ldloc.s    V_25
    IL_02c7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_02cc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02d1:  ldloc.s    V_25
    IL_02d3:  newobj     instance void assembly/'Pipe #4 input at line 65@66'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02d8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02dd:  ldsfld     class assembly/'Pipe #4 input at line 65@67-1' assembly/'Pipe #4 input at line 65@67-1'::@_instance
    IL_02e2:  ldsfld     class assembly/'Pipe #4 input at line 65@67-2' assembly/'Pipe #4 input at line 65@67-2'::@_instance
    IL_02e7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_02ec:  ldloc.s    V_25
    IL_02ee:  newobj     instance void assembly/'Pipe #4 input at line 65@67-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02f3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02f8:  ldsfld     class assembly/'Pipe #4 input at line 65@70-4' assembly/'Pipe #4 input at line 65@70-4'::@_instance
    IL_02fd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0302:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_0307:  stloc.s    V_24
    IL_0309:  ldloc.s    V_24
    IL_030b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0310:  stsfld     class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] assembly::categories3@64
    IL_0315:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_031a:  ldnull
    IL_031b:  ldc.i4.0
    IL_031c:  ldc.i4.0
    IL_031d:  newobj     instance void assembly/maxNum@74::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                            int32,
                                                                            int32)
    IL_0322:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0327:  ldsfld     class assembly/'maxNum@74-1' assembly/'maxNum@74-1'::@_instance
    IL_032c:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<int32,class [runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0331:  stsfld     int32 assembly::maxNum@74
    IL_0336:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_033b:  ldnull
    IL_033c:  ldc.i4.0
    IL_033d:  ldnull
    IL_033e:  newobj     instance void assembly/longestLength@77::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                   int32,
                                                                                   string)
    IL_0343:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0348:  ldsfld     class assembly/'longestLength@77-1' assembly/'longestLength@77-1'::@_instance
    IL_034d:  callvirt   instance !!2 [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::MaxBy<string,class [runtime]System.Collections.IEnumerable,int32>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0352:  stsfld     int32 assembly::longestLength@77
    IL_0357:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_035c:  stloc.s    V_27
    IL_035e:  ldloc.s    V_27
    IL_0360:  ldloc.s    V_27
    IL_0362:  ldloc.s    V_27
    IL_0364:  ldloc.s    V_27
    IL_0366:  ldloc.s    V_27
    IL_0368:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_036d:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0372:  ldloc.s    V_27
    IL_0374:  newobj     instance void assembly/'Pipe #5 input at line 81@82'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0379:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_037e:  ldsfld     class assembly/'Pipe #5 input at line 81@83-1' assembly/'Pipe #5 input at line 81@83-1'::@_instance
    IL_0383:  ldsfld     class assembly/'Pipe #5 input at line 81@83-2' assembly/'Pipe #5 input at line 81@83-2'::@_instance
    IL_0388:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_038d:  ldloc.s    V_27
    IL_038f:  newobj     instance void assembly/'Pipe #5 input at line 81@83-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0394:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0399:  ldsfld     class assembly/'Pipe #5 input at line 81@85-4' assembly/'Pipe #5 input at line 81@85-4'::@_instance
    IL_039e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03a3:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_03a8:  stloc.s    V_26
    IL_03aa:  ldloc.s    V_26
    IL_03ac:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03b1:  stsfld     class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] assembly::categories4@80
    IL_03b6:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03bb:  stloc.s    V_29
    IL_03bd:  ldloc.s    V_29
    IL_03bf:  ldloc.s    V_29
    IL_03c1:  ldloc.s    V_29
    IL_03c3:  ldloc.s    V_29
    IL_03c5:  ldloc.s    V_29
    IL_03c7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_03cc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03d1:  ldloc.s    V_29
    IL_03d3:  newobj     instance void assembly/'Pipe #6 input at line 90@91'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03d8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03dd:  ldsfld     class assembly/'Pipe #6 input at line 90@92-1' assembly/'Pipe #6 input at line 90@92-1'::@_instance
    IL_03e2:  ldsfld     class assembly/'Pipe #6 input at line 90@92-2' assembly/'Pipe #6 input at line 90@92-2'::@_instance
    IL_03e7:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_03ec:  ldloc.s    V_29
    IL_03ee:  newobj     instance void assembly/'Pipe #6 input at line 90@92-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03f3:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03f8:  ldsfld     class assembly/'Pipe #6 input at line 90@95-4' assembly/'Pipe #6 input at line 90@95-4'::@_instance
    IL_03fd:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`3<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0402:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_0407:  stloc.s    V_28
    IL_0409:  ldloc.s    V_28
    IL_040b:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0410:  stsfld     class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] assembly::categories5@89
    IL_0415:  ldc.r8     5.0999999999999996
    IL_041e:  ldc.r8     4.0999999999999996
    IL_0427:  ldc.r8     1.1000000000000001
    IL_0430:  ldc.r8     3.1000000000000001
    IL_0439:  ldc.r8     9.0999999999999996
    IL_0442:  ldc.r8     8.0999999999999996
    IL_044b:  ldc.r8     6.0999999999999996
    IL_0454:  ldc.r8     7.0999999999999996
    IL_045d:  ldc.r8     2.1000000000000001
    IL_0466:  ldc.r8     0.10000000000000001
    IL_046f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_Empty()
    IL_0474:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0479:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_047e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0483:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0488:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_048d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0492:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0497:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_049c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04a1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_04a6:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> assembly::numbers2@99
    IL_04ab:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_04b0:  stloc.s    V_30
    IL_04b2:  ldloc.s    V_30
    IL_04b4:  stloc.s    V_31
    IL_04b6:  ldnull
    IL_04b7:  ldc.i4.0
    IL_04b8:  ldc.r8     0.0
    IL_04c1:  newobj     instance void assembly/averageNum@100::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<float64>,
                                                                                 int32,
                                                                                 float64)
    IL_04c6:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_04cb:  stloc.s    V_32
    IL_04cd:  ldsfld     class assembly/'averageNum@100-1' assembly/'averageNum@100-1'::@_instance
    IL_04d2:  stloc.s    V_33
    IL_04d4:  ldloc.s    V_32
    IL_04d6:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<float64,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_04db:  stloc.s    V_34
    IL_04dd:  ldstr      "source"
    IL_04e2:  stloc.s    V_35
    IL_04e4:  ldloc.s    V_34
    IL_04e6:  stloc.s    V_36
    IL_04e8:  ldloc.s    V_36
    IL_04ea:  box        class [runtime]System.Collections.Generic.IEnumerable`1<float64>
    IL_04ef:  brtrue.s   IL_04fd

    IL_04f1:  ldloc.s    V_35
    IL_04f3:  stloc.s    V_37
    IL_04f5:  ldloc.s    V_37
    IL_04f7:  newobj     instance void [netstandard]System.ArgumentNullException::.ctor(string)
    IL_04fc:  throw

    IL_04fd:  nop
    IL_04fe:  ldloc.s    V_34
    IL_0500:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<float64>::GetEnumerator()
    IL_0505:  stloc.s    V_38
    .try
    {
      IL_0507:  ldc.r8     0.0
      IL_0510:  stloc.s    V_40
      IL_0512:  ldc.i4.0
      IL_0513:  stloc.s    V_41
      IL_0515:  br.s       IL_0530

      IL_0517:  ldloc.s    V_40
      IL_0519:  ldloc.s    V_33
      IL_051b:  ldloc.s    V_38
      IL_051d:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<float64>::get_Current()
      IL_0522:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>::Invoke(!0)
      IL_0527:  add
      IL_0528:  stloc.s    V_40
      IL_052a:  ldloc.s    V_41
      IL_052c:  ldc.i4.1
      IL_052d:  add
      IL_052e:  stloc.s    V_41
      IL_0530:  ldloc.s    V_38
      IL_0532:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_0537:  brtrue.s   IL_0517

      IL_0539:  ldloc.s    V_41
      IL_053b:  brtrue.s   IL_054c

      IL_053d:  ldstr      "source"
      IL_0542:  stloc.s    V_42
      IL_0544:  ldloc.s    V_42
      IL_0546:  newobj     instance void [netstandard]System.InvalidOperationException::.ctor(string)
      IL_054b:  throw

      IL_054c:  nop
      IL_054d:  ldloc.s    V_40
      IL_054f:  stloc.s    V_43
      IL_0551:  ldloc.s    V_41
      IL_0553:  stloc.s    V_44
      IL_0555:  ldloc.s    V_43
      IL_0557:  ldloc.s    V_44
      IL_0559:  conv.r8
      IL_055a:  div
      IL_055b:  stloc.s    V_39
      IL_055d:  leave.s    IL_0575

    }  
    finally
    {
      IL_055f:  ldloc.s    V_38
      IL_0561:  isinst     [runtime]System.IDisposable
      IL_0566:  stloc.s    V_45
      IL_0568:  ldloc.s    V_45
      IL_056a:  brfalse.s  IL_0574

      IL_056c:  ldloc.s    V_45
      IL_056e:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_0573:  endfinally
      IL_0574:  endfinally
    }  
    IL_0575:  ldloc.s    V_39
    IL_0577:  stsfld     float64 assembly::averageNum@100
    IL_057c:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0581:  stloc.s    V_46
    IL_0583:  ldloc.s    V_46
    IL_0585:  stloc.s    V_47
    IL_0587:  ldloc.s    V_46
    IL_0589:  ldloc.s    V_46
    IL_058b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
    IL_0590:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<string>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0595:  ldloc.s    V_46
    IL_0597:  newobj     instance void assembly/averageLength@105::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_059c:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<string,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,float64>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_05a1:  stloc.s    V_48
    IL_05a3:  ldsfld     class assembly/'averageLength@107-1' assembly/'averageLength@107-1'::@_instance
    IL_05a8:  stloc.s    V_49
    IL_05aa:  ldloc.s    V_48
    IL_05ac:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,float64>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_05b1:  stloc.s    V_50
    IL_05b3:  ldstr      "source"
    IL_05b8:  stloc.s    V_51
    IL_05ba:  ldloc.s    V_50
    IL_05bc:  stloc.s    V_52
    IL_05be:  ldloc.s    V_52
    IL_05c0:  box        class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,float64>>
    IL_05c5:  brtrue.s   IL_05d3

    IL_05c7:  ldloc.s    V_51
    IL_05c9:  stloc.s    V_53
    IL_05cb:  ldloc.s    V_53
    IL_05cd:  newobj     instance void [netstandard]System.ArgumentNullException::.ctor(string)
    IL_05d2:  throw

    IL_05d3:  nop
    IL_05d4:  ldloc.s    V_50
    IL_05d6:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,float64>>::GetEnumerator()
    IL_05db:  stloc.s    V_54
    .try
    {
      IL_05dd:  ldc.r8     0.0
      IL_05e6:  stloc.s    V_56
      IL_05e8:  ldc.i4.0
      IL_05e9:  stloc.s    V_57
      IL_05eb:  br.s       IL_0606

      IL_05ed:  ldloc.s    V_56
      IL_05ef:  ldloc.s    V_49
      IL_05f1:  ldloc.s    V_54
      IL_05f3:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<class [runtime]System.Tuple`2<string,float64>>::get_Current()
      IL_05f8:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<string,float64>,float64>::Invoke(!0)
      IL_05fd:  add
      IL_05fe:  stloc.s    V_56
      IL_0600:  ldloc.s    V_57
      IL_0602:  ldc.i4.1
      IL_0603:  add
      IL_0604:  stloc.s    V_57
      IL_0606:  ldloc.s    V_54
      IL_0608:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_060d:  brtrue.s   IL_05ed

      IL_060f:  ldloc.s    V_57
      IL_0611:  brtrue.s   IL_0622

      IL_0613:  ldstr      "source"
      IL_0618:  stloc.s    V_58
      IL_061a:  ldloc.s    V_58
      IL_061c:  newobj     instance void [netstandard]System.InvalidOperationException::.ctor(string)
      IL_0621:  throw

      IL_0622:  nop
      IL_0623:  ldloc.s    V_56
      IL_0625:  stloc.s    V_59
      IL_0627:  ldloc.s    V_57
      IL_0629:  stloc.s    V_60
      IL_062b:  ldloc.s    V_59
      IL_062d:  ldloc.s    V_60
      IL_062f:  conv.r8
      IL_0630:  div
      IL_0631:  stloc.s    V_55
      IL_0633:  leave.s    IL_064b

    }  
    finally
    {
      IL_0635:  ldloc.s    V_54
      IL_0637:  isinst     [runtime]System.IDisposable
      IL_063c:  stloc.s    V_61
      IL_063e:  ldloc.s    V_61
      IL_0640:  brfalse.s  IL_064a

      IL_0642:  ldloc.s    V_61
      IL_0644:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_0649:  endfinally
      IL_064a:  endfinally
    }  
    IL_064b:  ldloc.s    V_55
    IL_064d:  stsfld     float64 assembly::averageLength@103
    IL_0652:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0657:  stloc.s    V_63
    IL_0659:  ldloc.s    V_63
    IL_065b:  ldloc.s    V_63
    IL_065d:  ldloc.s    V_63
    IL_065f:  ldloc.s    V_63
    IL_0661:  ldloc.s    V_63
    IL_0663:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_0668:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_066d:  ldloc.s    V_63
    IL_066f:  newobj     instance void assembly/'Pipe #7 input at line 112@113'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0674:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0679:  ldsfld     class assembly/'Pipe #7 input at line 112@114-1' assembly/'Pipe #7 input at line 112@114-1'::@_instance
    IL_067e:  ldsfld     class assembly/'Pipe #7 input at line 112@114-2' assembly/'Pipe #7 input at line 112@114-2'::@_instance
    IL_0683:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0688:  ldloc.s    V_63
    IL_068a:  newobj     instance void assembly/'Pipe #7 input at line 112@114-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_068f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0694:  ldsfld     class assembly/'Pipe #7 input at line 112@116-4' assembly/'Pipe #7 input at line 112@116-4'::@_instance
    IL_0699:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [System.Core]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,valuetype [runtime]System.Decimal>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_069e:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_06a3:  stloc.s    V_62
    IL_06a5:  ldloc.s    V_62
    IL_06a7:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_06ac:  stsfld     class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] assembly::categories6@111
    IL_06b1:  ret
  } 

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          factorsOf300()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_factorsOf300()
  } 
  .property int32 uniqueFactors()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_uniqueFactors()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
  } 
  .property int32 numSum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_numSum()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
  } 
  .property int32 totalChars()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_totalChars()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
  } 
  .property class [runtime]System.Tuple`2<string,int32>[]
          categories()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,int32>[] assembly::get_categories()
  } 
  .property int32 minNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_minNum()
  } 
  .property int32 shortestWord()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_shortestWord()
  } 
  .property class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[]
          categories2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] assembly::get_categories2()
  } 
  .property class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[]
          categories3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] assembly::get_categories3()
  } 
  .property int32 maxNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_maxNum()
  } 
  .property int32 longestLength()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32 assembly::get_longestLength()
  } 
  .property class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[]
          categories4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] assembly::get_categories4()
  } 
  .property class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[]
          categories5()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>[] assembly::get_categories5()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>
          numbers2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> assembly::get_numbers2()
  } 
  .property float64 averageNum()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get float64 assembly::get_averageNum()
  } 
  .property float64 averageLength()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get float64 assembly::get_averageLength()
  } 
  .property class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[]
          categories6()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,valuetype [runtime]System.Decimal>[] assembly::get_categories6()
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
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






