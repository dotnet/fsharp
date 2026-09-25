module Module

let (|Id|) (x: int) = x

let f (l: int list) =
    [|
        for Id i in l do
            yield i
    |]
--------------------------------------------------------------------------------

Module::|Id|
  (3,23-3,24)  x
    IL_0000:  ldarg.0
    IL_0001:  ret

Module::f
  (6,5-9,7)  [| for Id i in l do yield i |]
    IL_0000:  nop

  (7,9-7,12)  for
    IL_0001:  ldarg.0
    IL_0002:  callvirt GetEnumerator
    IL_0007:  stloc.1
    IL_0008:  br.s IL_0025
    IL_000a:  ldloc.1
    IL_000b:  callvirt get_Current
    IL_0010:  stloc.2
    IL_0011:  ldloc.2
    IL_0012:  call Module::|Id|
    IL_0017:  stloc.3
    IL_0018:  ldloc.3
    IL_0019:  stloc.s 4

  (8,13-8,20)  yield i
    IL_001b:  ldloca.s 0
    IL_001d:  ldloc.s 4
    IL_001f:  call Add
    IL_0024:  nop

  (7,18-7,20)  in
    IL_0025:  ldloc.1
    IL_0026:  callvirt IEnumerator::MoveNext
    IL_002b:  brtrue.s IL_000a
    IL_002d:  leave.s IL_0044
    IL_002f:  ldloc.1
    IL_0030:  isinst IDisposable
    IL_0035:  stloc.s 5
    IL_0037:  ldloc.s 5
    IL_0039:  brfalse.s IL_0043

  <hidden>
    IL_003b:  ldloc.s 5
    IL_003d:  callvirt IDisposable::Dispose
    IL_0042:  endfinally

  <hidden>
    IL_0043:  endfinally

  <hidden>
    IL_0044:  ldloca.s 0
    IL_0046:  call Close
    IL_004b:  ret
