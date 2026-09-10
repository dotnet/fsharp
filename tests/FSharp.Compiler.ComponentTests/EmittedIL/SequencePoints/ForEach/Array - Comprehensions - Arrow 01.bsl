module Module

let f (l: int list) =
    [|
        for n in l -> n
    |]
--------------------------------------------------------------------------------

Module::f
  (4,5-6,7)  [| for n in l -> n |]
    IL_0000:  nop

  (5,9-5,12)  for
    IL_0001:  ldarg.0
    IL_0002:  callvirt GetEnumerator
    IL_0007:  stloc.1
    IL_0008:  br.s IL_001c
    IL_000a:  ldloc.1
    IL_000b:  callvirt get_Current
    IL_0010:  stloc.2
    IL_0011:  ldloca.s 0
    IL_0013:  stloc.3

  (5,23-5,24)  n
    IL_0014:  ldloc.3
    IL_0015:  ldloc.2
    IL_0016:  call Add
    IL_001b:  nop

  (5,15-5,17)  in
    IL_001c:  ldloc.1
    IL_001d:  callvirt IEnumerator::MoveNext
    IL_0022:  brtrue.s IL_000a
    IL_0024:  leave.s IL_003b
    IL_0026:  ldloc.1
    IL_0027:  isinst IDisposable
    IL_002c:  stloc.s 4
    IL_002e:  ldloc.s 4
    IL_0030:  brfalse.s IL_003a

  <hidden>
    IL_0032:  ldloc.s 4
    IL_0034:  callvirt IDisposable::Dispose
    IL_0039:  endfinally

  <hidden>
    IL_003a:  endfinally

  <hidden>
    IL_003b:  ldloca.s 0
    IL_003d:  call Close
    IL_0042:  ret
