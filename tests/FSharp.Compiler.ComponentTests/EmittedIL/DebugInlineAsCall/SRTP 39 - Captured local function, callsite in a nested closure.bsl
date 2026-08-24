let f () =
    let tee g (x: int) = g x; x
    let inline addEnum value = tee (fun x -> ignore (int value))
    let pipeline v = id >> addEnum v
    pipeline 1uy 41

[<EntryPoint>]
let main _ =
    if f () = 41 then 0 else 1
--------------------------------------------------------------------------------

Test::f
  <hidden>
    IL_0000:  ldsfld tee@3::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  newobj addEnum@4::.ctor
    IL_000c:  stloc.1
    IL_000d:  ldloc.0
    IL_000e:  newobj pipeline@5::.ctor
    IL_0013:  stloc.2

  (6,5-6,20)  pipeline 1uy 41
    IL_0014:  ldloc.2
    IL_0015:  ldc.i4.1
    IL_0016:  ldc.i4.s 41
    IL_0018:  tail.
    IL_001a:  call InvokeFast
    IL_001f:  ret

Test::main
  (10,5-10,22)  if f () = 41 then
    IL_0000:  call Test::f
    IL_0005:  ldc.i4.s 41
    IL_0007:  bne.un.s IL_000b

  (10,23-10,24)  0
    IL_0009:  ldc.i4.0
    IL_000a:  ret

  (10,30-10,31)  1
    IL_000b:  ldc.i4.1
    IL_000c:  ret

Test::<addEnum>__debug@5
  (4,32-4,65)  tee (fun x -> ignore (int value))
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj pipeline@4-2::.ctor
    IL_0007:  tail.
    IL_0009:  callvirt Invoke
    IL_000e:  ret

tee@3::Invoke
  (3,26-3,29)  g x
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  callvirt Invoke
    IL_0007:  pop

  (3,31-3,32)  x
    IL_0008:  ldarg.2
    IL_0009:  ret

addEnum@4-1::Invoke
  (4,32-4,65)  tee (fun x -> ignore (int value))
    IL_0000:  ldarg.0
    IL_0001:  ldfld tee
    IL_0006:  ldarg.1
    IL_0007:  newobj .ctor
    IL_000c:  tail.
    IL_000e:  callvirt Invoke
    IL_0013:  ret

addEnum@4-2::Invoke
  (4,46-4,64)  ignore (int value)
    IL_0000:  ldarg.0
    IL_0001:  ldfld value
    IL_0006:  stloc.1
    IL_0007:  ldloc.1
    IL_0008:  call LanguagePrimitives::ExplicitDynamic
    IL_000d:  stloc.0
    IL_000e:  ldnull
    IL_000f:  ret

pipeline@4-2::Invoke
  (4,46-4,64)  ignore (int value)
    IL_0000:  ldarg.0
    IL_0001:  ldfld pipeline@4-2::value
    IL_0006:  conv.i4
    IL_0007:  stloc.0
    IL_0008:  ldnull
    IL_0009:  ret

pipeline@5::Invoke
  (5,22-5,37)  id >> addEnum v
    IL_0000:  ldsfld pipeline@5-1::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldarg.0
    IL_0007:  ldfld pipeline@5::tee
    IL_000c:  ldarg.1
    IL_000d:  call Test::<addEnum>__debug@5
    IL_0012:  stloc.1
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  newobj pipeline@5-3::.ctor
    IL_001a:  ret
