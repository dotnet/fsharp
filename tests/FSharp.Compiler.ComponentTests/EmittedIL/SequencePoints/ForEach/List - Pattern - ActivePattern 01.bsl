module Module

let (|Id|) (x: int) = x

let f (l: int list) =
    for Id i in l do
        ()
--------------------------------------------------------------------------------

Module::f
  (6,17-6,18)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call get_TailOrNull
    IL_0008:  stloc.1
    IL_0009:  br.s IL_0026

  (6,5-6,13)  for Id i
    IL_000b:  ldloc.0
    IL_000c:  call get_HeadOrDefault
    IL_0011:  stloc.2
    IL_0012:  ldloc.2
    IL_0013:  call |Id|
    IL_0018:  stloc.3
    IL_0019:  ldloc.3
    IL_001a:  stloc.s 4

  (7,9-7,11)  ()
    IL_001c:  nop

  <hidden>
    IL_001d:  ldloc.1
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  call get_TailOrNull
    IL_0025:  stloc.1

  (6,14-6,16)  in
    IL_0026:  ldloc.1
    IL_0027:  brtrue.s IL_000b
    IL_0029:  ret

Module::|Id|
  (3,23-3,24)  x
    IL_0000:  ldarg.0
    IL_0001:  ret
