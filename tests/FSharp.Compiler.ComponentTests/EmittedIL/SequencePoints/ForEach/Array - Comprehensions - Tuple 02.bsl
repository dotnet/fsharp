module Module

let f (l: (int * int) list) =
    [|
        for i, i1 in l do
            yield i
    |]
--------------------------------------------------------------------------------

Module::f
  (4,5-7,7)  [| for i, i1 in l do yield i |]
    IL_0000:  nop

  (5,9-5,12)  for
    IL_0001:  ldarg.0
    IL_0002:  callvirt GetEnumerator
    IL_0007:  stloc.1
    IL_0008:  br.s IL_002a
    IL_000a:  ldloc.1
    IL_000b:  callvirt get_Current
    IL_0010:  stloc.2
    IL_0011:  ldloc.2
    IL_0012:  call get_Item2
    IL_0017:  stloc.3
    IL_0018:  ldloc.2
    IL_0019:  call get_Item1
    IL_001e:  stloc.s 4

  (6,13-6,20)  yield i
    IL_0020:  ldloca.s 0
    IL_0022:  ldloc.s 4
    IL_0024:  call Add
    IL_0029:  nop

  (5,19-5,21)  in
    IL_002a:  ldloc.1
    IL_002b:  callvirt IEnumerator::MoveNext
    IL_0030:  brtrue.s IL_000a
    IL_0032:  leave.s IL_0049
    IL_0034:  ldloc.1
    IL_0035:  isinst IDisposable
    IL_003a:  stloc.s 5
    IL_003c:  ldloc.s 5
    IL_003e:  brfalse.s IL_0048

  <hidden>
    IL_0040:  ldloc.s 5
    IL_0042:  callvirt IDisposable::Dispose
    IL_0047:  endfinally

  <hidden>
    IL_0048:  endfinally

  <hidden>
    IL_0049:  ldloca.s 0
    IL_004b:  call Close
    IL_0050:  ret
