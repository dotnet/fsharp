type Aw(value: int) =
    member _.GetResult() = value

type Code<'T> = delegate of byref<'T> -> unit

let inline bindDynamic<'TResult1, 'TOverall, 'Awaiter
    when 'Awaiter: (member GetResult: unit -> 'TResult1)>
    (d: byref<'TOverall>, awaiter: 'Awaiter, continuation: 'TResult1 -> Code<'TOverall>) =
    (continuation (awaiter.GetResult())).Invoke(&d)

let inline wrap<'T1, 'T2, 'Awaiter1, 'Awaiter2
    when 'Awaiter1: (member GetResult: unit -> 'T1)
    and 'Awaiter2: (member GetResult: unit -> 'T2)>
    (left: 'Awaiter1) (right: 'Awaiter2) : Code<'T1 * 'T2> =
    Code<'T1 * 'T2>(fun d ->
        bindDynamic (&d, left, fun leftR ->
            Code<'T1 * 'T2>(fun d2 ->
                bindDynamic (&d2, right, fun rightR ->
                    Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR))))))

[<EntryPoint>]
let main _ =
    let code = wrap (Aw(1)) (Aw(2))
    let mutable result = (0, 0)
    code.Invoke(&result)
    printfn "Result = %A" result
    0
--------------------------------------------------------------------------------

Test::bindDynamic
  (10,5-10,52)  (continuation (awaiter.GetResult())).Invoke(&d)
    IL_0000:  ldarg.2
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  brfalse.s IL_000e
    IL_0006:  ldnull
    IL_0007:  unbox.any 0x1b000004
    IL_000c:  br.s IL_0019
    IL_000e:  ldstr "Dynamic invocation of GetResult is not supported"
    IL_0013:  newobj NotSupportedException::.ctor
    IL_0018:  throw
    IL_0019:  callvirt Invoke
    IL_001e:  ldarg.0
    IL_001f:  callvirt Invoke
    IL_0024:  nop
    IL_0025:  ret

Test::bindDynamic$W
  (10,5-10,52)  (continuation (awaiter.GetResult())).Invoke(&d)
    IL_0000:  ldarg.3
    IL_0001:  ldarg.2
    IL_0002:  stloc.0
    IL_0003:  ldarg.0
    IL_0004:  ldloc.0
    IL_0005:  callvirt Invoke
    IL_000a:  callvirt Invoke
    IL_000f:  ldarg.1
    IL_0010:  callvirt Invoke
    IL_0015:  nop
    IL_0016:  ret

Test::wrap
  (16,20-20,73)  (fun d -> bindDynamic (&d, left, fun leftR -> Code<'T1 * 'T2>(fun d2 -> bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR))))))
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj .ctor
    IL_0007:  ldftn Invoke
    IL_000d:  newobj .ctor
    IL_0012:  ret

Test::wrap$W
  (16,20-20,73)  (fun d -> bindDynamic (&d, left, fun leftR -> Code<'T1 * 'T2>(fun d2 -> bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR))))))
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  newobj .ctor
    IL_0009:  ldftn Invoke
    IL_000f:  newobj .ctor
    IL_0014:  ret

Test::main
  (24,5-24,36)  let code = wrap (Aw(1)) (Aw(2))
    IL_0000:  ldc.i4.1
    IL_0001:  newobj Aw::.ctor
    IL_0006:  ldc.i4.2
    IL_0007:  newobj Aw::.ctor
    IL_000c:  call Test::<wrap>__debug@24
    IL_0011:  stloc.0

  (25,5-25,32)  let mutable result = (0, 0)
    IL_0012:  ldc.i4.0
    IL_0013:  ldc.i4.0
    IL_0014:  newobj .ctor
    IL_0019:  stloc.1

  (26,5-26,25)  code.Invoke(&result)
    IL_001a:  ldloc.0
    IL_001b:  ldloca.s 1
    IL_001d:  callvirt Invoke
    IL_0022:  nop

  (27,5-27,33)  printfn "Result = %A" result
    IL_0023:  ldstr "Result = %A"
    IL_0028:  newobj .ctor
    IL_002d:  call ExtraTopLevelOperators::PrintFormatLine
    IL_0032:  stloc.2
    IL_0033:  ldloc.1
    IL_0034:  stloc.3
    IL_0035:  ldloc.3
    IL_0036:  call get_Item1
    IL_003b:  stloc.s 4
    IL_003d:  ldloc.3
    IL_003e:  call get_Item2
    IL_0043:  stloc.s 5
    IL_0045:  ldloc.2
    IL_0046:  ldloc.s 4
    IL_0048:  ldloc.s 5
    IL_004a:  newobj .ctor
    IL_004f:  callvirt Invoke
    IL_0054:  pop

  (28,5-28,6)  0
    IL_0055:  ldc.i4.0
    IL_0056:  ret

