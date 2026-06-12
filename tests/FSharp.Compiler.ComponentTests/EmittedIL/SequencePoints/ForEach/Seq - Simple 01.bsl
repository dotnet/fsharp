Module::f
  (5,14-5,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt GetEnumerator
    IL_0008:  stloc.1
    IL_0009:  br.s IL_0013

  (5,5-5,10)  for i
    IL_000b:  ldloc.1
    IL_000c:  callvirt get_Current
    IL_0011:  stloc.2

  (6,9-6,11)  ()
    IL_0012:  nop

  (5,11-5,13)  in
    IL_0013:  ldloc.1
    IL_0014:  callvirt MoveNext
    IL_0019:  brtrue.s IL_000b
    IL_001b:  leave.s IL_002f
    IL_001d:  ldloc.1
    IL_001e:  isinst IDisposable
    IL_0023:  stloc.3

  <hidden>
    IL_0024:  ldloc.3
    IL_0025:  brfalse.s IL_002e

  <hidden>
    IL_0027:  ldloc.3
    IL_0028:  callvirt Dispose
    IL_002d:  endfinally

  <hidden>
    IL_002e:  endfinally

  <hidden>
    IL_002f:  ret
