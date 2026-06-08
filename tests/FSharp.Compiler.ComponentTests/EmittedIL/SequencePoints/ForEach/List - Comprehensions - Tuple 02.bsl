Module::f
  (5,5-8,6)  [ for i, i1 in l do yield i ]
    IL_0000:  nop

  (6,9-6,12)  for
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  call get_TailOrNull
    IL_0009:  stloc.2
    IL_000a:  br.s IL_003a
    IL_000c:  ldloc.1
    IL_000d:  call get_HeadOrDefault
    IL_0012:  stloc.3
    IL_0013:  ldloca.s 0
    IL_0015:  ldloc.3
    IL_0016:  call get_Item2
    IL_001b:  stloc.s 4
    IL_001d:  ldloc.3
    IL_001e:  call get_Item1
    IL_0023:  stloc.s 5
    IL_0025:  stloc.s 6

  (7,13-7,20)  yield i
    IL_0027:  ldloc.s 6
    IL_0029:  ldloc.s 5
    IL_002b:  call Add
    IL_0030:  nop
    IL_0031:  ldloc.2
    IL_0032:  stloc.1
    IL_0033:  ldloc.1
    IL_0034:  call get_TailOrNull
    IL_0039:  stloc.2

  (6,19-6,21)  in
    IL_003a:  ldloc.2
    IL_003b:  brtrue.s IL_000c
    IL_003d:  ldloca.s 0
    IL_003f:  call Close
    IL_0044:  ret
