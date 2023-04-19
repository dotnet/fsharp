




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly extern System.Linq
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         
  .ver 8:0:0:0
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





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname iAfterE@12
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
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/iAfterE@12::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/iAfterE@12::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string assembly/iAfterE@12::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 
            GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      
      .maxstack  6
      .locals init (string V_0,
               string V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/iAfterE@12::pc
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
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/iAfterE@12::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/iAfterE@12::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/iAfterE@12::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/iAfterE@12::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      string assembly/iAfterE@12::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/iAfterE@12::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/iAfterE@12::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/iAfterE@12::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/iAfterE@12::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/iAfterE@12::pc
      IL_008b:  ldarg.0
      IL_008c:  ldnull
      IL_008d:  stfld      string assembly/iAfterE@12::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } 

    .method public strict virtual instance void 
            Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/iAfterE@12::pc
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
        IL_0018:  ldfld      int32 assembly/iAfterE@12::pc
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
        IL_0044:  stfld      int32 assembly/iAfterE@12::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/iAfterE@12::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/iAfterE@12::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string assembly/iAfterE@12::current
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

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/iAfterE@12::pc
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

    .method public strict virtual instance string 
            get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/iAfterE@12::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<string> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void assembly/iAfterE@12::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                int32,
                                                                                string)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'iAfterE@13-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,bool>
  {
    .field static assembly initonly class assembly/'iAfterE@13-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(string w) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldstr      "ei"
      IL_0006:  callvirt   instance bool [runtime]System.String::Contains(string)
      IL_000b:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'iAfterE@13-1'::.ctor()
      IL_0005:  stsfld     class assembly/'iAfterE@13-1' assembly/'iAfterE@13-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 20@21'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 20@21'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 20@21'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Product,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 20@22-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 20@22-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 20@22-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 20@22-1' assembly/'Pipe #1 input at line 20@22-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 20@22-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 20@22-2' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 20@22-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 20@22-2' assembly/'Pipe #1 input at line 20@22-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 20@22-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 20@22-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #1 input at line 20@22-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #1 input at line 20@23-5'
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static bool  Invoke(class [Utils]Utils/Product x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 20@23-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,bool>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 20@23-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldnull
      IL_0002:  ldftn      bool assembly/'Pipe #1 input at line 20@23-5'::Invoke(class [Utils]Utils/Product)
      IL_0008:  newobj     instance void class [runtime]System.Func`2<class [Utils]Utils/Product,bool>::.ctor(object,
                                                                                                                     native int)
      IL_000d:  call       bool [System.Linq]System.Linq.Enumerable::Any<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                     class [runtime]System.Func`2<!!0,bool>)
      IL_0012:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 20@23-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 20@23-4' assembly/'Pipe #1 input at line 20@23-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 20@24-6'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 20@24-6' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  newobj     instance void class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                           !1)
      IL_000c:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 20@24-6'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 20@24-6' assembly/'Pipe #1 input at line 20@24-6'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname onlyOdd@32
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
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/onlyOdd@32::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/onlyOdd@32::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/onlyOdd@32::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 
            GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/onlyOdd@32::pc
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
      IL_0031:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/onlyOdd@32::'enum'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 assembly/onlyOdd@32::pc
      IL_003d:  br.s       IL_005d

      IL_003f:  ldarg.0
      IL_0040:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/onlyOdd@32::'enum'
      IL_0045:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stloc.0
      IL_004b:  ldloc.0
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldc.i4.2
      IL_004f:  stfld      int32 assembly/onlyOdd@32::pc
      IL_0054:  ldarg.0
      IL_0055:  ldloc.1
      IL_0056:  stfld      int32 assembly/onlyOdd@32::current
      IL_005b:  ldc.i4.1
      IL_005c:  ret

      IL_005d:  ldarg.0
      IL_005e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/onlyOdd@32::'enum'
      IL_0063:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0068:  brtrue.s   IL_003f

      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.3
      IL_006c:  stfld      int32 assembly/onlyOdd@32::pc
      IL_0071:  ldarg.0
      IL_0072:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/onlyOdd@32::'enum'
      IL_0077:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_007c:  nop
      IL_007d:  ldarg.0
      IL_007e:  ldnull
      IL_007f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/onlyOdd@32::'enum'
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/onlyOdd@32::pc
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  stfld      int32 assembly/onlyOdd@32::current
      IL_0092:  ldc.i4.0
      IL_0093:  ret
    } 

    .method public strict virtual instance void 
            Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/onlyOdd@32::pc
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
        IL_0018:  ldfld      int32 assembly/onlyOdd@32::pc
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
        IL_0044:  stfld      int32 assembly/onlyOdd@32::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/onlyOdd@32::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/onlyOdd@32::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 assembly/onlyOdd@32::current
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

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/onlyOdd@32::pc
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

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/onlyOdd@32::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void assembly/onlyOdd@32::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                int32,
                                                                                int32)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'onlyOdd@33-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class assembly/'onlyOdd@33-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.2
      IL_0002:  rem
      IL_0003:  ldc.i4.1
      IL_0004:  ceq
      IL_0006:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'onlyOdd@33-1'::.ctor()
      IL_0005:  stsfld     class assembly/'onlyOdd@33-1' assembly/'onlyOdd@33-1'::@_instance
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
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
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

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Product,object> 
            Invoke(class [Utils]Utils/Product _arg1) cil managed
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
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [Utils]Utils/Product>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [Utils]Utils/Product 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
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
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string 
            Invoke(class [Utils]Utils/Product p) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 38@40-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 38@40-2' assembly/'Pipe #2 input at line 38@40-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@40-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 38@40-3'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> _arg2) cil managed
    {
      
      .maxstack  6
      .locals init (class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #2 input at line 38@40-3'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #2 input at line 38@41-5'
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static bool  Invoke(class [Utils]Utils/Product x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance int32 [Utils]Utils/Product::get_UnitsInStock()
      IL_0006:  ldc.i4.0
      IL_0007:  cgt
      IL_0009:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@41-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,bool>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 38@41-4' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldnull
      IL_0002:  ldftn      bool assembly/'Pipe #2 input at line 38@41-5'::Invoke(class [Utils]Utils/Product)
      IL_0008:  newobj     instance void class [runtime]System.Func`2<class [Utils]Utils/Product,bool>::.ctor(object,
                                                                                                                     native int)
      IL_000d:  call       bool [System.Linq]System.Linq.Enumerable::All<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                     class [runtime]System.Func`2<!!0,bool>)
      IL_0012:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 38@41-4'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 38@41-4' assembly/'Pipe #2 input at line 38@41-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 38@42-6'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 38@42-6' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>> 
            Invoke(class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product> g) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  newobj     instance void class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>::.ctor(!0,
                                                                                                                                                                           !1)
      IL_000c:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 38@42-6'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 38@42-6' assembly/'Pipe #2 input at line 38@42-6'::@_instance
      IL_000a:  ret
    } 

  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> 
          get_words() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$assembly>'.$assembly::words@8
    IL_0005:  ret
  } 

  .method public specialname static bool 
          get_iAfterE() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     bool '<StartupCode$assembly>'.$assembly::iAfterE@10
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> 
          get_products() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$assembly>'.$assembly::products@17
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] 
          get_productGroups() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$assembly>'.$assembly::productGroups@19
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          get_numbers() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::numbers@28
    IL_0005:  ret
  } 

  .method public specialname static bool 
          get_onlyOdd() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     bool '<StartupCode$assembly>'.$assembly::onlyOdd@30
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] 
          get_productGroups2() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$assembly>'.$assembly::productGroups2@37
    IL_0005:  ret
  } 

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
  } 
  .property bool iAfterE()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get bool assembly::get_iAfterE()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
  } 
  .property class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[]
          productGroups()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] assembly::get_productGroups()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
  } 
  .property bool onlyOdd()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get bool assembly::get_onlyOdd()
  } 
  .property class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[]
          productGroups2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] assembly::get_productGroups2()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@8
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly bool iAfterE@10
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@17
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] productGroups@19
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@28
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly bool onlyOdd@30
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] productGroups2@37
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  10
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_0,
             bool V_1,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> V_2,
             class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] V_3,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_4,
             bool V_5,
             class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] V_6,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>> V_7,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_8,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>> V_9,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10)
    IL_0000:  ldstr      "believe"
    IL_0005:  ldstr      "relief"
    IL_000a:  ldstr      "receipt"
    IL_000f:  ldstr      "field"
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0023:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0028:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_002d:  dup
    IL_002e:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> '<StartupCode$assembly>'.$assembly::words@8
    IL_0033:  stloc.0
    IL_0034:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0039:  ldnull
    IL_003a:  ldc.i4.0
    IL_003b:  ldnull
    IL_003c:  newobj     instance void assembly/iAfterE@12::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                              int32,
                                                                              string)
    IL_0041:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0046:  ldsfld     class assembly/'iAfterE@13-1' assembly/'iAfterE@13-1'::@_instance
    IL_004b:  callvirt   instance bool [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Exists<string,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0050:  dup
    IL_0051:  stsfld     bool '<StartupCode$assembly>'.$assembly::iAfterE@10
    IL_0056:  stloc.1
    IL_0057:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_005c:  dup
    IL_005d:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> '<StartupCode$assembly>'.$assembly::products@17
    IL_0062:  stloc.2
    IL_0063:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0068:  stloc.s    V_8
    IL_006a:  ldloc.s    V_8
    IL_006c:  ldloc.s    V_8
    IL_006e:  ldloc.s    V_8
    IL_0070:  ldloc.s    V_8
    IL_0072:  ldloc.s    V_8
    IL_0074:  ldloc.s    V_8
    IL_0076:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_007b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0080:  ldloc.s    V_8
    IL_0082:  newobj     instance void assembly/'Pipe #1 input at line 20@21'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0087:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_008c:  ldsfld     class assembly/'Pipe #1 input at line 20@22-1' assembly/'Pipe #1 input at line 20@22-1'::@_instance
    IL_0091:  ldsfld     class assembly/'Pipe #1 input at line 20@22-2' assembly/'Pipe #1 input at line 20@22-2'::@_instance
    IL_0096:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_009b:  ldloc.s    V_8
    IL_009d:  newobj     instance void assembly/'Pipe #1 input at line 20@22-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_00a2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_00a7:  ldsfld     class assembly/'Pipe #1 input at line 20@23-4' assembly/'Pipe #1 input at line 20@23-4'::@_instance
    IL_00ac:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_00b1:  ldsfld     class assembly/'Pipe #1 input at line 20@24-6' assembly/'Pipe #1 input at line 20@24-6'::@_instance
    IL_00b6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_00bb:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_00c0:  stloc.s    V_7
    IL_00c2:  ldloc.s    V_7
    IL_00c4:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_00c9:  dup
    IL_00ca:  stsfld     class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$assembly>'.$assembly::productGroups@19
    IL_00cf:  stloc.3
    IL_00d0:  ldc.i4.1
    IL_00d1:  ldc.i4.s   11
    IL_00d3:  ldc.i4.3
    IL_00d4:  ldc.i4.s   19
    IL_00d6:  ldc.i4.s   41
    IL_00d8:  ldc.i4.s   65
    IL_00da:  ldc.i4.s   19
    IL_00dc:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_00e1:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e6:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00eb:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00f0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00f5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00fa:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ff:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0104:  dup
    IL_0105:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> '<StartupCode$assembly>'.$assembly::numbers@28
    IL_010a:  stloc.s    V_4
    IL_010c:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0111:  ldnull
    IL_0112:  ldc.i4.0
    IL_0113:  ldc.i4.0
    IL_0114:  newobj     instance void assembly/onlyOdd@32::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                              int32,
                                                                              int32)
    IL_0119:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,class [runtime]System.Collections.IEnumerable>::.ctor(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_011e:  ldsfld     class assembly/'onlyOdd@33-1' assembly/'onlyOdd@33-1'::@_instance
    IL_0123:  callvirt   instance bool [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::All<int32,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0128:  dup
    IL_0129:  stsfld     bool '<StartupCode$assembly>'.$assembly::onlyOdd@30
    IL_012e:  stloc.s    V_5
    IL_0130:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0135:  stloc.s    V_10
    IL_0137:  ldloc.s    V_10
    IL_0139:  ldloc.s    V_10
    IL_013b:  ldloc.s    V_10
    IL_013d:  ldloc.s    V_10
    IL_013f:  ldloc.s    V_10
    IL_0141:  ldloc.s    V_10
    IL_0143:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
    IL_0148:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Product>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_014d:  ldloc.s    V_10
    IL_014f:  newobj     instance void assembly/'Pipe #2 input at line 38@39'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0154:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Product,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0159:  ldsfld     class assembly/'Pipe #2 input at line 38@40-1' assembly/'Pipe #2 input at line 38@40-1'::@_instance
    IL_015e:  ldsfld     class assembly/'Pipe #2 input at line 38@40-2' assembly/'Pipe #2 input at line 38@40-2'::@_instance
    IL_0163:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [System.Linq]System.Linq.IGrouping`2<!!1,!!2>,!!3> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::GroupValBy<class [Utils]Utils/Product,string,class [Utils]Utils/Product,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!3>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>)
    IL_0168:  ldloc.s    V_10
    IL_016a:  newobj     instance void assembly/'Pipe #2 input at line 38@40-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_016f:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0174:  ldsfld     class assembly/'Pipe #2 input at line 38@41-4' assembly/'Pipe #2 input at line 38@41-4'::@_instance
    IL_0179:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_017e:  ldsfld     class assembly/'Pipe #2 input at line 38@42-6' assembly/'Pipe #2 input at line 38@42-6'::@_instance
    IL_0183:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0188:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_018d:  stloc.s    V_9
    IL_018f:  ldloc.s    V_9
    IL_0191:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0196:  dup
    IL_0197:  stsfld     class [runtime]System.Tuple`2<string,class [System.Linq]System.Linq.IGrouping`2<string,class [Utils]Utils/Product>>[] '<StartupCode$assembly>'.$assembly::productGroups2@37
    IL_019c:  stloc.s    V_6
    IL_019e:  ret
  } 

} 






