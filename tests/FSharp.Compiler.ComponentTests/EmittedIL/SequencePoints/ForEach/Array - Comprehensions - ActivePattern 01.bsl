Module::|Id|
  (4,23-4,24)  x
    IL_0000:  ldarg.0
    IL_0001:  ret

Module::f
  (7,5-10,7)  [| for Id i in l do yield i |]
    IL_0000:  nop

  (8,9-8,12)  for
    IL_0001:  ldarg.0
    IL_0002:  callvirt GetEnumerator
    IL_0007:  stloc.1
    IL_0008:  br.s IL_0027
    IL_000a:  ldloc.1
    IL_000b:  callvirt get_Current
    IL_0010:  stloc.3
    IL_0011:  ldloc.3
    IL_0012:  call |Id|
    IL_0017:  stloc.s 4
    IL_0019:  ldloc.s 4
    IL_001b:  stloc.s 5

  (9,13-9,20)  yield i
    IL_001d:  ldloca.s 0
    IL_001f:  ldloc.s 5
    IL_0021:  call Add
    IL_0026:  nop

  (8,18-8,20)  in
    IL_0027:  ldloc.1
    IL_0028:  callvirt MoveNext
    IL_002d:  brtrue.s IL_000a
    IL_002f:  ldnull
    IL_0030:  stloc.2
    IL_0031:  leave.s IL_0048
    IL_0033:  ldloc.1
    IL_0034:  isinst IDisposable
    IL_0039:  stloc.s 6

  <hidden>
    IL_003b:  ldloc.s 6
    IL_003d:  brfalse.s IL_0047

  <hidden>
    IL_003f:  ldloc.s 6
    IL_0041:  callvirt Dispose
    IL_0046:  endfinally

  <hidden>
    IL_0047:  endfinally

  <hidden>
    IL_0048:  ldloc.2
    IL_0049:  pop
    IL_004a:  ldloca.s 0
    IL_004c:  call Close
    IL_0051:  ret
