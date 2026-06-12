Module::f
  (5,14-5,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call get_TailOrNull
    IL_0008:  stloc.1
    IL_0009:  br.s IL_0021

  (5,5-5,10)  for i
    IL_000b:  ldloc.0
    IL_000c:  call get_HeadOrDefault
    IL_0011:  stloc.2

  (6,9-6,35)  System.Console.WriteLine i
    IL_0012:  ldloc.2
    IL_0013:  call WriteLine

  <hidden>
    IL_0018:  ldloc.1
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  call get_TailOrNull
    IL_0020:  stloc.1

  (5,11-5,13)  in
    IL_0021:  ldloc.1
    IL_0022:  brtrue.s IL_000b
    IL_0024:  ret
