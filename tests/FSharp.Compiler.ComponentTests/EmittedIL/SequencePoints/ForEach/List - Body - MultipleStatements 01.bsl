module Module

let f (l: int list) =
    for i in l do
        System.Console.WriteLine i
        System.Console.WriteLine(i + 1)
--------------------------------------------------------------------------------

Module::f
  (4,14-4,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call get_TailOrNull
    IL_0008:  stloc.1
    IL_0009:  br.s IL_0029

  (4,5-4,10)  for i
    IL_000b:  ldloc.0
    IL_000c:  call get_HeadOrDefault
    IL_0011:  stloc.2

  (5,9-5,35)  System.Console.WriteLine i
    IL_0012:  ldloc.2
    IL_0013:  call Console::WriteLine

  (6,9-6,40)  System.Console.WriteLine(i + 1)
    IL_0018:  ldloc.2
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  call Console::WriteLine

  <hidden>
    IL_0020:  ldloc.1
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  call get_TailOrNull
    IL_0028:  stloc.1

  (4,11-4,13)  in
    IL_0029:  ldloc.1
    IL_002a:  brtrue.s IL_000b
    IL_002c:  ret
