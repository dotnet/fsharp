module Module

let f (l: int list) =
    for i in l do
        ()
        ()
--------------------------------------------------------------------------------

Module::f
  (4,14-4,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call get_TailOrNull
    IL_0008:  stloc.1
    IL_0009:  br.s IL_001d

  (4,5-4,10)  for i
    IL_000b:  ldloc.0
    IL_000c:  call get_HeadOrDefault
    IL_0011:  stloc.2

  (5,9-5,11)  ()
    IL_0012:  nop

  (6,9-6,11)  ()
    IL_0013:  nop

  <hidden>
    IL_0014:  ldloc.1
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  call get_TailOrNull
    IL_001c:  stloc.1

  (4,11-4,13)  in
    IL_001d:  ldloc.1
    IL_001e:  brtrue.s IL_000b
    IL_0020:  ret
