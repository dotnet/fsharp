module Module

type T() =
    member private this.PrivateMethod() = 1
    member inline private this.InlinePrivateMethod() = this.PrivateMethod()
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
  (4,6-4,7)  T
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::PrivateMethod
  (5,43-5,44)  1
    IL_0000:  ldc.i4.1
    IL_0001:  ret

T::InlinePrivateMethod
  (6,56-6,76)  this.PrivateMethod()
    IL_0000:  ldarg.0
    IL_0001:  callvirt T::PrivateMethod
    IL_0006:  ret

T::PublicMethod
  (7,41-7,67)  this.InlinePrivateMethod()
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call T::<InlinePrivateMethod>__debug@7
    IL_0008:  ret

T::<InlinePrivateMethod>__debug@7
  (6,56-6,76)  this.PrivateMethod()
    IL_0000:  ldarg.0
    IL_0001:  callvirt T::PrivateMethod
    IL_0006:  ret
