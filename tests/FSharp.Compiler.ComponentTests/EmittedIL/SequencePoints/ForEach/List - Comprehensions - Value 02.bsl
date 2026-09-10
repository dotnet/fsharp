module Module

let f (l: int list) =
    [
        for n in l do
            yield n
    ]
--------------------------------------------------------------------------------

Module::f
  (4,5-7,6)  [ for n in l do yield n ]
    IL_0000:  nop

  (5,9-5,12)  for
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  call get_TailOrNull
    IL_0009:  stloc.2
    IL_000a:  br.s IL_0029
    IL_000c:  ldloc.1
    IL_000d:  call get_HeadOrDefault
    IL_0012:  stloc.3
    IL_0013:  ldloca.s 0
    IL_0015:  stloc.s 4

  (6,13-6,20)  yield n
    IL_0017:  ldloc.s 4
    IL_0019:  ldloc.3
    IL_001a:  call Add
    IL_001f:  nop
    IL_0020:  ldloc.2
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  call get_TailOrNull
    IL_0028:  stloc.2

  (5,15-5,17)  in
    IL_0029:  ldloc.2
    IL_002a:  brtrue.s IL_000c
    IL_002c:  ldloca.s 0
    IL_002e:  call Close
    IL_0033:  ret
