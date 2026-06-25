module Module

let f (l: (int * int) list) =
    [|
        for n in l do
            yield n
    |]
--------------------------------------------------------------------------------

Module::f
  (4,5-7,7)  [| for n in l do yield n |]
    IL_0000:  nop

  (5,9-5,12)  for
    IL_0001:  ldarg.0
    IL_0002:  callvirt GetEnumerator
    IL_0007:  stloc.1
    IL_0008:  br.s IL_001e
    IL_000a:  ldloc.1
    IL_000b:  callvirt get_Current
    IL_0010:  stloc.3
    IL_0011:  ldloca.s 0
    IL_0013:  stloc.s 4

  (6,13-6,20)  yield n
    IL_0015:  ldloc.s 4
    IL_0017:  ldloc.3
    IL_0018:  call Add
    IL_001d:  nop

  (5,15-5,17)  in
    IL_001e:  ldloc.1
    IL_001f:  callvirt IEnumerator::MoveNext
    IL_0024:  brtrue.s IL_000a
    IL_0026:  ldnull
    IL_0027:  stloc.2
    IL_0028:  leave.s IL_003f
    IL_002a:  ldloc.1
    IL_002b:  isinst IDisposable
    IL_0030:  stloc.s 5
    IL_0032:  ldloc.s 5
    IL_0034:  brfalse.s IL_003e

  <hidden>
    IL_0036:  ldloc.s 5
    IL_0038:  callvirt IDisposable::Dispose
    IL_003d:  endfinally

  <hidden>
    IL_003e:  endfinally

  <hidden>
    IL_003f:  ldloc.2
    IL_0040:  pop
    IL_0041:  ldloca.s 0
    IL_0043:  call Close
    IL_0048:  ret
