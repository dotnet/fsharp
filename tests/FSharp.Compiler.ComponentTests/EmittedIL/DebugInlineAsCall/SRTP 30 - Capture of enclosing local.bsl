let f () =
    let x = 42
    let inline g y = x + int y
    g 1uy

[<EntryPoint>]
let main _ =
    if f () = 43 then 0 else 1
--------------------------------------------------------------------------------

Test::f
  (3,5-3,15)  let x = 42
    IL_0000:  ldc.i4.s 42
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  newobj g@4::.ctor
    IL_0009:  stloc.1

  (5,5-5,10)  g 1uy
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  tail.
    IL_000e:  call Test::<g>__debug@5
    IL_0013:  ret

Test::main
  (9,5-9,22)  if f () = 43 then
    IL_0000:  call Test::f
    IL_0005:  ldc.i4.s 43
    IL_0007:  bne.un.s IL_000b

  (9,23-9,24)  0
    IL_0009:  ldc.i4.0
    IL_000a:  ret

  (9,30-9,31)  1
    IL_000b:  ldc.i4.1
    IL_000c:  ret

Test::<g>__debug@5
  (4,22-4,31)  x + int y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i4
    IL_0003:  add
    IL_0004:  ret

g@4-1::Invoke
  (4,22-4,31)  x + int y
    IL_0000:  ldarg.0
    IL_0001:  ldfld x
    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  call LanguagePrimitives::ExplicitDynamic
    IL_000e:  add
    IL_000f:  ret
