Module::f
  (5,14-5,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call get_TailOrNull
    IL_0008:  stloc.1
    IL_0009:  br.s IL_001c

  (5,5-5,10)  for i
    IL_000b:  ldloc.0
    IL_000c:  call get_HeadOrDefault
    IL_0011:  stloc.2

  (6,9-6,11)  ()
    IL_0012:  nop

  <hidden>
    IL_0013:  ldloc.1
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  call get_TailOrNull
    IL_001b:  stloc.1

  (5,11-5,13)  in
    IL_001c:  ldloc.1
    IL_001d:  brtrue.s IL_000b
    IL_001f:  ret