Test::<bindDynamic>__debug@17
  (10,5-10,52)  (continuation (awaiter.GetResult())).Invoke(&d)
    IL_0000:  ldarg.2
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  callvirt Aw::GetResult
    IL_0009:  callvirt Invoke
    IL_000e:  ldarg.0
    IL_000f:  callvirt Invoke
    IL_0014:  nop
    IL_0015:  ret

Test::<bindDynamic>__debug@19-1
  (10,5-10,52)  (continuation (awaiter.GetResult())).Invoke(&d)
    IL_0000:  ldarg.2
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  callvirt Aw::GetResult
    IL_0009:  callvirt Invoke
    IL_000e:  ldarg.0
    IL_000f:  callvirt Invoke
    IL_0014:  nop
    IL_0015:  ret

Test::<wrap>__debug@24
  (16,20-20,73)  (fun d -> bindDynamic (&d, left, fun leftR -> Code<'T1 * 'T2>(fun d2 -> bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR))))))
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj code@16::.ctor
    IL_0007:  ldftn code@16::Invoke
    IL_000d:  newobj .ctor
    IL_0012:  ret

Aw::.ctor
  (2,6-2,8)  Aw
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  stfld Aw::value
    IL_000f:  ret

Aw::GetResult
  (3,28-3,33)  value
    IL_0000:  ldarg.0
    IL_0001:  ldfld Aw::value
    IL_0006:  ret

wrap@16::Invoke
  (17,9-20,72)  bindDynamic (&d, left, fun leftR -> Code<'T1 * 'T2>(fun d2 -> bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR)))))
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld left
    IL_0007:  ldarg.0
    IL_0008:  ldfld right
    IL_000d:  newobj .ctor
    IL_0012:  call Test::bindDynamic
    IL_0017:  nop
    IL_0018:  ret

wrap@16-5::Invoke
  (17,9-20,72)  bindDynamic (&d, left, fun leftR -> Code<'T1 * 'T2>(fun d2 -> bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR)))))
    IL_0000:  ldarg.0
    IL_0001:  ldfld getResult
    IL_0006:  ldarg.1
    IL_0007:  ldarg.0
    IL_0008:  ldfld left
    IL_000d:  ldarg.0
    IL_000e:  ldfld getResult
    IL_0013:  ldarg.0
    IL_0014:  ldfld getResult0
    IL_0019:  ldarg.0
    IL_001a:  ldfld right
    IL_001f:  newobj .ctor
    IL_0024:  call Test::bindDynamic$W
    IL_0029:  nop
    IL_002a:  ret

code@16::Invoke
  (17,9-20,72)  bindDynamic (&d, left, fun leftR -> Code<'T1 * 'T2>(fun d2 -> bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR)))))
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld code@16::left
    IL_0007:  ldarg.0
    IL_0008:  ldfld code@16::right
    IL_000d:  newobj code@17-1::.ctor
    IL_0012:  call Test::<bindDynamic>__debug@17
    IL_0017:  nop
    IL_0018:  ret

wrap@17-1::Invoke
  (18,28-20,71)  (fun d2 -> bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR))))
    IL_0000:  ldarg.0
    IL_0001:  ldfld right
    IL_0006:  ldarg.1
    IL_0007:  newobj .ctor
    IL_000c:  ldftn Invoke
    IL_0012:  newobj .ctor
    IL_0017:  ret

wrap@17-6::Invoke
  (18,28-20,71)  (fun d2 -> bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR))))
    IL_0000:  ldarg.0
    IL_0001:  ldfld getResult
    IL_0006:  ldarg.0
    IL_0007:  ldfld getResult0
    IL_000c:  ldarg.0
    IL_000d:  ldfld right
    IL_0012:  ldarg.1
    IL_0013:  newobj .ctor
    IL_0018:  ldftn Invoke
    IL_001e:  newobj .ctor
    IL_0023:  ret

