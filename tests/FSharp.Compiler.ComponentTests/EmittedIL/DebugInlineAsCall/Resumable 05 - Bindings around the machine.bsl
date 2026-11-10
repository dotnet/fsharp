#nowarn "3513"
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

[<Struct; NoEquality; NoComparison>]
type D<'T> = { mutable Result: 'T }

type B() =
    member inline _.Delay([<InlineIfLambda>] f: unit -> ResumableCode<'D, 'T>) =
        ResumableCode<'D, 'T>(fun sm -> (f ()).Invoke(&sm))

    member inline _.Return(v: 'T) =
        ResumableCode<D<'T>, 'T>(fun sm -> sm.Data.Result <- v; true)

    member inline _.Run(code: ResumableCode<D<'T>, 'T>, [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line: int) : 'T =
        if __useResumableCode then
            let tag = 1
            __stateMachine<D<'T>, 'T>
                (MoveNextMethodImpl<D<'T>>(fun sm -> __resumeAt sm.ResumptionPoint; code.Invoke(&sm) |> ignore))
                (SetStateMachineMethodImpl<D<'T>>(fun _ _ -> ignore line; ignore tag))
                (AfterCode<D<'T>, 'T>(fun sm -> sm.Data.Result))
        else
            let mutable sm = ResumableStateMachine<D<'T>>()
            code.Invoke(&sm) |> ignore
            ignore line
            sm.Data.Result

[<EntryPoint>]
let main _ =
    let b = B()
    let result = b { return [ 3 ] |> List.sum }
    ignore result
    0
--------------------------------------------------------------------------------

Test::main
  (33,5-33,16)  let b = B()
    IL_0000:  newobj B::.ctor
    IL_0005:  stloc.0

  (34,18-34,19)  b
    IL_0006:  ldloca.s 2
    IL_0008:  initobj result@34
    IL_000e:  ldloca.s 2
    IL_0010:  stloc.3
    IL_0011:  ldloc.3
    IL_0012:  ldflda result@34::Data
    IL_0017:  ldfld Result@
    IL_001c:  stloc.1

  (35,5-35,18)  ignore result
    IL_001d:  ldloc.1
    IL_001e:  stloc.s 4

  (36,5-36,6)  0
    IL_0020:  ldc.i4.0
    IL_0021:  ret

Test::<sum>__debug@34
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  call get_TailOrNull
    IL_0006:  brtrue.s IL_000a

  <hidden>
    IL_0008:  ldc.i4.0
    IL_0009:  ret

  <hidden>
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.0
    IL_000c:  ldarg.0
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  call get_TailOrNull
    IL_0014:  stloc.2
    IL_0015:  br.s IL_002b
    IL_0017:  ldloc.1
    IL_0018:  call get_HeadOrDefault
    IL_001d:  stloc.3
    IL_001e:  ldloc.0
    IL_001f:  ldloc.3
    IL_0020:  add.ovf
    IL_0021:  stloc.0
    IL_0022:  ldloc.2
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  call get_TailOrNull
    IL_002a:  stloc.2
    IL_002b:  ldloc.2
    IL_002c:  brtrue.s IL_0017
    IL_002e:  ldloc.0
    IL_002f:  ret

B::.ctor
  (11,6-11,7)  B
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

B::Delay
  (13,30-13,60)  (fun sm -> (f ()).Invoke(&sm))
    IL_0000:  ldarg.1
    IL_0001:  newobj .ctor
    IL_0006:  ldftn Invoke
    IL_000c:  newobj .ctor
    IL_0011:  ret

B::Return
  (16,33-16,70)  (fun sm -> sm.Data.Result <- v; true)
    IL_0000:  ldarg.1
    IL_0001:  newobj .ctor
    IL_0006:  ldftn Invoke
    IL_000c:  newobj .ctor
    IL_0011:  ret

B::Run
  (26,13-26,60)  let mutable sm = ResumableStateMachine<D<'T>>()
    IL_0000:  ldloca.s 1
    IL_0002:  initobj 0x1b00000b
    IL_0008:  ldloc.1
    IL_0009:  stloc.0

  (27,13-27,29)  code.Invoke(&sm)
    IL_000a:  ldarg.1
    IL_000b:  ldloca.s 0
    IL_000d:  callvirt Invoke
    IL_0012:  stloc.2

  (27,33-27,39)  ignore
    IL_0013:  ldloc.2
    IL_0014:  stloc.3

  (28,13-28,24)  ignore line
    IL_0015:  ldarg.2
    IL_0016:  stloc.s 4

  (29,13-29,27)  sm.Data.Result
    IL_0018:  ldloca.s 0
    IL_001a:  ldflda Data
    IL_001f:  ldfld Result@
    IL_0024:  ret

Delay@13::Invoke
  (13,41-13,59)  (f ()).Invoke(&sm)
    IL_0000:  ldarg.0
    IL_0001:  ldfld f
    IL_0006:  ldnull
    IL_0007:  callvirt Invoke
    IL_000c:  ldarg.1
    IL_000d:  callvirt Invoke
    IL_0012:  ret

Return@16::Invoke
  (16,44-16,63)  sm.Data.Result <- v
    IL_0000:  ldarg.1
    IL_0001:  ldflda Data
    IL_0006:  ldarg.0
    IL_0007:  ldfld v
    IL_000c:  call set_Result

  (16,65-16,69)  true
    IL_0011:  ldc.i4.1
    IL_0012:  ret

result@34::MoveNext
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld result@34::ResumptionPoint
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  stloc.2

  (34,29-34,46)  [ 3 ] |> List.sum
    IL_0009:  nop

  (34,29-34,34)  [ 3 ]
    IL_000a:  ldc.i4.3
    IL_000b:  call get_Empty
    IL_0010:  call Cons
    IL_0015:  stloc.s 4

  (34,38-34,46)  List.sum
    IL_0017:  ldloc.s 4
    IL_0019:  call Test::<sum>__debug@34
    IL_001e:  stloc.3
    IL_001f:  ldloc.2
    IL_0020:  stloc.s 5
    IL_0022:  ldloc.s 5
    IL_0024:  ldflda result@34::Data
    IL_0029:  ldloc.3
    IL_002a:  call set_Result
    IL_002f:  ldc.i4.1
    IL_0030:  stloc.1
    IL_0031:  ret
