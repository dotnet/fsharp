







.class public abstract auto ansi sealed Test.Test
       extends [mscorlib]System.Object
{
  .method public static int32  main(string[] argv) cil managed
  {
    .entrypoint
    
    .maxstack  6
    .locals init (string V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_1)
    IL_0000:  ldc.i4.s   13
    IL_0002:  newarr     [mscorlib]System.String
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.5
    IL_000a:  call       string Test.Test::ss(int32)
    IL_000f:  stelem     [mscorlib]System.String
    IL_0014:  dup
    IL_0015:  ldc.i4.1
    IL_0016:  ldc.i4.6
    IL_0017:  call       string Test.Test::ss(int32)
    IL_001c:  stelem     [mscorlib]System.String
    IL_0021:  dup
    IL_0022:  ldc.i4.2
    IL_0023:  ldc.i4.7
    IL_0024:  call       string Test.Test::ss(int32)
    IL_0029:  stelem     [mscorlib]System.String
    IL_002e:  dup
    IL_002f:  ldc.i4.3
    IL_0030:  ldc.i4.8
    IL_0031:  call       string Test.Test::ss(int32)
    IL_0036:  stelem     [mscorlib]System.String
    IL_003b:  dup
    IL_003c:  ldc.i4.4
    IL_003d:  ldc.i4.s   9
    IL_003f:  call       string Test.Test::ss(int32)
    IL_0044:  stelem     [mscorlib]System.String
    IL_0049:  dup
    IL_004a:  ldc.i4.5
    IL_004b:  ldc.i4.s   10
    IL_004d:  call       string Test.Test::ss(int32)
    IL_0052:  stelem     [mscorlib]System.String
    IL_0057:  dup
    IL_0058:  ldc.i4.6
    IL_0059:  ldstr      "_50__60_"
    IL_005e:  stelem     [mscorlib]System.String
    IL_0063:  dup
    IL_0064:  ldc.i4.7
    IL_0065:  ldc.i4.s   100
    IL_0067:  call       string Test.Test::ss(int32)
    IL_006c:  stelem     [mscorlib]System.String
    IL_0071:  dup
    IL_0072:  ldc.i4.8
    IL_0073:  ldc.i4.s   101
    IL_0075:  call       string Test.Test::ss(int32)
    IL_007a:  stelem     [mscorlib]System.String
    IL_007f:  dup
    IL_0080:  ldc.i4.s   9
    IL_0082:  ldc.i4.s   102
    IL_0084:  call       string Test.Test::ss(int32)
    IL_0089:  stelem     [mscorlib]System.String
    IL_008e:  dup
    IL_008f:  ldc.i4.s   10
    IL_0091:  ldc.i4.s   103
    IL_0093:  call       string Test.Test::ss(int32)
    IL_0098:  stelem     [mscorlib]System.String
    IL_009d:  dup
    IL_009e:  ldc.i4.s   11
    IL_00a0:  ldstr      "_104__105_"
    IL_00a5:  stelem     [mscorlib]System.String
    IL_00aa:  dup
    IL_00ab:  ldc.i4.s   12
    IL_00ad:  ldc.i4.s   106
    IL_00af:  call       string Test.Test::ss(int32)
    IL_00b4:  stelem     [mscorlib]System.String
    IL_00b9:  call       string [mscorlib]System.String::Concat(string[])
    IL_00be:  stloc.0
    IL_00bf:  ldstr      "%A"
    IL_00c4:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string>::.ctor(string)
    IL_00c9:  stloc.1
    IL_00ca:  call       class [mscorlib]System.IO.TextWriter [mscorlib]System.Console::get_Out()
    IL_00cf:  ldloc.1
    IL_00d0:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_00d5:  ldloc.0
    IL_00d6:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_00db:  pop
    IL_00dc:  ldc.i4.0
    IL_00dd:  ret
  } 

} 



