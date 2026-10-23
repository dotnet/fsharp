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

    member inline _.Run(code: ResumableCode<D<'T>, 'T>) : 'T =
        if __useResumableCode then
            __stateMachine<D<'T>, 'T>
                (MoveNextMethodImpl<D<'T>>(fun sm -> __resumeAt sm.ResumptionPoint; code.Invoke(&sm) |> ignore; code.Invoke(&sm) |> ignore))
                (SetStateMachineMethodImpl<D<'T>>(fun _ _ -> ()))
                (AfterCode<D<'T>, 'T>(fun sm -> sm.Data.Result))
        else
            let mutable sm = ResumableStateMachine<D<'T>>()
            code.Invoke(&sm) |> ignore
            sm.Data.Result

[<EntryPoint>]
let main _ =
    let b = B()
    let result = b { return [ 3 ] |> List.sum }
    ignore result
    0
--------------------------------------------------------------------------------

Test::main
  (31,5-31,16)  let b = B()
    IL_0000:  newobj B::.ctor
    IL_0005:  stloc.0

  (32,18-32,19)  b
    IL_0006:  ldloca.s 2
    IL_0008:  initobj result@32
    IL_000e:  ldloca.s 2
    IL_0010:  stloc.3
    IL_0011:  ldloc.3
    IL_0012:  ldloc.0
    IL_0013:  stfld result@32::b
    IL_0018:  ldloc.3
    IL_0019:  ldflda result@32::Data
    IL_001e:  ldfld Result@
    IL_0023:  stloc.1

  (33,5-33,18)  ignore result
    IL_0024:  ldloc.1
    IL_0025:  stloc.s 4

  (34,5-34,6)  0
    IL_0027:  ldc.i4.0
    IL_0028:  ret

Test::<sum>__debug@32
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

Test::<sum>__debug@32-1
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
  (25,13-25,60)  let mutable sm = ResumableStateMachine<D<'T>>()
    IL_0000:  ldloca.s 1
    IL_0002:  initobj 0x1b00000b
    IL_0008:  ldloc.1
    IL_0009:  stloc.0

  (26,13-26,29)  code.Invoke(&sm)
    IL_000a:  ldarg.1
    IL_000b:  ldloca.s 0
    IL_000d:  callvirt Invoke
    IL_0012:  stloc.2

  (26,33-26,39)  ignore
    IL_0013:  ldloc.2
    IL_0014:  stloc.3

  (27,13-27,27)  sm.Data.Result
    IL_0015:  ldloca.s 0
    IL_0017:  ldflda Data
    IL_001c:  ldfld Result@
    IL_0021:  ret

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

result@32::MoveNext
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld result@32::ResumptionPoint
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldfld result@32::b
    IL_000d:  stloc.2
    IL_000e:  ldarg.0
    IL_000f:  stloc.3

  (32,29-32,46)  [ 3 ] |> List.sum
    IL_0010:  ldarg.0
    IL_0011:  ldfld result@32::b
    IL_0016:  stloc.s 4

  (32,29-32,34)  [ 3 ]
    IL_0018:  ldc.i4.3
    IL_0019:  call get_Empty
    IL_001e:  call Cons
    IL_0023:  stloc.s 6

  (32,38-32,46)  List.sum
    IL_0025:  ldloc.s 6
    IL_0027:  call Test::<sum>__debug@32
    IL_002c:  stloc.s 5
    IL_002e:  ldloc.3
    IL_002f:  stloc.s 7
    IL_0031:  ldloc.s 7
    IL_0033:  ldflda result@32::Data
    IL_0038:  ldloc.s 5
    IL_003a:  call set_Result
    IL_003f:  ldc.i4.1
    IL_0040:  stloc.1
    IL_0041:  ldloc.1
    IL_0042:  stloc.s 8
    IL_0044:  ldarg.0
    IL_0045:  ldfld result@32::b
    IL_004a:  stloc.s 10
    IL_004c:  ldarg.0
    IL_004d:  stloc.s 11

  (32,29-32,46)  [ 3 ] |> List.sum
    IL_004f:  ldarg.0
    IL_0050:  ldfld result@32::b
    IL_0055:  stloc.s 12

  (32,29-32,34)  [ 3 ]
    IL_0057:  ldc.i4.3
    IL_0058:  call get_Empty
    IL_005d:  call Cons
    IL_0062:  stloc.s 14

  (32,38-32,46)  List.sum
    IL_0064:  ldloc.s 14
    IL_0066:  call Test::<sum>__debug@32-1
    IL_006b:  stloc.s 13
    IL_006d:  ldloc.s 11
    IL_006f:  stloc.s 15
    IL_0071:  ldloc.s 15
    IL_0073:  ldflda result@32::Data
    IL_0078:  ldloc.s 13
    IL_007a:  call set_Result
    IL_007f:  ldc.i4.1
    IL_0080:  stloc.s 9
    IL_0082:  ldloc.s 9
    IL_0084:  stloc.s 16
    IL_0086:  ret
