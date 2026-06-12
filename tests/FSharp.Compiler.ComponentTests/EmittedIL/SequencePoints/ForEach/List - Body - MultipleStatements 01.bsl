Module::f
  (5,14-5,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call get_TailOrNull
    IL_0008:  stloc.1
    IL_0009:  br.s IL_0029

  (5,5-5,10)  for i
    IL_000b:  ldloc.0
    IL_000c:  call get_HeadOrDefault
    IL_0011:  stloc.2

  (6,9-6,35)  System.Console.WriteLine i
    IL_0012:  ldloc.2
    IL_0013:  call WriteLine

  (7,9-7,40)  System.Console.WriteLine(i + 1)
    IL_0018:  ldloc.2
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  call WriteLine

  <hidden>
    IL_0020:  ldloc.1
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  call get_TailOrNull
    IL_0028:  stloc.1

  (5,11-5,13)  in
    IL_0029:  ldloc.1
    IL_002a:  brtrue.s IL_000b
    IL_002c:  ret