code@17-1::Invoke
  (18,28-20,71)  (fun d2 -> bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR))))
    IL_0000:  ldarg.0
    IL_0001:  ldfld code@17-1::right
    IL_0006:  ldarg.1
    IL_0007:  newobj code@18-2::.ctor
    IL_000c:  ldftn code@18-2::Invoke
    IL_0012:  newobj .ctor
    IL_0017:  ret

wrap@18-2::Invoke
  (19,17-20,70)  bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR)))
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld right
    IL_0007:  ldarg.0
    IL_0008:  ldfld leftR
    IL_000d:  newobj .ctor
    IL_0012:  call Test::bindDynamic
    IL_0017:  nop
    IL_0018:  ret

wrap@18-7::Invoke
  (19,17-20,70)  bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR)))
    IL_0000:  ldarg.0
    IL_0001:  ldfld getResult0
    IL_0006:  ldarg.1
    IL_0007:  ldarg.0
    IL_0008:  ldfld right
    IL_000d:  ldarg.0
    IL_000e:  ldfld getResult
    IL_0013:  ldarg.0
    IL_0014:  ldfld getResult0
    IL_0019:  ldarg.0
    IL_001a:  ldfld leftR
    IL_001f:  newobj .ctor
    IL_0024:  call Test::bindDynamic$W
    IL_0029:  nop
    IL_002a:  ret

code@18-2::Invoke
  (19,17-20,70)  bindDynamic (&d2, right, fun rightR -> Code<'T1 * 'T2>(fun d3 -> d3 <- (leftR, rightR)))
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld code@18-2::right
    IL_0007:  ldarg.0
    IL_0008:  ldfld code@18-2::leftR
    IL_000d:  newobj code@19-3::.ctor
    IL_0012:  call Test::<bindDynamic>__debug@19-1
    IL_0017:  nop
    IL_0018:  ret

wrap@19-3::Invoke
  (20,36-20,69)  (fun d3 -> d3 <- (leftR, rightR))
    IL_0000:  ldarg.0
    IL_0001:  ldfld leftR
    IL_0006:  ldarg.1
    IL_0007:  newobj .ctor
    IL_000c:  ldftn Invoke
    IL_0012:  newobj .ctor
    IL_0017:  ret

wrap@19-8::Invoke
  (20,36-20,69)  (fun d3 -> d3 <- (leftR, rightR))
    IL_0000:  ldarg.0
    IL_0001:  ldfld getResult
    IL_0006:  ldarg.0
    IL_0007:  ldfld getResult0
    IL_000c:  ldarg.0
    IL_000d:  ldfld leftR
    IL_0012:  ldarg.1
    IL_0013:  newobj .ctor
    IL_0018:  ldftn Invoke
    IL_001e:  newobj .ctor
    IL_0023:  ret

code@19-3::Invoke
  (20,36-20,69)  (fun d3 -> d3 <- (leftR, rightR))
    IL_0000:  ldarg.0
    IL_0001:  ldfld code@19-3::leftR
    IL_0006:  ldarg.1
    IL_0007:  newobj code@20-4::.ctor
    IL_000c:  ldftn code@20-4::Invoke
    IL_0012:  newobj .ctor
    IL_0017:  ret

wrap@20-4::Invoke
  (20,47-20,68)  d3 <- (leftR, rightR)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld leftR
    IL_0007:  ldarg.0
    IL_0008:  ldfld rightR
    IL_000d:  newobj .ctor
    IL_0012:  stobj 0x1b00001a
    IL_0017:  ret

wrap@20-9::Invoke
  (20,47-20,68)  d3 <- (leftR, rightR)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld leftR
    IL_0007:  ldarg.0
    IL_0008:  ldfld rightR
    IL_000d:  newobj .ctor
    IL_0012:  stobj 0x1b00001a
    IL_0017:  ret

code@20-4::Invoke
  (20,47-20,68)  d3 <- (leftR, rightR)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld code@20-4::leftR
    IL_0007:  ldarg.0
    IL_0008:  ldfld code@20-4::rightR
    IL_000d:  newobj .ctor
    IL_0012:  stobj 0x1b00000b
    IL_0017:  ret
