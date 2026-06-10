Module::|Id|
  (4,23-4,24)  x
    IL_0000:  ldarg.0
    IL_0001:  ret

Module::f
  (7,17-7,18)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt GetEnumerator
    IL_0008:  stloc.1
    IL_0009:  br.s IL_001d

  (7,5-7,13)  for Id i
    IL_000b:  ldloc.1
    IL_000c:  callvirt get_Current
    IL_0011:  stloc.2
    IL_0012:  ldloc.2
    IL_0013:  call |Id|
    IL_0018:  stloc.3
    IL_0019:  ldloc.3
    IL_001a:  stloc.s 4

  (8,9-8,11)  ()
    IL_001c:  nop

  (7,14-7,16)  in
    IL_001d:  ldloc.1
    IL_001e:  callvirt MoveNext
    IL_0023:  brtrue.s IL_000b
    IL_0025:  leave.s IL_003c
    IL_0027:  ldloc.1
    IL_0028:  isinst IDisposable
    IL_002d:  stloc.s 5

  <hidden>
    IL_002f:  ldloc.s 5
    IL_0031:  brfalse.s IL_003b

  <hidden>
    IL_0033:  ldloc.s 5
    IL_0035:  callvirt Dispose
    IL_003a:  endfinally

  <hidden>
    IL_003b:  endfinally

  <hidden>
    IL_003c:  ret
