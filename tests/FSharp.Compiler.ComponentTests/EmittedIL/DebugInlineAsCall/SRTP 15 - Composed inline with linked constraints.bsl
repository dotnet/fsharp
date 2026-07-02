[<Struct>]
type S<'T> = member _.M(_: 'T) = ()

let inline f<'A, 'R, 'B when 'A: (member M: int -> 'R) and 'B: (member M: 'R -> unit)> (_a: 'A) : 'B = Unchecked.defaultof<_>

let inline g (_: S<'T>) = 42

let inline h a = g (f a)

[<EntryPoint>]
let main _ =
    let i = h (S<int>())
    if i = 42 then 0 else 1
--------------------------------------------------------------------------------

Test::f
  (5,104-5,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret

Test::f$W
  (5,104-5,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret

Test::g
  (7,27-7,29)  42
    IL_0000:  ldc.i4.s 42
    IL_0002:  ret

Test::h
  (9,18-9,25)  g (f a)
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  callvirt Invoke
    IL_000d:  call Test::g
    IL_0012:  ret

Test::h$W
  (9,18-9,25)  g (f a)
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.1
    IL_0009:  callvirt Invoke
    IL_000e:  call Test::g
    IL_0013:  ret

Test::main
  (13,5-13,25)  let i = h (S<int>())
    IL_0000:  ldloc.1
    IL_0001:  call Test::<h>__debug@13
    IL_0006:  stloc.0

  (14,5-14,19)  if i = 42 then
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s 42
    IL_000a:  bne.un.s IL_000e

  (14,20-14,21)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (14,27-14,28)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret

Test::<h>__debug@13
  (9,18-9,25)  g (f a)
    IL_0000:  ldsfld i@5::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  callvirt Invoke
    IL_000d:  call Test::g
    IL_0012:  ret

S`1::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst 0x1b000008
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s IL_001d

  <hidden>
    IL_000d:  ldarg.1
    IL_000e:  unbox.any 0x1b000008
    IL_0013:  stloc.1
    IL_0014:  ldarg.0
    IL_0015:  ldloc.1
    IL_0016:  ldarg.2
    IL_0017:  call Equals
    IL_001c:  ret

  <hidden>
    IL_001d:  ldc.i4.0
    IL_001e:  ret

S`1::M
  (3,34-3,36)  ()
    IL_0000:  ret

S`1::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst 0x1b000008
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s IL_001c

  <hidden>
    IL_000d:  ldarg.1
    IL_000e:  unbox.any 0x1b000008
    IL_0013:  stloc.1
    IL_0014:  ldarg.0
    IL_0015:  ldloc.1
    IL_0016:  call Equals
    IL_001b:  ret

  <hidden>
    IL_001c:  ldc.i4.0
    IL_001d:  ret

h@5::Invoke
  (5,104-5,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret

h@5-1::Invoke
  (5,104-5,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret

i@5::Invoke
  (5,104-5,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret
