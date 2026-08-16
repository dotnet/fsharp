module Module

let f (x: int) =
    x + x

let g () =
    let i =
        match f 5 with
        | 10 -> 0
        | _ -> 1
    i + 1
--------------------------------------------------------------------------------

Module::f
  (4,5-4,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Module::g
  (8,9-8,23)  match f 5 with
    IL_0000:  ldc.i4.5
    IL_0001:  call Module::f
    IL_0006:  ldc.i4.s 10
    IL_0008:  sub
    IL_0009:  switch (1 targets)
    IL_0012:  br.s IL_0018

  (9,17-9,18)  0
    IL_0014:  ldc.i4.0

  <hidden>
    IL_0015:  nop
    IL_0016:  br.s IL_001a

  (10,16-10,17)  1
    IL_0018:  ldc.i4.1

  <hidden>
    IL_0019:  nop
    IL_001a:  stloc.0

  (11,5-11,10)  i + 1
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.1
    IL_001d:  add
    IL_001e:  ret
