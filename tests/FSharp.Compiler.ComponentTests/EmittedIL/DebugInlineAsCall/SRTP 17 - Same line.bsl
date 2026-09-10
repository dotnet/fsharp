open Module

let inline foo x y = add x y |> ignore; fun () -> ()

[<EntryPoint>]
let main _ =
    let _ = foo 1 2
    0
--------------------------------------------------------------------------------

Test::foo
  (4,22-4,29)  add x y
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  call InvokeFast
    IL_000e:  stloc.0

  (4,33-4,39)  ignore
    IL_000f:  ldloc.0
    IL_0010:  stloc.2
    IL_0011:  ldsfld foo@4::@_instance
    IL_0016:  ret

Test::foo$W
  (4,22-4,29)  add x y
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.1
    IL_0007:  ldloc.1
    IL_0008:  ldarg.1
    IL_0009:  ldarg.2
    IL_000a:  call InvokeFast
    IL_000f:  stloc.0

  (4,33-4,39)  ignore
    IL_0010:  ldloc.0
    IL_0011:  stloc.2
    IL_0012:  ldsfld foo@4-1::@_instance
    IL_0017:  ret

Test::main
  (8,13-8,20)  foo 1 2
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  call Test::<foo>__debug@8
    IL_0007:  pop

  (9,5-9,6)  0
    IL_0008:  ldc.i4.0
    IL_0009:  ret

Test::<foo>__debug@8
  (4,22-4,29)  add x y
    IL_0000:  ldsfld Pipe #1 input at line 4@1-2::@_instance
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  call InvokeFast
    IL_000e:  stloc.0

  (4,33-4,39)  ignore
    IL_000f:  ldloc.0
    IL_0010:  stloc.2
    IL_0011:  ldsfld main@4::@_instance
    IL_0016:  ret

foo@4::Invoke
  (4,51-4,53)  ()
    IL_0000:  ldnull
    IL_0001:  ret

foo@4-1::Invoke
  (4,51-4,53)  ()
    IL_0000:  ldnull
    IL_0001:  ret

main@4::Invoke
  (4,51-4,53)  ()
    IL_0000:  ldnull
    IL_0001:  ret
