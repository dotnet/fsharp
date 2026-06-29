[<EntryPoint>]
let main _ =
    let inline double (x: int) = x + x
    let i = double 5
    if i = 10 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  <hidden>
    IL_0000:  ldsfld double@4::@_instance
    IL_0005:  stloc.0

  (5,5-5,21)  let i = double 5
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.5
    IL_0008:  callvirt Invoke
    IL_000d:  stloc.1

  (6,5-6,19)  if i = 10 then
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.s 10
    IL_0011:  bne.un.s IL_0015

  (6,20-6,21)  0
    IL_0013:  ldc.i4.0
    IL_0014:  ret

  (6,27-6,28)  1
    IL_0015:  ldc.i4.1
    IL_0016:  ret

double@4::Invoke
  (4,34-4,39)  x + x
    IL_0000:  ldarg.1
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
