type One() =
    member inline _.Source1 x = x

type Two() =
    inherit One()
    member inline this.Source2 x = base.Source1 x

[<EntryPoint>]
let main _ =
    if (Two()).Source2 42 = 42 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (11,5-11,36)  if (Two()).Source2 42 = 42 then
    IL_0000:  newobj Two::.ctor
    IL_0005:  ldc.i4.s 42
    IL_0007:  callvirt Two::Source2
    IL_000c:  ldc.i4.s 42
    IL_000e:  bne.un.s IL_0012

  (11,37-11,38)  0
    IL_0010:  ldc.i4.0
    IL_0011:  ret

  (11,44-11,45)  1
    IL_0012:  ldc.i4.1
    IL_0013:  ret

One::.ctor
  (2,6-2,9)  One
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

One::Source1
  (3,33-3,34)  x
    IL_0000:  ldarg.1
    IL_0001:  ret

Two::.ctor
  (6,13-6,18)  One()
    IL_0000:  ldarg.0
    IL_0001:  callvirt One::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

Two::Source2
  (7,36-7,50)  base.Source1 x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  callvirt One::Source1
    IL_0007:  ret
