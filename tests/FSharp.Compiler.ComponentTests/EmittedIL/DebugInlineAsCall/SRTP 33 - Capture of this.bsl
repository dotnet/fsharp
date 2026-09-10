type C(n: int) =
    member _.M(b: byte) =
        let inline g y = n + int y
        g b

[<EntryPoint>]
let main _ =
    if C(42).M(5uy) = 47 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (9,5-9,30)  if C(42).M(5uy) = 47 then
    IL_0000:  ldc.i4.s 42
    IL_0002:  newobj C::.ctor
    IL_0007:  ldc.i4.5
    IL_0008:  callvirt C::M
    IL_000d:  ldc.i4.s 47
    IL_000f:  bne.un.s IL_0013

  (9,31-9,32)  0
    IL_0011:  ldc.i4.0
    IL_0012:  ret

  (9,38-9,39)  1
    IL_0013:  ldc.i4.1
    IL_0014:  ret

C::.ctor
  (2,6-2,7)  C
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  stfld C::n
    IL_000f:  ret

C::M
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  newobj g@4::.ctor
    IL_0006:  stloc.0

  (5,9-5,12)  g b
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  tail.
    IL_000b:  call C::<g>__debug@5
    IL_0010:  ret

C::<g>__debug@5
  (4,26-4,35)  n + int y
    IL_0000:  ldarg.0
    IL_0001:  ldfld C::n
    IL_0006:  ldarg.1
    IL_0007:  conv.i4
    IL_0008:  add
    IL_0009:  ret

g@4-1::Invoke
  (4,26-4,35)  n + int y
    IL_0000:  ldarg.0
    IL_0001:  ldfld _
    IL_0006:  ldfld C::n
    IL_000b:  ldarg.1
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  call LanguagePrimitives::ExplicitDynamic
    IL_0013:  add
    IL_0014:  ret
