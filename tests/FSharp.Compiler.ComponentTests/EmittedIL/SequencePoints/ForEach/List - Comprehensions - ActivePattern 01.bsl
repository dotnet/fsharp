module Module

let (|Id|) (x: int) = x

let f (l: int list) =
    [
        for Id i in l do
            yield i
    ]
--------------------------------------------------------------------------------

Module::|Id|
  (3,23-3,24)  x
    IL_0000:  ldarg.0
    IL_0001:  ret

Module::f
  (6,5-9,6)  [ for Id i in l do yield i ]
    IL_0000:  nop

  (7,9-7,12)  for
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  call get_TailOrNull
    IL_0009:  stloc.2
    IL_000a:  br.s IL_0036
    IL_000c:  ldloc.1
    IL_000d:  call get_HeadOrDefault
    IL_0012:  stloc.3
    IL_0013:  ldloca.s 0
    IL_0015:  ldloc.3
    IL_0016:  call |Id|
    IL_001b:  stloc.s 4
    IL_001d:  ldloc.s 4
    IL_001f:  stloc.s 5
    IL_0021:  stloc.s 6

  (8,13-8,20)  yield i
    IL_0023:  ldloc.s 6
    IL_0025:  ldloc.s 5
    IL_0027:  call Add
    IL_002c:  nop
    IL_002d:  ldloc.2
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  call get_TailOrNull
    IL_0035:  stloc.2

  (7,18-7,20)  in
    IL_0036:  ldloc.2
    IL_0037:  brtrue.s IL_000c
    IL_0039:  ldloca.s 0
    IL_003b:  call Close
    IL_0040:  ret
