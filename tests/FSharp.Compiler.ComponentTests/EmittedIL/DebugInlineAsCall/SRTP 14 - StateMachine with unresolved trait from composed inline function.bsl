open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<Struct>]
type S<'T> = member _.M(_: 'T) = ()

let inline f<'A, 'R, 'B when 'A: (member M: int -> 'R) and 'B: (member M: 'R -> unit)> (_a: 'A) : 'B = Unchecked.defaultof<_>

let inline g (_: S<'T>) =
    if __useResumableCode then
        __stateMachine<S<'T>, int>
            (MoveNextMethodImpl<_>(fun _ -> ()))
            (SetStateMachineMethodImpl<_>(fun _ _ -> ()))
            (AfterCode<_, _>(fun _ -> 0))
    else 0

let inline h a = g (f a)

[<EntryPoint>]
let main _ =
    let _ = h (S<int>())
    0
--------------------------------------------------------------------------------

Test::f
  (9,104-9,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret

Test::f$W
  (9,104-9,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret

Test::g
  (17,10-17,11)  0
    IL_0000:  ldc.i4.0
    IL_0001:  ret

Test::h
  (19,18-19,25)  g (f a)
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  callvirt Invoke
    IL_000d:  tail.
    IL_000f:  call Test::g
    IL_0014:  ret

Test::h$W
  (19,18-19,25)  g (f a)
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.1
    IL_0009:  callvirt Invoke
    IL_000e:  tail.
    IL_0010:  call Test::g
    IL_0015:  ret

Test::main
  (23,13-23,25)  h (S<int>())
    IL_0000:  ldloc.0
    IL_0001:  call Test::<h>__debug@23
    IL_0006:  pop

  (24,5-24,6)  0
    IL_0007:  ldc.i4.0
    IL_0008:  ret

Test::<h>__debug@23
  (19,18-19,25)  g (f a)
    IL_0000:  ldsfld main@9::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  callvirt Invoke
    IL_000d:  tail.
    IL_000f:  call Test::g
    IL_0014:  ret

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
  (7,34-7,36)  ()
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

h@9::Invoke
  (9,104-9,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret

h@9-1::Invoke
  (9,104-9,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret

main@9::Invoke
  (9,104-9,123)  Unchecked.defaultof
    IL_0000:  ldloc.0
    IL_0001:  ret
