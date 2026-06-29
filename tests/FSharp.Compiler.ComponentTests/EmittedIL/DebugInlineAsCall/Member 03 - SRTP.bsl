type T() =
    member inline _.Add(x: ^T, y: ^T) = x + y

[<EntryPoint>]
let main _ =
    let t = T()
    let i = t.Add(3, 4)
    if i = 7 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (7,5-7,16)  let t = T()
    IL_0000:  newobj T::.ctor
    IL_0005:  stloc.0

  (8,5-8,24)  let i = t.Add(3, 4)
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.3
    IL_0008:  ldc.i4.4
    IL_0009:  call Test::<Add>__debug@8
    IL_000e:  stloc.1

  (9,5-9,18)  if i = 7 then
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.7
    IL_0011:  bne.un.s IL_0015

  (9,19-9,20)  0
    IL_0013:  ldc.i4.0
    IL_0014:  ret

  (9,26-9,27)  1
    IL_0015:  ldc.i4.1
    IL_0016:  ret

Test::<Add>__debug@8
  (3,41-3,46)  x + y
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  add
    IL_0003:  ret

T::.ctor
  (2,6-2,7)  T
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::Add
  (3,41-3,46)  x + y
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.2
    IL_0003:  stloc.1
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  tail.
    IL_0008:  call LanguagePrimitives::AdditionDynamic
    IL_000d:  ret

T::Add$W
  (3,41-3,46)  x + y
    IL_0000:  ldarg.2
    IL_0001:  stloc.0
    IL_0002:  ldarg.3
    IL_0003:  stloc.1
    IL_0004:  ldarg.1
    IL_0005:  ldloc.0
    IL_0006:  ldloc.1
    IL_0007:  tail.
    IL_0009:  call InvokeFast
    IL_000e:  ret
