open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

#nowarn "3501"
#nowarn "3513"

type Builder() =
    member inline _.Run(code: ResumableCode<unit, int>) =
        if __useResumableCode then
            __stateMachine<unit, int>
                (MoveNextMethodImpl<_>(fun sm -> code.Invoke(&sm) |> ignore))
                (SetStateMachineMethodImpl<_>(fun _ _ -> ()))
                (AfterCode<_, _>(fun _ -> 42))
        else
            0

let builder = Builder()

[<EntryPoint>]
let main _ =
    let code = ResumableCode<unit, int>(fun _ -> true)
    let result = builder.Run code
    if result = 42 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (22,5-22,55)  let code = ResumableCode<unit, int>(fun _ -> true)
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Test::init@
    IL_0006:  ldsfld $Test::init@
    IL_000b:  pop
    IL_000c:  ldnull
    IL_000d:  ldftn code@22::Invoke
    IL_0013:  newobj .ctor
    IL_0018:  stloc.0

  (23,5-23,34)  let result = builder.Run code
    IL_0019:  ldloca.s 2
    IL_001b:  initobj result@23
    IL_0021:  ldloca.s 2
    IL_0023:  stloc.3
    IL_0024:  ldc.i4.s 42
    IL_0026:  stloc.1

  (24,5-24,24)  if result = 42 then
    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.s 42
    IL_002a:  bne.un.s IL_002e

  (24,25-24,26)  0
    IL_002c:  ldc.i4.0
    IL_002d:  ret

  (24,32-24,33)  1
    IL_002e:  ldc.i4.1
    IL_002f:  ret

Test::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Test::init@
    IL_0006:  ldsfld $Test::init@
    IL_000b:  pop
    IL_000c:  ret

Test::staticInitialization@
  (18,1-18,24)  let builder = Builder()
    IL_0000:  newobj Builder::.ctor
    IL_0005:  stsfld Test::builder@18
    IL_000a:  ret

Builder::.ctor
  (8,6-8,13)  Builder
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

Builder::Run
  (16,13-16,14)  0
    IL_0000:  ldc.i4.0
    IL_0001:  ret

code@22::Invoke
  (22,50-22,54)  true
    IL_0000:  ldc.i4.1
    IL_0001:  ret

result@23::MoveNext
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  stloc.1

  (22,50-22,54)  true
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  stloc.2
    IL_0006:  ret
