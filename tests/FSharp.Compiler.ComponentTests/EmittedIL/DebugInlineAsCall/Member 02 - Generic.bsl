type T() =
    member inline _.Apply(f: 'a -> 'b, x: 'a) : 'b = f x

[<EntryPoint>]
let main _ =
    let t = T()
    let i = t.Apply((fun x -> x + 1), 5)
    if i = 6 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (7,5-7,16)  let t = T()
    IL_0000:  newobj T::.ctor
    IL_0005:  stloc.0

  (8,5-8,41)  let i = t.Apply((fun x -> x + 1), 5)
    IL_0006:  ldloc.0
    IL_0007:  ldsfld i@8::@_instance
    IL_000c:  ldc.i4.5
    IL_000d:  callvirt T::Apply
    IL_0012:  stloc.1

  (9,5-9,18)  if i = 6 then
    IL_0013:  ldloc.1
    IL_0014:  ldc.i4.6
    IL_0015:  bne.un.s IL_0019

  (9,19-9,20)  0
    IL_0017:  ldc.i4.0
    IL_0018:  ret

  (9,26-9,27)  1
    IL_0019:  ldc.i4.1
    IL_001a:  ret

T::.ctor
  (2,6-2,7)  T
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::Apply
  (3,54-3,57)  f x
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  tail.
    IL_0004:  callvirt Invoke
    IL_0009:  ret

i@8::Invoke
  (8,31-8,36)  x + 1
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  add
    IL_0003:  ret
