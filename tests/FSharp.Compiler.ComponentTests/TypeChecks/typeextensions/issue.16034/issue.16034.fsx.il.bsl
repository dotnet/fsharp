




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





.class public abstract auto ansi sealed Issue.'16034'
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public T
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
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

    .method public hidebysig specialname 
            instance int32  get_indexed1(object a1) cil managed
    {
      
      .maxstack  7
      IL_0000:  call       int32 Issue.'16034'::get_i()
      IL_0005:  ldc.i4.1
      IL_0006:  add
      IL_0007:  call       void Issue.'16034'::set_i(int32)
      IL_000c:  ldstr      "T().indexed1 %P() !\t%03i%P()"
      IL_0011:  ldc.i4.2
      IL_0012:  newarr     [runtime]System.Object
      IL_0017:  dup
      IL_0018:  ldc.i4.0
      IL_0019:  ldarg.1
      IL_001a:  box        [runtime]System.Object
      IL_001f:  stelem     [runtime]System.Object
      IL_0024:  dup
      IL_0025:  ldc.i4.1
      IL_0026:  call       int32 Issue.'16034'::get_i()
      IL_002b:  box        [runtime]System.Int32
      IL_0030:  stelem     [runtime]System.Object
      IL_0035:  ldnull
      IL_0036:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Tuple`2<object,int32>>::.ctor(string,
                                                                                                                                                                                                                                                                                                                                                 object[],
                                                                                                                                                                                                                                                                                                                                                 class [runtime]System.Type[])
      IL_003b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0040:  pop
      IL_0041:  ldc.i4.1
      IL_0042:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_indexed1(object a1,
                                        int32 'value') cil managed
    {
      
      .maxstack  7
      IL_0000:  call       int32 Issue.'16034'::get_i()
      IL_0005:  ldc.i4.1
      IL_0006:  add
      IL_0007:  call       void Issue.'16034'::set_i(int32)
      IL_000c:  ldstr      "T().indexed1 %P() <- %P() !\t%03i%P()"
      IL_0011:  ldc.i4.3
      IL_0012:  newarr     [runtime]System.Object
      IL_0017:  dup
      IL_0018:  ldc.i4.0
      IL_0019:  ldarg.1
      IL_001a:  box        [runtime]System.Object
      IL_001f:  stelem     [runtime]System.Object
      IL_0024:  dup
      IL_0025:  ldc.i4.1
      IL_0026:  ldarg.2
      IL_0027:  box        [runtime]System.Int32
      IL_002c:  stelem     [runtime]System.Object
      IL_0031:  dup
      IL_0032:  ldc.i4.2
      IL_0033:  call       int32 Issue.'16034'::get_i()
      IL_0038:  box        [runtime]System.Int32
      IL_003d:  stelem     [runtime]System.Object
      IL_0042:  ldnull
      IL_0043:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Tuple`3<object,int32,int32>>::.ctor(string,
                                                                                                                                                                                                                                                                                                                                                       object[],
                                                                                                                                                                                                                                                                                                                                                       class [runtime]System.Type[])
      IL_0048:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_004d:  pop
      IL_004e:  ret
    } 

    .property instance int32 indexed1(object)
    {
      .set instance void Issue.'16034'/T::set_indexed1(object,
                                                       int32)
      .get instance int32 Issue.'16034'/T::get_indexed1(object)
    } 
  } 

  .class abstract auto ansi sealed nested public Extensions
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public specialname static int32 
            get_j() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     int32 '<StartupCode$assembly>.$Issue'.'16034$fsx'::j@13
      IL_0005:  ret
    } 

    .method public specialname static void 
            set_j(int32 'value') cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>.$Issue'.'16034$fsx'::j@13
      IL_0006:  ret
    } 

    .method public static int32  T.get_indexed1(class Issue.'16034'/T x,
                                                object aa1) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  7
      IL_0000:  call       int32 Issue.'16034'::get_i()
      IL_0005:  ldc.i4.1
      IL_0006:  add
      IL_0007:  call       void Issue.'16034'::set_i(int32)
      IL_000c:  call       int32 Issue.'16034'/Extensions::get_j()
      IL_0011:  ldc.i4.1
      IL_0012:  add
      IL_0013:  call       void Issue.'16034'/Extensions::set_j(int32)
      IL_0018:  ldstr      "type extensions aa1 %P() !\t%03i%P()\t%03i%P()"
      IL_001d:  ldc.i4.3
      IL_001e:  newarr     [runtime]System.Object
      IL_0023:  dup
      IL_0024:  ldc.i4.0
      IL_0025:  ldarg.1
      IL_0026:  box        [runtime]System.Object
      IL_002b:  stelem     [runtime]System.Object
      IL_0030:  dup
      IL_0031:  ldc.i4.1
      IL_0032:  call       int32 Issue.'16034'::get_i()
      IL_0037:  box        [runtime]System.Int32
      IL_003c:  stelem     [runtime]System.Object
      IL_0041:  dup
      IL_0042:  ldc.i4.2
      IL_0043:  call       int32 Issue.'16034'/Extensions::get_j()
      IL_0048:  box        [runtime]System.Int32
      IL_004d:  stelem     [runtime]System.Object
      IL_0052:  ldnull
      IL_0053:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Tuple`3<object,int32,int32>>::.ctor(string,
                                                                                                                                                                                                                                                                                                                                                       object[],
                                                                                                                                                                                                                                                                                                                                                       class [runtime]System.Type[])
      IL_0058:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_005d:  pop
      IL_005e:  ldc.i4.1
      IL_005f:  ret
    } 

    .method public static void  T.set_indexed1(class Issue.'16034'/T x,
                                               object aa1,
                                               int32 'value') cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 02 00 00 00 00 00 ) 
      
      .maxstack  7
      IL_0000:  call       int32 Issue.'16034'::get_i()
      IL_0005:  ldc.i4.1
      IL_0006:  add
      IL_0007:  call       void Issue.'16034'::set_i(int32)
      IL_000c:  call       int32 Issue.'16034'/Extensions::get_j()
      IL_0011:  ldc.i4.1
      IL_0012:  add
      IL_0013:  call       void Issue.'16034'/Extensions::set_j(int32)
      IL_0018:  ldstr      "type extension aa1 %P() <- %P()!\t%03i%P()\t%03i%P()"
      IL_001d:  ldc.i4.4
      IL_001e:  newarr     [runtime]System.Object
      IL_0023:  dup
      IL_0024:  ldc.i4.0
      IL_0025:  ldarg.1
      IL_0026:  box        [runtime]System.Object
      IL_002b:  stelem     [runtime]System.Object
      IL_0030:  dup
      IL_0031:  ldc.i4.1
      IL_0032:  ldarg.2
      IL_0033:  box        [runtime]System.Int32
      IL_0038:  stelem     [runtime]System.Object
      IL_003d:  dup
      IL_003e:  ldc.i4.2
      IL_003f:  call       int32 Issue.'16034'::get_i()
      IL_0044:  box        [runtime]System.Int32
      IL_0049:  stelem     [runtime]System.Object
      IL_004e:  dup
      IL_004f:  ldc.i4.3
      IL_0050:  call       int32 Issue.'16034'/Extensions::get_j()
      IL_0055:  box        [runtime]System.Int32
      IL_005a:  stelem     [runtime]System.Object
      IL_005f:  ldnull
      IL_0060:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Tuple`4<object,int32,int32,int32>>::.ctor(string,
                                                                                                                                                                                                                                                                                                                                                             object[],
                                                                                                                                                                                                                                                                                                                                                             class [runtime]System.Type[])
      IL_0065:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_006a:  pop
      IL_006b:  ret
    } 

    .property int32 j()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .set void Issue.'16034'/Extensions::set_j(int32)
      .get int32 Issue.'16034'/Extensions::get_j()
    } 
  } 

  .method public specialname static int32 
          get_i() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$assembly>.$Issue'.'16034$fsx'::i@1
    IL_0005:  ret
  } 

  .method public specialname static void 
          set_i(int32 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>.$Issue'.'16034$fsx'::i@1
    IL_0006:  ret
  } 

  .method public specialname static class Issue.'16034'/T 
          get_t() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class Issue.'16034'/T '<StartupCode$assembly>.$Issue'.'16034$fsx'::t@25
    IL_0005:  ret
  } 

  .property int32 i()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void Issue.'16034'::set_i(int32)
    .get int32 Issue.'16034'::get_i()
  } 
  .property class Issue.'16034'/T t()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class Issue.'16034'/T Issue.'16034'::get_t()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>.$Issue'.'16034$fsx'
       extends [runtime]System.Object
{
  .field static assembly int32 i@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 j@13
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly initonly class Issue.'16034'/T t@25
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  5
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>.$Issue'.'16034$fsx'::i@1
    IL_0006:  ldc.i4.0
    IL_0007:  stsfld     int32 '<StartupCode$assembly>.$Issue'.'16034$fsx'::j@13
    IL_000c:  newobj     instance void Issue.'16034'/T::.ctor()
    IL_0011:  stsfld     class Issue.'16034'/T '<StartupCode$assembly>.$Issue'.'16034$fsx'::t@25
    IL_0016:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_001b:  ldstr      "ok"
    IL_0020:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0025:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_002a:  ldc.i4.1
    IL_002b:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_0030:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0035:  ldstr      "ok"
    IL_003a:  ldc.i4.2
    IL_003b:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_0040:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0045:  ldstr      "ok"
    IL_004a:  ldc.i4.3
    IL_004b:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_0050:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0055:  ldstr      "ok"
    IL_005a:  callvirt   instance int32 Issue.'16034'/T::get_indexed1(object)
    IL_005f:  pop
    IL_0060:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0065:  ldstr      "ok"
    IL_006a:  callvirt   instance int32 Issue.'16034'/T::get_indexed1(object)
    IL_006f:  pop
    IL_0070:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0075:  ldstr      "ok"
    IL_007a:  ldc.i4.1
    IL_007b:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_0080:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0085:  ldstr      "nok"
    IL_008a:  callvirt   instance int32 Issue.'16034'/T::get_indexed1(object)
    IL_008f:  pop
    IL_0090:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0095:  ldstr      "ok"
    IL_009a:  callvirt   instance int32 Issue.'16034'/T::get_indexed1(object)
    IL_009f:  pop
    IL_00a0:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_00a5:  ldstr      "ok"
    IL_00aa:  call       int32 Issue.'16034'/Extensions::T.get_indexed1(class Issue.'16034'/T,
                                                                        object)
    IL_00af:  pop
    IL_00b0:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_00b5:  ldstr      "nok"
    IL_00ba:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_00bf:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00c4:  ldc.i4.1
    IL_00c5:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_00ca:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_00cf:  ldstr      "nok"
    IL_00d4:  ldc.i4.2
    IL_00d5:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_00da:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_00df:  ldstr      "nok"
    IL_00e4:  ldc.i4.3
    IL_00e5:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_00ea:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_00ef:  ldstr      "nok"
    IL_00f4:  callvirt   instance int32 Issue.'16034'/T::get_indexed1(object)
    IL_00f9:  pop
    IL_00fa:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_00ff:  ldstr      "nok"
    IL_0104:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0109:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_010e:  callvirt   instance int32 Issue.'16034'/T::get_indexed1(object)
    IL_0113:  pop
    IL_0114:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0119:  ldstr      "nok"
    IL_011e:  callvirt   instance int32 Issue.'16034'/T::get_indexed1(object)
    IL_0123:  pop
    IL_0124:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0129:  ldstr      "nok_015"
    IL_012e:  ldc.i4.1
    IL_012f:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_0134:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0139:  ldstr      "nok_016"
    IL_013e:  ldc.i4.2
    IL_013f:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_0144:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0149:  ldstr      "ok_017"
    IL_014e:  ldc.i4.1
    IL_014f:  callvirt   instance void Issue.'16034'/T::set_indexed1(object,
                                                                     int32)
    IL_0154:  call       class Issue.'16034'/T Issue.'16034'::get_t()
    IL_0159:  ldstr      "ok_018"
    IL_015e:  ldc.i4.1
    IL_015f:  call       void Issue.'16034'/Extensions::T.set_indexed1(class Issue.'16034'/T,
                                                                       object,
                                                                       int32)
    IL_0164:  ret
  } 

} 





