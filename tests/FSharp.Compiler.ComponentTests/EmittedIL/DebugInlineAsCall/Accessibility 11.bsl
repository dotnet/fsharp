module Module

type T() =
    static let i = 1
    static member inline private InlinePrivateMethod() = i
    static member inline PublicMethod() = T.InlinePrivateMethod()

[<EntryPoint>]
let main _ =
    if T.PublicMethod() = 1 then 0 else 1
--------------------------------------------------------------------------------

Module::main
  (11,5-11,33)  if T.PublicMethod() = 1 then
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Module::init@
    IL_0006:  ldsfld $Module::init@
    IL_000b:  pop
    IL_000c:  call T::PublicMethod
    IL_0011:  ldc.i4.1
    IL_0012:  bne.un.s IL_0016

  (11,34-11,35)  0
    IL_0014:  ldc.i4.0
    IL_0015:  ret

  (11,41-11,42)  1
    IL_0016:  ldc.i4.1
    IL_0017:  ret

Module::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Module::init@
    IL_0006:  ldsfld $Module::init@
    IL_000b:  pop
    IL_000c:  ret

T::.ctor
  (4,6-4,7)  T
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::InlinePrivateMethod
  (6,58-6,59)  i
    IL_0000:  volatile.
    IL_0002:  ldsfld T::init@4
    IL_0007:  ldc.i4.1
    IL_0008:  bge.s IL_0013

  <hidden>
    IL_000a:  call IntrinsicFunctions::FailStaticInit
    IL_000f:  nop

  <hidden>
    IL_0010:  nop
    IL_0011:  br.s IL_0014

  <hidden>
    IL_0013:  nop
    IL_0014:  ldsfld T::i
    IL_0019:  ret

T::PublicMethod
  (7,43-7,66)  T.InlinePrivateMethod()
    IL_0000:  tail.
    IL_0002:  call T::<InlinePrivateMethod>__debug@7
    IL_0007:  ret

T::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Module::init@
    IL_0006:  ldsfld $Module::init@
    IL_000b:  pop
    IL_000c:  ret

T::staticInitialization@
  (5,12-5,21)  let i = 1
    IL_0000:  ldc.i4.1
    IL_0001:  stsfld T::i
    IL_0006:  ldc.i4.1
    IL_0007:  volatile.
    IL_0009:  stsfld T::init@4
    IL_000e:  ret

T::<InlinePrivateMethod>__debug@7
  (6,58-6,59)  i
    IL_0000:  volatile.
    IL_0002:  ldsfld T::init@4
    IL_0007:  ldc.i4.1
    IL_0008:  bge.s IL_0013

  <hidden>
    IL_000a:  call IntrinsicFunctions::FailStaticInit
    IL_000f:  nop

  <hidden>
    IL_0010:  nop
    IL_0011:  br.s IL_0014

  <hidden>
    IL_0013:  nop
    IL_0014:  ldsfld T::i
    IL_0019:  ret
