let f () =
    let x = 100
    let inline g y = x + int y
    g 1uy + g 2s

[<EntryPoint>]
let main _ =
    if f () = 203 then 0 else 1
--------------------------------------------------------------------------------

Test::f
  (3,5-3,16)  let x = 100
    IL_0000:  ldc.i4.s 100
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  newobj g@4::.ctor
    IL_0009:  stloc.1

  (5,5-5,17)  g 1uy + g 2s
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  call Test::<g>__debug@5
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.2
    IL_0013:  call Test::<g>__debug@5-1
    IL_0018:  add
    IL_0019:  ret

Test::main
  (9,5-9,23)  if f () = 203 then
    IL_0000:  call Test::f
    IL_0005:  ldc.i4 203
    IL_000a:  bne.un.s IL_000e

  (9,24-9,25)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (9,31-9,32)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret

Test::<g>__debug@5
  (4,22-4,31)  x + int y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i4
    IL_0003:  add
    IL_0004:  ret

Test::<g>__debug@5-1
  (4,22-4,31)  x + int y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret

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
