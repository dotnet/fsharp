[<EntryPoint>]
let main _ =
    let inline f (x: int) =
        let i = 5 + 10
        x + i

    let i = f 20
    if i = 35 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  <hidden>
    IL_0000:  ldsfld f@5::@_instance
    IL_0005:  stloc.0

  (8,5-8,17)  let i = f 20
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.s 20
    IL_0009:  callvirt Invoke
    IL_000e:  stloc.1

  (9,5-9,19)  if i = 35 then
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.s 35
    IL_0012:  bne.un.s IL_0016

  (9,20-9,21)  0
    IL_0014:  ldc.i4.0
    IL_0015:  ret

  (9,27-9,28)  1
    IL_0016:  ldc.i4.1
    IL_0017:  ret

f@5::Invoke
  (5,9-5,23)  let i = 5 + 10
    IL_0000:  ldc.i4.5
    IL_0001:  ldc.i4.s 10
    IL_0003:  add
    IL_0004:  stloc.0

  (6,9-6,14)  x + i
    IL_0005:  ldarg.1
    IL_0006:  ldloc.0
    IL_0007:  add
    IL_0008:  ret
