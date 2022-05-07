
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
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:1:0:0
}
.assembly ForEachOnString01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForEachOnString01
{
  // Offset: 0x00000000 Length: 0x0000039E
  // WARNING: managed resource file FSharpSignatureData.ForEachOnString01 created
}
.mresource public FSharpOptimizationData.ForEachOnString01
{
  // Offset: 0x000003A8 Length: 0x000000FF
  // WARNING: managed resource file FSharpOptimizationData.ForEachOnString01 created
}
.module ForEachOnString01.exe
// MVID: {624FB28C-9852-A882-A745-03838CB24F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000001FA29AF0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForEachOnString01
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit test8@54
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>
  {
    .field static assembly initonly class ForEachOnString01/test8@54 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>::.ctor()
      IL_0006:  ret
    } // end of method test8@54::.ctor

    .method public strict virtual instance char 
            Invoke(char x) cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  conv.i4
      IL_0002:  ldc.i4.1
      IL_0003:  add
      IL_0004:  conv.u2
      IL_0005:  ret
    } // end of method test8@54::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void ForEachOnString01/test8@54::.ctor()
      IL_0005:  stsfld     class ForEachOnString01/test8@54 ForEachOnString01/test8@54::@_instance
      IL_000a:  ret
    } // end of method test8@54::.cctor

  } // end of class test8@54

  .class auto ansi serializable sealed nested assembly beforefieldinit test9@63
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>
  {
    .field static assembly initonly class ForEachOnString01/test9@63 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>::.ctor()
      IL_0006:  ret
    } // end of method test9@63::.ctor

    .method public strict virtual instance char 
            Invoke(char x) cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  conv.i4
      IL_0002:  ldc.i4.1
      IL_0003:  add
      IL_0004:  conv.u2
      IL_0005:  ret
    } // end of method test9@63::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void ForEachOnString01/test9@63::.ctor()
      IL_0005:  stsfld     class ForEachOnString01/test9@63 ForEachOnString01/test9@63::@_instance
      IL_000a:  ret
    } // end of method test9@63::.cctor

  } // end of class test9@63

  .method public static void  test1(string str) cil managed
  {
    // Code size       41 (0x29)
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
    IL_0005:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
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
  } // end of method ForEachOnString01::test1

  .method public static void  test2() cil managed
  {
    // Code size       49 (0x31)
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
    IL_0009:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
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
  } // end of method ForEachOnString01::test2

  .method public static void  test3() cil managed
  {
    // Code size       50 (0x32)
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
    IL_000a:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
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
  } // end of method ForEachOnString01::test3

  .method public static void  test4() cil managed
  {
    // Code size       50 (0x32)
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
    IL_000a:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
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
  } // end of method ForEachOnString01::test4

  .method public static void  test5() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             char V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.1
    IL_0003:  ldstr      "123"
    IL_0008:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
    IL_000d:  ldc.i4.1
    IL_000e:  sub
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldloc.1
    IL_0012:  blt.s      IL_0040

    IL_0014:  ldstr      "123"
    IL_0019:  ldloc.1
    IL_001a:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_001f:  stloc.2
    IL_0020:  ldstr      "%A"
    IL_0025:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,char>::.ctor(string)
    IL_002a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002f:  ldloc.2
    IL_0030:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0035:  pop
    IL_0036:  ldloc.1
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  stloc.1
    IL_003a:  ldloc.1
    IL_003b:  ldloc.0
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  bne.un.s   IL_0014

    IL_0040:  ret
  } // end of method ForEachOnString01::test5

  .method public static void  test6(string str) cil managed
  {
    // Code size       41 (0x29)
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
    IL_0005:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
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
  } // end of method ForEachOnString01::test6

  .method public static void  test7() cil managed
  {
    // Code size       50 (0x32)
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
    IL_000a:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
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
  } // end of method ForEachOnString01::test7

  .method public static void  test8() cil managed
  {
    // Code size       72 (0x48)
    .maxstack  5
    .locals init (string V_0,
             int32 V_1,
             int32 V_2,
             char V_3)
    IL_0000:  ldsfld     class ForEachOnString01/test8@54 ForEachOnString01/test8@54::@_instance
    IL_0005:  ldstr      "1234"
    IL_000a:  call       string [FSharp.Core]Microsoft.FSharp.Core.StringModule::Map(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>,
                                                                                     string)
    IL_000f:  stloc.0
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldloc.2
    IL_001d:  blt.s      IL_0047

    IL_001f:  ldloc.0
    IL_0020:  ldloc.2
    IL_0021:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0026:  stloc.3
    IL_0027:  ldstr      "%O"
    IL_002c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,char>::.ctor(string)
    IL_0031:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0036:  ldloc.3
    IL_0037:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_003c:  pop
    IL_003d:  ldloc.2
    IL_003e:  ldc.i4.1
    IL_003f:  add
    IL_0040:  stloc.2
    IL_0041:  ldloc.2
    IL_0042:  ldloc.1
    IL_0043:  ldc.i4.1
    IL_0044:  add
    IL_0045:  bne.un.s   IL_001f

    IL_0047:  ret
  } // end of method ForEachOnString01::test8

  .method public static void  test9() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  5
    .locals init (string V_0,
             int32 V_1,
             int32 V_2,
             char V_3,
             string V_4)
    IL_0000:  ldsfld     class ForEachOnString01/test9@63 ForEachOnString01/test9@63::@_instance
    IL_0005:  ldstr      "1234"
    IL_000a:  call       string [FSharp.Core]Microsoft.FSharp.Core.StringModule::Map(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>,
                                                                                     string)
    IL_000f:  stloc.0
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance int32 [System.Runtime]System.String::get_Length()
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldloc.2
    IL_001d:  blt.s      IL_005a

    IL_001f:  ldloc.0
    IL_0020:  ldloc.2
    IL_0021:  callvirt   instance char [netstandard]System.String::get_Chars(int32)
    IL_0026:  stloc.3
    IL_0027:  ldstr      "{0} foo"
    IL_002c:  ldloc.3
    IL_002d:  box        [System.Runtime]System.Char
    IL_0032:  call       string [System.Runtime]System.String::Format(string,
                                                                      object)
    IL_0037:  stloc.s    V_4
    IL_0039:  ldstr      "%O"
    IL_003e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string>::.ctor(string)
    IL_0043:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [System.Runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0048:  ldloc.s    V_4
    IL_004a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_004f:  pop
    IL_0050:  ldloc.2
    IL_0051:  ldc.i4.1
    IL_0052:  add
    IL_0053:  stloc.2
    IL_0054:  ldloc.2
    IL_0055:  ldloc.1
    IL_0056:  ldc.i4.1
    IL_0057:  add
    IL_0058:  bne.un.s   IL_001f

    IL_005a:  ret
  } // end of method ForEachOnString01::test9

} // end of class ForEachOnString01

.class private abstract auto ansi sealed '<StartupCode$ForEachOnString01>'.$ForEachOnString01
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $ForEachOnString01::main@

} // end of class '<StartupCode$ForEachOnString01>'.$ForEachOnString01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\ForLoop\ForEachOnString01_fs\ForEachOnString01.res
