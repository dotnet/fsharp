module Module

type T() =
    let i = 1
    member inline private this.InlinePrivateMethod() = i
    member inline this.PublicMethod() = this.InlinePrivateMethod()

[<EntryPoint>]
let main _ =
    if T().PublicMethod() = 1 then 0 else 1
--------------------------------------------------------------------------------

Module::main
  (11,5-11,35)  if T().PublicMethod() = 1 then
    IL_0000:  newobj T::.ctor
    IL_0005:  callvirt T::PublicMethod
    IL_000a:  ldc.i4.1
    IL_000b:  bne.un.s IL_000f

  (11,36-11,37)  0
    IL_000d:  ldc.i4.0
    IL_000e:  ret

  (11,43-11,44)  1
    IL_000f:  ldc.i4.1
    IL_0010:  ret

T::.ctor
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop

  (5,5-5,14)  let i = 1
    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.1
    IL_000a:  stfld T::i
    IL_000f:  ret

T::InlinePrivateMethod
  (6,56-6,57)  i
    IL_0000:  ldarg.0
    IL_0001:  ldfld T::i
    IL_0006:  ret

T::PublicMethod
  (7,41-7,67)  this.InlinePrivateMethod()
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call T::<InlinePrivateMethod>__debug@7
    IL_0008:  ret

T::<InlinePrivateMethod>__debug@7
  (6,56-6,57)  i
    IL_0000:  ldarg.0
    IL_0001:  ldfld T::i
    IL_0006:  ret
