let f () =
    let mutable x = 10
    let inline g y = x <- x + int y
    g 5uy
    x

[<EntryPoint>]
let main _ =
    if f () = 15 then 0 else 1
--------------------------------------------------------------------------------

Test::f
  (3,5-3,23)  let mutable x = 10
    IL_0000:  ldc.i4.s 10
    IL_0002:  newobj .ctor
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  newobj g@4::.ctor
    IL_000e:  stloc.1

  (5,5-5,10)  g 5uy
    IL_000f:  ldc.i4.5
    IL_0010:  stloc.2

  (4,22-4,36)  x <- x + int y
    IL_0011:  ldloc.0
    IL_0012:  ldloc.0
    IL_0013:  call get_contents
    IL_0018:  ldloc.2
    IL_0019:  conv.i4
    IL_001a:  add
    IL_001b:  call set_contents

  (6,5-6,6)  x
    IL_0020:  ldloc.0
    IL_0021:  call get_contents
    IL_0026:  ret

Test::main
  (10,5-10,22)  if f () = 15 then
    IL_0000:  call Test::f
    IL_0005:  ldc.i4.s 15
    IL_0007:  bne.un.s IL_000b

  (10,23-10,24)  0
    IL_0009:  ldc.i4.0
    IL_000a:  ret

  (10,30-10,31)  1
    IL_000b:  ldc.i4.1
    IL_000c:  ret

g@4-1::Invoke
  (4,22-4,36)  x <- x + int y
    IL_0000:  ldarg.0
    IL_0001:  ldfld x
    IL_0006:  ldarg.0
    IL_0007:  ldfld x
    IL_000c:  call get_contents
    IL_0011:  ldarg.1
    IL_0012:  stloc.0
    IL_0013:  ldloc.0
    IL_0014:  call LanguagePrimitives::ExplicitDynamic
    IL_0019:  add
    IL_001a:  call set_contents
    IL_001f:  ldnull
    IL_0020:  ret
