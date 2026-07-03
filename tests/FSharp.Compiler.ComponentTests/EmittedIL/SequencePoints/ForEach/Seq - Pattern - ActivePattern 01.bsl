module Module

let (|Id|) (x: int) = x

let f (l: int seq) =
    for Id i in l do
        ()
--------------------------------------------------------------------------------

Module::|Id|
  (3,23-3,24)  x
    IL_0000:  ldarg.0
    IL_0001:  ret

Module::f
  (6,17-6,18)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt GetEnumerator
    IL_0008:  stloc.1
    IL_0009:  br.s IL_001d

  (6,5-6,13)  for Id i
    IL_000b:  ldloc.1
    IL_000c:  callvirt get_Current
    IL_0011:  stloc.2
    IL_0012:  ldloc.2
    IL_0013:  call Module::|Id|
    IL_0018:  stloc.3
    IL_0019:  ldloc.3
    IL_001a:  stloc.s 4

  (7,9-7,11)  ()
    IL_001c:  nop

  (6,14-6,16)  in
    IL_001d:  ldloc.1
    IL_001e:  callvirt IEnumerator::MoveNext
    IL_0023:  brtrue.s IL_000b
    IL_0025:  leave.s IL_003c
    IL_0027:  ldloc.1
    IL_0028:  isinst IDisposable
    IL_002d:  stloc.s 5
    IL_002f:  ldloc.s 5
    IL_0031:  brfalse.s IL_003b

  <hidden>
    IL_0033:  ldloc.s 5
    IL_0035:  callvirt IDisposable::Dispose
    IL_003a:  endfinally

  <hidden>
    IL_003b:  endfinally

  <hidden>
    IL_003c:  ret
