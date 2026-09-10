open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<Struct>]
type S<'T> = member _.M(_: 'T) = ()

let inline g (_: S<'T>) =
    if __useResumableCode then
        __stateMachine<S<'T>, int>
            (MoveNextMethodImpl<_>(fun _ -> ()))
            (SetStateMachineMethodImpl<_>(fun _ _ -> ()))
            (AfterCode<_, _>(fun _ -> 42))
    else 42

[<EntryPoint>]
let main _ =
    let r = g (S<int>())
    if r = 42 then 0 else 1
--------------------------------------------------------------------------------

Test::g
  (15,10-15,12)  42
    IL_0000:  ldc.i4.s 42
    IL_0002:  ret

Test::main
  (19,5-19,25)  let r = g (S<int>())
    IL_0000:  ldloc.1
    IL_0001:  call Test::g
    IL_0006:  stloc.0

  (20,5-20,19)  if r = 42 then
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s 42
    IL_000a:  bne.un.s IL_000e

  (20,20-20,21)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (20,27-20,28)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret

S`1::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst 0x1b000003
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s IL_001d

  <hidden>
    IL_000d:  ldarg.1
    IL_000e:  unbox.any 0x1b000003
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
    IL_0003:  isinst 0x1b000003
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s IL_001c

  <hidden>
    IL_000d:  ldarg.1
    IL_000e:  unbox.any 0x1b000003
    IL_0013:  stloc.1
    IL_0014:  ldarg.0
    IL_0015:  ldloc.1
    IL_0016:  call Equals
    IL_001b:  ret

  <hidden>
    IL_001c:  ldc.i4.0
    IL_001d:  ret
