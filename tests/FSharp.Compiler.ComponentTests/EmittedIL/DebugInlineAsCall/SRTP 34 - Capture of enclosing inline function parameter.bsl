let inline outer (a: int) =
    let inline g y = a + int y
    g 1uy

[<EntryPoint>]
let main _ =
    if outer 42 = 43 then 0 else 1
--------------------------------------------------------------------------------

Test::outer
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  newobj g@3::.ctor
    IL_0006:  stloc.0

  (4,5-4,10)  g 1uy
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.1
    IL_0009:  tail.
    IL_000b:  call Test::<g>__debug@4
    IL_0010:  ret

Test::main
  (8,5-8,26)  if outer 42 = 43 then
    IL_0000:  ldc.i4.s 42
    IL_0002:  call Test::outer
    IL_0007:  ldc.i4.s 43
    IL_0009:  bne.un.s IL_000d

  (8,27-8,28)  0
    IL_000b:  ldc.i4.0
    IL_000c:  ret

  (8,34-8,35)  1
    IL_000d:  ldc.i4.1
    IL_000e:  ret

Test::<g>__debug@4
  (3,22-3,31)  a + int y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i4
    IL_0003:  add
    IL_0004:  ret

g@3-1::Invoke
  (3,22-3,31)  a + int y
    IL_0000:  ldarg.0
    IL_0001:  ldfld a
    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  call LanguagePrimitives::ExplicitDynamic
    IL_000e:  add
    IL_000f:  ret
