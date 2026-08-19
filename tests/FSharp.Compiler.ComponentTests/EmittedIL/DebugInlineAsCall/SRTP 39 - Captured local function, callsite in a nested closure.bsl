let f () =
    let tee g (x: int) = g x; x
    let addName (key: string) (v: string) = tee (fun x -> printfn "%s=%s" key v)
    let inline addEnum (key: string) value = tee (fun x -> printfn "%s=%d" key (x + int value))
    let pipeline v = addName "a" "n" >> addEnum "b" v
    pipeline 1uy 41

[<EntryPoint>]
let main _ =
    if f () = 41 then 0 else 1
--------------------------------------------------------------------------------

Test::f
  <hidden>
    IL_0000:  ldsfld tee@3::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  newobj addName@4::.ctor
    IL_000c:  stloc.1
    IL_000d:  ldloc.0
    IL_000e:  newobj addEnum@5::.ctor
    IL_0013:  stloc.2
    IL_0014:  ldloc.0
    IL_0015:  ldloc.1
    IL_0016:  newobj pipeline@6::.ctor
    IL_001b:  stloc.3

  (7,5-7,20)  pipeline 1uy 41
    IL_001c:  ldloc.3
    IL_001d:  ldc.i4.1
    IL_001e:  ldc.i4.s 41
    IL_0020:  tail.
    IL_0022:  call InvokeFast
    IL_0027:  ret

Test::main
  (11,5-11,22)  if f () = 41 then
    IL_0000:  call Test::f
    IL_0005:  ldc.i4.s 41
    IL_0007:  bne.un.s IL_000b

  (11,23-11,24)  0
    IL_0009:  ldc.i4.0
    IL_000a:  ret

  (11,30-11,31)  1
    IL_000b:  ldc.i4.1
    IL_000c:  ret

Test::<addEnum>__debug@6
  (5,46-5,96)  tee (fun x -> printfn "%s=%d" key (x + int value))
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  newobj pipeline@5-1::.ctor
    IL_0008:  tail.
    IL_000a:  callvirt Invoke
    IL_000f:  ret

tee@3::Invoke
  (3,26-3,29)  g x
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  callvirt Invoke
    IL_0007:  pop

  (3,31-3,32)  x
    IL_0008:  ldarg.2
    IL_0009:  ret

addName@4::Invoke
  (4,45-4,81)  tee (fun x -> printfn "%s=%s" key v)
    IL_0000:  ldarg.0
    IL_0001:  ldfld addName@4::tee
    IL_0006:  ldarg.1
    IL_0007:  ldarg.2
    IL_0008:  newobj addName@4-1::.ctor
    IL_000d:  tail.
    IL_000f:  callvirt Invoke
    IL_0014:  ret

addName@4-1::Invoke
  (4,59-4,80)  printfn "%s=%s" key v
    IL_0000:  ldstr "%s=%s"
    IL_0005:  newobj .ctor
    IL_000a:  call ExtraTopLevelOperators::PrintFormatLine
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  newobj addName@4-2::.ctor
    IL_0016:  ldarg.0
    IL_0017:  ldfld addName@4-1::key
    IL_001c:  ldarg.0
    IL_001d:  ldfld addName@4-1::v
    IL_0022:  tail.
    IL_0024:  call InvokeFast
    IL_0029:  ret

addEnum@5-1::Invoke
  (5,46-5,96)  tee (fun x -> printfn "%s=%d" key (x + int value))
    IL_0000:  ldarg.0
    IL_0001:  ldfld tee
    IL_0006:  ldarg.1
    IL_0007:  ldarg.2
    IL_0008:  newobj .ctor
    IL_000d:  tail.
    IL_000f:  callvirt Invoke
    IL_0014:  ret

addEnum@5-2::Invoke
  (5,60-5,95)  printfn "%s=%d" key (x + int value)
    IL_0000:  ldstr "%s=%d"
    IL_0005:  newobj .ctor
    IL_000a:  call ExtraTopLevelOperators::PrintFormatLine
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  newobj addEnum@5-3::.ctor
    IL_0016:  ldarg.0
    IL_0017:  ldfld key
    IL_001c:  ldarg.1
    IL_001d:  ldarg.0
    IL_001e:  ldfld value
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  call LanguagePrimitives::ExplicitDynamic
    IL_002a:  add
    IL_002b:  tail.
    IL_002d:  call InvokeFast
    IL_0032:  ret

pipeline@5-1::Invoke
  (5,60-5,95)  printfn "%s=%d" key (x + int value)
    IL_0000:  ldstr "%s=%d"
    IL_0005:  newobj .ctor
    IL_000a:  call ExtraTopLevelOperators::PrintFormatLine
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  newobj pipeline@5-2::.ctor
    IL_0016:  ldarg.0
    IL_0017:  ldfld pipeline@5-1::key
    IL_001c:  ldarg.1
    IL_001d:  ldarg.0
    IL_001e:  ldfld pipeline@5-1::value
    IL_0023:  conv.i4
    IL_0024:  add
    IL_0025:  tail.
    IL_0027:  call InvokeFast
    IL_002c:  ret

pipeline@6::Invoke
  (6,22-6,54)  addName "a" "n" >> addEnum "b" v
    IL_0000:  ldarg.0
    IL_0001:  ldfld pipeline@6::addName
    IL_0006:  ldstr "a"
    IL_000b:  ldstr "n"
    IL_0010:  call InvokeFast
    IL_0015:  stloc.0
    IL_0016:  ldarg.0
    IL_0017:  ldfld pipeline@6::tee
    IL_001c:  ldstr "b"
    IL_0021:  ldarg.1
    IL_0022:  call Test::<addEnum>__debug@6
    IL_0027:  stloc.1
    IL_0028:  ldloc.0
    IL_0029:  ldloc.1
    IL_002a:  newobj pipeline@6-4::.ctor
    IL_002f:  ret
