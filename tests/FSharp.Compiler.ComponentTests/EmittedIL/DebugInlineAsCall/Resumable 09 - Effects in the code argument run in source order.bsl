#nowarn "3513"
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let log = ResizeArray<string>()

[<MethodImpl(MethodImplOptions.NoInlining)>]
let note (s: string) (v: 'a) = log.Add s; v

[<MethodImpl(MethodImplOptions.NoInlining)>]
let start<'M when 'M :> IAsyncStateMachine> (sm: byref<'M>) = sm.MoveNext()

[<Struct; NoEquality; NoComparison>]
type D<'T> = { mutable Result: 'T }

type B() =
    member inline _.Delay([<InlineIfLambda>] f: unit -> ResumableCode<'D, 'T>) =
        ResumableCode<'D, 'T>(fun sm -> (f ()).Invoke(&sm))

    member inline _.Return(v: 'T) =
        ResumableCode<D<'T>, 'T>(fun sm -> note "body" (); sm.Data.Result <- v; true)

    member inline _.Run(code: ResumableCode<D<'T>, 'T>, [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line: int) : 'T =
        if __useResumableCode then
            __stateMachine<D<'T>, 'T>
                (MoveNextMethodImpl<D<'T>>(fun sm -> __resumeAt sm.ResumptionPoint; code.Invoke(&sm) |> ignore))
                (SetStateMachineMethodImpl<D<'T>>(fun _ _ -> ()))
                (AfterCode<D<'T>, 'T>(fun sm -> start &sm; ignore line; sm.Data.Result))
        else
            let mutable sm = ResumableStateMachine<D<'T>>()
            code.Invoke(&sm) |> ignore
            ignore line
            sm.Data.Result

let b = B()

let inline mkCode () =
    note "make code" ()
    b.Delay(fun () -> b.Return 3)

[<EntryPoint>]
let main _ =
    let r = b.Run(mkCode (), note "line arg" 7)
    if r = 3 && String.concat ", " log = "make code, line arg, body" then 0 else 1
--------------------------------------------------------------------------------

Test::note
  (11,32-11,41)  log.Add s
    IL_0000:  call Test::get_log
    IL_0005:  ldarg.0
    IL_0006:  callvirt Add

  (11,43-11,44)  v
    IL_000b:  ldarg.1
    IL_000c:  ret

Test::start
  (14,63-14,76)  sm.MoveNext()
    IL_0000:  ldarg.0
    IL_0001:  constrained. 0x1b000004
    IL_0007:  callvirt IAsyncStateMachine::MoveNext
    IL_000c:  ret

Test::mkCode
  (41,5-41,24)  note "make code" ()
    IL_0000:  ldstr "make code"
    IL_0005:  ldnull
    IL_0006:  call Test::note
    IL_000b:  pop

  (42,5-42,34)  b.Delay(fun () -> b.Return 3)
    IL_000c:  call Test::get_b
    IL_0011:  stloc.0
    IL_0012:  ldsfld mkCode@42::@_instance
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  newobj mkCode@42-2::.ctor
    IL_001e:  ldftn mkCode@42-2::Invoke
    IL_0024:  newobj .ctor
    IL_0029:  ret

Test::main
  (46,5-46,48)  let r = b.Run(mkCode (), note "line arg" 7)
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Test::init@
    IL_0006:  ldsfld $Test::init@
    IL_000b:  pop
    IL_000c:  ldstr "make code"
    IL_0011:  ldnull
    IL_0012:  call Test::note
    IL_0017:  pop
    IL_0018:  ldstr "line arg"
    IL_001d:  ldc.i4.7
    IL_001e:  call Test::note
    IL_0023:  stloc.1
    IL_0024:  ldloca.s 2
    IL_0026:  initobj r@46
    IL_002c:  ldloca.s 2
    IL_002e:  stloc.3
    IL_002f:  ldloc.3
    IL_0030:  call Test::start
    IL_0035:  nop
    IL_0036:  ldloc.3
    IL_0037:  ldflda r@46::Data
    IL_003c:  ldfld Result@
    IL_0041:  stloc.0

  (47,5-47,74)  if r = 3 && String.concat ", " log = "make code, line arg, body" then
    IL_0042:  nop

  (47,8-47,13)  r = 3
    IL_0043:  ldloc.0
    IL_0044:  ldc.i4.3
    IL_0045:  bne.un.s IL_0063

  (47,17-47,69)  String.concat ", " log = "make code, line arg, body"
    IL_0047:  ldstr ", "
    IL_004c:  call Test::get_log
    IL_0051:  call StringModule::Concat
    IL_0056:  ldstr "make code, line arg, body"
    IL_005b:  call String::Equals

  <hidden>
    IL_0060:  nop
    IL_0061:  br.s IL_0065

  <hidden>
    IL_0063:  ldc.i4.0

  <hidden>
    IL_0064:  nop
    IL_0065:  brfalse.s IL_0069

  (47,75-47,76)  0
    IL_0067:  ldc.i4.0
    IL_0068:  ret

  (47,82-47,83)  1
    IL_0069:  ldc.i4.1
    IL_006a:  ret

Test::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Test::init@
    IL_0006:  ldsfld $Test::init@
    IL_000b:  pop
    IL_000c:  ret

Test::staticInitialization@
  (8,1-8,32)  let log = ResizeArray<string>()
    IL_0000:  newobj .ctor
    IL_0005:  stsfld Test::log@8

  (38,1-38,12)  let b = B()
    IL_000a:  newobj B::.ctor
    IL_000f:  stsfld Test::b@38
    IL_0014:  ret

B::.ctor
  (19,6-19,7)  B
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

B::Delay
  (21,30-21,60)  (fun sm -> (f ()).Invoke(&sm))
    IL_0000:  ldarg.1
    IL_0001:  newobj .ctor
    IL_0006:  ldftn Invoke
    IL_000c:  newobj .ctor
    IL_0011:  ret

B::Return
  (24,33-24,86)  (fun sm -> note "body" (); sm.Data.Result <- v; true)
    IL_0000:  ldarg.1
    IL_0001:  newobj .ctor
    IL_0006:  ldftn Invoke
    IL_000c:  newobj .ctor
    IL_0011:  ret

B::Run
  (33,13-33,60)  let mutable sm = ResumableStateMachine<D<'T>>()
    IL_0000:  ldloca.s 1
    IL_0002:  initobj 0x1b00000e
    IL_0008:  ldloc.1
    IL_0009:  stloc.0

  (34,13-34,29)  code.Invoke(&sm)
    IL_000a:  ldarg.1
    IL_000b:  ldloca.s 0
    IL_000d:  callvirt Invoke
    IL_0012:  stloc.2

  (34,33-34,39)  ignore
    IL_0013:  ldloc.2
    IL_0014:  stloc.3

  (35,13-35,24)  ignore line
    IL_0015:  ldarg.2
    IL_0016:  stloc.s 4

  (36,13-36,27)  sm.Data.Result
    IL_0018:  ldloca.s 0
    IL_001a:  ldflda Data
    IL_001f:  ldfld Result@
    IL_0024:  ret

Delay@21::Invoke
  (21,41-21,59)  (f ()).Invoke(&sm)
    IL_0000:  ldarg.0
    IL_0001:  ldfld f
    IL_0006:  ldnull
    IL_0007:  callvirt Invoke
    IL_000c:  ldarg.1
    IL_000d:  callvirt Invoke
    IL_0012:  ret

Return@24::Invoke
  (24,44-24,58)  note "body" ()
    IL_0000:  ldstr "body"
    IL_0005:  ldnull
    IL_0006:  call Test::note
    IL_000b:  pop

  (24,60-24,79)  sm.Data.Result <- v
    IL_000c:  ldarg.1
    IL_000d:  ldflda Data
    IL_0012:  ldarg.0
    IL_0013:  ldfld v
    IL_0018:  call set_Result

  (24,81-24,85)  true
    IL_001d:  ldc.i4.1
    IL_001e:  ret

mkCode@42::Invoke
  (42,23-42,33)  b.Return 3
    IL_0000:  call Test::get_b
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.3
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  newobj mkCode@42-1::.ctor
    IL_000e:  ldftn mkCode@42-1::Invoke
    IL_0014:  newobj .ctor
    IL_0019:  ret
