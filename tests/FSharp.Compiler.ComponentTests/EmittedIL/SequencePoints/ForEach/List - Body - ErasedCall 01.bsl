module Module

let f (l: int list) =
    for i in l do
        System.Diagnostics.Debug.Write ""
--------------------------------------------------------------------------------

Module::f
  (4,14-4,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call get_TailOrNull
    IL_0008:  stloc.1
    IL_0009:  br.s IL_001b

  (4,5-4,10)  for i
    IL_000b:  ldloc.0
    IL_000c:  call get_HeadOrDefault
    IL_0011:  stloc.2

  <hidden>
    IL_0012:  ldloc.1
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  call get_TailOrNull
    IL_001a:  stloc.1

  (4,11-4,13)  in
    IL_001b:  ldloc.1
    IL_001c:  brtrue.s IL_000b
    IL_001e:  ret
