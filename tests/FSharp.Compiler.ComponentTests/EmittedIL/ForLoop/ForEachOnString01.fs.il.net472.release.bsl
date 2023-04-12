




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
  .class auto ansi serializable sealed nested assembly beforefieldinit test8@54
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>
  {
    .field static assembly initonly class assembly/test8@54 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance char 
            Invoke(char x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  conv.i4
      IL_0002:  ldc.i4.1
      IL_0003:  add
      IL_0004:  conv.u2
      IL_0005:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/test8@54::.ctor()
      IL_0005:  stsfld     class assembly/test8@54 assembly/test8@54::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit test9@63
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>
  {
    .field static assembly initonly class assembly/test9@63 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance char 
            Invoke(char x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  conv.i4
      IL_0002:  ldc.i4.1
      IL_0003:  add
      IL_0004:  conv.u2
      IL_0005:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/test9@63::.ctor()
      IL_0005:  stsfld     class assembly/test9@63 assembly/test9@63::@_instance
      IL_000a:  ret
    } 

  } 

  .method public static void  test1(string str) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             char V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldloc.2
    IL_000f:  blt.s      IL_0028

    IL_0011:  ldarg.0
    IL_0012:  ldloc.2
    IL_0013:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0018:  stloc.3
    IL_0019:  ldloc.0
    IL_001a:  ldloc.3
    IL_001b:  conv.i4
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.2
    IL_0022:  ldloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  bne.un.s   IL_0011

    IL_0028:  ret
  } 

  .method public static void  test2() cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             char V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldstr      "123"
    IL_0009:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000e:  ldc.i4.1
    IL_000f:  sub
    IL_0010:  stloc.1
    IL_0011:  ldloc.1
    IL_0012:  ldloc.2
    IL_0013:  blt.s      IL_0030

    IL_0015:  ldstr      "123"
    IL_001a:  ldloc.2
    IL_001b:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0020:  stloc.3
    IL_0021:  ldloc.0
    IL_0022:  ldloc.3
    IL_0023:  conv.i4
    IL_0024:  add
    IL_0025:  stloc.0
    IL_0026:  ldloc.2
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.2
    IL_002a:  ldloc.2
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.1
    IL_002d:  add
    IL_002e:  bne.un.s   IL_0015

    IL_0030:  ret
  } 

  .method public static void  test3() cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             char V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  ldstr      "123"
    IL_000a:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.2
    IL_0014:  blt.s      IL_0031

    IL_0016:  ldstr      "123"
    IL_001b:  ldloc.2
    IL_001c:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0021:  stloc.3
    IL_0022:  ldloc.0
    IL_0023:  ldloc.3
    IL_0024:  conv.i4
    IL_0025:  add
    IL_0026:  stloc.0
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.2
    IL_002b:  ldloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  bne.un.s   IL_0016

    IL_0031:  ret
  } 

  .method public static void  test4() cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             char V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  nop
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  ldstr      "123"
    IL_000a:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.2
    IL_0014:  blt.s      IL_0031

    IL_0016:  ldstr      "123"
    IL_001b:  ldloc.2
    IL_001c:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0021:  stloc.3
    IL_0022:  ldloc.0
    IL_0023:  ldloc.3
    IL_0024:  conv.i4
    IL_0025:  add
    IL_0026:  stloc.0
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.2
    IL_002b:  ldloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  bne.un.s   IL_0016

    IL_0031:  ret
  } 

  .method public static void  test5() cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             char V_2,
             class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.1
    IL_0003:  ldstr      "123"
    IL_0008:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000d:  ldc.i4.1
    IL_000e:  sub
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldloc.1
    IL_0012:  blt.s      IL_0047

    IL_0014:  ldstr      "123"
    IL_0019:  ldloc.1
    IL_001a:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_001f:  stloc.2
    IL_0020:  ldstr      "%A"
    IL_0025:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,char>::.ctor(string)
    IL_002a:  stloc.3
    IL_002b:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0030:  ldloc.3
    IL_0031:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [runtime]System.IO.TextWriter,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0036:  ldloc.2
    IL_0037:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_003c:  pop
    IL_003d:  ldloc.1
    IL_003e:  ldc.i4.1
    IL_003f:  add
    IL_0040:  stloc.1
    IL_0041:  ldloc.1
    IL_0042:  ldloc.0
    IL_0043:  ldc.i4.1
    IL_0044:  add
    IL_0045:  bne.un.s   IL_0014

    IL_0047:  ret
  } 

  .method public static void  test6(string str) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             char V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldloc.2
    IL_000f:  blt.s      IL_0028

    IL_0011:  ldarg.0
    IL_0012:  ldloc.2
    IL_0013:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0018:  stloc.3
    IL_0019:  ldloc.0
    IL_001a:  ldloc.3
    IL_001b:  conv.i4
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.2
    IL_0022:  ldloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  bne.un.s   IL_0011

    IL_0028:  ret
  } 

  .method public static void  test7() cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             char V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  ldstr      "123"
    IL_000a:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.2
    IL_0014:  blt.s      IL_0031

    IL_0016:  ldstr      "123"
    IL_001b:  ldloc.2
    IL_001c:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0021:  stloc.3
    IL_0022:  ldloc.0
    IL_0023:  ldloc.3
    IL_0024:  conv.i4
    IL_0025:  add
    IL_0026:  stloc.0
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.2
    IL_002b:  ldloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  bne.un.s   IL_0016

    IL_0031:  ret
  } 

  .method public static void  test8() cil managed
  {
    
    .maxstack  5
    .locals init (string V_0,
             int32 V_1,
             int32 V_2,
             char V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_4)
    IL_0000:  ldsfld     class assembly/test8@54 assembly/test8@54::@_instance
    IL_0005:  ldstr      "1234"
    IL_000a:  call       string [FSharp.Core]Microsoft.FSharp.Core.StringModule::Map(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>,
                                                                                     string)
    IL_000f:  stloc.0
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldloc.2
    IL_001d:  blt.s      IL_0050

    IL_001f:  ldloc.0
    IL_0020:  ldloc.2
    IL_0021:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0026:  stloc.3
    IL_0027:  ldstr      "%O"
    IL_002c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,char>::.ctor(string)
    IL_0031:  stloc.s    V_4
    IL_0033:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0038:  ldloc.s    V_4
    IL_003a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [runtime]System.IO.TextWriter,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_003f:  ldloc.3
    IL_0040:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0045:  pop
    IL_0046:  ldloc.2
    IL_0047:  ldc.i4.1
    IL_0048:  add
    IL_0049:  stloc.2
    IL_004a:  ldloc.2
    IL_004b:  ldloc.1
    IL_004c:  ldc.i4.1
    IL_004d:  add
    IL_004e:  bne.un.s   IL_001f

    IL_0050:  ret
  } 

  .method public static void  test9() cil managed
  {
    
    .maxstack  5
    .locals init (string V_0,
             int32 V_1,
             int32 V_2,
             char V_3,
             string V_4,
             class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_5)
    IL_0000:  ldsfld     class assembly/test9@63 assembly/test9@63::@_instance
    IL_0005:  ldstr      "1234"
    IL_000a:  call       string [FSharp.Core]Microsoft.FSharp.Core.StringModule::Map(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>,
                                                                                     string)
    IL_000f:  stloc.0
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldloc.2
    IL_001d:  blt.s      IL_0063

    IL_001f:  ldloc.0
    IL_0020:  ldloc.2
    IL_0021:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0026:  stloc.3
    IL_0027:  ldstr      "{0} foo"
    IL_002c:  ldloc.3
    IL_002d:  box        [runtime]System.Char
    IL_0032:  call       string [runtime]System.String::Format(string,
                                                                object)
    IL_0037:  stloc.s    V_4
    IL_0039:  ldstr      "%O"
    IL_003e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string>::.ctor(string)
    IL_0043:  stloc.s    V_5
    IL_0045:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_004a:  ldloc.s    V_5
    IL_004c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [runtime]System.IO.TextWriter,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0051:  ldloc.s    V_4
    IL_0053:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0058:  pop
    IL_0059:  ldloc.2
    IL_005a:  ldc.i4.1
    IL_005b:  add
    IL_005c:  stloc.2
    IL_005d:  ldloc.2
    IL_005e:  ldloc.1
    IL_005f:  ldc.i4.1
    IL_0060:  add
    IL_0061:  bne.un.s   IL_001f

    IL_0063:  ret
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






