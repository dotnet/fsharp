let f () =
    let xs = [ 1; 2; 3 ]
    let inline g y = xs |> List.map (fun v -> v + int y) |> List.sum
    g 1uy

[<EntryPoint>]
let main _ =
    if f () = 9 then 0 else 1
--------------------------------------------------------------------------------

Test::f
  (3,5-3,25)  let xs = [ 1; 2; 3 ]
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  call get_Empty
    IL_0008:  call Cons
    IL_000d:  call Cons
    IL_0012:  call Cons
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  newobj g@4::.ctor
    IL_001e:  stloc.1

  (5,5-5,10)  g 1uy
    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.1
    IL_0021:  tail.
    IL_0023:  call Test::<g>__debug@5
    IL_0028:  ret

Test::main
  (9,5-9,21)  if f () = 9 then
    IL_0000:  call Test::f
    IL_0005:  ldc.i4.s 9
    IL_0007:  bne.un.s IL_000b

  (9,22-9,23)  0
    IL_0009:  ldc.i4.0
    IL_000a:  ret

  (9,29-9,30)  1
    IL_000b:  ldc.i4.1
    IL_000c:  ret

Test::<sum>__debug@4
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

Test::<sum>__debug@4-1
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

Test::<g>__debug@5
  (4,22-4,24)  xs
    IL_0000:  ldarg.0
    IL_0001:  stloc.0

  (4,28-4,57)  List.map (fun v -> v + int y)
    IL_0002:  ldarg.1
    IL_0003:  newobj Pipe #1 stage #1 at line 4@4-1::.ctor
    IL_0008:  ldloc.0
    IL_0009:  call ListModule::Map
    IL_000e:  stloc.1

  (4,61-4,69)  List.sum
    IL_000f:  ldloc.1
    IL_0010:  call Test::<sum>__debug@4-1
    IL_0015:  ret

g@4-1::Invoke
  (4,22-4,24)  xs
    IL_0000:  ldarg.0
    IL_0001:  ldfld xs
    IL_0006:  stloc.0

  (4,28-4,57)  List.map (fun v -> v + int y)
    IL_0007:  ldarg.1
    IL_0008:  newobj .ctor
    IL_000d:  ldloc.0
    IL_000e:  call ListModule::Map
    IL_0013:  stloc.1

  (4,61-4,69)  List.sum
    IL_0014:  ldloc.1
    IL_0015:  tail.
    IL_0017:  call Test::<sum>__debug@4
    IL_001c:  ret

Pipe #1 stage #1 at line 4@4::Invoke
  (4,47-4,56)  v + int y
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld y
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  call LanguagePrimitives::ExplicitDynamic
    IL_000e:  add
    IL_000f:  ret

Pipe #1 stage #1 at line 4@4-1::Invoke
  (4,47-4,56)  v + int y
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld Pipe #1 stage #1 at line 4@4-1::y
    IL_0007:  conv.i4
    IL_0008:  add
    IL_0009:  ret
