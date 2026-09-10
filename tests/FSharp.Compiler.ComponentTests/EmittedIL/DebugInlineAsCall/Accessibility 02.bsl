module Module

type T() =
    member inline internal this.InternalMethod() =
        ()

    member inline this.Method() =
        this.InternalMethod()
--------------------------------------------------------------------------------

T::.ctor
  (4,6-4,7)  T
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::InternalMethod
  (6,9-6,11)  ()
    IL_0000:  ret

T::Method
  (9,9-9,30)  this.InternalMethod()
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call T::<InternalMethod>__debug@9
    IL_0008:  ret

T::<InternalMethod>__debug@9
  (6,9-6,11)  ()
    IL_0000:  ret
