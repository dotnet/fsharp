module Module

type T() =
    static member private PrivateMethod() = 1
    static member inline private InlinePrivateMethod() = T.PrivateMethod()
    static member inline PublicMethod() = T.InlinePrivateMethod()

[<EntryPoint>]
let main _ =
    if T.PublicMethod() = 1 then 0 else 1
--------------------------------------------------------------------------------

Module::main
  (11,5-11,33)  if T.PublicMethod() = 1 then
    IL_0000:  call T::PublicMethod
    IL_0005:  ldc.i4.1
    IL_0006:  bne.un.s IL_000a

  (11,34-11,35)  0
    IL_0008:  ldc.i4.0
    IL_0009:  ret

  (11,41-11,42)  1
    IL_000a:  ldc.i4.1
    IL_000b:  ret

T::.ctor
  (4,6-4,7)  T
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::PrivateMethod
  (5,45-5,46)  1
    IL_0000:  ldc.i4.1
    IL_0001:  ret

T::InlinePrivateMethod
  (6,58-6,75)  T.PrivateMethod()
    IL_0000:  call T::PrivateMethod
    IL_0005:  ret

T::PublicMethod
  (7,43-7,66)  T.InlinePrivateMethod()
    IL_0000:  tail.
    IL_0002:  call T::<InlinePrivateMethod>__debug@7
    IL_0007:  ret

T::<InlinePrivateMethod>__debug@7
  (6,58-6,75)  T.PrivateMethod()
    IL_0000:  call T::PrivateMethod
    IL_0005:  ret
