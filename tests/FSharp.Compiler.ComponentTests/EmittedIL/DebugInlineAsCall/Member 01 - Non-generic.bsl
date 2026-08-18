type T() =
    member inline _.Double(x: int) = x + x

[<EntryPoint>]
let main _ =
    let t = T()
    let i = t.Double(5)
    if i = 10 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (7,5-7,16)  let t = T()
    IL_0000:  newobj T::.ctor
    IL_0005:  stloc.0

  (8,5-8,24)  let i = t.Double(5)
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.5
    IL_0008:  callvirt T::Double
    IL_000d:  stloc.1

  (9,5-9,19)  if i = 10 then
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.s 10
    IL_0011:  bne.un.s IL_0015

  (9,20-9,21)  0
    IL_0013:  ldc.i4.0
    IL_0014:  ret

  (9,27-9,28)  1
    IL_0015:  ldc.i4.1
    IL_0016:  ret

T::.ctor
  (2,6-2,7)  T
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::Double
  (3,38-3,43)  x + x
    IL_0000:  ldarg.1
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
