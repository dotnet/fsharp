[<Struct>]
type S =
    member _.M() = ()

type T() =
    member _.N() = ()

let inline f (a: ^A when ^A: (member M: unit -> unit)) (_b: ^B when ^B: (member N: unit -> unit)) =
    (^A: (member M: unit -> unit) a)

let inline g b = f (S()) b

[<EntryPoint>]
let main _ =
    g (T())
    0
--------------------------------------------------------------------------------

Test::f
  (10,5-10,37)  (^A: (member M: unit -> unit) a)
    IL_0000:  ldstr "Dynamic invocation of M is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

Test::f$W
  (10,5-10,37)  (^A: (member M: unit -> unit) a)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.2
    IL_0002:  callvirt Invoke
    IL_0007:  pop
    IL_0008:  ret

Test::g
  (12,18-12,27)  f (S()) b
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldarg.0
    IL_0009:  call InvokeFast
    IL_000e:  pop
    IL_000f:  ret

Test::g$W
  (12,18-12,27)  f (S()) b
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldloc.1
    IL_0009:  ldarg.1
    IL_000a:  call InvokeFast
    IL_000f:  pop
    IL_0010:  ret

Test::main
  (16,5-16,12)  g (T())
    IL_0000:  newobj T::.ctor
    IL_0005:  call Test::<g>__debug@16
    IL_000a:  nop

  (17,5-17,6)  0
    IL_000b:  ldc.i4.0
    IL_000c:  ret

Test::<g>__debug@16
  (12,18-12,27)  f (S()) b
    IL_0000:  ldsfld main@9::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldarg.0
    IL_0009:  call InvokeFast
    IL_000e:  pop
    IL_000f:  ret

S::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst S
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s IL_001d

  <hidden>
    IL_000d:  ldarg.1
    IL_000e:  unbox.any S
    IL_0013:  stloc.1
    IL_0014:  ldarg.0
    IL_0015:  ldloc.1
    IL_0016:  ldarg.2
    IL_0017:  call S::Equals
    IL_001c:  ret

  <hidden>
    IL_001d:  ldc.i4.0
    IL_001e:  ret

S::M
  (4,20-4,22)  ()
    IL_0000:  ret

S::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst S
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s IL_001c

  <hidden>
    IL_000d:  ldarg.1
    IL_000e:  unbox.any S
    IL_0013:  stloc.1
    IL_0014:  ldarg.0
    IL_0015:  ldloc.1
    IL_0016:  call S::Equals
    IL_001b:  ret

  <hidden>
    IL_001c:  ldc.i4.0
    IL_001d:  ret

T::.ctor
  (6,6-6,7)  T
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::N
  (7,20-7,22)  ()
    IL_0000:  ret

g@9::Invoke
  (10,5-10,37)  (^A: (member M: unit -> unit) a)
    IL_0000:  ldarga.s 1
    IL_0002:  call S::M
    IL_0007:  nop
    IL_0008:  ldnull
    IL_0009:  ret

g@9-1::Invoke
  (10,5-10,37)  (^A: (member M: unit -> unit) a)
    IL_0000:  ldarga.s 1
    IL_0002:  call S::M
    IL_0007:  nop
    IL_0008:  ldnull
    IL_0009:  ret

main@9::Invoke
  (10,5-10,37)  (^A: (member M: unit -> unit) a)
    IL_0000:  ldarga.s 1
    IL_0002:  call S::M
    IL_0007:  nop
    IL_0008:  ldnull
    IL_0009:  ret
