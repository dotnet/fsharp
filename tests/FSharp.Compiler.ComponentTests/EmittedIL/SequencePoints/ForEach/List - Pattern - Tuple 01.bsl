module Module

let f (l: (int * int) list) =
    for i1, i2 in l do
        ()
--------------------------------------------------------------------------------

Module::f
  (4,19-4,20)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call get_TailOrNull
    IL_0008:  stloc.1
    IL_0009:  br.s IL_002b

  (4,5-4,15)  for i1, i2
    IL_000b:  ldloc.0
    IL_000c:  call get_HeadOrDefault
    IL_0011:  stloc.2
    IL_0012:  ldloc.2
    IL_0013:  call get_Item2
    IL_0018:  stloc.3
    IL_0019:  ldloc.2
    IL_001a:  call get_Item1
    IL_001f:  stloc.s 4

  (5,9-5,11)  ()
    IL_0021:  nop

  <hidden>
    IL_0022:  ldloc.1
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  call get_TailOrNull
    IL_002a:  stloc.1

  (4,16-4,18)  in
    IL_002b:  ldloc.1
    IL_002c:  brtrue.s IL_000b
    IL_002e:  ret
